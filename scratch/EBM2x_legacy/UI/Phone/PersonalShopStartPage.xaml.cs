using EBM2x.Database.Master;
using EBM2x.Datafile.env;
using EBM2x.Datafile.trlog;
using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Process.start;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Phone.Menu;
using EBM2x.UI.Phone.Open;
using EBM2x.UI.Phone.SignOn;
using EBM2x.UI.Tablet.Menu;
using EBM2x.UI.UiUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopStartPage : ContentPage
    {
        public PersonalShopStartPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();

            Toast.Show("Sw Version");
            UIManager.SwVersionDownloadProcessRunFlag = false;
            SwVersion swVersion = new SwVersion();
            swVersion.SwVersionDownloadProcess(this);
            swVersion.TaxpayerInfoDownloadProcess(this);
            AnimationLoop();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Start your daily work.");

            //=========================================================================================
            // Z-REPORT Data 생성
            ZreportMaster zreportMaster = new ZreportMaster();
            zreportMaster.UpdateZreportTable();

            //=========================================================================================
        }
        public async void SelectServerTime()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + "selectServerTime";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Timeout = UIManager.TIME_OUT;
                    httpWebRequest.Method = "POST"; //POST
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(responseStream, Encoding.UTF8);

                    string jsonResponse = readerPost.ReadToEnd().Trim();
                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse.Length == 14)
                    {
                        string NowDate = DateTime.Now.ToString("yyyyMMdd");
                        string ServerDate = jsonResponse.Substring(0, 8);
                        if (NowDate != ServerDate)
                        {
                            // 시간 변경
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the system time. [" + jsonResponse + "]", "Ok");
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public async void CheckWaitTransaction()
        {
            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays > 0)
            {
                DateTime dateTime = DateTime.Now;
                dateTime.AddDays(UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays * (-1));
                int count = RraSdcJsonWriter.GetTransactionMobileCount(dateTime.ToString("yyyyMMdd"));
                if (count>100)
                {
                    //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the number of failed transfers. [" + count + "]", "Ok");
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your Invoices was not reported to KRA because your Device is not connected to internet. Please reconnect it and wait for synchronization to complete.", "Ok");
                    Navigation.InsertPageBefore(new PersonalShopSendMenuPage(), this);
                    await Navigation.PopAsync();
                    return;
                }
            }

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineAmount > 0)
            {
                double offlineMobile = 500000;
                double amount = RraSdcJsonWriter.GetTransactionSalesReceipt();
                //UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineAmount
                if (amount > offlineMobile)
                {
                    //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the amount of failed transfers. [" + amount + "]", "Ok");
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your Invoices was not reported to KRA because your Device is not connected to internet. Please reconnect it and wait for synchronization to complete.", "Ok");
                    Navigation.InsertPageBefore(new PersonalShopSendMenuPage(), this);
                    await Navigation.PopAsync();
                }
            }
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }

        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(1.0));

            //if (!UIManager.Instance().IsWindows && UIManager.SwVersionDownloadProcessRunFlag)
            //{
            //    Toast.Show("Sw Version");
            //    UIManager.SwVersionDownloadProcessRunFlag = false;
            //    SwVersionDownloadProcess();
            //}

            Toast.Show("Get Server Time");
            SelectServerTime();

            Toast.Show("Check transaction");
            CheckWaitTransaction();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            // 20210710
            if (UIManager.Instance().IsValidDevice == false)
            {
                string message = "[901] It is not valid device.\n";
                message += "["+ UIManager.Instance().PosModel.Environment.EnvPosSetup .GblTaxIdNo + "],["+ UIManager.Instance().PosModel.Environment.EnvPosSetup .GblBrcCod + "]"; 
                await DisplayAlert("Info", message, "Ok");
                return;
            }
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Start":
                    string result = "";
                    result = OpenCheck.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                    if (StateModel.IsIt_OP_NEXT(result))
                    {
                        // JINIT_전일자를 마감하지 않은 상태면 확인메시지를 표시함
                        if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate != System.DateTime.Now.ToString("yyyyMMdd"))
                        {
                            string locationTitle2 = UILocation.Instance().GetLocationText("Warning");
                            string locationMessage2 = UILocation.Instance().GetLocationText("The previous date is not close.Should we go to the menu?");
                            var retClose = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                            if (retClose)
                            {
                                Navigation.InsertPageBefore(new PersonalShopEndOfDayMainPage(), this);
                                await Navigation.PopAsync();
                            }
                            else
                            {
                                Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
                                await Navigation.PopAsync();
                            }
                        }
                        else
                        {
                            //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Start.", "OK");
                            Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
                            await Navigation.PopAsync();
                        }
                    }
                    else if (StateModel.IsIt_OP_FAR(result))
                    {
                        Navigation.InsertPageBefore(new PersonalShopOpenPage(), this);
                        await Navigation.PopAsync();
                    }
                    //Navigation.InsertPageBefore(new PersonalShopOpenPage(), this);
                    //await Navigation.PopAsync();
                    break;
                case "Exit":
                    var closer = DependencyService.Get<ICloseApplication>();
                    closer?.closeApplication();
                    break;
                default:
                    UIManager.Instance().InputModel.Clear();
                    break;
            }
        }
    }
}

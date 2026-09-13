using EBM2x.Database;
using EBM2x.Database.Master;
using EBM2x.Datafile.regitotal;
using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Process.open;
using EBM2x.RraSdc;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.Phone.SignOn;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Open
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopOpenPage : ContentPage
    {

        public PersonalShopOpenPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();

            textAppVersion.InvalidateSurface(RraSdcService.APPLICATION_NAME + " / " + UIManager.AppVersion);
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
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "CancelClose":
                    string adminPwd2 = Utils.Common.getAdminPass();
                    //if (UIManager.Instance().InputModel.EnteredText.Equals(adminPwd2))
                    //{
                        string closeDate2 = System.DateTime.Now.ToString("yyyyMMdd");
                        if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate.Equals(closeDate2))
                        {
                            UIManager.Instance().PosModel.RegiTotal.RegiHeader.CloseFlag = false;
                            RegiTotalWriter.write(UIManager.Instance().PosModel);
                            UIManager.Instance().InputModel.Clear();
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Canceled", "It was canceled.", "OK");

                            Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                            await Navigation.PopAsync();
                        }
                    //}
                    break;
                case "Open":
                    // POS가 마감되어 있으면
                    if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.IsClosed())
                    {
                        string closeDate = System.DateTime.Now.ToString("yyyyMMdd");
                        if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate.Equals(closeDate))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Closed", "It is settled and cannot be used.", "OK");
                            break;
                        }
                    }
                    string result = OpenProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    if (StateModel.IsIt_OP_NEXT(result))
                    {
                        BhfProcess bhfProcess = new BhfProcess();
                        bhfProcess.BhfDownloadProcess();
                        //DisplayAlert("ReceiveBhf", "This operation was called asynchronously.", "OK");
                        CodeProcess codeProcess = new CodeProcess();
                        codeProcess.CodeDownloadProcess();
                        //DisplayAlert("ReceiveCode", "This operation was called asynchronously.", "OK");
                        ClassificationProcess classificationProcess = new ClassificationProcess();
                        classificationProcess.ClassificationDownloadProcess();
                        //DisplayAlert("ReceiveClassification", "This operation was called asynchronously.", "OK");
                        CustomerTinProcess customerTinProcess = new CustomerTinProcess();
                        customerTinProcess.CustomerTinDownloadProcess();
                        //DisplayAlert("ReceiveCode", "This operation was called asynchronously.", "OK");
                        ItemProcess itemProcess = new ItemProcess();
                        itemProcess.ItemDownloadProcess();
                        //DisplayAlert("ReceiveItem", "This operation was called asynchronously.", "OK");
                        //NoticeProcess noticeProcess = new NoticeProcess();
                        //noticeProcess.NoticeDownloadProcess();
                        //DisplayAlert("ReceiveNotice", "This operation was called asynchronously.", "OK");
                        StockMoveProcess stockMoveProcess = new StockMoveProcess();
                        stockMoveProcess.StockMoveDownloadProcess();
                        //DisplayAlert("ReceiveStockMove", "This operation was called asynchronously.", "OK");

                        Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
                        await Navigation.PopAsync();
                    }
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

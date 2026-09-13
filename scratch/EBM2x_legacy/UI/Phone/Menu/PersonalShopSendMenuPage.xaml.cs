using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.Phone.SignOn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopSendMenuPage : ContentPage
    {
        int fromInvoice = 0;
        int toInvoice = 0;

        public PersonalShopSendMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            extFunctionPanel.IsVisible = true;
            extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
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
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a function");

            int countWait = RraSdcJsonWriter.GetCount();
            waitingEntry.InvalidateSurface("" + countWait);

            if (fromInvoice == 0)
            {
                fromInvoiceEntry.InvalidateSurface("");
                toInvoiceEntry.InvalidateSurface("");
            } else
            {
                fromInvoiceEntry.InvalidateSurface("" + fromInvoice);
                toInvoiceEntry.InvalidateSurface("" + toInvoice);
            }
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
                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Back":
                    UIManager.Instance().InputModel.Clear();
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays > 100)
                    {
                        DateTime dateTime = DateTime.Now;
                        dateTime.AddDays(UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays * (-1));
                        int count = RraSdcJsonWriter.GetTransactionMobileCount(dateTime.ToString("yyyyMMdd"));
                        if (count > 100)
                        {
                            //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the number of failed transfers. [" + count + "]", "Ok");
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your computer was not connected to internet for more than 3 days/ please connect it to internet to be synchronized and wait for synchronization to complete.", "Ok");
                            return;
                        }
                        else
                        {
                            Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                            await Navigation.PopAsync();
                        }
                    }
                    else if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineAmount > 0)
                    {
                        double amount = RraSdcJsonWriter.GetTransactionSalesReceipt();
                        if (amount > 5000000)
                        {
                            // EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the amount of failed transfers. [" + amount + "]", "Ok");
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your Invoices was not reported to KRA because your computer is not connected to internet. Please reconnect it and wait for synchronization to complete.", "Ok");
                            return;
                        }
                        else
                        {
                            Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                        await Navigation.PopAsync();
                    }
                    break;
                case "RraSdcSend":
                    // 복구
                    RraSdcJsonWriter.RestoreErrorFile();

                    UIManager.Instance().InputModel.Clear();
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "SendTran", "This operation was called asynchronously.", "OK");

                    int countWait2 = RraSdcJsonWriter.GetCount();
                    waitingEntry.InvalidateSurface("" + countWait2);
                    break;
                case "FromInvoice":
                    try
                    {
                        int newFromInvoice = int.Parse(UIManager.Instance().InputModel.EnteredText);
                        if (newFromInvoice + 1000 < toInvoice)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "From Invoice", "The maximum number that can be processed at one time is 100.", "OK");

                            UIManager.Instance().InputModel.Clear();
                            break;
                        }

                        if (newFromInvoice > toInvoice)
                        {
                            toInvoice = newFromInvoice;
                        }

                        fromInvoice = newFromInvoice;
                    }
                    catch
                    {
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    
                    UIManager.Instance().InputModel.Clear();

                    if (fromInvoice == 0)
                    {
                        toInvoice = 0;
                        
                        fromInvoiceEntry.InvalidateSurface("");
                        toInvoiceEntry.InvalidateSurface("");
                    }
                    else
                    {
                        fromInvoiceEntry.InvalidateSurface("" + fromInvoice);
                        toInvoiceEntry.InvalidateSurface("" + toInvoice);
                    }

                    break;
                case "ToInvoice":
                    try
                    {
                        int newToInvoice = int.Parse(UIManager.Instance().InputModel.EnteredText);
                        if (fromInvoice > newToInvoice)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "To Invoice", "To Invoice cannot be less than From Invoice.", "OK");

                            UIManager.Instance().InputModel.Clear();
                            break;
                        }
                        if (fromInvoice + 1000 < newToInvoice)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "To Invoice", "The maximum number that can be processed at one time is 100.", "OK");

                            UIManager.Instance().InputModel.Clear();
                            break;
                        }

                        toInvoice = newToInvoice;
                    }
                    catch
                    {
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }

                    UIManager.Instance().InputModel.Clear();

                    if (fromInvoice == 0)
                    {
                        fromInvoiceEntry.InvalidateSurface("");
                        toInvoiceEntry.InvalidateSurface("");
                    }
                    else
                    {
                        fromInvoiceEntry.InvalidateSurface("" + fromInvoice);
                        toInvoiceEntry.InvalidateSurface("" + toInvoice);
                    }

                    break;
                case "ReRraSdcSend":
                    if (fromInvoice == 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Resend Invoice", "Enter the invoice to be sent.(From ~ To)", "OK");

                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    else
                    {
                        //TR 생성
                        OnFunctionSendTranSales(fromInvoice, toInvoice);
                        OnFunctionSendTranSalesRcpt(fromInvoice, toInvoice);

                        fromInvoice = 0;
                        toInvoice = 0;
                        fromInvoiceEntry.InvalidateSurface("");
                        toInvoiceEntry.InvalidateSurface("");
                    }

                    UIManager.Instance().InputModel.Clear();
                    RraSdcUploadProcess rraSdcUploadProcessR = new RraSdcUploadProcess();
                    rraSdcUploadProcessR.UploadProcess();
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ResendTran", "This operation was called asynchronously.", "OK");

                    int countWait3 = RraSdcJsonWriter.GetCount();
                    waitingEntry.InvalidateSurface("" + countWait3);
                    break;
                default:
                    break;
            }

            int countWait = RraSdcJsonWriter.GetCount();
            waitingEntry.InvalidateSurface("" + countWait);
        }
        private void OnFunctionSendTranSales(int fromInvoice, int toInvoice)
        {
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            List<TrnsSalesSaveReq> sendList = trnsSaleRraSdcUpload.getTrnsSaleTable(fromInvoice, toInvoice);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Sales";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                if (sendList[i].itemList.Count > 0)
                {
                    RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
                }
            }
        }
        private void OnFunctionSendTranSalesRcpt(int fromInvoice, int toInvoice)
        {
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            List<TrnsSalesRcptSaveReq> sendListRcpt = trnsSaleRcptRraSdcUpload.getTrnsSaleRcptTable(fromInvoice, toInvoice);
            for (int i = 0; i < sendListRcpt.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "SalesReceipt";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_RECEIPT_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendListRcpt[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }
        

    }
}

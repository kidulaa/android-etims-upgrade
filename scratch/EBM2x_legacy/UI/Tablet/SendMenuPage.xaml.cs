using EBM2x.Database;
using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.Setup;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendMenuPage : ExtContentPage
    {
        int fromInvoice = 0;
        int toInvoice = 0;

        bool pageIsActive;
        public SendMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            UIManager.Instance().InputModel.Clear();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing(); 

            UIManager.Instance().PosModel.SetSalesTitleText("Send|Transaction");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a function.");

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            int countWait = RraSdcJsonWriter.GetCount();
            waitingEntry.InvalidateSurface("" + countWait);

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
                        int count = RraSdcJsonWriter.GetTransactionOldCount(dateTime.ToString("yyyyMMdd"));
                        if (count > 100)
                        {
                            //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the number of failed transfers. [" + count + "]", "Ok");
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your computer was not connected to internet for more than 3 days/ please connect it to internet to be synchronized and wait for synchronization to complete.", "Ok");
                        }
                        else
                        {
                            Navigation.InsertPageBefore(new SalesMenuPage(), this);
                            await Navigation.PopAsync();
                        }
                    }
                    else if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineAmount > 100)
                    {
                        double amount = RraSdcJsonWriter.GetTransactionSalesReceipt();
                        if (amount > UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineAmount)
                        {
                            // EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the amount of failed transfers. [" + amount + "]", "Ok");
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your Invoices was not reported to KRA because your computer is not connected to internet. Please reconnect it and wait for synchronization to complete.", "Ok");
                        }
                        else
                        {
                            Navigation.InsertPageBefore(new SalesMenuPage(), this);
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        Navigation.InsertPageBefore(new SalesMenuPage(), this);
                        await Navigation.PopAsync();
                    }
                    break;
                case "Setting":
                    string adminPwd3 = Utils.Common.getAdminPass();
                    if (UIManager.Instance().InputModel.EnteredText.Equals(adminPwd3))
                    {
                        UIManager.Instance().InputModel.Clear();
                        await Navigation.PushAsync(new EBM2xDeviceSetupPage());
                    }
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

                case "RraSdcSend":
                    // 복구
                    RraSdcJsonWriter.RestoreErrorFile();

                    if (UIManager.Instance().InputModel.EnteredText.Length == 8)
                    {
                        string startDate = UIManager.Instance().InputModel.EnteredText;
                        if (startDate.Substring(0, 3).Equals("202"))
                        {
                            OnFunctionSendItem();
                            OnFunctionSendTranSales(startDate);
                            OnFunctionSendTranSalesRcpt(startDate);
                            OnFunctionSendStockIoSave(startDate);

                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Sales, SalesReceipt Json file.", "OK");
                        }
                    }

                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    if (UIManager.Instance().InputModel.EnteredText.Equals("107"))
                    {
                        ////string startDate = "20200513";
                        ////OnUpdateClassification();
                        ////OnFunctionSendItem();
                        ////OnFunctionSendTranSales(startDate);
                        ////OnFunctionSendTranSalesRcpt(startDate);
                        ////OnFunctionSendStockIoSave(startDate);
                        ////rraSdcUploadProcess.UploadProcess(true);
                    }
                    //else if (UIManager.Instance().InputModel.EnteredText.Equals("901"))
                    //{
                    //    rraSdcUploadProcess.UploadProcess(true);
                    //}
                    //else if (UIManager.Instance().InputModel.EnteredText.Equals("999"))
                    //{
                    //    rraSdcUploadProcess.UploadClearProcess(true);
                    //}

                    else
                    {
                        rraSdcUploadProcess.UploadProcess();
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "SendTran", "This operation was called asynchronously.", "OK");

                    UIManager.Instance().InputModel.Clear();

                    int countWait2 = RraSdcJsonWriter.GetCount();
                    waitingEntry.InvalidateSurface("" + countWait2);
                    break;
                default:
                    break;
            }
            int countWait = RraSdcJsonWriter.GetCount();
            waitingEntry.InvalidateSurface("" + countWait);
        }

        private void OnFunctionSendItem()
        {
            ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
            List<ItemSaveReq> sendList = itemRraSdcUpload.getItemTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Item";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_ITEM_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
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


        private void OnFunctionSendTranSales(string startDate)
        {
            string endDate = DateTime.Now.ToString("yyyyMMdd");
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            List<TrnsSalesSaveReq> sendList = trnsSaleRraSdcUpload.getTrnsSaleTable(startDate, endDate);
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

        //판매 영수증 정보 JSON 생성
        private void OnFunctionSendTranSalesRcpt(string startDate)
        {
            string endDate = DateTime.Now.ToString("yyyyMMdd");
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            List<TrnsSalesRcptSaveReq> sendList = trnsSaleRcptRraSdcUpload.getTrnsSaleRcptTable(startDate, endDate);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "SalesReceipt";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_RECEIPT_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        private void OnFunctionSendStockIoSave(string startDate)
        {
            string endDate = DateTime.Now.ToString("yyyyMMdd");
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            List<StockIoSaveReq> sendList = stockIoRraSdcUpload.getStockIoTable(startDate, endDate);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "StockIo";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_STOCK_IO_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        private void OnUpdateClassification()
        {
            // DELETE RRASDC ITEM : OK

            try
            {
                EBM2xDBClientProvider bBM2xDBClientProvider = EBM2xDBClientProvider.getInstance();
                if (bBM2xDBClientProvider != null)
                {
                    if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
                    {
                        string DBServer = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer;
                        string Database = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLDatabase;
                        string DBUid = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLUid;
                        string DBPwd = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLPwd;
                        bBM2xDBClientProvider.OpenConnection(DBServer, Database, DBUid, DBPwd);
                    }
                    else
                    {
                        bBM2xDBClientProvider.OpenConnection("", "", "", "");
                    }
                    //bBM2xDBClientProvider.OpenConnection();
                    using (var command = bBM2xDBClientProvider.GetDbCommand())
                    {
                        string buffer = "";
                        // UPDATE TAXPAYER_ITEM 
                        buffer = "UPDATE TAXPAYER_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                        // UPDATE TRNS_SALE_ITEM 
                        buffer = "UPDATE TRNS_SALE_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                        // UPDATE TRNS_PURCHASE_ITEM 
                        buffer = "UPDATE TRNS_PURCHASE_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                        // UPDATE STOCK_IO_ITEM 
                        buffer = "UPDATE STOCK_IO_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", ex.ToString(), "OK");
            }
        }
    }
}


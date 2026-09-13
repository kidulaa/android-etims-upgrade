using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.RraSdc.process;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.RraSdc
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EBM2xSdcManagementPage : ContentPage
    {
        RequestResponList requestResponList;
        public ObservableCollection<RequestResponNode> RequestResponNodes { get; set; }
        
        public EBM2xSdcManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AnimationLoop();
        }

        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }

            SetList(requestResponList.NodeList);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
        void SetList(List<RequestResponNode> datas)
        {
            try
            {
                RequestResponNodes = new ObservableCollection<RequestResponNode>();
                listView.ItemsSource = RequestResponNodes;

                for (int i = 0; i < datas.Count; i++)
                {
                    RequestResponNodes.Add(datas[i]);
                }
            }
            catch
            {
            }
        }
        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        //================================================================//
        // RECV                                                           //
        //================================================================//

        //코드정보 수신
        private void OnFunctionCodeReq(object sender, EventArgs e)
        {
            CodeProcess codeProcess = new CodeProcess();
            codeProcess.CodeDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveCode", "This operation was called asynchronously.", "OK");
        }

        //지점정보 수신
        private void OnFunctionBhfReq(object sender, EventArgs e)
        {
            BhfProcess bhfProcess = new BhfProcess();
            bhfProcess.BhfDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveBhf", "This operation was called asynchronously.", "OK");
        }

        //상품정보 수신
        private void OnFunctionItem(object sender, EventArgs e)
        {
            ItemProcess itemProcess = new ItemProcess();
            itemProcess.ItemDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveItem", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionNotice(object sender, EventArgs e)
        {
            NoticeProcess noticeProcess = new NoticeProcess();
            noticeProcess.NoticeDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveNotice", "This operation was called asynchronously.", "OK");
        }
        private void OnFunctionClassification(object sender, EventArgs e)
        {
            ClassificationProcess classificationProcess = new ClassificationProcess();
            classificationProcess.ClassificationDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveClassification", "This operation was called asynchronously.", "OK");
        }

        //매출 수신 (자점)
        private void OnFunctionReceiveTranSales(object sender, EventArgs e)
        {
            TrnsSalesProcess trnsSalesProcess = new TrnsSalesProcess();
            trnsSalesProcess.TrnsSalesDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveTranSales", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveTranSalesReceipt(object sender, EventArgs e)
        {
            TrnsSalesReceiptProcess trnsSalesReceiptProcess = new TrnsSalesReceiptProcess();
            trnsSalesReceiptProcess.TrnsSalesReceiptDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveTranSalesReceipt", "This operation was called asynchronously.", "OK");
        }

        //매입 수신 (구매(타점판매) 매입 전송 내역)
        private void OnFunctionReceiveSalesTranPurchase(object sender, EventArgs e)
        {
            TrnsPurchaseSalesProcess trnsPurchaseSalesProcess = new TrnsPurchaseSalesProcess();
            trnsPurchaseSalesProcess.TrnsPurchaseSalesDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveTranPurchase", "This operation was called asynchronously.", "OK");
        }

        //매입 수신 (직매입 전송 내역)
        private void OnFunctionReceiveTranPurchase(object sender, EventArgs e)
        {
            TrnsPurchaseProcess trnsPurchaseProcess = new TrnsPurchaseProcess();
            trnsPurchaseProcess.TrnsPurchaseDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveTranPurchase", "This operation was called asynchronously.", "OK");
        }

        //수입품 수신
        private void OnFunctionReceiveImportItem(object sender, EventArgs e)
        {
            ImportItemProcess importItemProcess = new ImportItemProcess();
            importItemProcess.ImportItemDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveImportItem", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveStockMaser(object sender, EventArgs e)
        {
            StockMasterProcess stockMasterProcess = new StockMasterProcess();
            stockMasterProcess.StockMasterDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveStockMaster", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveStockIo(object sender, EventArgs e)
        {
            StockIoProcess stockIoProcess = new StockIoProcess();
            stockIoProcess.StockIoDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveStockIo", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveStockMove(object sender, EventArgs e)
        {
            StockMoveProcess stockMoveProcess = new StockMoveProcess();
            stockMoveProcess.StockMoveDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveStockMove", "This operation was called asynchronously.", "OK");
        }
        //================================================================//
        // CREATE JSON                                                    //
        //================================================================//

        //판매 정보 JSON 생성
        private void OnFunctionSendTranSales(object sender, EventArgs e)
        {
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            List<TrnsSalesSaveReq> sendList = trnsSaleRraSdcUpload.getTrnsSaleTable("20191122", "20191222");
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Sales";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        //판매 영수증 정보 JSON 생성
        private void OnFunctionSendTranSalesRcpt(object sender, EventArgs e)
        {
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            List<TrnsSalesRcptSaveReq> sendList = trnsSaleRcptRraSdcUpload.getTrnsSaleRcptTable("20191122", "20191222");
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "SalesReceipt";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_RECEIPT_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        //StockMaster JSON 생성
        private void OnFunctionSendTranPurchase(object sender, EventArgs e)
        {
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            List<TrnsPurchaseSaveReq> sendList = trnsPurchaseRraSdcUpload.getTrnsPurchaseTable("20191122", "20191222");
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Purchase";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_PURCHASE_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        //StockIo JSON 생성
        private void OnFunctionSendStockMaster(object sender, EventArgs e)
        {
            StockMasterRraSdcUpload stockMasterRraSdcUpload = new StockMasterRraSdcUpload();
            List<StockMasterSaveReq> sendList = stockMasterRraSdcUpload.getStockMasterTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "StockMaster";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_STOCK_MASTER_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        //상품 정보 JSON 생성
        private void OnFunctionSendStockIoSave(object sender, EventArgs e)
        {
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            List<StockIoSaveReq> sendList = stockIoRraSdcUpload.getStockIoTable("20191122", "20191222");
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "StockIo";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_STOCK_IO_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        //상품 정보 JSON 생성
        private void OnFunctionSendItem(object sender, EventArgs e)
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
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }
        private void OnFunctionSendUser(object sender, EventArgs e)
        {
            BhfUserRraSdcUpload bhfUserRraSdcUpload = new BhfUserRraSdcUpload();
            List<BhfUserSaveReq> sendList = bhfUserRraSdcUpload.getBhfUserTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "User";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_BHF_USER_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        private void OnFunctionSendCust(object sender, EventArgs e)
        {
            BhfCustRraSdcUpload bhfCustRraSdcUpload = new BhfCustRraSdcUpload();
            List<BhfCustSaveReq> sendList = bhfCustRraSdcUpload.getBhfCustTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Cust";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_BHF_CUST_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        private void OnFunctionSendInsurance(object sender, EventArgs e)
        {
            BhfInsuranceRraSdcUpload bhfInsuranceRraSdcUpload = new BhfInsuranceRraSdcUpload();
            List<BhfInsuranceSaveReq> sendList = bhfInsuranceRraSdcUpload.getBhfInsuranceTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Insurance";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_BHF_INSURANCE_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        private void OnFunctionSendItemComposition(object sender, EventArgs e)
        {
            ItemCompositionRraSdcUpload itemCompositionRraSdcUpload = new ItemCompositionRraSdcUpload();
            List<ItemCompositionSaveReq> sendList = itemCompositionRraSdcUpload.getItemCompositionTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "ItemComposition";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_ITEM_COMPOSITION_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Json file.", "OK");
        }

        //================================================================//
        // SEND JSON TO SERVER                                            //
        //================================================================//
        private void OnFunctionSendTran(object sender, EventArgs e)
        {
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "SendTran", "This operation was called asynchronously.", "OK");
        }


        private void OnFunctionReceiveSwVersion(object sender, EventArgs e)
        {
            SwVersionProcess swVersionProcess = new SwVersionProcess();
            swVersionProcess.SwVersionDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveImportItem", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionSendImportItem(object sender, EventArgs e)
        {

        }

    }
}

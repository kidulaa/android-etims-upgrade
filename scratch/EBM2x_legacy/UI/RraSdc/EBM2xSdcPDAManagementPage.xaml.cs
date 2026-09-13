using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.RraSdc.process;
using EBM2x.UI.i18n;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.RraSdc
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EBM2xSdcPDAManagementPage : ContentPage
    {
        RequestResponList requestResponList;
        public ObservableCollection<RequestResponNode> RequestResponNodes { get; set; }
        
        public EBM2xSdcPDAManagementPage()
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

        private void OnFunctionCustomerTin(object sender, EventArgs e)
        {
            CustomerTinProcess customerTinProcess = new CustomerTinProcess();
            customerTinProcess.CustomerTinDownloadProcess();
            DisplayAlert("ReceiveCode", "This operation was called asynchronously.", "OK");
        }

        //코드정보 수신
        private void OnFunctionCodeReq(object sender, EventArgs e)
        {
            CodeProcess codeProcess = new CodeProcess();
            codeProcess.CodeDownloadProcess();
            DisplayAlert("ReceiveCode", "This operation was called asynchronously.", "OK");
        }

        //지점정보 수신
        private void OnFunctionBhfReq(object sender, EventArgs e)
        {
            BhfProcess bhfProcess = new BhfProcess();
            bhfProcess.BhfDownloadProcess();
            DisplayAlert("ReceiveBhf", "This operation was called asynchronously.", "OK");
        }

        //상품정보 수신
        private void OnFunctionItem(object sender, EventArgs e)
        {
            ItemProcess itemProcess = new ItemProcess();
            itemProcess.ItemDownloadProcess();
            DisplayAlert("ReceiveItem", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionNotice(object sender, EventArgs e)
        {
            NoticeProcess noticeProcess = new NoticeProcess();
            noticeProcess.NoticeDownloadProcess();
            DisplayAlert("ReceiveNotice", "This operation was called asynchronously.", "OK");
        }
        private void OnFunctionClassification(object sender, EventArgs e)
        {
            ClassificationProcess classificationProcess = new ClassificationProcess();
            classificationProcess.ClassificationDownloadProcess();
            DisplayAlert("ReceiveClassification", "This operation was called asynchronously.", "OK");
        }

        //매출 수신 (자점)
        private void OnFunctionReceiveTranSales(object sender, EventArgs e)
        {
            TrnsSalesProcess trnsSalesProcess = new TrnsSalesProcess();
            trnsSalesProcess.TrnsSalesDownloadProcess();
            DisplayAlert("ReceiveTranSales", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveTranSalesReceipt(object sender, EventArgs e)
        {
            TrnsSalesReceiptProcess trnsSalesReceiptProcess = new TrnsSalesReceiptProcess();
            trnsSalesReceiptProcess.TrnsSalesReceiptDownloadProcess();
            DisplayAlert("ReceiveTranSalesReceipt", "This operation was called asynchronously.", "OK");
        }

        //매입 수신 (구매(타점판매) 매입 전송 내역)
        private void OnFunctionReceiveSalesTranPurchase(object sender, EventArgs e)
        {
            TrnsPurchaseSalesProcess trnsPurchaseSalesProcess = new TrnsPurchaseSalesProcess();
            trnsPurchaseSalesProcess.TrnsPurchaseSalesDownloadProcess();
            DisplayAlert("ReceiveTranPurchase", "This operation was called asynchronously.", "OK");
        }

        //매입 수신 (직매입 전송 내역)
        private void OnFunctionReceiveTranPurchase(object sender, EventArgs e)
        {
            TrnsPurchaseProcess trnsPurchaseProcess = new TrnsPurchaseProcess();
            trnsPurchaseProcess.TrnsPurchaseDownloadProcess();
            DisplayAlert("ReceiveTranPurchase", "This operation was called asynchronously.", "OK");
        }

        //수입품 수신
        private void OnFunctionReceiveImportItem(object sender, EventArgs e)
        {
            ImportItemProcess importItemProcess = new ImportItemProcess();
            importItemProcess.ImportItemDownloadProcess();
            DisplayAlert("ReceiveImportItem", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveStockMaser(object sender, EventArgs e)
        {
            StockMasterProcess stockMasterProcess = new StockMasterProcess();
            stockMasterProcess.StockMasterDownloadProcess();
            DisplayAlert("ReceiveStockMaster", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveStockIo(object sender, EventArgs e)
        {
            StockIoProcess stockIoProcess = new StockIoProcess();
            stockIoProcess.StockIoDownloadProcess();
            DisplayAlert("ReceiveStockIo", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionReceiveStockMove(object sender, EventArgs e)
        {
            StockMoveProcess stockMoveProcess = new StockMoveProcess();
            stockMoveProcess.StockMoveDownloadProcess();
            DisplayAlert("ReceiveStockMove", "This operation was called asynchronously.", "OK");
        }

        private void OnFunctionDateTime(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + "selectServerTime";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "POST"; //POST
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(responseStream, Encoding.UTF8);

                    string jsonResponse = readerPost.ReadToEnd().Trim();
                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse.Length == 14)
                    {
                        // 시간 변경
                        //bool result = DisplayAlert("Confirm", "Initialize the received information.", "Yes", "No");
                        //if (!result) return;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        async void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string locationTitle2 = UILocation.Instance().GetLocationText("Combine");
                string locationMessage2 = UILocation.Instance().GetLocationText("Initialize the received information.");
                var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                if (!result) return;

                RequestResponNode requestResponNode = (RequestResponNode)e.SelectedItem;
                // 저장 이력
                requestResponNode.ProcessCount = 0;
                if (requestResponNode.ProcessName.Equals(RraSdcService.URL_CODE_SEARCH))
                {
                    requestResponNode.LastDate = "20150101000000";
                }
                else
                {
                    requestResponNode.LastDate = "20200218000000";
                }
                RraSdcHistoryWriter.WriteTransaction(requestResponList);

                SetList(requestResponList.NodeList);
            }
        }
    }
}

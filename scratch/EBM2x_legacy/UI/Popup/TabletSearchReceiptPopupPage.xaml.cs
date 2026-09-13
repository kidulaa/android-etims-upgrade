using EBM2x.Datafile.trlog;
using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Models.journal;
using EBM2x.Models.ListView;
using EBM2x.Process.refund;
using EBM2x.Process.search;
using EBM2x.RraSdc;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletSearchReceiptPopupPage : ContentPage
    {
        TranModel SelectedTranModel = null;

        public ObservableCollection<SearchReceiptListViewModel> SearchReceiptList { get; set; }
        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }
        string curDate = ""; // JINIT_영업일자

        public TabletSearchReceiptPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchReceiptList = new ObservableCollection<SearchReceiptListViewModel>();
            SearchReceiptDetailList = new ObservableCollection<SearchReceiptDetailListViewModel>();

            listView.ItemsSource = SearchReceiptList;
            detailListView.ItemsSource = SearchReceiptDetailList;

            salesDateEntry.TitleInvalidateSurface("Sales Date");
            //invoiceNumEntry.TitleInvalidateSurface("Invoice Num");

            // JINIT_영업일자설정
            curDate = UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate;
            salesDateEntry.SetDateTime(DateTime.ParseExact(curDate, "yyyyMMdd", null));
        }

        bool GetSearchItemList()
        {
            SearchReceiptList.Clear();
            SearchReceiptDetailList.Clear();
            List<SearchReceiptNode> list = null;
            List<SearchReceiptNode> listBackup = null;

            // JINIT_조회하는일자가 현영업일자하고 같으면 tran/receipt 폴더에서 조회, 아니면 backup폴더에서 조회
            if (curDate == salesDateEntry.GetDateTime().ToString("yyyyMMdd"))
            {
                list = SearchReceiptProcess.QuerydSearchReceipt("");
                //listBackup = SearchReceiptProcess.QuerydSearchReceipt(salesDateEntry.GetDateTime().ToString("yyyyMMdd"));
                //if(listBackup.Count > 0)
                //{
                //    foreach (SearchReceiptNode node in listBackup)
                //    {
                //        list.Add(node);
                //    }
                //}
            }
            else
            {
                list = SearchReceiptProcess.QuerydSearchReceipt(salesDateEntry.GetDateTime().ToString("yyyyMMdd"));
            }
            if (list.Count == 0) return false;

            foreach (SearchReceiptNode node in list)
            {
                node.Amount *= node.Sign; // JINIT_반품인 경우 금액을 (-)로 표시
                SearchReceiptList.Add(new SearchReceiptListViewModel { Node = node });
            }
            return true;
        }
        void GetSearchItemDetailList(SearchReceiptNode searchReceiptNode)
        {
            TranModel tranModel = null;
            SearchReceiptDetailList.Clear();

            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
            {
                SocketModel socketRequestModel = new SocketModel();
                socketRequestModel.WCC = "LoadReceiptNode";
                socketRequestModel.JsonRequest = searchReceiptNode.SalesFilename;

                SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                if (socketResponseModel != null)
                {
                    if (socketResponseModel.WCC.Equals("SUCCESS"))
                    {
                        try
                        {
                            tranModel = JsonConvert.DeserializeObject<TranModel>(socketResponseModel.JsonRequest);
                            if (tranModel != null && tranModel.TranNode != null) SelectedTranModel = tranModel;
                            else SelectedTranModel = null;

                            foreach (JournalString node in tranModel.Journal.JournalStringList)
                            {
                                SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            else
            {
                tranModel = TransactionReader.readSystempath(searchReceiptNode.SalesFilename);
                if (tranModel != null && tranModel.TranNode != null) SelectedTranModel = tranModel;
                else SelectedTranModel = null;

                foreach (JournalString node in tranModel.Journal.JournalStringList)
                {
                    SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Search|Receipt");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Item");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }
        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SearchReceiptNode searchReceiptNode = ((SearchReceiptListViewModel)e.SelectedItem).Node;
                GetSearchItemDetailList(searchReceiptNode);
            }
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Query":
                    if (!GetSearchItemList())
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                    }
                    break;

                case "Refund":
                    if (OnResult != null && SelectedTranModel != null)
                    {
                        // JINIT_반품거래인 경우 다시 반품할 수 없음.
                        if (SelectedTranModel.TranNode.Sign == -1)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "This receipt cannot be refund.", "OK");
                            break;
                        }

                        // JINIT_이미 반품한 거래인 경우 반품할 수 없음.
                        if (! string.IsNullOrEmpty(SelectedTranModel.TranInformation.RefundBarcodeNo))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "This receipt has already been refund.", "OK");
                            break;
                        }

                        OnRefundSalesTapped(SelectedTranModel);
                    }
                    break;
                case "Reload":
                    if (OnResult != null && SelectedTranModel != null)
                    {
                        // JINIT_반품거래인 경우 재등록 할 수 없음.
                        if (SelectedTranModel.TranNode.Sign == -1)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "This receipt cannot be reload.", "OK");
                            break;
                        }
                        SelectedTranModel.TranInformation.RefundBarcodeNo = "";
                        ExtEventArgs extEventArgs = new ExtEventArgs("ReloadTranModel", SelectedTranModel);
                        OnResult?.Invoke(this, extEventArgs);
                    }
                    break;
                case "Reprint":
                    if (SelectedTranModel != null)
                    {
                        string locationTitle = UILocation.Instance().GetLocationText("Confirm");
                        string locationMessage = UILocation.Instance().GetLocationText("Do you want to print it out?");
                        var retPrint = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                        if (retPrint)
                        {
                            PrintingService printingService = new PrintingService();
                            // JINIT_파리미터를 List에서 JournalModel로 변경,
                            //printingService.writeJurnal(SelectedTranModel.Journal.JournalStringList, true);
                            printingService.writeJurnal(SelectedTranModel.Journal, true);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnRefundSalesTapped(TranModel SelectedTranModel)
        {
            var popupPage = new TabletRefundReasonPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    SearchRefundReasonNode searchRefundReasonNode = (SearchRefundReasonNode)((ExtEventArgs)ex).EnteredObject;
                    AddSearchRefundReasonProcess.excuteProcess(searchRefundReasonNode, SelectedTranModel, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                    UIManager.Instance().InputModel.Clear();

                    ExtEventArgs extEventArgs = new ExtEventArgs("RefundTranModel", SelectedTranModel);
                    OnResult?.Invoke(this, extEventArgs);

                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (sender, e) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }
    }
}

using EBM2x.Database.MasterEbm2x;
using EBM2x.Datafile.trlog;
using EBM2x.Journal.close;
using EBM2x.Models;
using EBM2x.Models.journal;
using EBM2x.Models.ListView;
using EBM2x.Process.search;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletZReportPopupPage : ContentPage
    {
        //TranModel SelectedTranModel = null;

        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }
        PosModel posModel = null; 
        string curDate = ""; // JINIT_영업일자

        public TabletZReportPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SearchReceiptDetailList = new ObservableCollection<SearchReceiptDetailListViewModel>();

            detailListView.ItemsSource = SearchReceiptDetailList;

            salesDateEntry.TitleInvalidateSurface("Sales Date");

            // JINIT_영업일자설정
            curDate = UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate;
            salesDateEntry.SetDateTime(DateTime.ParseExact(curDate, "yyyyMMdd", null));

            // JINIT_PosModel설정
            this.posModel = new PosModel();
            this.posModel.RegiTotal = UIManager.Instance().PosModel.RegiTotal;
        }

        void GetSearchItemDetailList()
        {
            //SearchReceiptDetailList.Clear();

            RegiTotalJournal regiTotalJournal = new RegiTotalJournal();
            //regiTotalJournal.create(UIManager.Instance().PosModel);
            //foreach (JournalString node in UIManager.Instance().PosModel.Journal.JournalStringList)
            //{
            //    SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            //}

            regiTotalJournal.create(posModel);
            foreach (JournalString node in posModel.Journal.JournalStringList)
            {
                SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            }

            //JournalModel journalModel = GetJournalModel();
            //foreach (JournalString node in journalModel.JournalStringList)
            //{
            //    SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            //}
        }

        JournalModel GetJournalModel()
        {
            double amount = 0;
            int count = 0;

            string fromStr = salesDateEntry.GetDateTime().ToString("yyyyMMdd");
            string toStr = salesDateEntry.GetDateTime().ToString("yyyyMMdd");

            SalesReportMaster salesReportMaster = new SalesReportMaster();

            JournalModel journal = new JournalModel();
            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            // Header
            journal.Add("bold", envPosSetup.GblTaxIdNm);
            journal.Add("", envPosSetup.GblBrcAdr);
            journal.Add("", "TEL: " + envPosSetup.GblBrcTel);
            journal.Add("", "EMAIL: " + envPosSetup.GblBrcEmail);
            journal.Add("", "PIN: " + envPosSetup.GblTaxIdNo);

            string fromStrText = salesDateEntry.GetDateTime().ToString("dd-MM-yyyy");
            string toStrText = salesDateEntry.GetDateTime().ToString("dd-MM-yyyy");
            journal.Add("DATE: " + fromStrText + " ~ " + toStrText);
            journal.Add("");
            journal.Add("--------------SALES-----------------");
            count = salesReportMaster.GetCodeCount(fromStr, toStr, "N", "S");
            journal.Add(string.Format("TOTAL NUMBER SALES NS: {0}", count));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TOT_AMT");
            journal.Add(string.Format("TOTAL AMOUNT SALES NS: {0:##,##0.00}", amount));
            count = salesReportMaster.GetCodeCount(fromStr, toStr, "C", "S");
            journal.Add(string.Format("TOTAL NUMBER COPY SALES NS: {0}", count));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "C", "S", "TRNS_SALE.TOT_AMT");
            journal.Add(string.Format("TOTAL AMOUNT COPY SALES NS: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("--------------REFUND----------------");
            count = salesReportMaster.GetCodeCount(fromStr, toStr, "N", "R");
            journal.Add(string.Format("TOTAL NUMBER REFUND NR: {0}", count));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TOT_AMT");
            journal.Add(string.Format("TOTAL AMOUNT REFUND NR: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("--------------SALE TAX--------------");
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_A");
            journal.Add(string.Format("TOTAL TAXABLE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_B");
            journal.Add(string.Format("TOTAL TAXABLE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_C");
            journal.Add(string.Format("TOTAL TAXABLE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_D");
            journal.Add(string.Format("TOTAL TAXABLE D: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_A");
            journal.Add(string.Format("TOTAL TAXE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_B");
            journal.Add(string.Format("TOTAL TAXE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_C");
            journal.Add(string.Format("TOTAL TAXE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_D");
            journal.Add(string.Format("TOTAL TAXE D: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("--------------REFUND TAX-----------");
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_A");
            journal.Add(string.Format("TOTAL TAXABLE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_B");
            journal.Add(string.Format("TOTAL TAXABLE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_C");
            journal.Add(string.Format("TOTAL TAXABLE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_D");
            journal.Add(string.Format("TOTAL TAXABLE D: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_A");
            journal.Add(string.Format("TOTAL TAXE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_B");
            journal.Add(string.Format("TOTAL TAXE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_C");
            journal.Add(string.Format("TOTAL TAXE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_D");
            journal.Add(string.Format("TOTAL TAXE D: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("Date : " + DateTime.Now.ToString("dd-MM-yyyy") + "  " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
            journal.Add("--------------END------------------");
            journal.Add("");
            journal.Add("");

            return journal;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Z Report");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Receipt");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            await Navigation.PopAsync();
        }

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            SearchReceiptDetailList.Clear();
            getRegiTotal();
            if (posModel.RegiTotal == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                return;
            }
            GetSearchItemDetailList();
        }

        // JINIT_조회일자에 해당되는 RegiTotal파일 설정
        void getRegiTotal()
        {
            if (curDate == salesDateEntry.GetDateTime().ToString("yyyyMMdd"))
            {
                posModel.RegiTotal = Datafile.regitotal.RegiTotalReader.read();
            }
            else
            {
                posModel.RegiTotal = Datafile.regitotal.RegiTotalReader.read(salesDateEntry.GetDateTime().ToString("yyyyMMdd"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

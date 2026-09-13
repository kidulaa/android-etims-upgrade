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
    public partial class PhoneZReportPopupPage : ContentPage
    {
        TranModel SelectedTranModel = null;

        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }

        PosModel posModel = null;
        string curDate = ""; // JINIT_영업일자

        public PhoneZReportPopupPage()
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
            //JINIT_201911, SearchReceiptDetailList.Clear();

            RegiTotalJournal regiTotalJournal = new RegiTotalJournal();

            /* JINIT_201911, 
            regiTotalJournal.create(UIManager.Instance().PosModel);

            foreach (JournalString node in UIManager.Instance().PosModel.Journal.JournalStringList)
            {
                SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            }
            */

            regiTotalJournal.create(posModel);
            foreach (JournalString node in posModel.Journal.JournalStringList)
            {
                SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Search|Receipt");
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
            /* JINIT_201911, 
            GetSearchItemDetailList();
            UIManager.Instance().InputModel.Clear();
            */

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

        // JINIT_201911, 조회일자에 해당되는 RegiTotal파일 설정
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

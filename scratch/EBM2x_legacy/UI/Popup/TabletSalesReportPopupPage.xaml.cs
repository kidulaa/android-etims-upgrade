using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Models.journal;
using EBM2x.Models.ListView;
using EBM2x.Models.regitotal;
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
    public partial class TabletSalesReportPopupPage : ContentPage
    {
 
        public ObservableCollection<SalesReportListViewModel> SalesReportList { get; set; }
        ClassTotalList classTotalList;
        RegiTotal regiTotal = null;
        string curDate = ""; // JINIT_당일영업일자

        public TabletSalesReportPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SalesReportList = new ObservableCollection<SalesReportListViewModel>();

            listView.ItemsSource = SalesReportList;

            salesDateEntry.TitleInvalidateSurface("Sales Date");

            // JINIT_영업일자설정
            curDate = UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate;
            salesDateEntry.SetDateTime(DateTime.ParseExact(curDate, "yyyyMMdd", null));
        }

        void GetSearchItemList()
        {
            //SalesReportList.Clear(); // JINIT_ListView가 Clear안됨

            //List<ClassTotal> list = SalesReportProcess.QuerydSalesReport();
            //ClassTotalList classTotalList = UIManager.Instance().PosModel.RegiTotal.RegiDetail.ClassTotalList;
            classTotalList = regiTotal.RegiDetail.ClassTotalList;
            foreach (ClassTotal node in classTotalList.List)
            {
                SalesReportList.Add(new SalesReportListViewModel { Node = node });
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Sales|Report");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please enter");
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
            SalesReportList.Clear(); // Listview Clear
            UIManager.Instance().InputModel.Clear();
            getRegiTotal();
            if (regiTotal == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                return;
            }
            GetSearchItemList();
        }

        // JINIT_조회일자에 해당되는 RegiTotal파일 설정
        void getRegiTotal()
        {
            if (curDate == salesDateEntry.GetDateTime().ToString("yyyyMMdd"))
            {
                regiTotal = Datafile.regitotal.RegiTotalReader.read();
            }
            else
            {
                regiTotal = Datafile.regitotal.RegiTotalReader.read(salesDateEntry.GetDateTime().ToString("yyyyMMdd"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

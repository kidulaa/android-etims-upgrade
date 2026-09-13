using EBM2x.Models.ListView;
using EBM2x.Models.regitotal;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneSalesReportPopupPage : ContentPage
    {
 
        public ObservableCollection<SalesReportListViewModel> SalesReportList { get; set; }
        ClassTotalList classTotalList;

        public PhoneSalesReportPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SalesReportList = new ObservableCollection<SalesReportListViewModel>();

            listView.ItemsSource = SalesReportList;

            salesDateEntry.TitleInvalidateSurface("Sales Date");
        }

        void GetSearchItemList()
        {
            SalesReportList.Clear();

            //List<ClassTotal> list = SalesReportProcess.QuerydSalesReport();
            classTotalList = UIManager.Instance().PosModel.RegiTotal.RegiDetail.ClassTotalList;
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

            GetSearchItemList();
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
        void OnQueryButtonClicked(object sender, EventArgs e)
        {
            GetSearchItemList();
            UIManager.Instance().InputModel.Clear();
        }
        
        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

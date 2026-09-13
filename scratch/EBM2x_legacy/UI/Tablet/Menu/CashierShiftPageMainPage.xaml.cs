using EBM2x.Dependency;
using EBM2x.Journal.close;
using EBM2x.Models;
using EBM2x.Models.journal;
using EBM2x.Models.ListView;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashierShiftPageMainPage : ContentPage
    {
        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }

        public CashierShiftPageMainPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();

            SearchReceiptDetailList = new ObservableCollection<SearchReceiptDetailListViewModel>();

            detailListView.ItemsSource = SearchReceiptDetailList;

            // JINIT_201911, 
            UIManager.Instance().InputModel.Clear();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Cashier Shift");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Proceed with Cashier Shift processing.");

            GetSearchItemDetailList();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        void GetSearchItemDetailList()
        {
            SearchReceiptDetailList.Clear();

            OperTotalJournal operTotalJournal = new OperTotalJournal();
            operTotalJournal.create(UIManager.Instance().PosModel);

            foreach (JournalString node in UIManager.Instance().PosModel.Journal.JournalStringList)
            {
                SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            }
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Print":
                    PrintingService printingService = new PrintingService();
                    // JINIT_파리미터를 List에서 JournalModel로 변경,
                    //printingService.writeJurnal(UIManager.Instance().PosModel.Journal.JournalStringList, false);
                    printingService.writeJurnal(UIManager.Instance().PosModel.Journal, false);
                    break;

                case "Back":
                    string locationTitle3 = UILocation.Instance().GetLocationText("Back?");
                    string locationMessage3 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultBack = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                    if (resultBack) await Navigation.PopAsync();
                    break;

                case "Confirm":
                    string locationTitle6 = UILocation.Instance().GetLocationText("Cashier Shift?");
                    string locationMessage6 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultConfirm = await DisplayAlert(locationTitle6, locationMessage6, "Yes", "No");
                    if (resultConfirm)
                    {
                        await Navigation.PopAsync();
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
    }
}

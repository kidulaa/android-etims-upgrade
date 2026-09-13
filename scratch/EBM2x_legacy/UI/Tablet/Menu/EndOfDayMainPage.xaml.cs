using EBM2x.Dependency;
using EBM2x.Journal.close;
using EBM2x.Models;
using EBM2x.Models.journal;
using EBM2x.Models.ListView;
using EBM2x.Process.close;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EndOfDayMainPage : ContentPage
    {
        TranModel SelectedTranModel = null;

        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }

        public EndOfDayMainPage()
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

            UIManager.Instance().PosModel.SetSalesTitleText("End of Day");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Proceed with End Of Day processing.");

            GetSearchItemDetailList();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        void GetSearchItemDetailList()
        {
            SearchReceiptDetailList.Clear();

            RegiTotalJournal regiTotalJournal = new RegiTotalJournal();
            regiTotalJournal.create(UIManager.Instance().PosModel);

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
                    if (resultBack)
                    {
                        if (Navigation.NavigationStack.Count == 1)
                        {
                            Navigation.InsertPageBefore(new SalesMenuPage(), this);
                        }
                        await Navigation.PopAsync();
                    }
                    break;

                case "Confirm":
                    string locationTitle6 = UILocation.Instance().GetLocationText("End Of Day?");
                    string locationMessage6 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultConfirm = await DisplayAlert(locationTitle6, locationMessage6, "Yes", "No");
                    if (resultConfirm)
                    {
                        //CloseProcess.excuteProcessTest(UIManager.Instance().PosModel);
                        CloseProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "End of day", "Complete !", "OK"); // JINIT_마감정산 후 마감완료 메시지 표시
                        
                        Navigation.InsertPageBefore(new SalesMenuPage(), this);
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

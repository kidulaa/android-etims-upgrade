using EBM2x.Dependency;
using EBM2x.Journal.close;
using EBM2x.Models.journal;
using EBM2x.Models.ListView;
using EBM2x.Models.ReadyMoney;
using EBM2x.Process.close;
using EBM2x.Process.eot;
using EBM2x.Process.money;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Phone.Open;
using EBM2x.UI.Phone.SignOn;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopEndOfDayMainPage : ContentPage
    {
        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }
        public PersonalShopEndOfDayMainPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchReceiptDetailList = new ObservableCollection<SearchReceiptDetailListViewModel>();
            listView.ItemsSource = SearchReceiptDetailList;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.InvalidateSurface();

            GetSearchItemDetailList();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

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
        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
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
                            Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
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
                        CloseProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "End of day", "Complete !", "OK"); // JINIT_마감정산 후 마감완료 메시지 표시
                        Navigation.InsertPageBefore(new PersonalShopOpenPage(), this);
                        await Navigation.PopAsync();
                    }
                    break;

                default:
                    break;
            }
        }
    }
}

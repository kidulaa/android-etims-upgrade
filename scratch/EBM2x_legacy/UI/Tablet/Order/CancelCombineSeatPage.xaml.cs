using EBM2x.Models.DiningTable;
using EBM2x.Process;
using EBM2x.Process.dining;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Tablet.SignOn;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Order
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CancelCombineSeatPage : ContentPage
    {
        bool pageIsActive;

        public CancelCombineSeatPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Cancel combine");
            UIManager.Instance().PosModel.InvalidateSurface();

            diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, false, false);

            pageIsActive = true;
            AnimationLoop();
        }

        protected override void OnDisappearing()
        {
            pageIsActive = false;

            Unsubscribe();

            base.OnDisappearing();
        }

        async void AnimationLoop()
        {
            while (pageIsActive)
            {
                diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, false, false);
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        void Subscribe()
        {
            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            MessagingCenter.Subscribe<Object, DiningRoomList>(this, "Dining Room Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(arg, false, false);
                });
            });

            MessagingCenter.Subscribe<Object, DiningTableList>(this, "Dining Table Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningTableManagementPanel.DiningTableListInvalidateSurface(arg, false, false);
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, DiningRoomList>(this, "Dining Room Node");
            MessagingCenter.Unsubscribe<Object, DiningTableList>(this, "Dining Table Node");
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            DiningRoomList diningRoomList = UIManager.Instance().PosModel.DiningTableModel.DiningRoomList;
            DiningTableList diningTableList = diningRoomList.List[diningRoomList.CurrentLineNumber - 1].DiningTableList;
            DiningTableNode diningTableNode = diningTableList.List[diningTableList.CurrentLineNumber - 1];

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Cancel":
                    DiningRoomNodeProcess.SaveDiningTable(diningRoomList);
                    await Navigation.PopAsync();
                    break;

                case "DiningTable":

                    if (diningTableNode.IsOrdered && diningTableNode.IsGroup)
                    {
                        string locationTitle2 = UILocation.Instance().GetLocationText("Cancel?");
                        string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure?");
                        var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                        if (result)
                        {
                            diningTableNode.IsGroup = false;
                            diningTableNode.GroupCode = string.Empty;
                        }
                    }

                    diningTableManagementPanel.DiningTableListInvalidateSurface(diningTableList, false, false);
                    break;

                default:
                    break;
            }
        }
    }
}

using EBM2x.Models.DiningTable;
using EBM2x.Models.HotelRoom;
using EBM2x.Process;
using EBM2x.Process.hotel;
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
    public partial class CancelCombineRoomPage : ContentPage
    {
        bool pageIsActive;

        public CancelCombineRoomPage()
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

            hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, false, false);

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
                hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, false, false);
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

            MessagingCenter.Subscribe<Object, HotelFloorList>(this, "Hotel Floor Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(arg, false, false);
                });
            });

            MessagingCenter.Subscribe<Object, HotelRoomList>(this, "Hotel Room Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    hotelRoomManagementPanel.HotelRoomListInvalidateSurface(arg, false, false);
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, HotelFloorList>(this, "Hotel Floor Node");
            MessagingCenter.Unsubscribe<Object, HotelRoomList>(this, "Hotel Room Node");
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            HotelFloorList hotelFloorList = UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList;
            HotelRoomList hotelRoomList = hotelFloorList.List[hotelFloorList.CurrentLineNumber - 1].HotelRoomList;
            HotelRoomNode hotelRoomNode = hotelRoomList.List[hotelRoomList.CurrentLineNumber - 1];

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Cancel":
                    HotelFloorNodeProcess.SaveHotelFloor(hotelFloorList);
                    await Navigation.PopAsync();
                    break;

                case "HotelRoom":
                    if (hotelRoomNode.IsOrdered && hotelRoomNode.IsGroup)
                    {
                        string locationTitle2 = UILocation.Instance().GetLocationText("Cancel?");
                        string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure?");
                        var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                        if (result)
                        {
                            hotelRoomNode.IsGroup = false;
                            hotelRoomNode.GroupCode = string.Empty;
                        }
                    }

                    hotelRoomManagementPanel.HotelRoomListInvalidateSurface(hotelRoomList, false, false);
                    break;

                default:
                    break;
            }
        }
    }
}

using EBM2x.UI.Draw;
using EBM2x.UI.Popup;
using EBM2x.UI.Tablet.SignOn;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotelAdminMenuPage : ContentPage
    {
        public HotelAdminMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();
            extFunctionPanel.InitializeComponent();

            extFunctionPanel.IsVisible = true;
            extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.SetSalesTitleText("Admin Menu");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a function");
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
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
                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Back":
                    Navigation.InsertPageBefore(new HotelSignOnPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "HotelRoomManagement":
                    await Navigation.PushAsync(new HotelRoomManagementPage());
                    break;

                case "StoreManagement":
                    await Navigation.PushAsync(new TabletStoreManagementPopupPage());
                    break;
                case "UserManagement":
                    await Navigation.PushAsync(new TabletUserManagementPopupPage());
                    break;
                case "ItemManagement":
                    await Navigation.PushAsync(new TabletItemManagementPopupPage());
                    break;
                case "CustomerManagement":
                    await Navigation.PushAsync(new TabletCustomerManagementPopupPage());
                    break;
                default:
                    break;
            }
        }
    }
}

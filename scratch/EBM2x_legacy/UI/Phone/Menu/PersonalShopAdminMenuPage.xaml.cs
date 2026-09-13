using EBM2x.Database;
using EBM2x.UI.Backoffice.Environment;
using EBM2x.UI.Backoffice.StockManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.Phone.SignOn;
using EBM2x.UI.Popup;
using EBM2x.UI.RraSdc;
using EBM2x.UI.Setup;
using EBM2x.UI.Web;
using System;
using System.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopAdminMenuPage : ContentPage
    {
        public PersonalShopAdminMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            if(!UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod.Equals("00"))
            {
                extAdjustMenuButton.IsVisible = false;
            }
            extFunctionPanel.IsVisible = true;
            extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
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
                //case "PosSetup":
                //    await Navigation.PushAsync(new EBM2xPhoneSetupPage());
                //    break;
                //case "PosReSetup":
                //    await Navigation.PushAsync(new EBM2xPhoneSetupPage(true));
                //    break;
                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Back":
                    Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
                    await Navigation.PopAsync(); 
                    break;

                case "StoreManagement":
                    await Navigation.PushAsync(new PhoneStoreManagementPopupPage());
                    break;
                case "UserManagement":
                    await Navigation.PushAsync(new PhoneUserManagementPopupPage());
                    break;
                case "ItemManagement":
                    await Navigation.PushAsync(new PhoneItemManagementPopupPage());
                    break;
                case "CustomerManagement":
                    await Navigation.PushAsync(new PhoneCustomerManagementPopupPage());
                    break;

                case "StockDate":
                    await Navigation.PushAsync(new PhoneStockDatePage());
                    break;
                //case "TinPopup":
                //    await Navigation.PushAsync(new PhoneSearchTinPopupPage());
                //    break;
                case "SystemSetting":
                    await Navigation.PushAsync(new PhoneSystemSettingPage());
                    break;
                case "DeviceSetting":
                    await Navigation.PushAsync(new EBM2xPhoneDeviceSetupPage());
                    break;


                default:
                    break;
            }
        }
    }
}

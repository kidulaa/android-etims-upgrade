using EBM2x.Dependency;
using EBM2x.Models.State;
using EBM2x.UI.Draw;
using EBM2x.UI.Phone.Sales;
using EBM2x.UI.Phone.SignOn;
using EBM2x.UI.Popup;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopMenuPage : ContentPage
    {
        public PersonalShopMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

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
                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Back":
                    Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
                    await Navigation.PopAsync(); 
                    break;

                case "SalesReport":
                    await Navigation.PushAsync(new PhoneSalesReportPopupPage());
                    break;

                case "ZReport":
                    await Navigation.PushAsync(new PhoneZReportPopupPage());
                    break;

                case "ReserveFund":
                    await Navigation.PushAsync(new PersonalShopReserveFundPage());
                    break;

                case "IntermediateDeposit":
                    await Navigation.PushAsync(new PersonalShopIntermediateDepositPage());
                    break;

                case "EndOfDay":
                    await Navigation.PushAsync(new PersonalShopEndOfDayPage());
                    break;
                default:
                    break;
            }
        }
    }
}

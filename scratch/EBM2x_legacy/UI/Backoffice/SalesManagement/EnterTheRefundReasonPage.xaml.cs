using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterTheRefundReasonPage : ContentPage
    {
        public EnterTheRefundReasonPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton , true: BackButton 
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnFunctionSave(object sender, EventArgs e)
        {

        }
    }
}

using EBM2x.Database.MasterEbm2x;
using EBM2x.Dependency;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectPrinterPage : ContentPage
    {
        public SelectPrinterPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            if(UIManager.Instance().IsWindows)
            {

            }
            else
            {
                btnPrintA4.IsVisible = false;
            }
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
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
        
        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        async void OnFunctionClose(object sender, EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        private void OnFunctionPrintA4(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "PrintA4", null);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionPrintReceipt(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "PrintReceipt", null);
                OnResult?.Invoke(this, extEventArgs);
            }
        }
    }
}

using EBM2x.Database.MasterEbm2x;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputPhoneCheckInformationPage : ContentPage
    {
        TransactionSalesModel SalesModel;
        public InputPhoneCheckInformationPage(TransactionSalesModel salesModel)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SalesModel = salesModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            etEnterTheCustomerPhoneOrTINNumber.SetReadOnly(true);
            etEnterTheCustomerPhoneOrTINNumber.SetEntryValue(SalesModel.TranRecord.CustTin);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        async void OnFunctionClose(object sender, EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        private void OnFunctionConfirm(object sender, EventArgs e)
        {
        }

        private void OnFunctionBankSlip(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "BankSlip", "04");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionMobileMoney(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "MobileMoney", "06");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionDebitCreditCard(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "DebitCreditCard", "05");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionBankCheck(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "BankCheck", "04");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionOther(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "Other", "07");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionCashCredit(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "CashCredit", "03");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionCredit(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "Credit", "02");
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnFunctionCash(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "Cash", "01");
                OnResult?.Invoke(this, extEventArgs);
            }
        }
    }
}

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
    public partial class PrintReceiptPage : ContentPage
    {
        TransactionSalesModel SalesModel;
        bool ProformaMode = false;
        public PrintReceiptPage(TransactionSalesModel salesModel, bool proformaMode)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SalesModel = salesModel;
            ProformaMode = proformaMode;

            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                //btnPrintA4.IsVisible = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //SalesModel
            etInvoiceID.SetEntryValue(SalesModel.TranRecord.InvcNo.ToString());
            etInvoiceID.SetReadOnly(true);
            etCustomerCode.SetEntryValue(SalesModel.TranRecord.CustTin);
            etCustomerCode.SetReadOnly(true);
            etCustomerName.SetEntryValue(SalesModel.TranRecord.CustNm);
            etCustomerName.SetReadOnly(true);
            if (SalesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                etSalesType.InvalidateSurface("Refund Sales");
            }
            else
            {
                etSalesType.InvalidateSurface("Normal Sales");
            }

            if(ProformaMode)
            {
                etSalesType.InvalidateSurface("Proforma Sales");
            }
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
        private void OnFunctionPrintReceipt58(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "PrintReceipt58", null);
                OnResult?.Invoke(this, extEventArgs);
            }
        }
        private void OnFunctionPrintReceipt80(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", "PrintReceipt80", null);
                OnResult?.Invoke(this, extEventArgs);
            }
        }
    }
}

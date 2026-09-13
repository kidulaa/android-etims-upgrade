using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.CustomerManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerPopupPage : ContentPage
    {
        public ObservableCollection<TaxpayerBhfCustRecord> lvCustManagement { get; set; }

        bool IsSelected = false;
        List<TaxpayerBhfCustRecord> listTaxpayerBhfCustRecord;
        TaxpayerBhfCustMaster master = null;
        TaxpayerBhfCustRecord record = null;

        public CustomerPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBhfCustMaster();
            record = new TaxpayerBhfCustRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };

            etSearchUsable.SetReadOnly(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AnimationLoop();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            if (listTaxpayerBhfCustRecord == null || listTaxpayerBhfCustRecord.Count <= 0)
            {
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                listTaxpayerBhfCustRecord = master.getTaxpayerBhfCustTable(tin, bhfid, "", etSearchUsable.GetSelectedItem().Id);
                SetList(listTaxpayerBhfCustRecord);
            }
        }
        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton , true: BackButton 
        }
        async void OnSearch(object sender, EventArgs e)
        {
            string valueLike = etLikeValue.GetEntryValue();
            string valueUseYn = etSearchUsable.GetSelectedItem().Id;
            //if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 3)
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 3 characters.", "OK");
            //    return;
            //}
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            listTaxpayerBhfCustRecord = master.getTaxpayerBhfCustTable(tin, bhfid, valueLike, valueUseYn);
            SetList(listTaxpayerBhfCustRecord);
            if (listTaxpayerBhfCustRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<TaxpayerBhfCustRecord> datas)
        {
            try
            {
                lvCustManagement = new ObservableCollection<TaxpayerBhfCustRecord>();
                listView.ItemsSource = lvCustManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvCustManagement.Add(datas[i]);
                }
            }
            catch
            {
            }
        }

        void OnFunctionCancel(object sender, EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        void OnFunctionConfirm(object sender, EventArgs e)
        {
            if (OnResult != null && IsSelected)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", record);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                IsSelected = true;
                record = (TaxpayerBhfCustRecord)e.SelectedItem;
                SetEntityData(record, true);
            }
            else
            {
                IsSelected = false;
                record.clear();
                SetEntityData(record, true);
            }
        }

        private void SetEntityData(TaxpayerBhfCustRecord record, bool readOnly)
        {
            etTin.SetEntryValue(record.CustNo);
            etTin.SetReadOnly(readOnly);
            etCustomerName.SetEntryValue(record.CustNm);
            etCustomerName.SetReadOnly(readOnly);
            etDelegator.SetEntryValue(record.ChargerNm);
            etDelegator.SetReadOnly(readOnly);
            etNationality.SetEntryValue(record.NationName);
            etNationality.SetReadOnly(readOnly);
            etPhone.SetEntryValue(record.Contact1);
            etPhone.SetReadOnly(readOnly);
            etFAX.SetEntryValue(record.Fax);
            etFAX.SetReadOnly(readOnly);
            etAddress.SetEntryValue(record.Adrs);
            etAddress.SetReadOnly(readOnly);
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }

        async private void OnFunctionAddCustomer(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomerManagementPage());
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.CustomerManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaxpayerPopupPage : ContentPage
    {
        public ObservableCollection<TaxpayerBaseRecord> lvTaxpayerBaseManagement { get; set; }

        bool IsSelected = false;
        TaxpayerBaseMaster master = null;
        TaxpayerBaseRecord record = null;

        public TaxpayerPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBaseMaster();
            record = new TaxpayerBaseRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton
        }
        async void OnSearch(object sender, EventArgs e)
        {
            string valueLike = etLikeValue.GetEntryValue();
            string valueUseYn = etSearchUsable.GetSelectedItem().Id;
            if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 9)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 9 characters.", "OK");
                return;
            }
            List<TaxpayerBaseRecord> list = master.getTaxpayerBaseTable(valueLike, valueUseYn);
            SetList(list);
            if (list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<TaxpayerBaseRecord> datas)
        {
            try
            {
                lvTaxpayerBaseManagement = new ObservableCollection<TaxpayerBaseRecord>();
                listView.ItemsSource = lvTaxpayerBaseManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvTaxpayerBaseManagement.Add(datas[i]);
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
                record = (TaxpayerBaseRecord)e.SelectedItem;
                SetEntityData(record, true);
            }
            else
            {
                IsSelected = false;
                record.clear();
                SetEntityData(record, true);
            }
        }

        private void SetEntityData(TaxpayerBaseRecord record, bool readOnly)
        {
            etTin.SetEntryValue(record.Tin);
            etTin.SetReadOnly(readOnly);
            etTaxprNm.SetEntryValue(record.TaxprNm);
            etTaxprNm.SetReadOnly(readOnly);
            etProvince.SetEntryValue(record.PrvncNm);
            etProvince.SetReadOnly(readOnly);
            etAddress.SetEntryValue(record.LocDesc);
            etAddress.SetReadOnly(readOnly);
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }
    }
}

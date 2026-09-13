using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefundReasonPopupPage : ContentPage
    {
        TransactionSalesModel SalesModel;

        public ObservableCollection<CodeDtlRecord> lvOriginManagement { get; set; }

        bool IsSelected = false;
        CodeDtlMaster master = null;
        CodeDtlRecord record = null;

        public RefundReasonPopupPage(TransactionSalesModel salesModel)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new CodeDtlMaster();
            record = new CodeDtlRecord();

            SalesModel = salesModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            List<CodeDtlRecord> list = master.getCodeDtlTable("32", "");
            SetList(list);
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
            List<CodeDtlRecord> list = master.getCodeDtlTable("32", "");
            SetList(list);
            if (list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<CodeDtlRecord> datas)
        {
            try
            {
                lvOriginManagement = new ObservableCollection<CodeDtlRecord>();
                listView.ItemsSource = lvOriginManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvOriginManagement.Add(datas[i]);
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
                record = (CodeDtlRecord)e.SelectedItem;
                SetEntityData(record, true);
            }
            else
            {
                IsSelected = false;
                record.clear();
                SetEntityData(record, true);
            }
        }

        private void SetEntityData(CodeDtlRecord record, bool readOnly)
        {
            etCd.SetEntryValue(record.Cd);
            etCd.SetReadOnly(readOnly);
            etCdNm.SetEntryValue(record.CdNm);
            etCdNm.SetReadOnly(readOnly);
        }
    }
}

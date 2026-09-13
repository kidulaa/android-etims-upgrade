using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.ItemManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPopupPage : ContentPage
    {
        public ObservableCollection<TaxpayerItemRecord> lvTaxpayerItemManagement { get; set; }

        bool IsSelected = false;
        List<TaxpayerItemRecord> listTaxpayerItemRecord;
        TaxpayerItemMaster master = null;
        TaxpayerItemRecord record = null;

        //@ychan_20191208 Item Type
        string ItemType = string.Empty;
        string WithoutItemType = string.Empty;
       
        public ItemPopupPage(string CallName = null)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerItemMaster();
            record = new TaxpayerItemRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id="Y", Name="" });

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };

            ItemType = string.Empty;
            
            etSearchUsable.SetReadOnly(true);
        }
        public ItemPopupPage(string valueItemTy, string valueWithoutItemTy)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerItemMaster();
            record = new TaxpayerItemRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };

            ItemType = valueItemTy;
            WithoutItemType = valueWithoutItemTy;

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

            if (listTaxpayerItemRecord == null || listTaxpayerItemRecord.Count <= 0)
            {
                bool nonVAT = UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                listTaxpayerItemRecord = master.getTaxpayerItemTable(tin, "", etSearchUsable.GetSelectedItem().Id, ItemType, WithoutItemType, nonVAT);
                SetList(listTaxpayerItemRecord);
            }
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
            string valueItemTy = ItemType;
            //if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 3)
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 3 characters.", "OK");
            //    return;
            //}
            bool nonVAT = UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT;
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTaxpayerItemRecord = master.getTaxpayerItemTable(tin, valueLike, valueUseYn, valueItemTy, WithoutItemType, nonVAT);
            SetList(listTaxpayerItemRecord);
            if (listTaxpayerItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<TaxpayerItemRecord> datas)
        {
            try
            {
                lvTaxpayerItemManagement = new ObservableCollection<TaxpayerItemRecord>();
                listView.ItemsSource = lvTaxpayerItemManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvTaxpayerItemManagement.Add(datas[i]);
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
                record = (TaxpayerItemRecord)e.SelectedItem;
                SetEntityData(record, true);
            }
            else
            {
                IsSelected = false;
                record.clear();
                SetEntityData(record, true);
            }
        }

        private void SetEntityData(TaxpayerItemRecord record, bool readOnly)
        {
            etItemCd.SetEntryValue(record.ItemCd);
            etItemCd.SetReadOnly(readOnly);
            etItemNm.SetEntryValue(record.ItemNm);
            etItemNm.SetReadOnly(readOnly);

            etOrigin.SetEntryValue(record.OrgnNatName);
            etOrigin.SetReadOnly(readOnly);
            etItemType.SetEntryValue(record.ItemTyName);
            etItemType.SetReadOnly(readOnly);
            etPkgUnit.SetEntryValue(record.PkgUnitName);
            etPkgUnit.SetReadOnly(readOnly);
            etQtyUnit.SetEntryValue(record.QtyUnitName);
            etQtyUnit.SetReadOnly(readOnly);

            etPurchasePrice.SetEntryValue(record.InitlWhUntpc.ToString());
            etPurchasePrice.SetReadOnly(readOnly);
            
            etSalePrice.SetEntryValue(record.DftPrc.ToString());
            etSalePrice.SetReadOnly(readOnly);
            
            etCurrentStock.SetEntryValue(record.RdsQty.ToString());
            etCurrentStock.SetReadOnly(readOnly);
            etSafetyStock.SetEntryValue(record.SftyQty.ToString());
            etSafetyStock.SetReadOnly(readOnly);
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }

        async void OnFunctionAddItem(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ItemManagementPage());
        }
    }
}

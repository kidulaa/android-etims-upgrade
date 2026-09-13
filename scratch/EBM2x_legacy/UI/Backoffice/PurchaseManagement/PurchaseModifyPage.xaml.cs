using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.PurchaseManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PurchaseModifyPage : ContentPage
    {
        // 입고 처리
        TransactionPurchaseModel PurchaseModel { get; set; }
        
        TrnsPurchaseItemRecord SelectedItem;

        public PurchaseModifyPage(TrnsPurchaseRecord recordTran, List<TrnsPurchaseItemRecord> listTrnsPurchaseItemRecord)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            Init(recordTran, listTrnsPurchaseItemRecord);
        }

        public void Init(TrnsPurchaseRecord recordTran, List<TrnsPurchaseItemRecord> listTrnsPurchaseItemRecord)
        {
            PurchaseModel = new TransactionPurchaseModel();
            PurchaseModel.TranRecord = recordTran;
            PurchaseModel.ItemRecords = listTrnsPurchaseItemRecord;

            SelectedItem = null;

            UpdateHeaderView();
            UpdateItemView(new TrnsPurchaseItemRecord());
            listView.ItemsSource = PurchaseModel.GetObservableCollection();

            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderView();

            if (SelectedItem != null)
            {
                UpdateItemView(SelectedItem);
                etUnitPrice.SetReadOnly(true);
                etPurchaseQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
                etPurchaseQty.SetFocus();
            }
            else
            {
                UpdateItemView(new TrnsPurchaseItemRecord());
                etUnitPrice.SetReadOnly(true);
                etPurchaseQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        public void UpdateHeaderView()
        {
            etPurchase.SetReadOnly(true);
            etInvoieID.SetReadOnly(true);
            etSupplierID.SetReadOnly(true);
            etSupplierName.SetReadOnly(true);
            etPurchaseDate.SetReadOnly(true);
            etReferenceId.SetReadOnly(true);
            etTotalAmount.SetReadOnly(true);
            etVAT.SetReadOnly(true);
            etRemark.SetReadOnly(true);

            etPurchase.SetEntryValue("Purchase");

            etInvoieID.SetEntryValue(PurchaseModel.TranRecord.InvcNo.ToString("#,##0"));
            etSupplierID.SetEntryValue(PurchaseModel.TranRecord.SpplrTin);
            etSupplierName.SetEntryValue(PurchaseModel.TranRecord.SpplrNm);
            etPurchaseDate.SetEntryValue(PurchaseModel.TranRecord.PchsDt);
            // 확인 필요
            etReferenceId.SetEntryValue(PurchaseModel.TranRecord.OrgInvcNo);
            etTotalAmount.SetEntryValue(PurchaseModel.TranRecord.TotAmt.ToString("#,##0.00"));
            etVAT.SetEntryValue(PurchaseModel.TranRecord.TotTaxAmt.ToString("#,##0.00"));
            etRemark.SetEntryValue(PurchaseModel.TranRecord.Remark);
        }
        public void UpdateItemView(TrnsPurchaseItemRecord itemRecord)
        {
            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etVat.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
            etDCAmount.SetReadOnly(true);
            etPurchasePrice.SetReadOnly(true);
            etTotalPrice.SetReadOnly(true);

            etItemCode.SetEntryValue(itemRecord.ItemCd);
            etItemName.SetEntryValue(itemRecord.ItemNm);
            etClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etClassName.SetEntryValue(itemRecord.ItemClsNm);
            etUnitPrice.SetEntryValue(itemRecord.Prc);
            etPurchaseQty.SetEntryValue(itemRecord.Qty);
            etTaxType.SetSelecteItem(new SystemCode() { Id = itemRecord.TaxTyCd, Name = "" });
            etVat.SetEntryValue(itemRecord.TaxAmt.ToString("#,##0.00"));
            etDCRate.SetEntryValue((int)itemRecord.DcRt);
            etDCAmount.SetEntryValue(itemRecord.DcAmt.ToString("#,##0.00"));
            etPurchasePrice.SetEntryValue(itemRecord.SplyAmt.ToString("#,##0.00"));
            etTotalPrice.SetEntryValue(itemRecord.TotAmt.ToString("#,##0.00"));

            if (string.IsNullOrEmpty(itemRecord.ItemExprDt))
            {
                etExpireDate.SetEntryValue(DateTime.Now);
            }
            else
            {
                try
                {
                    DateTime oDate = DateTime.ParseExact(itemRecord.ItemExprDt, "yyyyMMdd", null);
                    etExpireDate.SetEntryValue(oDate);
                }
                catch
                {
                    etExpireDate.SetEntryValue(DateTime.Now);
                }
            }
        }
        public void UpdateItemView()
        {
            if (SelectedItem != null)
            {
                etVat.SetEntryValue(PurchaseModel.CurrentItemRecord.TaxAmt.ToString("#,##0.00"));
                etDCAmount.SetEntryValue(PurchaseModel.CurrentItemRecord.DcAmt.ToString("#,##0.00"));
                etPurchasePrice.SetEntryValue(PurchaseModel.CurrentItemRecord.SplyAmt.ToString("#,##0.00"));
                etTotalPrice.SetEntryValue(PurchaseModel.CurrentItemRecord.TotAmt.ToString("#,##0.00"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton
        }
        
        async void OnFunctionClose(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnFunctionConfirm(object sender, System.EventArgs e)
        {
            if (SelectedItem != null)
            {
                //if (PurchaseModel.IsExist(SelectedItem.ItemCd))
                //{
                //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                //    return;
                //}

                TrnsPurchaseItemMaster TrnsPurchaseItemMaster = new TrnsPurchaseItemMaster();
                bool saveFlag = TrnsPurchaseItemMaster.ToTable(SelectedItem);
                if(!saveFlag)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "please try again.", "OK");
                    return;
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
                }

                listView.ItemsSource = PurchaseModel.GetObservableCollection();

                UpdateHeaderView();
                UpdateItemView(new TrnsPurchaseItemRecord());

                etUnitPrice.SetReadOnly(true);
                etPurchaseQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a product.", "OK");
            }

            ((ListView)listView).SelectedItem = null;
        }

        private void OnFunctionClear(object sender, EventArgs e)
        {
            SelectedItem = null;
            
            UpdateItemView(new TrnsPurchaseItemRecord());
            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);

            ((ListView)listView).SelectedItem = null;
        }

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (TrnsPurchaseItemRecord)e.SelectedItem;
                UpdateItemView(SelectedItem);
            }
            else
            {
                SelectedItem = null;
            }
        }

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            if (SelectedItem == null || string.IsNullOrEmpty(SelectedItem.ItemCd))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Item.", "OK");
                return;
            }

            var popupPage = new ItemPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;

                    if(PurchaseModel.IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {
                        SelectedItem.ItemCd = popupRecord.ItemCd;
                        SelectedItem.ItemNm = popupRecord.ItemNm;
                        SelectedItem.ItemClsCd = popupRecord.ItemClsCd;
                        SelectedItem.ItemClsNm = popupRecord.ItemClsName;
                    }
                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }
    }
}
using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.CustomerManagement;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.PurchaseManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PurchaseRegistrationPage : ContentPage
    {
        // 입고 처리
        TransactionPurchaseModel PurchaseModel { get; set; }
        
        TrnsPurchaseItemRecord SelectedItem;
        TaxpayerBhfCustRecord CustomerRecord;

        public PurchaseRegistrationPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            Init();

            etUnitPrice.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etPurchaseQty.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etDCRate.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etTaxType.GetPicker().SelectedIndexChanged += (sender, e) => {
                UpdateItemView();
            };
        }

        public void Init()
        {
            TrnsPurchaseMaster TrnsPurchaseMaster = new TrnsPurchaseMaster();
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            //JCNA 202001 DELETE string DvcId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblDvcId;
            long InvcNo = TrnsPurchaseMaster.GetPurchaseSeq();
            string PchsDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "M";
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            PurchaseModel = new TransactionPurchaseModel();
            PurchaseModel.CurrentItemRecord = null;
            PurchaseModel.InitModel(Tin, BhfId, InvcNo, PchsDt, RegTyCd, UserId, UserNm);

            UpdateHeaderView();
            UpdateItemView(new TrnsPurchaseItemRecord());

            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderView();

            if (PurchaseModel.CurrentItemRecord != null)
            {
                UpdateItemView(PurchaseModel.CurrentItemRecord);
                etUnitPrice.SetReadOnly(false);
                etPurchaseQty.SetReadOnly(false);
                etTaxType.SetReadOnly(false);
                etDCRate.SetReadOnly(false);
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
            etReferenceId.SetReadOnly(false);
            etTotalAmount.SetReadOnly(true);
            etVAT.SetReadOnly(true);
            etRemark.SetReadOnly(false);

            etPurchase.SetEntryValue("Purchase");

            etInvoieID.SetEntryValue(PurchaseModel.TranRecord.InvcNo.ToString("#,##0"));
            etSupplierID.SetEntryValue(PurchaseModel.TranRecord.SpplrTin);
            etSupplierName.SetEntryValue(PurchaseModel.TranRecord.SpplrNm);
            etPurchaseDate.SetEntryValue(PurchaseModel.TranRecord.PchsDt);
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
            etUnitPrice.SetReadOnly(false);
            etPurchaseQty.SetReadOnly(false);
            etTaxType.SetReadOnly(false);
            etVat.SetReadOnly(true);
            etDCRate.SetReadOnly(false);
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
            if (PurchaseModel.CurrentItemRecord != null)
            {
                PurchaseModel.CurrentItemRecord.TaxTyCd = etTaxType.GetSelectedItem().Id;
                PurchaseModel.CurrentItemRecord.Prc = etUnitPrice.GetEntryValue();
                PurchaseModel.CurrentItemRecord.Qty = etPurchaseQty.GetEntryValue();
                PurchaseModel.CurrentItemRecord.DcRt = etDCRate.GetEntryValue();
                PurchaseModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                PurchaseModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                PurchaseModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                PurchaseModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                PurchaseModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                PurchaseModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;
                PurchaseModel.CalculateCurrentItem();

                etVat.SetEntryValue(PurchaseModel.CurrentItemRecord.TaxAmt.ToString("#,##0.00"));
                etDCAmount.SetEntryValue(PurchaseModel.CurrentItemRecord.DcAmt.ToString("#,##0.00"));
                etPurchasePrice.SetEntryValue(PurchaseModel.CurrentItemRecord.SplyAmt.ToString("#,##0.00"));
                etTotalPrice.SetEntryValue(PurchaseModel.CurrentItemRecord.TotAmt.ToString("#,##0.00"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionSave(object sender, EventArgs e)
        {
            //=======================================================
            // JINIT_20191208, Check항목 추가
            //=======================================================
            // 거래처가 선택되어있지 않으면 오류
            if (string.IsNullOrEmpty(etSupplierID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Supplier.", "OK");
                return;
            }
            // 등록된 ITEM이 없으면 오류
            if (PurchaseModel.GetItemCount() == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please confirm a Item.", "OK");
                return;
            }

            if (etReferenceId.GetEntryValue() > 999999999)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid ReferenceId.", "OK");
                return;
            }
            //=======================================================

            PurchaseModel.TranRecord.OrgInvcNo = etReferenceId.GetEntryValue();
            PurchaseModel.TranRecord.Remark = etRemark.GetEntryValue();

            TrnsPurchaseMaster TrnsPurchaseMaster = new TrnsPurchaseMaster();
            TrnsPurchaseItemMaster TrnsPurchaseItemMaster = new TrnsPurchaseItemMaster();

            PurchaseModel.TranRecord.TaxprNm = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNm;
            PurchaseModel.TranRecord.PchsDt = DateTime.Now.ToString("yyyyMMdd");

            PurchaseModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            PurchaseModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            // 거래 Header, Items
            TrnsPurchaseMaster.InsertTable(PurchaseModel.TranRecord);
            TrnsPurchaseItemMaster.InsertTable(PurchaseModel.ItemRecords);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            trnsPurchaseRraSdcUpload.SendTranPurchaseSave(PurchaseModel.TranRecord.Tin, PurchaseModel.TranRecord.BhfId, PurchaseModel.TranRecord.SpplrTin, PurchaseModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
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
            if (PurchaseModel.CurrentItemRecord != null)
            {
                if (etUnitPrice.GetEntryValue() == 0 || etUnitPrice.GetEntryValue() > 99999999999)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid Unit Price value.", "OK");
                    etUnitPrice.SetFocus();
                    return;
                }
                if (etPurchaseQty.GetEntryValue() == 0 || etPurchaseQty.GetEntryValue() > 99999999)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
                    etPurchaseQty.SetFocus();
                    return;
                }

                if ((etUnitPrice.GetEntryValue() * etPurchaseQty.GetEntryValue()) > 99999999999)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid UnitPrice or PurchaseQty.", "OK");
                    return;
                }

                System.TimeSpan diff1 = etExpireDate.GetEntryValue().Subtract(DateTime.Now);
                if (diff1.Days <= 0)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Product already expired. Please check expiry date", "OK");
                    return;
                }
                if (diff1.Days < 30)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Your item will expire within a month.", "OK");
                }

                if ((etUnitPrice.GetEntryValue() * etPurchaseQty.GetEntryValue()) != 0)
                {
                    PurchaseModel.CurrentItemRecord.SpplrTin = PurchaseModel.TranRecord.SpplrTin;

                    PurchaseModel.CurrentItemRecord.TaxTyCd = etTaxType.GetSelectedItem().Id;
                    PurchaseModel.CurrentItemRecord.Prc = etUnitPrice.GetEntryValue();
                    PurchaseModel.CurrentItemRecord.Qty = etPurchaseQty.GetEntryValue();
                    PurchaseModel.CurrentItemRecord.DcRt = etDCRate.GetEntryValue();

                    PurchaseModel.CurrentItemRecord.ItemExprDt = etExpireDate.GetEntryValue().ToString("yyyyMMdd");

                    PurchaseModel.CalculateCurrentItem();
                    PurchaseModel.ConfirmCurrentItem();
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
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity or price value.", "OK");
                }
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a product.", "OK");
            }
        }

        private void OnFunctionClear(object sender, EventArgs e)
        {
            PurchaseModel.CurrentItemRecord = null;
            
            UpdateItemView(new TrnsPurchaseItemRecord());
            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (TrnsPurchaseItemRecord)e.SelectedItem;
            }
            else
            {
                SelectedItem = null;
            }
        }
        private void OnRemove(object sender, System.EventArgs e)
        {
            if (SelectedItem != null)
            {
                //PurchaseModel.Delete(SelectedItem);
                //listView.ItemsSource = PurchaseModel.GetObservableCollection();

                PurchaseModel.Delete(SelectedItem);
                PurchaseModel.CalculateTran();
                listView.ItemsSource = PurchaseModel.GetObservableCollection();

                UpdateHeaderView();
                UpdateItemView(new TrnsPurchaseItemRecord());

                etUnitPrice.SetReadOnly(true);
                etPurchaseQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
        }

        private void OnEmpty(object sender, System.EventArgs e)
        {
            PurchaseModel.DeleteAll();
            PurchaseModel.CalculateTran();
            listView.ItemsSource = PurchaseModel.GetObservableCollection();

            UpdateHeaderView();
            UpdateItemView(new TrnsPurchaseItemRecord());

            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            if (CustomerRecord == null || string.IsNullOrEmpty(etSupplierID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Customer.", "OK");
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
                        PurchaseModel.SetCurrentItem(popupRecord);
                        UpdateItemView(PurchaseModel.CurrentItemRecord);

                        etUnitPrice.SetReadOnly(false);
                        etPurchaseQty.SetReadOnly(false);
                        etTaxType.SetReadOnly(false);
                        etDCRate.SetReadOnly(false);
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

        async void OnFunctionCustomerID(object sender, EventArgs e)
        {
            var popupPage = new CustomerPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    CustomerRecord = (TaxpayerBhfCustRecord)((ExtEventArgs)ex).EnteredObject;
                    PurchaseModel.TranRecord.SpplrTin = CustomerRecord.CustTin;
                    PurchaseModel.TranRecord.SpplrNm = CustomerRecord.CustNm;
                    PurchaseModel.TranRecord.SpplrBhfId = "00";
                    UpdateHeaderView();

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
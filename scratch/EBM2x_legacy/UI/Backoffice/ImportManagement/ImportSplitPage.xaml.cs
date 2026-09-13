using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.ImportManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportSplitPage : ContentPage
    {
        ImportItemRecord RecordImportItem { get; set; }
        TransactionStockInOutModel StockInOutModel { get; set; }

        StockIoItemRecord SelectedItem = null;

        public ImportSplitPage(ImportItemRecord record)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            RecordImportItem = record;
            
            Init();

            //etSalesQty.GetEntry().Completed += (sender, e) => {
            //    UpdateItemView();
            //};

        }
        public void Init()
        {
            StockIoMaster StockIoMaster = new StockIoMaster();
            // 초기화
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            long SarNo = StockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "A"; // 입력유형 (A:자동, M:수기입력)
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            StockInOutModel = new TransactionStockInOutModel();
            StockInOutModel.CurrentItemRecord = null;
            StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
            StockInOutModel.TranRecord.SarTyCd = "01"; // 재고유형

            UpdateHeaderView();

            UpdateItemView(new StockIoItemRecord());

            if (StockInOutModel.CurrentItemRecord != null)
            {
                UpdateItemView(StockInOutModel.CurrentItemRecord);
                etQuantity.SetReadOnly(false);
                etQuantity.SetFocus();
            }
            else
            {
                UpdateItemView(new StockIoItemRecord());
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        public void UpdateHeaderView()
        {
            etOpCode.SetEntryValue(RecordImportItem.TaskCd);
            etDeclDate.SetEntryValue(RecordImportItem.DclDe);
            etHSCode.SetEntryValue(RecordImportItem.HsCd);
            etPkgQty.SetEntryValue(RecordImportItem.Pkg.ToString("#,##0"));
            etQty.SetEntryValue(RecordImportItem.Qty.ToString("#,##0"));
            etItemDesc.SetEntryValue(RecordImportItem.SupplierItemNm);

            etOpCode.SetReadOnly(true);
            etDeclDate.SetReadOnly(true);
            etHSCode.SetReadOnly(true);
            etPkgQty.SetReadOnly(true);
            etQty.SetReadOnly(true);
            etItemDesc.SetReadOnly(true);
        }
        public void UpdateItemView(StockIoItemRecord itemRecord)
        {
            etItemCode.SetEntryValue(itemRecord.ItemCd);
            etItemName.SetEntryValue(itemRecord.ItemNm);
            etClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etClassName.SetEntryValue(itemRecord.ItemClsNm);
            etCurrentStock.SetEntryValue(itemRecord.RdsQty.ToString());
            etQuantity.SetEntryValue(itemRecord.Qty);
  
            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            etCurrentStock.SetReadOnly(true);
            etQuantity.SetReadOnly(false);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionSave(object sender, System.EventArgs e)
        {
            if(StockInOutModel.ItemRecords.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Processing Fail", "Please fill in the item list.", "OK");
                return;
            }
            string locationTitle21 = UILocation.Instance().GetLocationText("Importation");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to approve this item?");
            var retNet = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (retNet)
            {
                StockIoMaster StockIoMaster = new StockIoMaster();
                StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();
                ImportItemMaster importItemMaster = new ImportItemMaster();

                // 수입품목 검사
                // 수입품목 UPDAYE -> APPROVAL_STATUS_CD = '3'
                string itemCode = StockInOutModel.ItemRecords[0].ItemCd;
                string itemClassCode = StockInOutModel.ItemRecords[0].ItemClsCd;
                string processDate = DateTime.Now.ToString("yyyyMMdd");
                importItemMaster.Update(RecordImportItem.TaskCd, RecordImportItem.DclDe, RecordImportItem.ItemSeq, RecordImportItem.HsCd, "3", itemCode, itemClassCode, UIManager.Instance().UserModel.UserId, UIManager.Instance().UserModel.UserNm, processDate);

                // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq(); 
                StockIoMaster.InsertTable(StockInOutModel.TranRecord);
                StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

                //===>>>>>>>>>
                //JCNA 20191204
                ImportItemRraSdcUpload importItemRraSdcUpload = new ImportItemRraSdcUpload();
                importItemRraSdcUpload.SendImportItemSave(RecordImportItem.TaskCd, RecordImportItem.DclDe, RecordImportItem.ItemSeq);

                //===>>>>>>>>>
                //JCNA 20191204
                StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
                stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);

                //===>>>>>>>>>
                // JCNA 20191204 TR 전송 명령 실행
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Message", "Processing Registered!", "OK");
                if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
            }
        }

        async void OnFunctionClose(object sender, System.EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnFunctionConfirm(object sender, System.EventArgs e)
        {
            if (StockInOutModel.CurrentItemRecord != null)
            {
                //etQuantity.GetEntry().C // Entry 입력 종료처리 필요.
                if (etQuantity.GetEntryValue() != 0)
                {
                    StockInOutModel.CurrentItemRecord.Qty = etQuantity.GetEntryValue();
                    StockInOutModel.CurrentItemRecord.AfterQty = StockInOutModel.CurrentItemRecord.RdsQty + StockInOutModel.CurrentItemRecord.Qty;
                    StockInOutModel.CalculateCurrentItem();
                    StockInOutModel.ConfirmCurrentItem();
                    listView.ItemsSource = StockInOutModel.GetObservableCollection();

                    UpdateHeaderView();
                    UpdateItemView(new StockIoItemRecord());
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
                }
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a product.", "OK");
            }
        }
        private void OnRemove(object sender, System.EventArgs e)
        {
            if (SelectedItem != null)
            {
                StockInOutModel.Delete(SelectedItem);
                listView.ItemsSource = StockInOutModel.GetObservableCollection();

                UpdateHeaderView();
                UpdateItemView(new StockIoItemRecord());
            }
        }

        private void OnEmpty(object sender, System.EventArgs e)
        {
            StockInOutModel.DeleteAll();
            listView.ItemsSource = StockInOutModel.GetObservableCollection();

            UpdateHeaderView();
            UpdateItemView(new StockIoItemRecord());
        }

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            var popupPage = new ItemPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;

                    if (StockInOutModel.IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {
                        StockInOutModel.SetCurrentItem(popupRecord);
                        UpdateItemView(StockInOutModel.CurrentItemRecord);
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

        private void OnSelectedTran(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (StockIoItemRecord)e.SelectedItem;
            }
            else
            {
                SelectedItem = null;
            }
        }
    }
}

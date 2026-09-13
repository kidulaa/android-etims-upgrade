using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.RraSdc.process;
using EBM2x.UI.i18n;
using Newtonsoft.Json;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.PurchaseManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailInformationOfPurchasePage : ContentPage
    {

        ItemClassMaster ItemClassMaster = new ItemClassMaster();
        // 입고 처리
        TransactionStockInOutModel StockInOutModel { get; set; }
        TransactionPurchaseModel PurchaseModel { get; set; }

        TrnsPurchaseItemRecord SelectedItem;

        public DetailInformationOfPurchasePage(TrnsPurchaseRecord recordTran)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            Init(recordTran);

            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnExport.IsVisible = false;
            }
        }

        public void Init(TrnsPurchaseRecord recordTran)
        {
            StockInOutModel = new TransactionStockInOutModel();
            PurchaseModel = new TransactionPurchaseModel();

            PurchaseModel.TranRecord = recordTran;

            UpdateHeaderView();
            UpdateItemView(new TrnsPurchaseItemRecord());
            etUnitPrice.SetReadOnly(true);
            etPurchaseQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);

            StockIoMaster StockIoMaster = new StockIoMaster();
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            long SarNo = StockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "A";
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            StockInOutModel.CurrentItemRecord = null;
            StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderView();
            OnSearchItems(PurchaseModel.TranRecord);

            if (PurchaseModel.CurrentItemRecord != null)
            {
                UpdateItemView(PurchaseModel.CurrentItemRecord);
                etUnitPrice.SetReadOnly(true);
                etPurchaseQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
            else
            {
                UpdateItemView(new TrnsPurchaseItemRecord());
                etUnitPrice.SetReadOnly(true);
                etPurchaseQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }

            etPurchaseDate.SetReadOnly(true);
            etAcceptDate.SetReadOnly(true);
            etCancelDate.SetReadOnly(true);
            etPurchaseDate.SetReadOnly(true);
            etPurchaseDate.SetReadOnly(true);

            // "01"Wait for Accept
            // Wait for Release
            if (PurchaseModel.TranRecord.PchsSttsCd.Equals("01"))
            {
                btnAccept.InvalidateSurfaceSetDisabled(false);
                btnCancel.InvalidateSurfaceSetDisabled(false);
                btnTransfer.InvalidateSurfaceSetDisabled(true);
                btnTransfer.IsVisible = false;
                btnCancelSave.InvalidateSurfaceSetDisabled(true);
                btnCancelSave.IsVisible = false;
                btnTransferSave.InvalidateSurfaceSetDisabled(true);
                btnTransferSave.IsVisible = false;
            }

            // "02"'Accepted
            if (PurchaseModel.TranRecord.PchsSttsCd.Equals("02"))
            {
                btnAccept.InvalidateSurfaceSetDisabled(true);
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnTransfer.InvalidateSurfaceSetDisabled(true);
                btnTransfer.IsVisible = false;
                btnCancelSave.InvalidateSurfaceSetDisabled(true);
                btnCancelSave.IsVisible = false;
                btnTransferSave.InvalidateSurfaceSetDisabled(true);
                btnTransferSave.IsVisible = false;
            }

            // "04"'Canceled
            if (PurchaseModel.TranRecord.PchsSttsCd.Equals("04"))
            {
                btnAccept.InvalidateSurfaceSetDisabled(true);
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnTransfer.InvalidateSurfaceSetDisabled(false);
                btnTransfer.IsVisible = false;
                btnCancelSave.InvalidateSurfaceSetDisabled(true);
                btnCancelSave.IsVisible = false;
                btnTransferSave.InvalidateSurfaceSetDisabled(true);
                btnTransferSave.IsVisible = false;
            }

            // "06"'Transfer
            if (PurchaseModel.TranRecord.PchsSttsCd.Equals("06"))
            {
                btnAccept.InvalidateSurfaceSetDisabled(true);
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnCancel.IsVisible = false;
                btnTransfer.InvalidateSurfaceSetDisabled(true);
                btnCancelSave.InvalidateSurfaceSetDisabled(true);
                btnCancelSave.IsVisible = false;
                btnTransferSave.InvalidateSurfaceSetDisabled(true);
                btnTransferSave.IsVisible = false;
            }

            /*
            If GblBrcCod <> "00" Or strRegTyCd = "M" Then
                btnTransfer.Visible = False
            Else
                btnTransfer.Visible = True
            End If
            */
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        public void UpdateHeaderView()
        {
            etCurrentStatus.SetReadOnly(true);
            etInvoieID.SetReadOnly(true);
            etSupplierID.SetReadOnly(true);
            etSupplierName.SetReadOnly(true);
            etTotalAmount.SetReadOnly(true);
            etTotalVAT.SetReadOnly(true);

            etTotalDCAmount.SetReadOnly(true);
            etTotalSupplyAmount.SetReadOnly(true);

            etAcceptDate.SetReadOnly(true);
            etCancelRequest.SetReadOnly(true);
            etCancelDate.SetReadOnly(true);
            etRefund.SetReadOnly(true);

            etRemark.SetReadOnly(true);

            //etPurchase.SetEntryValue("Purchase");

            etCurrentStatus.SetEntryValue(PurchaseModel.TranRecord.PchsSttsNm);
            etInvoieID.SetEntryValue(PurchaseModel.TranRecord.InvcNo.ToString("#,##0"));
            etSupplierID.SetEntryValue(PurchaseModel.TranRecord.SpplrTin);
            etSupplierName.SetEntryValue(PurchaseModel.TranRecord.SpplrNm);
            etPurchaseDate.SetEntryValue(PurchaseModel.TranRecord.PchsDt);
            etTotalAmount.SetEntryValue(PurchaseModel.TranRecord.TotAmt.ToString("#,##0.00"));
            etTotalVAT.SetEntryValue(PurchaseModel.TranRecord.TotTaxAmt.ToString("#,##0.00"));

            etTotalDCAmount.SetEntryValue(PurchaseModel.GetTotDCAmount().ToString("#,##0.00"));
            etTotalSupplyAmount.SetEntryValue(PurchaseModel.TranRecord.TotAmt.ToString("#,##0.00"));

            etAcceptDate.SetEntryValue(PurchaseModel.TranRecord.CfmDt);
            etCancelRequest.SetEntryValue(PurchaseModel.TranRecord.CnclReqDt);
            etCancelDate.SetEntryValue(PurchaseModel.TranRecord.CnclDt);
            etRefund.SetEntryValue(PurchaseModel.TranRecord.RfdDt);

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
            etUnitPrice.SetEntryValue(itemRecord.Prc.ToString("#,##0.00"));
            etPurchaseQty.SetEntryValue(itemRecord.Qty.ToString("#,##0"));
            etTaxType.SetEntryValue(itemRecord.TaxTyNm);
            etVat.SetEntryValue(itemRecord.TaxAmt.ToString("#,##0.00"));
            etDCRate.SetEntryValue(itemRecord.DcRt.ToString("#,##0"));
            etDCAmount.SetEntryValue(itemRecord.DcAmt.ToString("#,##0.00"));
            etPurchasePrice.SetEntryValue(itemRecord.SplyAmt.ToString("#,##0.00"));
            etTotalPrice.SetEntryValue(itemRecord.TotAmt.ToString("#,##0.00"));

            if(string.IsNullOrEmpty(itemRecord.ItemExprDt))
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
            if (itemRecord != null && !string.IsNullOrEmpty(itemRecord.ItemCd))
            {
                //double RdsQty = StockMasterMaster.GetCurrentStock(itemRecord.Tin, itemRecord.ItemCd); // 현재고 수량
                //etCurrentStock.SetEntryValue(RdsQty.ToString("#,##0.00"));
                string className = ItemClassMaster.GetItemClassName(itemRecord.ItemClsCd);
                etClassName.SetEntryValue(className);
            }
            else
            {
                //etCurrentStock.SetEntryValue("");
                etClassName.SetEntryValue("");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton 
        }

        async void OnFunctionExport(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (PurchaseModel.ItemRecords == null || PurchaseModel.ItemRecords.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(PurchaseModel.ItemRecords);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TrnsPurchaseItem.xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnSearchItems(TrnsPurchaseRecord record)
        {
            TrnsPurchaseItemMaster masterItems = new TrnsPurchaseItemMaster();
            PurchaseModel.ItemRecords = masterItems.getTrnsPurchaseItemTable(record.Tin, record.BhfId, record.SpplrTin, record.InvcNo);
            listView.ItemsSource = PurchaseModel.GetObservableCollection();
            if (PurchaseModel.ItemRecords.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        private void OnSelectedTran(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (TrnsPurchaseItemRecord)e.SelectedItem;
                UpdateItemView(SelectedItem);
            }
        }

        async void OnFunctionAccept(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the Purchase " + "Approved" + " Invoice?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            TrnsPurchaseMaster trnsPurchaseMaster = new TrnsPurchaseMaster();
            PurchaseModel.TranRecord.CfmDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            PurchaseModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            PurchaseModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            trnsPurchaseMaster.UpdateTable("02", PurchaseModel.TranRecord);

            StockInOutModel.TranRecord.SarTyCd = "02"; 
            StockInOutModel.TranRecord.TaxprNm = PurchaseModel.TranRecord.TaxprNm;
            StockInOutModel.TranRecord.CustTin = PurchaseModel.TranRecord.SpplrTin;
            StockInOutModel.TranRecord.CustNm = PurchaseModel.TranRecord.SpplrNm;
            StockInOutModel.TranRecord.CustBhfId = PurchaseModel.TranRecord.SpplrBhfId;

            StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            for (int i = 0; i < PurchaseModel.ItemRecords.Count; i++)
            {
                TrnsPurchaseItemRecord itemNode = PurchaseModel.ItemRecords[i];

                UpdateItemTable(itemNode);
                
                TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                bool ret = taxpayerItemMaster.ToRecord(itemRecord, StockInOutModel.TranRecord.Tin, itemNode.ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);

                StockInOutModel.SetCurrentItem(itemRecord);

                StockInOutModel.CurrentItemRecord.Prc = itemNode.Prc;
                StockInOutModel.CurrentItemRecord.Qty = itemNode.Qty;
                StockInOutModel.CurrentItemRecord.TotDcAmt = itemNode.DcAmt;
                StockInOutModel.CurrentItemRecord.ItemExprDt = itemNode.ItemExprDt;

                StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                StockInOutModel.CalculateCurrentItem();
                StockInOutModel.ConfirmCurrentItem();
            }

            StockIoMaster StockIoMaster = new StockIoMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
            StockIoMaster.InsertTable(StockInOutModel.TranRecord);
            StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            trnsPurchaseRraSdcUpload.SendTranPurchaseSave(PurchaseModel.TranRecord.Tin, PurchaseModel.TranRecord.BhfId, PurchaseModel.TranRecord.SpplrTin, PurchaseModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            //JCNA 20191204
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        private void OnFunctionCancel(object sender, EventArgs e)
        {
            //lblCancelType.Text = "Cancel Type";
            //cboCancelType.Visible = True;

            btnCancelSave.InvalidateSurfaceSetDisabled(false);
            btnCancelSave.IsVisible = true;
        }


        private void OnFunctionTransfer(object sender, EventArgs e)
        {
            btnTransferSave.InvalidateSurfaceSetDisabled(false);
            btnTransferSave.IsVisible = true;
        }

        async void OnFunctionCancelSave(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the Purchase " + "Canceled" + " Invoice?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            

            TrnsPurchaseMaster trnsPurchaseMaster = new TrnsPurchaseMaster();
            PurchaseModel.TranRecord.CnclDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            PurchaseModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            PurchaseModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            trnsPurchaseMaster.UpdateTable("04", PurchaseModel.TranRecord);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            trnsPurchaseRraSdcUpload.SendTranPurchaseSave(PurchaseModel.TranRecord.Tin, PurchaseModel.TranRecord.BhfId, PurchaseModel.TranRecord.SpplrTin, PurchaseModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        async void OnFunctionTransferSave(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the Purchase " + "Transfered" + " Invoice?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            /*
            strInvStatusCd = "06"
            strMsg = "Transfered"
            strCancelRequestedDate = Format(dtpAcceptDate.Value, "yyyyMMddHHmmss")        'Accept Date
            strOcdt1 = Format(dtpAcceptDate.Value, "ddMMyyyy")   
            */

            TrnsPurchaseMaster trnsPurchaseMaster = new TrnsPurchaseMaster();
            PurchaseModel.TranRecord.CnclReqDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //PurchaseModel.TranRecord.BhfId = "";    // 선택한
            //PurchaseModel.TranRecord.RegTyCd = "";  // 'T'

            PurchaseModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            PurchaseModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            PurchaseModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            PurchaseModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            trnsPurchaseMaster.UpdateTable("06", PurchaseModel.TranRecord);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            trnsPurchaseRraSdcUpload.SendTranPurchaseSave(PurchaseModel.TranRecord.Tin, PurchaseModel.TranRecord.BhfId, PurchaseModel.TranRecord.SpplrTin, PurchaseModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        async void OnFunctionConfirm(object sender, EventArgs e)
        {
            if(SelectedItem == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Item.", "OK");
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
            SelectedItem.ItemExprDt = etExpireDate.GetEntryValue().ToString("yyyyMMdd");

            TrnsPurchaseItemMaster trnsPurchaseItemMaster = new TrnsPurchaseItemMaster();
            trnsPurchaseItemMaster.UpdateItemExprDt(SelectedItem);

            //listView.ItemsSource = PurchaseModel.GetObservableCollection();
            //UpdateItemView(new TrnsPurchaseItemRecord());

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            trnsPurchaseRraSdcUpload.SendTranPurchaseSave(PurchaseModel.TranRecord.Tin, PurchaseModel.TranRecord.BhfId, PurchaseModel.TranRecord.SpplrTin, PurchaseModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
        }

        public bool UpdateItemTable(TrnsPurchaseItemRecord trnsPurchaseSalesItem)
        {
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            TaxpayerItemMaster master = new TaxpayerItemMaster();
            TaxpayerItemRecord custRecord = new TaxpayerItemRecord();

            bool IsExist = master.GetItemCode(tin, trnsPurchaseSalesItem.ItemCd);
            if (IsExist) return true;

            custRecord.Tin = tin;                  // Taxpayer Identification Number(TIN)
            custRecord.ItemCd = trnsPurchaseSalesItem.ItemCd;               // Item Code
            custRecord.ItemClsCd = trnsPurchaseSalesItem.ItemClsCd;            // Item Classification Code (RRA)
            custRecord.ItemTyCd = trnsPurchaseSalesItem.ItemCd.Substring(2, 1);             // Item Type Code
            custRecord.ItemNm = trnsPurchaseSalesItem.ItemNm;               // Item Name
            custRecord.ItemStdNm = trnsPurchaseSalesItem.ItemNm;            // Item Stand Name
            custRecord.OrgnNatCd = trnsPurchaseSalesItem.ItemCd.Substring(0, 2);            // Origin National Code
            custRecord.PkgUnitCd = trnsPurchaseSalesItem.PkgUnitCd;            // Package Unit Code
            custRecord.QtyUnitCd = trnsPurchaseSalesItem.QtyUnitCd;            // Quantity Unit Code
            custRecord.TaxTyCd = trnsPurchaseSalesItem.TaxTyCd;              // Taxation Type Code
            custRecord.Bcd = trnsPurchaseSalesItem.Bcd;                // Barcode
            custRecord.RegBhfId = bhfid;        // Branch Office ID
            custRecord.UseYn = "Y";             // Use(Y/N)
            custRecord.RraModYn = "Y";          // RRA Modified(Y/N)
            custRecord.AddInfo = "";            // Additional Information
            custRecord.SftyQty = 0;             // Safety Quantity
            custRecord.IsrcAplcbYn = "N";        // Insurance Appicable(Y/N)
            custRecord.DftPrc = 0;              // Default Price
            custRecord.GrpPrcL1 = 0;            // Group Default Price L1
            custRecord.GrpPrcL2 = 0;            // Group Default Price L2
            custRecord.GrpPrcL3 = 0;            // Group Default Price L3
            custRecord.GrpPrcL4 = 0;            // Group Default Price L4
            custRecord.GrpPrcL5 = 0;            // Group Default Price L5
            custRecord.RegrId = "";     // Registrant ID
            custRecord.RegrNm = "";       // Registrant Name
            custRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");      // Registered Date
            custRecord.ModrId = "";     // Modifier ID
            custRecord.ModrNm = "";       // Modifier Name
            custRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");      // Modified Date
            custRecord.InitlWhUntpc = trnsPurchaseSalesItem.Prc;             
            custRecord.InitlQty = 0;            
            custRecord.Rm = "";                 
            custRecord.UseBarcode = string.IsNullOrEmpty(trnsPurchaseSalesItem.Bcd) ? "N" : "Y";         // 바코드사용여부
            custRecord.UseAdiYn = "N";          
            custRecord.BatchNum = "";           // BatchNum

            StockMasterRecord stockRecord = new StockMasterRecord();
            stockRecord.Tin = custRecord.Tin;
            stockRecord.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            stockRecord.ItemCd = custRecord.ItemCd;
            stockRecord.RsdQty = 0;
            stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.ModrId = "";
            stockRecord.ModrNm = "";
            stockRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.RegrId = "";
            stockRecord.RegrNm = "";

            custRecord.UpdateNull();
            stockRecord.UpdateNull();
            bool ret = master.ToTable(custRecord, stockRecord);
            if (ret)
            {
                StockMasterRraSdcUpload stockMasterRraSdcUpload = new StockMasterRraSdcUpload();
                stockMasterRraSdcUpload.SendStockMasterSave(stockRecord.Tin, stockRecord.BhfId, stockRecord.ItemCd);

                ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
                itemRraSdcUpload.SendItemSave(custRecord.Tin, custRecord.ItemCd);
            }

            return true;
        }
    }
}

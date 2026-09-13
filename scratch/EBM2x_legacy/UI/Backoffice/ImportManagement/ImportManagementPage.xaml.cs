using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.ImportManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportManagementPage : ContentPage
    {
        public ObservableCollection<ImportItemRecord> lvItemManagement { get; set; }
        List<ImportItemRecord> listImportItemRecord;
        ImportItemRecord currentRecord = null;
        ImportItemMaster importItemMaster;
        TransactionStockInOutModel StockInOutModel { get; set; }

        bool flagCheckbox = true;

        public ImportManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            importItemMaster = new ImportItemMaster();

            DateTime today = DateTime.Now;
            DateTime answer = today.AddDays(-7);

            etFromDate.SetDateTime(answer);
            etToDate.SetDateTime(today);

            etFromDate.GetDatePicker().DateSelected += (sender, e) => {
                if (etFromDate.GetDateTime() > etToDate.GetDateTime())
                {
                    etToDate.SetEntryValue(etFromDate.GetDateTime());
                }
            };
            etToDate.GetDatePicker().DateSelected += (sender, e) => {
                if (etFromDate.GetDateTime() > etToDate.GetDateTime())
                {
                    etFromDate.SetEntryValue(etToDate.GetDateTime());
                }
            };

            etPKGQtyCheckbox.CheckedChanged += (sender, e) => {
                if(etPKGQtyCheckbox.IsChecked)
                {
                    if(flagCheckbox) etQtyCheckbox.IsChecked = false;
                }
                else
                {
                    if (flagCheckbox) etQtyCheckbox.IsChecked = true;
                }
            };
            etQtyCheckbox.CheckedChanged += (sender, e) => {
                if (etQtyCheckbox.IsChecked)
                {
                    if (flagCheckbox) etPKGQtyCheckbox.IsChecked = false;
                }
                else
                {
                    if (flagCheckbox) etPKGQtyCheckbox.IsChecked = true;
                }
            };

            SetEntityData(new ImportItemRecord(), false, true);
            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnExportVAT.IsVisible = false;
                btnExport.IsVisible = false;
            }

            etOpCode.SetReadOnly(true);
            etITEMDesc.SetReadOnly(true);
            etApprovalStatus.SetReadOnly(true);

            etDeclDate.SetReadOnly(true);
            etSupplier.SetReadOnly(true);
            etApprovalCancelDate.SetReadOnly(true);

            etSeq.SetReadOnly(true);
            etAgent.SetReadOnly(true);

            etHscode.SetReadOnly(true);
            etTaxPayerName.SetReadOnly(true);

            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);

            etOrigin.SetReadOnly(true);
            etGrossWT.SetReadOnly(true);

            etExport.SetReadOnly(true);
            etNetWT.SetReadOnly(true);

            etPKGQty.SetReadOnly(true);
            etInvoiceAMT.SetReadOnly(true);

            etQty.SetReadOnly(true);
            etInvoiceCurrency.SetReadOnly(true);

            etUnit.SetReadOnly(true);
            etRate.SetReadOnly(true);

            flagCheckbox = false;
            etPKGQtyCheckbox.IsChecked = false;
            etQtyCheckbox.IsChecked = false;
            flagCheckbox = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            if (listImportItemRecord == null || listImportItemRecord.Count <= 0)
            {
                OnSearch();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionExportVAT(object sender, System.EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            List<ImportItemRecordVAT> list = importItemMaster.getImportItemTableVAT(fromStr, toStr);

            if (list == null || list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
            else
            {
                List<string> titleList = new List<string>();

                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(list);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);
                    //createExcelUtil.CreateSpreadsheetWorkbook(stream, titleList, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string filename = "VatImportReport_" + dt + ".xlsx";

                    await iSave.SaveAndView(filename, "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }
        async void OnFunctionExport(object sender, System.EventArgs e)
        {
            if (listImportItemRecord == null || listImportItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listImportItemRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("ImportItem_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionClose(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnSearch(object sender, System.EventArgs e)
        {
            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            string valueSupplierName = etSupplierName.GetEntryValue();
            string valueImptItemStts = etImptItemStts.GetSelectedItem().Id;
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listImportItemRecord = importItemMaster.getImportItemTable(fromStr, toStr, valueSupplierName, valueImptItemStts);
            SetList(listImportItemRecord);
            if (listImportItemRecord == null || listImportItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }

            currentRecord = null;
            SetEntityData(new ImportItemRecord(), false, true);

            flagCheckbox = false;
            etPKGQtyCheckbox.IsChecked = false;
            etQtyCheckbox.IsChecked = false;
            flagCheckbox = true;
        }

        void OnSearch()
        {
            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            string valueSupplierName = etSupplierName.GetEntryValue();
            string valueImptItemStts = etImptItemStts.GetSelectedItem().Id;
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listImportItemRecord = importItemMaster.getImportItemTable(fromStr, toStr, valueSupplierName, valueImptItemStts);
            SetList(listImportItemRecord);

            currentRecord = null;
            SetEntityData(new ImportItemRecord(), false, true);

            flagCheckbox = false;
            etPKGQtyCheckbox.IsChecked = false;
            etQtyCheckbox.IsChecked = false;
            flagCheckbox = true;
        }

        void SetList(List<ImportItemRecord> datas)
        {
            try
            {
                lvItemManagement = new ObservableCollection<ImportItemRecord>();
                listView.ItemsSource = lvItemManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvItemManagement.Add(datas[i]);
                }
            }
            catch
            {
            }
        }

        async void OnFunctionApprove(object sender, System.EventArgs e)
        {
            if(etPKGQtyCheckbox.IsChecked == false && etQtyCheckbox.IsChecked == false)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select a PKGQty or Qty.", "OK");
                return;
            }

            if (currentRecord == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select a import transaction.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(etItemCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value of [Item code] cannot be empty.", "Ok");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Info");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to approve this item?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (result)
            {
                TaxpayerItemMaster itemMaster = new TaxpayerItemMaster();
                StockIoMaster stockIoMaster = new StockIoMaster();
                StockIoItemMaster stockIoItemMaster = new StockIoItemMaster();
                ImportItemMaster importItemMaster = new ImportItemMaster();

                // 수입품목 검사
                bool isExist = importItemMaster.IsExist(currentRecord.TaskCd, currentRecord.DclDe, currentRecord.ItemSeq, currentRecord.HsCd);
                if (isExist)
                {
                    // 수입품목 UPDAYE -> APPROVAL_STATUS_CD = '3'
                    string itemCode = etItemCode.GetEntryValue();
                    TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                    itemMaster.ToRecord(itemRecord, currentRecord.Tin, itemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                    string itemClassCode = itemRecord.ItemClsCd;
                    string processDate = DateTime.Now.ToString("yyyyMMdd");
                    importItemMaster.Update(currentRecord.TaskCd, currentRecord.DclDe, currentRecord.ItemSeq, currentRecord.HsCd, "3", itemCode, itemClassCode, UIManager.Instance().UserModel.UserId, UIManager.Instance().UserModel.UserNm, processDate);

                    // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                    string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                    long SarNo = stockIoMaster.GetStockIoSeq();
                    string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
                    string RegTyCd = "A"; // 입력유형 (A:자동, M:수기입력)
                    string UserId = UIManager.Instance().UserModel.UserId;
                    string UserNm = UIManager.Instance().UserModel.UserNm;

                    StockInOutModel = new TransactionStockInOutModel();
                    StockInOutModel.CurrentItemRecord = null;
                    StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
                    StockInOutModel.TranRecord.SarTyCd = "01"; // 재고유형, 재고유형코드(12: 01(수입매입))

                    StockInOutModel.SetCurrentItem(itemRecord);
                    //StockInOutModel.CurrentItemRecord.Qty = etAdjustQty.GetEntryValue();
                    if (etPKGQtyCheckbox.IsChecked)
                    {
                        StockInOutModel.CurrentItemRecord.Qty = currentRecord.Pkg;
                    }
                    else
                    {
                        StockInOutModel.CurrentItemRecord.Qty = currentRecord.Qty;
                    }
                    StockInOutModel.CurrentItemRecord.AfterQty = StockInOutModel.CurrentItemRecord.RdsQty + StockInOutModel.CurrentItemRecord.Qty;

                    StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    StockInOutModel.CalculateCurrentItem();
                    StockInOutModel.ConfirmCurrentItem();

                    // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    // stockIoItemMaster에서 재고 마스터에도 반영함, StockMaster에도 반영됨,
                    StockInOutModel.TranRecord.SarNo = stockIoMaster.GetStockIoSeq();
                    stockIoMaster.InsertTable(StockInOutModel.TranRecord);
                    stockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

                    //===>>>>>>>>>
                    //JCNA 20191204
                    ImportItemRraSdcUpload importItemRraSdcUpload = new ImportItemRraSdcUpload();
                    importItemRraSdcUpload.SendImportItemSave(currentRecord.TaskCd, currentRecord.DclDe, currentRecord.ItemSeq);

                    //===>>>>>>>>>
                    //JCNA 20191204
                    StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
                    stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);

                    //===>>>>>>>>>
                    // JCNA 20191204 TR 전송 명령 실행
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();

                    OnSearch();
                }
            }
        }

        async void OnFunctionSplit(object sender, System.EventArgs e)
        {
            if (currentRecord == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select a import transaction.", "OK");
                return;
            }

            var popupPage = new ImportSplitPage(currentRecord);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    OnSearch();
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        async void OnFunctionCancel(object sender, System.EventArgs e)
        {
            if (currentRecord == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select a import transaction.", "OK");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Info");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to cancel this item?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if(result)
            {
                ImportItemMaster importItemMaster = new ImportItemMaster();
                // 수입품목 검사
                bool isExist = importItemMaster.IsExist(currentRecord.TaskCd, currentRecord.DclDe, currentRecord.ItemSeq, currentRecord.HsCd);
                if (isExist)
                {
                    // 수입품목 UPDAYE -> APPROVAL_STATUS_CD = '4'
                    string itemCode = etItemCode.GetEntryValue();
                    TaxpayerItemMaster itemMaster = new TaxpayerItemMaster();
                    TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                    itemMaster.ToRecord(itemRecord, currentRecord.Tin, itemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                    string itemClassCode = itemRecord.ItemClsCd;
                    string processDate = DateTime.Now.ToString("yyyyMMdd");
                    importItemMaster.Update(currentRecord.TaskCd, currentRecord.DclDe, currentRecord.ItemSeq, currentRecord.HsCd, "4", itemCode, itemClassCode, UIManager.Instance().UserModel.UserId, UIManager.Instance().UserModel.UserNm, processDate);

                    //===>>>>>>>>>
                    //JCNA 20191204
                    ImportItemRraSdcUpload importItemRraSdcUpload = new ImportItemRraSdcUpload();
                    importItemRraSdcUpload.SendImportItemSave(currentRecord.TaskCd, currentRecord.DclDe, currentRecord.ItemSeq);

                    //===>>>>>>>>>
                    // JCNA 20191204 TR 전송 명령 실행
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();

                    OnSearch();
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Canceled successfully.", "OK");
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to Cancel.", "OK");
                }
            }
        }

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            if (currentRecord == null || !currentRecord.ImptItemSttsCd.Equals("2"))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select a import transaction.", "OK");
                return;
            }

            var popupPage = new ItemPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;
                    currentRecord.ItemCd = popupRecord.ItemCd;
                    currentRecord.ItemNm = popupRecord.ItemNm;
                    currentRecord.ItemClsCd = popupRecord.ItemCd;
                    etItemCode.SetEntryValue(currentRecord.ItemCd);
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

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                currentRecord = (ImportItemRecord)e.SelectedItem;
                currentRecord.ModrNm = UIManager.Instance().UserModel.UserNm;

                if (currentRecord.ImptItemSttsCd.Equals("2"))
                {
                    flagCheckbox = false;
                    etPKGQtyCheckbox.IsChecked = false;
                    etQtyCheckbox.IsChecked = false;
                    flagCheckbox = true; 
                    SetEntityData(currentRecord, true, false);
                }
                else
                {
                    flagCheckbox = false;
                    etPKGQtyCheckbox.IsChecked = false;
                    etQtyCheckbox.IsChecked = false;
                    flagCheckbox = true;
                    SetEntityData(currentRecord, true, true);
                }
            }
        }

        private void SetEntityData(ImportItemRecord record, bool readOnly, bool buttonDisabled)
        {
            etOpCode.SetEntryValue(record.TaskCd);
            etITEMDesc.SetEntryValue(record.ItemNm);
            etApprovalStatus.SetEntryValue(record.ImptItemSttsNm);

            etDeclDate.SetEntryValue(record.DclDe);
            etSupplier.SetEntryValue(record.SpplrNm);

            etApprovalCancelDate.SetEntryValue("");
            if(!record.ImptItemSttsCd.Equals("2"))
            {
                etApprovalCancelDate.SetEntryValue(record.ModDt);
            }

            etSeq.SetEntryValue(record.ItemSeq.ToString());
            etAgent.SetEntryValue(record.AgntNm);

            etHscode.SetEntryValue(record.HsCd);
            etTaxPayerName.SetEntryValue(record.TaxprNm);

            etItemCode.SetEntryValue(record.ItemCd);
            etItemName.SetEntryValue(record.ItemNm);

            etOrigin.SetEntryValue(record.OrgnNatNm);
            etGrossWT.SetEntryValue(record.TotWt.ToString());

            etExport.SetEntryValue(record.ExptNatNm);
            etNetWT.SetEntryValue(record.NetWt.ToString());

            etPKGQty.SetEntryValue(record.Pkg.ToString());
            etInvoiceAMT.SetEntryValue(record.InvcFcurAmt.ToString());

            etQty.SetEntryValue(record.Qty.ToString());
            etInvoiceCurrency.SetEntryValue("");

            etUnit.SetEntryValue(record.QtyUnitNm);
            etRate.SetEntryValue(record.InvcFcurExcrt.ToString());

            btnApprove.InvalidateSurfaceSetDisabled(buttonDisabled);
            btnSplit.InvalidateSurfaceSetDisabled(buttonDisabled);
            btnCancel.InvalidateSurfaceSetDisabled(buttonDisabled);
            //btnItemCode.InvalidateSurfaceSetDisabled(buttonDisabled);
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etImptItemStts.SetSelecteItem(new SystemCode());
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.i18n;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StockAdjustmentPage : ContentPage
    {
        TaxpayerItemMaster master = null;
        TaxpayerItemRecord record = null;
        TransactionStockInOutModel StockInOutModel { get; set; }

        string ItemCode = "";
        public StockAdjustmentPage(string itemCode)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            ItemCode = itemCode;

            master = new TaxpayerItemMaster();
            record = new TaxpayerItemRecord();

            Init();
                
            record.clear();
            SetEntityData(record, false);

            etCurrentQty.SetReadOnly(true);
            etQtyAfter.SetReadOnly(true);
            etBeforeLocation.SetReadOnly(true);
            etAfterLocation.SetReadOnly(true);

            etAdjustQty.GetEntry().Completed += (sender, e) => {
                string id = etAdjustType.GetSelectedItem().Id;
                if (string.IsNullOrEmpty(id))
                {
                    etAdjustQty.SetEntryValue(0);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select Adjust Type.", "OK");
                    return;
                }
                switch (id)
                {
                    case "01": // 01 : 창고이동출고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                        break;
                    case "02": // 02 : 창고이동입고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() + etAdjustQty.GetEntryValue());
                        break;
                    case "03": // 03 : 재고조정입고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() + etAdjustQty.GetEntryValue());
                        break;
                    case "04": // 04 : 재고조정출고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                        break;
                    case "05": // 05 : 폐기출고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                        break;
                    case "06": // 06 : 재가공입고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() + etAdjustQty.GetEntryValue());
                        break;
                    case "07": // 07 : 재가공출고
                        etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                        break;
                }
            };
            etAdjustType.GetPicker().SelectedIndexChanged += (sender, e) => {
                ChangeAdjustType();
            };
            etAfterLocation.GetPicker().SelectedIndexChanged += (sender, e) => {
                string id = etAdjustType.GetSelectedItem().Id;
                if (!string.IsNullOrEmpty(id)) //  && (id.Equals("01") || id.Equals("02"))
                {
                    ChangeAfterLocation();
                }
                else
                {
                    etAdjustQty.SetEntryValue(0);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select Adjust Type.", "OK");
                }
            };

            etReason.GetEntry().MaxLength = 400;

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
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            master.ToRecord(record, tin, ItemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
            SetEntityData(record, false);

            etItemCode.SetReadOnly(true);
            etItemCode.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);

            etItemName.SetReadOnly(true);
            etUseBarcode.SetReadOnly(true);
            etBarcode.SetReadOnly(true);

            etOrigin.SetReadOnly(true);
            etInsuranceYN.SetReadOnly(true);
            etL1SalePrice.SetReadOnly(true);

            etItemType.SetReadOnly(true);
            etPkgUnit.SetReadOnly(true);
            etQtyUnit.SetReadOnly(true);
            etL2SalePrice.SetReadOnly(true);

            etPurchasePrice.SetReadOnly(true);
            etSalePrice.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etL3SalePrice.SetReadOnly(true);

            etBeginningStock.SetReadOnly(true);
            etCurrentStock.SetReadOnly(true);
            etSafetyStock.SetReadOnly(true);
            etL4SalePrice.SetReadOnly(true);

            etDescription.SetReadOnly(true);
            etUse.SetReadOnly(true);
            etL5SalePrice.SetReadOnly(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void ChangeAdjustType()
        {

            etAfterLocation.SetReadOnly(true);
            string id = etAdjustType.GetSelectedItem().Id;
            switch(id)
            {
                case "01": // 01 : 창고이동출고
                    StockInOutModel.TranRecord.SarTyCd = "13"; // 재고유형 - 13 : 재고이동출고 
                    //dblUntpc = dblSalePrice;
                    etAfterLocation.SetReadOnly(false);
                    break;
                case "02": // 02 : 창고이동입고
                    StockInOutModel.TranRecord.SarTyCd = "04"; // 재고유형 - 04 : 재고이동입고
                    //dblUntpc = dblPurchasePrice;
                    etAfterLocation.SetReadOnly(false);
                    break;
                case "03": // 03 : 재고조정입고
                    StockInOutModel.TranRecord.SarTyCd = "06"; // 재고유형 - 06 : 재고조정입고
                    //dblUntpc = dblPurchasePrice;
                    break;
                case "04": // 04 : 재고조정출고
                    StockInOutModel.TranRecord.SarTyCd = "16"; // 재고유형 - 16 : 재고조정출고
                    //dblUntpc = dblSalePrice;
                    break;
                case "05": // 05 : 폐기출고
                    StockInOutModel.TranRecord.SarTyCd = "15"; // 재고유형 - 15 : 폐기출고
                    //dblUntpc = dblSalePrice;
                    break;
                case "06": // 06 : 재가공입고
                    StockInOutModel.TranRecord.SarTyCd = "05"; // 재고유형 - 05 : 재가공입고 
                    //// dblUntpc = dblPurchasePrice
                    Navigation.InsertPageBefore(new ProcessingManagementPage(), this);
                    await Navigation.PopAsync();
                    break;
                case "07": // 07 : 재가공출고
                    StockInOutModel.TranRecord.SarTyCd = "14"; // 재고유형 - 14 : 재가공출고
                                                               //// dblUntpc = dblSalePrice
                    Navigation.InsertPageBefore(new ProcessingManagementPage(), this);
                    await Navigation.PopAsync();
                    break;
            }
        }

        async void ChangeAfterLocation()
        {
            string id = etAfterLocation.GetSelectedItem().Id;
            //if (!id.Equals("00"))
            //{
                // Stock movement page
                Navigation.InsertPageBefore(new StockMovementPage(id), this);
                await Navigation.PopAsync();
            //}
        }

        async void OnSearch(object sender, EventArgs e)
        {
        }

        // 저장
        async void OnFunctionSave(object sender, EventArgs e)
        {
            string id = etAdjustType.GetSelectedItem().Id;
            if (string.IsNullOrEmpty(id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select Adjust Type.", "OK");
                return;
            }

            switch (id)
            {
                case "01": // 01 : 창고이동출고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                    break;
                case "02": // 02 : 창고이동입고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() + etAdjustQty.GetEntryValue());
                    break;
                case "03": // 03 : 재고조정입고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() + etAdjustQty.GetEntryValue());
                    break;
                case "04": // 04 : 재고조정출고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                    break;
                case "05": // 05 : 폐기출고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                    break;
                case "06": // 06 : 재가공입고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() + etAdjustQty.GetEntryValue());
                    break;
                case "07": // 07 : 재가공출고
                    etQtyAfter.SetEntryValue(etCurrentQty.GetEntryValue() - etAdjustQty.GetEntryValue());
                    break;
            }

            if (etAdjustQty.GetEntryValue() == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please input Adjust Qty.", "OK");
                return;
            }
            if (etQtyAfter.GetEntryValue() < 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "The Qty after is negative. Please check again.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(etReason.GetEntryValue()) || etReason.GetEntryValue().Length < 10)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a reason adjustment of 10 or more characters.", "OK");
                return;
            }

            StockInOutModel.SetCurrentItem(record);
            StockInOutModel.CurrentItemRecord.Qty = etAdjustQty.GetEntryValue();
            StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            StockInOutModel.ConfirmCurrentItem();

            StockIoMaster StockIoMaster = new StockIoMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();

            switch (id)
            {
                case "01": // 01 : 창고이동출고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;
                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
                case "02": // 02 : 창고이동입고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;
                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
                case "03": // 03 : 재고조정입고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
                case "04": // 04 : 재고조정출고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
                case "05": // 05 : 폐기출고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
                case "06": // 06 : 재가공입고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
                case "07": // 07 : 재가공출고
                           // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
                    StockInOutModel.TranRecord.Remark = etReason.GetEntryValue(); 
                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    if (StockIoMaster.InsertTable(StockInOutModel.TranRecord))
                    {
                        if (StockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo))
                        {
                            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                            break;
                        }
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "It has not been saved.", "OK");
                    return;
            }
            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            await Navigation.PopAsync();
        }
        async void OnFunctionClose(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            await Navigation.PopAsync();
        }

        private void SetEntityData(TaxpayerItemRecord record, bool readOnly)
        {
            etItemCode.SetEntryValue(record.ItemCd);
            etItemCode.SetReadOnly(readOnly);
            bool autoNumbering = false;
            if (!string.IsNullOrEmpty(record.UseAdiYn)) autoNumbering = record.UseAdiYn.Equals("Y") ? true : false;
            etClassCode.SetEntryValue(record.ItemClsCd);
            etClassName.SetEntryValue(record.ItemClsName);

            etItemName.SetEntryValue(record.ItemNm);
            etUseBarcode.SetSelecteItem(new SystemCode() { Id = record.UseBarcode, Name = "" });
            etBarcode.SetEntryValue(record.Bcd);

            etOrigin.SetEntryValue(record.OrgnNatName);
            etInsuranceYN.SetSelecteItem(new SystemCode() { Id = record.IsrcAplcbYn, Name = "" });
            etL1SalePrice.SetEntryValue(record.GrpPrcL1);
            
            etItemType.SetSelecteItem(new SystemCode() { Id = record.ItemTyCd, Name = "" });
            etPkgUnit.SetSelecteItem(new SystemCode() { Id = record.PkgUnitCd, Name = "" });
            etQtyUnit.SetSelecteItem(new SystemCode() { Id = record.QtyUnitCd, Name = "" });
            etL2SalePrice.SetEntryValue(record.GrpPrcL2);

            etPurchasePrice.SetEntryValue(record.InitlWhUntpc);
            etSalePrice.SetEntryValue(record.DftPrc);
            etTaxType.SetSelecteItem(new SystemCode() { Id = record.TaxTyCd, Name = "" });
            etL3SalePrice.SetEntryValue(record.GrpPrcL3);

            etBeginningStock.SetEntryValue(record.InitlQty);
            etCurrentStock.SetEntryValue(record.RdsQty);
            etSafetyStock.SetEntryValue(record.SftyQty);
            etL4SalePrice.SetEntryValue(record.GrpPrcL4);

            etDescription.SetEntryValue(record.Rm);
            etUse.SetSelecteItem(new SystemCode() { Id = record.UseYn, Name = "" });
            etL5SalePrice.SetEntryValue(record.GrpPrcL5);

            etCurrentQty.SetEntryValue(record.RdsQty);
            etQtyAfter.SetEntryValue(record.RdsQty);

            // 본점이 아니면
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            if(BhfId.Equals("00")) etBeforeLocation.SetEntryValue("HQ");
            else etBeforeLocation.SetEntryValue(BhfId);
        }
    }
}

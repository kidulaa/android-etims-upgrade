using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.Utils;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.ItemManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemManagementPage : ContentPage
    {
        ObservableCollection<TaxpayerItemRecord> lvItemManagement { get; set; }
        List<TaxpayerItemRecord> listTaxpayerItemRecord;
        TaxpayerItemMaster master = null;
        TaxpayerItemRecord record = null;

        bool NewFlag = true;

        public ItemManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerItemMaster();
            record = new TaxpayerItemRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

            record.clear();
            SetEntityData(record, false);
            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnImport.IsVisible = false;
                btnExport.IsVisible = false;
            }

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };

            etUseExpire.GetPicker().SelectedIndexChanged += (sender, e) => {
                string id = etUseExpire.GetSelectedItem().Id;
                if (id.Equals("Y") && !etBeginningStock.GetEntry().IsReadOnly)
                {
                    etExpireDateInput.IsVisible = true;
                }
                else
                {
                    etExpireDateInput.IsVisible = false;
                }
            };

            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            etOrigin.SetReadOnly(true);
            etCurrentStock.SetReadOnly(true);
            etExpireDate.SetReadOnly(true);

            etItemName.GetEntry().MaxLength = 200;

            //etLikeValue.SetFocus();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(listTaxpayerItemRecord == null || listTaxpayerItemRecord.Count <= 0)
            {
                AnimationLoop();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTaxpayerItemRecord = master.getTaxpayerItemTable(tin, "", etSearchUsable.GetSelectedItem().Id);
            SetList(listTaxpayerItemRecord);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton
        }

        async void OnSearch(object sender, EventArgs e)
        {
            string valueLike = etLikeValue.GetEntryValue();
            string valueUseYn = etSearchUsable.GetSelectedItem().Id;
            //if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 3)
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 3 characters.", "OK");
            //    etLikeValue.SetFocus();
            //    return;
            //}
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTaxpayerItemRecord = master.getTaxpayerItemTable(tin, valueLike, valueUseYn);
            SetList(listTaxpayerItemRecord);
            if (listTaxpayerItemRecord == null || listTaxpayerItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
 
            record.clear();
            SetEntityData(record, false);
        }
        void SetList(List<TaxpayerItemRecord> datas)
        {
            try
            {
                lvItemManagement = new ObservableCollection<TaxpayerItemRecord>();
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


        private string GetCellValue(SpreadsheetDocument doc, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            try
            {
                string value = cell.CellValue.InnerText;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                }
                return value;
            }
            catch
            {
                return "";
            }
        }

        private async void FunctionImport(string filepath)
        {
            try
            {
                string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;


                using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))//Added By Aime on 10.5.2022
                {
                    using (var excel = SpreadsheetDocument.Open(fileStream, false))//Added By Aime on 10.5.2022
                    /*
                     * using (var excel = SpreadsheetDocument.Open(filepath, false)) 
                     * Commented on 10.5.2022  By Bright When Adding Aime's Code
                     */
                    {
                        var sheets = excel.WorkbookPart.WorksheetParts;
                        foreach (var wp in sheets)
                        {
                            Worksheet worksheet = wp.Worksheet;
                            IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Elements<Row>();
                            List<Row> rowList = rows.ToList();
                            int rcount = rowList.Count();

                            var rowTitle = rowList[0];
                            var cellsTitle = rowTitle.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                            List<DocumentFormat.OpenXml.Spreadsheet.Cell> cellListTitle = cellsTitle.ToList();
                            int ccountTitle = cellListTitle.Count();
                            string dataTitle = GetCellValue(excel, cellListTitle[0]);
                            dataTitle += GetCellValue(excel, cellListTitle[1]);

                            if (!dataTitle.Equals("TinItemCd"))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file. [TaxpayerItem.xlsx]", "OK");
                                break;
                            }

                            for (int i = 1; i < rcount; i++)
                            {
                                var row = rowList[i];
                                var cells = row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                                List<DocumentFormat.OpenXml.Spreadsheet.Cell> cellList = cells.ToList();

                                TaxpayerItemRecord custRecord = new TaxpayerItemRecord();

                                if (cellList.Count >= 1) custRecord.Tin = GetCellValue(excel, cellList[0]);      //cellList[0].CellValue.Text;                  // Taxpayer Identification Number(TIN)
                                if (string.IsNullOrEmpty(custRecord.Tin) || !custRecord.Tin.Equals(Tin))
                                {
                                    continue;
                                }
                                if (cellList.Count > 1) custRecord.ItemCd = GetCellValue(excel, cellList[1]);      //cellList[1].CellValue.Text;               // Item Code
                                if (cellList.Count > 2) custRecord.ItemClsCd = GetCellValue(excel, cellList[2]);      //cellList[2].CellValue.Text;            // Item Classification Code (RRA)
                                if (cellList.Count > 3) custRecord.ItemTyCd = GetCellValue(excel, cellList[3]);      //cellList[3].CellValue.Text;             // Item Type Code
                                if (cellList.Count > 4) custRecord.ItemNm = GetCellValue(excel, cellList[4]);      //cellList[4].CellValue.Text;               // Item Name
                                if (cellList.Count > 5) custRecord.ItemStdNm = GetCellValue(excel, cellList[5]);      //cellList[5].CellValue.Text;            // Item Stand Name
                                if (cellList.Count > 6) custRecord.OrgnNatCd = GetCellValue(excel, cellList[6]);      //cellList[6].CellValue.Text;            // Origin National Code
                                if (cellList.Count > 7) custRecord.PkgUnitCd = GetCellValue(excel, cellList[7]);      //cellList[7].CellValue.Text;            // Package Unit Code
                                if (cellList.Count > 8) custRecord.QtyUnitCd = GetCellValue(excel, cellList[8]);      //cellList[8].CellValue.Text;            // Quantity Unit Code
                                if (cellList.Count > 9) custRecord.TaxTyCd = GetCellValue(excel, cellList[9]);      //cellList[9].CellValue.Text;              // Taxation Type Code
                                if (cellList.Count > 10) custRecord.Bcd = GetCellValue(excel, cellList[10]);      //cellList[10].CellValue.Text;                // Barcode
                                if (cellList.Count > 11) custRecord.RegBhfId = GetCellValue(excel, cellList[11]);      //cellList[11].CellValue.Text;           // Branch Office ID
                                if (cellList.Count > 12) custRecord.UseYn = GetCellValue(excel, cellList[12]);      //cellList[12].CellValue.Text;              // Use(Y/N)
                                if (cellList.Count > 13) custRecord.RraModYn = GetCellValue(excel, cellList[13]);      //cellList[13].CellValue.Text;           // RRA Modified(Y/N)
                                if (cellList.Count > 14) custRecord.AddInfo = GetCellValue(excel, cellList[14]);      //cellList[14].CellValue.Text;            // Additional Information
                                if (cellList.Count > 15) custRecord.SftyQty = GetDouble(GetCellValue(excel, cellList[15]));             // Safety Quantity
                                if (cellList.Count > 16) custRecord.IsrcAplcbYn = GetCellValue(excel, cellList[16]);      //cellList[16].CellValue.Text;        // Insurance Appicable(Y/N)
                                if (cellList.Count > 17) custRecord.DftPrc = GetDouble(GetCellValue(excel, cellList[17]));              // Default Price
                                if (cellList.Count > 18) custRecord.GrpPrcL1 = GetDouble(GetCellValue(excel, cellList[18]));            // Group Default Price L1
                                if (cellList.Count > 19) custRecord.GrpPrcL2 = GetDouble(GetCellValue(excel, cellList[19]));            // Group Default Price L2
                                if (cellList.Count > 20) custRecord.GrpPrcL3 = GetDouble(GetCellValue(excel, cellList[20]));            // Group Default Price L3
                                if (cellList.Count > 21) custRecord.GrpPrcL4 = GetDouble(GetCellValue(excel, cellList[21]));            // Group Default Price L4
                                if (cellList.Count > 22) custRecord.GrpPrcL5 = GetDouble(GetCellValue(excel, cellList[22]));            // Group Default Price L5

                                if (cellList.Count > 23) custRecord.RegrId = GetCellValue(excel, cellList[23]);      //cellList[23].CellValue.Text;             // Registrant ID
                                if (cellList.Count > 24) custRecord.RegrNm = GetCellValue(excel, cellList[24]);      //cellList[24].CellValue.Text;             // Registrant Name
                                if (cellList.Count > 25) custRecord.RegDt = GetCellValue(excel, cellList[25]);      //cellList[25].CellValue.Text;      // Registered Date
                                if (cellList.Count > 26) custRecord.ModrId = GetCellValue(excel, cellList[26]);      //cellList[26].CellValue.Text;             // Modifier ID
                                if (cellList.Count > 27) custRecord.ModrNm = GetCellValue(excel, cellList[27]);      //cellList[27].CellValue.Text;             // Modifier Name
                                if (cellList.Count > 28) custRecord.ModDt = GetCellValue(excel, cellList[28]);      //cellList[28].CellValue.Text;      // Modified Date
                                if (cellList.Count > 29) custRecord.InitlWhUntpc = GetDouble(GetCellValue(excel, cellList[29]));        // 초기 입고단가
                                if (cellList.Count > 30) custRecord.InitlQty = GetDouble(GetCellValue(excel, cellList[30]));            // 초기 입고수량
                                if (cellList.Count > 31) custRecord.Rm = GetCellValue(excel, cellList[31]);      //cellList[31].CellValue.Text;                 // 비고
                                if (cellList.Count > 32) custRecord.UseBarcode = GetCellValue(excel, cellList[32]);      //cellList[32].CellValue.Text;         // 바코드사용여부
                                if (cellList.Count > 33) custRecord.UseAdiYn = GetCellValue(excel, cellList[33]);      //cellList[33].CellValue.Text;           // 부가정보사용여부
                                if (cellList.Count > 34) custRecord.BatchNum = GetCellValue(excel, cellList[34]);      //cellList[34].CellValue.Text;           // BatchNum
                                if (cellList.Count > 35) custRecord.UseExpiration = GetCellValue(excel, cellList[35]);      //cellList[35].CellValue.Text;      // UseExpiration
                                                                                                                            //JCNA 202001 DELETE 
                                                                                                                            //if (cellList[35].CellValue != null) custRecord.ExpirationDtUse = cellList[35].CellValue.Text;    // Expiration Dt Use
                                custRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                                custRecord.ModrId = UIManager.Instance().UserModel.UserId;
                                custRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                                custRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                                custRecord.RegrId = UIManager.Instance().UserModel.UserId;
                                custRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                                //bool retSaved = master.ToTable(custRecord);
                                StockMasterRecord stockRecord = new StockMasterRecord();
                                stockRecord.Tin = custRecord.Tin;
                                stockRecord.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                                stockRecord.ItemCd = custRecord.ItemCd;
                                stockRecord.RsdQty = 0;
                                stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                                stockRecord.ModrId = UIManager.Instance().UserModel.UserId;
                                stockRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                                stockRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                                stockRecord.RegrId = UIManager.Instance().UserModel.UserId;
                                stockRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                                bool IsExist = master.GetItemCode(custRecord.Tin, custRecord.ItemCd);
                                bool isClassCodeExist = master.GetItemClassCode(custRecord.ItemClsCd);

                                //Added by Aime on 10.5.2022
                                /*
                                 * 24.05.2022 Commented BY Bright, while adding Aime Code
                                bool isClassCodeExist = master.GetItemClassCode(custRecord.ItemClsCd, custRecord.ItemTyCd);
                                bool ret = master.ToTable(custRecord, stockRecord);
                                */
                                //END
                                bool ret = false;

                              if(isClassCodeExist && custRecord.ItemClsCd.Length <= 10 && Convert.ToInt32(custRecord.ItemTyCd) <= 3)
                                {
                                    ret = master.ToTable(custRecord, stockRecord);
                                }

                                if (ret)
                                {
                                    //============ 20191210 JCNA
                                    if (!IsExist)
                                    {
                                        StockIoMaster StockIoMaster = new StockIoMaster();
                                        StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();
                                        //string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                                        string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                                        long SarNo = StockIoMaster.GetStockIoSeq();
                                        string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
                                        string RegTyCd = "A"; 
                                        string UserId = UIManager.Instance().UserModel.UserId;
                                        string UserNm = UIManager.Instance().UserModel.UserNm;

                                        TransactionStockInOutModel StockInOutModel = new TransactionStockInOutModel();
                                        StockInOutModel.CurrentItemRecord = null;
                                        StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
                                        StockInOutModel.SetCurrentItem(custRecord);
                                        StockInOutModel.CurrentItemRecord.Qty = custRecord.InitlQty;
                                        StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                                        StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                                        StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                                        StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                                        StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                                        StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;
                                        StockInOutModel.CalculateCurrentItem();
                                        StockInOutModel.ConfirmCurrentItem();
                                        StockInOutModel.TranRecord.SarTyCd = "06"; 
                                        StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();

                                        StockIoMaster.InsertTable(StockInOutModel.TranRecord);
                                        StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

                                        //===>>>>>>>>>
                                        //JCNA 20191204
                                        StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
                                        stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                                    }

                                    //===>>>>>>>>>
                                    //JCNA 20191204
                                    ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
                                    itemRraSdcUpload.SendItemSave(custRecord.Tin, custRecord.ItemCd);
                                }
                                else
                                {
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Didn't Save! (" + custRecord.ItemCd + ")", "Ok");
                                }
                              
                            }

                            //===>>>>>>>>>
                            // JCNA 20191204 TR 
                            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                            rraSdcUploadProcess.UploadProcess();

                            AnimationLoop();
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Excel file import", "OK");
                            break;
                        }
                        excel.Close();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString(); //"Only one operation can be active at a time"
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file.", "OK");
            }
        }
        public double GetDouble(string value)
        {
            double dval = 0;
            try
            {
                dval = double.Parse(value);
            }
            catch
            {

            }
            return dval;
        }
        private async void OnFunctionImport(object sender, EventArgs e)
        {
            try
            {
                FileData filedata = await CrossFilePicker.Current.PickFile();
                if (filedata != null && !string.IsNullOrEmpty(filedata.FileName))
                {
                    FunctionImport(filedata.FilePath);
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString(); //"Only one operation can be active at a time"
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file.", "OK");
            }
        }

        async void OnFunctionExport(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (listTaxpayerItemRecord == null || listTaxpayerItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listTaxpayerItemRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TaxpayerItem_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionNew(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Info");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to proceed  the New item creation?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            NewFlag = true;

            record.clear();
            record.OrgnNatCd = "RW";
            record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.ModrId = UIManager.Instance().UserModel.UserId;
            record.ModrNm = UIManager.Instance().UserModel.UserNm;
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;
            
            SetEntityData(record, false);
        }

        async void OnFunctionSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etItemCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Item code..", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(record.OrgnNatCd))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "OrgnNatCd is Empty.", "OK");
                return;
            }

            if (!record.OrgnNatCd.Equals(etItemCode.GetEntryValue().Substring(0,2)))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "OrgnNatCd is wrong.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(etClassCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Class code..", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etItemName.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Item name.", "Ok");
                return;
            }

            if (record == null) {
                record = new TaxpayerItemRecord();
            }

            record.ItemTyCd = etItemType.GetSelectedItem().Id;
            record.PkgUnitCd = etPkgUnit.GetSelectedItem().Id;
            record.QtyUnitCd = etQtyUnit.GetSelectedItem().Id;
            //record.GrpPrcL2 = etL2SalePrice.GetEntryValue();

            if (etPurchasePrice.GetEntryValue() > 99999999999)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid PurchasePrice.", "OK");
                return;
            }
            if (etSalePrice.GetEntryValue() > 99999999999)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid SalePrice.", "OK");
                return;
            }

            if (etPurchasePrice.GetEntryValue() > etSalePrice.GetEntryValue())
            {
                string locationTitle211 = UILocation.Instance().GetLocationText("Confirm");
                string locationMessage211 = UILocation.Instance().GetLocationText("The purchase price is greater than the sale price. Do you want to continue?");
                var retContinue = await DisplayAlert(locationTitle211, locationMessage211, "Yes", "No"); 
                if(!retContinue) return;
            }
            if (string.IsNullOrEmpty(etItemType.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the item type.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etPkgUnit.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the pkg unit.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etTaxType.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the tax type.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etQtyUnit.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the Qty Unit.", "Ok");
                return;
            }

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.ItemCd = etItemCode.GetEntryValue();

            record.UseAdiYn = etAutoNumbering.IsChecked() ? "Y" : "N";
            record.ItemClsCd = etClassCode.GetEntryValue();
            record.ItemClsName = etClassName.GetEntryValue();

            record.ItemNm = etItemName.GetEntryValue();
            record.UseBarcode = etUseBarcode.GetSelectedItem().Id;
            record.Bcd = etBarcode.GetEntryValue();
            if (!string.IsNullOrEmpty(record.UseBarcode) && record.UseBarcode.Equals("Y"))
            {
                if (string.IsNullOrEmpty(record.Bcd))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please enter the Barcode.", "Ok");
                    return;

                }
                if (!string.IsNullOrEmpty(record.Bcd))
                {
                    string itemCode = master.GetItemCodeWithBarcode(record.Tin, record.Bcd);
                    if(!string.IsNullOrEmpty(itemCode))
                    {
                        string locationTitle211 = UILocation.Instance().GetLocationText("Confirm");
                        string locationMessage211 = UILocation.Instance().GetLocationText("I have an ITEM(" + itemCode + ") that uses this barcode. Do you want to generate additional ITEM CODE?");
                        var resultBcd = await DisplayAlert(locationTitle211, locationMessage211, "Yes", "No");
                        if (!resultBcd) return;
                    }
                }
            }

            record.UseExpiration = etUseExpire.GetSelectedItem().Id;

            if (record.UseBarcode.Equals("Y") && string.IsNullOrEmpty(record.Bcd))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please enter the Barcode.", "Ok");
                return;
            }

            record.BatchNum = etBatchNum.GetEntryValue();
            if (!string.IsNullOrEmpty(record.UseBarcode) && record.UseBarcode.Equals("Y"))
            {
                if (!string.IsNullOrEmpty(record.Bcd) && !string.IsNullOrEmpty(record.BatchNum))
                {
                    string itemCode = master.GetItemCodeWithBarcodeBatchNum(record.Tin, record.Bcd, record.BatchNum);
                    if (!string.IsNullOrEmpty(itemCode))
                    {
                        string locationTitle211 = UILocation.Instance().GetLocationText("Confirm");
                        string locationMessage211 = UILocation.Instance().GetLocationText("I have an ITEM(" + record.BatchNum + ") that uses this BatchNum. Do you want to generate additional ITEM CODE?");
                        var resultBcd = await DisplayAlert(locationTitle211, locationMessage211, "Yes", "No");
                        if (!resultBcd) return;
                    }
                }
            }

            record.OrgnNatName = etOrigin.GetEntryValue();
            record.IsrcAplcbYn = etInsuranceYN.GetSelectedItem().Id;
            record.GrpPrcL1 = etL1SalePrice.GetEntryValue();

            record.ItemTyCd = etItemType.GetSelectedItem().Id;
            record.PkgUnitCd = etPkgUnit.GetSelectedItem().Id;
            record.QtyUnitCd = etQtyUnit.GetSelectedItem().Id;
            //record.GrpPrcL2 = etL2SalePrice.GetEntryValue();

            record.InitlWhUntpc = etPurchasePrice.GetEntryValue();
            record.DftPrc = etSalePrice.GetEntryValue();
            record.TaxTyCd = etTaxType.GetSelectedItem().Id;
            //record.GrpPrcL3 = etL3SalePrice.GetEntryValue();

            record.InitlQty = etBeginningStock.GetEntryValue();
            record.RdsQty = etCurrentStock.GetEntryValue();
            record.SftyQty = etSafetyStock.GetEntryValue();
            //record.GrpPrcL4 = etL4SalePrice.GetEntryValue();

            record.Rm = etDescription.GetEntryValue();
            record.UseYn = etUse.GetSelectedItem().Id;
            //record.GrpPrcL5 = etL5SalePrice.GetEntryValue();

            record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.ModrId = UIManager.Instance().UserModel.UserId;
            record.ModrNm = UIManager.Instance().UserModel.UserNm;
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;

            StockMasterRecord stockRecord = new StockMasterRecord();
            stockRecord.Tin = record.Tin;
            stockRecord.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            stockRecord.ItemCd = record.ItemCd;
            stockRecord.RsdQty = 0;
            stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.ModrId = UIManager.Instance().UserModel.UserId;
            stockRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            stockRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.RegrId = UIManager.Instance().UserModel.UserId;
            stockRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to save this Item?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            bool IsExist = master.GetItemCode(record.Tin, record.ItemCd);

            if (NewFlag)
            {
                if (IsExist)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This code already exists.", "Ok");
                    return;
                }
            }

            bool ret = master.ToTable(record, stockRecord);
            if(ret)
            {
                //============ 20191210 JCNA
                if (!IsExist)
                {
                    StockIoMaster StockIoMaster = new StockIoMaster();
                    StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();
                    string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                    string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                    long SarNo = StockIoMaster.GetStockIoSeq();
                    string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
                    string RegTyCd = "A";
                    string UserId = UIManager.Instance().UserModel.UserId;
                    string UserNm = UIManager.Instance().UserModel.UserNm;

                    TransactionStockInOutModel StockInOutModel = new TransactionStockInOutModel();
                    StockInOutModel.CurrentItemRecord = null;
                    StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
                    StockInOutModel.SetCurrentItem(record);
                    StockInOutModel.CurrentItemRecord.Qty = record.InitlQty;
                    StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    if (record.UseExpiration.Equals("Y"))
                    {
                        StockInOutModel.CurrentItemRecord.ItemExprDt = etExpireDateInput.GetDateTime().ToString("yyyyMMdd");
                    }
                    StockInOutModel.ConfirmCurrentItem();
                    StockInOutModel.TranRecord.SarTyCd = "06"; 
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();

                    StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                    StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                    StockIoMaster.InsertTable(StockInOutModel.TranRecord);
                    StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

                    //===>>>>>>>>>
                    //JCNA 20191204
                    StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
                    stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
                }

                //===>>>>>>>>>
                //JCNA 20191204
                ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
                itemRraSdcUpload.SendItemSave(record.Tin, record.ItemCd);

                //===>>>>>>>>>
                // JCNA 20191204 TR 
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully!", "Ok");

                string valueLike = etLikeValue.GetEntryValue();
                string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                List<TaxpayerItemRecord> list = master.getTaxpayerItemTable(tin, valueLike, valueUseYn);
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                NewFlag = true;
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Didn't Save!", "Ok");
            }
        }

        async void OnFunctionDelete(object sender, EventArgs e)
        {
            /*
            Dim t = model.hasRecordHistory(strItemCode)
            Dim st = model.hasStockInfo(strItemCode)

            If t > 0 Then
                MessageBox.Show("Item can't be deleted. It presents historical data!",
                        strFormTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1)
                Exit Sub
            End If
            */

                        if (!etItemCode.IsValid())
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select the data you want to delete.", "Ok");
                etItemCode.SetFocus();
                return;
            }
            string locationTitle21 = UILocation.Instance().GetLocationText("Delete");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to delete the item " + etItemName.GetEntryValue() + "?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.ItemCd = etItemCode.GetEntryValue();

            bool delItem = master.DeleteTable(record);
            if (delItem)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Item deleted succcessfully.", "Ok");

                //// Inventory Detele
                //bool delInventory = master.DeleteTable(record);
                //if (delInventory)
                //{
                //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Delete inventory failed. Please check database.", "Ok");
                //}

                string valueLike = etLikeValue.GetEntryValue();
                string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                List<TaxpayerItemRecord> list = master.getTaxpayerItemTable(tin, valueLike, valueUseYn);
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                NewFlag = true;
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Delete failed. Please check database.", "Ok");
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            await Navigation.PopAsync();
        }

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                record = (TaxpayerItemRecord)e.SelectedItem;
                record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                record.ModrId = UIManager.Instance().UserModel.UserId;
                record.ModrNm = UIManager.Instance().UserModel.UserNm;
                SetEntityData(record, true);
                NewFlag = false;
            }
        }
        private void SetEntityData(TaxpayerItemRecord record, bool readOnly)
        {
            if (readOnly)
            {
                etAutoNumberingImageButton.IsVisible = false;
                etClassCodeImageButton.IsVisible = false;
                etOriginImageButton.IsVisible = false;
            }
            else
            {
                etAutoNumberingImageButton.IsVisible = true;
                etClassCodeImageButton.IsVisible = true;
                etOriginImageButton.IsVisible = true;

            }
            etItemCode.SetEntryValue(record.ItemCd);
            etItemCode.SetReadOnly(readOnly);
            bool autoNumbering = true;
            if (!string.IsNullOrEmpty(record.UseAdiYn)) autoNumbering = record.UseAdiYn.Equals("Y") ? true : false;
            etAutoNumbering.SetCheck(autoNumbering);
            etClassCode.SetEntryValue(record.ItemClsCd);
            etClassName.SetEntryValue(record.ItemClsName);

            etItemName.SetEntryValue(record.ItemNm);
            etUseBarcode.SetSelecteItem(new SystemCode() { Id = record.UseBarcode, Name = "" });
            etBarcode.SetEntryValue(record.Bcd);

            etBatchNum.SetEntryValue(record.BatchNum);
            etBatchNum.SetReadOnly(readOnly);
            etOrigin.SetEntryValue(record.OrgnNatName);
            etInsuranceYN.SetSelecteItem(new SystemCode() { Id = record.IsrcAplcbYn, Name = "" });
            etL1SalePrice.SetEntryValue(record.GrpPrcL1);
            
            etItemType.SetSelecteItem(new SystemCode() { Id = record.ItemTyCd, Name = "" });
            etItemType.SetReadOnly(readOnly);
            etPkgUnit.SetSelecteItem(new SystemCode() { Id = record.PkgUnitCd, Name = "" });
            etPkgUnit.SetReadOnly(readOnly);
            etQtyUnit.SetSelecteItem(new SystemCode() { Id = record.QtyUnitCd, Name = "" });
            etQtyUnit.SetReadOnly(readOnly);
            //etL2SalePrice.SetEntryValue(record.GrpPrcL2);

            etPurchasePrice.SetEntryValue(record.InitlWhUntpc);
            etSalePrice.SetEntryValue(record.DftPrc);
            etTaxType.SetSelecteItem(new SystemCode() { Id = record.TaxTyCd, Name = "" });
            //etL3SalePrice.SetEntryValue(record.GrpPrcL3);

            /*
             * etBeginningStock.SetEntryValue((int)record.InitlQty);
             * 24.05.2022 Commented BY Bright, while adding Aime Code
             */
            etBeginningStock.SetEntryValue(record.InitlQty);
            etBeginningStock.SetReadOnly(readOnly);
            etCurrentStock.SetEntryValue((int)record.RdsQty);
            etCurrentStock.SetReadOnly(readOnly);
            etSafetyStock.SetEntryValue((int)record.SftyQty);
            //etL4SalePrice.SetEntryValue(record.GrpPrcL4);

            etDescription.SetEntryValue(record.Rm);
            etUse.SetSelecteItem(new SystemCode() { Id = record.UseYn, Name = "" });
            //etL5SalePrice.SetEntryValue(record.GrpPrcL5);

            etUseExpire.SetSelecteItem(new SystemCode() { Id = record.UseExpiration, Name = "" });
            if (!string.IsNullOrEmpty(record.ExpirationDate)) {
                string textPrice = Common.DateTimeFormat(record.ExpirationDate).ToString("dd-MM-yyyy");
                etExpireDate.SetEntryValue(textPrice);
            }
            else
            {
                etExpireDate.SetEntryValue("");
            }
            //etUseExpire.SetReadOnly(readOnly);

            if (etUseExpire.GetSelectedItem().Equals("Y"))
            {
                etExpireDateInput.IsVisible = !readOnly;
            }
            else
            {
                etExpireDateInput.IsVisible = false;
            }
        }

        async void OnFunctionQueryClassCode(object sender, EventArgs e)
        {
            var popupPage = new ClassificationPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    ItemClassRecord popupRecord = (ItemClassRecord)((ExtEventArgs)ex).EnteredObject;

                    etClassCode.SetEntryValue(popupRecord.ItemClsCd);
                    etClassName.SetEntryValue(popupRecord.ItemClsNm);

                    etTaxType.SetSelecteItem(new SystemCode() { Id = popupRecord.TaxTyCd, Name = "" });

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
        async void OnFunctionQueryOrigin(object sender, EventArgs e)
        {
            var popupPage = new OriginPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    CodeDtlRecord popupRecord = (CodeDtlRecord)((ExtEventArgs)ex).EnteredObject;
                    record.OrgnNatCd = popupRecord.Cd;
                    etOrigin.SetEntryValue(popupRecord.CdNm);
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

        async void OnFunctionNumbering(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etItemCode.GetEntryValue()))
            {
                if(string.IsNullOrEmpty(record.OrgnNatCd))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "OrgnNatCd is Empty.", "OK");
                    return;
                }

                if (etItemType.GetSelectedItem() == null || string.IsNullOrEmpty(etItemType.GetSelectedItem().Id))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "ItemType is Empty.", "OK");
                    return;
                }
                record.ItemTyCd = etItemType.GetSelectedItem().Id;
                if (etPkgUnit.GetSelectedItem() == null || string.IsNullOrEmpty(etPkgUnit.GetSelectedItem().Id))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "PkgUnit is Empty.", "OK");
                    return;
                }
                record.PkgUnitCd = etPkgUnit.GetSelectedItem().Id;

                if (etQtyUnit.GetSelectedItem() == null || string.IsNullOrEmpty(etQtyUnit.GetSelectedItem().Id))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "QtyUnit is Empty.", "OK");
                    return;
                }
                record.QtyUnitCd = etQtyUnit.GetSelectedItem().Id;

                string strItemCode = master.GenItemCode(record, etAutoNumbering.IsChecked());
                etItemCode.SetEntryValue(strItemCode);
            }
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }
    }
}

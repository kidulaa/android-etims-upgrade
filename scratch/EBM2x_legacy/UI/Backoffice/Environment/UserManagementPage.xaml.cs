using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.i18n;
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

namespace EBM2x.UI.Backoffice.Environment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserManagementPage : ContentPage
    {
        public ObservableCollection<TaxpayerBhfDeviceUserRecord> lvUserManagement { get; set; }
        List<TaxpayerBhfDeviceUserRecord> listTaxpayerBhfDeviceUserRecord;
        TaxpayerBhfDeviceUserMaster master = null;
        TaxpayerBhfDeviceUserRecord record = null;

        bool NewFlag = true;

        public UserManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBhfDeviceUserMaster();
            record = new TaxpayerBhfDeviceUserRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

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

            cbSetting.TitleInvalidateSurface("Setting");
            cbUseMgt.TitleInvalidateSurface("UseMgt");
            cbCustomer.TitleInvalidateSurface("Customer");
            cbRefund.TitleInvalidateSurface("Refund");
            cbPrice.TitleInvalidateSurface("Price");
            cbZReport.TitleInvalidateSurface("ZReport");
            cbSaleRpt.TitleInvalidateSurface("SaleRpt");
            cbProforma.TitleInvalidateSurface("Proforma");
            cbStock.TitleInvalidateSurface("Stock");
            cbAdjust.TitleInvalidateSurface("Adjust");
            cbImport.TitleInvalidateSurface("Import");
            cbPurchase.TitleInvalidateSurface("Purchase");

            etUserName.GetEntry().MaxLength = 60;

            SetEntityData(record, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(listTaxpayerBhfDeviceUserRecord == null || listTaxpayerBhfDeviceUserRecord.Count <= 0)
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
            string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            listTaxpayerBhfDeviceUserRecord = master.getTaxpayerBhfDeviceUserTable(tin, bhfid, "", etSearchUsable.GetSelectedItem().Id);
            SetList(listTaxpayerBhfDeviceUserRecord);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton
        }

        async void OnImportPhoto(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etUserID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the User ID.", "Ok");
                return;
            }

            try
            {
                FileData filedata = await CrossFilePicker.Current.PickFile();
                if (filedata != null && !string.IsNullOrEmpty(filedata.FileName))
                {
                    string imageFileName = filedata.FileName.Substring(0, filedata.FileName.LastIndexOf("."));
                    string filepath = filedata.FilePath;
                    string extName = filedata.FileName.Substring(filedata.FileName.LastIndexOf(".") + 1);
                    if (extName.ToLower() == "png" || extName.ToLower() == "gif" || extName.ToLower() == "bmp" || extName.ToLower() == "jpg")
                    {
                        // JINIT_20191210
                        imgUser.InvalidateSurface(filepath);
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected image file.", "OK");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString(); //"Only one operation can be active at a time"
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected image file.", "OK");
            }

        }


        async void OnSearch(object sender, EventArgs e)
        {
            // JINIT_20191210, Clear
            record.clear();
            SetEntityData(record, false);
            
            string valueLike = etLikeValue.GetEntryValue();
            string valueUseYn = etSearchUsable.GetSelectedItem().Id;
            //if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 4)
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 4 characters.", "OK");
            //    return;
            //}
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            listTaxpayerBhfDeviceUserRecord = master.getTaxpayerBhfDeviceUserTable(tin, bhfid, valueLike, valueUseYn);
            SetList(listTaxpayerBhfDeviceUserRecord);
            if (listTaxpayerBhfDeviceUserRecord == null || listTaxpayerBhfDeviceUserRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<TaxpayerBhfDeviceUserRecord> datas)
        {
            try
            {
                lvUserManagement = new ObservableCollection<TaxpayerBhfDeviceUserRecord>();
                listView.ItemsSource = lvUserManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvUserManagement.Add(datas[i]);
                }
            }
            catch
            {
            }
        }
        private string GetCellValue(SpreadsheetDocument doc, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

        private async void FunctionImport(string filepath)
        {
            try
            {
                string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                using (var excel = SpreadsheetDocument.Open(filepath, false))
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
                        dataTitle += GetCellValue(excel, cellListTitle[2]);

                        if (!dataTitle.Equals("TinBhfIdUserId"))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file. [TaxpayerBhfDeviceUser.xlsx]", "OK");
                            break;
                        }

                        for (int i = 1; i < rcount; i++)
                        {
                            var row = rowList[i];
                            var cells = row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                            List<DocumentFormat.OpenXml.Spreadsheet.Cell> cellList = cells.ToList();

                            TaxpayerBhfDeviceUserRecord custRecord = new TaxpayerBhfDeviceUserRecord();

                            if (cellList.Count >= 1) custRecord.Tin = GetCellValue(excel, cellList[0]);      //cellList[0].CellValue.Text;                  // Taxpayer Identification Number(TIN)
                            if (string.IsNullOrEmpty(custRecord.Tin) || !custRecord.Tin.Equals(Tin))
                            {
                                continue;
                            }
                            if (cellList.Count > 1) custRecord.BhfId = GetCellValue(excel, cellList[1]);    //cellList[1].CellValue.Text;                // Branch Office ID
                            if (cellList.Count > 2) custRecord.UserId = GetCellValue(excel, cellList[2]);   //cellList[2].CellValue.Text;               // User ID
                            if (cellList.Count > 3) custRecord.UserNm = GetCellValue(excel, cellList[3]);   //cellList[3].CellValue.Text;               // User Name
                            if (cellList.Count > 4) custRecord.Pwd = GetCellValue(excel, cellList[4]);      //cellList[4].CellValue.Text;                  // Password
                            if (cellList.Count > 5) custRecord.Adrs = GetCellValue(excel, cellList[5]);     //cellList[5].CellValue.Text;                 // Address
                            if (cellList.Count > 6) custRecord.Cntc = GetCellValue(excel, cellList[6]);     //cellList[6].CellValue.Text;                 // Contact
                            if (cellList.Count > 7) custRecord.AuthCd = GetCellValue(excel, cellList[7]);   //cellList[7].CellValue.Text;               // Authority Code
                            if (cellList.Count > 8) custRecord.Remark = GetCellValue(excel, cellList[8]);   //cellList[8].CellValue.Text;               // Remark
                            if (cellList.Count > 9) custRecord.UseYn = GetCellValue(excel, cellList[9]);    //cellList[9].CellValue.Text;                // Use(Y/N)
                            if (cellList.Count > 10) custRecord.RegrId = GetCellValue(excel, cellList[10]); //cellList[10].CellValue.Text;             // Registrant ID
                            if (cellList.Count > 11) custRecord.RegrNm = GetCellValue(excel, cellList[11]); //cellList[11].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 12) custRecord.RegDt = GetCellValue(excel, cellList[12]);  //cellList[12].CellValue.Text;              // 
                            if (cellList.Count > 13) custRecord.Contact = GetCellValue(excel, cellList[13]);//cellList[13].CellValue.Text;            // 사용자전화번호
                            if (cellList.Count > 14) custRecord.RoleCd = GetCellValue(excel, cellList[14]); //cellList[14].CellValue.Text;             // 권한코드
                            if (cellList.Count > 15) custRecord.ImageNm = GetCellValue(excel, cellList[15]);//cellList[15].CellValue.Text;            // 사용자 사진 파일 경로

                            custRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                            if (UIManager.Instance().UserModel != null)
                            {
                                custRecord.RegrId = UIManager.Instance().UserModel.UserId;
                                custRecord.RegrNm = UIManager.Instance().UserModel.UserNm;
                            }

                            bool retSaved = master.ToTable(custRecord);
                        }

                        AnimationLoop();
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Excel file import", "OK");
                        break;
                    }
                    excel.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString(); //"Only one operation can be active at a time"
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file.", "OK");
            }
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

            if (listTaxpayerBhfDeviceUserRecord == null || listTaxpayerBhfDeviceUserRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listTaxpayerBhfDeviceUserRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TaxpayerBhfDeviceUser_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionNew(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Info");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to create new user?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            NewFlag = true;

            record.clear();
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;
            SetEntityData(record, false);
        }
        async void OnFunctionSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etUserID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the User ID.", "Ok");
                return;
            }
            if (etUserID.GetEntryValue().Length > 20) // 2021.7.24
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter up to 20 characters.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etUserName.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the User Name.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etPassword.GetEntryValue()) || etPassword.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the New password.", "Ok");
                etPassword.SetFocus();
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Save?");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to save the user?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            if (string.IsNullOrEmpty(record.BhfId))
            {
                record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            }

            record.UserId = etUserID.GetEntryValue();
            record.UserNm = etUserName.GetEntryValue();

            record.Pwd = etPassword.GetEntryValue();
            record.Cntc = etPhoneNumber.GetEntryValue();
            record.RoleCd = etRole.GetSelectedItem().Id;
            record.UseYn = etUsable.GetSelectedItem().Id;
            record.Adrs = etAddress.GetEntryValue();

            record.AuthCd = "";

            if (cbUseMgt.IsChecked()) record.AuthCd += "USERMGT;";
            if (cbRefund.IsChecked()) record.AuthCd += "REFUND;";
            if (cbZReport.IsChecked()) record.AuthCd += "ZREPORT;";
            if (cbPrice.IsChecked()) record.AuthCd += "PRICE;";

            if (cbSaleRpt.IsChecked()) record.AuthCd += "SALERPT;";
            if (cbSetting.IsChecked()) record.AuthCd += "SETTING;";
            if (cbAdjust.IsChecked()) record.AuthCd += "ADJUST;";
            if (cbProforma.IsChecked()) record.AuthCd += "PROFORMA;";

            if (cbCustomer.IsChecked()) record.AuthCd += "CUSTOMER;";
            if (cbStock.IsChecked()) record.AuthCd += "STOCK;";
            if (cbImport.IsChecked()) record.AuthCd += "IMPORT;";
            if (cbPurchase.IsChecked()) record.AuthCd += "PURCHASE;";

            if (record.AuthCd.Length < 20)
            {
                record.AuthCd += "00000000000000000000";
                record.AuthCd = record.AuthCd.Substring(0, 20);
            }

            // JINIT_20191210,
            if (imgUser.bitmap == null) record.Image = null;
            else record.Image = Utils.Common.FileToByteArray(imgUser.Icon);

            if (NewFlag)
            {
                TaxpayerBhfDeviceUserRecord recordNew = new TaxpayerBhfDeviceUserRecord();
                bool exist = master.ToRecord(recordNew, record.Tin, record.BhfId, record.UserId);
                if (exist)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This code already exists.", "Ok");
                    return;
                }
            }

            bool ret = master.ToTable(record);
            if (ret)
            {

                //===>>>>>>>>>
                //JCNA 20191204
                BhfUserRraSdcUpload bhfUserRraSdcUpload = new BhfUserRraSdcUpload();
                bhfUserRraSdcUpload.SendBhfUserSave(record.Tin, record.BhfId, record.UserId);

                //===>>>>>>>>>
                // JCNA 20191204 TR
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();
                
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "User saved successfully.", "Ok");

                string valueLike = etLikeValue.GetEntryValue();
                string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                List<TaxpayerBhfDeviceUserRecord> list = master.getTaxpayerBhfDeviceUserTable(tin, bhfid, valueLike, valueUseYn);
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                NewFlag = true;
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving user.", "Ok");
            }
        }

        async void OnFunctionDelete(object sender, EventArgs e)
        {
            if (!etUserID.IsValid())
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select the data you want to delete.", "Ok");
                return;
            }

            if (!UIManager.Instance().UserModel.RoleCd.Equals("1"))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "This operation is not allowed.", "Ok");
                return;
            }

            if (etRole.GetSelectedItem().Id.Equals("1"))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Can't delete admin.", "Ok");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to delete the user " + etUserName.GetEntryValue() + "?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            record.UserId = etUserID.GetEntryValue();

            bool retDelete = master.DeleteTable(record);
            if (retDelete)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "User deleted succcessfully.", "Ok");

                string valueLike = etLikeValue.GetEntryValue();
                string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                List<TaxpayerBhfDeviceUserRecord> list = master.getTaxpayerBhfDeviceUserTable(tin, bhfid, valueLike, valueUseYn);
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                NewFlag = true;
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to delete user. Please check database.", "Ok");
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
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
                record = (TaxpayerBhfDeviceUserRecord)e.SelectedItem;
                record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                record.RegrId = UIManager.Instance().UserModel.UserId;
                record.RegrNm = UIManager.Instance().UserModel.UserNm;
                SetEntityData(record, true);
                NewFlag = false;

                listView.SelectedItem = null;
            }
        }

        private void SetEntityData(TaxpayerBhfDeviceUserRecord record, bool readOnly)
        {
            etUserID.SetEntryValue(record.UserId);
            etUserID.SetReadOnly(readOnly);
            etUserName.SetEntryValue(record.UserNm);
            etUserName.SetReadOnly(readOnly);

            etPassword.SetEntryValue(record.Pwd);
            etPhoneNumber.SetEntryValue(record.Cntc);
            etRole.SetSelecteItem(new SystemCode() { Id = record.RoleCd, Name = "" }); 
            etUsable.SetSelecteItem(new SystemCode() { Id = record.UseYn, Name = "" }); 
            etAddress.SetEntryValue(record.Adrs);

            if (record.AuthCd.Length < 20)
            {
                record.AuthCd += "00000000000000000000";
                record.AuthCd = record.AuthCd.Substring(0, 20);
            }

            cbUseMgt.SetCheck(record.AuthCd.Contains("USERMGT;") ? true : false);
            cbRefund.SetCheck(record.AuthCd.Contains("REFUND;") ? true : false);
            cbZReport.SetCheck(record.AuthCd.Contains("ZREPORT;") ? true : false);
            cbPrice.SetCheck(record.AuthCd.Contains("PRICE;") ? true : false);

            cbSaleRpt.SetCheck(record.AuthCd.Contains("SALERPT;") ? true : false);
            cbSetting.SetCheck(record.AuthCd.Contains("SETTING;") ? true : false);
            cbAdjust.SetCheck(record.AuthCd.Contains("ADJUST;") ? true : false);
            cbProforma.SetCheck(record.AuthCd.Contains("PROFORMA;") ? true : false);

            cbCustomer.SetCheck(record.AuthCd.Contains("CUSTOMER;") ? true : false);
            cbStock.SetCheck(record.AuthCd.Contains("STOCK;") ? true : false);
            cbImport.SetCheck(record.AuthCd.Contains("IMPORT;") ? true : false);
            cbPurchase.SetCheck(record.AuthCd.Contains("PURCHASE;") ? true : false);

            // JINIT_20191210
            if (record.Image != null) imgUser.InvalidateSurface(record.Image);
            else imgUser.clear();
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }

        /*
        void Clear()
        {
            etUserID.SetEntryValue("");
            etUserID.SetReadOnly(true);
            etUserName.SetEntryValue("");

            etPassword.SetEntryValue("");
            etPhoneNumber.SetEntryValue("");
            etRole.SetSelecteItem(new SystemCode() { Id = record.RoleCd, Name = "" });
            etUsable.SetSelecteItem(new SystemCode() { Id = record.UseYn, Name = "" });
            etAddress.SetEntryValue("");


            //if (record.AuthCd.Length < 20)
            //{
            //    record.AuthCd += "00000000000000000000";
            //    record.AuthCd = record.AuthCd.Substring(0, 20);
            //}

            cbUseMgt.SetCheck(false);
            cbRefund.SetCheck(false);
            cbZReport.SetCheck(false);
            cbPrice.SetCheck(false);

            cbSaleRpt.SetCheck(false);
            cbSetting.SetCheck(false);
            cbAdjust.SetCheck(false);
            cbProforma.SetCheck(false);

            cbCustomer.SetCheck(false);
            cbStock.SetCheck(false);
            cbImport.SetCheck(false);
            cbPurchase.SetCheck(false);

        }
        */
    }
}

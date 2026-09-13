using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
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

namespace EBM2x.UI.Backoffice.CustomerManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerManagementPage : ContentPage
    {
        public ObservableCollection<TaxpayerBhfCustRecord> lvCustManagement { get; set; }
        List<TaxpayerBhfCustRecord> listTaxpayerBhfCustRecord;
        TaxpayerBhfCustMaster master = null;
        TaxpayerBhfCustRecord record = null;

        bool NewFlag = true;

        public CustomerManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBhfCustMaster();
            record = new TaxpayerBhfCustRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

            record.clear();
            SetEntityData(record, false);

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };

            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnImport.IsVisible = false;
                btnExport.IsVisible = false;
            }

            // JCNA 20200130 
            etTIN.SetReadOnly(false);
            etName.SetReadOnly(false);
            etNationality.SetReadOnly(true);

            etName.GetEntry().MaxLength = 100;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(listTaxpayerBhfCustRecord == null || listTaxpayerBhfCustRecord.Count <= 0)
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
            listTaxpayerBhfCustRecord = master.getTaxpayerBhfCustTable(tin, bhfid, "", etSearchUsable.GetSelectedItem().Id);
            SetList(listTaxpayerBhfCustRecord);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton , true: BackButton
        }

        async void OnSearch(object sender, EventArgs e)
        {
            string valueLike = etLikeValue.GetEntryValue();
            string valueUseYn = etSearchUsable.GetSelectedItem().Id;
            
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            listTaxpayerBhfCustRecord = master.getTaxpayerBhfCustTable(tin, bhfid, valueLike, valueUseYn);
            SetList(listTaxpayerBhfCustRecord);
            if (listTaxpayerBhfCustRecord == null || listTaxpayerBhfCustRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }

            record.clear();
            SetEntityData(record, false);
        }
        void SetList(List<TaxpayerBhfCustRecord> datas)
        {
            try
            {
                lvCustManagement = new ObservableCollection<TaxpayerBhfCustRecord>();
                listView.ItemsSource = lvCustManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvCustManagement.Add(datas[i]);
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

                        if (!dataTitle.Equals("TinBhfIdCustNo"))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file. [TaxpayerBhfCust.xlsx]", "OK");
                            break;
                        }

                        for (int i = 1; i < rcount; i++)
                        {
                            var row = rowList[i];
                            var cells = row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                            List<DocumentFormat.OpenXml.Spreadsheet.Cell> cellList = cells.ToList();

                            TaxpayerBhfCustRecord custRecord = new TaxpayerBhfCustRecord();
                            if (cellList.Count >= 1) custRecord.Tin = GetCellValue(excel, cellList[0]);      //cellList[0].CellValue.Text;                      // Taxpayer Identification Number(TIN)
                            if (string.IsNullOrEmpty(custRecord.Tin) || !custRecord.Tin.Equals(Tin))
                            {
                                continue;
                            }
                            if (cellList.Count > 1) custRecord.BhfId = GetCellValue(excel, cellList[1]);      //cellList[1].CellValue.Text;                // Branch Office ID
                            if (cellList.Count > 2) custRecord.CustNo = GetCellValue(excel, cellList[2]);      //cellList[2].CellValue.Text;               // Customer No.
                            if (cellList.Count > 3) custRecord.CustTin = GetCellValue(excel, cellList[3]);      //cellList[3].CellValue.Text;              // Customer Taxpayer Identification Number(TIN)
                            if (cellList.Count > 4) custRecord.CustBhfId = GetCellValue(excel, cellList[4]);      //cellList[4].CellValue.Text;            // Customer Branch ID
                            if (cellList.Count > 5) custRecord.CustNid = GetCellValue(excel, cellList[5]);      //cellList[5].CellValue.Text;              // Customer National Idetification
                            if (cellList.Count > 6) custRecord.CustNm = GetCellValue(excel, cellList[6]);      //cellList[6].CellValue.Text;               // Customer Name
                            if (cellList.Count > 7) custRecord.TelNo = GetCellValue(excel, cellList[7]);      //cellList[7].CellValue.Text;                // Telephone Number
                            if (cellList.Count > 8) custRecord.Adrs = GetCellValue(excel, cellList[8]);      //cellList[8].CellValue.Text;                 // Address
                            if (cellList.Count > 9) custRecord.UseYn = GetCellValue(excel, cellList[9]);      //cellList[9].CellValue.Text;                // Use(Y/N)
                            if (cellList.Count > 10) custRecord.RegrId = GetCellValue(excel, cellList[10]);      //cellList[10].CellValue.Text;             // Registrant ID
                            if (cellList.Count > 11) custRecord.RegrNm = GetCellValue(excel, cellList[11]);      //cellList[11].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 12) custRecord.RegDt = GetCellValue(excel, cellList[12]);      //cellList[12].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 13) custRecord.ModrId = GetCellValue(excel, cellList[13]);      //cellList[13].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 14) custRecord.ModrNm = GetCellValue(excel, cellList[14]);      //cellList[14].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 15) custRecord.ModDt = GetCellValue(excel, cellList[15]);      //cellList[15].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 16) custRecord.NationCd = GetCellValue(excel, cellList[16]);      //cellList[16].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 17) custRecord.ChargerNm = GetCellValue(excel, cellList[17]);      //cellList[17].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 18) custRecord.Contact1 = GetCellValue(excel, cellList[18]);      //cellList[18].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 19) custRecord.Contact2 = GetCellValue(excel, cellList[19]);      //cellList[19].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 20) custRecord.Email = GetCellValue(excel, cellList[20]);      //cellList[20].CellValue.Text;            // Registrant Name
                            if (cellList.Count > 21) custRecord.Fax = GetCellValue(excel, cellList[21]);      //cellList[21].CellValue.Text;              // Registrant Name
                            if (cellList.Count > 22) custRecord.Rm = GetCellValue(excel, cellList[22]);      //cellList[22].CellValue.Text;               // Registrant Name
                            if (cellList.Count > 23) custRecord.InitlUnclamt = GetDouble(GetCellValue(excel, cellList[23]));             // Registrant Name
                            if (cellList.Count > 24) custRecord.InitlNpyamt = GetDouble(GetCellValue(excel, cellList[24]));             // Registrant Name
                            if (cellList.Count > 25) custRecord.Unclamt = GetDouble(GetCellValue(excel, cellList[25]));             // Registrant Name
                            if (cellList.Count > 26) custRecord.Npyamt = GetDouble(GetCellValue(excel, cellList[26]));             // Registrant Name
                            if (cellList.Count > 27) custRecord.BcncType = GetCellValue(excel, cellList[27]);      //cellList[27].CellValue.Text;         // Registrant Name
                            if (cellList.Count > 28) custRecord.Bank = GetCellValue(excel, cellList[28]);      //cellList[28].CellValue.Text;             // Registrant Name
                            if (cellList.Count > 29) custRecord.Account = GetCellValue(excel, cellList[29]);      //cellList[29].CellValue.Text;          // Registrant Name
                            if (cellList.Count > 30) custRecord.Depositor = GetCellValue(excel, cellList[30]);      //cellList[30].CellValue.Text;        // Registrant Name
                            if (cellList.Count > 31) custRecord.CustGroup = GetCellValue(excel, cellList[31]);      //cellList[31].CellValue.Text;        // Registrant Name

                            custRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                            custRecord.ModrId = UIManager.Instance().UserModel.UserId;
                            custRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                            custRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                            custRecord.RegrId = UIManager.Instance().UserModel.UserId;
                            custRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

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

            if (listTaxpayerBhfCustRecord == null || listTaxpayerBhfCustRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listTaxpayerBhfCustRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    await iSave.SaveAndView("TaxpayerBhfCust_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionNew(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Info");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to create a new Customer?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            NewFlag = true;

            record.clear();
            // 저장 이력
            record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.ModrId = UIManager.Instance().UserModel.UserId;
            record.ModrNm = UIManager.Instance().UserModel.UserNm;
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;
            SetEntityData(record, false);

            etTIN.SetReadOnly(false);
        }

        // 저장
        async void OnFunctionSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etTIN.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the PIN.", "Ok");
                return;
            }

            if (etTIN.GetEntryValue().Length != 11)
            {
                //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the TIN [11 byte].", "Ok");
                //return;
            }

            if (string.IsNullOrEmpty(etName.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Cust Name.", "Ok");
                return;
            }

            if (!string.IsNullOrEmpty(etEMail.GetEntryValue()))
            {
                if (!etEMail.IsValid())
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "not a well-formed e-mail address.", "Ok");
                    return;
                }
            }

            // check_existing()
            // checkRealTIN(strBcncId)

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to save this customer account?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;

            record.CustNo = etTIN.GetEntryValue();
            record.CustTin = etTIN.GetEntryValue();
            record.CustNm = etName.GetEntryValue();

            record.BcncType = etType.GetSelectedItem().Id;

            record.ChargerNm = etDelegator.GetEntryValue();
            record.NationName = etNationality.GetEntryValue();
            record.Contact1 = etPhone1.GetEntryValue();
            record.Contact2 = etPhone2.GetEntryValue();
            record.Email = etEMail.GetEntryValue();
            record.Fax = etFaxNo.GetEntryValue();
            record.Adrs = etAddress.GetEntryValue();

            record.Bank = etBank.GetEntryValue();
            record.Account = etAccount.GetEntryValue();
            record.Depositor = etDepositor.GetEntryValue();
            record.Adrs = etAddress.GetEntryValue();

            record.Rm = etRemark.GetEntryValue();
            record.UseYn = etUse.GetSelectedItem().Id;
            record.CustGroup = etCustGroup.GetSelectedItem().Id;

            record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.ModrId = UIManager.Instance().UserModel.UserId;
            record.ModrNm = UIManager.Instance().UserModel.UserNm;
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;

            if (NewFlag)
            {
                TaxpayerBhfCustRecord recordNew = new TaxpayerBhfCustRecord();
                bool exist = master.ToRecord(recordNew, record.Tin, record.BhfId, record.CustNo);
                if (exist)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This code already exists.", "Ok");
                    return;
                }
            }

            bool retSaved = master.ToTable(record);
            if (retSaved)
            {
                //===>>>>>>>>>
                //JCNA 20191204
                BhfCustRraSdcUpload bhfCustRraSdcUpload = new BhfCustRraSdcUpload();
                bhfCustRraSdcUpload.SendBhfCustSave(record.Tin, record.BhfId, record.CustNo);

                //===>>>>>>>>>
                // JCNA 20191204 TR 전송 명령 실행
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully.", "Ok");

                string valueLike = etLikeValue.GetEntryValue();
                string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                List<TaxpayerBhfCustRecord> list = master.getTaxpayerBhfCustTable(tin, bhfid, valueLike, valueUseYn);
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                etTIN.SetReadOnly(false);
                NewFlag = true;
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please contact admin.", "Ok");
            }
        }

        async void OnFunctionDelete(object sender, EventArgs e)
        {
            if (!etTIN.IsValid())
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Delete?", "Please select the data you want to delete.", "Ok");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to delete the customer " + etName.GetEntryValue() + "?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            record.CustNo = etTIN.GetEntryValue();
            record.CustTin = etTIN.GetEntryValue();

            bool retDelete = master.DeleteTable(record);
            if (retDelete)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Customer deleted succcessfully.", "OK");

                string valueLike = etLikeValue.GetEntryValue();
                string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                List<TaxpayerBhfCustRecord> list = master.getTaxpayerBhfCustTable(tin, bhfid, valueLike, valueUseYn);
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                etTIN.SetReadOnly(false);
                NewFlag = true;

            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Delete failed. Please check database.", "OK");
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
                record = (TaxpayerBhfCustRecord)e.SelectedItem;
                // 저장 이력
                record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                record.RegrId = UIManager.Instance().UserModel.UserId;
                record.RegrNm = UIManager.Instance().UserModel.UserNm;
                SetEntityData(record, true);
                etTIN.SetReadOnly(true);
                NewFlag = false;

            }
        }

        private void SetEntityData(TaxpayerBhfCustRecord record, bool readOnly)
        {
            etTIN.SetEntryValue(record.CustTin);
            etName.SetEntryValue(record.CustNm);
 
            etType.SetSelecteItem(new SystemCode() { Id = record.BcncType, Name = "" });

            etDelegator.SetEntryValue(record.ChargerNm);
            etNationality.SetEntryValue(record.NationName);
            etPhone1.SetEntryValue(record.Contact1);
            etPhone2.SetEntryValue(record.Contact2);
            etEMail.SetEntryValue(record.Email);
            etFaxNo.SetEntryValue(record.Fax);
            etAddress.SetEntryValue(record.Adrs);

            etBank.SetEntryValue(record.Bank);
            etAccount.SetEntryValue(record.Account);
            etDepositor.SetEntryValue(record.Depositor);
            etAddress.SetEntryValue(record.Adrs);

            etRemark.SetEntryValue(record.Rm);
            etUse.SetSelecteItem(new SystemCode() { Id = record.UseYn, Name = "" });
            etCustGroup.SetSelecteItem(new SystemCode() { Id = record.CustGroup, Name = "" });
        }

        async void OnFunctionQueryTaxpayer(object sender, EventArgs e)
        {
            var popupPage = new TaxpayerPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerBaseRecord popupRecord = (TaxpayerBaseRecord)((ExtEventArgs)ex).EnteredObject;
                    record.CustTin = popupRecord.Tin;
                    record.CustNm = popupRecord.TaxprNm;

                    etTIN.SetEntryValue(popupRecord.Tin);
                    etName.SetEntryValue(popupRecord.TaxprNm);

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
                    record.NationCd = popupRecord.Cd;

                    etNationality.SetEntryValue(popupRecord.CdNm);

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

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }
    }
}

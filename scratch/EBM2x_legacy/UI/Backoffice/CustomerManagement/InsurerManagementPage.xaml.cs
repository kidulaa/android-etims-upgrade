using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.RraSdc.process;
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
    public partial class InsurerManagementPage : ContentPage
    {
        public ObservableCollection<TaxpayerBhfInsuranceRecord> lvInsuranceManagement { get; set; }
        List<TaxpayerBhfInsuranceRecord> listTaxpayerBhfInsuranceRecord;
        TaxpayerBhfInsuranceMaster master = null;
        TaxpayerBhfInsuranceRecord record = null;

        bool NewFlag = true;
        
        public InsurerManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBhfInsuranceMaster();
            record = new TaxpayerBhfInsuranceRecord();

            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnImport.IsVisible = false;
                btnExport.IsVisible = false;
            }

            etInsurerName.GetEntry().MaxLength = 100;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AnimationLoop();
            /*
            DrawGridHead drawGridHead;

            drawGridHead = new DrawGridHead { Text = "Code" };
            listViewHeader.Children.Add(drawGridHead, 0, 6, 0, 1);
            drawGridHead = new DrawGridHead { Text = "Insurer Name" };
            listViewHeader.Children.Add(drawGridHead, 7, 13, 0, 1);
            drawGridHead = new DrawGridHead { Text = "Rate" };
            listViewHeader.Children.Add(drawGridHead, 14, 21, 0, 1);
            */
           
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            listTaxpayerBhfInsuranceRecord = master.getTaxpayerBhfInsuranceTable();
            SetList(listTaxpayerBhfInsuranceRecord);

            record.clear();
            SetEntityData(record, false);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton , true: BackButton
        }
        async void OnSearch(object sender, EventArgs e)
        {
            listTaxpayerBhfInsuranceRecord = master.getTaxpayerBhfInsuranceTable();
            SetList(listTaxpayerBhfInsuranceRecord);
            if (listTaxpayerBhfInsuranceRecord == null || listTaxpayerBhfInsuranceRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<TaxpayerBhfInsuranceRecord> datas)
        {
            try
            {
                lvInsuranceManagement = new ObservableCollection<TaxpayerBhfInsuranceRecord>();
                listView.ItemsSource = lvInsuranceManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvInsuranceManagement.Add(datas[i]);
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

                        if (!dataTitle.Equals("TinBhfIdIssrccCd"))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Check the selected excel file. [TaxpayerBhfInsurance.xlsx]", "OK");
                            break;
                        }

                        for (int i = 1; i < rcount; i++)
                        {
                            var row = rowList[i];
                            var cells = row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                            List<DocumentFormat.OpenXml.Spreadsheet.Cell> cellList = cells.ToList();

                            TaxpayerBhfInsuranceRecord custRecord = new TaxpayerBhfInsuranceRecord();

                            if (cellList.Count >= 1) custRecord.Tin = GetCellValue(excel, cellList[0]);      //cellList[0].CellValue.Text;                  // Taxpayer Identification Number(TIN)
                            if (string.IsNullOrEmpty(custRecord.Tin) || !custRecord.Tin.Equals(Tin))
                            {
                                continue;
                            }
                            if (cellList.Count > 1) custRecord.BhfId = GetCellValue(excel, cellList[1]);      //cellList[1].CellValue.Text;            // Branch Office ID
                            if (cellList.Count > 2) custRecord.IssrccCd = GetCellValue(excel, cellList[2]);      //cellList[2].CellValue.Text;             // Insurance Company Code
                            if (cellList.Count > 3) custRecord.IsrccNm = GetCellValue(excel, cellList[3]);      //cellList[3].CellValue.Text;              // Insurance Company Name
                            if (cellList.Count > 4) custRecord.IsrcRt = GetInt(GetCellValue(excel, cellList[4]));       // Insurance Rate
                            if (cellList.Count > 5) custRecord.UseYn = GetCellValue(excel, cellList[5]);      //cellList[5].CellValue.Text;            // Use(Y/N)
                            if (cellList.Count > 6) custRecord.ModrId = GetCellValue(excel, cellList[6]);      //cellList[6].CellValue.Text;           // Registrant Name
                            if (cellList.Count > 7) custRecord.ModrNm = GetCellValue(excel, cellList[7]);      //cellList[7].CellValue.Text;           // Registrant Name
                            if (cellList.Count > 8) custRecord.ModDt = GetCellValue(excel, cellList[8]);      //cellList[8].CellValue.Text;            // Registrant Name
                            if (cellList.Count > 9) custRecord.RegrId = GetCellValue(excel, cellList[9]);      //cellList[9].CellValue.Text;           // Registrant ID
                            if (cellList.Count > 10) custRecord.RegrNm = GetCellValue(excel, cellList[10]);      //cellList[10].CellValue.Text;         // Registrant Name
                            if (cellList.Count > 11) custRecord.RegDt = GetCellValue(excel, cellList[11]);      //cellList[11].CellValue.Text;          // Registrant Name

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
        public int GetInt(string value)
        {
            int dval = 0;
            try
            {
                dval = int.Parse(value);
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
            
            if (listTaxpayerBhfInsuranceRecord == null || listTaxpayerBhfInsuranceRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listTaxpayerBhfInsuranceRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TaxpayerBhfInsurance_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionNew(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Info");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to create a new Insurance?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            NewFlag = true;

            record.clear();
            record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.ModrId = UIManager.Instance().UserModel.UserId;
            record.ModrNm = UIManager.Instance().UserModel.UserNm;
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;
            SetEntityData(record, false);
            etCode.SetFocus();
        }

        async void OnFunctionSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Code.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(etInsurerName.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Name.", "Ok");
                return;
            }
            if (etRate.GetEntryValue() <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Enter a value between 1 and 99.", "Ok");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to save this Insurance?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            record.IssrccCd = etCode.GetEntryValue();
            record.IsrccNm = etInsurerName.GetEntryValue();
            record.IsrcRt = etRate.GetEntryValue();

            record.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.ModrId = UIManager.Instance().UserModel.UserId;
            record.ModrNm = UIManager.Instance().UserModel.UserNm;
            record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record.RegrId = UIManager.Instance().UserModel.UserId;
            record.RegrNm = UIManager.Instance().UserModel.UserNm;


            if (NewFlag)
            {
                TaxpayerBhfInsuranceRecord recordNew = new TaxpayerBhfInsuranceRecord();
                bool exist = master.ToRecord(recordNew, record.Tin, record.BhfId, record.IssrccCd);
                if(exist)
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
                BhfInsuranceRraSdcUpload BhfInsuranceRraSdcUpload = new BhfInsuranceRraSdcUpload();
                BhfInsuranceRraSdcUpload.SendBhfInsuranceSave(record.Tin, record.BhfId, record.IssrccCd);

                //===>>>>>>>>>
                // JCNA 20191204 TR 
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully.", "Ok");

                List<TaxpayerBhfInsuranceRecord> list = master.getTaxpayerBhfInsuranceTable();
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                etCode.SetFocus();
                NewFlag = true;
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please contact admin.", "Ok");
            }
        }

        async void OnFunctionDelete(object sender, EventArgs e)
        {
            if(!etCode.IsValid())
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Delete?", "Please select the data you want to delete.", "Ok");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to delete the Insurance " + etInsurerName.GetEntryValue() + "?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            record.IssrccCd = etCode.GetEntryValue();

            bool retDeleted = master.DeleteTable(record);
            if (retDeleted)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Insurance deleted succcessfully.", "OK");

                List<TaxpayerBhfInsuranceRecord> list = master.getTaxpayerBhfInsuranceTable();
                SetList(list);

                record.clear();
                SetEntityData(record, false);
                etCode.SetFocus();
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
                record = (TaxpayerBhfInsuranceRecord)e.SelectedItem;
                record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                record.RegrId = UIManager.Instance().UserModel.UserId;
                record.RegrNm = UIManager.Instance().UserModel.UserNm;
                SetEntityData(record, true);
                etRate.SetFocus();
                NewFlag = false;
            }
        }

        private void SetEntityData(TaxpayerBhfInsuranceRecord record, bool readOnly)
        {
            etCode.SetEntryValue(record.IssrccCd);
            etCode.SetReadOnly(readOnly);
            etInsurerName.SetEntryValue(record.IsrccNm);
            etRate.SetEntryValue(record.IsrcRt);
        }
    }
}

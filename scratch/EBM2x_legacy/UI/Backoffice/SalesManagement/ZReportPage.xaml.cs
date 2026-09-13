using EBM2x.Database.Excel;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Dependency;
using EBM2x.Models.journal;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZReportPage : ContentPage
    {
        List<OpeningCloseingStockRecord> listOpeningCloseingStockRecord;
        StockOutHistoryMaster master = null;
        OpeningCloseingStockRecord record = null;

        public ZReportPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new StockOutHistoryMaster();
            record = new OpeningCloseingStockRecord();

            etFromDate.GetDatePicker().DateSelected += (sender, e) => {
                if (etFromDate.GetDateTime() > DateTime.Now)
                {
                    etFromDate.SetEntryValue(DateTime.Now);
                }
                if (etFromDate.GetDateTime() > etToDate.GetDateTime())
                {
                    etToDate.SetEntryValue(etFromDate.GetDateTime());
                }
            };
            etToDate.GetDatePicker().DateSelected += (sender, e) => {
                if (etToDate.GetDateTime() > DateTime.Now)
                {
                    etToDate.SetEntryValue(DateTime.Now);
                }
                if (etFromDate.GetDateTime() > etToDate.GetDateTime())
                {
                    etFromDate.SetEntryValue(etToDate.GetDateTime());
                }
            }; 
            
            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnExport.IsVisible = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionExport(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            JournalModel journal = GetJournalModel();
            CreateExcelUtil createExcelUtil = new CreateExcelUtil();
            string jsonRequest = JsonConvert.SerializeObject(journal.JournalStringList);

            ISave iSave = DependencyService.Get<ISave>();
            if (iSave != null)
            {
                MemoryStream stream = new MemoryStream();
                createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                await iSave.SaveAndView("ZReport_" + dt + ".xlsx", "", stream);
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        JournalModel GetJournalModel()
        {
            double amount = 0;
            int count = 0;

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");

            SalesReportMaster salesReportMaster = new SalesReportMaster();

            JournalModel journal = new JournalModel();
            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            // Header
            journal.Add("bold", envPosSetup.GblTaxIdNm);
            journal.Add("", envPosSetup.GblBrcAdr);
            journal.Add("", "TEL: " + envPosSetup.GblBrcTel);
            journal.Add("", "EMAIL: " + envPosSetup.GblBrcEmail);
            journal.Add("", "PIN: " + envPosSetup.GblTaxIdNo);

            string fromStrText = etFromDate.GetEntryValue().ToString("dd-MM-yyyy");
            string toStrText = etToDate.GetEntryValue().ToString("dd-MM-yyyy");
            journal.Add("DATE: " + fromStrText + " ~ " + toStrText);
            journal.Add("");
            journal.Add("--------------SALES-----------------");
            count = salesReportMaster.GetCodeCount(fromStr, toStr, "N", "S");
            journal.Add(string.Format("TOTAL NUMBER SALES NS: {0}", count));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TOT_AMT");
            journal.Add(string.Format("TOTAL AMOUNT SALES NS: {0:##,##0.00}", amount));
            count = salesReportMaster.GetCodeCount(fromStr, toStr, "C", "S");
            journal.Add(string.Format("TOTAL NUMBER COPY SALES NS: {0}", count));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "C", "S", "TRNS_SALE.TOT_AMT");
            journal.Add(string.Format("TOTAL AMOUNT COPY SALES NS: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("--------------REFUND----------------");
            count = salesReportMaster.GetCodeCount(fromStr, toStr, "N", "R");
            journal.Add(string.Format("TOTAL NUMBER REFUND NR: {0}", count));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TOT_AMT");
            journal.Add(string.Format("TOTAL AMOUNT REFUND NR: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("--------------SALE TAX--------------");
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_A");
            journal.Add(string.Format("TOTAL TAXABLE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_B");
            journal.Add(string.Format("TOTAL TAXABLE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_C");
            journal.Add(string.Format("TOTAL TAXABLE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAXBL_AMT_D");
            journal.Add(string.Format("TOTAL TAXABLE D: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_A");
            journal.Add(string.Format("TOTAL TAXE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_B");
            journal.Add(string.Format("TOTAL TAXE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_C");
            journal.Add(string.Format("TOTAL TAXE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "S", "TRNS_SALE.TAX_AMT_D");
            journal.Add(string.Format("TOTAL TAXE D: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("--------------REFUND TAX-----------");
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_A");
            journal.Add(string.Format("TOTAL TAXABLE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_B");
            journal.Add(string.Format("TOTAL TAXABLE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_C");
            journal.Add(string.Format("TOTAL TAXABLE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAXBL_AMT_D");
            journal.Add(string.Format("TOTAL TAXABLE D: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_A");
            journal.Add(string.Format("TOTAL TAXE A-EX: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_B");
            journal.Add(string.Format("TOTAL TAXE B: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_C");
            journal.Add(string.Format("TOTAL TAXE C: {0:##,##0.00}", amount));
            amount = salesReportMaster.GetCodeValue(fromStr, toStr, "N", "R", "TRNS_SALE.TAX_AMT_D");
            journal.Add(string.Format("TOTAL TAXE D: {0:##,##0.00}", amount));
            journal.Add("");
            journal.Add("Date : " + DateTime.Now.ToString("dd-MM-yyyy") + "  " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
            journal.Add("--------------END------------------");
            journal.Add("");
            journal.Add("");

            journal.Add("cutpaper", string.Empty);  //밑에 공백을 주는 부분

            return journal;
        }

        async void OnFunctionPrintReport(object sender, EventArgs e)
        {
            JournalModel journal = GetJournalModel();

            // PRINTING
            var popupPage = new SelectPrinterPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    string paymentName = (string)((ExtEventArgs)ex).EnteredText;
                    Navigation.PopAsync();
                    if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintReceipt"))
                    {
                        PrintingService printingService = new PrintingService();
                        printingService.writeJurnal(journal, false);
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Print the document.", "OK");
                    }
                    else if(!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintA4"))
                    {
                        PrintingService printingService = new PrintingService();
                        printingService.writeJurnalA4(journal, false);
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Print the document.", "OK");
                    }
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        private void OnFunctionQuery(object sender, EventArgs e)
        {
            JournalModel journal = GetJournalModel();

            try
            {
                ObservableCollection<JournalString>  lvItemManagement = new ObservableCollection<JournalString>();
                listView.ItemsSource = lvItemManagement;

                for (int i = 0; i < journal.JournalStringList.Count; i++)
                {
                    lvItemManagement.Add(journal.JournalStringList[i]);
                }
            }
            catch
            {
            }
        }
    }
}

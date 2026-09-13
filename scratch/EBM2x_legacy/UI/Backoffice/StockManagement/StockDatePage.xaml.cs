using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.UI.i18n;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StockDatePage : ContentPage
    {
        public ObservableCollection<StockDateRecord> lvItemManagement { get; set; }
        List<StockDateRecord> listOpeningCloseingStockRecord;
        StockDateMaster master = null;
        StockDateRecord record = null;

        public StockDatePage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new StockDateMaster();
            record = new StockDateRecord();

            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnExport.IsVisible = false;
            }

            dpDate.GetDatePicker().DateSelected += (sender, e) => {
                if (dpDate.GetDateTime() > DateTime.Now)
                {
                    dpDate.SetEntryValue(DateTime.Now);
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            string fromStr = dpDate.GetEntryValue().ToString("yyyyMMdd");
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listOpeningCloseingStockRecord = master.getStackDateTable(fromStr);
            SetList(listOpeningCloseingStockRecord);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnSearch(object sender, EventArgs e)
        {
            string fromStr = dpDate.GetEntryValue().ToString("yyyyMMdd");

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listOpeningCloseingStockRecord = master.getStackDateTable(fromStr);
            SetList(listOpeningCloseingStockRecord);
            if (listOpeningCloseingStockRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<StockDateRecord> datas)
        {
            try
            {
                lvItemManagement = new ObservableCollection<StockDateRecord>();
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

        async void OnFunctionExport(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (listOpeningCloseingStockRecord == null || listOpeningCloseingStockRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listOpeningCloseingStockRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("SalesReport_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnFunctionPrintReport(object sender, EventArgs e)
        {

        }
    }
}

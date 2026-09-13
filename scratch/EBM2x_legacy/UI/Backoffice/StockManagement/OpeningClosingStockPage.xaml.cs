using EBM2x.Database.Excel;
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
    public partial class OpeningClosingStockPage : ContentPage
    {
        public ObservableCollection<OpeningCloseingStockRecord> lvItemManagement { get; set; }
        List<OpeningCloseingStockRecord> listOpeningCloseingStockRecord;
        OpeningCloseingStockMaster master = null;
        OpeningCloseingStockRecord record = null;

        public OpeningClosingStockPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            string AuthCd = UIManager.Instance().UserModel.AuthCd;
            if (UIManager.Instance().UserModel.RoleCd != "1")
            {
                if (!AuthCd.Contains("ADJUST;")) {
                    btnAdjust.IsVisible = false;
                }
            }

            master = new OpeningCloseingStockMaster();
            record = new OpeningCloseingStockRecord();

            DateTime today = DateTime.Now;
            DateTime answer = today.AddDays(-7);

            etFromDate.SetDateTime(answer); 
            etToDate.SetDateTime(today);

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

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listOpeningCloseingStockRecord = master.getOpeningCloseingStockTable(fromStr, toStr, etLikeValue.GetEntryValue());
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

        async void OnSearch(object sender, System.EventArgs e)
        {
            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listOpeningCloseingStockRecord = master.getOpeningCloseingStockTable(fromStr, toStr, etLikeValue.GetEntryValue());
            SetList(listOpeningCloseingStockRecord);
            if (listOpeningCloseingStockRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<OpeningCloseingStockRecord> datas)
        {
            try
            {
                lvItemManagement = new ObservableCollection<OpeningCloseingStockRecord>();
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

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                record = (OpeningCloseingStockRecord)e.SelectedItem;
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
                    await iSave.SaveAndView("OpeningCloseingStock_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnFunctionAdjust(object sender, EventArgs e)
        {
            //// 권한 Check
            //if(GblUsrRole == "1" || GblUsrAuth.Contains("ADJUST"))
            //{
            //}
            //else
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Access prohibited", "You are not allowed to access the item adjustment .", "OK");
            //    return;
            //}

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to adjust the stock?");
            var retNet = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (retNet)
            {
                if (record == null || string.IsNullOrEmpty(record.ItemCd))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select an Item.", "OK");
                }
                else
                {
                    await Navigation.PushAsync(new StockAdjustmentPage(record.ItemCd));
                }
            }
        }
    }
}

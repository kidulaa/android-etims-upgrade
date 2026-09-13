using EBM2x.Database.Excel;
using EBM2x.Database.Master;
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
    public partial class SalesManagementPage : ContentPage
    {
        public ObservableCollection<TrnsSaleRecord> lvTranManagement { get; set; }
        public ObservableCollection<TrnsSaleItemRecord> lvItemsManagement { get; set; }
        List<TrnsSaleRecord> listTrnsSaleRecord;
        List<TrnsSaleItemRecord> listTrnsSaleItemRecord;
        TrnsSaleMaster masterTran = null;
        TrnsSaleRecord recordTran = null;

        TrnsSaleItemMaster masterItems = null;
        TrnsSaleItemRecord recordItems = null;

        double TotalVAT = 0;
        double Total = 0;

        public SalesManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            string AuthCd = UIManager.Instance().UserModel.AuthCd;
            if (UIManager.Instance().UserModel.RoleCd != "1")
            {
                if(!AuthCd.Contains("SALERPT;"))
                {
                    btnSalesReport.IsVisible = false;
                }
                if (!AuthCd.Contains("ZREPORT;"))
                {
                    btnZReport.IsVisible = false;
                }
            }

            masterTran = new TrnsSaleMaster();
            recordTran = new TrnsSaleRecord();
            masterItems = new TrnsSaleItemMaster();
            recordItems = new TrnsSaleItemRecord();

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
                btnExportVAT.IsVisible = false;
                btnExport.IsVisible = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            etTotalVAT.SetEntryValue(TotalVAT.ToString("#,##0.00"));
            etTotalVAT.SetReadOnly(true);
            etTotal.SetEntryValue(Total.ToString("#,##0.00"));
            etTotal.SetReadOnly(true);

            etSelectStatus.SetReadOnly(true);
            etSelectInvoiceID.SetReadOnly(true);
            etSelectCustomer.SetReadOnly(true);

            etSelectVAT.SetReadOnly(true);
            etSelectTotalAmount.SetReadOnly(true);
            etSelectSalesAmount.SetReadOnly(true);

            OnSearch();
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
            if (listTrnsSaleItemRecord != null)
            {
                listTrnsSaleItemRecord.Clear();
                SetListItems(listTrnsSaleItemRecord);
                SetEntityData(new TrnsSaleRecord(), true);
            }

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            long InvoiceNo = etInvoice.GetEntryValue();
            string valueSalesStts = etSalesStts.GetSelectedItem().Id;

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTrnsSaleRecord = masterTran.getTrnsSaleTable(fromStr, toStr, InvoiceNo, valueSalesStts);
            SetList(listTrnsSaleRecord);
            if (listTrnsSaleRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        async void OnSearch()
        {
            if (listTrnsSaleItemRecord != null)
            {
                listTrnsSaleItemRecord.Clear();
                SetListItems(listTrnsSaleItemRecord);
                SetEntityData(new TrnsSaleRecord(), true);
            }

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            long InvoiceNo = etInvoice.GetEntryValue();
            string valueSalesStts = etSalesStts.GetSelectedItem().Id;

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTrnsSaleRecord = masterTran.getTrnsSaleTable(fromStr, toStr, InvoiceNo, valueSalesStts);
            SetList(listTrnsSaleRecord);
        }
        void SetList(List<TrnsSaleRecord> datas)
        {
            try
            {
                lvTranManagement = new ObservableCollection<TrnsSaleRecord>();
                listViewTran.ItemsSource = lvTranManagement;

                TotalVAT = 0;
                Total = 0;
                for (int i = 0; i < datas.Count; i++)
                {
                    TotalVAT += datas[i].TotTaxAmt;
                    Total += datas[i].TotAmt;
                    lvTranManagement.Add(datas[i]);
                }
                etTotalVAT.SetEntryValue(TotalVAT.ToString("#,##0.00"));
                etTotal.SetEntryValue(Total.ToString("#,##0.00"));
            }
            catch
            {
            }
        }
        async void OnSearchItems(TrnsSaleRecord record)
        {
            // JINIT_20191201, 
            //listTrnsSaleItemRecord = masterItems.getTrnsSaleItemTable(record.Tin, record.BhfId, record.InvcNo);
            listTrnsSaleItemRecord = masterItems.getTrnsSaleItemTable(record.Tin, record.BhfId, record.InvcNo, record.RcptTyCd);

            SetListItems(listTrnsSaleItemRecord);
            if (listTrnsSaleItemRecord == null || listTrnsSaleItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetListItems(List<TrnsSaleItemRecord> datas)
        {
            try
            {
                lvItemsManagement = new ObservableCollection<TrnsSaleItemRecord>();
                listViewItems.ItemsSource = lvItemsManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvItemsManagement.Add(datas[i]);
                }
            }
            catch
            {
            }
        }

        async void OnZReport(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ZReportPage());
            //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "You are not allowed to print the Zreport.", "OK");
            
        }

        async void OnSalesReport(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SalesReportPage());
            //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "You are not allowed to print the Sales report.", "OK");
            
        }

        async void OnFunctionExportVAT(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");

            List<TrnsSaleRecordVAT> list = masterTran.getTrnsSaleTableVAT(fromStr, toStr);

            if (list == null || list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
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
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, titleList, jsonRequest);
                    //createExcelUtil.CreateSpreadsheetWorkbook(stream, titleList, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    string filename = "SalesReport_" + dt + ".xlsx";

                    await iSave.SaveAndView(filename, "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }
        async void OnFunctionExportAll(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (listTrnsSaleRecord == null || listTrnsSaleRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateSalesExcelUtil createExcelUtil = new CreateSalesExcelUtil();

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, listTrnsSaleRecord);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    await iSave.SaveAndView("TrnsSaleItem_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }
        async void OnFunctionExport(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (listTrnsSaleItemRecord == null || listTrnsSaleItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listTrnsSaleItemRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TrnsSaleItem_" + dt + ".xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionDelete(object sender, EventArgs e)
        {
            /* JINIT_20191208,
          if (recordTran == null || recordTran.InvcNo <= 0)
           {
               EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select an Invoice to delete.", "OK");
           }
           else
           {
               if(recordTran.SalesSttsCd.Equals("01"))
               {
                   bool retDelete = EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Do you want to delete this invoice?", "Yes", "No");
                   if(retDelete)
                   {
               //strQuery = ""
               //strQuery = " UPDATE TRNSALE "
               //strQuery = strQuery & "     SET " & vbCrLf
               //strQuery = strQuery + "         COMM_F  = 'D' "                     '송수신구분
               //strQuery = strQuery & "   WHERE INV_ID = '" + strInvId.Trim() + "' "
               //strQuery = strQuery & "     AND BHF_ID = '" + strBhfId.Trim() + "' "
               //queries.Add(strQuery)
               //
               //strQuery = ""
               //strQuery = " UPDATE TRNSALEITEM "
               //strQuery = strQuery & "     SET " & vbCrLf
               //strQuery = strQuery + "         COMM_F  = 'D' "                     '송수신구분
               //strQuery = strQuery & "   WHERE INV_ID = '" + strInvId.Trim() + "' "
               //strQuery = strQuery & "     AND BHF_ID = '" + strBhfId.Trim() + "' "
               //queries.Add(strQuery)
               //
               //strQuery = ""
               //strQuery = " UPDATE TRNRECEIPT "
               //strQuery = strQuery & "     SET " & vbCrLf
               //strQuery = strQuery + "         COMM_F  = 'D' "                     '송수신구분
               //strQuery = strQuery & "   WHERE INV_ID = '" + strInvId.Trim() + "' "
               //strQuery = strQuery & "     AND BHF_ID = '" + strBhfId.Trim() + "' "
               //queries.Add(strQuery)
               //
               //strQuery = ""
               //strQuery = " UPDATE TRNRECEIPTITEM "
               //strQuery = strQuery & "     SET " & vbCrLf
               //strQuery = strQuery + "         COMM_F  = 'D' "                     '송수신구분
               //strQuery = strQuery & "   WHERE INV_ID = '" + strInvId.Trim() + "' "
               //strQuery = strQuery & "     AND BHF_ID = '" + strBhfId.Trim() + "' "
               //queries.Add(strQuery)


            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Item deleted succcessfully.", "OK");

                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Delete failed. Please check database.", "OK");
                    }
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This invoice status is not [open].Cannot be deleted!", "OK");
                }
            }
            */

            if (recordTran == null || recordTran.InvcNo <= 0 || listTrnsSaleItemRecord == null || listTrnsSaleItemRecord.Count <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select an Invoice to delete.", "OK");
                return;
            }

            if (recordTran.SalesSttsCd.Equals("01"))
            {
                //bool retDelete = EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Do you want to delete this invoice?", "Yes", "No");
                string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
                string locationMessage21 = UILocation.Instance().GetLocationText("Are you sure you want to delete this Sale Invoice?");
                var retDelete = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
                if (retDelete)
                {
                    bool ret = false;
                    TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
                    if (trnsSaleMaster.DeleteTable(recordTran))
                    {
                        TrnsSaleItemMaster trnsSaleItemMaster = new TrnsSaleItemMaster();
                        if (trnsSaleItemMaster.DeleteTable(recordTran))
                        {
                            OnSearch();
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Item deleted succcessfully.", "OK");
                        }
                        else
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to delete the item. Please contact [Support].", "OK");
                        }
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to delete the item. Please contact [Support].", "OK");
                    }
                }
                {
                }
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This sale is not in [Waiting] status. Unable to delete!", "OK");
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnFunctionNew(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to proceed to New Invoice?");
            var retNet = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (retNet)
            {
                var popupPage = new SaleRegistrationPage();
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
        }

        async void OnFunctionDetail(object sender, EventArgs e)
        {
            if (recordTran == null || recordTran.InvcNo <= 0 || listTrnsSaleItemRecord == null || listTrnsSaleItemRecord.Count <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a sales transaction.", "OK");
            }
            else
            {
                //if(!string.IsNullOrEmpty(recordTran.PrchrAcptcYn) && recordTran.PrchrAcptcYn.Equals("Y"))
                //{
                //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This sale is not in [Waiting] status. Unable to 'Detail'!", "OK");
                //    return;
                //}

                var popupPage = new DetailInformationOfSalePage(recordTran);
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
        }

        async void OnFunctionModify(object sender, EventArgs e)
        {
            if (recordTran == null || !recordTran.SalesSttsCd.Equals("01") || listTrnsSaleItemRecord == null || listTrnsSaleItemRecord.Count <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This invoice cannot be modified.", "OK");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to modify the current invoice?");
            var retNet = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (retNet)
            {
                var popupPage = new SaleRegistrationPage(recordTran, listTrnsSaleItemRecord);
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
        }

        private void OnSelectedTran(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                recordTran = (TrnsSaleRecord)e.SelectedItem;
                OnSearchItems(recordTran);
                SetEntityData(recordTran, true);
            }
        }

        private void SetEntityData(TrnsSaleRecord record, bool readOnly)
        {
            etSelectStatus.SetEntryValue(record.SalesSttsNm);
            etSelectInvoiceID.SetEntryValue(record.InvcNo);
            etSelectCustomer.SetEntryValue(record.TradeNm);

            etSelectVAT.SetEntryValue(record.TotTaxAmt.ToString("#,##0.00"));
            etSelectTotalAmount.SetEntryValue(record.TotTaxblAmt.ToString("#,##0.00"));
            etSelectSalesAmount.SetEntryValue(record.TotAmt.ToString("#,##0.00"));
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSalesStts.SetSelecteItem(new Models.config.SystemCode());
        }

    }
}

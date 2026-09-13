using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.i18n;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.PurchaseManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PurchaseManagementPage : ContentPage
    {
        public ObservableCollection<TrnsPurchaseRecord> lvTranManagement { get; set; }
        public ObservableCollection<TrnsPurchaseItemRecord> lvItemsManagement { get; set; }
        List<TrnsPurchaseRecord> listTrnsPurchaseRecord;
        List<TrnsPurchaseItemRecord> listTrnsPurchaseItemRecord;
        TrnsPurchaseMaster masterTran = null;
        TrnsPurchaseRecord recordTran = null;

        TrnsPurchaseItemMaster masterItems = null;
        TrnsPurchaseItemRecord recordItems = null;

        double TotalVAT = 0;
        double Total = 0;
        public PurchaseManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            masterTran = new TrnsPurchaseMaster();
            recordTran = new TrnsPurchaseRecord();
            masterItems = new TrnsPurchaseItemMaster();
            recordItems = new TrnsPurchaseItemRecord();

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
            etSelectSupplier.SetReadOnly(true);

            etSelectVAT.SetReadOnly(true);
            etSelectPurchaseAmount.SetReadOnly(true);

            //if (listTrnsPurchaseItemRecord == null || listTrnsPurchaseItemRecord.Count <= 0)
            //{
                OnSearch();
            //}
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; 
        }

        async void OnSearch(object sender, System.EventArgs e)
        {
            if (listTrnsPurchaseItemRecord != null)
            {
                listTrnsPurchaseItemRecord.Clear();
                SetListItems(listTrnsPurchaseItemRecord);
                SetEntityData(new TrnsPurchaseRecord(), true);
            }

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            long InvoiceNo = etInvoice.GetEntryValue();
            string valuePchsStts = etPchsStts.GetSelectedItem().Id;

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTrnsPurchaseRecord = masterTran.getTrnsPurchaseTable(fromStr, toStr, InvoiceNo, valuePchsStts);
            SetList(listTrnsPurchaseRecord);
            if (listTrnsPurchaseRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void OnSearch()
        {
            if (listTrnsPurchaseItemRecord != null)
            {
                listTrnsPurchaseItemRecord.Clear();
                SetListItems(listTrnsPurchaseItemRecord);
                SetEntityData(new TrnsPurchaseRecord(), true);
            }

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            long InvoiceNo = etInvoice.GetEntryValue();
            string valuePchsStts = etPchsStts.GetSelectedItem().Id;

            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            listTrnsPurchaseRecord = masterTran.getTrnsPurchaseTable(fromStr, toStr, InvoiceNo, valuePchsStts);
            SetList(listTrnsPurchaseRecord);
        }
        void SetList(List<TrnsPurchaseRecord> datas)
        {
            try
            {
                lvTranManagement = new ObservableCollection<TrnsPurchaseRecord>();
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
        async void OnSearchItems(TrnsPurchaseRecord record)
        {
            listTrnsPurchaseItemRecord = masterItems.getTrnsPurchaseItemTable(record.Tin, record.BhfId, record.SpplrTin, record.InvcNo);
            SetListItems(listTrnsPurchaseItemRecord);
            if (listTrnsPurchaseItemRecord == null || listTrnsPurchaseItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetListItems(List<TrnsPurchaseItemRecord> datas)
        {
            try
            {
                lvItemsManagement = new ObservableCollection<TrnsPurchaseItemRecord>();
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

        async void OnFunctionExportVAT(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            string fromStr = etFromDate.GetEntryValue().ToString("yyyyMMdd");
            string toStr = etToDate.GetEntryValue().ToString("yyyyMMdd");
            List<TrnsPurchaseRecordVAT> list = masterTran.getTrnsPurchaseTableVAT(fromStr, toStr);

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
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);
                    //createExcelUtil.CreateSpreadsheetWorkbook(stream, titleList, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    string filename = "PurchaseReport_" + dt + ".xlsx";
                    await iSave.SaveAndView(filename, "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }
        //

        async void OnFunctionExportAll(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (listTrnsPurchaseRecord == null || listTrnsPurchaseRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreatePurchaseExcelUtil createExcelUtil = new CreatePurchaseExcelUtil();

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, listTrnsPurchaseRecord);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TrnsPurchaseItem_" + dt + ".xlsx", "", stream);
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

            if (listTrnsPurchaseItemRecord == null || listTrnsPurchaseItemRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(listTrnsPurchaseItemRecord);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                    await iSave.SaveAndView("TrnsPurchaseItem_" + dt + ".xlsx", "", stream);
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
                if (recordTran.PchsSttsCd.Equals("01"))
                {
                    bool retDelete = EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Are you sure you want to delete this purchase Invoice?", "Yes", "No");
                    if (retDelete)
                    {
                //strQuery = ""
                //strQuery = " UPDATE TRNPURCHASE "
                //strQuery = strQuery + "    SET COMM_F = 'D' "
                //strQuery = strQuery & " WHERE BCNC_ID = '" + strBcncId.Trim() + "' "
                //strQuery = strQuery & "   AND INV_ID = '" + strInvId.Trim() + "' "
                //
                //queries.Add(strQuery)
                //
                //strQuery = ""
                //strQuery = " UPDATE TRNPURCHASEITEM "
                //strQuery = strQuery + "   SET COMM_F = 'D' "
                //strQuery = strQuery & " WHERE BCNC_ID = '" + strBcncId.Trim() + "' "
                //strQuery = strQuery & "   AND INV_ID = '" + strInvId.Trim() + "' "


                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Item deleted succcessfully.", "OK");

                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to delete the item. Please contact [Support].", "OK");
                    }
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This purchase is not in [Waiting] status. Unable to delete!", "OK");
                }
            }
            */

            if (recordTran == null || recordTran.InvcNo <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select an Invoice to delete.", "OK");
                return;
            }

            if (recordTran.PchsSttsCd.Equals("01"))
            {
                string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
                string locationMessage21 = UILocation.Instance().GetLocationText("Are you sure you want to delete this purchase Invoice?");
                var retDelete = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
                if (retDelete)
                {
                    bool ret = false;
                    TrnsPurchaseMaster trnsPurchaseMaster = new TrnsPurchaseMaster();
                    if (trnsPurchaseMaster.DeleteTable(recordTran))
                    {
                        TrnsPurchaseItemMaster trnsPurchaseItemMaster = new TrnsPurchaseItemMaster();
                        if (trnsPurchaseItemMaster.DeleteTable(recordTran))
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
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This purchase is not in [Waiting] status. Unable to delete!", "OK");
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnFunctionNew(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the new purchase?");
            var retNet = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (retNet)
            {
                var popupPage = new PurchaseRegistrationPage();
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
            if(recordTran == null || recordTran.InvcNo <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a purchase transaction.", "OK");
            }
            else
            {
                var popupPage = new DetailInformationOfPurchasePage(recordTran);
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
            if (recordTran == null || recordTran.InvcNo <= 0 || !recordTran.PchsSttsCd.Equals("01"))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a purchase transaction.", "OK");
                return;
            }

            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to modify the current invoice?");
            var retNet = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (retNet)
            {
                var popupPage = new PurchaseModifyPage(recordTran, listTrnsPurchaseItemRecord);
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
                recordTran = (TrnsPurchaseRecord)e.SelectedItem;
                OnSearchItems(recordTran);
                SetEntityData(recordTran, true);
            }
        }

        private void SetEntityData(TrnsPurchaseRecord record, bool readOnly)
        {
            etSelectStatus.SetEntryValue(record.PchsSttsNm);
            etSelectInvoiceID.SetEntryValue(record.InvcNo);
            etSelectSupplier.SetEntryValue(record.TradeNm);

            etSelectVAT.SetEntryValue(record.TotTaxAmt.ToString("#,##0.00"));
            etSelectPurchaseAmount.SetEntryValue(record.TotAmt.ToString("#,##0.00"));
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etPchsStts.SetSelecteItem(new SystemCode());
        }

        private void OnFunctionReceive(object sender, EventArgs e)
        {
            TrnsPurchaseSalesProcess trnsPurchaseSalesProcess = new TrnsPurchaseSalesProcess();
            trnsPurchaseSalesProcess.TrnsPurchaseSalesDownloadProcess();
            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "ReceiveTranPurchase", "This operation was called asynchronously.", "OK");
        }
    }
}

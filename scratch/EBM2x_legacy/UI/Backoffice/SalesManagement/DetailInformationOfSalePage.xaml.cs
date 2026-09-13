using EBM2x.Database.Excel;
using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Dependency;
using EBM2x.Models.journal;
using EBM2x.Modules;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailInformationOfSalePage : ContentPage
    {
        ItemClassMaster ItemClassMaster = new ItemClassMaster();
        StockMasterMaster StockMasterMaster = new StockMasterMaster();
        TransactionStockInOutModel StockInOutModel { get; set; }
        TransactionSalesModel SalesModel { get; set; }

        TrnsSaleItemRecord SelectedItem;

        public DetailInformationOfSalePage(TrnsSaleRecord recordTran)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            Init(recordTran);

            if (UIManager.Instance().IsWindows)
            {
            }
            else
            {
                btnExport.IsVisible = false;
            }
        }
        public void Init(TrnsSaleRecord recordTran)
        {
            StockInOutModel = new TransactionStockInOutModel();
            SalesModel = new TransactionSalesModel();

            TrnsSaleMaster masterTran = new TrnsSaleMaster();
            TrnsSaleItemMaster masterItems = new TrnsSaleItemMaster();
            masterTran.ToRecord(SalesModel.TranRecord, recordTran.Tin, recordTran.BhfId, recordTran.InvcNo);
            // JINIT_20191201, 
            //SalesModel.ItemRecords = masterItems.getTrnsSaleItemTable(recordTran.Tin, recordTran.BhfId, recordTran.InvcNo);
            SalesModel.ItemRecords = masterItems.getTrnsSaleItemTable(recordTran.Tin, recordTran.BhfId, recordTran.InvcNo, recordTran.RcptTyCd);

            UpdateHeaderView();
            UpdateItemView(new TrnsSaleItemRecord());

            etTotalDCAmount.SetReadOnly(true);
            etTotalSupplyAmount.SetReadOnly(true);
            etUnitPrice.SetReadOnly(true);
            etSalesQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);

            StockIoMaster StockIoMaster = new StockIoMaster();
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            long SarNo = StockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "A";
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            StockInOutModel.CurrentItemRecord = null;
            StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderView();

            if (SalesModel.CurrentItemRecord != null)
            {
                UpdateItemView(SalesModel.CurrentItemRecord);
                etUnitPrice.SetReadOnly(true);
                etSalesQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
            else
            {
                UpdateItemView(new TrnsSaleItemRecord());
                etUnitPrice.SetReadOnly(true);
                etSalesQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }

            etValidateDate.SetReadOnly(true);       // RELEASE_DATE
            etCancelReqDate.SetReadOnly(true);      // CANCEL_REQUEST_DATE
            etCanceledDate.SetReadOnly(true);       // CANCELED_DATE
            etRefundDate.SetReadOnly(true);         // REFUND_DATE

            // (11: 01(Wait for Release), 02(Released), 03(Cancel Requested), 04(Canceled), 05(Refunded))
            // Wait for Release
            if (SalesModel.TranRecord.SalesSttsCd.Equals("01"))
            {
                string AuthCd = UIManager.Instance().UserModel.AuthCd;
                if (UIManager.Instance().UserModel.RoleCd != "1")
                {
                    if (!AuthCd.Contains("PROFORMA;"))
                    {
                        btnPrint.IsVisible = false;
                    }
                }
                btnApprove.InvalidateSurfaceSetDisabled(false);
                btnApprove.IsVisible = true;
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnRefund.InvalidateSurfaceSetDisabled(true);
                btnCancelReq.InvalidateSurfaceSetDisabled(true);
            }

            // Released
            if (SalesModel.TranRecord.SalesSttsCd.Equals("02"))
            {
                btnApprove.InvalidateSurfaceSetDisabled(true);
                btnApprove.IsVisible = true;
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnRefund.InvalidateSurfaceSetDisabled(true);
                btnCancelReq.InvalidateSurfaceSetDisabled(false);
            }

            // REFUND
            if (SalesModel.TranRecord.SalesSttsCd.Equals("03"))
            {
                btnApprove.InvalidateSurfaceSetDisabled(true);
                btnApprove.IsVisible = false;
                btnCancel.InvalidateSurfaceSetDisabled(true);
                string AuthCd = UIManager.Instance().UserModel.AuthCd;
                if (UIManager.Instance().UserModel.RoleCd != "1")
                {
                    if (!AuthCd.Contains("REFUND;"))
                    {
                        btnRefund.InvalidateSurfaceSetDisabled(true);
                       
                    }
                    else
                    {
                        btnRefund.InvalidateSurfaceSetDisabled(false);
                    }
                }
                btnCancelReq.InvalidateSurfaceSetDisabled(true);
            }

            // CANCELED
            if (SalesModel.TranRecord.SalesSttsCd.Equals("04"))
            {
                btnApprove.InvalidateSurfaceSetDisabled(true);
                btnApprove.IsVisible = true;
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnRefund.InvalidateSurfaceSetDisabled(true);
                btnCancelReq.InvalidateSurfaceSetDisabled(true);
                etUnitPrice.SetReadOnly(false);
                etSalesQty.SetReadOnly(false);
            }

            // REFUNDED
            if (SalesModel.TranRecord.SalesSttsCd.Equals("05"))
            {
                btnApprove.InvalidateSurfaceSetDisabled(true);
                btnApprove.IsVisible = true;
                btnCancel.InvalidateSurfaceSetDisabled(true);
                btnRefund.InvalidateSurfaceSetDisabled(true);
                btnCancelReq.InvalidateSurfaceSetDisabled(true);
            }

            OnSearchItems(SalesModel.TranRecord);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        public void UpdateHeaderView()
        {
            etCurrentStatus.SetReadOnly(true);
            etInvoieID.SetReadOnly(true);
            etCustomerID.SetReadOnly(true);
            etCustomerName.SetReadOnly(true);
            etSaleDate.SetReadOnly(true);
            //etReleaseDate.SetReadOnly(true);
            etTotalAmount.SetReadOnly(true);
            etTotalVAT.SetReadOnly(true);

            etTotalDCAmount.SetReadOnly(true);
            etTotalSupplyAmount.SetReadOnly(true);

            etValidateDate.SetReadOnly(true);
            etCancelReqDate.SetReadOnly(true);
            etCanceledDate.SetReadOnly(true);
            etRefundDate.SetReadOnly(true);

            etRemark.SetReadOnly(true);

            etCurrentStatus.SetEntryValue(SalesModel.TranRecord.SalesSttsNm);
            etInvoieID.SetEntryValue(SalesModel.TranRecord.InvcNo.ToString("#,##0"));
            etCustomerID.SetEntryValue(SalesModel.TranRecord.CustTin);
            etCustomerName.SetEntryValue(SalesModel.TranRecord.CustNm);
            etSaleDate.SetEntryValue(SalesModel.TranRecord.SalesDt);
            etTotalAmount.SetEntryValue(SalesModel.TranRecord.TotAmt.ToString("#,##0.00"));
            etTotalVAT.SetEntryValue(SalesModel.TranRecord.TotTaxAmt.ToString("#,##0.00"));

            etTotalDCAmount.SetEntryValue(SalesModel.GetTotDCAmount().ToString("#,##0.00"));
            etTotalSupplyAmount.SetEntryValue(SalesModel.TranRecord.TotAmt.ToString("#,##0.00"));

            etValidateDate.SetEntryValue(SalesModel.TranRecord.CfmDt);
            etCancelReqDate.SetEntryValue(SalesModel.TranRecord.CnclReqDt);
            etCanceledDate.SetEntryValue(SalesModel.TranRecord.CnclDt);
            etRefundDate.SetEntryValue(SalesModel.TranRecord.RfdDt);

            etRemark.SetEntryValue(SalesModel.TranRecord.Remark);
        }
        public void UpdateItemView(TrnsSaleItemRecord itemRecord)
        {
            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            etUnitPrice.SetReadOnly(false);
            etSalesQty.SetReadOnly(false);
            etTaxType.SetReadOnly(true);
            etVat.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
            etDCAmount.SetReadOnly(true);
            etSalesPrice.SetReadOnly(true);
            etTotalPrice.SetReadOnly(true);
            etCurrentStock.SetReadOnly(true);

            etItemCode.SetEntryValue(itemRecord.ItemCd);
            etItemName.SetEntryValue(itemRecord.ItemNm);
            etClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etUnitPrice.SetEntryValue(itemRecord.Prc.ToString("#,##0.00"));
            etSalesQty.SetEntryValue(itemRecord.Qty.ToString("#,##0"));
            etTaxType.SetEntryValue(itemRecord.TaxTyNm);
            etVat.SetEntryValue(itemRecord.TaxAmt.ToString("#,##0.00"));
            etDCRate.SetEntryValue(itemRecord.DcRt.ToString("#,##0"));
            etDCAmount.SetEntryValue(itemRecord.DcAmt.ToString("#,##0.00"));
            etSalesPrice.SetEntryValue(itemRecord.SplyAmt.ToString("#,##0.00"));
            etTotalPrice.SetEntryValue(itemRecord.TotAmt.ToString("#,##0.00"));

            if (itemRecord != null && !string.IsNullOrEmpty(itemRecord.ItemCd))
            {
                double RdsQty = StockMasterMaster.GetCurrentStock(itemRecord.Tin, itemRecord.ItemCd); // 현재고 수량
                etCurrentStock.SetEntryValue(RdsQty.ToString("#,##0.00"));
                string className = ItemClassMaster.GetItemClassName(itemRecord.ItemClsCd);
                etClassName.SetEntryValue(className);
            }
            else
            {
                etCurrentStock.SetEntryValue("");
                etClassName.SetEntryValue("");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton , true: BackButton
        }

        async void OnFunctionPrintReceipt(object sender, EventArgs e)
        {
            if (SalesModel.TranRecord.SalesSttsCd.Equals("02") || SalesModel.TranRecord.SalesSttsCd.Equals("05"))
            {
                // PRINTING
                var popupPage = new PrintReceiptPage(SalesModel, false);
                popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
                popupPage.OnResult += (popup, ex) => {
                    Device.BeginInvokeOnMainThread(() => {
                        string paymentName = (string)((ExtEventArgs)ex).EnteredText;
                        Navigation.PopAsync();
                        if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintReceipt"))
                        {
                            TrnsSaleReceiptRecord trnsSaleReceiptRecord = new TrnsSaleReceiptRecord();
                            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
                            trnsSaleReceiptMaster.ToRecord(trnsSaleReceiptRecord, SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo.ToString());

                            JournalModel journal = GetEJournal(SalesModel, trnsSaleReceiptRecord);
                            PrintingService printingService = new PrintingService();
                            printingService.writeJurnal(journal, true);
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Print the document.", "OK");
                        }
                        else if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintA4"))
                        {
                            TrnsSaleReceiptRecord trnsSaleReceiptRecord = new TrnsSaleReceiptRecord();
                            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
                            trnsSaleReceiptMaster.ToRecord(trnsSaleReceiptRecord, SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo.ToString());

                            JournalModel journal1 = GetEJournalInvoice1(SalesModel, trnsSaleReceiptRecord, true);
                            JournalModel journal2 = GetEJournalInvoice2(SalesModel, trnsSaleReceiptRecord, true);
                            JournalModel journal3 = GetEJournalInvoice3(SalesModel, trnsSaleReceiptRecord, true);
                            JournalModel journal4 = GetEJournalInvoice4(SalesModel, trnsSaleReceiptRecord, true);  // 재발행
                            JournalModel journal5 = GetEJournalInvoice5(SalesModel, trnsSaleReceiptRecord);

                            //clinton
                            Console.WriteLine(SalesModel);

                            PrintingService printingService = new PrintingService();
                            printingService.writeInvoiceA4(SalesModel, journal1, journal2, journal3, journal4, true, journal5);
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Print the document. PDF", "OK");
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
            else if (SalesModel.TranRecord.SalesSttsCd.Equals("01"))
            {
                // PRINTING
                var popupPage = new PrintReceiptPage(SalesModel, true);
                popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
                popupPage.OnResult += (popup, ex) => {
                    Device.BeginInvokeOnMainThread(() => {
                        string paymentName = (string)((ExtEventArgs)ex).EnteredText;
                        Navigation.PopAsync();

                        TrnsSaleReceiptRecord trnsSaleReceiptRecord = new TrnsSaleReceiptRecord();
                        TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
                        trnsSaleReceiptMaster.ToRecord(trnsSaleReceiptRecord, SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo.ToString());
                        JournalModel journal = GetEJournalProforma(SalesModel, trnsSaleReceiptRecord);
                        PrintingService printingService = new PrintingService();
                        if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintReceipt"))
                        {
                            printingService.writeJurnal(journal, true);
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Print the document.", "OK");
                        }
                        else if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintA4"))
                        {
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
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This invoice cannot be printed.", "OK");
            }
        }

        async void OnFunctionExport(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to export?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (SalesModel.ItemRecords == null || SalesModel.ItemRecords.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is no search result, so it cannot be exported. Please try at least one.", "OK");
            }
            else
            {
                CreateExcelUtil createExcelUtil = new CreateExcelUtil();
                string jsonRequest = JsonConvert.SerializeObject(SalesModel.ItemRecords);

                ISave iSave = DependencyService.Get<ISave>();
                if (iSave != null)
                {
                    MemoryStream stream = new MemoryStream();
                    createExcelUtil.CreateSpreadsheetWorkbook(stream, jsonRequest);

                    string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    await iSave.SaveAndView("TrnsSaleItem.xlsx", "", stream);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Excel file.", "OK");
                }
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnSearchItems(TrnsSaleRecord record)
        {
            TrnsSaleItemMaster masterItems = new TrnsSaleItemMaster();
            // JINIT_20191201, 
            //SalesModel.ItemRecords = masterItems.getTrnsSaleItemTable(record.Tin, record.BhfId, record.InvcNo);
            SalesModel.ItemRecords = masterItems.getTrnsSaleItemTable(record.Tin, record.BhfId, record.InvcNo, record.RcptTyCd);

            listView.ItemsSource = SalesModel.GetObservableCollection();
            if (SalesModel.ItemRecords.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        private void OnSelectedTran(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (TrnsSaleItemRecord)e.SelectedItem;
                UpdateItemView(SelectedItem);
            }
        }

        async void OnFunctionApprove(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the invoice" + "Approved" + "?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            if (!CheckStock(SalesModel.ItemRecords))
            {
                // Please check the Current Quantity stock.
                return;
            }

            var popupPage = new InputPhoneCheckInformationPage(SalesModel);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string paymentCode = (string)((ExtEventArgs)ex).EnteredObject;
                    Navigation.PopAsync();

                    ApproveProcess(paymentCode);
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }
        async void ApproveProcess(string paymentCode)
        {
            /*
            tranTyCD = transactionTyCd
            rcptTyCD = "S"
            sign = ""
            invTrans_PayType = tranTyCD & rcptTyCD */

            SalesModel.TranRecord.SalesSttsCd = "02";
            SalesModel.TranRecord.CfmDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.PmtTyCd = paymentCode;

            SalesModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            SalesModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            TrnsSaleItemMaster trnsSaleItemMaster = new TrnsSaleItemMaster();
            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
            StockIoMaster stockIoMaster = new StockIoMaster();
            StockIoItemMaster stockIoItemMaster = new StockIoItemMaster();

            trnsSaleMaster.UpdateTable("02", paymentCode, SalesModel.TranRecord);

            //DB INSERT
            TrnsSaleReceiptRecord trnsSaleReceiptRecord = new TrnsSaleReceiptRecord();
            trnsSaleReceiptRecord.SetTrnsSaleRecord(SalesModel.TranRecord);
            trnsSaleReceiptRecord.TaxprNm = SalesModel.TranRecord.TaxprNm;
            trnsSaleReceiptRecord.RcptPbctDt = SalesModel.TranRecord.CfmDt; // DateTime.Now.ToString("yyyyMMdd");
            trnsSaleReceiptRecord.CurRcptNo = trnsSaleReceiptMaster.GetReceiptSeq();
            trnsSaleReceiptRecord.TotRcptNo = trnsSaleReceiptMaster.GetTotReceiptSeq();

            string GblKeySign = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeySign;
            string GblKeyInternal = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeyInternal;
            ReceiptSignature receiptSignature = new ReceiptSignature();
            receiptSignature.RcptDt = SalesModel.TranRecord.CfmDt;
            receiptSignature.Tin = SalesModel.TranRecord.Tin;
            receiptSignature.CustTin = SalesModel.TranRecord.CustTin;
            receiptSignature.GblMrcSysCod = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblMrcSysCod;
            receiptSignature.DblSpcpcA = SalesModel.TranRecord.TaxblAmtA;
            receiptSignature.DblVatA = SalesModel.TranRecord.TaxAmtA;
            receiptSignature.DblSpcpcB = SalesModel.TranRecord.TaxblAmtB;
            receiptSignature.DblSpcpcE = SalesModel.TranRecord.TaxblAmtE;
            receiptSignature.DblVatB = SalesModel.TranRecord.TaxAmtB;
            receiptSignature.DblVatE = SalesModel.TranRecord.TaxAmtE;
            receiptSignature.DblSpcpcC = SalesModel.TranRecord.TaxblAmtC;
            receiptSignature.DblVatC = SalesModel.TranRecord.TaxAmtC;
            receiptSignature.GblSdcSysNum = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblSdcSysNum;
            if (!string.IsNullOrEmpty(SalesModel.TranRecord.RcptTyCd) && SalesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                receiptSignature.TranRcptTyCD = "NR";
            }
            else
            {
                receiptSignature.TranRcptTyCD = "NS";
            }
            receiptSignature.CurReceiptID = trnsSaleReceiptRecord.CurRcptNo;
            receiptSignature.TotReceiptID = trnsSaleReceiptRecord.TotRcptNo;
            //receiptSignature.CurReceiptID = trnsSaleReceiptMaster.GetReceiptSeq();
            //receiptSignature.TotReceiptID = trnsSaleReceiptMaster.GetTotReceiptSeq();
            trnsSaleReceiptRecord.RcptSign = (string)Base32.Receipt_signature(trnsSaleReceiptMaster.GetReceiptSignature(receiptSignature), GblKeySign);

            SalesReportMaster salesReportMaster = new SalesReportMaster();
            long normalSales = (long)salesReportMaster.GetCodeValue("20150101", DateTime.Now.ToString("yyyyMMdd"), "N", "S", "TRNS_SALE.TOT_AMT");
            long returnSales = (long)salesReportMaster.GetCodeValue("20150101", DateTime.Now.ToString("yyyyMMdd"), "N", "R", "TRNS_SALE.TOT_AMT");
            string strOcdt = SalesModel.TranRecord.SalesDt;
            long receipt = trnsSaleReceiptRecord.CurRcptNo;
            trnsSaleReceiptRecord.IntrlData = (string)Base32.Internal_data(trnsSaleReceiptMaster.GetInternalData(normalSales, returnSales, strOcdt, receipt), GblKeySign);

            JournalModel journal = GetEJournal(SalesModel, trnsSaleReceiptRecord);

            trnsSaleReceiptRecord.Jrnl = journal.SetJournal();
            // 20200309 JCNA
            //trnsSaleReceiptRecord.Jrnl = JsonConvert.SerializeObject(journal, Formatting.Indented); 
            // 20200720 JCNA
            //DateTime date = DateTime.Now;
            //trnsSaleReceiptRecord.RptNo = date.Year + (date.Month * 100) + date.Day; // 년도 + (월 * 100) + 일
            trnsSaleReceiptRecord.RptNo = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount();

            trnsSaleReceiptRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            trnsSaleReceiptRecord.ModrId = UIManager.Instance().UserModel.UserId;
            trnsSaleReceiptRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            trnsSaleReceiptRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            trnsSaleReceiptRecord.RegrId = UIManager.Instance().UserModel.UserId;
            trnsSaleReceiptRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            trnsSaleReceiptMaster.InsertTable(trnsSaleReceiptRecord);

            //StockInOutModel.TranRecord
            StockInOutModel.TranRecord.SarTyCd = "11";        
            StockInOutModel.TranRecord.TaxprNm = SalesModel.TranRecord.TaxprNm;
            StockInOutModel.TranRecord.CustTin = SalesModel.TranRecord.CustTin;
            StockInOutModel.TranRecord.CustNm = SalesModel.TranRecord.CustNm;
            StockInOutModel.TranRecord.CustBhfId = SalesModel.TranRecord.CustBhfId;

            StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            for (int i = 0; i < SalesModel.ItemRecords.Count; i++)
            {
                //StockInOutModel.ItemRecords
                TrnsSaleItemRecord itemNode = SalesModel.ItemRecords[i];

                TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                bool ret = taxpayerItemMaster.ToRecord(itemRecord, StockInOutModel.TranRecord.Tin, itemNode.ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);

                StockInOutModel.SetCurrentItem(itemRecord);

                StockInOutModel.CurrentItemRecord.Prc = itemNode.Prc;
                StockInOutModel.CurrentItemRecord.Qty = itemNode.Qty;
                StockInOutModel.CurrentItemRecord.TotDcAmt = itemNode.DcAmt;

                StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                StockInOutModel.CalculateCurrentItem();
                StockInOutModel.ConfirmCurrentItem();
            }

            // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
            StockInOutModel.TranRecord.SarNo = stockIoMaster.GetStockIoSeq();
            stockIoMaster.InsertTable(StockInOutModel.TranRecord);
            stockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            trnsSaleRcptRraSdcUpload.SendTranSalesRcptSave(trnsSaleReceiptRecord.Tin, trnsSaleReceiptRecord.BhfId, trnsSaleReceiptRecord.InvcNo);

            //===>>>>>>>>>
            //JCNA 20191204
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            // PRINTING
            var popupPage = new PrintReceiptPage(SalesModel, false);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string paymentName = (string)((ExtEventArgs)ex).EnteredText;
                    Navigation.PopAsync();
                    if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintReceipt"))
                    {
                        PrintingService printingService = new PrintingService();
                        printingService.writeJurnal(journal, false);
                    }
                    else if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintA4"))
                    {
                        JournalModel journal1 = GetEJournalInvoice1(SalesModel, trnsSaleReceiptRecord, false);
                        JournalModel journal2 = GetEJournalInvoice2(SalesModel, trnsSaleReceiptRecord, false);
                        JournalModel journal3 = GetEJournalInvoice3(SalesModel, trnsSaleReceiptRecord, false);
                        // JournalModel journal4 = GetEJournalInvoice4(SalesModel, trnsSaleReceiptRecord, true); // Commented by Brighr
                        JournalModel journal4 = GetEJournalInvoice4(SalesModel, trnsSaleReceiptRecord, false); // Added by Brighr on 01.02.2021
                        JournalModel journal5 = GetEJournal(SalesModel, trnsSaleReceiptRecord);
                        PrintingService printingService = new PrintingService();
                        printingService.writeInvoiceA4(SalesModel, journal1, journal2, journal3, journal4, false, journal5);
                    }

                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
                    if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }

        public bool CheckStock(List<TrnsSaleItemRecord> ItemRecords)
        {
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            TaxpayerItemRecord recordItem = new TaxpayerItemRecord();

            for (int i = 0; i < ItemRecords.Count; i++)
            {
                TrnsSaleItemRecord record = ItemRecords[i];
                double count = 0;

                for (int j = 0; j < ItemRecords.Count; j++)
                {
                    TrnsSaleItemRecord record2 = ItemRecords[j];
                    if (record.ItemCd.Equals(record2.ItemCd))
                    {
                        count += record.Qty;
                    }
                }

                taxpayerItemMaster.ToRecord(recordItem, record.Tin, record.ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                if (recordItem.RdsQty == 0 && !recordItem.ItemTyCd.Equals("3"))
                {
                    // Please check the Current Quantity stock.
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please check the Current Quantity stock.", "OK");
                    return false;
                }
                if (recordItem.RdsQty < count && !recordItem.ItemTyCd.Equals("3"))
                {
                    // Sales Qty is greater than current stock qty. Please Check.
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Sales Qty is greater than current stock qty. Please Check.", "OK");
                    return false;
                }

                if (recordItem.UseExpiration.Equals("Y") && !string.IsNullOrEmpty(recordItem.ExpirationDate))
                {
                    DateTime ExpirationDate = Common.DateFormat(recordItem.ExpirationDate);
                    TimeSpan TS = ExpirationDate - DateTime.Now;

                    int diffDay = TS.Days;  //날짜의 차이 구하기
                    if (diffDay < 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There is an expired product. Please Check.", "OK");
                        return false;
                    }
                }

                /*
               '수량 SAFETY_STOCK CHECK
               If dgvDetail.Rows(x).Cells("SAFETY_STOCK").Value = 0 And dgvDetail.Rows(x).Cells("ITM_TY_CD").Value = 3 Then
               Else
                If (CDbl(dgvDetail.Rows(x).Cells("CURR_QTY").Value) - CDbl(dgvDetail.Rows(x).Cells("QTY").Value)) < dgvDetail.Rows(x).Cells("SAFETY_STOCK").Value Then
                    MessageBox.Show("Your stock(" & dgvDetail.Rows(x).Cells("CURR_QTY").Value & ") is going below the safety stock (" & dgvDetail.Rows(x).Cells("SAFETY_STOCK").Value & "). ")

                End If
            End If
            */
            }

            return true;
        }

        async void OnFunctionApproveSet(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the invoice" + "Rollback" + "?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            // 권한 
            // If (GblUsrRole = "1" Or GblUsrAuth.Contains("REFUND")) Then

            SalesModel.TranRecord.SalesSttsCd = "02";
            SalesModel.TranRecord.CnclReqDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            SalesModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            SalesModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            trnsSaleMaster.UpdateTable("02", null, SalesModel.TranRecord);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        async void OnFunctionCancelRequest(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the invoice" + "Cancel Requested" + "?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            // 권한 
            // If (GblUsrRole = "1" Or GblUsrAuth.Contains("REFUND")) Then

            SalesModel.TranRecord.SalesSttsCd = "03";
            SalesModel.TranRecord.CnclReqDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            SalesModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            SalesModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            trnsSaleMaster.UpdateTable("03", null, SalesModel.TranRecord);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        async void OnFunctionCancel(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the invoice" + "Canceled" + "?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            SalesModel.TranRecord.SalesSttsCd = "04";
            SalesModel.TranRecord.CnclDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            SalesModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            SalesModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            trnsSaleMaster.UpdateTable("04", null, SalesModel.TranRecord);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        async void OnFunctionRefund(object sender, EventArgs e)
        {
            
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to process the invoice" + "Refunded" + "?");
            var retConfirm = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!retConfirm) return;

            var popupPage = new InputPhoneCheckInformationPage(SalesModel);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    string paymentCode = (string)((ExtEventArgs)ex).EnteredObject;
                    Navigation.PopAsync();

                    // RefundReason(paymentCode);
                    RefundProcess(paymentCode, "03", "Damaged");

                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }
        async void RefundReason(string paymentCode)
        {
            var popupPage = new RefundReasonPopupPage(SalesModel);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {


                    SalesModel = (TransactionSalesModel)((ExtEventArgs)ex).EnteredObject;
                    Navigation.PopAsync();

                    // CodeDtlRecord codeDtlRecordRefundReason = (CodeDtlRecord)((ExtEventArgs)ex).EnteredObject;
                    //Navigation.PopAsync();

                    //RefundProcess(paymentCode, codeDtlRecordRefundReason);
                    Console.WriteLine("SalesModel OnRefundReason :: " + JsonConvert.SerializeObject(SalesModel));
                   // SalesModelRefund.TranRecord.RefundReason = SalesModel.TranRecord.RefundReason;
                   // SalesModelRefund.TranRecord.RefundReasonText = SalesModel.TranRecord.RefundReasonText;
                   // Console.WriteLine("SalesModelRefund OnRefundReason :: " + JsonConvert.SerializeObject(SalesModelRefund));
                    RefundProcess(SalesModel.TranRecord.PmtTyCd, SalesModel.TranRecord.RefundReason, SalesModel.TranRecord.RefundReasonText.ToString());
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }


        // remove items

        private void OnRemove(object sender, System.EventArgs e)
        {
            if (SelectedItem != null)
            {
                SalesModel.Delete(SelectedItem);
                SalesModel.CalculateTran();
                listView.ItemsSource = SalesModel.GetObservableCollection();

                UpdateHeaderView();
                UpdateItemView(new TrnsSaleItemRecord());
                etUnitPrice.SetReadOnly(true);
                etSalesQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
        }

        //saving items;

        void OnFunctionConfirm(object sender, EventArgs e)
        {
            bool flag = true;

            if (SelectedItem == null)
            {
                flag = false;
                return;
            }
            String strUnitPrice = etUnitPrice.GetEntryValue().ToString();
            Console.WriteLine("Unit Price changed to: " + strUnitPrice);
            String strSalesQty = etSalesQty.GetEntryValue().ToString();
            Console.WriteLine("Quantity changed to: " + strUnitPrice);
            if (string.IsNullOrEmpty(strUnitPrice) && Convert.ToDouble(strUnitPrice) <= 0.0)
            {
                flag = false;
                return;
            }
            if (string.IsNullOrEmpty(strSalesQty) && Convert.ToDouble(strSalesQty) <= 0.0)
            {
                flag = false;
                return;
            }

            double dblUnitPrice = Convert.ToDouble(strUnitPrice);
            if (dblUnitPrice > SelectedItem.Prc)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "You cannot increase Unit Price on a Credit Note.", "OK");
                flag = false;
                return;
            }
            double dblSalesQty = Convert.ToDouble(strSalesQty);
            if (dblSalesQty > SelectedItem.Qty)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "You cannot increase Quantity on a Credit Note.", "OK");
                flag = false;
                return;
            }

            if (dblUnitPrice > 0 && dblSalesQty > 0 && flag)
            {
                TrnsSaleItemRecord itemRecord = new TrnsSaleItemRecord();

                itemRecord = SelectedItem;

                itemRecord.Prc = dblUnitPrice;
                itemRecord.Qty = dblSalesQty;
                itemRecord.SplyAmt = Math.Round(itemRecord.Prc * itemRecord.Qty, 2);
                itemRecord.DcAmt = (itemRecord.SplyAmt * itemRecord.DcRt) / 100;

                itemRecord.TaxblAmt = itemRecord.SplyAmt - itemRecord.DcAmt;
                if (itemRecord.TaxTyCd.Equals("B"))
                {
                    double vatRate = 16;
                    itemRecord.TaxAmt = (itemRecord.TaxblAmt / 1.16) * (vatRate / 100);
                    itemRecord.TaxAmt = Math.Round(itemRecord.TaxAmt, 2);
                }
                else if (itemRecord.TaxTyCd.Equals("E"))
                {
                    double vatRate = 8;
                    itemRecord.TaxAmt = (itemRecord.TaxblAmt / 1.08) * (vatRate / 100);
                    itemRecord.TaxAmt = Math.Round(itemRecord.TaxAmt, 2);
                }
                else
                {
                    itemRecord.TaxAmt = 0;
                }
                itemRecord.TotAmt = itemRecord.TaxblAmt;

                if (Object.ReferenceEquals(itemRecord, null))
                {
                    return;
                }
                UpdateItemView(itemRecord);

                if (SalesModel.ItemRecords.Count > 0)
                {
                    for (int index = 0; index < SalesModel.ItemRecords.Count; index++)
                    {
                        if (SalesModel.ItemRecords[index].ItemCd.Equals(itemRecord.ItemCd))
                        {
                            SalesModel.ItemRecords[index] = itemRecord;
                        }
                        listView.ItemsSource = SalesModel.GetObservableCollection();
                    }
                }

            }

            SalesModel.CalculateTran();
            UpdateHeaderView();
            UpdateItemView(new TrnsSaleItemRecord());
        }
        // end

        async void RefundProcess(string paymentCode, string refundReason, string RefundReasonText)
        {
            /*
            strInvStctycd = "32"                       '재고유형코드(12: 32(매출환불) ==> 매출환불전표 구분)
            strStctycd = "02"                          '재고유형코드(12: 32(매출환불) ==> 02(반품입고))

            tranTyCD = transactionTyCd
            rcptTyCD = "R"
            sign = "-"

            invTrans_PayType = tranTyCD & rcptTyCD                     
             */
           // Console.WriteLine("SalesModelRefund OnRefundReason 1 :: " + JsonConvert.SerializeObject(SalesModelRefund));
            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            TrnsSaleItemMaster trnsSaleItemMaster = new TrnsSaleItemMaster();
            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
            StockIoMaster stockIoMaster = new StockIoMaster();
            StockIoItemMaster stockIoItemMaster = new StockIoItemMaster();

            SalesModel.TranRecord.SalesSttsCd = "05";
            SalesModel.TranRecord.RfdDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.PmtTyCd = paymentCode;

            SalesModel.TranRecord.RefundReason = refundReason;
            SalesModel.TranRecord.RefundReasonText = RefundReasonText;

            Console.WriteLine("SalesModel OnRefund :: " + JsonConvert.SerializeObject(SalesModel));

            trnsSaleMaster.UpdateTable("05", paymentCode, SalesModel.TranRecord);

            // 전표번호 생성 ==> INV_ID (15) : 재고(전표)ID ==> 지점(2)+재고유형(2)+DDMMYYYY(8)+SEQ(3)

            //반품이면 새로운 INV_ID를 채번한다.
            long orgInvcNo = SalesModel.TranRecord.InvcNo;

            // 반품 거래 Header, Items
            SalesModel.TranRecord.OrgInvcNo = orgInvcNo;
            SalesModel.TranRecord.InvcNo = trnsSaleMaster.GetSalesSeq();
            SalesModel.TranRecord.SalesSttsCd = "05";
            SalesModel.TranRecord.SalesTyCd = "N";
            SalesModel.TranRecord.RcptTyCd = "R";
            SalesModel.TranRecord.RfdDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.SalesDt = DateTime.Now.ToString("yyyyMMdd");
            SalesModel.TranRecord.CfmDt = DateTime.Now.ToString("yyyyMMddHHmmss");

            SalesModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            SalesModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            trnsSaleMaster.InsertTable(SalesModel.TranRecord);
            // JINIT_20191208, 인보이스번호를 확인하기 위해서 SalesModel.TranRecord 를 넘김
            //trnsSaleItemMaster.InsertTable(SalesModel.ItemRecords);
            trnsSaleItemMaster.InsertTable(SalesModel.TranRecord, SalesModel.ItemRecords);

            // 영수증 DB INSERT
            TrnsSaleReceiptRecord trnsSaleReceiptRecord = new TrnsSaleReceiptRecord();
            trnsSaleReceiptRecord.SetTrnsSaleRecord(SalesModel.TranRecord);
            trnsSaleReceiptRecord.OrgInvcNo = orgInvcNo;

            string GblKeySign = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeySign;
            string GblKeyInternal = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeyInternal;
            ReceiptSignature receiptSignature = new ReceiptSignature();
            receiptSignature.RcptDt = SalesModel.TranRecord.RfdDt;
            receiptSignature.Tin = SalesModel.TranRecord.Tin;
            receiptSignature.CustTin = SalesModel.TranRecord.CustTin;
            receiptSignature.GblMrcSysCod = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblMrcSysCod;
            receiptSignature.DblSpcpcA = SalesModel.TranRecord.TaxblAmtA;
            receiptSignature.DblVatA = SalesModel.TranRecord.TaxAmtA;
            receiptSignature.DblSpcpcB = SalesModel.TranRecord.TaxblAmtB;
            receiptSignature.DblVatB = SalesModel.TranRecord.TaxAmtB;
            receiptSignature.DblSpcpcE = SalesModel.TranRecord.TaxblAmtE;
            receiptSignature.DblVatE = SalesModel.TranRecord.TaxAmtE;
            receiptSignature.DblSpcpcC = SalesModel.TranRecord.TaxblAmtC;
            receiptSignature.DblVatC = SalesModel.TranRecord.TaxAmtC;
            receiptSignature.GblSdcSysNum = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblSdcSysNum;
            if (!string.IsNullOrEmpty(SalesModel.TranRecord.RcptTyCd) && SalesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                receiptSignature.TranRcptTyCD = "NR";
            }
            else
            {
                receiptSignature.TranRcptTyCD = "NS";
            }
            receiptSignature.CurReceiptID = trnsSaleReceiptRecord.CurRcptNo;
            receiptSignature.TotReceiptID = trnsSaleReceiptRecord.TotRcptNo;
            //receiptSignature.CurReceiptID = trnsSaleReceiptMaster.GetReceiptSeq();
            //receiptSignature.TotReceiptID = trnsSaleReceiptMaster.GetTotReceiptSeq();
            trnsSaleReceiptRecord.RcptSign = (string)Base32.Receipt_signature(trnsSaleReceiptMaster.GetReceiptSignature(receiptSignature), GblKeySign);

            SalesReportMaster salesReportMaster = new SalesReportMaster();
            long normalSales = (long)salesReportMaster.GetCodeValue("20150101", DateTime.Now.ToString("yyyyMMdd"), "N", "S", "TRNS_SALE.TOT_AMT");
            long returnSales = (long)salesReportMaster.GetCodeValue("20150101", DateTime.Now.ToString("yyyyMMdd"), "N", "R", "TRNS_SALE.TOT_AMT");
            string strOcdt = SalesModel.TranRecord.SalesDt;
            long receipt = trnsSaleReceiptRecord.CurRcptNo;
            trnsSaleReceiptRecord.IntrlData = (string)Base32.Internal_data(trnsSaleReceiptMaster.GetInternalData(normalSales, returnSales, strOcdt, receipt), GblKeySign);

            trnsSaleReceiptRecord.TaxprNm = SalesModel.TranRecord.TaxprNm;
            trnsSaleReceiptRecord.RcptPbctDt = SalesModel.TranRecord.CfmDt; // DateTime.Now.ToString("yyyyMMdd");
            trnsSaleReceiptRecord.CurRcptNo = trnsSaleReceiptMaster.GetReceiptSeq();
            trnsSaleReceiptRecord.TotRcptNo = trnsSaleReceiptMaster.GetTotReceiptSeq();

            JournalModel journal = GetEJournal(SalesModel, trnsSaleReceiptRecord);

            trnsSaleReceiptRecord.Jrnl = journal.SetJournal();
            // 20200309 JCNA
            //trnsSaleReceiptRecord.Jrnl = JsonConvert.SerializeObject(journal, Formatting.Indented); 
            // 20200720 JCNA
            //DateTime date = DateTime.Now;
            //trnsSaleReceiptRecord.RptNo = date.Year + (date.Month * 100) + date.Day; // 년도 + (월 * 100) + 일
            trnsSaleReceiptRecord.RptNo = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount();

            trnsSaleReceiptRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            trnsSaleReceiptRecord.ModrId = UIManager.Instance().UserModel.UserId;
            trnsSaleReceiptRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            trnsSaleReceiptRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            trnsSaleReceiptRecord.RegrId = UIManager.Instance().UserModel.UserId;
            trnsSaleReceiptRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            trnsSaleReceiptMaster.InsertTable(trnsSaleReceiptRecord);
            //StockInOutModel.TranRecord
            StockInOutModel.TranRecord.SarTyCd = "02";
            StockInOutModel.TranRecord.TaxprNm = SalesModel.TranRecord.TaxprNm;
            StockInOutModel.TranRecord.CustTin = SalesModel.TranRecord.CustTin;
            StockInOutModel.TranRecord.CustNm = SalesModel.TranRecord.CustNm;
            StockInOutModel.TranRecord.CustBhfId = SalesModel.TranRecord.CustBhfId;

            StockInOutModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            StockInOutModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            StockInOutModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            StockInOutModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            for (int i = 0; i < SalesModel.ItemRecords.Count; i++)
            {
                //StockInOutModel.ItemRecords
                TrnsSaleItemRecord itemNode = SalesModel.ItemRecords[i];

                TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                bool ret = taxpayerItemMaster.ToRecord(itemRecord, StockInOutModel.TranRecord.Tin, itemNode.ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);

                StockInOutModel.SetCurrentItem(itemRecord);

                StockInOutModel.CurrentItemRecord.Prc = itemNode.Prc;
                StockInOutModel.CurrentItemRecord.Qty = itemNode.Qty;
                StockInOutModel.CurrentItemRecord.TotDcAmt = itemNode.DcAmt;

                StockInOutModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                StockInOutModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                StockInOutModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                StockInOutModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                StockInOutModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                StockInOutModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                StockInOutModel.CalculateCurrentItem();
                StockInOutModel.ConfirmCurrentItem();
            }
            // STOCK_IO_ITEM
            StockInOutModel.TranRecord.SarNo = stockIoMaster.GetStockIoSeq();
            stockIoMaster.InsertTable(StockInOutModel.TranRecord);
            // stockIoItemMaster
            stockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            trnsSaleRcptRraSdcUpload.SendTranSalesRcptSave(trnsSaleReceiptRecord.Tin, trnsSaleReceiptRecord.BhfId, trnsSaleReceiptRecord.InvcNo);

            //===>>>>>>>>>
            //JCNA 20191204
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            // PRINTING
            var popupPage = new PrintReceiptPage(SalesModel, false);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    string paymentName = (string)((ExtEventArgs)ex).EnteredText;
                    Navigation.PopAsync();
                    if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintReceipt"))
                    {
                        PrintingService printingService = new PrintingService();
                        printingService.writeJurnal(journal, false);
                    }
                    else if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintA4"))
                    {
                        JournalModel journal1 = GetEJournalInvoice1(SalesModel, trnsSaleReceiptRecord, false);
                        JournalModel journal2 = GetEJournalInvoice2(SalesModel, trnsSaleReceiptRecord, false);
                        JournalModel journal3 = GetEJournalInvoice3(SalesModel, trnsSaleReceiptRecord, false);
                        //JournalModel journal4 = GetEJournalInvoice4(SalesModel, trnsSaleReceiptRecord, true); // Commented by Bright
                        JournalModel journal4 = GetEJournalInvoice4(SalesModel, trnsSaleReceiptRecord, false); // Added by Bright on 01.02.2021
                        JournalModel journal5 = GetEJournal(SalesModel, trnsSaleReceiptRecord);
                        PrintingService printingService = new PrintingService();
                        printingService.writeInvoiceA4(SalesModel, journal1, journal2, journal3, journal4, false, journal5);
                    }

                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
                    if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        public JournalModel GetEJournalProforma(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord)
        {
            string line = "", line2 = "";
            if (UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
                line2 = "================================"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
                line2 = "==================================="; //35Byte
            }

            JournalModel journal = new JournalModel();

            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            // Header
            journal.Add("", "");
            journal.Add("", envPosSetup.GblTaxIdNm);
            journal.Add("", envPosSetup.GblBrcAdr);
            journal.Add("", "TEL: " + envPosSetup.GblBrcTel);
            journal.Add("", "EMAIL: " + envPosSetup.GblBrcEmail);
            journal.Add("", "PIN: " + envPosSetup.GblTaxIdNo);
            journal.Add("", "CASHIER: " + UIManager.Instance().UserModel.UserNm + "(" + UIManager.Instance().UserModel.UserId + ")");
            journal.Add("", line);
            if (!string.IsNullOrEmpty(salesModel.TranRecord.CustTin) && salesModel.TranRecord.CustTin.Length > 2)
            {
                journal.Add("CLIENT PIN: " + salesModel.TranRecord.CustTin);
                journal.Add("CLIENT NAME: " + salesModel.TranRecord.CustNm);
                journal.Add("", line);
            }

            for (int i = 0; i < salesModel.ItemRecords.Count; i++)
            {
                TrnsSaleItemRecord itemNode = salesModel.ItemRecords[i];
                getItemNodeString(journal, i, itemNode, 1);
            }

            if (UIManager.Instance().Is58mmPrinter)
            {
                journal.AddLine();
                journal.Add("THIS IS NOT AN OFFICIAL RECEIPT");
                journal.AddLine();
                if (salesModel.GetTotDCAmount() > 0)
                {
                    string strDcAmt = string.Format("{0:###,###,###0.00}", salesModel.GetTotDCAmount());
                    journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 10}", strDcAmt);
                }

                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (1))));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtA * (1))));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, ((salesModel.TranRecord.TaxblAmtB * (1))- (salesModel.TranRecord.TaxAmtB * (1)))));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtB * (1))));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, ((salesModel.TranRecord.TaxblAmtE * (1)) - (salesModel.TranRecord.TaxAmtE * (1)))));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtE * (1))));
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                {
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtD * (1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtD * (1))));
                }
                else
                {
                }
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotTaxAmt * (1))));

                journal.Add(line);

                journal.Add("           PROFORMA");
                journal.Add(line);
                journal.Add(Journal.JournalUtil.rpad(17, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotItemCnt)));
                journal.Add(line);

                journal.Add("".PadLeft(8) + "SCU INFORMATION" + "".PadLeft(9));
                journal.Add("Date: " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + " " + "Time:" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                journal.Add("SCU ID:  " + Journal.JournalUtil.lpad(17, envPosSetup.GblSdcSysNum));

                string flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo + "PS";
                journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));
                journal.Add("", line);
                journal.Add(Journal.JournalUtil.lpad(17, "INVOICE NUMBER :") + Journal.JournalUtil.lpad(15, envPosSetup.GblSdcSysNum) +"/" +salesModel.TranRecord.InvcNo) ;
                journal.Add("Date: " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + " " + "Time:" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                //journal.Add("MRC: " + envPosSetup.GblMrcSysCod);
                journal.Add("", line);
                if (!string.IsNullOrEmpty(salesModel.TranRecord.Remark))
                {
                    journal.Add("NON FISCAL DATA:");
                    journal.Add(salesModel.TranRecord.Remark);
                }
                //if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                //{
                //  journal.Add("Iyi nyemezabuguzi yemewe na RRA,");
                //journal.Add("      n'ubwo itari iya TVA");
                //journal.Add("This invoice is approved by RRA,");
                //journal.Add("      though is not for VAT");
                //journal.Add("", line);
                //}
                journal.Add("End of Legal Receipt");
                journal.Add("Powered by ETIMS v1");
                //journal.Add("", line2);

                journal.Add("");
                journal.Add("cutpaper", string.Empty);  //밑에 공백을 주는 부분
            }
            else
            {
                journal.AddLine();
                journal.Add("  THIS IS NOT AN OFFICIAL RECEIPT");
                journal.AddLine();
                if (salesModel.GetTotDCAmount() > 0)
                {
                    string strDcAmt = string.Format("{0:###,###,###0.00}", salesModel.GetTotDCAmount());
                    journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 13}", strDcAmt);
                }

                journal.Add("bold", Journal.JournalUtil.lpad(20, "TOTAL") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (1))));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtA * (1))));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtB * (1))));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtB * (1))));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtE * (1))));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtE * (1))));
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                {
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtD * (1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtD * (1))));
                }
                else
                {
                }
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotTaxAmt * (1))));

                journal.Add(line);

                journal.Add("             PROFORMA");
                journal.Add(line);
                journal.Add(Journal.JournalUtil.rpad(20, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotItemCnt)));
                journal.Add(line);

                journal.Add("".PadLeft(10) + "SCU INFORMATION" + "".PadLeft(10));
                journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + "   " + "Time :" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(20, envPosSetup.GblSdcSysNum));

                string flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo + "PS";
                journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));
                journal.Add("", line);
                journal.Add(Journal.JournalUtil.lpad(20, "INVOICE NUMBER :") + Journal.JournalUtil.lpad(15, envPosSetup.GblSdcSysNum) + "/" + salesModel.TranRecord.InvcNo);
                journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + "   " + "Time :" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                //journal.Add("MRC : " + envPosSetup.GblMrcSysCod);
                journal.Add("", line);
                if (!string.IsNullOrEmpty(salesModel.TranRecord.Remark))
                {
                    journal.Add("Non Fiscal Information:");
                    journal.Add(salesModel.TranRecord.Remark);
                }
                
                journal.Add("End of Legal Receipt");
                journal.Add("Powered by ETIMS v1");
                //journal.Add("", line2);

                journal.Add("");
                journal.Add("cutpaper", string.Empty);  
            }

            return journal;
        }
        public JournalModel GetEJournal(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord)
        {
            string line = "", line2 = "";
            if (UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
                line2 = "================================"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
                line2 = "==================================="; //35Byte
            }

            JournalModel journal = new JournalModel();

            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            // Header            
            journal.Add("", "");
            journal.Add("", envPosSetup.GblTaxIdNm);
            journal.Add("", envPosSetup.GblBrcAdr);
            journal.Add("", "TEL: " + envPosSetup.GblBrcTel);
            journal.Add("", "EMAIL: " + envPosSetup.GblBrcEmail);
            journal.Add("", "PIN: " + envPosSetup.GblTaxIdNo);
            journal.Add("", "CASHIER: " + UIManager.Instance().UserModel.UserNm + "(" + UIManager.Instance().UserModel.UserId + ")");
            journal.Add("", line);
            if (!string.IsNullOrEmpty(salesModel.TranRecord.CustTin) && salesModel.TranRecord.CustTin.Length > 2)
            {
                journal.Add("CLIENT PIN: " + salesModel.TranRecord.CustTin);
                journal.Add("CLIENT NAME: " + salesModel.TranRecord.CustNm);
                journal.Add("", line);
            }
            if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                if (UIManager.Instance().Is58mmPrinter)
                {
                    journal.Add("bold", "".PadLeft(13) + "CREDIT NOTE");
                }
                else
                {
                    journal.Add("bold", "".PadLeft(15) + "CREDIT NOTE");
                }
                // JCNA 20191203 inv id
                //posModel.Journal.Add("", "NORMAL RECEIPT#: " + posModel.TranModel.TranNode.RefundReasonNode.OrgBarcodeNo);
                journal.Add("", "NORMAL RECEIPT#: " + salesModel.TranRecord.OrgInvcNo);
                journal.Add("", line);
                journal.Add("", "CREDIT NOTE IS APPROVED ONLY FOR");
                journal.Add("", "ORIGINAL SALES RECEIPT");
                journal.Add(line);

                for (int i = 0; i < salesModel.ItemRecords.Count; i++)
                {
                    TrnsSaleItemRecord itemNode = SalesModel.ItemRecords[i];
                    getItemNodeString(journal, i, itemNode, -1);
                }

                if (salesModel.GetTotDCAmount() > 0)
                {
                    journal.AddLine();
                    string strDcAmt = string.Format("{0:###,###,###0.00}", salesModel.GetTotDCAmount() * (-1));
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 10}", strDcAmt);
                    }
                    else
                    {
                        journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 13}", strDcAmt);
                    }
                }

                if (UIManager.Instance().Is58mmPrinter)
                {
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtA * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtB * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtB * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtE * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtE * (-1))));
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                    {
                        journal.Add(Journal.JournalUtil.lpad(17, "TOTAL D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtD * (-1))));
                        journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtD * (-1))));
                    }
                    else
                    {
                    }
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotTaxAmt * (-1))));

                    journal.Add(line);
                    if (salesModel.TranRecord.PmtTyCd.Equals("01"))
                        journal.Add(Journal.JournalUtil.lpad(17, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("02"))
                        journal.Add(Journal.JournalUtil.lpad(17, "CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("03"))
                        journal.Add(Journal.JournalUtil.lpad(17, "CASH CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("04"))
                        journal.Add(Journal.JournalUtil.lpad(17, "BANK CHEQUE") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("05"))
                        journal.Add(Journal.JournalUtil.lpad(17, "DEBIT CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("06"))
                        journal.Add(Journal.JournalUtil.lpad(17, "MOBILE MONEY") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("07"))
                        journal.Add(Journal.JournalUtil.lpad(17, "OTHER") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else
                        journal.Add(Journal.JournalUtil.lpad(17, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                }
                else
                {
                    journal.Add("bold", Journal.JournalUtil.lpad(20, "TOTAL") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtA * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtB * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtB * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtE * (-1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtE * (-1))));
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                    {
                        journal.Add(Journal.JournalUtil.lpad(20, "TOTAL D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtD * (-1))));
                        journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtD * (-1))));
                    }
                    else
                    {
                    }
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotTaxAmt * (-1))));

                    journal.Add(line);
                    if (salesModel.TranRecord.PmtTyCd.Equals("01"))
                        journal.Add(Journal.JournalUtil.lpad(20, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("02"))
                        journal.Add(Journal.JournalUtil.lpad(20, "CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("03"))
                        journal.Add(Journal.JournalUtil.lpad(20, "CASH CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("04"))
                        journal.Add(Journal.JournalUtil.lpad(20, "BANK CHEQUE") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("05"))
                        journal.Add(Journal.JournalUtil.lpad(20, "DEBIT CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("06"))
                        journal.Add(Journal.JournalUtil.lpad(20, "MOBILE MONEY") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("07"))
                        journal.Add(Journal.JournalUtil.lpad(20, "OTHER") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                    else
                        journal.Add(Journal.JournalUtil.lpad(20, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (-1))));
                }
            }
            else
            {
                for (int i = 0; i < salesModel.ItemRecords.Count; i++)
                {
                    TrnsSaleItemRecord itemNode = salesModel.ItemRecords[i];
                    getItemNodeString(journal, i, itemNode, 1);
                }

                journal.AddLine();
                if (salesModel.GetTotDCAmount() > 0)
                {
                    string strDcAmt = string.Format("{0:###,###,###0.00}", salesModel.GetTotDCAmount());
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 10}", strDcAmt);
                    }
                    else
                    {
                        journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 13}", strDcAmt);
                    }
                }

                if (UIManager.Instance().Is58mmPrinter)
                {
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtA * (1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtB * (1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtB * (1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtE * (1))));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtE * (1))));
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                    {
                        journal.Add(Journal.JournalUtil.lpad(17, "TOTAL D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtD * (1))));
                        journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtD * (1))));
                    }
                    else
                    {
                    }
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotTaxAmt * (1))));

                    journal.Add(line);
                    if (salesModel.TranRecord.PmtTyCd.Equals("01"))
                        journal.Add(Journal.JournalUtil.lpad(17, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("02"))
                        journal.Add(Journal.JournalUtil.lpad(17, "CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("03"))
                        journal.Add(Journal.JournalUtil.lpad(17, "CASH CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("04"))
                        journal.Add(Journal.JournalUtil.lpad(17, "BANK CHEQUE") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("05"))
                        journal.Add(Journal.JournalUtil.lpad(17, "DEBIT CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("06"))
                        journal.Add(Journal.JournalUtil.lpad(17, "MOBILE MONEY") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("07"))
                        journal.Add(Journal.JournalUtil.lpad(17, "OTHER") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else
                        journal.Add(Journal.JournalUtil.lpad(17, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                }
                else
                {
                    journal.Add("bold", Journal.JournalUtil.lpad(20, "TOTAL") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt * (1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtA * (1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtB * (1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtB * (1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtE * (1))));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtE * (1))));
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                    {
                        journal.Add(Journal.JournalUtil.lpad(20, "TOTAL D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxblAmtD * (1))));
                        journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TaxAmtD * (1))));
                    }
                    else
                    {
                    }
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotTaxAmt * (1))));

                    journal.Add(line);
                    if (salesModel.TranRecord.PmtTyCd.Equals("01"))
                        journal.Add(Journal.JournalUtil.lpad(20, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("02"))
                        journal.Add(Journal.JournalUtil.lpad(20, "CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("03"))
                        journal.Add(Journal.JournalUtil.lpad(20, "CASH CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("04"))
                        journal.Add(Journal.JournalUtil.lpad(20, "BANK CHEQUE") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("05"))
                        journal.Add(Journal.JournalUtil.lpad(20, "DEBIT CREDIT") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("06"))
                        journal.Add(Journal.JournalUtil.lpad(20, "MOBILE MONEY") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else if (salesModel.TranRecord.PmtTyCd.Equals("07"))
                        journal.Add(Journal.JournalUtil.lpad(20, "OTHER") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                    else
                        journal.Add(Journal.JournalUtil.lpad(20, "CASH") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotAmt)));
                }
            }


            if (UIManager.Instance().Is58mmPrinter)
            {
                journal.Add(line);
                // JCNA 20200130
                journal.Add("reprint", "");
                journal.Add(Journal.JournalUtil.rpad(17, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotItemCnt)));
                journal.Add(line);

                journal.Add("".PadLeft(8) + "SCU INFORMATION" + "".PadLeft(9));
                journal.Add("Date: " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + " " + "Time:" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                journal.Add("SCU ID:  " + Journal.JournalUtil.lpad(17, envPosSetup.GblSdcSysNum));
            }
            else
            {
                journal.Add(line);
                // JCNA 20200130
                journal.Add("reprint", "");
                journal.Add(Journal.JournalUtil.rpad(20, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (salesModel.TranRecord.TotItemCnt)));
                journal.Add(line);

                journal.Add("".PadLeft(10) + "SCU INFORMATION" + "".PadLeft(10));
                journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + "   " + "Time :" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(20, envPosSetup.GblSdcSysNum));
            }

            string flag = "";
            if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo + "NR";
            }
            else
            {
                flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo + "NS";
            }

            if (UIManager.Instance().Is58mmPrinter)
            {
                //posModel.Journal.AddFormat("RECEIPT NUMBER : {0, 18}", flag);
                journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));

                journal.Add("".PadLeft(8) + "Internal Data :");
                journal.Add(GetFormatData(trnsSaleReceiptRecord.IntrlData));
                //posModel.Journal.Add("2ZQS-U6NW-7NYF-MLWN-ZFHR-6FF5-AQ");
                journal.Add("".PadLeft(6) + "Receipt Signature :");
                journal.Add(GetFormatData(trnsSaleReceiptRecord.RcptSign));
                //posModel.Journal.Add("PE5K-66C7-RE3L-BZCB");
                journal.Add("", line);
                //posModel.Journal.AddFormat("RECEIPT NUMBER :    {0,-15}", posModel.TranInformation.ReceiptNumber.ToString());

                // JCNA 20191203 inv id로 교체
                //posModel.Journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, posModel.TranInformation.ReceiptNumber));
                journal.Add(Journal.JournalUtil.lpad(17, "INVOICE NUMBER :") + Journal.JournalUtil.lpad(15, envPosSetup.GblSdcSysNum) + "/" + salesModel.TranRecord.InvcNo);
                journal.Add("Date: " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + " " + "Time:" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                //journal.Add("MRC: " + envPosSetup.GblMrcSysCod);
                journal.Add("", line);
            }
            else
            {
                //posModel.Journal.AddFormat("RECEIPT NUMBER : {0, 18}", flag);
                journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));

                journal.Add("".PadLeft(10) + "Internal Data :");
                journal.Add(GetFormatData(trnsSaleReceiptRecord.IntrlData));
                //posModel.Journal.Add("2ZQS-U6NW-7NYF-MLWN-ZFHR-6FF5-AQ");
                journal.Add("".PadLeft(8) + "Receipt Signature :");
                journal.Add(GetFormatData(trnsSaleReceiptRecord.RcptSign));
                //posModel.Journal.Add("PE5K-66C7-RE3L-BZCB");
                journal.Add("", line);
                //posModel.Journal.AddFormat("RECEIPT NUMBER :    {0,-15}", posModel.TranInformation.ReceiptNumber.ToString());

                // JCNA 20191203 inv id로 교체
                //posModel.Journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, posModel.TranInformation.ReceiptNumber));
                journal.Add(Journal.JournalUtil.lpad(20, "INVOICE NUMBER :") + Journal.JournalUtil.lpad(15, envPosSetup.GblSdcSysNum) + "/" + salesModel.TranRecord.InvcNo);
                journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + "   " + "Time :" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                //journal.Add("MRC : " + envPosSetup.GblMrcSysCod);
                journal.Add("", line);
            }
            if (!string.IsNullOrEmpty(salesModel.TranRecord.Remark))
            {
                journal.Add("NON FISCAL DATA:");
                journal.Add(salesModel.TranRecord.Remark);
            }
            //commented by Aime
            // if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
            //{
            //  journal.Add("Iyi nyemezabuguzi yemewe na RRA,");
            // journal.Add("      n'ubwo itari iya TVA");
            //journal.Add("This invoice is approved by RRA,");
            //journal.Add("      though is not for VAT");
            //journal.Add("", line);
            //}
            journal.Add("End of Legal Receipt");
            journal.Add("Powered by ETIMS v1");
            //journal.Add("", line2);

            // 반품일 경우 바코드를 출력하지 않음
            if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                journal.Add(string.Empty);
                /*Added By Bright 4.6.2022
              string qrcode =trnsSaleReceiptRecord.RcptSign;
              */
                //string qrcode = trnsSaleReceiptRecord.RcptSign;
                string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + trnsSaleReceiptRecord.RcptSign;

                //END Added By Bright 4.6.2022

                //string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + salesModel.TranRecord.InvcNo;
                //Added By Bright 3.4.2022 End
                journal.Add("qrcode", qrcode);
            }
            else
            {   //Added By Bright 3.4.2022 start

                /*Added By Bright 4.6.2022
                string qrcode =trnsSaleReceiptRecord.RcptSign;
                */
                //string qrcode = trnsSaleReceiptRecord.RcptSign;
                string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + trnsSaleReceiptRecord.RcptSign;

                //END Added By Bright 4.6.2022

                //string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + salesModel.TranRecord.InvcNo;
                //Added By Bright 3.4.2022 End
                journal.Add("qrcode", qrcode);
            }

            journal.Add("cutpaper", string.Empty);  //밑에 공백을 주는 부분

            return journal;
        }

        //method added by Aime for qr code of printout of A4
        public JournalModel GetEJournalInvoice5(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord)
        {
            JournalModel journal = new JournalModel();
            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            // 반품일 경우 바코드를 출력하지 않음
            if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                journal.Add(string.Empty);
            }
            else
            {
                string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + trnsSaleReceiptRecord.RcptSign;

                journal.Add("qrcode", qrcode);
            }
            return journal;
        }

        public JournalModel GetEJournalInvoice1(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord, bool reprint)
        {
            JournalModel journal = new JournalModel();

            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            // Header
            journal.Add("", "");
            journal.Add("", envPosSetup.GblTaxIdNm);
            journal.Add("", envPosSetup.GblBrcAdr);
            journal.Add("", "TEL: " + envPosSetup.GblBrcTel);
            journal.Add("", "EMAIL: " + envPosSetup.GblBrcEmail);
            journal.Add("", "PIN: " + envPosSetup.GblTaxIdNo);
            journal.Add("", "CASHIER: " + UIManager.Instance().UserModel.UserNm + "(" + UIManager.Instance().UserModel.UserId + ")");

            return journal;
        }
        public JournalModel GetEJournalInvoice2(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord, bool reprint)
        {
            JournalModel journal = new JournalModel();
            // Header
            journal.Add("PIN : " + salesModel.TranRecord.CustTin);
            if (salesModel.TranRecord.CustNm.Length > 30)
            {
                journal.Add("Name :" + salesModel.TranRecord.CustNm.Substring(0, 30));
                journal.Add(salesModel.TranRecord.CustNm.Substring(30));
                journal.Add("");
            }
            else
            {
                journal.Add("Name : " + salesModel.TranRecord.CustNm);
            }


            return journal;
        }
        public JournalModel GetEJournalInvoice3(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord, bool reprint)
        {
            JournalModel journal = new JournalModel();

            // Header
            journal.Add("bold", "INVOICE NO : " + salesModel.TranRecord.InvcNo);
            if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
            {
                journal.Add("REFUND");
                journal.Add("REF.NORMAL RECEIPT#: " + salesModel.TranRecord.OrgInvcNo);
            }
            else
            {
                journal.Add("");
                journal.Add("");
            }
            //journal.Add("");
            journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy"));

            return journal;
        }

        public JournalModel GetEJournalInvoice4(TransactionSalesModel salesModel, TrnsSaleReceiptRecord trnsSaleReceiptRecord, bool reprint)
        {
            string line = "", line2 = "";
            if (UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
                line2 = "================================"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
                line2 = "==================================="; //35Byte
            }


            JournalModel journal = new JournalModel();

            Models.config.EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            if (UIManager.Instance().Is58mmPrinter)
            {
                journal.Add("SCU INFORMATION");
                journal.Add("", line);
                journal.Add("Date: " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + " " + "Time:" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                journal.Add("SCU ID:  " + envPosSetup.GblSdcSysNum);

                string flag = "";
                if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
                {
                    flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo;
                    if (reprint) flag += "CR";
                    else flag += "NR";
                }
                else
                {
                    flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo;
                    if (reprint) flag += "CS";
                    else flag += "NS";
                }

                journal.Add("RECEIPT NUMBER : " + flag);
                journal.Add("Internal Data : " + GetFormatData(trnsSaleReceiptRecord.IntrlData));
                journal.Add("Receipt Signature : " + GetFormatData(trnsSaleReceiptRecord.RcptSign));
                journal.Add("", line);
                journal.Add("INVOICE NUMBER : " + salesModel.TranRecord.InvcNo);
                journal.Add("Date: " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + " " + "Time:" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                //journal.Add("MRC: " + envPosSetup.GblMrcSysCod);
                journal.Add("Powered by ETIMS v1");
                //string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + trnsSaleReceiptRecord.RcptSign;
                //journal.Add("qrcode", qrcode);
            }
            else
            {
                journal.Add("SCU INFORMATION");
                journal.Add("", line);
                journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + "   " + "Time :" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                journal.Add("SCU ID :  " + envPosSetup.GblSdcSysNum);

                string flag = "";
                if (!string.IsNullOrEmpty(salesModel.TranRecord.RcptTyCd) && salesModel.TranRecord.RcptTyCd.Equals("R"))
                {
                    flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo;
                    if (reprint) flag += "CR";
                    else flag += "NR";
                }
                else
                {
                    flag = trnsSaleReceiptRecord.CurRcptNo + "/" + trnsSaleReceiptRecord.TotRcptNo;
                    if (reprint) flag += "CS";
                    else flag += "NS";
                }

                journal.Add("RECEIPT NUMBER : " + flag);

                journal.Add("Internal Data : ");
                //journal.Add(GetFormatData(trnsSaleReceiptRecord.IntrlData));
                // journal.Add("Receipt Signature : ");
                //journal.Add(GetFormatData(trnsSaleReceiptRecord.RcptSign));
                //journal.Add("", line);
                journal.Add("RECEIPT NUMBER : " + salesModel.TranRecord.InvcNo);
                journal.Add("Date : " + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString("dd-MM-yyyy") + "   " + "Time :" + Common.DateTimeFormat(salesModel.TranRecord.CfmDt).ToString(" HH:mm:ss"));
                //journal.Add("MRC : " + envPosSetup.GblMrcSysCod);
                journal.Add("Powered by ETIMS v1");
            }
            //add data from qr code

            string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + trnsSaleReceiptRecord.RcptSign;
            journal.Add("qrcode", qrcode);
            //end of data for qr code

            return journal;
        }
        public string GetFormatData(string data)
        {
            string buffer = "";
            for (int i = 1; i <= data.Length; i++)
            {
                buffer = buffer + data.Substring(i - 1, 1);
                if (i % 4 == 0 && i < data.Length) buffer = buffer + "-";
            }
            return buffer;
        }
        public void getItemNodeString(JournalModel journal, int seq, TrnsSaleItemRecord itemNode, int sign)
        {
            journal.Add("", itemNode.ItemNm);
            journal.Add("", itemNode.ItemCd);

            string taxName = "";
            switch (itemNode.TaxTyCd)
            {
                case "A":
                    taxName = "A-EX ";
                    break;
                case "B":
                    taxName = "B-16%";
                    break;
                case "C":
                    taxName = "TAX C";
                    break;
                case "D":
                    taxName = "TAX D";
                    break;
                case "E":
                    taxName = "E-8%";
                    break;
                default:
                    break;
            }

            string txtPrcie = string.Format("{0:###,###,###0.00}", itemNode.Prc * sign);
            string txtTotal = string.Format("{0:###,###,###0.00}", itemNode.TotAmt * sign);

            //journal.AddFormat("{0,-18}{1,17}", txtPrcie + "x" + itemNode.Qty, txtTotal + taxName);
            if (UIManager.Instance().Is58mmPrinter)
            {
                journal.AddFormat("{0,-15}{1,17}", txtPrcie + "x" + itemNode.Qty, txtTotal + taxName);
            }
            else
            {
                journal.AddFormat("{0,-18}{1,17}", txtPrcie + "x" + itemNode.Qty, txtTotal + taxName);
            }

            if (itemNode.DcAmt != 0)
            {
                string strDiscountAmount = string.Format("{0:#,###,###0.00}", itemNode.DcAmt * sign);
                string strSubtotal = string.Format("{0:###,###,###0.00}", itemNode.SplyAmt * sign);
                if (itemNode.IsrcRt > 0)
                {
                    //journal.AddFormat("{0,10} Insurance {1,10}", strDiscountAmount, strSubtotal + "     ");
                    //journal.AddFormat("{0,12} {1,-8} {2,18}", strDiscountAmount, "INS:" + itemNode.IsrcRt + "%", strSubtotal + "     ");
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        journal.AddFormat("{0,10} {1,-8} {2,12}", strDiscountAmount, "INS:" + itemNode.IsrcRt + "%", strSubtotal);
                    }
                    else
                    {
                        journal.AddFormat("{0,12} {1,-8} {2,18}", strDiscountAmount, "INS:" + itemNode.IsrcRt + "%", strSubtotal + "     ");
                    }
                }
                else
                {
                    //journal.AddFormat("{0,13} D/C {1,17}", strDiscountAmount, strSubtotal + "     ");
                    //journal.AddFormat("{0,12} {1,-8} {2,18}", strDiscountAmount, "D/C:" + itemNode.DcRt + "%", strSubtotal + "     ");
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        journal.AddFormat("{0,10} {1,-8} {2,12}", strDiscountAmount, "D/C:" + itemNode.DcRt + "%", strSubtotal);
                    }
                    else
                    {
                        journal.AddFormat("{0,12} {1,-8} {2,18}", strDiscountAmount, "D/C:" + itemNode.DcRt + "%", strSubtotal + "     ");
                    }
                }
            }
        }


    }
}

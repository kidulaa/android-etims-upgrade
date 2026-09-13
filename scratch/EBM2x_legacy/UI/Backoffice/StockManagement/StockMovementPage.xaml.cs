using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Dependency;
using EBM2x.Models.journal;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.Utils;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StockMovementPage : ContentPage
    {
        // 재고이동
        TransactionStockInOutModel StockInOutModel { get; set; }
        StockIoItemRecord SelectedItem;

        public StockMovementPage(string id)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            Init(id);

            if (!string.IsNullOrEmpty(id)) etAfterLocation.SetSelecteItem(new Models.config.SystemCode() { Id = id, Name = "" });
        }
        public void Init(string id)
        {
            StockIoMaster StockIoMaster = new StockIoMaster();
            // 초기화
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            long SarNo = StockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "M"; // 입력유형 (A:자동, M:수기입력)
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            StockInOutModel = new TransactionStockInOutModel();
            StockInOutModel.CurrentItemRecord = null;
            StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
            StockInOutModel.TranRecord.CustBhfId = id;
            StockInOutModel.TranRecord.SarTyCd = "13"; // 재고유형 - 13 : 재고이동출고 
                                                       //            04 : 재고이동입고
            UpdateHeaderView();
            UpdateItemView(new StockIoItemRecord());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderView();

            if (StockInOutModel.CurrentItemRecord != null)
            {
                UpdateItemView(StockInOutModel.CurrentItemRecord);
                etQuantity.SetReadOnly(false);
                etQuantity.SetFocus();
            }
            else
            {
                UpdateItemView(new StockIoItemRecord());
            }
        }

        public void UpdateHeaderView()
        {
        }
        public void UpdateItemView(StockIoItemRecord itemRecord)
        {
            etItemCode.SetEntryValue(itemRecord.ItemCd);
            etItemName.SetEntryValue(itemRecord.ItemNm);
            etClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etClassName.SetEntryValue(itemRecord.ItemClsNm);
            etCurrentStock.SetEntryValue(itemRecord.RdsQty.ToString());
            etQuantity.SetEntryValue(itemRecord.Qty);

            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            etCurrentStock.SetReadOnly(true);
            etQuantity.SetReadOnly(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionSave(object sender, System.EventArgs e)
        {
            StockIoMaster StockIoMaster = new StockIoMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            if (string.IsNullOrEmpty(etAfterLocation.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select the After Location.", "Ok");
                return;
            }

            StockInOutModel.TranRecord.CustBhfId = etAfterLocation.GetSelectedItem().Id;

            // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
            StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
            StockIoMaster.InsertTable(StockInOutModel.TranRecord);
            StockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            //JCNA 20191204
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            // PRINTING
            var popupPage = new PrintReceiptPage(StockInOutModel, false);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string paymentName = (string)((ExtEventArgs)ex).EnteredText;
                    Navigation.PopAsync();

                    JournalModel journal = GetEJournalDeliveryNote(StockInOutModel);
                    if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintReceipt"))
                    {
                        PrintingService printingService = new PrintingService();
                        printingService.writeJurnal(journal, false);
                    }
                    else if (!string.IsNullOrEmpty(paymentName) && paymentName.Equals("PrintA4"))
                    {
                        PrintingService printingService = new PrintingService();
                        printingService.writeJurnalA4(journal, false);
                    }

                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
                    Navigation.PopAsync();
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }

        public JournalModel GetEJournalDeliveryNote(TransactionStockInOutModel stockInOutModel)
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
            journal.Add("CLIENT PIN: " + stockInOutModel.TranRecord.CustBhfId);
            journal.Add("CLIENT NAME: " + "");
            journal.Add("", line);

            for (int i = 0; i < stockInOutModel.ItemRecords.Count; i++)
            {
                StockIoItemRecord itemNode = stockInOutModel.ItemRecords[i];
                getItemNodeString(journal, i, itemNode, 1);
            }

            if (UIManager.Instance().Is58mmPrinter)
            {
                journal.AddLine();
                journal.Add("THIS IS NOT AN OFFICIAL RECEIPT");
                journal.AddLine();

                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotAmt)));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxblAmtA)));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxblAmtB)));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxAmtB)));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxblAmtE)));
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxAmtE)));
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                {
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL D") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxblAmtD)));
                    journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxAmtD)));
                }
                else
                {
                }
                journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotTaxAmt)));

                journal.Add(line);

                journal.Add("         DELIVERY NOTE");
                journal.Add(line);
                journal.Add(Journal.JournalUtil.rpad(17, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotItemCnt)));
                journal.Add(line);

                journal.Add("".PadLeft(9) + "SDC INFORMATION" + "".PadLeft(8));
                journal.Add("Date: " + Common.DateFormat(stockInOutModel.TranRecord.OcrnDt).ToString("dd-MM-yyyy") + " " + "Time:" + DateTime.Now.ToString(" HH:mm:ss"));
                journal.Add("SCU ID:  " + Journal.JournalUtil.lpad(17, envPosSetup.GblSdcSysNum));

                TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
                long curRcptNo = trnsSaleReceiptMaster.GetReceiptSeq();
                long totRcptNo = trnsSaleReceiptMaster.GetTotReceiptSeq();
                string flag = curRcptNo + "/" + totRcptNo + "DS";
                journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));
                journal.Add("", line);
                journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, stockInOutModel.TranRecord.SarNo));
                journal.Add("Date: " + Common.DateFormat(stockInOutModel.TranRecord.OcrnDt).ToString("dd-MM-yyyy") + " " + "Time:" + DateTime.Now.ToString(" HH:mm:ss"));
                //journal.Add("MRC: " + envPosSetup.GblMrcSysCod);
                journal.Add("", line);
                //if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                //{
                //  journal.Add("Iyi nyemezabuguzi yemewe na RRA,");
                // journal.Add("      n'ubwo itari iya TVA");
                //journal.Add("This invoice is approved by RRA,");
                //journal.Add("      though is not for VAT");
                //journal.Add("", line);
                // }
                journal.Add("End of Legal Receipt  Powered by EBM v2");
                journal.Add("", line2);
            }
            else
            {
                journal.AddLine();
                journal.Add("  THIS IS NOT AN OFFICIAL RECEIPT");
                journal.AddLine();

                //journal.Add(Journal.JournalUtil.lpad(20, "TOTAL") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotAmt)));
                //journal.Add(Journal.JournalUtil.lpad(20, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxblAmtA)));
                //journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-18%") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxblAmtB)));
                //journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TaxAmtB)));
                //journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotTaxAmt)));

                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL") + Journal.JournalUtil.lpad(15, (0)));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (0)));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (0)));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (0)));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (0)));
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (0)));
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                {
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL D") + Journal.JournalUtil.lpad(15, (0)));
                    journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (0)));
                }
                else
                {
                }
                journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (0)));
                journal.Add(line);

                journal.Add("          DELIVERY NOTE");
                journal.Add(line);
                journal.Add(Journal.JournalUtil.rpad(20, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotItemCnt)));
                journal.Add(line);

                journal.Add("".PadLeft(10) + "SDC INFORMATION" + "".PadLeft(10));
                journal.Add("Date : " + Common.DateFormat(stockInOutModel.TranRecord.OcrnDt).ToString("dd-MM-yyyy") + "   " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
                journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(20, envPosSetup.GblSdcSysNum));

                TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
                long curRcptNo = trnsSaleReceiptMaster.GetReceiptSeq();
                long totRcptNo = trnsSaleReceiptMaster.GetTotReceiptSeq();
                string flag = curRcptNo + "/" + totRcptNo + "DS";
                journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));
                journal.Add("", line);
                journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, stockInOutModel.TranRecord.SarNo));
                journal.Add("Date : " + Common.DateFormat(stockInOutModel.TranRecord.OcrnDt).ToString("dd-MM-yyyy") + "   " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
               // journal.Add("MRC : " + envPosSetup.GblMrcSysCod);
                journal.Add("", line);
                //if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                //{
                //  journal.Add("Iyi nyemezabuguzi yemewe na RRA,");
                // journal.Add("      n'ubwo itari iya TVA");
                //journal.Add("This invoice is approved by RRA,");
                //journal.Add("      though is not for VAT");
                //journal.Add("", line);
                //}
                journal.Add("End of Legal Receipt");
                journal.Add("Powered by EBM v2");
                journal.Add("", line2);
            }

            journal.Add("");
            journal.Add("cutpaper", string.Empty);  //밑에 공백을 주는 부분

            return journal;
        }

        async void OnFunctionClose(object sender, System.EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            await Navigation.PopAsync();
        }

        async void OnFunctionConfirm(object sender, System.EventArgs e)
        {
            if (StockInOutModel.CurrentItemRecord != null)
            {
                //etQuantity.GetEntry().C // Entry 입력 종료처리 필요.
                if (etQuantity.GetEntryValue() != 0)
                {
                    if (StockInOutModel.CurrentItemRecord.RdsQty - etQuantity.GetEntryValue() < 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Processing Quantity is greater than current stock qty. Please Check.", "OK");
                    }
                    else
                    {
                        StockInOutModel.CurrentItemRecord.Qty = etQuantity.GetEntryValue();
                        StockInOutModel.CurrentItemRecord.AfterQty = StockInOutModel.CurrentItemRecord.RdsQty - StockInOutModel.CurrentItemRecord.Qty;
                        StockInOutModel.CalculateCurrentItem();
                        StockInOutModel.ConfirmCurrentItem();
                        listView.ItemsSource = StockInOutModel.GetObservableCollection();

                        UpdateHeaderView();
                        UpdateItemView(new StockIoItemRecord());
                    }
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
                }
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a product.", "OK");
            }
        }
        private void OnRemove(object sender, System.EventArgs e)
        {
            if (SelectedItem != null)
            {
                StockInOutModel.Delete(SelectedItem);
                listView.ItemsSource = StockInOutModel.GetObservableCollection();
            }
        }
        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (StockIoItemRecord)e.SelectedItem;
            }
            else
            {
                SelectedItem = null;
            }
        }

        private void OnEmpty(object sender, System.EventArgs e)
        {
            StockInOutModel.DeleteAll();
            listView.ItemsSource = StockInOutModel.GetObservableCollection();
        }

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            var popupPage = new ItemPopupPage("", "3");
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;
                    if (StockInOutModel.IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {
                        StockInOutModel.SetCurrentItem(popupRecord);
                        UpdateItemView(StockInOutModel.CurrentItemRecord);
                    }
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

        public void getItemNodeString(JournalModel journal, int seq, StockIoItemRecord itemNode, int sign)
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

            string txtPrcie = string.Format("{0:###,###,###0.00}", 0 * sign);
            string txtTotal = string.Format("{0:###,###,###0.00}", 0 * sign);
            //string txtPrcie = string.Format("{0:###,###,###0.00}", itemNode.Prc * sign);
            //string txtTotal = string.Format("{0:###,###,###0.00}", itemNode.TotAmt * sign);

            //journal.AddFormat("{0,-15}{1,3}{2,17}", txtPrcie + "x", itemNode.Qty, txtTotal + taxName);
            if (UIManager.Instance().Is58mmPrinter)
            {
                journal.AddFormat("{0,-15}{1,17}", txtPrcie + "x" + itemNode.Qty, txtTotal + taxName);
            }
            else
            {
                journal.AddFormat("{0,-18}{1,17}", txtPrcie + "x" + itemNode.Qty, txtTotal + taxName);
            }
        }
    }
}

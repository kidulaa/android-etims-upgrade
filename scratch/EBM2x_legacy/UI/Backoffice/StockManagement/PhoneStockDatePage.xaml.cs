using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Dependency;
using EBM2x.Models.journal;
using EBM2x.RraSdc.process;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneStockDatePage : ContentPage
    {
        public ObservableCollection<StockDateRecord> lvItemManagement { get; set; }
        List<StockDateRecord> listOpeningCloseingStockRecord;
        StockDateMaster master = null;
        StockDateRecord record = null;

        // 재고이동
        TransactionStockInOutModel StockInOutModel { get; set; }
        StockIoItemRecord SelectedItem;

        public PhoneStockDatePage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new StockDateMaster();
            record = new StockDateRecord();

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
            if (listOpeningCloseingStockRecord == null || listOpeningCloseingStockRecord.Count <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "No product found.", "OK");
                return;
            }

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;

            if (BhfId.Equals("00"))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Not available in Head office.", "OK");
                return;
            }

            StockIoMaster StockIoMaster = new StockIoMaster();
            long SarNo = StockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "M"; // 입력유형 (A:자동, M:수기입력)
            string UserId = "";
            string UserNm = "";

            StockInOutModel = new TransactionStockInOutModel();
            StockInOutModel.CurrentItemRecord = null;
            StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
            StockInOutModel.TranRecord.CustBhfId = "00";
            StockInOutModel.TranRecord.SarTyCd = "13"; // 재고유형 - 13 : 재고이동출고 
                                                       //            04 : 재고이동입고

            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            for (int i = 0; i < listOpeningCloseingStockRecord.Count; i++)
            {
                StockDateRecord stockDateRecord = listOpeningCloseingStockRecord[i];
                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                taxpayerItemMaster.ToRecord(itemRecord, Tin, stockDateRecord.ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                StockInOutModel.SetCurrentItem(itemRecord);
                StockInOutModel.CurrentItemRecord.Qty = stockDateRecord.InitlQty;
                StockInOutModel.CurrentItemRecord.AfterQty = 0;
                StockInOutModel.CalculateCurrentItem();
                StockInOutModel.ConfirmCurrentItem();
            }

            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

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

            JournalModel journal = GetEJournalDeliveryNote(StockInOutModel);
            PrintingService printingService = new PrintingService();
            printingService.writeJurnal(journal, false);
        }

        public JournalModel GetEJournalDeliveryNote(TransactionStockInOutModel stockInOutModel)
        {
            string line = "", line2 = "";
            line = "--------------------------------"; //35Byte
            line2 = "================================"; //35Byte

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

            journal.AddLine();
            journal.Add("THIS IS NOT AN OFFICIAL RECEIPT");
            journal.AddLine();

            journal.Add("bold", Journal.JournalUtil.lpad(17, "TOTAL") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotAmt)));
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

            journal.Add("       DELIVERY NOTE");
            journal.Add(line);
            journal.Add(Journal.JournalUtil.rpad(17, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (stockInOutModel.TranRecord.TotItemCnt)));
            journal.Add(line);

            journal.Add("".PadLeft(8) + "SDC INFORMATION" + "".PadLeft(9));
            journal.Add("Date : " + Common.DateFormat(stockInOutModel.TranRecord.OcrnDt).ToString("dd-MM-yyyy") + "   " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
            journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(17, envPosSetup.GblSdcSysNum));

            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
            long curRcptNo = trnsSaleReceiptMaster.GetReceiptSeq();
            long totRcptNo = trnsSaleReceiptMaster.GetTotReceiptSeq();
            string flag = curRcptNo + "/" + totRcptNo + "DS";
            journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));
            journal.Add("", line);
            journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, stockInOutModel.TranRecord.SarNo));
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
            journal.Add("Powered by ETIMS v2");
            //journal.Add("", line2);

            journal.Add("");
            journal.Add("cutpaper", string.Empty);  //밑에 공백을 주는 부분

            return journal;
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

            string txtPrcie = string.Format("{0:###,###,###0.00}", itemNode.Prc * sign);
            string txtTotal = string.Format("{0:###,###,###0.00}", itemNode.TotAmt * sign);

            journal.AddFormat("{0,-15}{1,17}", txtPrcie + "x" + itemNode.Qty, txtTotal + taxName);
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

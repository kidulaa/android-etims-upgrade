using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Datafile.regitotal;
using EBM2x.Datafile.trlog;
using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Models.regitotal;
using EBM2x.Models.tran;
using EBM2x.Modules;
using EBM2x.RraSdc;
using EBM2x.RraSdc.process;
using EBM2x.UI;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;

namespace EBM2x.Process.eot
{
    public class TranEndOfTransaction
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (!posModel.RegiTotal.RegiHeader.OpenDate.Equals(string.Empty) && posModel.TranModel.TranNode != null)
            {
                RegiAmount regiAmount = new RegiAmount();
                regiAmount.excuteProcess(posModel, inputModel, informationModel);
            }

            string barcodeNo = posModel.RegiTotal.RegiHeader.getBarcodeText(posModel.TranModel.TranInformation.ReceiptNumber);

            posModel.RegiTotal.RegiHeader.UpdateDateTime();
            posModel.TranInformation.DateTime = posModel.RegiTotal.RegiHeader.UpdateDate + posModel.RegiTotal.RegiHeader.UpdateTime;
            posModel.TranModel.TranInformation.initialize(posModel.TranInformation);
            
            posModel.TranModel.Journal = posModel.Journal;

            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            posModel.TranModel.TranInformation.InvoiceN0 = trnsSaleMaster.GetSalesSeq();
            TransactionWriter.write(posModel);

            updateProcess(posModel);

            // RegiTotal json 
            RegiTotalWriter.write(posModel);

            // OperTotal json
            OperTotalWriter.write(posModel);

            //    // Tran Database 
            //    TranWriter.write(posModel);
            //    DataFile.trandb.Writer.write(posModel);


            

           
            if (posModel.Journal.PrintFlag)
            {
                PrintingService printingService = new PrintingService();
                //JINIT_printingService.writeJurnal(posModel.TranModel.Journal.JournalStringList, false);
                printingService.writeJurnal(posModel.TranModel.Journal, false);
            }

            
            TranModel tranModel = null;
            if (! string.IsNullOrEmpty(posModel.TranModel.TranNode.RefundReasonNode.OrgBarcodeNo))
            {
                string orgBarcodeNo = posModel.TranModel.TranNode.RefundReasonNode.OrgBarcodeNo;

                if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL && UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServerType.Equals("Slave"))
                {
                    SocketModel socketRequestModel = new SocketModel();
                    socketRequestModel.WCC = "Overwrite";
                    socketRequestModel.JsonRequest = orgBarcodeNo;

                    SocketModel socketResponseModel = Common.Send(socketRequestModel, UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer, 11129);
                    if (socketResponseModel != null)
                    {
                        if (socketResponseModel.WCC.Equals("SUCCESS"))
                        {
                        }
                        else
                        {
                            tranModel = TransactionReader.read(orgBarcodeNo);
                            if (tranModel != null)
                            {
                                tranModel.TranInformation.RefundBarcodeNo = barcodeNo;
                                TransactionWriter.overwrite(tranModel, orgBarcodeNo);
                            }
                            else
                            {
                                try
                                {
                                    tranModel = TransactionReader.read(orgBarcodeNo.Substring(1, 8), orgBarcodeNo);
                                    if (tranModel != null)
                                    {
                                        tranModel.TranInformation.RefundBarcodeNo = barcodeNo;
                                        TransactionWriter.overwrite(tranModel, orgBarcodeNo.Substring(1, 8), orgBarcodeNo);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        tranModel = TransactionReader.read(orgBarcodeNo);
                        if (tranModel != null)
                        {
                            tranModel.TranInformation.RefundBarcodeNo = barcodeNo;
                            TransactionWriter.overwrite(tranModel, orgBarcodeNo);
                        }
                        else
                        {
                            tranModel = TransactionReader.read(orgBarcodeNo.Substring(1, 8), orgBarcodeNo);
                            if (tranModel != null)
                            {
                                tranModel.TranInformation.RefundBarcodeNo = barcodeNo;
                                TransactionWriter.overwrite(tranModel, orgBarcodeNo.Substring(1, 8), orgBarcodeNo);
                            }
                        }
                    }
                }
                else
                {
                    tranModel = TransactionReader.read(orgBarcodeNo);
                    if (tranModel != null)
                    {
                        tranModel.TranInformation.RefundBarcodeNo = barcodeNo;
                        TransactionWriter.overwrite(tranModel, orgBarcodeNo);
                    }
                    else
                    {
                        tranModel = TransactionReader.read(orgBarcodeNo.Substring(1, 8), orgBarcodeNo);
                        if (tranModel != null)
                        {
                            tranModel.TranInformation.RefundBarcodeNo = barcodeNo;
                            TransactionWriter.overwrite(tranModel, orgBarcodeNo.Substring(1, 8), orgBarcodeNo);
                        }
                    }
                }
            }

            // JCNA 20191204 TR 
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            return StateModel.OP_FAR;
        }

        public static void updateProcess(PosModel posModel)
        {
            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            TrnsSaleItemMaster trnsSaleItemMaster = new TrnsSaleItemMaster();
            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
            StockIoMaster stockIoMaster = new StockIoMaster();
            StockIoItemMaster stockIoItemMaster = new StockIoItemMaster();

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string TaxprNm = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNm;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            //JCNA 202001 DELETE string DvcId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblDvcId;
            long InvcNo = trnsSaleMaster.GetSalesSeq();
            string SalesDt = DateTime.Now.ToString("yyyyMMdd");
            string SalesTyCd = "N";

            // JINIT_20191201, 
            //string RcptTyCd = "S";             // S : Sale, R : Refund  
            string RcptTyCd = UIManager.Instance().PosModel.TranModel.TranNode.TranFlag;

            // JCNA 20191211
            if(RcptTyCd.Equals("N")) RcptTyCd = "S";
            else if (RcptTyCd.Equals("R")) RcptTyCd = "R";
            else RcptTyCd = "S";

            /* JINIT_20191201, 
            string UserId = posModel.TranModel.TranInformation.UserCode;
            string UserNm = posModel.TranModel.TranInformation.UserName;
            */
            string UserId = posModel.RegiTotal.RegiHeader.UserID;
            string UserNm = posModel.RegiTotal.RegiHeader.UserName;

            TransactionSalesModel SalesModel = new TransactionSalesModel();
            SalesModel.InitModel(Tin, BhfId, InvcNo, SalesDt, SalesTyCd, RcptTyCd, UserId, UserNm);

            SalesModel.TranRecord.SalesSttsCd = "02";
            SalesModel.TranRecord.CfmDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.TaxprNm = TaxprNm;
            //JCNA 202001 DELETE SalesModel.TranRecord.CustNo = posModel.TranModel.TranNode.CustomerNode.CustomerCode; 
            SalesModel.TranRecord.CustTin = posModel.TranModel.TranNode.CustomerNode.Tin;
            SalesModel.TranRecord.CustNm = posModel.TranModel.TranNode.CustomerNode.CustomerName;
            SalesModel.TranRecord.CustBhfId = "00";
            // 20191213 JCNA 
            SalesModel.TranRecord.PrchrAcptcYn = "Y";

            // 20200120 JCNA : OrgInvoiceNo
            if (UIManager.Instance().PosModel.TranModel.TranNode.TranFlag == "R")
            {
                if (string.IsNullOrEmpty(posModel.TranModel.TranNode.RefundReasonNode.OrgInvoiceNo))
                {
                    SalesModel.TranRecord.OrgInvcNo = 0;
                }
                else
                {
                    SalesModel.TranRecord.OrgInvcNo = long.Parse(posModel.TranModel.TranNode.RefundReasonNode.OrgInvoiceNo);
                }
            }

            // 20200116 JCNA 
            for (int i = 0; i < posModel.TranModel.TranNode.TenderList.Count(); i++)
            {
                TenderNode tenderNode = posModel.TranModel.TranNode.TenderList.Get(i);
                if (tenderNode.TenderFlag.Equals("Cash"))
                {
                    if (i == 0)
                    {
                        SalesModel.TranRecord.PmtTyCd = "01";
                    }
                    else
                    {
                        if (SalesModel.TranRecord.PmtTyCd.Equals("02"))
                        {
                            SalesModel.TranRecord.PmtTyCd = "03";
                        }
                        else
                        {
                            SalesModel.TranRecord.PmtTyCd = "07";
                        }
                    }
                }
                else if (tenderNode.TenderFlag.Equals("Credit"))
                {
                    if (i == 0)
                    {
                        SalesModel.TranRecord.PmtTyCd = "02";
                    }
                    else
                    {
                        if (SalesModel.TranRecord.PmtTyCd.Equals("01"))
                        {
                            SalesModel.TranRecord.PmtTyCd = "03";
                        }
                        else
                        {
                            SalesModel.TranRecord.PmtTyCd = "07";
                        }
                    }
                }
                else if (tenderNode.TenderFlag.Equals("Debit"))
                {
                    if (i == 0)
                    {
                        SalesModel.TranRecord.PmtTyCd = "05";
                    }
                    else
                    {
                        SalesModel.TranRecord.PmtTyCd = "07";
                    }
                }
                else if (tenderNode.TenderFlag.Equals("Mobile Wallet"))
                {
                    if (i == 0)
                    {
                        SalesModel.TranRecord.PmtTyCd = "06";
                    }
                    else
                    {
                        SalesModel.TranRecord.PmtTyCd = "07";
                    }
                }
                else
                {
                    SalesModel.TranRecord.PmtTyCd = "07";
                }
            }

            for (int i = 0; i < posModel.TranModel.TranNode.ItemList.Count(); i++)
            {
                ItemNode itemNode = posModel.TranModel.TranNode.ItemList.Get(i);

                TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                bool ret = taxpayerItemMaster.ToRecord(itemRecord, Tin, itemNode.ItemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);

                SalesModel.SetCurrentItem(itemRecord);

                SalesModel.CurrentItemRecord.Prc = itemNode.Price;
                SalesModel.CurrentItemRecord.Qty = itemNode.Quantity;
                SalesModel.CurrentItemRecord.RegrId = UserId;
                SalesModel.CurrentItemRecord.RegrNm = UserNm;
                SalesModel.CurrentItemRecord.ModrId = UserId;
                SalesModel.CurrentItemRecord.ModrNm = UserNm;

                if (itemNode.IsrcAplcbYn.Equals("Y"))
                {
                    SalesModel.CurrentItemRecord.IsrccCd = posModel.TranModel.TranNode.InsurerNode.InsurerCode;
                    SalesModel.CurrentItemRecord.IsrccNm = posModel.TranModel.TranNode.InsurerNode.InsurerName;
                    SalesModel.CurrentItemRecord.IsrcRt = posModel.TranModel.TranNode.InsurerNode.InsurerRate;
                    SalesModel.CurrentItemRecord.IsrcAmt = itemNode.DiscountAmount;

                    SalesModel.CurrentItemRecord.DcRt = 0;
                }
                else
                {
                    SalesModel.CurrentItemRecord.IsrccCd = "";
                    SalesModel.CurrentItemRecord.IsrccNm = "";
                    SalesModel.CurrentItemRecord.IsrcRt = 0;
                    SalesModel.CurrentItemRecord.IsrcAmt = 0;
                    SalesModel.CurrentItemRecord.DcRt = itemNode.DiscountRate;
                }

                SalesModel.CalculateCurrentItem();
                SalesModel.ConfirmCurrentItem();
            }

            if (posModel.TranModel.TranNode.RefundReasonNode != null)
            {
                SalesModel.TranRecord.RefundReason = posModel.TranModel.TranNode.RefundReasonNode.ReasonCode;
                SalesModel.TranRecord.RefundReasonText = posModel.TranModel.TranNode.RefundReasonNode.ReasonText;
            }

          
            trnsSaleMaster.InsertTable(SalesModel.TranRecord);

            // JINIT_20191208,
            //trnsSaleItemMaster.InsertTable(SalesModel.ItemRecords);
            trnsSaleItemMaster.InsertTable(SalesModel.TranRecord, SalesModel.ItemRecords);

            //===>>>>>>>>>
            //JCNA 20191204
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);
             //DB INSERT
            TrnsSaleReceiptRecord trnsSaleReceiptRecord = new TrnsSaleReceiptRecord();
            trnsSaleReceiptRecord.SetTrnsSaleRecord(SalesModel.TranRecord);
            trnsSaleReceiptRecord.TaxprNm = SalesModel.TranRecord.TaxprNm;
            // 20191213 JCNA : 
            trnsSaleReceiptRecord.PrchrAcptcYn = "Y";

            trnsSaleReceiptRecord.RcptPbctDt = SalesModel.TranRecord.CfmDt; // DateTime.Now.ToString("yyyyMMdd"); 
            
            posModel.TranInformation.ReceiptNumber = (int)trnsSaleReceiptMaster.GetReceiptSeq();
            posModel.TranModel.TranInformation.ReceiptNumber = (int)trnsSaleReceiptMaster.GetReceiptSeq();
            trnsSaleReceiptRecord.CurRcptNo = trnsSaleReceiptMaster.GetReceiptSeq();
            trnsSaleReceiptRecord.TotRcptNo = trnsSaleReceiptMaster.GetTotReceiptSeq();
           
            trnsSaleReceiptRecord.Jrnl = posModel.Journal.SetJournal();
            // 20200309 JCNA
            //trnsSaleReceiptRecord.RptNo = posModel.TranModel.TranInformation.ReceiptNumber;
            // 20200720 JCNA
            //DateTime date = DateTime.Now;
            //trnsSaleReceiptRecord.RptNo = date.Year + (date.Month * 100) + date.Day; 
            trnsSaleReceiptRecord.RptNo = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount();

            //JCNA 202001 DELETE  trnsSaleReceiptRecord.SdcDt = posModel.TranModel.TranInformation.SaleDate;
            if (UIManager.Instance().PosModel.TranModel.TranNode.TranFlag == "R")
            {
                if(string.IsNullOrEmpty(posModel.TranModel.TranNode.RefundReasonNode.OrgInvoiceNo))
                {
                    trnsSaleReceiptRecord.OrgInvcNo = 0;
                }
                else
                {
                    trnsSaleReceiptRecord.OrgInvcNo = long.Parse(posModel.TranModel.TranNode.RefundReasonNode.OrgInvoiceNo);
                }
            }
            string GblKeySign = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeySign;
            string GblKeyInternal = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeyInternal;
            trnsSaleReceiptRecord.RcptSign = posModel.TranModel.TranNode.RcptSign;
            trnsSaleReceiptRecord.IntrlData = posModel.TranModel.TranNode.IntrlData;

            trnsSaleReceiptRecord.RegrId = UserId;
            trnsSaleReceiptRecord.RegrNm = UserNm;
            trnsSaleReceiptRecord.ModrId = UserId;
            trnsSaleReceiptRecord.ModrNm = UserNm;

            trnsSaleReceiptMaster.InsertTable(trnsSaleReceiptRecord);

            //JCNA 20191204
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            trnsSaleRcptRraSdcUpload.SendTranSalesRcptSave(trnsSaleReceiptRecord.Tin, trnsSaleReceiptRecord.BhfId, trnsSaleReceiptRecord.InvcNo);

            long SarNo = stockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "M"; 
            TransactionStockInOutModel StockInOutModel = new TransactionStockInOutModel();
            StockInOutModel.CurrentItemRecord = null;
            StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);

            //StockInOutModel.TranRecord
            StockInOutModel.TranRecord.SarTyCd = "11"; 
            // JINIT_20191201,
            if(UIManager.Instance().PosModel.TranModel.TranNode.TranFlag == "R")
                StockInOutModel.TranRecord.SarTyCd = "02";     
            StockInOutModel.TranRecord.TaxprNm = SalesModel.TranRecord.TaxprNm;
            StockInOutModel.TranRecord.CustTin = SalesModel.TranRecord.CustTin;
            StockInOutModel.TranRecord.CustNm = SalesModel.TranRecord.CustNm;
            StockInOutModel.TranRecord.CustBhfId = SalesModel.TranRecord.CustBhfId;
            StockInOutModel.TranRecord.RegrId = UserId;
            StockInOutModel.TranRecord.RegrNm = UserNm;
            StockInOutModel.TranRecord.ModrId = UserId;
            StockInOutModel.TranRecord.ModrNm = UserNm;
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
                StockInOutModel.CurrentItemRecord.RegrId = UserId;
                StockInOutModel.CurrentItemRecord.RegrNm = UserNm;
                StockInOutModel.CurrentItemRecord.ModrId = UserId;
                StockInOutModel.CurrentItemRecord.ModrNm = UserNm;

                StockInOutModel.CalculateCurrentItem();
                StockInOutModel.ConfirmCurrentItem();
            }

           
            StockInOutModel.TranRecord.SarNo = stockIoMaster.GetStockIoSeq(); // JINIT_20191201 
            stockIoMaster.InsertTable(StockInOutModel.TranRecord);
            // stockIoItemMaster
            // JINIT_20191201
            //stockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords);
            //stockIoItemMaster.InsertTable(-1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

            // JINIT_20191201, 
            int sign = -1;
            if (UIManager.Instance().PosModel.TranModel.TranNode.TranFlag == "R")
                sign = 1; 
            stockIoItemMaster.InsertTable(sign, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);

            //JCNA 20191204
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModel.TranRecord.Tin, StockInOutModel.TranRecord.BhfId, StockInOutModel.TranRecord.SarNo);
        }
        public static TransactionSalesModel GetSalesModel(PosModel posModel)
        {
            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string TaxprNm = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNm;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            //JCNA 202001 DELETE string DvcId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblDvcId;
            long InvcNo = trnsSaleMaster.GetSalesSeq();
            string SalesDt = DateTime.Now.ToString("yyyyMMdd");
            string SalesTyCd = "N";

            // JINIT_20191201, 
            //string RcptTyCd = "S";             // S : Sale, R : Refund  
            string RcptTyCd = UIManager.Instance().PosModel.TranModel.TranNode.TranFlag;

            // JCNA 20191211
            if (RcptTyCd.Equals("N")) RcptTyCd = "S";
            else if (RcptTyCd.Equals("R")) RcptTyCd = "R";
            else RcptTyCd = "S";

            /* JINIT_20191201, 
            string UserId = posModel.TranModel.TranInformation.UserCode;
            string UserNm = posModel.TranModel.TranInformation.UserName;
            */
            string UserId = posModel.RegiTotal.RegiHeader.UserID;
            string UserNm = posModel.RegiTotal.RegiHeader.UserName;

            TransactionSalesModel SalesModel = new TransactionSalesModel();
            SalesModel.InitModel(Tin, BhfId, InvcNo, SalesDt, SalesTyCd, RcptTyCd, UserId, UserNm);

            SalesModel.TranRecord.SalesSttsCd = "02";
            SalesModel.TranRecord.CfmDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.TaxprNm = TaxprNm;
            //JCNA 202001 DELETESalesModel.TranRecord.CustNo = posModel.TranModel.TranNode.CustomerNode.CustomerCode;
            SalesModel.TranRecord.CustTin = posModel.TranModel.TranNode.CustomerNode.Tin;
            SalesModel.TranRecord.CustNm = posModel.TranModel.TranNode.CustomerNode.CustomerName;
            SalesModel.TranRecord.CustBhfId = "00";
            // 20191213 JCNA 
            SalesModel.TranRecord.PrchrAcptcYn = "Y";

            for (int i = 0; i < posModel.TranModel.TranNode.ItemList.Count(); i++)
            {
                ItemNode itemNode = posModel.TranModel.TranNode.ItemList.Get(i);

                TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                bool ret = taxpayerItemMaster.ToRecord(itemRecord, Tin, itemNode.ItemCode, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);

                SalesModel.SetCurrentItem(itemRecord);

                SalesModel.CurrentItemRecord.Prc = itemNode.Price;
                SalesModel.CurrentItemRecord.Qty = itemNode.Quantity;
                SalesModel.CurrentItemRecord.RegrId = UserId;
                SalesModel.CurrentItemRecord.RegrNm = UserNm;
                SalesModel.CurrentItemRecord.ModrId = UserId;
                SalesModel.CurrentItemRecord.ModrNm = UserNm;

                if (itemNode.IsrcAplcbYn.Equals("Y"))
                {
                    SalesModel.CurrentItemRecord.IsrccCd = posModel.TranModel.TranNode.InsurerNode.InsurerCode;
                    SalesModel.CurrentItemRecord.IsrccNm = posModel.TranModel.TranNode.InsurerNode.InsurerName;
                    SalesModel.CurrentItemRecord.IsrcRt = posModel.TranModel.TranNode.InsurerNode.InsurerRate;
                    SalesModel.CurrentItemRecord.IsrcAmt = itemNode.DiscountAmount;

                    SalesModel.CurrentItemRecord.DcRt = 0;
                }
                else
                {
                    SalesModel.CurrentItemRecord.IsrccCd = "";
                    SalesModel.CurrentItemRecord.IsrccNm = "";
                    SalesModel.CurrentItemRecord.IsrcRt = 0;
                    SalesModel.CurrentItemRecord.IsrcAmt = 0;
                    SalesModel.CurrentItemRecord.DcRt = itemNode.DiscountRate;
                }

                SalesModel.CalculateCurrentItem();
                SalesModel.ConfirmCurrentItem();
            }

            return SalesModel;
        }
    }
}

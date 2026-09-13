using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models;
using EBM2x.Modules;
using EBM2x.Process.eot;
using EBM2x.UI;
using System;

namespace EBM2x.Process.tran
{
    public class TransactionEndProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if(posModel.TranModel.TranNode.ItemList.Count() == 0)
            {
               
                return StateModel.OP_RETRY;
            }

            // JCNA 20191204
            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
            TrnsSaleReceiptMaster trnsSaleReceiptMaster = new TrnsSaleReceiptMaster();
            posModel.TranInformation.InvoiceN0 = trnsSaleMaster.GetSalesSeq();

            string GblKeySign = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeySign;
            string GblKeyInternal = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblKeyInternal;

            ReceiptSignature receiptSignature = new ReceiptSignature();
            receiptSignature.RcptDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            receiptSignature.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            if(posModel.TranModel.TranNode.CustomerNode != null && !string.IsNullOrEmpty(posModel.TranModel.TranNode.CustomerNode.CustomerName))
            {
                if(!string.IsNullOrEmpty(posModel.TranModel.TranNode.CustomerNode.CustomerCode) && posModel.TranModel.TranNode.CustomerNode.CustomerCode.Length == 9)
                {
                    receiptSignature.CustTin = posModel.TranModel.TranNode.CustomerNode.CustomerCode;
                }
                else
                {
                    receiptSignature.CustTin = "";
                }
            }
            else
            {
                receiptSignature.CustTin = "";
            }
            receiptSignature.GblMrcSysCod = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblMrcSysCod;

            TransactionSalesModel SalesModel = TranEndOfTransaction.GetSalesModel(posModel);
            receiptSignature.DblSpcpcA = SalesModel.TranRecord.TaxblAmtA;
            receiptSignature.DblVatA = SalesModel.TranRecord.TaxAmtA;
            receiptSignature.DblSpcpcB = SalesModel.TranRecord.TaxblAmtB;
            receiptSignature.DblVatB = SalesModel.TranRecord.TaxAmtB;
            receiptSignature.DblSpcpcC = SalesModel.TranRecord.TaxblAmtC;
            receiptSignature.DblVatC = SalesModel.TranRecord.TaxAmtC;
            receiptSignature.GblSdcSysNum = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblSdcSysNum;
            if (posModel.TranModel.TranNode.TranFlag == "R")
            {
                receiptSignature.TranRcptTyCD = "NR";
            }
            else
            {
                receiptSignature.TranRcptTyCD = "NS";
            }
            posModel.TranInformation.ReceiptNumber = (int)trnsSaleReceiptMaster.GetReceiptSeq();
            posModel.TranModel.TranInformation.ReceiptNumber = (int)trnsSaleReceiptMaster.GetReceiptSeq();
            receiptSignature.CurReceiptID = trnsSaleReceiptMaster.GetReceiptSeq();
            receiptSignature.TotReceiptID = trnsSaleReceiptMaster.GetTotReceiptSeq();
            //receiptSignature.CurReceiptID = posModel.TranModel.TranInformation.ReceiptNumber;
            //receiptSignature.TotReceiptID = posModel.TranModel.TranInformation.ReceiptNumber;
            posModel.TranModel.TranNode.RcptSign = (string)Base32.Receipt_signature(trnsSaleReceiptMaster.GetReceiptSignature(receiptSignature), GblKeySign);

            SalesReportMaster salesReportMaster = new SalesReportMaster();
            long normalSales = (long)salesReportMaster.GetCodeValue("20150101", DateTime.Now.ToString("yyyyMMdd"), "N", "S", "TRNS_SALE.TOT_AMT");
            long returnSales = (long)salesReportMaster.GetCodeValue("20150101", DateTime.Now.ToString("yyyyMMdd"), "N", "R", "TRNS_SALE.TOT_AMT");
            if (posModel.TranModel.TranNode.TranFlag == "R")
            {
                returnSales += (long)posModel.TranModel.TranNode.Subtotal;
            }
            else
            {
                normalSales += (long)posModel.TranModel.TranNode.Subtotal;
            }
            string strOcdt = DateTime.Now.ToString("yyyyMMdd");
            long receipt = posModel.TranModel.TranInformation.ReceiptNumber;
            posModel.TranModel.TranNode.IntrlData = (string)Base32.Internal_data(trnsSaleReceiptMaster.GetInternalData(normalSales, returnSales, strOcdt, receipt), GblKeySign);

            bool PrintFlag = posModel.Journal.PrintFlag;
            posModel.Journal.Clear();

            Journal.tran.TranNodeJournal journalCreate = new Journal.tran.TranNodeJournal();

            journalCreate.create(posModel);
            posModel.Journal.PrintFlag = PrintFlag;

            TranEndOfTransaction.excuteProcess(posModel, inputModel, informationModel);

            return StateModel.OP_NEXT;
        }
    }
}

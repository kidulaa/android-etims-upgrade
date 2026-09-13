using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TrnsSaleRecord.
    /// </summary>
    public class TrnsSaleRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public long InvcNo { get; set; }              // Invoice No.
        public string PrchrAcptcYn { get; set; }        // Purchaser Accepted(Y/N)
        public long OrgInvcNo { get; set; }           // Original Invoice No.
        public string TaxprNm { get; set; }             // Taxpayer Name
        //JCNA 202001 DELETE
        //public string CustNo { get; set; }              // Customer No.
        public string CustTin { get; set; }             // Customer Taxpayer Identification Number(TIN)
        public string CustBhfId { get; set; }           // Customer Branch Office ID
        public string CustNm { get; set; }              // Customer Name
        //JCNA 202001 DELETE
        //public string DvcId { get; set; }               // Device ID
        public string SalesTyCd { get; set; }           // Sale Type Code
        public string RcptTyCd { get; set; }            // Receipt Type Code
        public string PmtTyCd { get; set; }             // Payment Type Code
        public string SalesSttsCd { get; set; }         // Sale Status Code
        public string CfmDt { get; set; }               // Confirmed Date
        public string SalesDt { get; set; }             // Sale Date
        public string StockRlsDt { get; set; }          // Stock Released Date
        public string CnclReqDt { get; set; }           // Cancel Reqeuested Date
        public string CnclDt { get; set; }              // Canceled Date
        public string RfdDt { get; set; }               // Refunded Date
        public int TotItemCnt { get; set; }             // Total Item Count
        public double TaxblAmtA { get; set; }           // Taxable Amount A
        public double TaxblAmtB { get; set; }           // Taxable Amount B
        public double TaxblAmtC { get; set; }           // Taxable Amount C
        public double TaxblAmtD { get; set; }          // Taxable Amount D
        public double TaxblAmtE { get; set; }          // Taxable Amount E
        public int TaxRtA { get; set; }              // Tax Rate A
        public int TaxRtB { get; set; }              // Tax Rate B
        public int TaxRtC { get; set; }              // Tax Rate C
        public int TaxRtD { get; set; }              // Tax Rate D
        public int TaxRtE { get; set; }              // Tax Rate E
        public double TaxAmtA { get; set; }             // Tax Amount A
        public double TaxAmtB { get; set; }             // Tax Amount B
        public double TaxAmtC { get; set; }             // Tax Amount C
        public double TaxAmtD { get; set; }             // Tax Amount D
        public double TaxAmtE { get; set; }             // Tax Amount E
        public double TotTaxblAmt { get; set; }         // Total Taxable Amount
        public double TotTaxAmt { get; set; }           // Total Tax Amount
        public double TotAmt { get; set; }              // Total Amount
        public string Remark { get; set; }              // Remark
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
        public string SalesSttsNm { get; set; }         // Sale Status Name
        public string TradeNm { get; set; }             // Trade Name
        public string RefundReason { get; set; }       // RefundReason
        public string RefundReasonText { get; set; }       // RefundReasonText

        public TrnsSaleRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.InvcNo = 0;                 // Invoice No.
            this.PrchrAcptcYn = string.Empty;           // Purchaser Accepted(Y/N)
            this.OrgInvcNo = 0;              // Original Invoice No.
            this.TaxprNm = string.Empty;                // Taxpayer Name
            //JCNA 202001 DELETE this.CustNo = string.Empty;                 // Customer No.
            this.CustTin = string.Empty;                // Customer Taxpayer Identification Number(TIN)
            this.CustBhfId = string.Empty;              // Customer Branch Office ID
            this.CustNm = string.Empty;                 // Customer Name
            //JCNA 202001 DELETE this.DvcId = string.Empty;                  // Device ID
            this.SalesTyCd = string.Empty;              // Sale Type Code
            this.RcptTyCd = string.Empty;               // Receipt Type Code
            this.PmtTyCd = string.Empty;                // Payment Type Code
            this.SalesSttsCd = string.Empty;            // Sale Status Code
            this.CfmDt = string.Empty;                  // Confirmed Date
            this.SalesDt = string.Empty;                // Sale Date
            this.StockRlsDt = string.Empty;             // Stock Released Date
            this.CnclReqDt = string.Empty;              // Cancel Reqeuested Date
            this.CnclDt = string.Empty;                 // Canceled Date
            this.RfdDt = string.Empty;                  // Refunded Date
            this.TotItemCnt = 0;                        // Total Item Count
            this.TaxblAmtA = 0;                         // Taxable Amount A
            this.TaxblAmtB = 0;                         // Taxable Amount B
            this.TaxblAmtC = 0;                         // Taxable Amount C
            this.TaxblAmtD = 0;                         // Taxable Amount D
            this.TaxblAmtE = 0;                         // Taxable Amount E
            this.TaxRtA = 0;                            // Tax Rate A
            this.TaxRtB = 0;                            // Tax Rate B
            this.TaxRtC = 0;                            // Tax Rate C
            this.TaxRtD = 0;                            // Tax Rate D
            this.TaxRtE = 0;                            // Tax Rate E
            this.TaxAmtA = 0;                           // Tax Amount A
            this.TaxAmtB = 0;                           // Tax Amount B
            this.TaxAmtC = 0;                           // Tax Amount C
            this.TaxAmtD = 0;                           // Tax Amount D
            this.TaxAmtE = 0;                           // Tax Amount E
            this.TotTaxblAmt = 0;                       // Total Taxable Amount
            this.TotTaxAmt = 0;                         // Total Tax Amount
            this.TotAmt = 0;                            // Total Amount
            this.Remark = string.Empty;                 // Remark
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date

            // JINIT_20191201
            this.SalesSttsNm = string.Empty;
            this.TradeNm = string.Empty;

            this.RefundReason = "";
            this.RefundReasonText = "";
        }
    }
}

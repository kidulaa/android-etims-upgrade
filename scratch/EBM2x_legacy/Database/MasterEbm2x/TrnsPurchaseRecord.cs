using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TrnsPurchaseRecord.
    /// </summary>
    public class TrnsPurchaseRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string SpplrTin { get; set; }            // Cupplier Taxpayer Identification Number(TIN)
        public long InvcNo { get; set; }              // Invoice No.
        public long OrgInvcNo { get; set; }           // Original Invoice No.
        public string TaxprNm { get; set; }             // Taxpayer's Name
        //JCNA 202001 DELETE
        //public string DvcId { get; set; }               // Device ID
        public string SpplrBhfId { get; set; }          // Supplier Branch Offie ID
        public string SpplrNm { get; set; }             // Customer Name
        //JCNA 202001 DELETE
        //public string SpplrDvcId { get; set; }          // Supplier Device ID
        public long SpplrInvcNo { get; set; }           // Supplier Receipt No.
        public string RegTyCd { get; set; }             // Registration Type Code
        public string PchsTyCd { get; set; }            // Purchase Type Code
        public string RcptTyCd { get; set; }            // Receipt Type Code
        public string PmtTyCd { get; set; }             // Payment Type Code
        public string PchsSttsCd { get; set; }          // Purchase Status Code
        public string CfmDt { get; set; }               // Confirmed Date
        public string PchsDt { get; set; }              // Purchased Date
        public string WrhsDt { get; set; }              // Warehousing Date
        public string CnclReqDt { get; set; }           // Cancel Requested Date
        public string CnclDt { get; set; }              // Canceled Date
        public string RfdDt { get; set; }               // Refunded Date
        public int TotItemCnt { get; set; }            // Total Item Count
        public double TaxblAmtA { get; set; }           // Taxable Amount A
        public double TaxblAmtB { get; set; }           // Taxable Amount B
        public double TaxblAmtC { get; set; }           // Taxable Amount C
        public double TaxblAmtD { get; set; }           // Taxable Amount D
        public double TaxblAmtE { get; set; }           // Taxable Amount E
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
        public string PchsSttsNm { get; set; }           // Status Name
        public string TradeNm { get; set; }             // Trade Name
        public int Rowid { get; set; }                 // Rowid

        public TrnsPurchaseRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;         // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;       // Branch Office ID
            this.SpplrTin = string.Empty;    // Cupplier Taxpayer Identification Number(TIN)
            this.InvcNo = 0;      // Invoice No.
            this.OrgInvcNo = 0;   // Original Invoice No.
            this.TaxprNm = string.Empty;     // Taxpayer's Name
            //JCNA 202001 DELETE this.DvcId = string.Empty;       // Device ID
            this.SpplrBhfId = string.Empty;  // Supplier Branch Offie ID
            this.SpplrNm = string.Empty;     // Customer Name
            //JCNA 202001 DELETE this.SpplrDvcId = string.Empty;  // Supplier Device ID
            this.SpplrInvcNo = 0;            // Supplier Receipt No.
            this.RegTyCd = string.Empty;     // Registration Type Code
            this.PchsTyCd = string.Empty;    // Purchase Type Code
            this.RcptTyCd = string.Empty;    // Receipt Type Code
            this.PmtTyCd = string.Empty;     // Payment Type Code
            this.PchsSttsCd = string.Empty;  // Purchase Status Code
            this.CfmDt = string.Empty;       // Confirmed Date
            this.PchsDt = string.Empty;      // Purchased Date
            this.WrhsDt = string.Empty;      // Warehousing Date
            this.CnclReqDt = string.Empty;   // Cancel Requested Date
            this.CnclDt = string.Empty;      // Canceled Date
            this.RfdDt = string.Empty;       // Refunded Date
            this.TotItemCnt = 0;             // Total Item Count
            this.TaxblAmtA = 0;              // Taxable Amount A
            this.TaxblAmtB = 0;              // Taxable Amount B
            this.TaxblAmtC = 0;              // Taxable Amount C
            this.TaxblAmtD = 0;              // Taxable Amount D
            this.TaxblAmtE = 0;              // Taxable Amount E
            this.TaxRtA = 0;                 // Tax Rate A
            this.TaxRtB = 0;                 // Tax Rate B
            this.TaxRtC = 0;                 // Tax Rate C
            this.TaxRtD = 0;                 // Tax Rate D
            this.TaxRtE = 0;                 // Tax Rate E
            this.TaxAmtA = 0;                // Tax Amount A
            this.TaxAmtB = 0;                // Tax Amount B
            this.TaxAmtC = 0;                // Tax Amount C
            this.TaxAmtD = 0;                // Tax Amount D
            this.TaxAmtE = 0;                // Tax Amount E
            this.TotTaxblAmt = 0;            // Total Taxable Amount
            this.TotTaxAmt = 0;              // Total Tax Amount
            this.TotAmt = 0;                 // Total Amount
            this.Remark = string.Empty;      // Remark
            this.RegrId = "system";      // Registrant ID
            this.RegrNm = "system";      // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");       // Registered Date
            this.ModrId = "system";      // Modifier ID
            this.ModrNm = "system";      // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");       // Modified Date
        }
    }
}

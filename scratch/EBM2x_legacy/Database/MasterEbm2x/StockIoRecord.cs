using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of StockIoRecord.
    /// </summary>
    public class StockIoRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public long SarNo { get; set; }               // Stored and Released No.
        public long OrgSarNo { get; set; }            // Original Stored and Released No.
        public string RegTyCd { get; set; }             // Registration Type Code
        public string TaxprNm { get; set; }             // Taxpayer's Name
        public string CustTin { get; set; }             // Customer Taxpayer Identification Number(TIN)
        public string CustBhfId { get; set; }           // Customer Branch ID
        public string CustNm { get; set; }              // Customer Name
        //JCNA 202001 DELETE
        //public long InvcNo { get; set; }              // Onvoice No.
        public string SarTyCd { get; set; }             // Stored and Released Type Code
        //JCNA 202001 DELETE
        //public string SarRsnCd { get; set; }            // Stored and Released Reason Code
        public string OcrnDt { get; set; }              // Occurred Date time
        public int TotItemCnt { get; set; }            // Total Item Count
        public double TotTaxblAmt { get; set; }         // Total Taxable Amount
        public double TotTaxAmt { get; set; }           // Total Tax Amount
        public double TotAmt { get; set; }              // Total Amount
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
        public string Remark { get; set; }              // Remark
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public StockIoRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.SarNo = 0;                             // Stored and Released No.
            this.OrgSarNo = 0;                          // Original Stored and Released No.
            this.RegTyCd = string.Empty;                // Registration Type Code
            this.TaxprNm = string.Empty;                // Taxpayer's Name
            this.CustTin = string.Empty;                // Customer Taxpayer Identification Number(TIN)
            this.CustBhfId = string.Empty;              // Customer Branch ID
            this.CustNm = string.Empty;                 // Customer Name
            //JCNA 202001 DELETE this.InvcNo = 0;                            // Onvoice No.
            this.SarTyCd = string.Empty;                // Stored and Released Type Code
            //JCNA 202001 DELETE this.SarRsnCd = string.Empty;               // Stored and Released Reason Code
            this.OcrnDt = string.Empty;                 // Occurred Date time
            this.TotItemCnt = 0;                        // Total Item Count
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
        }
        public void UpdateNull()
        {
            if(string.IsNullOrEmpty(this.Tin)) this.Tin = "";                    // Taxpayer Identification Number(TIN)
            if (string.IsNullOrEmpty(this.BhfId)) this.BhfId = "";                  // Branch Office ID
            //this.SarNo = 0;                             // Stored and Released No.
            //this.OrgSarNo = 0;                          // Original Stored and Released No.
            if (string.IsNullOrEmpty(this.RegTyCd)) this.RegTyCd = "";                // Registration Type Code
            if (string.IsNullOrEmpty(this.TaxprNm)) this.TaxprNm = "";                // Taxpayer's Name
            if (string.IsNullOrEmpty(this.CustTin)) this.CustTin = "";                // Customer Taxpayer Identification Number(TIN)
            if (string.IsNullOrEmpty(this.CustBhfId)) this.CustBhfId = "";              // Customer Branch ID
            if (string.IsNullOrEmpty(this.CustNm)) this.CustNm = "";                 // Customer Name
            //this.InvcNo = 0;                            // Onvoice No.
            if (string.IsNullOrEmpty(this.SarTyCd)) this.SarTyCd = "";                // Stored and Released Type Code
            //JCNA 202001 DELETE
            //if (string.IsNullOrEmpty(this.SarRsnCd)) this.SarRsnCd = "";               // Stored and Released Reason Code
            if (string.IsNullOrEmpty(this.OcrnDt)) this.OcrnDt = string.Empty;                 // Occurred Date time
            //this.TotItemCnt = 0;                        // Total Item Count
            //this.TotTaxblAmt = 0;                       // Total Taxable Amount
            //this.TotTaxAmt = 0;                         // Total Tax Amount
            //this.TotAmt = 0;                            // Total Amount
            if (string.IsNullOrEmpty(this.Remark)) this.Remark = "";                 // Remark
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

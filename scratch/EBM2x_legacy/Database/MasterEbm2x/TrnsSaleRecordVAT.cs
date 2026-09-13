using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TrnsSaleRecordVAT.
    /// </summary>
    public class TrnsSaleRecordVAT
    {
        public string CustTin { get; set; }             // Customer Taxpayer Identification Number(TIN)
        public string CustNm { get; set; }              // Customer Name
        public long InvcNo { get; set; }              // Invoice No.
        public string RcptPbctDt { get; set; }            // Receipt Type Code
        public double TotAmt { get; set; }              // Total Amount
        public double TaxblAmtA { get; set; }           // Taxable Amount A
        public double TaxblAmtB { get; set; }           // Taxable Amount B
        public double TaxblAmtC { get; set; }           // Taxable Amount C
        public double TaxblAmtE { get; set; }           // Taxable Amount E
        public double TotTaxAmt { get; set; }           // Total Tax Amount

        public TrnsSaleRecordVAT()
        {
            clear();
        }

        public void clear()
        {
            this.CustTin = string.Empty;                // Customer Taxpayer Identification Number(TIN)
            this.CustNm = string.Empty;                 // Customer Name
            this.InvcNo = 0;                            // Invoice No.
            this.RcptPbctDt = string.Empty;             // RcptPbctDt
            this.TotAmt = 0;                            // Total Amount
            this.TaxblAmtA = 0;                         // Taxable Amount A
            this.TaxblAmtB = 0;                         // Taxable Amount B
            this.TaxblAmtC = 0;                         // Taxable Amount C
            this.TaxblAmtE = 0;                         // Taxable Amount E
            this.TotTaxAmt = 0;                         // Total Tax Amount
        }
    }
}

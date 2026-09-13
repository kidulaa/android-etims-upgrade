using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TrnsPurchaseRecordVAT.
    /// </summary>
    public class TrnsPurchaseRecordVAT
    {
        public long InvcNo { get; set; }                // Invoice No.
        public string SpplrTin { get; set; }            // Cupplier Taxpayer Identification Number(TIN)
        public string SpplrNm { get; set; }             // Customer Name
        public long SpplrInvcNo { get; set; }           // SpplrInvcNo
        public string PchsDt { get; set; }              // Purchased Date
        public string RcptPbctDt { get; set; }          // RcptPbctDt Date
        public double TotAmt { get; set; }              // Total Amount
        public double TotTaxAmt { get; set; }           // Total Tax Amount

        public TrnsPurchaseRecordVAT()
        {
            clear();
        }

        public void clear()
        {
            this.SpplrTin = string.Empty;    // Cupplier Taxpayer Identification Number(TIN)
            this.InvcNo = 0;                 // Invoice No.
            this.SpplrNm = string.Empty;     // Customer Name
            this.SpplrInvcNo = 0;            // SpplrInvcNo
            this.PchsDt = string.Empty;      // Purchased Date
            this.RcptPbctDt = string.Empty;  // RcptPbctDt
            this.TotTaxAmt = 0;              // Total Tax Amount
            this.TotAmt = 0;                 // Total Amount
        }
    }
}

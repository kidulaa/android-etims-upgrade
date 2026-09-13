using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of ImportItemRecord.
    /// </summary>
    public class ImportItemRecordVAT
    {
        public string DclTaxofcCd { get; set; }         // Declaration Tax Office Code
        public string DclNo { get; set; }               // Declaration Number
        public string DclDe { get; set; }               // Declaration Date
        public string ItemNm { get; set; }              // ItemName
        public string OrgnNatCd { get; set; }           // Country Code of Origin
        public double TrffAmt { get; set; }             // Tariff Amount
        public double VatAmt { get; set; }              // VAT

        public ImportItemRecordVAT()
        {
            clear();
        }

        public void clear()
        {
            this.DclDe = string.Empty;                  // Declaration Date
            this.DclNo = string.Empty;                  // Declaration Number
            this.ItemNm = string.Empty;                 // ItemName
            this.OrgnNatCd = string.Empty;              // Country Code of Origin
            this.DclTaxofcCd = string.Empty;            // Declaration Tax Office Code
            this.TrffAmt = 0;                           // Tariff Amount
            this.VatAmt = 0;                            // VAT
        }
    }
}

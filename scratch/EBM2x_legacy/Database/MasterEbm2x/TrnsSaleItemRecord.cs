using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TrnsSaleItemRecord.
    /// </summary>
    public class TrnsSaleItemRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public long InvcNo { get; set; }              // Invoice No.
        public int ItemSeq { get; set; }                // Item Sequence
        public string ItemCd { get; set; }              // Item Code
        public string IsrccCd { get; set; }             // Insurance Company Code
        public string ItemClsCd { get; set; }           // Item Classification Code
        public string ItemNm { get; set; }              // Item Name
        public string Bcd { get; set; }                 // Barcode
        public string PkgUnitCd { get; set; }           // Package Unit Code
        public double Pkg { get; set; }                 // Package
        public string QtyUnitCd { get; set; }           // Quantity Unit Code
        public double Qty { get; set; }                 // Quantity
        public double Prc { get; set; }                 // Unit Price
        public double SplyAmt { get; set; }             // Supply Price
        public int DcRt { get; set; }                   // Discount Rate
        public double DcAmt { get; set; }               // Discount Amount
        public string IsrccNm { get; set; }             // Insurance Company Name
        public int IsrcRt { get; set; }                 // Insurance Rate
        public double IsrcAmt { get; set; }             // Insurance Amount
        public string TaxTyCd { get; set; }             // Tax type code
        public double TaxblAmt { get; set; }            // Taxable Amount
        public double TaxAmt { get; set; }              // Tax Amount
        public double TotAmt { get; set; }              // Total Amount
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
        public string TaxTyNm { get; set; }             // Tax type Name
        public string ItemClsNm { get; set; }           // Item Classification Name

        public TrnsSaleItemRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.InvcNo = 0;                 // Invoice No.
            this.ItemSeq = 0;                           // Item Sequence
            this.ItemCd = string.Empty;                 // Item Code
            this.IsrccCd = string.Empty;                // Insurance Company Code
            this.ItemClsCd = string.Empty;              // Item Classification Code
            this.ItemNm = string.Empty;                 // Item Name
            this.Bcd = string.Empty;                    // Barcode
            this.PkgUnitCd = string.Empty;              // Package Unit Code
            this.Pkg = 0;                               // Package
            this.QtyUnitCd = string.Empty;              // Quantity Unit Code
            this.Qty = 0;                               // Quantity
            this.Prc = 0;                               // Unit Price
            this.SplyAmt = 0;                           // Supply Price
            this.DcRt = 0;                              // Discount Rate
            this.DcAmt = 0;                             // Discount Amount
            this.IsrccNm = string.Empty;                // Insurance Company Name
            this.IsrcRt = 0;                            // Insurance Rate
            this.IsrcAmt = 0;                           // Insurance Amount
            this.TaxTyCd = string.Empty;                // Tax type code
            this.TaxblAmt = 0;                          // Taxable Amount
            this.TaxAmt = 0;                            // Tax Amount
            this.TotAmt = 0;                            // Total Amount
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

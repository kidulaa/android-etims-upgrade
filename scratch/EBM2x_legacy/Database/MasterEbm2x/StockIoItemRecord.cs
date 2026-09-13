using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of StockIoItemRecord.
    /// </summary>
    public class StockIoItemRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public long SarNo { get; set; }               // Stored and Released No.
        public int ItemSeq { get; set; }                // Item Sequence
        public string ItemCd { get; set; }              // Item Code
        public string ItemClsCd { get; set; }           // Item Classification Code
        public string ItemNm { get; set; }              // Item Name
        public string Bcd { get; set; }                 // Barcode
        public string PkgUnitCd { get; set; }           // Package unit code
        public double Pkg { get; set; }                 // Package
        public string QtyUnitCd { get; set; }           // Quantity Unit Code
        public double Qty { get; set; }                 // Quantity
        public string ItemExprDt { get; set; }          // Item Expiration Date
        public double Prc { get; set; }                 // Price
        public double SplyAmt { get; set; }             // Supply Amount
        public double TotDcAmt { get; set; }            // Total Discount Amount
        public double TaxblAmt { get; set; }            // Taxable Amount
        public string TaxTyCd { get; set; }             // Taxation Type Code
        public double TaxAmt { get; set; }              // Tax Amount
        public double TotAmt { get; set; }              // Total Amount
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
        public string ItemClsNm { get; set; }           // Item Classification Name       
        public double RdsQty { get; set; }                // 현재고 수량
        public double AfterQty { get; set; }              // 잔여 수량

        public StockIoItemRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.SarNo = 0;                             // Stored and Released No.
            this.ItemSeq = 0;                           // Item Sequence
            this.ItemCd = string.Empty;                 // Item Code
            this.ItemClsCd = string.Empty;              // Item Classification Code
            this.ItemNm = string.Empty;                 // Item Name
            this.Bcd = string.Empty;                    // Barcode
            this.PkgUnitCd = string.Empty;              // Package unit code
            this.Pkg = 0;                               // Package
            this.QtyUnitCd = string.Empty;              // Quantity Unit Code
            this.Qty = 0;                               // Quantity
            this.ItemExprDt = string.Empty;             // Item Expiration Date
            this.Prc = 0;                               // Price
            this.SplyAmt = 0;                           // Supply Amount
            this.TotDcAmt = 0;                          // Total Discount Amount
            this.TaxblAmt = 0;                          // Taxable Amount
            this.TaxTyCd = string.Empty;                // Taxation Type Code
            this.TaxAmt = 0;                            // Tax Amount
            this.TotAmt = 0;                            // Total Amount
            this.RegrId = string.Empty;                 // Registrant ID
            this.RegrNm = string.Empty;                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = string.Empty;                 // Modifier ID
            this.ModrNm = string.Empty;                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
        public void UpdateNull()
        {
            if (string.IsNullOrEmpty(this.Tin)) this.Tin = "";                    // Taxpayer Identification Number(TIN)
            if (string.IsNullOrEmpty(this.BhfId)) this.BhfId = "";                  // Branch Office ID
            //this.SarNo = 0;                             // Stored and Released No.
            //this.ItemSeq = 0;                           // Item Sequence
            if (string.IsNullOrEmpty(this.ItemCd)) this.ItemCd = "";                 // Item Code
            if (string.IsNullOrEmpty(this.ItemClsCd)) this.ItemClsCd = "";              // Item Classification Code
            if (string.IsNullOrEmpty(this.ItemNm)) this.ItemNm = "";                 // Item Name
            if (string.IsNullOrEmpty(this.Bcd)) this.Bcd = "";                    // Barcode
            if (string.IsNullOrEmpty(this.PkgUnitCd)) this.PkgUnitCd = "";              // Package unit code
            //this.Pkg = 0;                               // Package
            if (string.IsNullOrEmpty(this.QtyUnitCd)) this.QtyUnitCd = "";              // Quantity Unit Code
            //this.Qty = 0;                               // Quantity
            if (string.IsNullOrEmpty(this.ItemExprDt)) this.ItemExprDt = "";             // Item Expiration Date
            //this.Prc = 0;                               // Price
            //this.SplyAmt = 0;                           // Supply Amount
            //this.TotDcAmt = 0;                          // Total Discount Amount
            //this.TaxblAmt = 0;                          // Taxable Amount
            if (string.IsNullOrEmpty(this.TaxTyCd)) this.TaxTyCd = "";                // Taxation Type Code
            //this.TaxAmt = 0;                            // Tax Amount
            //this.TotAmt = 0;                            // Total Amount
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

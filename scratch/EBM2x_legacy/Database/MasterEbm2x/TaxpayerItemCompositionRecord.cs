using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerItemCompositionRecord.
    /// </summary>
    public class TaxpayerItemCompositionRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string ItemCd { get; set; }              // Item Code
        public string ItemNm { get; set; }              // Item Code
        public string CpstItemCd { get; set; }          // Composition Item Code
        public string CpstItemNm { get; set; }          // Composition Item Code
        public string CpstItemClsCd { get; set; }          // Composition Item Class Code
        public string CpstItemClsNm { get; set; }          // Composition Item Class Code
        public double CpstQty { get; set; }             // Composition Quantity
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public double RdsQty { get; set; } 

        public TaxpayerItemCompositionRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.ItemCd = string.Empty;                 // Item Code
            this.ItemNm = string.Empty;                 // Item Code
            this.CpstItemCd = string.Empty;             // Composition Item Code
            this.CpstItemNm = string.Empty;             // Composition Item Code
            this.CpstItemClsCd = string.Empty;                 // Item Code
            this.CpstItemClsNm = string.Empty;                 // Item Code
            this.CpstQty = 0;                           // Composition Quantity
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date

            this.RdsQty = 0;
        }
        public void UpdateNull()
        {
            if (string.IsNullOrEmpty(this.Tin)) this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            if (string.IsNullOrEmpty(this.ItemCd)) this.ItemCd = string.Empty;                 // Item Code
            if (string.IsNullOrEmpty(this.ItemNm)) this.ItemNm = string.Empty;                 // Item Code
            if (string.IsNullOrEmpty(this.CpstItemCd)) this.CpstItemCd = string.Empty;             // Composition Item Code
            if (string.IsNullOrEmpty(this.CpstItemNm)) this.CpstItemNm = string.Empty;             // Composition Item Code
            if (string.IsNullOrEmpty(this.CpstItemClsCd)) this.CpstItemClsCd = string.Empty;                 // Item Code
            if (string.IsNullOrEmpty(this.CpstItemClsNm)) this.CpstItemClsNm = string.Empty;                 // Item Code
            //this.CpstQty = 0;                           // Composition Quantity
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date

            //this.RdsQty = 0;
        }
    }
}

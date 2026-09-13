using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of StockMasterRecord.
    /// </summary>
    public class StockMasterRecord
    {

        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string ItemCd { get; set; }              // Item Code
        public double RsdQty { get; set; }              // Resodual Quantity
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public StockMasterRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.ItemCd = string.Empty;                 // Item Code
            this.RsdQty = 0d;                           // Resodual Quantity
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
        public void UpdateNull()
        {
            if (string.IsNullOrEmpty(this.Tin)) this.Tin = "";                    // Taxpayer Identification Number(TIN)
            if (string.IsNullOrEmpty(this.BhfId)) this.BhfId = "";                  // Branch Office ID
            if (string.IsNullOrEmpty(this.ItemCd)) this.ItemCd = "";                 // Item Code
            //this.RsdQty = 0d;                           // Resodual Quantity
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

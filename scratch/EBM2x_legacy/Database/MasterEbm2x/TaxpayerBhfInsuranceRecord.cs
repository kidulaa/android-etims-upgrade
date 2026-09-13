using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerBhfInsuranceRecord.
    /// </summary>
    public class TaxpayerBhfInsuranceRecord
    {

        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string IssrccCd { get; set; }            // Insurance Company Code
        public string IsrccNm { get; set; }             // Insurance Company Name
        public int IsrcRt { get; set; }                 // Insurance Rate
        public string UseYn { get; set; }               // Use(Y/N)
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date

        public TaxpayerBhfInsuranceRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.IssrccCd = string.Empty;               // Insurance Company Code
            this.IsrccNm = string.Empty;                // Insurance Company Name
            this.IsrcRt = 0;                            // Insurance Rate
            this.UseYn = string.Empty;                  // Use(Y/N)
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
        }
    }
}

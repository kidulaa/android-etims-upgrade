using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerBhfRecord.
    /// </summary>
    public class TaxpayerBhfRecord
    {

        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string BhfNm { get; set; }               // Branch Name
        public string BhfSttsCd { get; set; }           // Branch Status Code
        public string PrvncNm { get; set; }                // Province No.
        public string DstrtNm { get; set; }             // District No.
        public string SctrNm { get; set; }              // Sector No.
        public string LocDesc { get; set; }             // Location Description
        public string MgrNm { get; set; }               // Manager Name
        public string MgrTelNo { get; set; }            // Manager Telephone number
        public string MgrEmail { get; set; }            // Manager Email
        public string HqYn { get; set; }                // Headquarter(Y/N)
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public TaxpayerBhfRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.BhfNm = string.Empty;                  // Branch Name
            this.BhfSttsCd = string.Empty;              // Branch Status Code
            this.PrvncNm = string.Empty;                // Province No.
            this.DstrtNm = string.Empty;                // District No.
            this.SctrNm = string.Empty;                 // Sector No.
            this.LocDesc = string.Empty;                // Location Description
            this.MgrNm = string.Empty;                  // Manager Name
            this.MgrTelNo = string.Empty;               // Manager Telephone number
            this.MgrEmail = string.Empty;               // Manager Email
            this.HqYn = string.Empty;                   // Headquarter(Y/N)
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
            if (string.IsNullOrEmpty(this.BhfNm)) this.BhfNm = "";                  // Branch Name
            if (string.IsNullOrEmpty(this.BhfSttsCd)) this.BhfSttsCd = "";              // Branch Status Code
            if (string.IsNullOrEmpty(this.PrvncNm)) this.PrvncNm = "";                // Province No.
            if (string.IsNullOrEmpty(this.DstrtNm)) this.DstrtNm = "";                // District No.
            if (string.IsNullOrEmpty(this.SctrNm)) this.SctrNm = "";                 // Sector No.
            if (string.IsNullOrEmpty(this.LocDesc)) this.LocDesc = "";                // Location Description
            if (string.IsNullOrEmpty(this.MgrNm)) this.MgrNm = "";                  // Manager Name
            if (string.IsNullOrEmpty(this.MgrTelNo)) this.MgrTelNo = "";               // Manager Telephone number
            if (string.IsNullOrEmpty(this.MgrEmail)) this.MgrEmail = "";               // Manager Email
            if (string.IsNullOrEmpty(this.HqYn)) this.HqYn = "";                   // Headquarter(Y/N)
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

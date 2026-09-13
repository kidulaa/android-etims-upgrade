using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerBhfDeviceUserRecord.
    /// </summary>
    public class TaxpayerBhfDeviceUserRecord
    {

        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string UserId { get; set; }              // User ID
        public string UserNm { get; set; }              // User Name
        public string Pwd { get; set; }                 // Password
        public string Adrs { get; set; }                // Address
        public string Cntc { get; set; }                // Contact
        public string AuthCd { get; set; }              // Authority Code
        public string Remark { get; set; }              // Remark
        public string UseYn { get; set; }               // Use(Y/N)
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // 
        public string Contact { get; set; }             
        public string RoleCd { get; set; }             
        public string ImageNm { get; set; }         
        public byte[] Image { get; set; }               
        public string RoleName { get; set; }   

        public TaxpayerBhfDeviceUserRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.UserId = string.Empty;                 // User ID
            this.UserNm = string.Empty;                 // User Name
            this.Pwd = string.Empty;                    // Password
            this.Adrs = string.Empty;                   // Address
            this.Cntc = string.Empty;                   // Contact
            this.AuthCd = "00000000000000000000";       // Authority Code
            this.Remark = string.Empty;                 // Remark
            this.UseYn = "Y";                  // Use(Y/N)
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // 
            this.Contact = string.Empty;    
            this.RoleCd = "2";              
            this.ImageNm = string.Empty;               
            this.Image = null;                    
        }
    }
}

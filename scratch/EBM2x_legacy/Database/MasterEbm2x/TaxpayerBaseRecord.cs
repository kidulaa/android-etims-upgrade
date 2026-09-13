using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerBaseRecord.
    /// </summary>
    public class TaxpayerBaseRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string TaxprNm { get; set; }             // Taxpayer's Name
        public string TaxprSttsCd { get; set; }         // Taxpayer Status Code
        public string BsnsActv { get; set; }            // Business Activities
        public string PrvncNm { get; set; }                // Province No.
        public string DstrtNm { get; set; }             // District No.
        public string SctrNm { get; set; }              // Sector No.
        public string LocDesc { get; set; }             // Location Description
        public string TelNo { get; set; }               // Telephone number
        public string Email { get; set; }               // Email
        public string BankCd { get; set; }              // Bank Code
        public string BankAccntNo { get; set; }         // Bank Account Number
        public string BankAccntHldr { get; set; }       // Bank Account Holder
        public string ApcntNm { get; set; }             // Applicant name
        public string ApcntTelno { get; set; }          // Applicant telephone number
        public string ApcntEmail { get; set; }          // Applicant Email
        public string Remark { get; set; }              // Remark
        public string EbmTyCd { get; set; }             // EBM Type Code
        public string UserNo { get; set; }              // User No.
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public TaxpayerBaseRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.TaxprNm = string.Empty;                // Taxpayer's Name
            this.TaxprSttsCd = string.Empty;            // Taxpayer Status Code
            this.BsnsActv = string.Empty;               // Business Activities
            this.PrvncNm = string.Empty;                // Province No.
            this.DstrtNm = string.Empty;                // District No.
            this.SctrNm = string.Empty;                 // Sector No.
            this.LocDesc = string.Empty;                // Location Description
            this.TelNo = string.Empty;                  // Telephone number
            this.Email = string.Empty;                  // Email
            this.BankCd = string.Empty;                 // Bank Code
            this.BankAccntNo = string.Empty;            // Bank Account Number
            this.BankAccntHldr = string.Empty;          // Bank Account Holder
            this.ApcntNm = string.Empty;                // Applicant name
            this.ApcntTelno = string.Empty;             // Applicant telephone number
            this.ApcntEmail = string.Empty;             // Applicant Email
            this.Remark = string.Empty;                 // Remark
            this.EbmTyCd = string.Empty;                // EBM Type Code
            this.UserNo = string.Empty;                 // User No.
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

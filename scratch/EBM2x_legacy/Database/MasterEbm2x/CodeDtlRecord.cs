using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of CodeDtlRecord.
    /// </summary>
    public class CodeDtlRecord
    {

        public string Cd { get; set; }                  // Code
        public string CdCls { get; set; }               // Code classification
        public string CdNm { get; set; }                // Name of Code
        public string CdDesc { get; set; }              // Description of the Code
        public int SrtOrd { get; set; }                 // Sort Order
        public string UserDfnCd1 { get; set; }          // User Define Code 1
        public string UserDfnCd2 { get; set; }          // User Define Code 2
        public string UserDfnCd3 { get; set; }          // User Define Code 3
        public string UseYn { get; set; }               // Use(Y/N)
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public CodeDtlRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Cd = string.Empty;                     // Code
            this.CdCls = string.Empty;                  // Code classification
            this.CdNm = string.Empty;                   // Name of Code
            this.CdDesc = string.Empty;                 // Description of the Code
            this.SrtOrd = 0;                            // Sort Order
            this.UserDfnCd1 = string.Empty;             // User Define Code 1
            this.UserDfnCd2 = string.Empty;             // User Define Code 2
            this.UserDfnCd3 = string.Empty;             // User Define Code 3
            this.UseYn = "Y";                  // Use(Y/N)
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
        public void UpdateNull()
        {
            if (string.IsNullOrEmpty(this.Cd)) this.Cd = string.Empty;                     // Code
            if (string.IsNullOrEmpty(this.CdCls)) this.CdCls = string.Empty;                  // Code classification
            if (string.IsNullOrEmpty(this.CdNm)) this.CdNm = string.Empty;                   // Name of Code
            if (string.IsNullOrEmpty(this.CdDesc)) this.CdDesc = string.Empty;                 // Description of the Code
            //this.SrtOrd = 0;                            // Sort Order
            if (string.IsNullOrEmpty(this.UserDfnCd1)) this.UserDfnCd1 = string.Empty;             // User Define Code 1
            if (string.IsNullOrEmpty(this.UserDfnCd2)) this.UserDfnCd2 = string.Empty;             // User Define Code 2
            if (string.IsNullOrEmpty(this.UserDfnCd3)) this.UserDfnCd3 = string.Empty;             // User Define Code 3
            if (string.IsNullOrEmpty(this.UseYn)) this.UseYn = "Y";                  // Use(Y/N)
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

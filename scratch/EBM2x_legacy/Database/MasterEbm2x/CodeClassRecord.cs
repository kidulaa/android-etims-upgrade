using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of CodeClassRecord.
    /// </summary>
    public class CodeClassRecord
    {
        public string CdCls { get; set; }               // Code classification
        public string CdClsNm { get; set; }             // Name of code classification
        public string CdClsDesc { get; set; }           // Description of code classification
        public string UserDfnNm1 { get; set; }          // User Define Name 1
        public string UserDfnNm2 { get; set; }          // User Define Name 2
        public string UserDfnNm3 { get; set; }          // User Define Name 3

        //JCNA 202001 DELETE
        //public string ClientUseYn { get; set; }         // Use of Client(Y/N)
        public string UseYn { get; set; }               // Use(Y/N)
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public CodeClassRecord()
        {
            clear();
        }

        public void clear()
        {
            this.CdCls = string.Empty;                  // Code classification
            this.CdClsNm = string.Empty;                // Name of code classification
            this.CdClsDesc = string.Empty;              // Description of code classification
            this.UserDfnNm1 = string.Empty;             // User Define Name 1
            this.UserDfnNm2 = string.Empty;             // User Define Name 2
            this.UserDfnNm3 = string.Empty;             // User Define Name 3
            //JCNA 202001 DELETE
            //this.ClientUseYn = string.Empty;            // Use of Client(Y/N)
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
            if (string.IsNullOrEmpty(this.CdCls)) this.CdCls = "";                  // Code classification
            if (string.IsNullOrEmpty(this.CdClsNm)) this.CdClsNm = "";                // Name of code classification
            if (string.IsNullOrEmpty(this.CdClsDesc)) this.CdClsDesc = "";              // Description of code classification
            if (string.IsNullOrEmpty(this.UserDfnNm1)) this.UserDfnNm1 = "";             // User Define Name 1
            if (string.IsNullOrEmpty(this.UserDfnNm2)) this.UserDfnNm2 = "";             // User Define Name 2
            if (string.IsNullOrEmpty(this.UserDfnNm3)) this.UserDfnNm3 = "";             // User Define Name 3
            //JCNA 202001 DELETE
            //if (string.IsNullOrEmpty(this.ClientUseYn)) this.ClientUseYn = "";            // Use of Client(Y/N)
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

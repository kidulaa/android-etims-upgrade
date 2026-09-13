using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of BbsNoticeRecord.
    /// </summary>
    public class BbsNoticeRecord
    {
        public int NoticeNo { get; set; }               // Notice No.
        public string Title { get; set; }               // Title
        public string Cont { get; set; }                // Contents
        public string DtlUrl { get; set; }              // Read Count
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
        public string RegDt { get; set; }               // Registered Date

        public BbsNoticeRecord()
        {
            clear();
        }

        public void clear()
        {
            this.NoticeNo = 0;                          // Notice No.
            this.Title = string.Empty;                  // Title
            this.Cont = string.Empty;                   // Contents
            this.DtlUrl = string.Empty;                 // Read Count
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
        }
        public void UpdateNull()
        {
            //this.NoticeNo = 0;                          // Notice No.
            if (string.IsNullOrEmpty(this.Title)) this.Title = string.Empty;                  // Title
            if (string.IsNullOrEmpty(this.Cont)) this.Cont = string.Empty;                   // Contents
            if (string.IsNullOrEmpty(this.DtlUrl)) this.DtlUrl = string.Empty;                 // Read Count
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
        }
    }
}

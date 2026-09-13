using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TrnsSaleReceiptRecord.
    /// </summary>
    public class TrnsSaleReceiptRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public long InvcNo { get; set; }              // Invoice No.
        public string PrchrAcptcYn { get; set; }        // Purchaser Accepted(Y/N)
        public long OrgInvcNo { get; set; }           // Original Invoice No.
        public long CurRcptNo { get; set; }             // Current Receipt No.
        public long TotRcptNo { get; set; }             // Total Receipt No.
        public string TaxprNm { get; set; }             // Taxpayer Name
        public string RcptPbctDt { get; set; }          // Receipt Published Date
        public string IntrlData { get; set; }           // Internal Data
        public string RcptSign { get; set; }            // Receipt Signature
        public string Jrnl { get; set; }                // Journal
        public string TrdeNm { get; set; }              // Tradmark Name
        public string Adrs { get; set; }                // Address
        public string TopMsg { get; set; }              // Top Message
        public string BtmMsg { get; set; }              // Bottom Message
        public long RptNo { get; set; }                 // Receipt No.
        public string RptDt { get; set; }               // Receipt Date
        //JCNA 202001 DELETE 
        //public long TaskId { get; set; }                // Task ID
        //public string TaskStrtDt { get; set; }          // Task Start Date
        //public string TaskEndDt { get; set; }           // Task End Date
        //public string TaskCmptYn { get; set; }          // Task Completed(Y/N)
        //public string AudtFile { get; set; }            // Audit File
        //public string AudtFileEcrt { get; set; }        // Audit File Encryption
        //public string EbmSendDt { get; set; }           // EBM Send Date
        //public string EbmRes { get; set; }              // EBM Response
        //public string EbmResCd { get; set; }            // EBM Response Code
        //public string ScmSignData { get; set; }         // SCM Signature Date
        //public string ScmSign { get; set; }             // SCM Signature
        //public string ScmSignCfm { get; set; }          // SCM Signature Confirmation
        //public string SdcDt { get; set; }               // SDC Date

        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date



        public TrnsSaleReceiptRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.InvcNo = 0;                            // Invoice No.
            this.PrchrAcptcYn = string.Empty;           // Purchaser Accepted(Y/N)
            this.OrgInvcNo = 0;                         // Original Invoice No.
            this.CurRcptNo = 0;                         // Current Receipt No.
            this.TotRcptNo = 0;                         // Total Receipt No.
            this.TaxprNm = string.Empty;                // Taxpayer Name
            this.RcptPbctDt = string.Empty;             // Receipt Published Date
            this.IntrlData = string.Empty;              // Internal Data
            this.RcptSign = string.Empty;               // Receipt Signature
            this.Jrnl = string.Empty;                   // Journal
            this.TrdeNm = string.Empty;                 // Tradmark Name
            this.Adrs = string.Empty;                   // Address
            this.TopMsg = string.Empty;                 // Top Message
            this.BtmMsg = string.Empty;                 // Bottom Message
            this.RptNo = 0;                             // Receipt No.
            this.RptDt = string.Empty;                  // Receipt Date
            //JCNA 202001 DELETE 
            //this.TaskId = 0;                            // Task ID
            //this.TaskStrtDt = string.Empty;             // Task Start Date
            //this.TaskEndDt = string.Empty;              // Task End Date
            //this.TaskCmptYn = string.Empty;             // Task Completed(Y/N)
            //this.AudtFile = string.Empty;               // Audit File
            //this.AudtFileEcrt = string.Empty;           // Audit File Encryption
            //this.EbmSendDt = string.Empty;              // EBM Send Date
            //this.EbmRes = string.Empty;                 // EBM Response
            //this.EbmResCd = string.Empty;               // EBM Response Code
            //this.ScmSignData = string.Empty;            // SCM Signature Date
            //this.ScmSign = string.Empty;                // SCM Signature
            //this.ScmSignCfm = string.Empty;             // SCM Signature Confirmation
            //this.SdcDt = string.Empty;                  // SDC Date
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date

        }
        public void SetTrnsSaleRecord(TrnsSaleRecord trnsSaleRecord)
        {
            this.Tin = trnsSaleRecord.Tin;                      // Taxpayer Identification Number(TIN)
            this.BhfId = trnsSaleRecord.BhfId;                  // Branch Office ID
            this.InvcNo = trnsSaleRecord.InvcNo;                // Invoice No.
            this.PrchrAcptcYn = trnsSaleRecord.PrchrAcptcYn;    // Purchaser Accepted(Y/N)
            this.OrgInvcNo = trnsSaleRecord.OrgInvcNo;          // Original Invoice No.
            this.CurRcptNo = 0;                         // Current Receipt No. ==
            this.TotRcptNo = 0;                         // Total Receipt No.   ==
            this.TaxprNm = trnsSaleRecord.TaxprNm;      // Taxpayer Name
            this.RcptPbctDt = string.Empty;             // Receipt Published Date ==
            this.IntrlData = string.Empty;              // Internal Data ==
            this.RcptSign = string.Empty;               // Receipt Signature ==
            this.Jrnl = string.Empty;                   // Journal ==
            
            //if (!string.IsNullOrEmpty(trnsSaleRecord.TradeNm) && trnsSaleRecord.TradeNm.Length > 20)
            //{
            //    this.TrdeNm = trnsSaleRecord.TradeNm.Substring(0,20);       // Tradmark Name
            //}
            //else
            //{
            //    this.TrdeNm = trnsSaleRecord.TradeNm;       // Tradmark Name
            //}
            this.TrdeNm = string.Empty;

            this.Adrs = string.Empty;                   // Address ==
            this.TopMsg = string.Empty;                 // Top Message ==
            this.BtmMsg = string.Empty;                 // Bottom Message ==
            this.RptNo = 0;                             // Receipt No. ==
            this.RptDt = string.Empty;                  // Receipt Date ==
            //JCNA 202001 DELETE 
            //this.TaskId = 0;                            // Task ID
            //this.TaskStrtDt = string.Empty;             // Task Start Date
            //this.TaskEndDt = string.Empty;              // Task End Date
            //this.TaskCmptYn = string.Empty;             // Task Completed(Y/N)
            //this.AudtFile = string.Empty;               // Audit File
            //this.AudtFileEcrt = string.Empty;           // Audit File Encryption
            //this.EbmSendDt = string.Empty;              // EBM Send Date
            //this.EbmRes = string.Empty;                 // EBM Response
            //this.EbmResCd = string.Empty;               // EBM Response Code
            //this.ScmSignData = string.Empty;            // SCM Signature Date
            //this.ScmSign = string.Empty;                // SCM Signature
            //this.ScmSignCfm = string.Empty;             // SCM Signature Confirmation
            //this.SdcDt = string.Empty;                  // SDC Date
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

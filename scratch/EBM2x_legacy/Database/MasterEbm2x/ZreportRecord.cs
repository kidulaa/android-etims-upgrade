using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of ZreportRecord.
    /// </summary>
    public class ZreportRecord
    {
        public string Tin { get; set; }                 // TIN
        public string BhfId { get; set; }               // BHF_ID
        public string SdcId { get; set; }               // SDC_ID
        public string Reportdate { get; set; }          // REPORTDATE
        public long Reportnumber { get; set; }          // REPORTNUMBER
        public long Dailynbofreceipts { get; set; }     // DAILYNBOFRECEIPTS
        public long Openingrunnumber { get; set; }      // OPENINGRUNNUMBER
        public long Closingrunnumber { get; set; }      // CLOSINGRUNNUMBER
        public long Normaltotalreceipts { get; set; }   // NORMALTOTALRECEIPTS
        public long Normalopeningrunnumber { get; set; }  // NORMALOPENINGRUNNUMBER
        public long Normalclosingrunnumber { get; set; }  // NORMALCLOSINGRUNNUMBER
        public double Normaltotalsaleamount { get; set; }  // NORMALTOTALSALEAMOUNT
        public double Normaltotalreturnamount { get; set; }  // NORMALTOTALRETURNAMOUNT
        public double Normaltotaltaxsaleamount { get; set; }  // NORMALTOTALTAXSALEAMOUNT
        public double Normaltotaltaxreturnamount { get; set; }  // NORMALTOTALTAXRETURNAMOUNT
        public long Copytotalreceipts { get; set; }     // COPYTOTALRECEIPTS
        public long Copyopeningrunnumber { get; set; }  // COPYOPENINGRUNNUMBER
        public long Copyclosingrunnumber { get; set; }  // COPYCLOSINGRUNNUMBER
        public double Copytotalsaleamount { get; set; }  // COPYTOTALSALEAMOUNT
        public double Copytotalreturnamount { get; set; }  // COPYTOTALRETURNAMOUNT
        public double Copytotaltaxsaleamount { get; set; }  // COPYTOTALTAXSALEAMOUNT
        public double Copytotaltaxreturnamount { get; set; }  // COPYTOTALTAXRETURNAMOUNT
        public long Trainingtotalreceipts { get; set; }  // TRAININGTOTALRECEIPTS
        public long Trainingopeningrunnumber { get; set; }  // TRAININGOPENINGRUNNUMBER
        public long Trainingclosingrunnumber { get; set; }  // TRAININGCLOSINGRUNNUMBER
        public double Trainingtotalsaleamount { get; set; }  // TRAININGTOTALSALEAMOUNT
        public double Trainingtotalreturnamount { get; set; }  // TRAININGTOTALRETURNAMOUNT
        public double Trainingtotaltaxsaleamount { get; set; }  // TRAININGTOTALTAXSALEAMOUNT
        public double Trainingtotaltaxreturnamount { get; set; }  // TRAININGTOTALTAXRETURNAMOUNT
        public long Proformatotalreceipts { get; set; }  // PROFORMATOTALRECEIPTS
        public long Proformaopeningrunnumber { get; set; }  // PROFORMAOPENINGRUNNUMBER
        public long Proformaclosingrunnumber { get; set; }  // PROFORMACLOSINGRUNNUMBER
        public double Proformatotalsaleamount { get; set; }  // PROFORMATOTALSALEAMOUNT
        public double Proformatotalreturnamount { get; set; }  // PROFORMATOTALRETURNAMOUNT
        public double Proformatotaltaxsaleamount { get; set; }  // PROFORMATOTALTAXSALEAMOUNT
        public double Proformatotaltaxreturnamount { get; set; }  // PROFORMATOTALTAXRETURNAMOUNT
        public long Totnbreceipts { get; set; }         // TOTNBRECEIPTS
        public long Totnbreceiptsnormal { get; set; }   // TOTNBRECEIPTSNORMAL
        public long Totnbreceiptscopy { get; set; }     // TOTNBRECEIPTSCOPY
        public long Totnbreceiptstraining { get; set; }  // TOTNBRECEIPTSTRAINING
        public long Totnbreceiptsproforma { get; set; }  // TOTNBRECEIPTSPROFORMA
        public double Totalsaleamount { get; set; }     // TOTALSALEAMOUNT
        public double Totalsalestaxamount { get; set; }  // TOTALSALESTAXAMOUNT
        public double Totalreturnamount { get; set; }   // TOTALRETURNAMOUNT
        public double Totalreturntaxamount { get; set; }  // TOTALRETURNTAXAMOUNT
        public string RegDt { get; set; }               // REG_DT
        public string UserId { get; set; }            // USERID
        public string Username { get; set; }            // USERNAME

        public ZreportRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // TIN
            this.BhfId = string.Empty;                  // BHF_ID
            this.SdcId = string.Empty;                  // SDC_ID
            this.Reportdate = string.Empty;             // REPORTDATE
            this.Reportnumber = 0;                      // REPORTNUMBER
            this.Dailynbofreceipts = 0;                 // DAILYNBOFRECEIPTS
            this.Openingrunnumber = 0;                  // OPENINGRUNNUMBER
            this.Closingrunnumber = 0;                  // CLOSINGRUNNUMBER
            this.Normaltotalreceipts = 0;               // NORMALTOTALRECEIPTS
            this.Normalopeningrunnumber = 0;            // NORMALOPENINGRUNNUMBER
            this.Normalclosingrunnumber = 0;            // NORMALCLOSINGRUNNUMBER
            this.Normaltotalsaleamount = 0;             // NORMALTOTALSALEAMOUNT
            this.Normaltotalreturnamount = 0;           // NORMALTOTALRETURNAMOUNT
            this.Normaltotaltaxsaleamount = 0;          // NORMALTOTALTAXSALEAMOUNT
            this.Normaltotaltaxreturnamount = 0;        // NORMALTOTALTAXRETURNAMOUNT
            this.Copytotalreceipts = 0;                 // COPYTOTALRECEIPTS
            this.Copyopeningrunnumber = 0;              // COPYOPENINGRUNNUMBER
            this.Copyclosingrunnumber = 0;              // COPYCLOSINGRUNNUMBER
            this.Copytotalsaleamount = 0;               // COPYTOTALSALEAMOUNT
            this.Copytotalreturnamount = 0;             // COPYTOTALRETURNAMOUNT
            this.Copytotaltaxsaleamount = 0;            // COPYTOTALTAXSALEAMOUNT
            this.Copytotaltaxreturnamount = 0;          // COPYTOTALTAXRETURNAMOUNT
            this.Trainingtotalreceipts = 0;             // TRAININGTOTALRECEIPTS
            this.Trainingopeningrunnumber = 0;          // TRAININGOPENINGRUNNUMBER
            this.Trainingclosingrunnumber = 0;          // TRAININGCLOSINGRUNNUMBER
            this.Trainingtotalsaleamount = 0;           // TRAININGTOTALSALEAMOUNT
            this.Trainingtotalreturnamount = 0;         // TRAININGTOTALRETURNAMOUNT
            this.Trainingtotaltaxsaleamount = 0;        // TRAININGTOTALTAXSALEAMOUNT
            this.Trainingtotaltaxreturnamount = 0;      // TRAININGTOTALTAXRETURNAMOUNT
            this.Proformatotalreceipts = 0;             // PROFORMATOTALRECEIPTS
            this.Proformaopeningrunnumber = 0;          // PROFORMAOPENINGRUNNUMBER
            this.Proformaclosingrunnumber = 0;          // PROFORMACLOSINGRUNNUMBER
            this.Proformatotalsaleamount = 0;           // PROFORMATOTALSALEAMOUNT
            this.Proformatotalreturnamount = 0;         // PROFORMATOTALRETURNAMOUNT
            this.Proformatotaltaxsaleamount = 0;        // PROFORMATOTALTAXSALEAMOUNT
            this.Proformatotaltaxreturnamount = 0;      // PROFORMATOTALTAXRETURNAMOUNT
            this.Totnbreceipts = 0;                     // TOTNBRECEIPTS
            this.Totnbreceiptsnormal = 0;               // TOTNBRECEIPTSNORMAL
            this.Totnbreceiptscopy = 0;                 // TOTNBRECEIPTSCOPY
            this.Totnbreceiptstraining = 0;             // TOTNBRECEIPTSTRAINING
            this.Totnbreceiptsproforma = 0;             // TOTNBRECEIPTSPROFORMA
            this.Totalsaleamount = 0;                   // TOTALSALEAMOUNT
            this.Totalsalestaxamount = 0;               // TOTALSALESTAXAMOUNT
            this.Totalreturnamount = 0;                 // TOTALRETURNAMOUNT
            this.Totalreturntaxamount = 0;              // TOTALRETURNTAXAMOUNT
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // REG_DT
            this.UserId = "system";                 // USERID
            this.Username = "system";               // USERNAME
        }
    }
}

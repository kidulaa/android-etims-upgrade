using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.MobileWallet
{
    /// <summary>
	/// Description of DebitReqRecord.
	/// </summary>
	public class DebitReqRecord
    {
        public string Grphdr { get; set; }              // GrpHdr - Group Header
        public string Msgid { get; set; }               // MsgId - Message Unique  Identifier
        public string Credttm { get; set; }             // CreDtTm - Creation Date and Time
        public string Nboftxs { get; set; }             // NbOfTxs - Number of Transactions
        public string Ctrlsum { get; set; }             // CtrlSum - Control Sum
        public string Initgpty { get; set; }            // InitgPty - Initiating Party
        public string Nm { get; set; }                  // Nm - Initiator’s Name
        public string Id { get; set; }                  // Id - Identifier
        public string Orgid { get; set; }               // OrgId - Organization Identification
        public string Othr { get; set; }                // Othr - Other
        public string Pmtinf { get; set; }              // PmtInf - Payment  Information
        public string Pmtinfid { get; set; }            // PmtInfId - Payment  Information  Identification
        public string Pmtmtd { get; set; }              // PmtMtd - PaymentMethod
        public string Btchbookg { get; set; }           // BtchBookg - BatchBooking
        public string Pmttpinf { get; set; }            // PmtTpInf - Payment Type Information
        public string Svclvl { get; set; }              // SvcLvl - Service Level
        public string Cd { get; set; }                  // Cd - code
        public string Lclinstrm { get; set; }           // LclInstrm - Local Instrument
        public string Seqtp { get; set; }               // SeqTp - Sequence Type
        public string Reqdcolltndt { get; set; }        // ReqdColltnDt - Requested  Collection Date
        public string Cdtr { get; set; }                // Cdtr - Creditor
        public string Cdtracct { get; set; }            // CdtrAcct - Creditor Account
        public string Iban { get; set; }                // IBAN - International  Bank Account  Number
        public string Cdtragt { get; set; }             // CdtrAgt - Creditor Agent
        public string Fininstnid { get; set; }          // FinInstnId - Financial  Institution  Identification
        public string Bicfi { get; set; }               // BICFI - BIC
        public string Chrgbr { get; set; }              // ChrgBr - Change Bearer
        public string Cdtrschmeid { get; set; }         // CdtrSchmeId - Creditor Scheme  Identification
        public string Prvtid { get; set; }              // PrvtId - Private  Identification
        public string Schmenm { get; set; }             // SchmeNm - Scheme Name
        public string Prtry { get; set; }               // Prtry - Proprietary
        public string Drctdbttxinf { get; set; }        // DrctDbtTxInf - Direct Debit  Transaction  Information
        public string Pmtid { get; set; }               // PmtId - Payment  Identification
        public string Instrid { get; set; }             // InstrId - Instruction  Identification
        public string Endtoendid { get; set; }          // EndToEndId - End to End identification
        public string InstdamtCcy { get; set; }         // InstdAmt Ccy - Instructed  Amount
        public string Drctdbttx { get; set; }           // DrctDbtTx - Direct Debit Transaction
        public string Mndtrltdinf { get; set; }         // MndtRltdInf - Mandate Related Information
        public string Mndtid { get; set; }              // MndtId - Mandate  Identification
        public string Dtofsgntr { get; set; }           // DtOfSgntr - Date of Signature
        public string Dbtragt { get; set; }             // DbtrAgt - Debtor Agent
        public string Rmtinf { get; set; }              // RmtInf - Related  Remittance  Information
        public string Ustrd { get; set; }               // Ustrd - Unstructured

        public DebitReqRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Grphdr = string.Empty;                 // GrpHdr - Group Header
            this.Msgid = string.Empty;                  // MsgId - Message Unique  Identifier
            this.Credttm = string.Empty;                // CreDtTm - Creation Date and Time
            this.Nboftxs = string.Empty;                // NbOfTxs - Number of Transactions
            this.Ctrlsum = string.Empty;                // CtrlSum - Control Sum
            this.Initgpty = string.Empty;               // InitgPty - Initiating Party
            this.Nm = string.Empty;                     // Nm - Initiator’s Name
            this.Id = string.Empty;                     // Id - Identifier
            this.Orgid = string.Empty;                  // OrgId - Organization Identification
            this.Othr = string.Empty;                   // Othr - Other
            this.Pmtinf = string.Empty;                 // PmtInf - Payment  Information
            this.Pmtinfid = string.Empty;               // PmtInfId - Payment  Information  Identification
            this.Pmtmtd = string.Empty;                 // PmtMtd - PaymentMethod
            this.Btchbookg = string.Empty;              // BtchBookg - BatchBooking
            this.Pmttpinf = string.Empty;               // PmtTpInf - Payment Type Information
            this.Svclvl = string.Empty;                 // SvcLvl - Service Level
            this.Cd = string.Empty;                     // Cd - code
            this.Lclinstrm = string.Empty;              // LclInstrm - Local Instrument
            this.Seqtp = string.Empty;                  // SeqTp - Sequence Type
            this.Reqdcolltndt = string.Empty;           // ReqdColltnDt - Requested  Collection Date
            this.Cdtr = string.Empty;                   // Cdtr - Creditor
            this.Cdtracct = string.Empty;               // CdtrAcct - Creditor Account
            this.Iban = string.Empty;                   // IBAN - International  Bank Account  Number
            this.Cdtragt = string.Empty;                // CdtrAgt - Creditor Agent
            this.Fininstnid = string.Empty;             // FinInstnId - Financial  Institution  Identification
            this.Bicfi = string.Empty;                  // BICFI - BIC
            this.Chrgbr = string.Empty;                 // ChrgBr - Change Bearer
            this.Cdtrschmeid = string.Empty;            // CdtrSchmeId - Creditor Scheme  Identification
            this.Prvtid = string.Empty;                 // PrvtId - Private  Identification
            this.Schmenm = string.Empty;                // SchmeNm - Scheme Name
            this.Prtry = string.Empty;                  // Prtry - Proprietary
            this.Drctdbttxinf = string.Empty;           // DrctDbtTxInf - Direct Debit  Transaction  Information
            this.Pmtid = string.Empty;                  // PmtId - Payment  Identification
            this.Instrid = string.Empty;                // InstrId - Instruction  Identification
            this.Endtoendid = string.Empty;             // EndToEndId - End to End identification
            this.InstdamtCcy = string.Empty;            // InstdAmt Ccy - Instructed  Amount
            this.Drctdbttx = string.Empty;              // DrctDbtTx - Direct Debit Transaction
            this.Mndtrltdinf = string.Empty;            // MndtRltdInf - Mandate Related Information
            this.Mndtid = string.Empty;                 // MndtId - Mandate  Identification
            this.Dtofsgntr = string.Empty;              // DtOfSgntr - Date of Signature
            this.Dbtragt = string.Empty;                // DbtrAgt - Debtor Agent
            this.Rmtinf = string.Empty;                 // RmtInf - Related  Remittance  Information
            this.Ustrd = string.Empty;                  // Ustrd - Unstructured
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.MobileWallet
{
    /// <summary>
	/// Description of DebitResRecord.
	/// </summary>
	public class DebitResRecord
    {
        public string Grphdr { get; set; }              // GrpHdr - Group Header
        public string Msgid { get; set; }               // MsgId - Message Unique  Identifier
        public string Credttm { get; set; }             // CreDtTm - Creation Date and Time
        public string Nboftxs { get; set; }             // InitgPty - Message Unique  Identifier
        public string Ctrlsum { get; set; }             // Nm - Name
        public string Initgpty { get; set; }            // OrgnlGrpInfAndSts - Original Group   Information And  Status
        public string Nm { get; set; }                  // OrgnlMsgId - Original  Message  Identification
        public string Id { get; set; }                  // OrgnlMsgNmId - Original  Message Name  Identification
        public string Orgid { get; set; }               // OrgnlCreDtTm - Original  Creation Date Time
        public string Othr { get; set; }                // OrgnlNbOfTxs - Original  Number Of  Transactions
        public string Pmtinf { get; set; }              // OrgnlCtrlSum - OriginalControlS um
        public string Pmtinfid { get; set; }            // GrpSts - GroupStatus
        public string Pmtmtd { get; set; }              // PmtInfSts - Payment  Information Status
        public string Btchbookg { get; set; }           // OrgnlPmtInfId - Original  Payment  Information  Identification
        public string Pmttpinf { get; set; }            // OrgnlInstrId - Original  Instruction  Identification
        public string Svclvl { get; set; }              // TxInfAndSts - Transaction  Information And Status
        public string Cd { get; set; }                  // OrgnlEndToEndId - Original End To  End  Identification
        public string Lclinstrm { get; set; }           // TxSts - Transaction Status
        public string Seqtp { get; set; }               // OrgnlTxRef - Original  Transaction  Reference
        public string Reqdcolltndt { get; set; }        // InstdAmt Ccy - Instructed  Amount
        public string Cdtr { get; set; }                // Amt - Amount
        public string Cdtracct { get; set; }            // ReqdExctnDt - Requested  Execution Date
        public string Iban { get; set; }                // Dt - Date
        public string Cdtragt { get; set; }             // Dbtr - Debtor
        public string Fininstnid { get; set; }          // Pty - Party
        public string Bicfi { get; set; }               // Nm - Name
        public string Chrgbr { get; set; }              // DbtrAcct - Debtor Account
        public string Cdtrschmeid { get; set; }         // Id - Identification
        public string Prvtid { get; set; }              // IBAN - International  Bank Account  Number (IBAN)
        public string Schmenm { get; set; }             // DbtrAgt - Debtor Agent
        public string Prtry { get; set; }               // CdtrAgt - Creditor Agent
        public string Drctdbttxinf { get; set; }        // Cdtr - Creditor
        public string Pmtid { get; set; }               // CdtrAcct - Creditor Account
        public string Instrid { get; set; }             // FinInstnId - Financial  Institution  Identification
        public string Endtoendid { get; set; }          // BICFI - BIC
        public string InstdamtCcy { get; set; }         // RmtInf - Related  Remittance  Information

        public DebitResRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Grphdr = string.Empty;                 // GrpHdr - Group Header
            this.Msgid = string.Empty;                  // MsgId - Message Unique  Identifier
            this.Credttm = string.Empty;                // CreDtTm - Creation Date and Time
            this.Nboftxs = string.Empty;                // InitgPty - Message Unique  Identifier
            this.Ctrlsum = string.Empty;                // Nm - Name
            this.Initgpty = string.Empty;               // OrgnlGrpInfAndSts - Original Group   Information And  Status
            this.Nm = string.Empty;                     // OrgnlMsgId - Original  Message  Identification
            this.Id = string.Empty;                     // OrgnlMsgNmId - Original  Message Name  Identification
            this.Orgid = string.Empty;                  // OrgnlCreDtTm - Original  Creation Date Time
            this.Othr = string.Empty;                   // OrgnlNbOfTxs - Original  Number Of  Transactions
            this.Pmtinf = string.Empty;                 // OrgnlCtrlSum - OriginalControlS um
            this.Pmtinfid = string.Empty;               // GrpSts - GroupStatus
            this.Pmtmtd = string.Empty;                 // PmtInfSts - Payment  Information Status
            this.Btchbookg = string.Empty;              // OrgnlPmtInfId - Original  Payment  Information  Identification
            this.Pmttpinf = string.Empty;               // OrgnlInstrId - Original  Instruction  Identification
            this.Svclvl = string.Empty;                 // TxInfAndSts - Transaction  Information And Status
            this.Cd = string.Empty;                     // OrgnlEndToEndId - Original End To  End  Identification
            this.Lclinstrm = string.Empty;              // TxSts - Transaction Status
            this.Seqtp = string.Empty;                  // OrgnlTxRef - Original  Transaction  Reference
            this.Reqdcolltndt = string.Empty;           // InstdAmt Ccy - Instructed  Amount
            this.Cdtr = string.Empty;                   // Amt - Amount
            this.Cdtracct = string.Empty;               // ReqdExctnDt - Requested  Execution Date
            this.Iban = string.Empty;                   // Dt - Date
            this.Cdtragt = string.Empty;                // Dbtr - Debtor
            this.Fininstnid = string.Empty;             // Pty - Party
            this.Bicfi = string.Empty;                  // Nm - Name
            this.Chrgbr = string.Empty;                 // DbtrAcct - Debtor Account
            this.Cdtrschmeid = string.Empty;            // Id - Identification
            this.Prvtid = string.Empty;                 // IBAN - International  Bank Account  Number (IBAN)
            this.Schmenm = string.Empty;                // DbtrAgt - Debtor Agent
            this.Prtry = string.Empty;                  // CdtrAgt - Creditor Agent
            this.Drctdbttxinf = string.Empty;           // Cdtr - Creditor
            this.Pmtid = string.Empty;                  // CdtrAcct - Creditor Account
            this.Instrid = string.Empty;                // FinInstnId - Financial  Institution  Identification
            this.Endtoendid = string.Empty;             // BICFI - BIC
            this.InstdamtCcy = string.Empty;            // RmtInf - Related  Remittance  Information
        }
    }
}

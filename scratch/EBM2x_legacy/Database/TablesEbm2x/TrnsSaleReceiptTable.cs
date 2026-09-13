using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TrnsSaleReceiptTable.
    /// </summary>
    public class TrnsSaleReceiptTable
    {
        public TrnsSaleReceiptTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TRNS_SALE_RECEIPT ( "); // 거래 판매 영수증
            sql.Append("       TIN                CHAR(9)             not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)             not null , ");      // Branch Office ID
            sql.Append("       INVC_NO            INT                 not null , ");      // Invoice No.
            sql.Append("       PRCHR_ACPTC_YN     CHAR(1)             not null , ");      // Purchaser Accepted(Y/N)
            sql.Append("       ORG_INVC_NO        INT                 null , ");          // Original Invoice No.
            sql.Append("       CUR_RCPT_NO        INT                 null , ");          // Current Receipt No.
            sql.Append("       TOT_RCPT_NO        INT                 null , ");          // Total Receipt No.
            sql.Append("       TAXPR_NM           VARCHAR(60)         null , ");          // Taxpayer Name
            sql.Append("       RCPT_PBCT_DT       CHAR(8)             null , ");          // Receipt Published Date
            sql.Append("       INTRL_DATA         VARCHAR(26)         null , ");          // Internal Data
            sql.Append("       RCPT_SIGN          VARCHAR(16)         null , ");          // Receipt Signature
            sql.Append("       JRNL               CLOB                null , ");          // Journal
            sql.Append("       TRDE_NM            VARCHAR(20)         null , ");          // Tradmark Name
            sql.Append("       ADRS               VARCHAR(30)         null , ");          // Address
            sql.Append("       TOP_MSG            VARCHAR(20)         null , ");          // Top Message
            sql.Append("       BTM_MSG            VARCHAR(20)         null , ");          // Bottom Message
            sql.Append("       RPT_NO             INT                 null , ");          // Receipt No.
            sql.Append("       RPT_DT             CHAR(8)             null , ");          // Receipt Date
            //JCNA 202001 DELETE
            //sql.Append("       TASK_ID            INT                 null , ");          // Task ID
            //sql.Append("       TASK_STRT_DT       CHAR(8)             null , ");          // Task Start Date
            //sql.Append("       TASK_END_DT        CHAR(8)             null , ");          // Task End Date
            //sql.Append("       TASK_CMPT_YN       CHAR(1)             null , ");          // Task Completed(Y/N)
            //sql.Append("       AUDT_FILE          VARCHAR(255)        null , ");          // Audit File
            //sql.Append("       AUDT_FILE_ECRT     VARCHAR(255)        null , ");          // Audit File Encryption
            //sql.Append("       EBM_SEND_DT        CHAR(8)             null , ");          // EBM Send Date
            //sql.Append("       EBM_RES            VARCHAR(400)        null , ");          // EBM Response
            //sql.Append("       EBM_RES_CD         VARCHAR(5)          null , ");          // EBM Response Code
            //sql.Append("       SCM_SIGN_DATA      VARCHAR(400)        null , ");          // SCM Signature Date
            //sql.Append("       SCM_SIGN           VARCHAR(50)         null , ");          // SCM Signature
            //sql.Append("       SCM_SIGN_CFM       CHAR(1)             null , ");          // SCM Signature Confirmation
            //sql.Append("       SDC_DT             CHAR(8)             null , ");          // SDC Date
            sql.Append("       REGR_ID            VARCHAR(20)         null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)         null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)         null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)         null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)         null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)         null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID, INVC_NO ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       PRCHR_ACPTC_YN, ");          // Purchaser Accepted(Y/N)
            sql.Append("       ORG_INVC_NO, ");             // Original Invoice No.
            sql.Append("       CUR_RCPT_NO, ");             // Current Receipt No.
            sql.Append("       TOT_RCPT_NO, ");             // Total Receipt No.
            sql.Append("       TAXPR_NM, ");                // Taxpayer Name
            sql.Append("       RCPT_PBCT_DT, ");            // Receipt Published Date
            sql.Append("       INTRL_DATA, ");              // Internal Data
            sql.Append("       RCPT_SIGN, ");               // Receipt Signature
            sql.Append("       JRNL, ");                    // Journal
            sql.Append("       TRDE_NM, ");                 // Tradmark Name
            sql.Append("       ADRS, ");                    // Address
            sql.Append("       TOP_MSG, ");                 // Top Message
            sql.Append("       BTM_MSG, ");                 // Bottom Message
            sql.Append("       RPT_NO, ");                  // Receipt No.
            sql.Append("       RPT_DT, ");                  // Receipt Date
            //JCNA 202001 DELETE
            //sql.Append("       TASK_ID, ");                 // Task ID
            //sql.Append("       TASK_STRT_DT, ");            // Task Start Date
            //sql.Append("       TASK_END_DT, ");             // Task End Date
            //sql.Append("       TASK_CMPT_YN, ");            // Task Completed(Y/N)
            //sql.Append("       AUDT_FILE, ");               // Audit File
            //sql.Append("       AUDT_FILE_ECRT, ");          // Audit File Encryption
            //sql.Append("       EBM_SEND_DT, ");             // EBM Send Date
            //sql.Append("       EBM_RES, ");                 // EBM Response
            //sql.Append("       EBM_RES_CD, ");              // EBM Response Code
            //sql.Append("       SCM_SIGN_DATA, ");           // SCM Signature Date
            //sql.Append("       SCM_SIGN, ");                // SCM Signature
            //sql.Append("       SCM_SIGN_CFM, ");            // SCM Signature Confirmation
            //sql.Append("       SDC_DT, ");                  // SDC Date
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                 // Modified Date
            sql.Append("  from TRNS_SALE_RECEIPT "); // 거래 판매 영수증
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TRNS_SALE_RECEIPT ( "); // 거래 판매 영수증
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       PRCHR_ACPTC_YN, ");          // Purchaser Accepted(Y/N)
            sql.Append("       ORG_INVC_NO, ");             // Original Invoice No.
            sql.Append("       CUR_RCPT_NO, ");             // Current Receipt No.
            sql.Append("       TOT_RCPT_NO, ");             // Total Receipt No.
            sql.Append("       TAXPR_NM, ");                // Taxpayer Name
            sql.Append("       RCPT_PBCT_DT, ");            // Receipt Published Date
            sql.Append("       INTRL_DATA, ");              // Internal Data
            sql.Append("       RCPT_SIGN, ");               // Receipt Signature
            sql.Append("       JRNL, ");                    // Journal
            sql.Append("       TRDE_NM, ");                 // Tradmark Name
            sql.Append("       ADRS, ");                    // Address
            sql.Append("       TOP_MSG, ");                 // Top Message
            sql.Append("       BTM_MSG, ");                 // Bottom Message
            sql.Append("       RPT_NO, ");                  // Receipt No.
            sql.Append("       RPT_DT, ");                  // Receipt Date
            //JCNA 202001 DELETE
            //sql.Append("       TASK_ID, ");                 // Task ID
            //sql.Append("       TASK_STRT_DT, ");            // Task Start Date
            //sql.Append("       TASK_END_DT, ");             // Task End Date
            //sql.Append("       TASK_CMPT_YN, ");            // Task Completed(Y/N)
            //sql.Append("       AUDT_FILE, ");               // Audit File
            //sql.Append("       AUDT_FILE_ECRT, ");          // Audit File Encryption
            //sql.Append("       EBM_SEND_DT, ");             // EBM Send Date
            //sql.Append("       EBM_RES, ");                 // EBM Response
            //sql.Append("       EBM_RES_CD, ");              // EBM Response Code
            //sql.Append("       SCM_SIGN_DATA, ");           // SCM Signature Date
            //sql.Append("       SCM_SIGN, ");                // SCM Signature
            //sql.Append("       SCM_SIGN_CFM, ");            // SCM Signature Confirmation
            //sql.Append("       SDC_DT, ");                  // SDC Date
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                 // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @InvcNo, ");                 // Invoice No.
            sql.Append("       @PrchrAcptcYn, ");           // Purchaser Accepted(Y/N)
            sql.Append("       @OrgInvcNo, ");              // Original Invoice No.
            sql.Append("       @CurRcptNo, ");              // Current Receipt No.
            sql.Append("       @TotRcptNo, ");              // Total Receipt No.
            sql.Append("       @TaxprNm, ");                // Taxpayer Name
            sql.Append("       @RcptPbctDt, ");             // Receipt Published Date
            sql.Append("       @IntrlData, ");              // Internal Data
            sql.Append("       @RcptSign, ");               // Receipt Signature
            sql.Append("       @Jrnl, ");                   // Journal
            sql.Append("       @TrdeNm, ");                 // Tradmark Name
            sql.Append("       @Adrs, ");                   // Address
            sql.Append("       @TopMsg, ");                 // Top Message
            sql.Append("       @BtmMsg, ");                 // Bottom Message
            sql.Append("       @RptNo, ");                  // Receipt No.
            sql.Append("       @RptDt, ");                  // Receipt Date
            //JCNA 202001 DELETE
            //sql.Append("       @TaskId, ");                 // Task ID
            //sql.Append("       @TaskStrtDt, ");             // Task Start Date
            //sql.Append("       @TaskEndDt, ");              // Task End Date
            //sql.Append("       @TaskCmptYn, ");             // Task Completed(Y/N)
            //sql.Append("       @AudtFile, ");               // Audit File
            //sql.Append("       @AudtFileEcrt, ");           // Audit File Encryption
            //sql.Append("       @EbmSendDt, ");              // EBM Send Date
            //sql.Append("       @EbmRes, ");                 // EBM Response
            //sql.Append("       @EbmResCd, ");               // EBM Response Code
            //sql.Append("       @ScmSignData, ");            // SCM Signature Date
            //sql.Append("       @ScmSign, ");                // SCM Signature
            //sql.Append("       @ScmSignCfm, ");             // SCM Signature Confirmation
            //sql.Append("       @SdcDt, ");                  // SDC Date
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @RegDt, ");                  // Registered Date
            sql.Append("       @ModrId, ");                 // Modifier ID
            sql.Append("       @ModrNm, ");                 // Modifier Name
            sql.Append("       @ModDt  ");                  // Modified Date
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TRNS_SALE_RECEIPT "); // 거래 판매 영수증
            sql.Append("   set PRCHR_ACPTC_YN = @PrchrAcptcYn, ");  // Purchaser Accepted(Y/N)
            sql.Append("       ORG_INVC_NO = @OrgInvcNo, ");  // Original Invoice No.
            sql.Append("       CUR_RCPT_NO = @CurRcptNo, ");  // Current Receipt No.
            sql.Append("       TOT_RCPT_NO = @TotRcptNo, ");  // Total Receipt No.
            sql.Append("       TAXPR_NM = @TaxprNm, ");     // Taxpayer Name
            sql.Append("       RCPT_PBCT_DT = @RcptPbctDt, ");  // Receipt Published Date
            sql.Append("       INTRL_DATA = @IntrlData, ");  // Internal Data
            sql.Append("       RCPT_SIGN = @RcptSign, ");   // Receipt Signature
            sql.Append("       JRNL = @Jrnl, ");            // Journal
            sql.Append("       TRDE_NM = @TrdeNm, ");       // Tradmark Name
            sql.Append("       ADRS = @Adrs, ");            // Address
            sql.Append("       TOP_MSG = @TopMsg, ");       // Top Message
            sql.Append("       BTM_MSG = @BtmMsg, ");       // Bottom Message
            sql.Append("       RPT_NO = @RptNo, ");         // Receipt No.
            sql.Append("       RPT_DT = @RptDt, ");         // Receipt Date
            //JCNA 202001 DELETE
            //sql.Append("       TASK_ID = @TaskId, ");       // Task ID
            //sql.Append("       TASK_STRT_DT = @TaskStrtDt, ");  // Task Start Date
            //sql.Append("       TASK_END_DT = @TaskEndDt, ");  // Task End Date
            //sql.Append("       TASK_CMPT_YN = @TaskCmptYn, ");  // Task Completed(Y/N)
            //sql.Append("       AUDT_FILE = @AudtFile, ");   // Audit File
            //sql.Append("       AUDT_FILE_ECRT = @AudtFileEcrt, ");  // Audit File Encryption
            //sql.Append("       EBM_SEND_DT = @EbmSendDt, ");  // EBM Send Date
            //sql.Append("       EBM_RES = @EbmRes, ");       // EBM Response
            //sql.Append("       EBM_RES_CD = @EbmResCd, ");  // EBM Response Code
            //sql.Append("       SCM_SIGN_DATA = @ScmSignData, ");  // SCM Signature Date
            //sql.Append("       SCM_SIGN = @ScmSign, ");     // SCM Signature
            //sql.Append("       SCM_SIGN_CFM = @ScmSignCfm, ");  // SCM Signature Confirmation
            //sql.Append("       SDC_DT = @SdcDt, ");         // SDC Date
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TRNS_SALE_RECEIPT "); // 거래 판매 영수증
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TrnsSaleReceiptRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@BhfId";
            param.Value = record.BhfId;
            command.Parameters.Add(param);                  // Branch Office ID

            param = command.CreateParameter();
            param.ParameterName = "@InvcNo";
            param.Value = record.InvcNo;
            command.Parameters.Add(param);                  // Invoice No.

            param = command.CreateParameter();
            param.ParameterName = "@PrchrAcptcYn";
            param.Value = record.PrchrAcptcYn;
            command.Parameters.Add(param);                  // Purchaser Accepted(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@OrgInvcNo";
            param.Value = record.OrgInvcNo;
            command.Parameters.Add(param);                  // Original Invoice No.

            param = command.CreateParameter();
            param.ParameterName = "@CurRcptNo";
            param.Value = record.CurRcptNo;
            command.Parameters.Add(param);                  // Current Receipt No.

            param = command.CreateParameter();
            param.ParameterName = "@TotRcptNo";
            param.Value = record.TotRcptNo;
            command.Parameters.Add(param);                  // Total Receipt No.

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = record.TaxprNm;
            command.Parameters.Add(param);                  // Taxpayer Name

            param = command.CreateParameter();
            param.ParameterName = "@RcptPbctDt";
            param.Value = record.RcptPbctDt;
            command.Parameters.Add(param);                  // Receipt Published Date

            param = command.CreateParameter();
            param.ParameterName = "@IntrlData";
            param.Value = record.IntrlData;
            command.Parameters.Add(param);                  // Internal Data

            param = command.CreateParameter();
            param.ParameterName = "@RcptSign";
            param.Value = record.RcptSign;
            command.Parameters.Add(param);                  // Receipt Signature

            param = command.CreateParameter();
            param.ParameterName = "@Jrnl";
            param.Value = record.Jrnl;
            command.Parameters.Add(param);                  // Journal

            param = command.CreateParameter();
            param.ParameterName = "@TrdeNm";
            param.Value = record.TrdeNm;
            command.Parameters.Add(param);                  // Tradmark Name

            param = command.CreateParameter();
            param.ParameterName = "@Adrs";
            param.Value = record.Adrs;
            command.Parameters.Add(param);                  // Address

            param = command.CreateParameter();
            param.ParameterName = "@TopMsg";
            param.Value = record.TopMsg;
            command.Parameters.Add(param);                  // Top Message

            param = command.CreateParameter();
            param.ParameterName = "@BtmMsg";
            param.Value = record.BtmMsg;
            command.Parameters.Add(param);                  // Bottom Message

            param = command.CreateParameter();
            param.ParameterName = "@RptNo";
            param.Value = record.RptNo;
            command.Parameters.Add(param);                  // Receipt No.

            param = command.CreateParameter();
            param.ParameterName = "@RptDt";
            param.Value = record.RptDt;
            command.Parameters.Add(param);                  // Receipt Date

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@TaskId";
            //param.Value = record.TaskId;
            //command.Parameters.Add(param);                  // Task ID

            //param = command.CreateParameter();
            //param.ParameterName = "@TaskStrtDt";
            //param.Value = record.TaskStrtDt;
            //command.Parameters.Add(param);                  // Task Start Date

            //param = command.CreateParameter();
            //param.ParameterName = "@TaskEndDt";
            //param.Value = record.TaskEndDt;
            //command.Parameters.Add(param);                  // Task End Date

            //param = command.CreateParameter();
            //param.ParameterName = "@TaskCmptYn";
            //param.Value = record.TaskCmptYn;
            //command.Parameters.Add(param);                  // Task Completed(Y/N)

            //param = command.CreateParameter();
            //param.ParameterName = "@AudtFile";
            //param.Value = record.AudtFile;
            //command.Parameters.Add(param);                  // Audit File

            //param = command.CreateParameter();
            //param.ParameterName = "@AudtFileEcrt";
            //param.Value = record.AudtFileEcrt;
            //command.Parameters.Add(param);                  // Audit File Encryption

            //param = command.CreateParameter();
            //param.ParameterName = "@EbmSendDt";
            //param.Value = record.EbmSendDt;
            //command.Parameters.Add(param);                  // EBM Send Date

            //param = command.CreateParameter();
            //param.ParameterName = "@EbmRes";
            //param.Value = record.EbmRes;
            //command.Parameters.Add(param);                  // EBM Response

            //param = command.CreateParameter();
            //param.ParameterName = "@EbmResCd";
            //param.Value = record.EbmResCd;
            //command.Parameters.Add(param);                  // EBM Response Code

            //param = command.CreateParameter();
            //param.ParameterName = "@ScmSignData";
            //param.Value = record.ScmSignData;
            //command.Parameters.Add(param);                  // SCM Signature Date

            //param = command.CreateParameter();
            //param.ParameterName = "@ScmSign";
            //param.Value = record.ScmSign;
            //command.Parameters.Add(param);                  // SCM Signature

            //param = command.CreateParameter();
            //param.ParameterName = "@ScmSignCfm";
            //param.Value = record.ScmSignCfm;
            //command.Parameters.Add(param);                  // SCM Signature Confirmation

            //param = command.CreateParameter();
            //param.ParameterName = "@SdcDt";
            //param.Value = record.SdcDt;
            //command.Parameters.Add(param);                  // SDC Date

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record.RegDt;
            command.Parameters.Add(param);                  // Registered Date

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record.ModrId;
            command.Parameters.Add(param);                  // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record.ModrNm;
            command.Parameters.Add(param);                  // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record.ModDt;
            command.Parameters.Add(param);                  // Modified Date

            return true;
        }
    }
}

/*

insert into TRNS_SALE_RECEIPT ( 
       TIN, 
       BHF_ID, 
       INVC_NO, 
       PRCHR_ACPTC_YN, 
       ORG_INVC_NO, 
       CUR_RCPT_NO, 
       TOT_RCPT_NO, 
       TAXPR_NM, 
       RCPT_PBCT_DT, 
       INTRL_DATA, 
       RCPT_SIGN, 
       JRNL, 
       TRDE_NM, 
       ADRS, 
       TOP_MSG, 
       BTM_MSG, 
       RPT_NO, 
       RPT_DT, 
       TASK_ID, 
       TASK_STRT_DT, 
       TASK_END_DT, 
       TASK_CMPT_YN, 
       AUDT_FILE, 
       AUDT_FILE_ECRT, 
       EBM_SEND_DT, 
       EBM_RES, 
       EBM_RES_CD, 
       SCM_SIGN_DATA, 
       SCM_SIGN, 
       SCM_SIGN_CFM, 
       SDC_DT, 
       REGR_ID, 
       REGR_NM, 
       REG_DT, 
       MODR_ID, 
       MODR_NM, 
       MOD_DT
) 					
select     
       '000000000' as TIN, 
       BHF_ID as BHF_ID, 
       INV_ID as INVC_NO, 
       'N' as PRCHR_ACPTC_YN, 
       '' as ORG_INVC_NO, 
       SDC_RCPT_NO as CUR_RCPT_NO, 
       TOT_SDC_RCPT_NO as TOT_RCPT_NO, 
       BCNC_ID as TAXPR_NM, 
       VALID_DT as RCPT_PBCT_DT, 
       INTERNAL_DATA as INTRL_DATA, 
       SIGNATURE as RCPT_SIGN, 
       JOURNAL as JRNL, 
       '' as TRDE_NM, 
       '' as ADRS, 
       '' as TOP_MSG, 
       '' as BTM_MSG, 
       RPT_NO as RPT_NO, 
       VALID_DT as RPT_DT, 
       '' as TASK_ID, 
       REG_DT as TASK_STRT_DT, 
       SND_DT as TASK_END_DT, 
       'N' as TASK_CMPT_YN, 
       '' as AUDT_FILE, 
       '' as AUDT_FILE_ECRT, 
       SND_DT as EBM_SEND_DT, 
       '' as EBM_RES, 
       '' as EBM_RES_CD, 
       '' as SCM_SIGN_DATA, 
       '' as SCM_SIGN, 
       '' as SCM_SIGN_CFM, 
       REG_DT as SDC_DT, 
       REGUSR_ID as REGR_ID, 
       REGUSR_NM as REGR_NM, 
       REG_DT as REG_DT, 
       REGUSR_ID as MODR_ID, 
       REGUSR_NM as MODR_NM, 
       REG_DT as MOD_DT
  from TRNRECEIPT 

 */

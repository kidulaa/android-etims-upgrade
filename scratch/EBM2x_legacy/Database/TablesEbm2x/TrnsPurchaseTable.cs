using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TrnsPurchaseTable.
    /// </summary>
    public class TrnsPurchaseTable
    {
        public TrnsPurchaseTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TRNS_PURCHASE ( "); // 거래 구매
            sql.Append("       TIN                CHAR(9)             not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)             not null , ");      // Branch Office ID
            sql.Append("       SPPLR_TIN          CHAR(9)             not null , ");      // Cupplier Taxpayer Identification Number(TIN)
            sql.Append("       INVC_NO            INT                 not null , ");      // Invoice No.
            sql.Append("       ORG_INVC_NO        INT                 null , ");          // Original Invoice No.
            sql.Append("       TAXPR_NM           VARCHAR(60)         null , ");          // Taxpayer's Name
            //JCNA 202001 DELETE sql.Append("       DVC_ID             VARCHAR(16)         null , ");          // Device ID
            sql.Append("       SPPLR_BHF_ID       CHAR(2)             null , ");          // Supplier Branch Offie ID
            sql.Append("       SPPLR_NM           VARCHAR(60)         null , ");          // Customer Name
            //JCNA 202001 DELETE sql.Append("       SPPLR_DVC_ID       VARCHAR(16)         null , ");          // Supplier Device ID
            sql.Append("       SPPLR_INVC_NO      INT                 null , ");          // Supplier Receipt No.
            sql.Append("       REG_TY_CD          VARCHAR(5)          null , ");          // Registration Type Code
            sql.Append("       PCHS_TY_CD         VARCHAR(5)          null , ");          // Purchase Type Code
            sql.Append("       RCPT_TY_CD         VARCHAR(5)          null , ");          // Receipt Type Code
            sql.Append("       PMT_TY_CD          VARCHAR(5)          null , ");          // Payment Type Code
            sql.Append("       PCHS_STTS_CD       VARCHAR(5)          null , ");          // Purchase Status Code
            sql.Append("       CFM_DT             CHAR(8)             null , ");          // Confirmed Date
            sql.Append("       PCHS_DT            CHAR(8)             null , ");          // Purchased Date
            sql.Append("       WRHS_DT            CHAR(8)             null , ");          // Warehousing Date
            sql.Append("       CNCL_REQ_DT        CHAR(8)             null , ");          // Cancel Requested Date
            sql.Append("       CNCL_DT            CHAR(8)             null , ");          // Canceled Date
            sql.Append("       RFD_DT             CHAR(8)             null , ");          // Refunded Date
            sql.Append("       TOT_ITEM_CNT       INT                 null , ");          // Total Item Count
            sql.Append("       TAXBL_AMT_A        DECIMAL(18,2)       null , ");          // Taxable Amount A
            sql.Append("       TAXBL_AMT_B        DECIMAL(18,2)       null , ");          // Taxable Amount B
            sql.Append("       TAXBL_AMT_C        DECIMAL(18,2)       null , ");          // Taxable Amount C
            sql.Append("       TAXBL_AMT_D        DECIMAL(18,2)       null , ");          // Taxable Amount D
            sql.Append("       TAX_RT_A           INT                 null , ");          // Tax Rate A
            sql.Append("       TAX_RT_B           INT                 null , ");          // Tax Rate B
            sql.Append("       TAX_RT_C           INT                 null , ");          // Tax Rate C
            sql.Append("       TAX_RT_D           INT                 null , ");          // Tax Rate D
            sql.Append("       TAX_AMT_A          DECIMAL(18,2)       null , ");          // Tax Amount A
            sql.Append("       TAX_AMT_B          DECIMAL(18,2)       null , ");          // Tax Amount B
            sql.Append("       TAX_AMT_C          DECIMAL(18,2)       null , ");          // Tax Amount C
            sql.Append("       TAX_AMT_D          DECIMAL(18,2)       null , ");          // Tax Amount D
            sql.Append("       TOT_TAXBL_AMT      DECIMAL(18,2)       null , ");          // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT        DECIMAL(18,2)       null , ");          // Total Tax Amount
            sql.Append("       TOT_AMT            DECIMAL(18,2)       null , ");          // Total Amount
            sql.Append("       REMARK             VARCHAR(400)        null , ");          // Remark
            sql.Append("       REGR_ID            VARCHAR(20)         null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)         null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)         null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)         null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)         null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)         null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID, SPPLR_TIN, INVC_NO ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       SPPLR_TIN, ");               // Cupplier Taxpayer Identification Number(TIN)
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       ORG_INVC_NO, ");             // Original Invoice No.
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            //JCNA 202001 DELETE sql.Append("       DVC_ID, ");                  // Device ID
            sql.Append("       SPPLR_BHF_ID, ");            // Supplier Branch Offie ID
            sql.Append("       SPPLR_NM, ");                // Customer Name
            //JCNA 202001 DELETE sql.Append("       SPPLR_DVC_ID, ");            // Supplier Device ID
            sql.Append("       SPPLR_INVC_NO, ");           // Supplier Receipt No.
            sql.Append("       REG_TY_CD, ");               // Registration Type Code
            sql.Append("       PCHS_TY_CD, ");              // Purchase Type Code
            sql.Append("       RCPT_TY_CD, ");              // Receipt Type Code
            sql.Append("       PMT_TY_CD, ");               // Payment Type Code
            sql.Append("       PCHS_STTS_CD, ");            // Purchase Status Code
            sql.Append("       CFM_DT, ");                  // Confirmed Date
            sql.Append("       PCHS_DT, ");                 // Purchased Date
            sql.Append("       WRHS_DT, ");                 // Warehousing Date
            sql.Append("       CNCL_REQ_DT, ");             // Cancel Requested Date
            sql.Append("       CNCL_DT, ");                 // Canceled Date
            sql.Append("       RFD_DT, ");                  // Refunded Date
            sql.Append("       TOT_ITEM_CNT, ");            // Total Item Count
            sql.Append("       TAXBL_AMT_A, ");             // Taxable Amount A
            sql.Append("       TAXBL_AMT_B, ");             // Taxable Amount B
            sql.Append("       TAXBL_AMT_C, ");             // Taxable Amount C
            sql.Append("       TAXBL_AMT_D, ");             // Taxable Amount D
            sql.Append("       TAX_RT_A, ");                // Tax Rate A
            sql.Append("       TAX_RT_B, ");                // Tax Rate B
            sql.Append("       TAX_RT_C, ");                // Tax Rate C
            sql.Append("       TAX_RT_D, ");                // Tax Rate D
            sql.Append("       TAX_AMT_A, ");               // Tax Amount A
            sql.Append("       TAX_AMT_B, ");               // Tax Amount B
            sql.Append("       TAX_AMT_C, ");               // Tax Amount C
            sql.Append("       TAX_AMT_D, ");               // Tax Amount D
            sql.Append("       TOT_TAXBL_AMT, ");           // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT, ");             // Total Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT,  ");                  // Modified Date
            sql.Append("       A.rowid  ");                  // Modified Date
            sql.Append("  from TRNS_PURCHASE A "); // 거래 구매
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TRNS_PURCHASE ( "); // 거래 구매
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       SPPLR_TIN, ");               // Cupplier Taxpayer Identification Number(TIN)
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       ORG_INVC_NO, ");             // Original Invoice No.
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            //JCNA 202001 DELETE sql.Append("       DVC_ID, ");                  // Device ID
            sql.Append("       SPPLR_BHF_ID, ");            // Supplier Branch Offie ID
            sql.Append("       SPPLR_NM, ");                // Customer Name
            //JCNA 202001 DELETE sql.Append("       SPPLR_DVC_ID, ");            // Supplier Device ID
            sql.Append("       SPPLR_INVC_NO, ");           // Supplier Receipt No.
            sql.Append("       REG_TY_CD, ");               // Registration Type Code
            sql.Append("       PCHS_TY_CD, ");              // Purchase Type Code
            sql.Append("       RCPT_TY_CD, ");              // Receipt Type Code
            sql.Append("       PMT_TY_CD, ");               // Payment Type Code
            sql.Append("       PCHS_STTS_CD, ");            // Purchase Status Code
            sql.Append("       CFM_DT, ");                  // Confirmed Date
            sql.Append("       PCHS_DT, ");                 // Purchased Date
            sql.Append("       WRHS_DT, ");                 // Warehousing Date
            sql.Append("       CNCL_REQ_DT, ");             // Cancel Requested Date
            sql.Append("       CNCL_DT, ");                 // Canceled Date
            sql.Append("       RFD_DT, ");                  // Refunded Date
            sql.Append("       TOT_ITEM_CNT, ");            // Total Item Count
            sql.Append("       TAXBL_AMT_A, ");             // Taxable Amount A
            sql.Append("       TAXBL_AMT_B, ");             // Taxable Amount B
            sql.Append("       TAXBL_AMT_C, ");             // Taxable Amount C
            sql.Append("       TAXBL_AMT_D, ");             // Taxable Amount D
            sql.Append("       TAX_RT_A, ");                // Tax Rate A
            sql.Append("       TAX_RT_B, ");                // Tax Rate B
            sql.Append("       TAX_RT_C, ");                // Tax Rate C
            sql.Append("       TAX_RT_D, ");                // Tax Rate D
            sql.Append("       TAX_AMT_A, ");               // Tax Amount A
            sql.Append("       TAX_AMT_B, ");               // Tax Amount B
            sql.Append("       TAX_AMT_C, ");               // Tax Amount C
            sql.Append("       TAX_AMT_D, ");               // Tax Amount D
            sql.Append("       TOT_TAXBL_AMT, ");           // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT, ");             // Total Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @SpplrTin, ");               // Cupplier Taxpayer Identification Number(TIN)
            sql.Append("       @InvcNo, ");                 // Invoice No.
            sql.Append("       @OrgInvcNo, ");              // Original Invoice No.
            sql.Append("       @TaxprNm, ");                // Taxpayer's Name
            //JCNA 202001 DELETE sql.Append("       @DvcId, ");                  // Device ID
            sql.Append("       @SpplrBhfId, ");             // Supplier Branch Offie ID
            sql.Append("       @SpplrNm, ");                // Customer Name
            //JCNA 202001 DELETE sql.Append("       @SpplrDvcId, ");             // Supplier Device ID
            sql.Append("       @SpplrInvcNo, ");            // Supplier Receipt No.
            sql.Append("       @RegTyCd, ");                // Registration Type Code
            sql.Append("       @PchsTyCd, ");               // Purchase Type Code
            sql.Append("       @RcptTyCd, ");               // Receipt Type Code
            sql.Append("       @PmtTyCd, ");                // Payment Type Code
            sql.Append("       @PchsSttsCd, ");             // Purchase Status Code
            sql.Append("       @CfmDt, ");                  // Confirmed Date
            sql.Append("       @PchsDt, ");                 // Purchased Date
            sql.Append("       @WrhsDt, ");                 // Warehousing Date
            sql.Append("       @CnclReqDt, ");              // Cancel Requested Date
            sql.Append("       @CnclDt, ");                 // Canceled Date
            sql.Append("       @RfdDt, ");                  // Refunded Date
            sql.Append("       @TotItemCnt, ");             // Total Item Count
            sql.Append("       @TaxblAmtA, ");              // Taxable Amount A
            sql.Append("       @TaxblAmtB, ");              // Taxable Amount B
            sql.Append("       @TaxblAmtC, ");              // Taxable Amount C
            sql.Append("       @TaxblAmtD, ");              // Taxable Amount D
            //sql.Append("       @TaxblAmtE, ");              // Taxable Amount E
            sql.Append("       @TaxRtA, ");                 // Tax Rate A
            sql.Append("       @TaxRtB, ");                 // Tax Rate B
            sql.Append("       @TaxRtC, ");                 // Tax Rate C
            sql.Append("       @TaxRtD, ");                 // Tax Rate D
           // sql.Append("       @TaxRtE, ");                 // Tax Rate E
            sql.Append("       @TaxAmtA, ");                // Tax Amount A
            sql.Append("       @TaxAmtB, ");                // Tax Amount B
            sql.Append("       @TaxAmtC, ");                // Tax Amount C
            sql.Append("       @TaxAmtD, ");                // Tax Amount D
            //sql.Append("       @TaxAmtE, ");                // Tax Amount E
            sql.Append("       @TotTaxblAmt, ");            // Total Taxable Amount
            sql.Append("       @TotTaxAmt, ");              // Total Tax Amount
            sql.Append("       @TotAmt, ");                 // Total Amount
            sql.Append("       @Remark, ");                 // Remark
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
            sql.Append("update TRNS_PURCHASE "); // 거래 구매
            sql.Append("   set ORG_INVC_NO = @OrgInvcNo, ");  // Original Invoice No.
            sql.Append("       TAXPR_NM = @TaxprNm, ");     // Taxpayer's Name
            //JCNA 202001 DELETE sql.Append("       DVC_ID = @DvcId, ");         // Device ID
            sql.Append("       SPPLR_BHF_ID = @SpplrBhfId, ");  // Supplier Branch Offie ID
            sql.Append("       SPPLR_NM = @SpplrNm, ");     // Customer Name
            //JCNA 202001 DELETE sql.Append("       SPPLR_DVC_ID = @SpplrDvcId, ");  // Supplier Device ID
            sql.Append("       SPPLR_INVC_NO = @SpplrInvcNo, ");  // Supplier Receipt No.
            sql.Append("       REG_TY_CD = @RegTyCd, ");    // Registration Type Code
            sql.Append("       PCHS_TY_CD = @PchsTyCd, ");  // Purchase Type Code
            sql.Append("       RCPT_TY_CD = @RcptTyCd, ");  // Receipt Type Code
            sql.Append("       PMT_TY_CD = @PmtTyCd, ");    // Payment Type Code
            sql.Append("       PCHS_STTS_CD = @PchsSttsCd, ");  // Purchase Status Code
            sql.Append("       CFM_DT = @CfmDt, ");         // Confirmed Date
            sql.Append("       PCHS_DT = @PchsDt, ");       // Purchased Date
            sql.Append("       WRHS_DT = @WrhsDt, ");       // Warehousing Date
            sql.Append("       CNCL_REQ_DT = @CnclReqDt, ");  // Cancel Requested Date
            sql.Append("       CNCL_DT = @CnclDt, ");       // Canceled Date
            sql.Append("       RFD_DT = @RfdDt, ");         // Refunded Date
            sql.Append("       TOT_ITEM_CNT = @TotItemCnt, ");  // Total Item Count
            sql.Append("       TAXBL_AMT_A = @TaxblAmtA, ");  // Taxable Amount A
            sql.Append("       TAXBL_AMT_B = @TaxblAmtB, ");  // Taxable Amount B
            sql.Append("       TAXBL_AMT_C = @TaxblAmtC, ");  // Taxable Amount C
            sql.Append("       TAXBL_AMT_D = @TaxblAmtD, ");  // Taxable Amount D
            //sql.Append("       TAXBL_AMT_E = @TaxblAmtE, ");  // Taxable Amount E
            sql.Append("       TAX_RT_A = @TaxRtA, ");      // Tax Rate A
            sql.Append("       TAX_RT_B = @TaxRtB, ");      // Tax Rate B
            sql.Append("       TAX_RT_C = @TaxRtC, ");      // Tax Rate C
            sql.Append("       TAX_RT_D = @TaxRtD, ");      // Tax Rate D
            //sql.Append("       TAX_RT_E = @TaxRtE, ");      // Tax Rate E
            sql.Append("       TAX_AMT_A = @TaxAmtA, ");    // Tax Amount A
            sql.Append("       TAX_AMT_B = @TaxAmtB, ");    // Tax Amount B
            sql.Append("       TAX_AMT_C = @TaxAmtC, ");    // Tax Amount C
            sql.Append("       TAX_AMT_D = @TaxAmtD, ");    // Tax Amount D
            //sql.Append("       TAX_AMT_E = @TaxAmtE, ");    // Tax Amount E
            sql.Append("       TOT_TAXBL_AMT = @TotTaxblAmt, ");  // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT = @TotTaxAmt, ");  // Total Tax Amount
            sql.Append("       TOT_AMT = @TotAmt, ");       // Total Amount
            sql.Append("       REMARK = @Remark, ");        // Remark
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and SPPLR_TIN = @SpplrTin ");  // Cupplier Taxpayer Identification Number(TIN)
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            return sql.ToString();
        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TRNS_PURCHASE "); // 거래 구매
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and SPPLR_TIN = @SpplrTin ");  // Cupplier Taxpayer Identification Number(TIN)
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TrnsPurchaseRecord record)
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
            param.ParameterName = "@SpplrTin";
            param.Value = record.SpplrTin;
            command.Parameters.Add(param);                  // Cupplier Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@InvcNo";
            param.Value = record.InvcNo;
            command.Parameters.Add(param);                  // Invoice No.

            param = command.CreateParameter();
            param.ParameterName = "@OrgInvcNo";
            param.Value = record.OrgInvcNo;
            command.Parameters.Add(param);                  // Original Invoice No.

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = record.TaxprNm;
            command.Parameters.Add(param);                  // Taxpayer's Name

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@DvcId";
            //param.Value = record.DvcId;
            //command.Parameters.Add(param);                  // Device ID

            param = command.CreateParameter();
            param.ParameterName = "@SpplrBhfId";
            param.Value = record.SpplrBhfId;
            command.Parameters.Add(param);                  // Supplier Branch Offie ID

            param = command.CreateParameter();
            param.ParameterName = "@SpplrNm";
            param.Value = record.SpplrNm;
            command.Parameters.Add(param);                  // Customer Name

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@SpplrDvcId";
            //param.Value = record.SpplrDvcId;
            //command.Parameters.Add(param);                  // Supplier Device ID

            param = command.CreateParameter();
            param.ParameterName = "@SpplrInvcNo";
            param.Value = record.SpplrInvcNo;
            command.Parameters.Add(param);                  // Supplier Receipt No.

            param = command.CreateParameter();
            param.ParameterName = "@RegTyCd";
            param.Value = record.RegTyCd;
            command.Parameters.Add(param);                  // Registration Type Code

            param = command.CreateParameter();
            param.ParameterName = "@PchsTyCd";
            param.Value = record.PchsTyCd;
            command.Parameters.Add(param);                  // Purchase Type Code

            param = command.CreateParameter();
            param.ParameterName = "@RcptTyCd";
            param.Value = record.RcptTyCd;
            command.Parameters.Add(param);                  // Receipt Type Code

            param = command.CreateParameter();
            param.ParameterName = "@PmtTyCd";
            param.Value = record.PmtTyCd;
            command.Parameters.Add(param);                  // Payment Type Code

            param = command.CreateParameter();
            param.ParameterName = "@PchsSttsCd";
            param.Value = record.PchsSttsCd;
            command.Parameters.Add(param);                  // Purchase Status Code

            param = command.CreateParameter();
            param.ParameterName = "@CfmDt";
            param.Value = record.CfmDt;
            command.Parameters.Add(param);                  // Confirmed Date

            param = command.CreateParameter();
            param.ParameterName = "@PchsDt";
            param.Value = record.PchsDt;
            command.Parameters.Add(param);                  // Purchased Date

            param = command.CreateParameter();
            param.ParameterName = "@WrhsDt";
            param.Value = record.WrhsDt;
            command.Parameters.Add(param);                  // Warehousing Date

            param = command.CreateParameter();
            param.ParameterName = "@CnclReqDt";
            param.Value = record.CnclReqDt;
            command.Parameters.Add(param);                  // Cancel Requested Date

            param = command.CreateParameter();
            param.ParameterName = "@CnclDt";
            param.Value = record.CnclDt;
            command.Parameters.Add(param);                  // Canceled Date

            param = command.CreateParameter();
            param.ParameterName = "@RfdDt";
            param.Value = record.RfdDt;
            command.Parameters.Add(param);                  // Refunded Date

            param = command.CreateParameter();
            param.ParameterName = "@TotItemCnt";
            param.Value = record.TotItemCnt;
            command.Parameters.Add(param);                  // Total Item Count

            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmtA";
            param.Value = record.TaxblAmtA;
            command.Parameters.Add(param);                  // Taxable Amount A

            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmtB";
            param.Value = record.TaxblAmtB;
            command.Parameters.Add(param);                  // Taxable Amount B

            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmtC";
            param.Value = record.TaxblAmtC;
            command.Parameters.Add(param);                  // Taxable Amount C

            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmtD";
            param.Value = record.TaxblAmtD;
            command.Parameters.Add(param);                  // Taxable Amount D


            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmtE";
            param.Value = record.TaxblAmtE;
            command.Parameters.Add(param);                  // Taxable Amount E

            param = command.CreateParameter();
            param.ParameterName = "@TaxRtA";
            param.Value = record.TaxRtA;
            command.Parameters.Add(param);                  // Tax Rate A

            param = command.CreateParameter();
            param.ParameterName = "@TaxRtB";
            param.Value = record.TaxRtB;
            command.Parameters.Add(param);                  // Tax Rate B

            param = command.CreateParameter();
            param.ParameterName = "@TaxRtC";
            param.Value = record.TaxRtC;
            command.Parameters.Add(param);                  // Tax Rate C

            param = command.CreateParameter();
            param.ParameterName = "@TaxRtD";
            param.Value = record.TaxRtD;
            command.Parameters.Add(param);                  // Tax Rate D

            param = command.CreateParameter();
            param.ParameterName = "@TaxRtE";
            param.Value = record.TaxRtE;
            command.Parameters.Add(param);                  // Tax Rate E

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmtA";
            param.Value = record.TaxAmtA;
            command.Parameters.Add(param);                  // Tax Amount A

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmtB";
            param.Value = record.TaxAmtB;
            command.Parameters.Add(param);                  // Tax Amount B

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmtC";
            param.Value = record.TaxAmtC;
            command.Parameters.Add(param);                  // Tax Amount C

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmtD";
            param.Value = record.TaxAmtD;
            command.Parameters.Add(param);                  // Tax Amount D

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmtE";
            param.Value = record.TaxAmtE;
            command.Parameters.Add(param);                  // Tax Amount E

            param = command.CreateParameter();
            param.ParameterName = "@TotTaxblAmt";
            param.Value = record.TotTaxblAmt;
            command.Parameters.Add(param);                  // Total Taxable Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotTaxAmt";
            param.Value = record.TotTaxAmt;
            command.Parameters.Add(param);                  // Total Tax Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotAmt";
            param.Value = record.TotAmt;
            command.Parameters.Add(param);                  // Total Amount

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = record.Remark;
            command.Parameters.Add(param);                  // Remark

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
insert into TRNS_PURCHASE ( 
       TIN, 
       BHF_ID, 
       SPPLR_TIN, 
       INVC_NO, 
       ORG_INVC_NO, 
       TAXPR_NM, 
       DVC_ID, 
       SPPLR_BHF_ID, 
       SPPLR_NM, 
       SPPLR_DVC_ID, 
       SPPLR_RCPT_NO, 
       REG_TY_CD, 
       PCHS_TY_CD, 
       RCPT_TY_CD, 
       PMT_TY_CD, 
       PCHS_STTS_CD, 
       CFM_DT, 
       PCHS_DT, 
       WRHS_DT, 
       CNCL_REQ_DT, 
       CNCL_DT, 
       RFD_DT, 
       TOT_ITEM_CNT, 
       TAXBL_AMT_A, 
       TAXBL_AMT_B, 
       TAXBL_AMT_C, 
       TAXBL_AMT_D, 
       TAX_RT_A, 
       TAX_RT_B, 
       TAX_RT_C, 
       TAX_RT_D, 
       TAX_AMT_A, 
       TAX_AMT_B, 
       TAX_AMT_C, 
       TAX_AMT_D, 
       TOT_TAXBL_AMT, 
       TOT_TAX_AMT, 
       TOT_AMT, 
       REMARK, 
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
       BCNC_ID as SPPLR_TIN, 
       INV_ID as INVC_NO, 
       '' as ORG_INVC_NO, 
       BCNC_ID as TAXPR_NM, 
       '01' as DVC_ID, 
       '00' as SPPLR_BHF_ID, 
       BCNC_ID as SPPLR_NM, 
       '00' as SPPLR_DVC_ID, 
       0 as SPPLR_RCPT_NO, 
       REG_TY_CD as REG_TY_CD, 
       '00' as PCHS_TY_CD, 
       '00' as RCPT_TY_CD, 
       PAY_TY_CD as PMT_TY_CD, 
       INV_STATUS_CD as PCHS_STTS_CD, 
       VALID_DT as CFM_DT, 
       OCDE as PCHS_DT, 
       VALID_DT as WRHS_DT, 
       CANCEL_REQ_DT as CNCL_REQ_DT, 
       CANCEL_DT as CNCL_DT, 
       REFUND_DT as RFD_DT, 
       TOT_NUM_ITEM as TOT_ITEM_CNT, 
       TOT_TAXABL_AMT_A as TAXBL_AMT_A, 
       TOT_TAXABL_AMT_B as TAXBL_AMT_B, 
       TOT_TAXABL_AMT_C as TAXBL_AMT_C, 
       TOT_TAXABL_AMT_D as TAXBL_AMT_D, 
       0 as TAX_RT_A, 
       0 as TAX_RT_B, 
       0 as TAX_RT_C, 
       0 as TAX_RT_D, 
       TOT_TAX_A as TAX_AMT_A, 
       TOT_TAX_B as TAX_AMT_B, 
       TOT_TAX_C as TAX_AMT_C, 
       TOT_TAX_D as TAX_AMT_D, 
       TOT_SPLPC as TOT_TAXBL_AMT, 
       TOT_TAX as TOT_TAX_AMT, 
       TOT_AMT as TOT_AMT, 
       REMARK as REMARK, 
       REGUSR_ID as REGR_ID, 
       REGUSR_ID as REGR_NM, 
       REG_DT as REG_DT, 
       REGUSR_ID as MODR_ID, 
       REGUSR_ID as MODR_NM, 
       REG_DT as MOD_DT
  from TRNPURCHASE
  */

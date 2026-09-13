using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerBaseTable.
    /// </summary>
    public class TaxpayerBaseTable
    {
        public TaxpayerBaseTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TAXPAYER_BASE ( "); 
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM           VARCHAR(60)        not null , ");      // Taxpayer's Name
            sql.Append("       TAXPR_STTS_CD      VARCHAR(5)         not null , ");      // Taxpayer Status Code
            sql.Append("       BSNS_ACTV          VARCHAR(100)       null , ");          // Business Activities
            sql.Append("       PRVNC_NM           VARCHAR(100)       null , ");          // Province No.
            sql.Append("       DSTRT_NM           VARCHAR(100)       null , ");          // District No.
            sql.Append("       SCTR_NM            VARCHAR(100)        null , ");          // Sector No.
            sql.Append("       LOC_DESC           VARCHAR(100)       null , ");          // Location Description
            sql.Append("       TEL_NO             VARCHAR(20)        not null , ");      // Telephone number
            sql.Append("       EMAIL              VARCHAR(100)       null , ");          // Email
            sql.Append("       BANK_CD            VARCHAR(5)         null , ");          // Bank Code
            sql.Append("       BANK_ACCNT_NO      VARCHAR(100)       null , ");          // Bank Account Number
            sql.Append("       BANK_ACCNT_HLDR    VARCHAR(60)        null , ");          // Bank Account Holder
            sql.Append("       APCNT_NM           VARCHAR(60)        not null , ");      // Applicant name
            sql.Append("       APCNT_TELNO        VARCHAR(20)        not null , ");      // Applicant telephone number
            sql.Append("       APCNT_EMAIL        VARCHAR(50)        null , ");          // Applicant Email
            sql.Append("       REMARK             VARCHAR(400)       null , ");          // Remark
            sql.Append("       EBM_TY_CD          VARCHAR(5)         null , ");          // EBM Type Code
            sql.Append("       USER_NO            INT                null , ");          // User No.
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( TIN ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            sql.Append("       TAXPR_STTS_CD, ");           // Taxpayer Status Code
            sql.Append("       BSNS_ACTV, ");               // Business Activities
            sql.Append("       PRVNC_NM, ");                // Province No.
            sql.Append("       DSTRT_NM, ");                // District No.
            sql.Append("       SCTR_NM, ");                 // Sector No.
            sql.Append("       LOC_DESC, ");                // Location Description
            sql.Append("       TEL_NO, ");                  // Telephone number
            sql.Append("       EMAIL, ");                   // Email
            sql.Append("       BANK_CD, ");                 // Bank Code
            sql.Append("       BANK_ACCNT_NO, ");           // Bank Account Number
            sql.Append("       BANK_ACCNT_HLDR, ");         // Bank Account Holder
            sql.Append("       APCNT_NM, ");                // Applicant name
            sql.Append("       APCNT_TELNO, ");             // Applicant telephone number
            sql.Append("       APCNT_EMAIL, ");             // Applicant Email
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       EBM_TY_CD, ");               // EBM Type Code
            sql.Append("       USER_NO, ");                 // User No.
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from TAXPAYER_BASE "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TAXPAYER_BASE ( ");
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            sql.Append("       TAXPR_STTS_CD, ");           // Taxpayer Status Code
            sql.Append("       BSNS_ACTV, ");               // Business Activities
            sql.Append("       PRVNC_NM, ");                // Province No.
            sql.Append("       DSTRT_NM, ");                // District No.
            sql.Append("       SCTR_NM, ");                 // Sector No.
            sql.Append("       LOC_DESC, ");                // Location Description
            sql.Append("       TEL_NO, ");                  // Telephone number
            sql.Append("       EMAIL, ");                   // Email
            sql.Append("       BANK_CD, ");                 // Bank Code
            sql.Append("       BANK_ACCNT_NO, ");           // Bank Account Number
            sql.Append("       BANK_ACCNT_HLDR, ");         // Bank Account Holder
            sql.Append("       APCNT_NM, ");                // Applicant name
            sql.Append("       APCNT_TELNO, ");             // Applicant telephone number
            sql.Append("       APCNT_EMAIL, ");             // Applicant Email
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       EBM_TY_CD, ");               // EBM Type Code
            sql.Append("       USER_NO, ");                 // User No.
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @TaxprNm, ");                // Taxpayer's Name
            sql.Append("       @TaxprSttsCd, ");            // Taxpayer Status Code
            sql.Append("       @BsnsActv, ");               // Business Activities
            sql.Append("       @PrvncNm, ");                // Province No.
            sql.Append("       @DstrtNm, ");                // District No.
            sql.Append("       @SctrNm, ");                 // Sector No.
            sql.Append("       @LocDesc, ");                // Location Description
            sql.Append("       @TelNo, ");                  // Telephone number
            sql.Append("       @Email, ");                  // Email
            sql.Append("       @BankCd, ");                 // Bank Code
            sql.Append("       @BankAccntNo, ");            // Bank Account Number
            sql.Append("       @BankAccntHldr, ");          // Bank Account Holder
            sql.Append("       @ApcntNm, ");                // Applicant name
            sql.Append("       @ApcntTelno, ");             // Applicant telephone number
            sql.Append("       @ApcntEmail, ");             // Applicant Email
            sql.Append("       @Remark, ");                 // Remark
            sql.Append("       @EbmTyCd, ");                // EBM Type Code
            sql.Append("       @UserNo, ");                 // User No.
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
            sql.Append("update TAXPAYER_BASE ");
            sql.Append("   set TAXPR_NM = @TaxprNm, ");     // Taxpayer's Name
            sql.Append("       TAXPR_STTS_CD = @TaxprSttsCd, ");  // Taxpayer Status Code
            sql.Append("       BSNS_ACTV = @BsnsActv, ");   // Business Activities
            sql.Append("       PRVNC_NM = @PrvncNm, ");     // Province No.
            sql.Append("       DSTRT_NM = @DstrtNm, ");     // District No.
            sql.Append("       SCTR_NM = @SctrNm, ");       // Sector No.
            sql.Append("       LOC_DESC = @LocDesc, ");     // Location Description
            sql.Append("       TEL_NO = @TelNo, ");         // Telephone number
            sql.Append("       EMAIL = @Email, ");          // Email
            sql.Append("       BANK_CD = @BankCd, ");       // Bank Code
            sql.Append("       BANK_ACCNT_NO = @BankAccntNo, ");  // Bank Account Number
            sql.Append("       BANK_ACCNT_HLDR = @BankAccntHldr, ");  // Bank Account Holder
            sql.Append("       APCNT_NM = @ApcntNm, ");     // Applicant name
            sql.Append("       APCNT_TELNO = @ApcntTelno, ");  // Applicant telephone number
            sql.Append("       APCNT_EMAIL = @ApcntEmail, ");  // Applicant Email
            sql.Append("       REMARK = @Remark, ");        // Remark
            sql.Append("       EBM_TY_CD = @EbmTyCd, ");    // EBM Type Code
            sql.Append("       USER_NO = @UserNo, ");       // User No.
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt ");          // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TAXPAYER_BASE "); 
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerBaseRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = record.TaxprNm;
            command.Parameters.Add(param);                  // Taxpayer's Name

            param = command.CreateParameter();
            param.ParameterName = "@TaxprSttsCd";
            param.Value = record.TaxprSttsCd;
            command.Parameters.Add(param);                  // Taxpayer Status Code

            param = command.CreateParameter();
            param.ParameterName = "@BsnsActv";
            param.Value = record.BsnsActv;
            command.Parameters.Add(param);                  // Business Activities

            param = command.CreateParameter();
            param.ParameterName = "@PrvncNm";
            param.Value = record.PrvncNm;
            command.Parameters.Add(param);                  // Province No.

            param = command.CreateParameter();
            param.ParameterName = "@DstrtNm";
            param.Value = record.DstrtNm;
            command.Parameters.Add(param);                  // District No.

            param = command.CreateParameter();
            param.ParameterName = "@SctrNm";
            param.Value = record.SctrNm;
            command.Parameters.Add(param);                  // Sector No.

            param = command.CreateParameter();
            param.ParameterName = "@LocDesc";
            param.Value = record.LocDesc;
            command.Parameters.Add(param);                  // Location Description

            param = command.CreateParameter();
            param.ParameterName = "@TelNo";
            param.Value = record.TelNo;
            command.Parameters.Add(param);                  // Telephone number

            param = command.CreateParameter();
            param.ParameterName = "@Email";
            param.Value = record.Email;
            command.Parameters.Add(param);                  // Email

            param = command.CreateParameter();
            param.ParameterName = "@BankCd";
            param.Value = record.BankCd;
            command.Parameters.Add(param);                  // Bank Code

            param = command.CreateParameter();
            param.ParameterName = "@BankAccntNo";
            param.Value = record.BankAccntNo;
            command.Parameters.Add(param);                  // Bank Account Number

            param = command.CreateParameter();
            param.ParameterName = "@BankAccntHldr";
            param.Value = record.BankAccntHldr;
            command.Parameters.Add(param);                  // Bank Account Holder

            param = command.CreateParameter();
            param.ParameterName = "@ApcntNm";
            param.Value = record.ApcntNm;
            command.Parameters.Add(param);                  // Applicant name

            param = command.CreateParameter();
            param.ParameterName = "@ApcntTelno";
            param.Value = record.ApcntTelno;
            command.Parameters.Add(param);                  // Applicant telephone number

            param = command.CreateParameter();
            param.ParameterName = "@ApcntEmail";
            param.Value = record.ApcntEmail;
            command.Parameters.Add(param);                  // Applicant Email

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = record.Remark;
            command.Parameters.Add(param);                  // Remark

            param = command.CreateParameter();
            param.ParameterName = "@EbmTyCd";
            param.Value = record.EbmTyCd;
            command.Parameters.Add(param);                  // EBM Type Code

            param = command.CreateParameter();
            param.ParameterName = "@UserNo";
            param.Value = record.UserNo;
            command.Parameters.Add(param);                  // User No.

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
        public bool SetParametersSDC(IDbCommand command, CustomerTin record, TaxpayerBaseRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = (string.IsNullOrEmpty(record.taxprNm)) ? "" : record.taxprNm;
            command.Parameters.Add(param);                  // Taxpayer's Name

            param = command.CreateParameter();
            param.ParameterName = "@TaxprSttsCd";
            param.Value = record.taxprSttsCd;
            command.Parameters.Add(param);                  // Taxpayer Status Code

            param = command.CreateParameter();
            param.ParameterName = "@BsnsActv";
            param.Value = record2.BsnsActv;
            command.Parameters.Add(param);                  // Business Activities

            param = command.CreateParameter();
            param.ParameterName = "@PrvncNm";
            param.Value = (string.IsNullOrEmpty(record.prvncNm)) ? "" : record.prvncNm;
            command.Parameters.Add(param);                  // Province No.

            param = command.CreateParameter();
            param.ParameterName = "@DstrtNm";
            param.Value = (string.IsNullOrEmpty(record.dstrtNm)) ? "" : record.dstrtNm;
            command.Parameters.Add(param);                  // District No.

            param = command.CreateParameter();
            param.ParameterName = "@SctrNm";
            param.Value = (string.IsNullOrEmpty(record.sctrNm)) ? "" : record.sctrNm;
            command.Parameters.Add(param);                  // Sector No.

            param = command.CreateParameter();
            param.ParameterName = "@LocDesc";
            param.Value = (string.IsNullOrEmpty(record.locDesc)) ? "" : record.locDesc; 
            command.Parameters.Add(param);                  // Location Description

            param = command.CreateParameter();
            param.ParameterName = "@TelNo";
            param.Value = record2.TelNo;
            command.Parameters.Add(param);                  // Telephone number

            param = command.CreateParameter();
            param.ParameterName = "@Email";
            param.Value = record2.Email;
            command.Parameters.Add(param);                  // Email

            param = command.CreateParameter();
            param.ParameterName = "@BankCd";
            param.Value = record2.BankCd;
            command.Parameters.Add(param);                  // Bank Code

            param = command.CreateParameter();
            param.ParameterName = "@BankAccntNo";
            param.Value = record2.BankAccntNo;
            command.Parameters.Add(param);                  // Bank Account Number

            param = command.CreateParameter();
            param.ParameterName = "@BankAccntHldr";
            param.Value = record2.BankAccntHldr;
            command.Parameters.Add(param);                  // Bank Account Holder

            param = command.CreateParameter();
            param.ParameterName = "@ApcntNm";
            param.Value = record2.ApcntNm;
            command.Parameters.Add(param);                  // Applicant name

            param = command.CreateParameter();
            param.ParameterName = "@ApcntTelno";
            param.Value = record2.ApcntTelno;
            command.Parameters.Add(param);                  // Applicant telephone number

            param = command.CreateParameter();
            param.ParameterName = "@ApcntEmail";
            param.Value = record2.ApcntEmail;
            command.Parameters.Add(param);                  // Applicant Email

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = record2.Remark;
            command.Parameters.Add(param);                  // Remark

            param = command.CreateParameter();
            param.ParameterName = "@EbmTyCd";
            param.Value = record2.EbmTyCd;
            command.Parameters.Add(param);                  // EBM Type Code

            param = command.CreateParameter();
            param.ParameterName = "@UserNo";
            param.Value = record2.UserNo;
            command.Parameters.Add(param);                  // User No.

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record2.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record2.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record2.RegDt;
            command.Parameters.Add(param);                  // Registered Date

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record2.ModrId;
            command.Parameters.Add(param);                  // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record2.ModrNm;
            command.Parameters.Add(param);                  // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record2.ModDt;
            command.Parameters.Add(param);                  // Modified Date

            return true;
        }
    }
}

/*
INSERT INTO TAXPAYER_BASE (
                              TIN,
                              TAXPR_NM,
                              TAXPR_STTS_CD,
                              BSNS_ACTV,
                              PRVNC_NM,
                              DSTRT_NM,
                              SCTR_NM,
                              LOC_DESC,
                              TEL_NO,
                              EMAIL,
                              BANK_CD,
                              BANK_ACCNT_NO,
                              BANK_ACCNT_HLDR,
                              APCNT_NM,
                              APCNT_TELNO,
                              APCNT_EMAIL,
                              REMARK,
                              EBM_TY_CD,
                              USER_NO,
                              REGR_ID,
                              REGR_NM,
                              REG_DT,
                              MODR_ID,
                              MODR_NM,
                              MOD_DT
                          )
                          SELECT TIN as TIN,
                                 TAXPAYER_NM as TAXPR_NM,
                                 '' as TAXPR_STTS_CD,
                                 BIZCND as BSNS_ACTV,
                                 PROVINCE as PRVNC_NM,
                                 DISTRICT as DSTRT_NM,
                                 SECTOR as SCTR_NM,
                                 LOC_DC as LOC_DESC,
                                 '' as TEL_NO,
                                 '' as EMAIL,
                                 '' as BANK_CD,
                                 '' as BANK_ACCNT_NO,
                                 '' as BANK_ACCNT_HLDR,
                                 '' as APCNT_NM,
                                 '' as APCNT_TELNO,
                                 '' as APCNT_EMAIL,
                                 '' as REMARK,
                                 '' as EBM_TY_CD,
                                 '' as USER_NO,
                                 '' as REGR_ID,
                                 '' as REGR_NM,
                                 strftime('%Y%m%d%H%M%S',SND_DT) as REG_DT,
                                 '' as MODR_ID,
                                 '' as MODR_NM,
                                 strftime('%Y%m%d%H%M%S',SND_DT) as MOD_DT
                            FROM TAXPAYER; */

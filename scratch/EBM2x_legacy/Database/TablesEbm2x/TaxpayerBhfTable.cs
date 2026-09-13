using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerBhfTable.
    /// </summary>
    public class TaxpayerBhfTable
    {
        public TaxpayerBhfTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TAXPAYER_BHF ( "); 
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       BHF_NM             VARCHAR(60)        null , ");          // Branch Name
            sql.Append("       BHF_STTS_CD        VARCHAR(5)         null , ");          // Branch Status Code
            sql.Append("       PRVNC_NM           VARCHAR(100)       null , ");          // Province No.
            sql.Append("       DSTRT_NM           VARCHAR(100)       null , ");          // District No.
            sql.Append("       SCTR_NM            VARCHAR(100)       null , ");          // Sector No.
            sql.Append("       LOC_DESC           VARCHAR(100)       null , ");          // Location Description
            sql.Append("       MGR_NM             VARCHAR(60)        null , ");          // Manager Name
            sql.Append("       MGR_TEL_NO         VARCHAR(20)        not null , ");      // Manager Telephone number
            sql.Append("       MGR_EMAIL          VARCHAR(100)       null , ");          // Manager Email
            sql.Append("       HQ_YN              CHAR(1)            not null , ");      // Headquarter(Y/N)
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       BHF_NM, ");                  // Branch Name
            sql.Append("       BHF_STTS_CD, ");             // Branch Status Code
            sql.Append("       PRVNC_NM, ");                // Province No.
            sql.Append("       DSTRT_NM, ");                // District No.
            sql.Append("       SCTR_NM, ");                 // Sector No.
            sql.Append("       LOC_DESC, ");                // Location Description
            sql.Append("       MGR_NM, ");                  // Manager Name
            sql.Append("       MGR_TEL_NO, ");              // Manager Telephone number
            sql.Append("       MGR_EMAIL, ");               // Manager Email
            sql.Append("       HQ_YN, ");                   // Headquarter(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from TAXPAYER_BHF "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TAXPAYER_BHF ( "); 
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       BHF_NM, ");                  // Branch Name
            sql.Append("       BHF_STTS_CD, ");             // Branch Status Code
            sql.Append("       PRVNC_NM, ");                // Province No.
            sql.Append("       DSTRT_NM, ");                // District No.
            sql.Append("       SCTR_NM, ");                 // Sector No.
            sql.Append("       LOC_DESC, ");                // Location Description
            sql.Append("       MGR_NM, ");                  // Manager Name
            sql.Append("       MGR_TEL_NO, ");              // Manager Telephone number
            sql.Append("       MGR_EMAIL, ");               // Manager Email
            sql.Append("       HQ_YN, ");                   // Headquarter(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @BhfNm, ");                  // Branch Name
            sql.Append("       @BhfSttsCd, ");              // Branch Status Code
            sql.Append("       @PrvncNm, ");                // Province No.
            sql.Append("       @DstrtNm, ");                // District No.
            sql.Append("       @SctrNm, ");                 // Sector No.
            sql.Append("       @LocDesc, ");                // Location Description
            sql.Append("       @MgrNm, ");                  // Manager Name
            sql.Append("       @MgrTelNo, ");               // Manager Telephone number
            sql.Append("       @MgrEmail, ");               // Manager Email
            sql.Append("       @HqYn, ");                   // Headquarter(Y/N)
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
            sql.Append("update TAXPAYER_BHF "); 
            sql.Append("   set BHF_NM = @BhfNm, ");         // Branch Name
            sql.Append("       BHF_STTS_CD = @BhfSttsCd, ");  // Branch Status Code
            sql.Append("       PRVNC_NM = @PrvncNm, ");     // Province No.
            sql.Append("       DSTRT_NM = @DstrtNm, ");     // District No.
            sql.Append("       SCTR_NM = @SctrNm, ");       // Sector No.
            sql.Append("       LOC_DESC = @LocDesc, ");     // Location Description
            sql.Append("       MGR_NM = @MgrNm, ");         // Manager Name
            sql.Append("       MGR_TEL_NO = @MgrTelNo, ");  // Manager Telephone number
            sql.Append("       MGR_EMAIL = @MgrEmail, ");   // Manager Email
            sql.Append("       HQ_YN = @HqYn, ");           // Headquarter(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TAXPAYER_BHF "); // 납세자 지점
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerBhfRecord record)
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
            param.ParameterName = "@BhfNm";
            param.Value = record.BhfNm;
            command.Parameters.Add(param);                  // Branch Name

            param = command.CreateParameter();
            param.ParameterName = "@BhfSttsCd";
            param.Value = record.BhfSttsCd;
            command.Parameters.Add(param);                  // Branch Status Code

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
            param.ParameterName = "@MgrNm";
            param.Value = record.MgrNm;
            command.Parameters.Add(param);                  // Manager Name

            param = command.CreateParameter();
            param.ParameterName = "@MgrTelNo";
            param.Value = record.MgrTelNo;
            command.Parameters.Add(param);                  // Manager Telephone number

            param = command.CreateParameter();
            param.ParameterName = "@MgrEmail";
            param.Value = record.MgrEmail;
            command.Parameters.Add(param);                  // Manager Email

            param = command.CreateParameter();
            param.ParameterName = "@HqYn";
            param.Value = record.HqYn;
            command.Parameters.Add(param);                  // Headquarter(Y/N)

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
        public bool SetParametersSDC(IDbCommand command, Bhf record, TaxpayerBhfRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@BhfId";
            param.Value = record.bhfId;
            command.Parameters.Add(param);                  // Branch Office ID

            param = command.CreateParameter();
            param.ParameterName = "@BhfNm";
            param.Value = record.bhfNm;
            command.Parameters.Add(param);                  // Branch Name

            param = command.CreateParameter();
            param.ParameterName = "@BhfSttsCd";
            param.Value = record.bhfSttsCd;
            command.Parameters.Add(param);                  // Branch Status Code

            param = command.CreateParameter();
            param.ParameterName = "@PrvncNm";
            param.Value = record.prvncNm;
            command.Parameters.Add(param);                  // Province No.

            param = command.CreateParameter();
            param.ParameterName = "@DstrtNm";
            param.Value = record.dstrtNm;
            command.Parameters.Add(param);                  // District No.

            param = command.CreateParameter();
            param.ParameterName = "@SctrNm";
            param.Value = record.sctrNm;
            command.Parameters.Add(param);                  // Sector No.

            param = command.CreateParameter();
            param.ParameterName = "@LocDesc";
            param.Value = record.locDesc;
            command.Parameters.Add(param);                  // Location Description

            param = command.CreateParameter();
            param.ParameterName = "@MgrNm";
            param.Value = record.mgrNm;
            command.Parameters.Add(param);                  // Manager Name

            param = command.CreateParameter();
            param.ParameterName = "@MgrTelNo";
            param.Value = record.mgrTelNo;
            command.Parameters.Add(param);                  // Manager Telephone number

            param = command.CreateParameter();
            param.ParameterName = "@MgrEmail";
            param.Value = record.mgrEmail;
            command.Parameters.Add(param);                  // Manager Email

            param = command.CreateParameter();
            param.ParameterName = "@HqYn";
            param.Value = record.hqYn;
            command.Parameters.Add(param);                  // Headquarter(Y/N)

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

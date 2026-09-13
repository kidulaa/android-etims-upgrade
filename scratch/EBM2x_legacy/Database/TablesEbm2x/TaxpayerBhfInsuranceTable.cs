using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerBhfInsuranceTable.
    /// </summary>
    public class TaxpayerBhfInsuranceTable
    {
        public TaxpayerBhfInsuranceTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TAXPAYER_BHF_INSURANCE ( "); 
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       ISSRCC_CD          VARCHAR(10)        not null , ");      // Insurance Company Code
            sql.Append("       ISRCC_NM           VARCHAR(100)       null , ");          // Insurance Company Name
            sql.Append("       ISRC_RT            INT                null , ");          // Insurance Rate
            sql.Append("       USE_YN             CHAR(1)            null , ");          // Use(Y/N)
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       primary key ( TIN, BHF_ID, ISSRCC_CD ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       ISSRCC_CD, ");               // Insurance Company Code
            sql.Append("       ISRCC_NM, ");                // Insurance Company Name
            sql.Append("       ISRC_RT, ");                 // Insurance Rate
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT  ");                  // Registered Date
            sql.Append("  from TAXPAYER_BHF_INSURANCE "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TAXPAYER_BHF_INSURANCE ( "); // 납세자 지점 보험사
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       ISSRCC_CD, ");               // Insurance Company Code
            sql.Append("       ISRCC_NM, ");                // Insurance Company Name
            sql.Append("       ISRC_RT, ");                 // Insurance Rate
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT  ");                  // Registered Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @IssrccCd, ");               // Insurance Company Code
            sql.Append("       @IsrccNm, ");                // Insurance Company Name
            sql.Append("       @IsrcRt, ");                 // Insurance Rate
            sql.Append("       @UseYn, ");                  // Use(Y/N)
            sql.Append("       @ModrId, ");                 // Modifier ID
            sql.Append("       @ModrNm, ");                 // Modifier Name
            sql.Append("       @ModDt, ");                  // Modified Date
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @RegDt  ");                  // Registered Date
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_BHF_INSURANCE "); // 납세자 지점 보험사
            sql.Append("   set ISRCC_NM = @IsrccNm, ");     // Insurance Company Name
            sql.Append("       ISRC_RT = @IsrcRt, ");       // Insurance Rate
            sql.Append("       USE_YN = @UseYn, ");         // Use(Y/N)
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt, ");         // Modified Date
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt  ");         // Registered Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and ISSRCC_CD = @IssrccCd ");  // Insurance Company Code
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TAXPAYER_BHF_INSURANCE "); // 납세자 지점 보험사
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and ISSRCC_CD = @IssrccCd ");  // Insurance Company Code
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerBhfInsuranceRecord record)
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
            param.ParameterName = "@IssrccCd";
            param.Value = record.IssrccCd;
            command.Parameters.Add(param);                  // Insurance Company Code

            param = command.CreateParameter();
            param.ParameterName = "@IsrccNm";
            param.Value = record.IsrccNm;
            command.Parameters.Add(param);                  // Insurance Company Name

            param = command.CreateParameter();
            param.ParameterName = "@IsrcRt";
            param.Value = record.IsrcRt;
            command.Parameters.Add(param);                  // Insurance Rate

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.UseYn;
            command.Parameters.Add(param);                  // Use(Y/N)

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

            return true;
        }
    }
}

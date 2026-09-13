using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of CodeDtlTable.
    /// </summary>
    public class CodeDtlTable
    {
        public CodeDtlTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists CODE_DTL ( "); 
            sql.Append("       CD                 VARCHAR(5)         not null , ");      // Code
            sql.Append("       CD_CLS             CHAR(2)            not null , ");      // Code classification
            sql.Append("       CD_NM              VARCHAR(60)        not null , ");      // Name of Code
            sql.Append("       CD_DESC            VARCHAR(500)       null , ");          // Description of the Code
            sql.Append("       SRT_ORD            INT                null , ");          // Sort Order
            sql.Append("       USER_DFN_CD1       VARCHAR(20)        null , ");          // User Define Code 1
            sql.Append("       USER_DFN_CD2       VARCHAR(20)        null , ");          // User Define Code 2
            sql.Append("       USER_DFN_CD3       VARCHAR(20)        null , ");          // User Define Code 3
            sql.Append("       USE_YN             CHAR(1)            null , ");          // Use(Y/N)
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( CD, CD_CLS ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select CD, ");                      // Code
            sql.Append("       CD_CLS, ");                  // Code classification
            sql.Append("       CD_NM, ");                   // Name of Code
            sql.Append("       CD_DESC, ");                 // Description of the Code
            sql.Append("       SRT_ORD, ");                 // Sort Order
            sql.Append("       USER_DFN_CD1, ");            // User Define Code 1
            sql.Append("       USER_DFN_CD2, ");            // User Define Code 2
            sql.Append("       USER_DFN_CD3, ");            // User Define Code 3
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from CODE_DTL ");
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into CODE_DTL ( ");
            sql.Append("       CD, ");                      // Code
            sql.Append("       CD_CLS, ");                  // Code classification
            sql.Append("       CD_NM, ");                   // Name of Code
            sql.Append("       CD_DESC, ");                 // Description of the Code
            sql.Append("       SRT_ORD, ");                 // Sort Order
            sql.Append("       USER_DFN_CD1, ");            // User Define Code 1
            sql.Append("       USER_DFN_CD2, ");            // User Define Code 2
            sql.Append("       USER_DFN_CD3, ");            // User Define Code 3
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Cd, ");                     // Code
            sql.Append("       @CdCls, ");                  // Code classification
            sql.Append("       @CdNm, ");                   // Name of Code
            sql.Append("       @CdDesc, ");                 // Description of the Code
            sql.Append("       @SrtOrd, ");                 // Sort Order
            sql.Append("       @UserDfnCd1, ");             // User Define Code 1
            sql.Append("       @UserDfnCd2, ");             // User Define Code 2
            sql.Append("       @UserDfnCd3, ");             // User Define Code 3
            sql.Append("       @UseYn, ");                  // Use(Y/N)
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
            sql.Append("update CODE_DTL ");
            sql.Append("   set CD_NM = @CdNm, ");           // Name of Code
            sql.Append("       CD_DESC = @CdDesc, ");       // Description of the Code
            sql.Append("       SRT_ORD = @SrtOrd, ");       // Sort Order
            sql.Append("       USER_DFN_CD1 = @UserDfnCd1, ");  // User Define Code 1
            sql.Append("       USER_DFN_CD2 = @UserDfnCd2, ");  // User Define Code 2
            sql.Append("       USER_DFN_CD3 = @UserDfnCd3, ");  // User Define Code 3
            sql.Append("       USE_YN = @UseYn, ");         // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where CD = @Cd ");           // Code
            sql.Append("   and CD_CLS = @CdCls ");    // Code classification
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from CODE_DTL ");
            sql.Append(" where CD = @Cd ");           // Code
            sql.Append("   and CD_CLS = @CdCls ");    // Code classification
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, CodeDtlRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Cd";
            param.Value = record.Cd;
            command.Parameters.Add(param);                  // Code

            param = command.CreateParameter();
            param.ParameterName = "@CdCls";
            param.Value = record.CdCls;
            command.Parameters.Add(param);                  // Code classification

            param = command.CreateParameter();
            param.ParameterName = "@CdNm";
            param.Value = record.CdNm;
            command.Parameters.Add(param);                  // Name of Code

            param = command.CreateParameter();
            param.ParameterName = "@CdDesc";
            param.Value = record.CdDesc;
            command.Parameters.Add(param);                  // Description of the Code

            param = command.CreateParameter();
            param.ParameterName = "@SrtOrd";
            param.Value = record.SrtOrd;
            command.Parameters.Add(param);                  // Sort Order

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnCd1";
            param.Value = record.UserDfnCd1;
            command.Parameters.Add(param);                  // User Define Code 1

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnCd2";
            param.Value = record.UserDfnCd2;
            command.Parameters.Add(param);                  // User Define Code 2

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnCd3";
            param.Value = record.UserDfnCd3;
            command.Parameters.Add(param);                  // User Define Code 3

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.UseYn;
            command.Parameters.Add(param);                  // Use(Y/N)

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
        public bool SetParametersSDC(IDbCommand command, CodeDtlLVO record, CodeClassLVO record2, CodeClassRecord record3)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Cd";
            param.Value = string.IsNullOrEmpty(record.cd) ? "" : record.cd;
            command.Parameters.Add(param);                  // Code

            param = command.CreateParameter();
            param.ParameterName = "@CdCls";
            param.Value = record2.cdCls;
            command.Parameters.Add(param);                  // Code classification

            param = command.CreateParameter();
            param.ParameterName = "@CdNm";
            param.Value = string.IsNullOrEmpty(record.cdNm) ? "" : record.cdNm;
            command.Parameters.Add(param);                  // Name of Code

            param = command.CreateParameter();
            param.ParameterName = "@CdDesc";
            param.Value = string.IsNullOrEmpty(record.cdDesc) ? "" : record.cdDesc;
            command.Parameters.Add(param);                  // Description of the Code

            param = command.CreateParameter();
            param.ParameterName = "@SrtOrd";
            param.Value = record.srtOrd;
            command.Parameters.Add(param);                  // Sort Order

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnCd1";
            param.Value = string.IsNullOrEmpty(record.userDfnCd1) ? "" : record.userDfnCd1;
            command.Parameters.Add(param);                  // User Define Code 1

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnCd2";
            param.Value = string.IsNullOrEmpty(record.userDfnCd2) ? "" : record.userDfnCd2;
            command.Parameters.Add(param);                  // User Define Code 2

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnCd3";
            param.Value = string.IsNullOrEmpty(record.userDfnCd3) ? "" : record.userDfnCd3;
            command.Parameters.Add(param);                  // User Define Code 3

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = string.IsNullOrEmpty(record.useYn) ? "" : record.useYn;
            command.Parameters.Add(param);                  // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record3.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record3.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record3.RegDt;
            command.Parameters.Add(param);                  // Registered Date

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record3.ModrId;
            command.Parameters.Add(param);                  // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record3.ModrNm;
            command.Parameters.Add(param);                  // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record3.ModDt;
            command.Parameters.Add(param);                  // Modified Date

            return true;
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of CodeClassTable.
    /// </summary>
    public class CodeClassTable
    {
        public CodeClassTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists CODE_CLASS ( "); 
            sql.Append("       CD_CLS             CHAR(2)             null , ");  // Code classification
            sql.Append("       CD_CLS_NM          VARCHAR(60)        null , ");  // Name of code classification
            sql.Append("       CD_CLS_DESC        VARCHAR(500)       null , ");  // Description of code classification
            sql.Append("       USER_DFN_NM1       VARCHAR(60)        null , ");  // User Define Name 1
            sql.Append("       USER_DFN_NM2       VARCHAR(60)        null , ");  // User Define Name 2
            sql.Append("       USER_DFN_NM3       VARCHAR(60)        null , ");  // User Define Name 3
            //JCNA 202001 DELETE sql.Append("       CLIENT_USE_YN      CHAR(1)             null , ");  // Use of Client(Y/N)
            sql.Append("       USE_YN             CHAR(1)             null , ");  // Use(Y/N)
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");  // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");  // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");  // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");  // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");  // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");  // Modified Date
            sql.Append("       primary key ( CD_CLS ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select CD_CLS, ");                                      // Code classification
            sql.Append("       CD_CLS_NM, ");                                   // Name of code classification
            sql.Append("       CD_CLS_DESC, ");                                 // Description of code classification
            sql.Append("       USER_DFN_NM1, ");                                // User Define Name 1
            sql.Append("       USER_DFN_NM2, ");                                // User Define Name 2
            sql.Append("       USER_DFN_NM3, ");                                // User Define Name 3
            //JCNA 202001 DELETE sql.Append("       CLIENT_USE_YN, ");                               // Use of Client(Y/N)
            sql.Append("       USE_YN, ");                                      // Use(Y/N)
            sql.Append("       REGR_ID, ");                                     // Registrant ID
            sql.Append("       REGR_NM, ");                                     // Registrant Name
            sql.Append("       REG_DT, ");                                      // Registered Date
            sql.Append("       MODR_ID, ");                                     // Modifier ID
            sql.Append("       MODR_NM, ");                                     // Modifier Name
            sql.Append("       MOD_DT  ");                                      // Modified Date
            sql.Append("  from CODE_CLASS "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into CODE_CLASS ( "); 
            sql.Append("       CD_CLS, ");                                      // Code classification
            sql.Append("       CD_CLS_NM, ");                                   // Name of code classification
            sql.Append("       CD_CLS_DESC, ");                                 // Description of code classification
            sql.Append("       USER_DFN_NM1, ");                                // User Define Name 1
            sql.Append("       USER_DFN_NM2, ");                                // User Define Name 2
            sql.Append("       USER_DFN_NM3, ");                                // User Define Name 3
            //JCNA 202001 DELETE sql.Append("       CLIENT_USE_YN, ");                               // Use of Client(Y/N)
            sql.Append("       USE_YN, ");                                      // Use(Y/N)
            sql.Append("       REGR_ID, ");                                     // Registrant ID
            sql.Append("       REGR_NM, ");                                     // Registrant Name
            sql.Append("       REG_DT, ");                                      // Registered Date
            sql.Append("       MODR_ID, ");                                     // Modifier ID
            sql.Append("       MODR_NM, ");                                     // Modifier Name
            sql.Append("       MOD_DT  ");                                      // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @CdCls, ");                                      // Code classification
            sql.Append("       @CdClsNm, ");                                    // Name of code classification
            sql.Append("       @CdClsDesc, ");                                  // Description of code classification
            sql.Append("       @UserDfnNm1, ");                                 // User Define Name 1
            sql.Append("       @UserDfnNm2, ");                                 // User Define Name 2
            sql.Append("       @UserDfnNm3, ");                                 // User Define Name 3
            //JCNA 202001 DELETE sql.Append("       @ClientUseYn, ");                                // Use of Client(Y/N)
            sql.Append("       @UseYn, ");                                      // Use(Y/N)
            sql.Append("       @RegrId, ");                                     // Registrant ID
            sql.Append("       @RegrNm, ");                                     // Registrant Name
            sql.Append("       @RegDt, ");                                      // Registered Date
            sql.Append("       @ModrId, ");                                     // Modifier ID
            sql.Append("       @ModrNm, ");                                     // Modifier Name
            sql.Append("       @ModDt  ");                                      // Modified Date
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update CODE_CLASS "); 
            sql.Append("   set CD_CLS_NM = @CdClsNm, ");                                  // Name of code classification
            sql.Append("       CD_CLS_DESC = @CdClsDesc, ");                              // Description of code classification
            sql.Append("       USER_DFN_NM1 = @UserDfnNm1, ");                            // User Define Name 1
            sql.Append("       USER_DFN_NM2 = @UserDfnNm2, ");                            // User Define Name 2
            sql.Append("       USER_DFN_NM3 = @UserDfnNm3, ");                            // User Define Name 3
            //JCNA 202001 DELETE sql.Append("       CLIENT_USE_YN = @ClientUseYn, ");                          // Use of Client(Y/N)
            sql.Append("       USE_YN = @UseYn, ");                                       // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");                                     // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");                                     // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");                                       // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");                                     // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");                                     // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");                                       // Modified Date
            sql.Append(" where CD_CLS = @CdCls ");  // Code classification
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from CODE_CLASS ");
            sql.Append(" where CD_CLS = @CdCls "); // Code classification
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, CodeClassRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@CdCls";
            param.Value = record.CdCls;
            command.Parameters.Add(param);        // Code classification

            param = command.CreateParameter();
            param.ParameterName = "@CdClsNm";
            param.Value = record.CdClsNm;
            command.Parameters.Add(param);        // Name of code classification

            param = command.CreateParameter();
            param.ParameterName = "@CdClsDesc";
            param.Value = record.CdClsDesc;
            command.Parameters.Add(param);        // Description of code classification

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnNm1";
            param.Value = record.UserDfnNm1;
            command.Parameters.Add(param);        // User Define Name 1

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnNm2";
            param.Value = record.UserDfnNm2;
            command.Parameters.Add(param);        // User Define Name 2

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnNm3";
            param.Value = record.UserDfnNm3;
            command.Parameters.Add(param);        // User Define Name 3

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@ClientUseYn";
            //param.Value = record.ClientUseYn;
            //command.Parameters.Add(param);        // Use of Client(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.UseYn;
            command.Parameters.Add(param);        // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record.RegrId;
            command.Parameters.Add(param);        // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record.RegrNm;
            command.Parameters.Add(param);        // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record.RegDt;
            command.Parameters.Add(param);        // Registered Date

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record.ModrId;
            command.Parameters.Add(param);        // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record.ModrNm;
            command.Parameters.Add(param);        // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record.ModDt;
            command.Parameters.Add(param);        // Modified Date

            return true;
        }
        public bool SetParametersSDC(IDbCommand command, CodeClassLVO record, CodeClassRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@CdCls";
            param.Value = string.IsNullOrEmpty(record.cdCls) ? "" : record.cdCls;
            command.Parameters.Add(param);        // Code classification

            param = command.CreateParameter();
            param.ParameterName = "@CdClsNm";
            param.Value = string.IsNullOrEmpty(record.cdClsNm) ? "" : record.cdClsNm;
            command.Parameters.Add(param);        // Name of code classification

            param = command.CreateParameter();
            param.ParameterName = "@CdClsDesc";
            param.Value = string.IsNullOrEmpty(record.cdClsDesc) ? "" : record.cdClsDesc;
            command.Parameters.Add(param);        // Description of code classification

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnNm1";
            param.Value = string.IsNullOrEmpty(record.userDfnNm1) ? "" : record.userDfnNm1;
            command.Parameters.Add(param);        // User Define Name 1

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnNm2";
            param.Value = string.IsNullOrEmpty(record.userDfnNm2) ? "" : record.userDfnNm2;
            command.Parameters.Add(param);        // User Define Name 2

            param = command.CreateParameter();
            param.ParameterName = "@UserDfnNm3";
            param.Value = string.IsNullOrEmpty(record.userDfnNm3) ? "" : record.userDfnNm3;
            command.Parameters.Add(param);        // User Define Name 3

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@ClientUseYn";
            //param.Value = string.IsNullOrEmpty(record.useYn) ? "" : record.useYn;
            //command.Parameters.Add(param);        // Use of Client(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = string.IsNullOrEmpty(record.useYn) ? "" : record.useYn;
            command.Parameters.Add(param);        // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record2.RegrId;
            command.Parameters.Add(param);        // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record2.RegrNm;
            command.Parameters.Add(param);        // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record2.RegDt;
            command.Parameters.Add(param);        // Registered Date

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record2.ModrId;
            command.Parameters.Add(param);        // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record2.ModrNm;
            command.Parameters.Add(param);        // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record2.ModDt;
            command.Parameters.Add(param);        // Modified Date

            return true;
        }
    }
}

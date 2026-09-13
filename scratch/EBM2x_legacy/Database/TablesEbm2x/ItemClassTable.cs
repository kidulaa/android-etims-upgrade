using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of ItemClassTable.
    /// </summary>
    public class ItemClassTable
    {
        public ItemClassTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists ITEM_CLASS ( ");
            sql.Append("       ITEM_CLS_CD        VARCHAR(10)        not null , ");      // Item Classification Code (RRA)
            sql.Append("       ITEM_CLS_LVL       LONG               null , ");          // Item Category Code(UN/SPSC Code)
            sql.Append("       ITEM_CLS_NM        VARCHAR(200)       null , ");          // Item Classification Name
            sql.Append("       TAX_TY_CD          VARCHAR(5)         null , ");          // Taxation Type Code
            sql.Append("       MJR_TG_YN          CHAR(1)            null , ");          // Major Taget(Y/N)
            sql.Append("       USE_YN             CHAR(1)            null , ");          // Use(Y/N)
            sql.Append("       REMARK             VARCHAR(400)       null , ");          // Remark
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( ITEM_CLS_CD ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ITEM_CLS_CD, ");             // Item Classification Code (RRA)
            sql.Append("       ITEM_CLS_LVL, ");            // Item Category Code(UN/SPSC Code)
            sql.Append("       ITEM_CLS_NM, ");             // Item Classification Name
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       MJR_TG_YN, ");               // Major Taget(Y/N)
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from ITEM_CLASS ");
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into ITEM_CLASS ( ");
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code (RRA)
            sql.Append("       ITEM_CLS_LVL, ");            // Item Category Code(UN/SPSC Code)
            sql.Append("       ITEM_CLS_NM, ");             // Item Classification Name
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       MJR_TG_YN, ");               // Major Taget(Y/N)
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @ItemClsCd, ");              // Item Classification Code (RRA)
            sql.Append("       @ItemClsLvl, ");             // Item Category Code(UN/SPSC Code)
            sql.Append("       @ItemClsNm, ");              // Item Classification Name
            sql.Append("       @TaxTyCd, ");                // Taxation Type Code
            sql.Append("       @MjrTgYn, ");                // Major Taget(Y/N)
            sql.Append("       @UseYn, ");                  // Use(Y/N)
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
            sql.Append("update ITEM_CLASS ");
            sql.Append("   set ITEM_CLS_LVL = @ItemClsLvl, ");  // Item Category Code(UN/SPSC Code)
            sql.Append("       ITEM_CLS_NM = @ItemClsNm, ");  // Item Classification Name
            sql.Append("       TAX_TY_CD = @TaxTyCd, ");    // Taxation Type Code
            sql.Append("       MJR_TG_YN = @MjrTgYn, ");    // Major Taget(Y/N)
            sql.Append("       USE_YN = @UseYn, ");         // Use(Y/N)
            sql.Append("       REMARK = @Remark, ");        // Remark
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where ITEM_CLS_CD = @ItemClsCd ");  // Item Classification Code (RRA)
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ITEM_CLASS ");
            sql.Append(" where ITEM_CLS_CD = @ItemClsCd ");  // Item Classification Code (RRA)
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, ItemClassRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = record.ItemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code (RRA)

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsLvl";
            param.Value = record.ItemClsLvl;
            command.Parameters.Add(param);                  // Item Category Code(UN/SPSC Code)

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsNm";
            param.Value = record.ItemClsNm;
            command.Parameters.Add(param);                  // Item Classification Name

            param = command.CreateParameter();
            param.ParameterName = "@TaxTyCd";
            param.Value = record.TaxTyCd;
            command.Parameters.Add(param);                  // Taxation Type Code

            param = command.CreateParameter();
            param.ParameterName = "@MjrTgYn";
            param.Value = record.MjrTgYn;
            command.Parameters.Add(param);                  // Major Taget(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.UseYn;
            command.Parameters.Add(param);                  // Use(Y/N)

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
        public bool SetParametersSDC(IDbCommand command, ItemClassLVO record, ItemClassRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = string.IsNullOrEmpty(record.itemClsCd) ? "" : record.itemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code (RRA)

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsLvl";
            param.Value = record.itemClsLvl;
            command.Parameters.Add(param);                  // Item Category Code(UN/SPSC Code)

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsNm";
            param.Value = string.IsNullOrEmpty(record.itemClsNm) ? "" : record.itemClsNm;
            command.Parameters.Add(param);                  // Item Classification Name

            param = command.CreateParameter();
            param.ParameterName = "@TaxTyCd";
            param.Value = string.IsNullOrEmpty(record.taxTyCd) ? "" : record.taxTyCd;
            command.Parameters.Add(param);                  // Taxation Type Code

            param = command.CreateParameter();
            param.ParameterName = "@MjrTgYn";
            param.Value = string.IsNullOrEmpty(record.mjrTgYn) ? "" : record.mjrTgYn;
            command.Parameters.Add(param);                  // Major Taget(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.useYn;
            command.Parameters.Add(param);                  // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = string.IsNullOrEmpty(record2.Remark) ? "" : record2.Remark;
            command.Parameters.Add(param);                  // Remark

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

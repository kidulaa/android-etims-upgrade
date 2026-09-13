using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of StockMasterTable.
    /// </summary>
    public class StockMasterTable
    {
        public StockMasterTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists STOCK_MASTER ( "); 
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       ITEM_CD            VARCHAR(20)        not null , ");      // Item Code
            sql.Append("       RSD_QTY            DECIMAL(13,2)      null , ");          // Resodual Quantity
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( TIN , BHF_ID, ITEM_CD ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       RSD_QTY, ");                 // Resodual Quantity
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from STOCK_MASTER "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into STOCK_MASTER ( "); 
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       RSD_QTY, ");                 // Resodual Quantity
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @ItemCd, ");                 // Item Code
            sql.Append("       @RsdQty, ");                 // Resodual Quantity
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
            sql.Append("update STOCK_MASTER ");
            sql.Append("   set RSD_QTY = @RsdQty, ");       // Resodual Quantity
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and ITEM_CD = @ItemCd ");  // Item Code
            return sql.ToString();
        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from STOCK_MASTER ");
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and ITEM_CD = @ItemCd ");  // Item Code
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, StockMasterRecord record)
        {
            record.UpdateNull();

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
            param.ParameterName = "@ItemCd";
            param.Value = record.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@RsdQty";
            param.Value = record.RsdQty;
            command.Parameters.Add(param);                  // Resodual Quantity

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

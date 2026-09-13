using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerItemCompositionTable.
    /// </summary>
    public class TaxpayerItemCompositionTable
    {
        public TaxpayerItemCompositionTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("create table if not exists TAXPAYER_ITEM_COMPOSITION ( "); // 납세자 품목 구성
            sql.AppendFormat("       TIN                CHAR(9)             not null , ");      // Taxpayer Identification Number(TIN)
            sql.AppendFormat("       ITEM_CD            VARCHAR2(20)        not null , ");      // Item Code
            sql.AppendFormat("       CPST_ITEM_CD       VARCHAR2(20)        not null , ");      // Composition Item Code
            sql.AppendFormat("       CPST_QTY           NUMBER(13,2)        null , ");          // Composition Quantity
            sql.AppendFormat("       REGR_ID            VARCHAR2(20)        null , ");          // Registrant ID
            sql.AppendFormat("       REGR_NM            VARCHAR2(60)        null , ");          // Registrant Name
            sql.AppendFormat("       REG_DT             VARCHAR2(14)        null , ");          // Registered Date
            sql.AppendFormat("       primary key ( TIN,ITEM_CD,CPST_ITEM_CD ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select  ");
            sql.AppendFormat("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.AppendFormat("       ITEM_CD, ");                 // Item Code
            sql.AppendFormat("       CPST_ITEM_CD, ");            // Composition Item Code
            sql.AppendFormat("       CPST_QTY, ");                // Composition Quantity
            sql.AppendFormat("       REGR_ID, ");                 // Registrant ID
            sql.AppendFormat("       REGR_NM, ");                 // Registrant Name
            sql.AppendFormat("       REG_DT  ");                  // Registered Date
            sql.AppendFormat("  from TAXPAYER_ITEM_COMPOSITION "); // 납세자 품목 구성
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            //@ychan_20191208 insert -> Replacd 로 변경
            sql.AppendFormat("insert into TAXPAYER_ITEM_COMPOSITION ( "); // 납세자 품목 구성
            sql.AppendFormat("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.AppendFormat("       ITEM_CD, ");                 // Item Code
            sql.AppendFormat("       CPST_ITEM_CD, ");            // Composition Item Code
            sql.AppendFormat("       CPST_QTY, ");                // Composition Quantity
            sql.AppendFormat("       REGR_ID, ");                 // Registrant ID
            sql.AppendFormat("       REGR_NM, ");                 // Registrant Name
            sql.AppendFormat("       REG_DT  ");                  // Registered Date
            sql.AppendFormat("     ) values ( ");
            sql.AppendFormat("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.AppendFormat("       @ItemCd, ");                 // Item Code
            sql.AppendFormat("       @CpstItemCd, ");             // Composition Item Code
            sql.AppendFormat("       @CpstQty, ");                // Composition Quantity
            sql.AppendFormat("       @RegrId, ");                 // Registrant ID
            sql.AppendFormat("       @RegrNm, ");                 // Registrant Name
            sql.AppendFormat("       @RegDt  ");                  // Registered Date
            sql.AppendFormat("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update TAXPAYER_ITEM_COMPOSITION "); // 납세자 품목 구성
            sql.AppendFormat("   set ");
            sql.AppendFormat("       TIN = @Tin, ");              // Taxpayer Identification Number(TIN)
            sql.AppendFormat("       ITEM_CD = @ItemCd, ");       // Item Code
            sql.AppendFormat("       CPST_ITEM_CD = @CpstItemCd, ");  // Composition Item Code
            sql.AppendFormat("       CPST_QTY = @CpstQty, ");     // Composition Quantity
            sql.AppendFormat("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.AppendFormat("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.AppendFormat("       REG_DT = @RegDt  ");         // Registered Date
            sql.AppendFormat(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.AppendFormat("   and ITEM_CD = @ItemCd ");  // Item Code
            sql.AppendFormat("   and CPST_ITEM_CD = @CpstItemCd ");  // Composition Item Code
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from TAXPAYER_ITEM_COMPOSITION "); // 납세자 품목 구성
            sql.AppendFormat(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.AppendFormat("   and ITEM_CD = @ItemCd ");  // Item Code
            sql.AppendFormat("   and CPST_ITEM_CD = @CpstItemCd ");  // Composition Item Code
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerItemCompositionRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = record.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@CpstItemCd";
            param.Value = record.CpstItemCd;
            command.Parameters.Add(param);                  // Composition Item Code

            param = command.CreateParameter();
            param.ParameterName = "@CpstQty";
            param.Value = record.CpstQty;
            command.Parameters.Add(param);                  // Composition Quantity

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

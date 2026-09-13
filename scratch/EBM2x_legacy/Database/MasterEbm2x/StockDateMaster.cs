using EBM2x.Database.Master;
using EBM2x.Database.TableIO;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class StockDateMaster : ModelIO
    {
        public List<StockDateRecord> getStackDateTable(string StartDate)
        {
            List<StockDateRecord> arrayList = new List<StockDateRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TAXPAYER_ITEM.ITEM_CD,    ");
            sql.Append("       TAXPAYER_ITEM.ITEM_CLS_CD,  ");
            sql.Append("       ITEM_NM,        ");
            sql.Append("       ITEM_TY_CD,  ");
            sql.Append("       IFNULL(INITL_QTY,0) AS INITQTY,    ");
            sql.Append("       IFNULL(INITL_WH_UNTPC,0) AS PURCHASE,    ");
            sql.Append("       IFNULL(DFT_PRC,0) AS PRICE,    ");
            sql.Append("       STOCK_MASTER.RSD_QTY AS QTY    ");
            sql.Append("  FROM TAXPAYER_ITEM,STOCK_MASTER    ");
            sql.Append(" WHERE TAXPAYER_ITEM.ITEM_CD = STOCK_MASTER.ITEM_CD   ");
            sql.Append("   AND TAXPAYER_ITEM.ITEM_TY_CD IN ( '1', '2' )    ");
            sql.Append("   AND TAXPAYER_ITEM.USE_YN = 'Y'    ");
            sql.Append(" ORDER BY ITEM_NM     ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;
                
                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockDateRecord record = new StockDateRecord();

                    if (!reader.IsDBNull(0)) record.ItemCd = reader.GetString(0);   
                    if (!reader.IsDBNull(1)) record.ItemClsCd = reader.GetString(1);  
                    if (!reader.IsDBNull(2)) record.ItemNm = reader.GetString(2); 
                    if (!reader.IsDBNull(3)) record.ItemTyCd = reader.GetString(3);
                    if (!reader.IsDBNull(4)) record.InitlQty = reader.GetDouble(4);
                    if (!reader.IsDBNull(5)) record.InitlWhPrc = reader.GetDouble(5);
                    if (!reader.IsDBNull(6)) record.Prc = reader.GetDouble(6); 
                    if (!reader.IsDBNull(7)) record.Qty = reader.GetDouble(7);

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
    }
}

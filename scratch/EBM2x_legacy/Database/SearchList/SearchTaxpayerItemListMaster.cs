using System;
using System.Data;
using System.Text;

namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Models.ListView;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of SearchTaxpayerItemListMaster.
    /// </summary>
    public class SearchTaxpayerItemListMaster : ModelIO
    {		
            /*
                       sql.Append("select a.ITEM_CD,          ");
                       sql.Append("       b.ITEM_NM,          ");
                       sql.Append("       b.BCD,              ");
                       sql.Append("       a.RSD_QTY,          ");
                       sql.Append("       b.DFT_PRC           ");
                       sql.Append("  from                     ");
                       sql.Append("(select ITEM_CD,           ");
                       sql.Append("       RSD_QTY             ");
                       sql.Append("  from STOCK_MASTER        ");
                       sql.Append(" where TIN = '000000000'   ");
                       sql.Append("   and BHF_ID = '00') a    ");
                       sql.Append("INNER JOIN                 ");
                       sql.Append("(select ITEM_CD,           ");
                       sql.Append("       BCD,                ");
                       sql.Append("       ITEM_NM,            ");
                       sql.Append("       DFT_PRC             ");
                       sql.Append("  from TAXPAYER_ITEM       ");
                       sql.Append(" where TIN = '000000000'   ");
                       sql.Append("   and ITEM_CD = 'RW2BGXKGX0000001') b ");
                       sql.Append("ON a.ITEM_CD = b.ITEM_CD   ");

select a.ITEM_CD,
       b.ITEM_NM,
       b.BCD,
       a.RSD_QTY,
       b.DFT_PRC,
       b.BTC_NUM,
       b.EXP_DT_YN,
       b.EXP_DT 
  from 
(select ITEM_CD,
       RSD_QTY 
  from STOCK_MASTER
 where TIN = '000000000' 
   and BHF_ID = '00') a
INNER JOIN
(select ITEM_CD, 
       BCD,  
       ITEM_NM,
       DFT_PRC,
       BTC_NUM,
       EXP_DT_YN,
       EXP_DT 
  from TAXPAYER_ITEM
 where TIN = '000000000' 
   and ITEM_CD = 'RW2BGXKGX0000001') b
ON a.ITEM_CD = b.ITEM_CD      
            */

        public List<SearchItemNode> GetTableBarcode(string tin, string barcode)
        {
            List<SearchItemNode> arrayList = new List<SearchItemNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select ITEM_CD,      ");
            sql.Append("       BCD,          ");
            sql.Append("       ITEM_NM,      ");
            sql.Append("       DFT_PRC,      ");
            sql.Append("       BATCH_NUM     ");
            sql.Append("  from TAXPAYER_ITEM ");
            sql.Append(" where TIN = @TIN and BCD = @BCD  AND USE_YN = 'Y'");
            sql.Append(" order by ITEM_CD    ");

            ItemClassMaster ItemClassMaster = new ItemClassMaster();
            StockMasterMaster StockMasterMaster = new StockMasterMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BCD";
                param.Value = barcode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                { 
                    SearchItemNode record = new SearchItemNode();

                    record.ItemCode = reader.GetValue(0).ToString();
                    record.Barcode = reader.GetValue(1).ToString();
                    record.ItemName = reader.GetValue(2).ToString();
                    record.Price = reader.GetDouble(3);
                    if(!reader.IsDBNull(4)) record.BarchNumber = reader.GetValue(4).ToString(); ;

                    arrayList.Add(record);
                }
                reader.Close();
                
                foreach (SearchItemNode record in arrayList)
                {
                    record.StockQty = (int)StockMasterMaster.GetCurrentStock(tin, record.ItemCode);  // 현재고 수량
                    record.ExpirationDate = StockIoItemMaster.GetExpirationDate(tin, record.ItemCode);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }

        public List<SearchItemNode> GetTable(string tin, string likeValue)
        {
            List<SearchItemNode> arrayList = new List<SearchItemNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select ITEM_CD,      ");
            sql.Append("       BCD,          ");
            sql.Append("       ITEM_NM,      ");
            sql.Append("       DFT_PRC,      ");
            sql.Append("       BATCH_NUM     ");
            sql.Append("  from TAXPAYER_ITEM ");
            if (string.IsNullOrEmpty(likeValue))
            {
                sql.Append(" where USE_YN = 'Y' ");
                sql.Append(" order by mod_dt desc ");
                sql.Append(" LIMIT 20 ");
            }
            else
            {
                sql.Append(" where USE_YN = 'Y' ");
                sql.Append("   and (  ITEM_CD like @likeValue ");
                sql.Append("       or BCD like @likeValue ");
                sql.Append("       or ITEM_NM like @likeValue ) ");
                sql.Append(" order by ITEM_CD ");
            }
            ItemClassMaster ItemClassMaster = new ItemClassMaster();
            StockMasterMaster StockMasterMaster = new StockMasterMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SearchItemNode record = new SearchItemNode();

                    record.ItemCode = reader.GetValue(0).ToString();
                    record.Barcode = reader.GetValue(1).ToString();
                    record.ItemName = reader.GetValue(2).ToString();
                    record.Price = reader.GetDouble(3);
                    if (!reader.IsDBNull(4)) record.BarchNumber = reader.GetValue(4).ToString(); ;

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (SearchItemNode record in arrayList)
                {
                    record.StockQty = (int)StockMasterMaster.GetCurrentStock(tin, record.ItemCode);  // 현재고 수량
                    record.ExpirationDate = StockIoItemMaster.GetExpirationDate(tin, record.ItemCode);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
    }
}

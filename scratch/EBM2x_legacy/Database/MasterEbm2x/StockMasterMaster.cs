using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of StockMasterMaster.
    /// </summary>
    public class StockMasterMaster : ModelIO
    {
        public long GetStockSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(SAR_NO), 0)  FROM STOCK_IO";
                command.CommandType = CommandType.Text;

                long StockSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    StockSeq = long.Parse(firstColumn.ToString());
                }
                return (long)StockSeq + 1;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 1;
            }
        }

        public double GetCurrentStock(string valTin, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                string itemType = taxpayerItemMaster.GetItemType(valTin, valCode);

                command.CommandText = "SELECT IFNULL(RSD_QTY,0) FROM STOCK_MASTER WHERE TIN = @TIN AND ITEM_CD = @ITEM_CD";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                double StockSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    StockSeq = double.Parse(firstColumn.ToString());
                }

                if(StockSeq < 0 && !string.IsNullOrEmpty(itemType) && itemType.Equals("3"))
                {
                    StockSeq = 0;
                }

                return (double)StockSeq;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }

        public List<StockMasterRecord> getStockMasterTable()
        {
            List<StockMasterRecord> arrayList = new List<StockMasterRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockMasterTable stockMasterTable = new StockMasterTable();

            try
            {
                command.CommandText = stockMasterTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockMasterRecord record = new StockMasterRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.ItemCd = reader.GetString(2);               // Item Code
                    if (!reader.IsDBNull(3)) record.RsdQty = reader.GetDouble(3);               // Resodual Quantity
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);        // Registered Date
                    if (!reader.IsDBNull(7)) record.ModrId = reader.GetString(7);               // Modifier ID
                    if (!reader.IsDBNull(8)) record.ModrNm = reader.GetString(8);               // Modifier Name
                    if (!reader.IsDBNull(9)) record.ModDt = reader.GetString(9);        // Modified Date

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

        public bool ToRecord(StockMasterRecord record, string valTin, string valBhfId, string valItemCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterTable stockMasterTable = new StockMasterTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(stockMasterTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND ITEM_CD = @ITEM_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = valBhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.ItemCd = reader.GetString(2);               // Item Code
                    if (!reader.IsDBNull(3)) record.RsdQty = reader.GetDouble(3);               // Resodual Quantity
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);        // Registered Date
                    if (!reader.IsDBNull(7)) record.ModrId = reader.GetString(7);               // Modifier ID
                    if (!reader.IsDBNull(8)) record.ModrNm = reader.GetString(8);               // Modifier Name
                    if (!reader.IsDBNull(9)) record.ModDt = reader.GetString(9);        // Modified Date

                    reader.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool UpdateTable(StockMasterRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterTable stockMasterTable = new StockMasterTable();
            StringBuilder sql = new StringBuilder();
            sql.Append("update STOCK_MASTER "); 
            sql.Append("   set RSD_QTY = @RsdQty, ");       // Resodual Quantity
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");               // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");          // Branch Office ID
            sql.Append("   and ITEM_CD = @ItemCd ");        // Item Code

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockMasterTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool InsertTable(StockMasterRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterTable stockMasterTable = new StockMasterTable();

            try
            {
                command.CommandText = stockMasterTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockMasterTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool DeleteTable(StockMasterRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterTable stockMasterTable = new StockMasterTable();

            try
            {
                command.CommandText = stockMasterTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockMasterTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool UpdateStock(StockMasterRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterTable stockMasterTable = new StockMasterTable();
            StringBuilder sql = new StringBuilder();
            sql.Append("update STOCK_MASTER ");                 
            sql.Append("   set RSD_QTY = RSD_QTY + @RsdQty, "); // Resodual Quantity
            sql.Append("       MODR_ID = @ModrId, ");           // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");           // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");             // Modified Date
            sql.Append(" where TIN = @Tin ");                   // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");              // Branch Office ID
            sql.Append("   and ITEM_CD = @ItemCd ");            // Item Code

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockMasterTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
    }
}

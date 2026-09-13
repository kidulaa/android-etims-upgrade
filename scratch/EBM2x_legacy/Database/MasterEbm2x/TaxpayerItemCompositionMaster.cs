using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.UI;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of TaxpayerItemCompositionMaster.
    /// </summary>
    public class TaxpayerItemCompositionMaster : ModelIO
    {
        public List<TaxpayerItemCompositionRecord> getTaxpayerItemCompositionTable(string tin, string itemCd)
        {
            List<TaxpayerItemCompositionRecord> arrayList = new List<TaxpayerItemCompositionRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemCompositionTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND ITEM_CD = @ITEM_CD ");

            StockMasterMaster stockMasterMaster = new StockMasterMaster();
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();

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
                param.ParameterName = "@ITEM_CD";
                param.Value = itemCd;
                command.Parameters.Add(param);


                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TaxpayerItemCompositionRecord record = new TaxpayerItemCompositionRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.ItemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.CpstItemCd = reader.GetString(2);           // Composition Item Code
                    if (!reader.IsDBNull(3)) record.CpstQty = reader.GetDouble(3);              // Composition Quantity
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    //if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);                // Registered Date

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TaxpayerItemCompositionRecord record in arrayList)
            {
                record.RdsQty = stockMasterMaster.GetCurrentStock(record.Tin, record.CpstItemCd); 

                TaxpayerItemRecord taxpayerItemRecord = new TaxpayerItemRecord();
                taxpayerItemMaster.ToRecord(taxpayerItemRecord, record.Tin, record.CpstItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                record.CpstItemNm = taxpayerItemRecord.ItemNm;
                record.CpstItemClsCd = taxpayerItemRecord.ItemClsCd;
                record.CpstItemClsNm = taxpayerItemRecord.ItemClsName;
            }

            return arrayList;
        }

        public bool ToRecord(TaxpayerItemCompositionRecord record, string valTin, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemCompositionTable.GetSelectSQL());
            sql.Append(" where TIN = @TIN ");
            sql.Append("   AND ITEM_CD = @ITEM_CD ");
            //sql.Append("   AND CPST_ITEM_CD = @CPST_ITEM_CD ");

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
                param.ParameterName = "@ITEM_CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                StockMasterMaster stockMasterMaster = new StockMasterMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.ItemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.CpstItemCd = reader.GetString(2);           // Composition Item Code
                    if (!reader.IsDBNull(3)) record.CpstQty = reader.GetDouble(3);              // Composition Quantity
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);                // Registered Date

                    reader.Close();

                    record.RdsQty = stockMasterMaster.GetCurrentStock(record.Tin, record.CpstItemCd); // 현재고 수량

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
        public bool ToRecord(TaxpayerItemCompositionRecord record, string valTin, string valItemCode, string valCpstCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemCompositionTable.GetSelectSQL());
            sql.Append(" where TIN = @TIN ");
            sql.Append("   AND ITEM_CD = @ITEM_CD ");
            sql.Append("   AND CPST_ITEM_CD = @CPST_ITEM_CD ");

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
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCode;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CPST_ITEM_CD";
                param.Value = valCpstCode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.ItemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.CpstItemCd = reader.GetString(2);           // Composition Item Code
                    if (!reader.IsDBNull(3)) record.CpstQty = reader.GetDouble(3);              // Composition Quantity
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    //if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);                // Registered Date

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

        public bool ToTable(TaxpayerItemCompositionRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();

            try
            {
                command.Parameters.Clear();
                taxpayerItemCompositionTable.SetParameters(command, record);

                command.CommandText = taxpayerItemCompositionTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerItemCompositionTable.GetInsertSQL();
                    command.CommandType = CommandType.Text;

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool InsertTable(List<TaxpayerItemCompositionRecord> listItemComposition)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();

            try
            {

                command.CommandText = taxpayerItemCompositionTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                bool result = false;
                for (int i = 0; i < listItemComposition.Count; i++)
                {
                    command.Parameters.Clear();
                    taxpayerItemCompositionTable.SetParameters(command, listItemComposition[i]);
                    if (command.ExecuteNonQuery() >= 1)
                    {
                        result = true;
                        continue;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool DeleteComposition(TaxpayerItemCompositionRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null || record == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from TAXPAYER_ITEM_COMPOSITION ");
            sql.AppendFormat(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.AppendFormat("   and ITEM_CD = @ItemCd ");  // Item Code

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = record.Tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = record.ItemCd;
                command.Parameters.Add(param);

                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool DeleteTable(TaxpayerItemCompositionRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();

            try
            {
                command.CommandText = taxpayerItemCompositionTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerItemCompositionTable.SetParameters(command, record);
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

using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.RraSdc.model;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ItemClassMaster.
    /// </summary>
    public class ItemClassMaster : ModelIO
    {
        public string GetItemClassName(string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT ITEM_CLS_NM FROM ITEM_CLASS WHERE ITEM_CLS_CD = @ITEM_CLS_CD";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CLS_CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (codeName == null) codeName = "";
                return codeName;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return "";
            }
        }

        public List<ItemClassRecord> getItemClassTable(string likeValue, string valueUseYn, string itemClsLvl)
        {
            List<ItemClassRecord> arrayList = new List<ItemClassRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ItemClassTable itemClassTable = new ItemClassTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(itemClassTable.GetSelectSQL());
            sql.Append(" where (ITEM_CLS_CD like @likeValue or ITEM_CLS_NM like @likeValue )");
            sql.Append("   and item_cls_lvl = @itemClsLvl");
            if (!string.IsNullOrEmpty(valueUseYn)) sql.Append(" and USE_YN = @USE_YN");
            sql.Append(" order by ITEM_CLS_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@USE_YN";
                param.Value = valueUseYn;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@itemClsLvl";
                param.Value = itemClsLvl;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ItemClassRecord record = new ItemClassRecord();

                    if (!reader.IsDBNull(0)) record.ItemClsCd = reader.GetString(0);            // Item Classification Code (RRA)
                    if (!reader.IsDBNull(1)) record.ItemClsLvl = reader.GetInt32(1);           // Item Category Code (UN/SPSC Code)
                    if (!reader.IsDBNull(2)) record.ItemClsNm = reader.GetString(2);            // Item Classification Name
                    if (!reader.IsDBNull(3)) record.TaxTyCd = reader.GetString(3);              // Taxation Type Code
                    if (!reader.IsDBNull(4)) record.MjrTgYn = reader.GetString(4);              // Major Taget(Y/N)
                    if (!reader.IsDBNull(5)) record.UseYn = reader.GetString(5);                // Use(Y/N)
                    if (!reader.IsDBNull(6)) record.Remark = reader.GetString(6);               // Remark
                    if (!reader.IsDBNull(7)) record.RegrId = reader.GetString(7);               // Registrant ID
                    if (!reader.IsDBNull(8)) record.RegrNm = reader.GetString(8);               // Registrant Name
                    if (!reader.IsDBNull(9)) record.RegDt = reader.GetString(9);        // Registered Date
                    if (!reader.IsDBNull(10)) record.ModrId = reader.GetString(10);             // Modifier ID
                    if (!reader.IsDBNull(11)) record.ModrNm = reader.GetString(11);             // Modifier Name
                    if (!reader.IsDBNull(12)) record.ModDt = reader.GetString(12);      // Modified Date

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

        public bool ToRecord(ItemClassRecord record, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ItemClassTable itemClassTable = new ItemClassTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(itemClassTable.GetSelectSQL());
            sql.Append(" where ITEM_CLS_CD = @ITEM_CLS_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CLS_CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.ItemClsCd = reader.GetString(0);            // Item Classification Code (RRA)
                    if (!reader.IsDBNull(1)) record.ItemClsLvl = reader.GetInt32(1);           // Item Category Code (UN/SPSC Code)
                    if (!reader.IsDBNull(2)) record.ItemClsNm = reader.GetString(2);            // Item Classification Name
                    if (!reader.IsDBNull(3)) record.TaxTyCd = reader.GetString(3);              // Taxation Type Code
                    if (!reader.IsDBNull(4)) record.MjrTgYn = reader.GetString(4);              // Major Taget(Y/N)
                    if (!reader.IsDBNull(5)) record.UseYn = reader.GetString(5);                // Use(Y/N)
                    if (!reader.IsDBNull(6)) record.Remark = reader.GetString(6);               // Remark
                    if (!reader.IsDBNull(7)) record.RegrId = reader.GetString(7);               // Registrant ID
                    if (!reader.IsDBNull(8)) record.RegrNm = reader.GetString(8);               // Registrant Name
                    if (!reader.IsDBNull(9)) record.RegDt = reader.GetString(9);        // Registered Date
                    if (!reader.IsDBNull(10)) record.ModrId = reader.GetString(10);             // Modifier ID
                    if (!reader.IsDBNull(11)) record.ModrNm = reader.GetString(11);             // Modifier Name
                    if (!reader.IsDBNull(12)) record.ModDt = reader.GetString(12);      // Modified Date

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

        public bool ToTable(ItemClassRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ItemClassTable itemClassTable = new ItemClassTable();

            try
            {
                command.Parameters.Clear();
                itemClassTable.SetParameters(command, record);

                command.CommandText = itemClassTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = itemClassTable.GetInsertSQL();
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
        public bool ToTableSDC(ItemClassLVO itemClass,ItemClassRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ItemClassTable itemClassTable = new ItemClassTable();

            try
            {
                command.Parameters.Clear();
                itemClassTable.SetParametersSDC(command, itemClass, record);

                command.CommandText = itemClassTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = itemClassTable.GetInsertSQL();
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

        public bool DeleteTable(ItemClassRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ItemClassTable itemClassTable = new ItemClassTable();

            try
            {
                command.CommandText = itemClassTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                itemClassTable.SetParameters(command, record);
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

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
    /// Description of StockIoMaster.
    /// </summary>
    public class StockIoMaster : ModelIO
    {
        public long GetStockIoSeq()
        {
            long stockIoItemSeq = GetStockIoItemSeq();

            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(SAR_NO), 0)  FROM STOCK_IO";
                command.CommandType = CommandType.Text;

                long StockIoSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    StockIoSeq = long.Parse(firstColumn.ToString());
                    //2021.02.26
                    if (StockIoSeq < stockIoItemSeq) StockIoSeq = stockIoItemSeq;
                }
                return (long)StockIoSeq + 1;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 1;
            }
        }
        public long GetStockIoItemSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(SAR_NO), 0)  FROM STOCK_IO_ITEM";
                command.CommandType = CommandType.Text;

                long StockIoSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    StockIoSeq = long.Parse(firstColumn.ToString());
                }
                return (long)StockIoSeq;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }
        public string GetStockIoDate()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT MIN(OCRN_DT)  FROM STOCK_IO";
                command.CommandType = CommandType.Text;

                var firstColumn = command.ExecuteScalar();
                return firstColumn.ToString();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return "";
            }
        }
        public bool IsStockIoTable(string sarTyCd, string custTin, string custBhfId, long orgSarNo)
        {
            bool ret = false;
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoTable stockIoTable = new StockIoTable();

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(stockIoTable.GetSelectSQL());
                sql.Append(" WHERE SAR_TY_CD = @SAR_TY_CD ");
                sql.Append("   AND CUST_TIN = @CUST_TIN ");
                sql.Append("   AND CUST_BHF_ID = @CUST_BHF_ID ");
                sql.Append("   AND ORG_SAR_NO = @ORG_SAR_NO ");

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@SAR_TY_CD";
                param.Value = sarTyCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CUST_TIN";
                param.Value = custTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CUST_BHF_ID";
                param.Value = custBhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ORG_SAR_NO";
                param.Value = orgSarNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    ret = true;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                ret = false;
            }

            return ret;
        }

        public List<StockIoRecord> getStockIoTable()
        {
            List<StockIoRecord> arrayList = new List<StockIoRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockIoTable stockIoTable = new StockIoTable();

            try
            {
                command.CommandText = stockIoTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockIoRecord record = new StockIoRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.SarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.OrgSarNo = GetLong(reader, 3);             // Original Stored and Released No.
                    if (!reader.IsDBNull(4)) record.RegTyCd = reader.GetString(4);              // Registration Type Code
                    if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer's Name
                    if (!reader.IsDBNull(6)) record.CustTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(7)) record.CustBhfId = reader.GetString(7);            // Customer Branch ID
                    if (!reader.IsDBNull(8)) record.CustNm = reader.GetString(8);               // Customer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(9)) record.InvcNo = reader.GetInt32(9);               // Onvoice No.
                    if (!reader.IsDBNull(9)) record.SarTyCd = reader.GetString(9);            // Stored and Released Type Code
                    //JCNA 202001 DELETE if (!reader.IsDBNull(11)) record.SarRsnCd = reader.GetString(11);           // Stored and Released Reason Code
                    if (!reader.IsDBNull(10)) record.OcrnDt = reader.GetString(10);     // Occurred Date time
                    if (!reader.IsDBNull(11)) record.TotItemCnt = reader.GetInt32(11);          // Total Item Count
                    if (!reader.IsDBNull(12)) record.TotTaxblAmt = reader.GetDouble(12);        // Total Taxable Amount
                    if (!reader.IsDBNull(13)) record.TotTaxAmt = reader.GetDouble(13);          // Total Tax Amount
                    if (!reader.IsDBNull(14)) record.TotAmt = reader.GetDouble(14);             // Total Amount
                    if (!reader.IsDBNull(15)) record.Remark = reader.GetString(15);             // Remark
                    if (!reader.IsDBNull(16)) record.RegrId = reader.GetString(16);             // Registrant ID
                    if (!reader.IsDBNull(17)) record.RegrNm = reader.GetString(17);             // Registrant Name
                    if (!reader.IsDBNull(18)) record.RegDt = reader.GetString(18);      // Registered Date
                    if (!reader.IsDBNull(19)) record.ModrId = reader.GetString(19);             // Modifier ID
                    if (!reader.IsDBNull(20)) record.ModrNm = reader.GetString(20);             // Modifier Name
                    if (!reader.IsDBNull(21)) record.ModDt = reader.GetString(21);      // Modified Date

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

        public bool ToRecord(StockIoRecord record, string valTin, string valBhfId, string valSarNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoTable stockIoTable = new StockIoTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(stockIoTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND SAR_NO = @SAR_NO ");

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
                param.ParameterName = "@SAR_NO";
                param.Value = valSarNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.SarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.OrgSarNo = GetLong(reader, 3);             // Original Stored and Released No.
                    if (!reader.IsDBNull(4)) record.RegTyCd = reader.GetString(4);              // Registration Type Code
                    if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer's Name
                    if (!reader.IsDBNull(6)) record.CustTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(7)) record.CustBhfId = reader.GetString(7);            // Customer Branch ID
                    if (!reader.IsDBNull(8)) record.CustNm = reader.GetString(8);               // Customer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(9)) record.InvcNo = reader.GetInt32(9);               // Onvoice No.
                    if (!reader.IsDBNull(9)) record.SarTyCd = reader.GetString(9);            // Stored and Released Type Code
                    //JCNA 202001 DELETE if (!reader.IsDBNull(11)) record.SarRsnCd = reader.GetString(11);           // Stored and Released Reason Code
                    if (!reader.IsDBNull(10)) record.OcrnDt = reader.GetString(10);     // Occurred Date time
                    if (!reader.IsDBNull(11)) record.TotItemCnt = reader.GetInt32(11);          // Total Item Count
                    if (!reader.IsDBNull(12)) record.TotTaxblAmt = reader.GetDouble(12);        // Total Taxable Amount
                    if (!reader.IsDBNull(13)) record.TotTaxAmt = reader.GetDouble(13);          // Total Tax Amount
                    if (!reader.IsDBNull(14)) record.TotAmt = reader.GetDouble(14);             // Total Amount
                    if (!reader.IsDBNull(15)) record.Remark = reader.GetString(15);             // Remark
                    if (!reader.IsDBNull(16)) record.RegrId = reader.GetString(16);             // Registrant ID
                    if (!reader.IsDBNull(17)) record.RegrNm = reader.GetString(17);             // Registrant Name
                    if (!reader.IsDBNull(18)) record.RegDt = reader.GetString(18);      // Registered Date
                    if (!reader.IsDBNull(19)) record.ModrId = reader.GetString(19);             // Modifier ID
                    if (!reader.IsDBNull(20)) record.ModrNm = reader.GetString(20);             // Modifier Name
                    if (!reader.IsDBNull(21)) record.ModDt = reader.GetString(21);      // Modified Date

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

        public bool ToTable(StockIoRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoTable stockIoTable = new StockIoTable();

            try
            {
                command.Parameters.Clear();
                stockIoTable.SetParameters(command, record);

                command.CommandText = stockIoTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = stockIoTable.GetInsertSQL();
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
        public bool InsertTable(StockIoRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoTable stockIoTable = new StockIoTable();

            try
            {
                command.CommandText = stockIoTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockIoTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool DeleteTable(StockIoRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoTable stockIoTable = new StockIoTable();

            try
            {
                command.CommandText = stockIoTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockIoTable.SetParameters(command, record);
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

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
    /// Description of TaxpayerBhfMaster.
    /// </summary>
    public class TaxpayerBhfMaster : ModelIO
    {
        public List<TaxpayerBhfRecord> getTaxpayerBhfTable()
        {
            List<TaxpayerBhfRecord> arrayList = new List<TaxpayerBhfRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfTable taxpayerBhfTable = new TaxpayerBhfTable();

            try
            {
                command.CommandText = taxpayerBhfTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TaxpayerBhfRecord record = new TaxpayerBhfRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.BhfNm = reader.GetString(2);                // Branch Name
                    if (!reader.IsDBNull(3)) record.BhfSttsCd = reader.GetString(3);            // Branch Status Code
                    if (!reader.IsDBNull(4)) record.PrvncNm = reader.GetString(4);               // Province No.
                    if (!reader.IsDBNull(5)) record.DstrtNm = reader.GetString(5);              // District No.
                    if (!reader.IsDBNull(6)) record.SctrNm = reader.GetString(6);               // Sector No.
                    if (!reader.IsDBNull(7)) record.LocDesc = reader.GetString(7);              // Location Description
                    if (!reader.IsDBNull(8)) record.MgrNm = reader.GetString(8);                // Manager Name
                    if (!reader.IsDBNull(9)) record.MgrTelNo = reader.GetString(9);             // Manager Telephone number
                    if (!reader.IsDBNull(10)) record.MgrEmail = reader.GetString(10);           // Manager Email
                    if (!reader.IsDBNull(11)) record.HqYn = reader.GetString(11);               // Headquarter(Y/N)
                    if (!reader.IsDBNull(12)) record.RegrId = reader.GetString(12);             // Registrant ID
                    if (!reader.IsDBNull(13)) record.RegrNm = reader.GetString(13);             // Registrant Name
                    if (!reader.IsDBNull(14)) record.RegDt = reader.GetString(14);      // Registered Date
                    if (!reader.IsDBNull(15)) record.ModrId = reader.GetString(15);             // Modifier ID
                    if (!reader.IsDBNull(16)) record.ModrNm = reader.GetString(16);             // Modifier Name
                    if (!reader.IsDBNull(17)) record.ModDt = reader.GetString(17);      // Modified Date

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

        public bool ToRecord(TaxpayerBhfRecord record, string valTin, string valBhfId)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfTable taxpayerBhfTable = new TaxpayerBhfTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");

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

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.BhfNm = reader.GetString(2);                // Branch Name
                    if (!reader.IsDBNull(3)) record.BhfSttsCd = reader.GetString(3);            // Branch Status Code
                    if (!reader.IsDBNull(4)) record.PrvncNm = reader.GetString(4);               // Province No.
                    if (!reader.IsDBNull(5)) record.DstrtNm = reader.GetString(5);              // District No.
                    if (!reader.IsDBNull(6)) record.SctrNm = reader.GetString(6);               // Sector No.
                    if (!reader.IsDBNull(7)) record.LocDesc = reader.GetString(7);              // Location Description
                    if (!reader.IsDBNull(8)) record.MgrNm = reader.GetString(8);                // Manager Name
                    if (!reader.IsDBNull(9)) record.MgrTelNo = reader.GetString(9);             // Manager Telephone number
                    if (!reader.IsDBNull(10)) record.MgrEmail = reader.GetString(10);           // Manager Email
                    if (!reader.IsDBNull(11)) record.HqYn = reader.GetString(11);               // Headquarter(Y/N)
                    if (!reader.IsDBNull(12)) record.RegrId = reader.GetString(12);             // Registrant ID
                    if (!reader.IsDBNull(13)) record.RegrNm = reader.GetString(13);             // Registrant Name
                    if (!reader.IsDBNull(14)) record.RegDt = reader.GetString(14);      // Registered Date
                    if (!reader.IsDBNull(15)) record.ModrId = reader.GetString(15);             // Modifier ID
                    if (!reader.IsDBNull(16)) record.ModrNm = reader.GetString(16);             // Modifier Name
                    if (!reader.IsDBNull(17)) record.ModDt = reader.GetString(17);      // Modified Date

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

        public bool ToTable(TaxpayerBhfRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfTable taxpayerBhfTable = new TaxpayerBhfTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBhfTable.SetParameters(command, record);
                command.CommandText = taxpayerBhfTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBhfTable.GetInsertSQL();
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
        public bool ToTableSDC(Bhf bhf, TaxpayerBhfRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(bhf.locDesc)) bhf.locDesc = "";

            TaxpayerBhfTable taxpayerBhfTable = new TaxpayerBhfTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBhfTable.SetParametersSDC(command, bhf, record);
                command.CommandText = taxpayerBhfTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBhfTable.GetInsertSQL();
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

        public bool DeleteTable(TaxpayerBhfRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfTable taxpayerBhfTable = new TaxpayerBhfTable();

            try
            {
                command.CommandText = taxpayerBhfTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerBhfTable.SetParameters(command, record);
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

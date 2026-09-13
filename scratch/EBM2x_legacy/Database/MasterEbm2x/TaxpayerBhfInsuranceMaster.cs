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
    /// Description of TaxpayerBhfInsuranceMaster.
    /// </summary>
    public class TaxpayerBhfInsuranceMaster : ModelIO
    {
        public List<TaxpayerBhfInsuranceRecord> getTaxpayerBhfInsuranceTable()
        {
            List<TaxpayerBhfInsuranceRecord> arrayList = new List<TaxpayerBhfInsuranceRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfInsuranceTable taxpayerBhfInsuranceTable = new TaxpayerBhfInsuranceTable();

            try
            {
                command.CommandText = taxpayerBhfInsuranceTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TaxpayerBhfInsuranceRecord record = new TaxpayerBhfInsuranceRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.IssrccCd = reader.GetString(2);             // Insurance Company Code
                    if (!reader.IsDBNull(3)) record.IsrccNm = reader.GetString(3);              // Insurance Company Name
                    if (!reader.IsDBNull(4)) record.IsrcRt = reader.GetInt16(4);                // Insurance Rate
                    if (!reader.IsDBNull(5)) record.UseYn = reader.GetString(5);                // Use(Y/N)
                    if (!reader.IsDBNull(6)) record.ModrId = reader.GetString(6);               // Modifier ID
                    if (!reader.IsDBNull(7)) record.ModrNm = reader.GetString(7);               // Modifier Name
                    if (!reader.IsDBNull(8)) record.ModDt = reader.GetString(8);        // Modified Date
                    if (!reader.IsDBNull(9)) record.RegrId = reader.GetString(9);               // Registrant ID
                    if (!reader.IsDBNull(10)) record.RegrNm = reader.GetString(10);             // Registrant Name
                    if (!reader.IsDBNull(11)) record.RegDt = reader.GetString(11);      // Registered Date

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

        public bool ToRecord(TaxpayerBhfInsuranceRecord record, string valTin, string valBhfId, string valIssrccCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfInsuranceTable taxpayerBhfInsuranceTable = new TaxpayerBhfInsuranceTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfInsuranceTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND ISSRCC_CD = @ISSRCC_CD ");

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
                param.ParameterName = "@ISSRCC_CD";
                param.Value = valIssrccCd;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.IssrccCd = reader.GetString(2);             // Insurance Company Code
                    if (!reader.IsDBNull(3)) record.IsrccNm = reader.GetString(3);              // Insurance Company Name
                    if (!reader.IsDBNull(4)) record.IsrcRt = reader.GetInt16(4);                // Insurance Rate
                    if (!reader.IsDBNull(5)) record.UseYn = reader.GetString(5);                // Use(Y/N)
                    if (!reader.IsDBNull(6)) record.ModrId = reader.GetString(6);               // Modifier ID
                    if (!reader.IsDBNull(7)) record.ModrNm = reader.GetString(7);               // Modifier Name
                    if (!reader.IsDBNull(8)) record.ModDt = reader.GetString(8);        // Modified Date
                    if (!reader.IsDBNull(9)) record.RegrId = reader.GetString(9);               // Registrant ID
                    if (!reader.IsDBNull(10)) record.RegrNm = reader.GetString(10);             // Registrant Name
                    if (!reader.IsDBNull(11)) record.RegDt = reader.GetString(11);      // Registered Date

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

        public bool ToTable(TaxpayerBhfInsuranceRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfInsuranceTable taxpayerBhfInsuranceTable = new TaxpayerBhfInsuranceTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBhfInsuranceTable.SetParameters(command, record);

                command.CommandText = taxpayerBhfInsuranceTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBhfInsuranceTable.GetInsertSQL();
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

        public bool DeleteTable(TaxpayerBhfInsuranceRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfInsuranceTable taxpayerBhfInsuranceTable = new TaxpayerBhfInsuranceTable();

            try
            {
                command.CommandText = taxpayerBhfInsuranceTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerBhfInsuranceTable.SetParameters(command, record);
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

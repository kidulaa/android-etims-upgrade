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
    /// Description of TaxpayerBaseMaster.
    /// </summary>
    public class TaxpayerBaseMaster : ModelIO
    {
        public List<TaxpayerBaseRecord> getTaxpayerBaseTable(string likeValue, string valueUseYn)
        {
            List<TaxpayerBaseRecord> arrayList = new List<TaxpayerBaseRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBaseTable taxpayerBaseTable = new TaxpayerBaseTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBaseTable.GetSelectSQL());
            sql.Append(" where TIN like @likeValue or TAXPR_NM like @likeValue ");
            sql.Append(" order by TIN ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TaxpayerBaseRecord record = new TaxpayerBaseRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                    // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.TaxprNm = reader.GetString(1);                // Taxpayer's Name
                    if (!reader.IsDBNull(2)) record.TaxprSttsCd = reader.GetString(2);            // Taxpayer Status Code
                    if (!reader.IsDBNull(3)) record.BsnsActv = reader.GetString(3);               // Business Activities
                    if (!reader.IsDBNull(4)) record.PrvncNm = reader.GetString(4);                 // Province No.
                    if (!reader.IsDBNull(5)) record.DstrtNm = reader.GetString(5);                // District No.
                    if (!reader.IsDBNull(6)) record.SctrNm = reader.GetString(6);                 // Sector No.
                    if (!reader.IsDBNull(7)) record.LocDesc = reader.GetString(7);                // Location Description
                    if (!reader.IsDBNull(8)) record.TelNo = reader.GetString(8);                  // Telephone number
                    if (!reader.IsDBNull(9)) record.Email = reader.GetString(9);                  // Email
                    if (!reader.IsDBNull(10)) record.BankCd = reader.GetString(10);               // Bank Code
                    if (!reader.IsDBNull(11)) record.BankAccntNo = reader.GetString(11);          // Bank Account Number
                    if (!reader.IsDBNull(12)) record.BankAccntHldr = reader.GetString(12);        // Bank Account Holder
                    if (!reader.IsDBNull(13)) record.ApcntNm = reader.GetString(13);              // Applicant name
                    if (!reader.IsDBNull(14)) record.ApcntTelno = reader.GetString(14);           // Applicant telephone number
                    if (!reader.IsDBNull(15)) record.ApcntEmail = reader.GetString(15);           // Applicant Email
                    if (!reader.IsDBNull(16)) record.Remark = reader.GetString(16);               // Remark
                    if (!reader.IsDBNull(17)) record.EbmTyCd = reader.GetString(17);              // EBM Type Code
                    if (!reader.IsDBNull(18)) record.UserNo = reader.GetString(18);               // User No.
                    if (!reader.IsDBNull(19)) record.RegrId = reader.GetString(19);               // Registrant ID
                    if (!reader.IsDBNull(20)) record.RegrNm = reader.GetString(20);               // Registrant Name
                    if (!reader.IsDBNull(21)) record.RegDt = reader.GetString(21);        // Registered Date
                    if (!reader.IsDBNull(22)) record.ModrId = reader.GetString(22);               // Modifier ID
                    if (!reader.IsDBNull(23)) record.ModrNm = reader.GetString(23);               // Modifier Name
                    if (!reader.IsDBNull(24)) record.ModDt = reader.GetString(24);        // Modified Date

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

        public bool ToRecord(TaxpayerBaseRecord record, string valTin)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBaseTable taxpayerBaseTable = new TaxpayerBaseTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBaseTable.GetSelectSQL());
            sql.Append(" where TIN = @TIN ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                    // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.TaxprNm = reader.GetString(1);                // Taxpayer's Name
                    if (!reader.IsDBNull(2)) record.TaxprSttsCd = reader.GetString(2);            // Taxpayer Status Code
                    if (!reader.IsDBNull(3)) record.BsnsActv = reader.GetString(3);               // Business Activities
                    if (!reader.IsDBNull(4)) record.PrvncNm = reader.GetString(4);                 // Province No.
                    if (!reader.IsDBNull(5)) record.DstrtNm = reader.GetString(5);                // District No.
                    if (!reader.IsDBNull(6)) record.SctrNm = reader.GetString(6);                 // Sector No.
                    if (!reader.IsDBNull(7)) record.LocDesc = reader.GetString(7);                // Location Description
                    if (!reader.IsDBNull(8)) record.TelNo = reader.GetString(8);                  // Telephone number
                    if (!reader.IsDBNull(9)) record.Email = reader.GetString(9);                  // Email
                    if (!reader.IsDBNull(10)) record.BankCd = reader.GetString(10);               // Bank Code
                    if (!reader.IsDBNull(11)) record.BankAccntNo = reader.GetString(11);          // Bank Account Number
                    if (!reader.IsDBNull(12)) record.BankAccntHldr = reader.GetString(12);        // Bank Account Holder
                    if (!reader.IsDBNull(13)) record.ApcntNm = reader.GetString(13);              // Applicant name
                    if (!reader.IsDBNull(14)) record.ApcntTelno = reader.GetString(14);           // Applicant telephone number
                    if (!reader.IsDBNull(15)) record.ApcntEmail = reader.GetString(15);           // Applicant Email
                    if (!reader.IsDBNull(16)) record.Remark = reader.GetString(16);               // Remark
                    if (!reader.IsDBNull(17)) record.EbmTyCd = reader.GetString(17);              // EBM Type Code
                    if (!reader.IsDBNull(18)) record.UserNo = reader.GetString(18);               // User No.
                    if (!reader.IsDBNull(19)) record.RegrId = reader.GetString(19);               // Registrant ID
                    if (!reader.IsDBNull(20)) record.RegrNm = reader.GetString(20);               // Registrant Name
                    if (!reader.IsDBNull(21)) record.RegDt = reader.GetString(21);        // Registered Date
                    if (!reader.IsDBNull(22)) record.ModrId = reader.GetString(22);               // Modifier ID
                    if (!reader.IsDBNull(23)) record.ModrNm = reader.GetString(23);               // Modifier Name
                    if (!reader.IsDBNull(24)) record.ModDt = reader.GetString(24);        // Modified Date

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

        public bool ToTable(TaxpayerBaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBaseTable taxpayerBaseTable = new TaxpayerBaseTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBaseTable.SetParameters(command, record);
 
                command.CommandText = taxpayerBaseTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBaseTable.GetInsertSQL();
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
        public bool ToTableSDC(CustomerTin customerTin, TaxpayerBaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBaseTable taxpayerBaseTable = new TaxpayerBaseTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBaseTable.SetParametersSDC(command, customerTin, record);

                command.CommandText = taxpayerBaseTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBaseTable.GetInsertSQL();
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

        public bool DeleteTable(TaxpayerBaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBaseTable taxpayerBaseTable = new TaxpayerBaseTable();

            try
            {
                command.CommandText = taxpayerBaseTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerBaseTable.SetParameters(command, record);
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

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
    /// Description of BbsNoticeMaster.
    /// </summary>
    public class BbsNoticeMaster : ModelIO
    {
        public List<BbsNoticeRecord> getBbsNoticeTable(string curDate)
        {
            List<BbsNoticeRecord> arrayList = new List<BbsNoticeRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            BbsNoticeTable bbsNoticeTable = new BbsNoticeTable();

            try
            {
                command.CommandText = bbsNoticeTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BbsNoticeRecord record = new BbsNoticeRecord();

                    if (!reader.IsDBNull(0)) record.NoticeNo = reader.GetInt16(0);              // Notice No.
                    if (!reader.IsDBNull(1)) record.Title = reader.GetString(1);                // Title
                    if (!reader.IsDBNull(2)) record.Cont = reader.GetString(2);                 // Contents
                    if (!reader.IsDBNull(3)) record.DtlUrl = reader.GetString(3);                 // Read Count
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(6)) record.ModrId = reader.GetString(6);               // Modifier ID
                    if (!reader.IsDBNull(7)) record.ModrNm = reader.GetString(7);               // Modifier Name
                    if (!reader.IsDBNull(8)) record.ModDt = reader.GetString(8);                // Modified Date
                    if (!reader.IsDBNull(9)) record.RegDt = reader.GetString(9);                // Registered Date

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
        public int getBbsNoticeCount(string curDate)
        {
            int count = 0;

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            BbsNoticeTable bbsNoticeTable = new BbsNoticeTable();

            try
            {
                command.CommandText = bbsNoticeTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    count++;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return count;
        }

        public bool ToRecord(BbsNoticeRecord record, int valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            BbsNoticeTable bbsNoticeTable = new BbsNoticeTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(bbsNoticeTable.GetSelectSQL());
            sql.Append(" where NOTICE_NO = @NOTICE_NO ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@NOTICE_NO";
                param.Value = valCode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.NoticeNo = reader.GetInt16(0);              // Notice No.
                    if (!reader.IsDBNull(1)) record.Title = reader.GetString(1);                // Title
                    if (!reader.IsDBNull(2)) record.Cont = reader.GetString(2);                 // Contents
                    if (!reader.IsDBNull(3)) record.DtlUrl = reader.GetString(3);                 // Read Count
                    if (!reader.IsDBNull(4)) record.RegrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.RegrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(6)) record.ModrId = reader.GetString(6);               // Modifier ID
                    if (!reader.IsDBNull(7)) record.ModrNm = reader.GetString(7);               // Modifier Name
                    if (!reader.IsDBNull(8)) record.ModDt = reader.GetString(8);                // Modified Date
                    if (!reader.IsDBNull(9)) record.RegDt = reader.GetString(9);                // Registered Date

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

        public bool ToTable(BbsNoticeRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            BbsNoticeTable bbsNoticeTable = new BbsNoticeTable();

            try
            {
                command.Parameters.Clear();
                bbsNoticeTable.SetParameters(command, record);

                command.CommandText = bbsNoticeTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = bbsNoticeTable.GetInsertSQL();
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
        public bool ToTableSDC(Notice notice, BbsNoticeRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            BbsNoticeTable bbsNoticeTable = new BbsNoticeTable();

            try
            {
                command.Parameters.Clear();
                bbsNoticeTable.SetParametersSDC(command, notice, record);

                command.CommandText = bbsNoticeTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = bbsNoticeTable.GetInsertSQL();
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

        public bool DeleteTable(BbsNoticeRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            BbsNoticeTable bbsNoticeTable = new BbsNoticeTable();

            try
            {
                command.CommandText = bbsNoticeTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                bbsNoticeTable.SetParameters(command, record);
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

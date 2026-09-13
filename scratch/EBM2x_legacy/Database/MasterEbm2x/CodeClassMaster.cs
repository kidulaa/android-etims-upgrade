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
    /// Description of CodeClassMaster.
    /// </summary>
    public class CodeClassMaster : ModelIO
    {
        public List<CodeClassRecord> getCodeClassTable()
        {
            List<CodeClassRecord> arrayList = new List<CodeClassRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            CodeClassTable codeClassTable = new CodeClassTable();

            try
            {
                command.CommandText = codeClassTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    CodeClassRecord record = new CodeClassRecord();

                    if (!reader.IsDBNull(0)) record.CdCls = reader.GetString(0);                // Code classification
                    if (!reader.IsDBNull(1)) record.CdClsNm = reader.GetString(1);              // Name of code classification
                    if (!reader.IsDBNull(2)) record.CdClsDesc = reader.GetString(2);            // Description of code classification
                    if (!reader.IsDBNull(3)) record.UserDfnNm1 = reader.GetString(3);           // User Define Name 1
                    if (!reader.IsDBNull(4)) record.UserDfnNm2 = reader.GetString(4);           // User Define Name 2
                    if (!reader.IsDBNull(5)) record.UserDfnNm3 = reader.GetString(5);           // User Define Name 3
                    //JCNA 202001 DELETE
                    //if (!reader.IsDBNull(6)) record.ClientUseYn = reader.GetString(6);          // Use of Client(Y/N)
                    if (!reader.IsDBNull(6)) record.UseYn = reader.GetString(6);                // Use(Y/N)
                    if (!reader.IsDBNull(7)) record.RegrId = reader.GetString(7);               // Registrant ID
                    if (!reader.IsDBNull(8)) record.RegrNm = reader.GetString(8);               // Registrant Name
                    if (!reader.IsDBNull(9)) record.RegDt = reader.GetString(9);               // Registered Date
                    if (!reader.IsDBNull(10)) record.ModrId = reader.GetString(10);             // Modifier ID
                    if (!reader.IsDBNull(11)) record.ModrNm = reader.GetString(11);             // Modifier Name
                    if (!reader.IsDBNull(12)) record.ModDt = reader.GetString(12);              // Modified Date

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

        public bool ToRecord(CodeClassRecord record, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeClassTable codeClassTable = new CodeClassTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(codeClassTable.GetSelectSQL());
            sql.Append(" where CD_CLS = @CD_CLS ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CD_CLS";
                param.Value = valCode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.CdCls = reader.GetString(0);                // Code classification
                    if (!reader.IsDBNull(1)) record.CdClsNm = reader.GetString(1);              // Name of code classification
                    if (!reader.IsDBNull(2)) record.CdClsDesc = reader.GetString(2);            // Description of code classification
                    if (!reader.IsDBNull(3)) record.UserDfnNm1 = reader.GetString(3);           // User Define Name 1
                    if (!reader.IsDBNull(4)) record.UserDfnNm2 = reader.GetString(4);           // User Define Name 2
                    if (!reader.IsDBNull(5)) record.UserDfnNm3 = reader.GetString(5);           // User Define Name 3
                    //JCNA 202001 DELETE
                    //if (!reader.IsDBNull(6)) record.ClientUseYn = reader.GetString(6);          // Use of Client(Y/N)
                    if (!reader.IsDBNull(6)) record.UseYn = reader.GetString(6);                // Use(Y/N)
                    if (!reader.IsDBNull(7)) record.RegrId = reader.GetString(7);               // Registrant ID
                    if (!reader.IsDBNull(8)) record.RegrNm = reader.GetString(8);               // Registrant Name
                    if (!reader.IsDBNull(9)) record.RegDt = reader.GetString(9);               // Registered Date
                    if (!reader.IsDBNull(10)) record.ModrId = reader.GetString(10);             // Modifier ID
                    if (!reader.IsDBNull(11)) record.ModrNm = reader.GetString(11);             // Modifier Name
                    if (!reader.IsDBNull(12)) record.ModDt = reader.GetString(12);              // Modified Date

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

        public bool ToTable(CodeClassRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeClassTable codeClassTable = new CodeClassTable();

            try
            {
                command.Parameters.Clear();
                codeClassTable.SetParameters(command, record);

                command.CommandText = codeClassTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = codeClassTable.GetInsertSQL();
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

        public bool DeleteTable(CodeClassRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeClassTable codeClassTable = new CodeClassTable();

            try
            {
                command.CommandText = codeClassTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                codeClassTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool ToTableSDC(CodeClassLVO record, CodeClassRecord record2)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeClassTable codeClassTable = new CodeClassTable();

            try
            {
                command.Parameters.Clear();
                codeClassTable.SetParametersSDC(command, record, record2);
                command.CommandText = codeClassTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = codeClassTable.GetInsertSQL();
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
    }
}

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
    /// Description of CodeDtlMaster.
    /// </summary>
    public class CodeDtlMaster : ModelIO
    {
        public string OrgnNatName(string valCode)
        {
            return GetCodeName(valCode, "05");
        }
        public string ItemTyName(string valCode)
        {
            return GetCodeName(valCode, "24");
        }
        public string PkgUnitName(string valCode)
        {
            return GetCodeName(valCode, "17");
        }
        public string QtyUnitName(string valCode)
        {
            return GetCodeName(valCode, "10");
        }
        public string TaxTyName(string valCode)
        {
            return GetCodeName(valCode, "04");
        }

        public string SalesSttsName(string valCode)
        {
            return GetCodeName(valCode, "11");
        }
        public string PchsSttsName(string valCode)
        {
            return GetCodeName(valCode, "34");
        }
        public string RoleName(string valCode)
        {
            return GetCodeName(valCode, "28");
        }
        public string ImptItemSttsNm(string valCode)
        {
            return GetCodeName(valCode, "26");
        }
        public string InvcFcurNm(string valCode)
        {
            return GetCodeName(valCode, "33");
        }

        public string GetCodeName(string valCode, string valCodeCls)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = @CD_CLS AND CD = @CD";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CD_CLS";
                param.Value = valCodeCls;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (codeName == null) codeName = "";
                return codeName;

                //string codeName = "";
                //IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                //if (reader.Read())
                //{
                //    if (!reader.IsDBNull(0)) codeName =  reader.GetString(0);
                //}
                //reader.Close();
                //return codeName;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return "";
            }
        }

        public List<CodeDtlRecord> getCodeDtlTable(string valCodeCls, string likeValue)
        {
            List<CodeDtlRecord> arrayList = new List<CodeDtlRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(codeDtlTable.GetSelectSQL());
            sql.Append(" where CD_CLS = @CD_CLS");
            if(!string.IsNullOrEmpty(likeValue)) sql.Append("   and (CD like @likeValue or CD_NM like @likeValue ) and USE_YN = 'Y'");
            sql.Append(" order by CD_NM ASC ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CD_CLS";
                param.Value = valCodeCls;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    CodeDtlRecord record = new CodeDtlRecord();

                    if (!reader.IsDBNull(0)) record.Cd = reader.GetString(0);                   // Code
                    if (!reader.IsDBNull(1)) record.CdCls = reader.GetString(1);                // Code classification
                    if (!reader.IsDBNull(2)) record.CdNm = reader.GetString(2);                 // Name of Code
                    if (!reader.IsDBNull(3)) record.CdDesc = reader.GetString(3);               // Description of the Code
                    if (!reader.IsDBNull(4)) record.SrtOrd = reader.GetInt16(4);                // Sort Order
                    if (!reader.IsDBNull(5)) record.UserDfnCd1 = reader.GetString(5);           // User Define Code 1
                    if (!reader.IsDBNull(6)) record.UserDfnCd2 = reader.GetString(6);           // User Define Code 2
                    if (!reader.IsDBNull(7)) record.UserDfnCd3 = reader.GetString(7);           // User Define Code 3
                    if (!reader.IsDBNull(8)) record.UseYn = reader.GetString(8);                // Use(Y/N)
                    if (!reader.IsDBNull(9)) record.RegrId = reader.GetString(9);               // Registrant ID
                    if (!reader.IsDBNull(10)) record.RegrNm = reader.GetString(10);             // Registrant Name
                    if (!reader.IsDBNull(11)) record.RegDt = reader.GetString(11);      // Registered Date
                    if (!reader.IsDBNull(12)) record.ModrId = reader.GetString(12);             // Modifier ID
                    if (!reader.IsDBNull(13)) record.ModrNm = reader.GetString(13);             // Modifier Name
                    if (!reader.IsDBNull(14)) record.ModDt = reader.GetString(14);      // Modified Date

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

        public bool ToRecord(CodeDtlRecord record, string valCode, string valCodeCls)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(codeDtlTable.GetSelectSQL());
            sql.Append(" WHERE CD = @CD ");
            sql.Append("   AND CD_CLS = @CD_CLS ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CD_CLS";
                param.Value = valCodeCls;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Cd = reader.GetString(0);                   // Code
                    if (!reader.IsDBNull(1)) record.CdCls = reader.GetString(1);                // Code classification
                    if (!reader.IsDBNull(2)) record.CdNm = reader.GetString(2);                 // Name of Code
                    if (!reader.IsDBNull(3)) record.CdDesc = reader.GetString(3);               // Description of the Code
                    if (!reader.IsDBNull(4)) record.SrtOrd = reader.GetInt16(4);                // Sort Order
                    if (!reader.IsDBNull(5)) record.UserDfnCd1 = reader.GetString(5);           // User Define Code 1
                    if (!reader.IsDBNull(6)) record.UserDfnCd2 = reader.GetString(6);           // User Define Code 2
                    if (!reader.IsDBNull(7)) record.UserDfnCd3 = reader.GetString(7);           // User Define Code 3
                    if (!reader.IsDBNull(8)) record.UseYn = reader.GetString(8);                // Use(Y/N)
                    if (!reader.IsDBNull(9)) record.RegrId = reader.GetString(9);               // Registrant ID
                    if (!reader.IsDBNull(10)) record.RegrNm = reader.GetString(10);             // Registrant Name
                    if (!reader.IsDBNull(11)) record.RegDt = reader.GetString(11);      // Registered Date
                    if (!reader.IsDBNull(12)) record.ModrId = reader.GetString(12);             // Modifier ID
                    if (!reader.IsDBNull(13)) record.ModrNm = reader.GetString(13);             // Modifier Name
                    if (!reader.IsDBNull(14)) record.ModDt = reader.GetString(14);      // Modified Date

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

        public bool ToTable(CodeDtlRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();

            try
            {
                command.Parameters.Clear();
                codeDtlTable.SetParameters(command, record);

                command.CommandText = codeDtlTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = codeDtlTable.GetInsertSQL();
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

        public bool DeleteTable(CodeDtlRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();

            try
            {
                command.CommandText = codeDtlTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                codeDtlTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool ToTableSDC(CodeDtlLVO record, CodeClassLVO record2, CodeClassRecord record3)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();

            try
            {
                command.CommandText = codeDtlTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                codeDtlTable.SetParametersSDC(command, record, record2, record3);
                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = codeDtlTable.GetInsertSQL();
                    command.CommandType = CommandType.Text;

                    command.Parameters.Clear();
                    codeDtlTable.SetParametersSDC(command, record, record2, record3);
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

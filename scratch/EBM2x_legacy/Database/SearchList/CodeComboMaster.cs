using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Models.config;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of CodeComboMaster.
    /// </summary>
    public class CodeComboMaster : ModelIO
    {
        public List<SystemCode> LoadBhf(string bhf)
        {
            List<SystemCode> arrayList = new List<SystemCode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT BHF_ID, ");                      // Code
            sql.Append("       BHF_NM  ");                   // Name of Code
            sql.Append("  FROM TAXPAYER_BHF ");                 // 内靛 惑技
            sql.Append(" WHERE BHF_ID<> '" + bhf + "' ");
            //            sql.Append(" ORDER BY @ORDER_BY ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SystemCode record = new SystemCode();

                    if (!reader.IsDBNull(0)) record.Id = reader.GetString(0);                   // Code
                    if (!reader.IsDBNull(1)) record.Name = reader.GetString(0) + " " + reader.GetString(1);                 // Name of Code

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

        public List<SystemCode> LoadCombo(string valCdCls, string valOrderBy)
        {
            List<SystemCode> arrayList = new List<SystemCode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT CD, ");                      // Code
            sql.Append("       CD_NM  ");                   // Name of Code
            sql.Append("  FROM CODE_DTL ");                 // 内靛 惑技
            sql.Append(" WHERE CD_CLS = @CD_CLS ");
            sql.Append(" ORDER BY CD_NM ASC ");
            //            sql.Append(" ORDER BY @ORDER_BY ");
            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CD_CLS";
                param.Value = valCdCls;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ORDER_BY";
                param.Value = valOrderBy;
                command.Parameters.Add(param);
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SystemCode record = new SystemCode();

                    if (!reader.IsDBNull(0)) record.Id = reader.GetString(0);                   // Code
                    if (!reader.IsDBNull(1)) record.Name = reader.GetString(1);                 // Name of Code

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
        public List<SystemCode> LoadComboTax(string valCdCls, string valOrderBy, string vatTyCd)
        {
            List<SystemCode> arrayList = new List<SystemCode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT CD, ");                      // Code
            sql.Append("       CD_NM  ");                   // Name of Code
            sql.Append("  FROM CODE_DTL ");                 // 内靛 惑技
            sql.Append(" WHERE CD_CLS = @CD_CLS ");
            if (vatTyCd == "1")
            {
                sql.Append(" ORDER BY CD_NM ASC LIMIT 3");
            }
            else
            {
                sql.Append(" ORDER BY CD_NM DESC LIMIT 1");
            }

            //            sql.Append(" ORDER BY @ORDER_BY ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CD_CLS";
                param.Value = valCdCls;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ORDER_BY";
                param.Value = valOrderBy;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@VAT_TY_CD";
                param.Value = vatTyCd;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SystemCode record = new SystemCode();

                    if (!reader.IsDBNull(0)) record.Id = reader.GetString(0);                   // Code
                    if (!reader.IsDBNull(1)) record.Name = reader.GetString(1);                 // Name of Code

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
    }
}

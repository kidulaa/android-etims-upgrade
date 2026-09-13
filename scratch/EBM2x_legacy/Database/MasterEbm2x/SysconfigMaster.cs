using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.Master;
    using EBM2x.Database.TableIO;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of SysconfigMaster.
    /// </summary>
    public class SysconfigMaster : ModelIO
    {
        public List<SysconfigRecord> getSysconfigTable()
        {
            List<SysconfigRecord> arrayList = new List<SysconfigRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select CONFIG_CD          , ");                        
            sql.Append("       CONFIG_VALUE       , ");                         
            sql.Append("       UPD_TY_CD            ");                         
            sql.Append("  from SYSCONFIG          ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SysconfigRecord record = new SysconfigRecord();

                    record.ConfigCd = reader.GetString(0);      
                    record.ConfigValue = reader.GetString(1);     
                    record.UpdTyCd = reader.GetString(2);

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

        public bool ToRecord(SysconfigRecord record, string keycode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select CONFIG_CD          , "); 
            sql.Append("       CONFIG_VALUE       , ");
            sql.Append("       UPD_TY_CD            ");
            sql.Append("  from SYSCONFIG          ");
            sql.Append(" where CONFIG_CD = @CONFIG_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@CONFIG_CD";
                param.Value = keycode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    record.ConfigCd = reader.GetString(0);
                    record.ConfigValue = reader.GetString(1);
                    record.UpdTyCd = reader.GetString(2);

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
    }
}

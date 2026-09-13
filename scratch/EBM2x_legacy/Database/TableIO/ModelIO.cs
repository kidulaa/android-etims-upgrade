using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Data;

namespace EBM2x.Database.TableIO
{
    public class ModelIO
    {
        public IDbCommand GetDbCommand()
        {
            EBM2xDBClientProvider provider = EBM2xDBClientProvider.getInstance();
            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
            {
                string DBServer = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer;
                string Database = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLDatabase;
                string DBUid = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLUid;
                string DBPwd = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLPwd;
                provider.OpenConnection(DBServer, Database, DBUid, DBPwd);
            }
            else
            {
                provider.OpenConnection("", "", "", "");
            }

            try
            {
                if (provider.Connected())
                {
                    return provider.GetDbCommand();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return null;
            }
        }

        public int ExecuteNonQuery(string strSql, IDbCommand command)
        {
            try
            {
                command.CommandText = strSql;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }

        public string MakeLikeString(string strSql)
        {
            if (string.IsNullOrEmpty(strSql)) return "%";
            return strSql + "%";
        }
        public static long GetLong(IDataReader reader, int index)
        {
            try
            {
                return reader.GetInt64(index);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}

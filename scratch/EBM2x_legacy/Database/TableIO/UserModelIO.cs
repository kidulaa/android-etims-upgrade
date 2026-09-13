using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class UserModelIO : ModelIO
    {

        public int SetUserDeleted(string rowID)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE USREMP ");
            sql.Append("    SET COMM_F  = 'D' ");
            sql.Append("  WHERE rowid = @rowid ");

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@rowid";
            param.Value = rowID;
            command.Parameters.Add(param);

            return ExecuteNonQuery(sql.ToString(), command);
        }
    }
}

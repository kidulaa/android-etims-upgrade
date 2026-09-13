using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class ItemModelIO : ModelIO
    {

        public int SetItemDeleted(string rowID)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE ITMITEM ");
            sql.Append("    SET COMM_F = 'D' ");
            sql.Append("  WHERE rowid = @rowid ");

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@rowid";
            param.Value = rowID;
            command.Parameters.Add(param);

            return ExecuteNonQuery(sql.ToString(), command);
        }

        public int SetInventoryDetele(string strItemCode, string strItemClassCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE STCINVENTORY ");
            sql.Append("    SET COMM_F = 'D' ");
            sql.Append("  WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("    AND ITEM_CLS_CD = @ITEM_CLS_CD ");

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@ITEM_CD";
            param.Value = strItemCode;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@ITEM_CLS_CD";
            param.Value = strItemClassCode;
            command.Parameters.Add(param);

            return ExecuteNonQuery(sql.ToString(), command);
        }

        public int HasRecordHistory(string strItemCode)
        {            
            int countVal = 0;
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return countVal;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT SUM(CNT) AS CNT FROM ( ");
            sql.Append("        SELECT COUNT(*) AS CNT FROM STCWHIOITEM ");
            sql.Append("        WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("  UNION SELECT SUM(QTY) AS CNT FROM STCINVENTORY ");
            sql.Append("        WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("  UNION SELECT COUNT(*) AS CNT FROM TRNSALEITEM ");
            sql.Append("        WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("  UNION SELECT COUNT(*) AS CNT FROM TRNPURCHASEITEM ");
            sql.Append("        WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("  UNION SELECT COUNT(*) AS CNT FROM STCIMPORTITEM ");
            sql.Append("        WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("  UNION SELECT COUNT(*) AS CNT FROM TRNRECEIPTITEM ");
            sql.Append("        WHERE ITEM_CD = @ITEM_CD ");
            sql.Append("  ) AS CNT ");

            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@ITEM_CD";
            param.Value = strItemCode;
            command.Parameters.Add(param);

            IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
            while (reader.Read())
            {
                countVal = reader.GetInt32(0);
                return countVal;
            }

            return 0;
        }

        public int HasStockInfo(string strItemCode)
        {
            int countVal = 0;
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return countVal;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT SUM(QTY) AS CNT FROM STCINVENTORY ");
            sql.Append("  WHERE ITEM_CD = @ITEM_CD ");

            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@ITEM_CD";
            param.Value = strItemCode;
            command.Parameters.Add(param);

            IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
            while (reader.Read())
            {
                countVal = reader.GetInt32(0);
                return countVal;
            }

            return 0;
        }

        public string GetNextSerial(string strItemCode2)
        {
            int length = strItemCode2.Length;
            string offset = Convert.ToString(3 + length + 1);

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return "0000000";
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(CAST((SUBSTR(ITEM_CD, @Offset, 7)) as decimal)), '0000000') ");
            sql.Append("   FROM ITMITEM ");
            sql.Append("  WHERE SUBSTR(ITEM_CD, 3, @Length) = @strItemCode2 ");

            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Offset";
            param.Value = offset;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@Length";
            param.Value = length;
            command.Parameters.Add(param);

            //Int16 serial = (Int16)command.ExecuteScalar();
            Decimal serial = (Decimal)command.ExecuteScalar();
            return String.Format("{0:0000000}", serial + 1);
        }
    }
}

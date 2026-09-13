using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class CustomerModelIO : ModelIO
    {
        public bool CheckRealTIN(string strTIN)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT TAXPAYER_NM ");
            sql.Append("   FROM TAXPAYER ");
            sql.Append("  WHERE TIN = @TIN ");

            string taxpayerName;

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = strTIN;
                command.Parameters.Add(param);

                taxpayerName = (string)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return false;
            }

            return string.IsNullOrEmpty(taxpayerName);
        }

        public string GetNextIndividualCode(string strBranchCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return null;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(SUBSTR(BCNC_ID,4,6)), 0) ");
            sql.Append("   FROM USRBCNC ");
            sql.Append("  WHERE BHF_ID = @BHF_ID ");
            sql.Append("    AND SUBSTR(BCNC_ID,1,3) = 'IND' ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = strBranchCode;
                command.Parameters.Add(param);

                //Int16 serial = (Int16)command.ExecuteScalar();
                Decimal serial = (Decimal)command.ExecuteScalar();
                string serialText = "IND" + String.Format("{0:000000}", serial + 1);
                
                return serialText;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int HasRecordHistory(string strTIN)
        {
            int countVal = 0;
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return countVal;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT SUM(CNT) AS CNT FROM ( ");
            sql.Append("        SELECT COUNT(*) AS CNT FROM STCWHIO ");
            sql.Append("        WHERE BCNC_ID = @BCNC_ID ");
            sql.Append("  UNION SELECT COUNT(*) AS CNT FROM TRNSALE ");
            sql.Append("        WHERE BCNC_ID = @BCNC_ID ");
            sql.Append("  UNION SELECT COUNT(*) AS CNT FROM TRNPURCHASE ");
            sql.Append("        WHERE BCNC_ID = @BCNC_ID ");
            sql.Append("  ) AS CNT ");

            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@BCNC_ID";
            param.Value = strTIN;
            command.Parameters.Add(param);

            IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
            while (reader.Read())
            {
                countVal = reader.GetInt32(0);
                return countVal;
            }

            return 0;
        }

        public int SetUserDeleted(string rowID)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" DELETE FROM USRBCNC ");
            sql.Append("  WHERE BCNC_ID = @BCNC_ID");

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@BCNC_ID";
            param.Value = rowID;
            command.Parameters.Add(param);

            return ExecuteNonQuery(sql.ToString(), command);
        }
    }
}

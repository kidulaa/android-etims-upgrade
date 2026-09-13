using EBM2x.Modules;
using System;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class SalesModelIO : ModelIO
    {
        public string GetSalesSeq(string strCol, string strTable)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return null;
            }

            try
            {
                string strSql = " SELECT IFNULL(MAX(" + strCol + "), 0)  FROM " + strTable;

                command.CommandText = strSql;
                command.CommandType = CommandType.Text;

                //Int32 seq = (Int32)command.ExecuteScalar();
                Decimal seq = (Decimal)command.ExecuteScalar();
                return String.Format("{0:0}", seq + 1);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetReceiptDate()
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return "";
            }

            StringBuilder sql = new StringBuilder();
            if (Session.SQLTimeCol.Contains("DATETIME"))
            {
                sql.Append(" SELECT SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),9,2)||SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),6,2)||SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),1,4)|| SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),12,2)||SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),15,2)|| SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),18,2) ");
            }
            else
            {
                sql.Append(" SELECT CONCAT(SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),9,2), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),6,2), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),1,4), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),12,2),SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),15,2), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),18,2)) ");
            }

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@SQLTimeCol";
                param.Value = Session.SQLTimeCol;
                command.Parameters.Add(param);

                string seq = (string)command.ExecuteScalar();

                return seq;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public int GetMaxSaleItemSeq(string strInvID, string strBrcCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(ITEM_SEQ), 0) ");
            sql.Append("   FROM TRNSALEITEM ");
            sql.Append("  WHERE INV_ID = @INV_ID ");
            sql.Append("    AND BHF_ID = @BHF_ID ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@INV_ID";
                param.Value = strInvID;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = strBrcCode;
                command.Parameters.Add(param);

                //Int32 seq = (Int32)command.ExecuteScalar();
                Decimal seq = (Decimal)command.ExecuteScalar();
                return (int)seq;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int GetMaxStockItemSeq(string strStcID, string strBrcCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(ITEM_SEQ), 0) ");
            sql.Append("   FROM STCWHIOITEM ");
            sql.Append("  WHERE STC_ID = @STC_ID ");
            sql.Append("    AND BHF_ID = @BHF_ID ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@STC_ID";
                param.Value = strStcID;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = strBrcCode;
                command.Parameters.Add(param);

                //Int32 seq = (Int32)command.ExecuteScalar();
                Decimal seq = (Decimal)command.ExecuteScalar();
                return (int)seq;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int GetTotalNumberofSaleItem(string strInvID, string strBrcCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }
            
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(TOT_NUM_ITEM,0) ");
            sql.Append("   FROM TRNSALE ");
            sql.Append("  WHERE INV_ID = @INV_ID ");
            sql.Append("    AND BHF_ID = @BHF_ID ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@INV_ID";
                param.Value = strInvID;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = strBrcCode;
                command.Parameters.Add(param);

                //Int32 seq = (Int32)command.ExecuteScalar();
                Decimal seq = (Decimal)command.ExecuteScalar();
                return (int)seq;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}

using EBM2x.Modules;
using System;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class PurchaseModelIO : ModelIO
    {
        public string GetPurchaseSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return null;
            }

            try
            {
                string strSql = " SELECT IFNULL(MAX(INV_ID), 0) FROM TRNPURCHASE ";

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
 
        public int GetMaxPurchaseItemSeq(string strInvID, string strBCNCID)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(ITEM_SEQ), 0) ");
            sql.Append("   FROM TRNPURCHASEITEM ");
            sql.Append("  WHERE INV_ID = @INV_ID ");
            sql.Append("    AND BCNC_ID = @BCNC_ID ");

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
                param.ParameterName = "@BCNC_ID";
                param.Value = strBCNCID;
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

        public string GetStockSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return null;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("  SELECT IFNULL(MAX(STC_ID), 0) ");
            sql.Append("   FROM STCWHIO ");

            try
            {
                command.CommandText = sql.ToString();
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

        public int GetTotalNumberofPurchaseItem(string strInvID, string strBCNCID)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }
            
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(TOT_NUM_ITEM,0) ");
            sql.Append("   FROM TRNPURCHASE ");
            sql.Append("  WHERE INV_ID = @INV_ID ");
            sql.Append("    AND BCNC_ID = @BCNC_ID ");

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
                param.ParameterName = "@BCNC_ID";
                param.Value = strBCNCID;
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

using System;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class StockModelIO : ModelIO
    {

        public string GetStockSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return null;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(STC_ID), 0)  ");
            sql.Append("   FROM STCWHIO ");

            string seq;

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;
                seq = (string)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return null;
            }

            return String.Format("{0:0}", Convert.ToDecimal(seq) + 1);
        }
    }
}

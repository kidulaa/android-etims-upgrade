using System;
using System.Data;
using System.Text;

namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Models.ListView;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of SearchTaxpayerBaseListMaster.
    /// </summary>
    public class SearchTaxpayerBaseListMaster : ModelIO
    {		
        public List<SearchTinNode> GetTable(string likeValue)
        {
            List<SearchTinNode> arrayList = new List<SearchTinNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                         // Taxpayer
            sql.Append("       TAXPR_NM ");                     // Name
            sql.Append("  from TAXPAYER_BASE ");
            sql.Append(" where TIN like @likeValue or TAXPR_NM like @likeValue ");
            sql.Append(" order by TIN ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SearchTinNode record = new SearchTinNode();

                    record.Tin = reader.GetValue(0).ToString();
                    record.TaxprNm = reader.GetValue(1).ToString();

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

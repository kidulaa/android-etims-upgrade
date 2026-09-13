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
    /// Description of SearchItemClassListMaster.
    /// </summary>
    public class SearchItemClassListMaster : ModelIO
    {		
        public List<SearchClassNode> GetTable(string likeValue)
        {
            List<SearchClassNode> arrayList = new List<SearchClassNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select ITEM_CLS_CD, ");                      // Item Classification Code (RRA)
            sql.Append("       ITEM_CLS_NM, ");                      // Item Classification Name
            sql.Append("       TAX_TY_CD, ");                        // Taxation Type Code
            sql.Append("       MJR_TG_YN ");                         // Major Taget(Y/N)
            sql.Append("  from ITEM_CLASS ");
            sql.Append(" where (ITEM_CLS_CD like @likeValue or ITEM_CLS_NM like @likeValue ) and USE_YN = 'Y'");
            sql.Append("   and item_cls_lvl = '5'");
            sql.Append(" order by ITEM_CLS_CD ");

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
                    SearchClassNode record = new SearchClassNode();

                    record.ItemClsCd = reader.GetValue(0).ToString();
                    record.ItemClsNm = reader.GetValue(1).ToString();
                    record.TaxTyCd = reader.GetValue(2).ToString();
                    record.MjrTgYn = reader.GetValue(3).ToString();

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

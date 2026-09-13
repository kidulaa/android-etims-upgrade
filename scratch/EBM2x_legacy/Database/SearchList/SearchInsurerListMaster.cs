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
    /// Description of SearchInsurerListMaster.
    /// </summary>
    public class SearchInsurerListMaster : ModelIO
    {		
        public List<SearchInsurerNode> GetTable()
        {
            List<SearchInsurerNode> arrayList = new List<SearchInsurerNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select ISSRCC_CD, ");                    // Insurer Code
            sql.Append("       ISRCC_NM, ");                     // Insurer Name
            sql.Append("       ISRC_RT ");                       // Insurer Rate
            sql.Append("  from TAXPAYER_BHF_INSURANCE ");
            sql.Append(" order by ISSRCC_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SearchInsurerNode record = new SearchInsurerNode();

                    record.InsurerCode = reader.GetString(0);
                    record.InsurerName = reader.GetString(1);
                    record.InsurerRate = reader.GetInt16(2);

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

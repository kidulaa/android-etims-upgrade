using System;
using System.Data;
using System.Text;

namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Models.ListView;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of SearchRefundReasonListMaster.
    /// </summary>
    public class SearchRefundReasonListMaster : ModelIO
    {		
        public List<SearchRefundReasonNode> GetTable()
        {
            List<SearchRefundReasonNode> arrayList = new List<SearchRefundReasonNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            CodeDtlTable codeDtlTable = new CodeDtlTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(codeDtlTable.GetSelectSQL());
            sql.Append(" where CD_CLS = '32'");
            sql.Append("   and USE_YN = 'Y'");

/*JCNA20191129
            for (int i = 0; i < 7; i++)
            {
                SearchRefundReasonNode node = new SearchRefundReasonNode
                {
                    ReasonCode = string.Format("{0:00000}", i),
                    ReasonText = "Refund Reason " + i
                };
                arrayList.Add(node);
            }
            if (arrayList.Count > 0) return arrayList;
*/
            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SearchRefundReasonNode record = new SearchRefundReasonNode();

                    if (!reader.IsDBNull(0)) record.ReasonCode = reader.GetString(0);                 // Code
                    if (!reader.IsDBNull(2)) record.ReasonText = reader.GetString(2);                 // Name of Code

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

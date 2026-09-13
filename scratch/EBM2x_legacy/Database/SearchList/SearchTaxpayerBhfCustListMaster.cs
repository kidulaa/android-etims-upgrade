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
    /// Description of SearchTaxpayerBhfCustListMaster.
    /// </summary>
    public class SearchTaxpayerBhfCustListMaster : ModelIO
    {		
        public List<SearchCustomerNode> GetTable(string tin, string bhfId, string likeValue)
        {
            List<SearchCustomerNode> arrayList = new List<SearchCustomerNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN                , ");                         // Taxpayer Identification Number(TIN)
            sql.Append("       CUST_NO            , ");                         // Customer No.
            sql.Append("       CUST_TIN           , ");                         // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID        , ");                         // Customer Branch ID
            sql.Append("       CUST_NID           , ");                         // Customer National Idetification
            sql.Append("       CUST_NM            , ");                         // Customer Name
            sql.Append("       TEL_NO             , ");                         // Telephone Number
            sql.Append("       ADRS               , ");                         // Address
            sql.Append("       USE_YN             , ");                         // Use(Y/N)
            sql.Append("       CUST_GROUP           ");                         // CUST_GROUP

            sql.Append("  from TAXPAYER_BHF_CUST          ");
            if (string.IsNullOrEmpty(likeValue))
            {
                sql.Append(" where USE_YN = 'Y' ");
                sql.Append(" order by mod_dt desc ");
                sql.Append(" LIMIT 20 ");
            }
            else
            {
                sql.Append(" where USE_YN = 'Y' ");
                sql.Append("   and ( CUST_NO like @likeValue ");
                sql.Append(" or    CUST_TIN like @likeValue ");
                sql.Append(" or    CUST_NM like @likeValue ) ");
                sql.Append(" order by CUST_NO ");
            }

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;
 
                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = bhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SearchCustomerNode record = new SearchCustomerNode();

                    record.Tin = reader.GetValue(3).ToString();
                    record.CustomerCode = reader.GetValue(2).ToString();
                    record.CustomerName = reader.GetValue(5).ToString();
                    record.CustGroup = reader.GetValue(9).ToString();

                    if (string.IsNullOrEmpty(record.Tin)) record.Tin = record.CustomerCode;

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
        public List<SearchCustomerNode> GetTinTable(string tin, string likeValue)
        {
            List<SearchCustomerNode> arrayList = new List<SearchCustomerNode>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN                , ");                         // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM             ");
            sql.Append("  from TAXPAYER_BASE          ");
            if (string.IsNullOrEmpty(likeValue))
            {
                sql.Append(" order by mod_dt desc ");
                sql.Append(" LIMIT 20 ");
            }
            else
            {
                sql.Append(" where  ( TIN like @likeValue ");
                sql.Append(" or    TAXPR_NM like @likeValue ) ");
                sql.Append(" order by TAXPR_NM ");
            }
            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SearchCustomerNode record = new SearchCustomerNode();

                    record.Tin = reader.GetValue(0).ToString();
                    record.CustomerCode = reader.GetValue(0).ToString();
                    record.CustomerName = reader.GetValue(1).ToString();

                    if (string.IsNullOrEmpty(record.Tin)) record.Tin = record.CustomerCode;

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

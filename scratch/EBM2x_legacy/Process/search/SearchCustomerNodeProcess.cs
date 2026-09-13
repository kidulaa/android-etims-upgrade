using EBM2x.Database.Master;
using EBM2x.Models.ListView;
using System;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchCustomerNodeProcess
    {
        public static List<SearchCustomerNode> QuerydSearchCustomer(string tin, string bhfId, string searchText)
        {
            SearchTaxpayerBhfCustListMaster searchTaxpayerBhfCustListMaster = new SearchTaxpayerBhfCustListMaster();
            List<SearchCustomerNode> list = searchTaxpayerBhfCustListMaster.GetTable(tin, bhfId, searchText);
            return list;
        }
        public static List<SearchCustomerNode> QuerydSearchTinCustomer(string tin, string bhfId, string searchText)
        {
            SearchTaxpayerBhfCustListMaster searchTaxpayerBhfCustListMaster = new SearchTaxpayerBhfCustListMaster();
            List<SearchCustomerNode> list = searchTaxpayerBhfCustListMaster.GetTinTable(tin, searchText);
            return list;
        }
    }
}

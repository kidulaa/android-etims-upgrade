using EBM2x.Database.Master;
using EBM2x.Models.ListView;
using System;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchTinNodeProcess
    {
        public static List<SearchTinNode> QuerydSearchTin(string searchText)
        {
            SearchTaxpayerBaseListMaster searchTaxpayerBaseListMaster = new SearchTaxpayerBaseListMaster();
            List<SearchTinNode> list = searchTaxpayerBaseListMaster.GetTable(searchText);
            return list;
        }
    }
}

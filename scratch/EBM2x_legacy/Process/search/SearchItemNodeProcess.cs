using EBM2x.Database.Master;
using EBM2x.Models.ListView;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchItemProcess
    {
        public static List<SearchItemNode> QuerydSearchItemBarcode(string tin, string searchText)
        {
            //SearchItemListMaster searchItemListMaster = new SearchItemListMaster();
            //List<SearchItemNode> list = searchItemListMaster.GetTableBarcode(searchText);

            if (string.IsNullOrEmpty(searchText)) return null;

            SearchTaxpayerItemListMaster searchItemListMaster = new SearchTaxpayerItemListMaster();
            List<SearchItemNode> list = searchItemListMaster.GetTableBarcode(tin, searchText);

            return list;
        }
        public static List<SearchItemNode> QuerydSearchItem(string tin, string searchText)
        {
            //SearchItemListMaster searchItemListMaster = new SearchItemListMaster();
            //List<SearchItemNode> list = searchItemListMaster.GetTable(searchText);

            SearchTaxpayerItemListMaster searchItemListMaster = new SearchTaxpayerItemListMaster();
            List<SearchItemNode> list = searchItemListMaster.GetTable(tin, searchText);

            return list;
        }
    }
}

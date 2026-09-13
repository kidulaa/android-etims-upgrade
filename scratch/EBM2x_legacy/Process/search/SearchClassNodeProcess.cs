using EBM2x.Database.Master;
using EBM2x.Models.ListView;
using System;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchClassNodeProcess
    {
        public static List<SearchClassNode> QuerydSearchClass(string searchText)
        {
            SearchItemClassListMaster searchItemClassListMaster = new SearchItemClassListMaster();
            List<SearchClassNode> list = searchItemClassListMaster.GetTable(searchText);
            return list;
        }
    }
}

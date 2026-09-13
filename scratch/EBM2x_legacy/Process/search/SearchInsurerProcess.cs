using EBM2x.Database.Master;
using EBM2x.Models.ListView;
using System;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchInsurerProcess
    {
        public static List<SearchInsurerNode> QuerydSearchInsurer()
        {
            SearchInsurerListMaster searchInsurerListMaster = new SearchInsurerListMaster();
            List<SearchInsurerNode> list = searchInsurerListMaster.GetTable();

            return list;
        }
    }
}

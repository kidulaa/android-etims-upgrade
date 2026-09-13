using EBM2x.Database.Master;
using EBM2x.Models.ListView;
using System;
using System.Collections.Generic;

namespace EBM2x.Process.search
{
    public class SearchRefundReasonProcess
    {
        public static List<SearchRefundReasonNode> QuerydSearchRefundReason()
        {
            SearchRefundReasonListMaster searchRefundReasonListMaster = new SearchRefundReasonListMaster();
            List<SearchRefundReasonNode> list = searchRefundReasonListMaster.GetTable();

            return list;
        }
    }
}

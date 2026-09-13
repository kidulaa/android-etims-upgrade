using EBM2x.Datafile.trlog;
using System.Collections.Generic;

namespace EBM2x.RraSdc
{
    public class RequestResponList
    {
        public List<RequestResponNode> NodeList { get; set; } 

        public RequestResponList()
        {
            NodeList = new List<RequestResponNode>();
        }

        public void InitRequestRespon()
        {
            NodeList.Clear();
            
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_CODE_SEARCH, LastDate = "20150101000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_ITEM_CLASS_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_CUSTOMER_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_BHF_CUST_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_BHF_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_ITEM_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_IMPORT_ITEM_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_TRNS_SALES_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_TRNS_PURCHASE_SALES_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_TRNS_PURCHASE_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_STOCK_MASTER_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_STOCK_IO_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_STOCK_MOVE_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });
            NodeList.Add(new RequestResponNode { RraSdcType = "RECEIVE", ProcessName = RraSdcService.URL_NOTICE_SEARCH, LastDate = "20200218000000", ProcessCount = 0 });

            RraSdcHistoryWriter.WriteTransaction(this);
        }

        public RequestResponNode GetRequestResponNode(string ProcessName)
        {
            if(NodeList.Count == 0)
            {
                InitRequestRespon();
            }

            for(int i = 0;i < NodeList.Count; i++)
            {
                if(NodeList[i].ProcessName.Equals(ProcessName))
                {
                    return NodeList[i];
                }
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.RraSdc.model
{
    class NonReportingReceiptsRes
    {
        public string resultCd { get; set; }  // Result Code
        public string resultMsg { get; set; }  // Result Message
        public string resultDt { get; set; }  // Result Date
        public NonReportingReceiptsResData data { get; set; }  // Result Date

        public class NonReportingReceiptsResData
        {
            public List<NonReportingInvoice> trnsSaleNonReportingList { get; set; }  // Item List
        }
        public class NonReportingInvoice
        {
            public string tin { get; set; }  // TIN
            public string bhfId { get; set; }  // Branch ID
            public string curRcptNo { get; set; }  //  Current Receipts Number
        }
    }
}

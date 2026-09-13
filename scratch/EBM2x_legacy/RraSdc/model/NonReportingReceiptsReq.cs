using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.RraSdc.model
{
    class NonReportingReceiptsReq
    {
        public string tin { get; set; }  // TIN
        public string bhfId { get; set; }  // Branch ID
        public long curRcptNo { get; set; }// Last Current Receipt Number
        public string lastReqDt { get; set; }  // Last Request Dt
    }
}

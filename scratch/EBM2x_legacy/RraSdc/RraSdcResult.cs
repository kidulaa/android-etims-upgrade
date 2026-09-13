using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.RraSdc
{
    public class RraSdcResult
    {
        public string resultCd { get; set; }  // Result Code
        public string resultMsg { get; set; }  // Result Message
        public string resultDt { get; set; }  // Result Date

        public RraSdcResult()
        {
            resultCd = "000";
            resultMsg = "";
            resultDt = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.RraSdc
{
    public class RraSdcUploadModel
    {
        public string FileType { get; set; }
        public string FileName { get; set; }

        // JsonRequest  UPDATE
        public string FunctionName { get; set; }
        public string JsonRequest { get; set; }
        public string RequestDt { get; set; }  // Request Date

        //UPDATE
        public string ResultCd { get; set; }  // Result Code
        public string ResultMsg { get; set; }  // Result Message
        public string ResultDt { get; set; }  // Result Date
    }

    public class RraSdcDownloadReq
    {
        public string tin { get; set; }  // TIN
        public string bhfId { get; set; }  // Branch ID
        public string lastReqDt { get; set; }  // Last Request Dt
    }
    public class RraSdcUploadRes
    {
        public string resultCd { get; set; }  // Result Code
        public string resultMsg { get; set; }  // Result Message
        public string resultDt { get; set; }  // Result Date
    }
}

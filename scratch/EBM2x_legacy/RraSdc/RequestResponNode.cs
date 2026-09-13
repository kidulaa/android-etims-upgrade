using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.RraSdc
{
    public class RequestResponNode
    {
        public string RraSdcType { get; set; }   // SEND / RECEIVE
        public string ProcessName { get; set; }  // SDC
        public string LastDate { get; set; }     // yyyyMMddHHmmss
        public int ProcessCount { get; set; }    // 
        public string UpdateDate { get; set; }     // yyyyMMddHHmmss

        public RequestResponNode()
        {
            RraSdcType = string.Empty;   //  SEND / RECEIVE
            ProcessName = string.Empty;  // SDC 
            LastDate = string.Empty;     // yyyyMMddHHmmss
            ProcessCount = 0;            // 
            UpdateDate = string.Empty;     // yyyyMMddHHmmss
        }
    }
}

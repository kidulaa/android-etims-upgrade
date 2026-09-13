using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.open
{
    public class OpenTimeNode
    {

        public string InitStartTime { get; set; }
        public string InitEndTime { get; set; }
        public string MasterStartTime { get; set; }
        public string MasterEndTime { get; set; }
        public string UpdateCheckFlag { get; set; }
        public string CloseCheckFlag { get; set; }

        public void clear()
        {
            InitStartTime = string.Empty;
            InitEndTime = string.Empty;
            MasterStartTime = string.Empty;
            MasterEndTime = string.Empty;
            UpdateCheckFlag = string.Empty;
            CloseCheckFlag = string.Empty;
        }
    }
}

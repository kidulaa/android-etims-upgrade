using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.hold
{
    public class HoldNode
    {
        public string HoldFileName { get; set; }       
        public string UpdateTime { get; set; }        

        public string SalesDate { get; set; }
        public double Amount { get; set; }
        public TranModel TranModel { get; set; }       // TranModel

        public HoldNode()
        {
            HoldFileName = string.Empty;
            UpdateTime = string.Empty;

            TranModel = null;
        }
    }
}

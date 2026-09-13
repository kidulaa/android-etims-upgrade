using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class OperTotal
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public OperHeader OperHeader { get; set; }
        public RegiDetail RegiDetail { get; set; }

        public OperTotal()
        {
            OperHeader = new OperHeader();
            RegiDetail = new RegiDetail();
        }
    }
}

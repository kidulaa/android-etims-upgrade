using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class RegiTotal
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public RegiHeader RegiHeader { get; set; }     // 
        public RegiDetail RegiDetail { get; set; }     // 

        public RegiTotal()
        {
            RegiHeader = new RegiHeader();
            RegiDetail = new RegiDetail();
        }
    }
}

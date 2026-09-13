using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class StoreAuthNode
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string BusiRegiNo { get; set; }

        public StoreAuthNode()
        {
          StoreCode = string.Empty;
          StoreName = string.Empty;
          BusiRegiNo = string.Empty;
        }
    }
}

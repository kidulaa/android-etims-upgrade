using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class LabelPtrNode
    {
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string GoodsPrice { get; set; }
        public string LineSpace { get; set; }

        public LabelPtrNode()
        {
            LineSpace = "0";
        }
    }
}

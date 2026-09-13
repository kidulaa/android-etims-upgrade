using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.inforlist
{
    public class InforNode
    {
        public string Type { get; set; }    // type
        public string Message { get; set; } // message

        public InforNode()
        {
            Type = string.Empty;
            Message = string.Empty;
        }

        public InforNode(string type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}

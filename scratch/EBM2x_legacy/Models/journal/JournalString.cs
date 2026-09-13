using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.journal
{
    public class JournalString
    {
        public string Type { get; set; }    
        public string Data { get; set; }   

        public JournalString()
        {
            Type = string.Empty;
            Data = string.Empty;
        }
    }
}

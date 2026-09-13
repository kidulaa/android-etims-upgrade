using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class SystemBoolCode
    {
        public bool Id { get; set; } 
        public string Name { get; set; }
        public SystemBoolCode()
        {
            Id = false; 
            Name = string.Empty; 
        }
    }
}

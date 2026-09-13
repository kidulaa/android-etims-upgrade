using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class SystemCode
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public SystemCode()
        {
            Id = string.Empty; 
            Name = string.Empty; 
        }
    }
}

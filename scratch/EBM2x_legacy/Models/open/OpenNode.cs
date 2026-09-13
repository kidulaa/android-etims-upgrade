using EBM2x.Models.inforlist;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.open
{
    public class OpenNode
    {
        public string OpenDate { get; set; }        
        public string OpenTime { get; set; } 
        public int OpenSequence { get; set; } 
        public string OpenState { get; set; } 
        public string InputYYYYMM { get; set; }   
        public string InputTT { get; set; } 
        public string InputMM { get; set; } 
        public string InputSS { get; set; } 
        public string VanFlag { get; set; }
        public string IpAddr { get; set; }
        public string SwVer { get; set; }
        public string PluFlag { get; set; }
        public string UseFlag { get; set; }

        public List<InforNode> InforNodeList { get; set; }

        public OpenNode()
        {
            OpenDate = string.Empty;          
            OpenTime = string.Empty;          
            OpenSequence = 0;                
            InputYYYYMM = string.Empty;       
            InputTT = string.Empty;           
            InputMM = string.Empty;         
            InputSS = string.Empty;           
            OpenState = string.Empty;   

            InforNodeList = new List<InforNode>();
        }
    }
}

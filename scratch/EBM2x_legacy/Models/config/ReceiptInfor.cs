using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class ReceiptInfor
    {
        public string PosImageYn { get; set; }  
        public string PosRcptMsg1 { get; set; } 
        public string PosRcptMsg2 { get; set; } 
        public string PosRcptMsg3 { get; set; }   
        public string PosRcptMsg4 { get; set; } 
        public string PosLocMsg1 { get; set; }  
        public string PosLocMsg2 { get; set; }  
        public string PosInfoMsg1 { get; set; } 
        public string PosInfoMsg2 { get; set; } 
        public string PosInfoMsg3 { get; set; } 
        public string Attr1 { get; set; }       
        public string Attr2 { get; set; } 

        public ReceiptInfor()
        {
            PosImageYn = string.Empty;  
            PosRcptMsg1 = string.Empty; 
            PosRcptMsg2 = string.Empty; 
            PosRcptMsg3 = string.Empty;   
            PosRcptMsg4 = string.Empty; 
            PosLocMsg1 = string.Empty;  
            PosLocMsg2 = string.Empty;  
            PosInfoMsg1 = string.Empty; 
            PosInfoMsg2 = string.Empty; 
            PosInfoMsg3 = string.Empty; 
            Attr1 = string.Empty;       
            Attr2 = string.Empty;       
        }
    }
}

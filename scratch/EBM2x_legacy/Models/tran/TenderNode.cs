using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.tran
{
    public class TenderNode
    {
        public string TranFlag { get; set; }          
        public string TenderFlag { get; set; }        
        public string TenderName { get; set; }        
        public double ReceiveAmount { get; set; }     
        public double SubtotalAmount { get; set; }    
        public int Sign { get; set; }              

        public double TaxAmount { get; set; }            
        public double VatAmount { get; set; }            
        public double TaxfreeAmount { get; set; }        

        public string CardNumber { get; set; }          
        public string PhoneNumber { get; set; }         

        public TenderNode()
        {
            clear();
        }

        public void clear()
        {
            TranFlag = string.Empty;          
            TenderFlag = string.Empty;        
            TenderName = string.Empty;        
            ReceiveAmount = 0;                
            SubtotalAmount = 0;               
            Sign = 0;                         

            TaxAmount = 0;                    
            TaxfreeAmount = 0;                
            VatAmount = 0;                    

            CardNumber = string.Empty;
            PhoneNumber = string.Empty;
        }
    }
}

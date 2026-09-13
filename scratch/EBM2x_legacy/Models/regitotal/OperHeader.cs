using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class OperHeader
    {
        public string StoreCode { get; set; }      
        public string RegiNo { get; set; }         
        public bool OpenFlag { get; set; }         
        public string OpenDate { get; set; }       
        public string OpenTime { get; set; }       
        public int OpenSequence { get; set; }   

        public bool CloseFlag { get; set; }       
        public string CloseDate { get; set; }      
        public string CloseTime { get; set; }     
        public int CloseSequence { get; set; }   

        //20200226 JCNA : public int ReceiptNo { get; set; }        
        public string UserID { get; set; }        
        public string UserName { get; set; }       
        public string UpdateDate { get; set; }     
        public string UpdateTime { get; set; }        

        public long PreOperCashStock { get; set; }

        public OperHeader()
        {
        }
        public void Clear()
        {
            StoreCode = string.Empty;     
            RegiNo = string.Empty;        
            OpenFlag = false;             
            OpenDate = string.Empty;      
            OpenTime = string.Empty;      
            OpenSequence = 1;             

            CloseFlag = true;             
            CloseDate = string.Empty;     
            CloseTime = string.Empty;     
            CloseSequence = 0;            

            //20200226 JCNA : ReceiptNo = 1;              
            UserID = string.Empty;        
            UserName = string.Empty;      
            UpdateDate = string.Empty;    
            UpdateTime = string.Empty;       

            PreOperCashStock = 0;         
        }    
    }
}

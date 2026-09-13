using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class EnvPosNode
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public string StoreCode { get; set; }        
        public string StoreName { get; set; }         
        public string PosNumber { get; set; }        
        public string PosName { get; set; }           
        public string PosInstallDate { get; set; }    

        public string ServerIP { get; set; }         
        public string CommURL { get; set; }          
        public string FtpUser { get; set; }           // FtpUser
        public string FtpPassword { get; set; }       // FtpPassword
        public int FtpTimeout { get; set; }           // FtpTimeout
        public string FtpPath { get; set; }           // FtpPath

        public bool NameCardUse { get; set; }         
        public bool CustZoneUse { get; set; }         
        public int MaxItemCount { get; set; }         
        public int MaxTrlogSend { get; set; }       
        public int MaxTrlogCount { get; set; }        
        public int CashReceiptAmount { get; set; }    

        public bool ItemOverlapUse { get; set; }	  
        public string PosFlag { get; set; }			  
        public string PosType { get; set; }         

        public string OwnIP { get; set; }		   	  

        public EnvPosNode()
        {
            StoreCode = string.Empty;           
            StoreName = string.Empty;          
            PosNumber = string.Empty;          
            PosName = string.Empty;             
            PosInstallDate = string.Empty;      

            ServerIP = string.Empty;            // ServerIP
            CommURL = string.Empty;             
            FtpUser = string.Empty;             // FtpUser
            FtpPassword = string.Empty;         // FtpPassword
            FtpTimeout = 0;                     // FtpTimeout
            FtpPath = string.Empty;             // FtpPath

            NameCardUse = false;                
            CustZoneUse = false;                
            MaxItemCount = 0;                   
            MaxTrlogSend = 0;                   
            MaxTrlogCount = 0;                  
            CashReceiptAmount = 0;              

            ItemOverlapUse = true;              
            PosFlag = string.Empty;             
            PosType = string.Empty;            

            OwnIP = string.Empty;		   	
        }
    }
}

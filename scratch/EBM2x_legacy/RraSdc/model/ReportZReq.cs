using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.RraSdc.model
{
    public class ReportZReq
    {
        public string tin { get; set; }  // TIN
        public string bhfId { get; set; }  // Branch ID

        public string rptDe { get; set; } 
        public string sdcId { get; set; } 
        public string rptNo { get; set; } 
        public string rcptPbctCnt { get; set; } 
        public long rcptOpnNo { get; set; } 
        public long rcptClsNo { get; set; } 
        public long nrmRcptPbctCnt { get; set; }
        public long nrmRcptOpnNo { get; set; } 
        public long nrmRcptClsNo { get; set; } 
        public double nrmSalesAmt { get; set; } 
        public double nrmRfdAmt { get; set; } 
        public double nrmSalesTaxAmt { get; set; } 
        public double nrmRfdTaxAmt { get; set; } 
        public long cpyRcptPbctCnt { get; set; } 
        public long cpyRcptOpnNo { get; set; } 
        public long cpyRcptClsNo { get; set; } 
        public double cpySalesAmt { get; set; } 
        public double cpyRfdAmt { get; set; } 
        public double cpySalesTaxAmt { get; set; } 
        public double cpyRfdTaxAmt { get; set; } 
        public long trnRcptPbctCnt { get; set; } 
        public long trnRcptOpnNo { get; set; } 
        public long trnRcptClsNo { get; set; } 
        public double trnSalesAmt { get; set; } 
        public double trnRfdAmt { get; set; } 
        public double trnSalesTaxAmt { get; set; } 
        public double trnRfdTaxAmt { get; set; } 
        public long pfmRcptPbctCnt { get; set; } 
        public long pfmRcptOpnNo { get; set; } 
        public long pfmRcptClsNo { get; set; } 
        public double pfmSalesAmt { get; set; } 
        public double pfmRfdAmt { get; set; } 
        public double pfmSalesTaxAmt { get; set; } 
        public double pfmRfdTaxAmt { get; set; } 
        public string regrId { get; set; } 
        public string regrNm { get; set; } 


        public ReportZReq()
        {
            rptDe = "";         
            sdcId = "";          //	SDC 
            rptNo = "";         
            rcptPbctCnt = "";  
            rcptOpnNo = 0;      
            rcptClsNo = 0;      
            nrmRcptPbctCnt = 0; 
            nrmRcptOpnNo = 0;   
            nrmRcptClsNo = 0;  
            nrmSalesAmt = 0;    
            nrmRfdAmt = 0;      
            nrmSalesTaxAmt = 0; 
            nrmRfdTaxAmt = 0;   
           
            cpyRcptPbctCnt = 0; 
            cpyRcptOpnNo = 0;  
            cpyRcptClsNo = 0; 
            cpySalesAmt = 0;    
            cpyRfdAmt = 0;      
            cpySalesTaxAmt = 0; 
            cpyRfdTaxAmt = 0;   
            trnRcptPbctCnt = 0; 
            trnRcptOpnNo = 0;  
            trnRcptClsNo = 0;  
            trnSalesAmt = 0;  
            trnRfdAmt = 0;      
            trnSalesTaxAmt = 0; 
            trnRfdTaxAmt = 0;   
            pfmRcptPbctCnt = 0; 
            pfmRcptOpnNo = 0; 
            pfmRcptClsNo = 0;  
            pfmSalesAmt = 0;    
            pfmRfdAmt = 0;     
            pfmSalesTaxAmt = 0; 
            pfmRfdTaxAmt = 0;  
            
            regrId = "";        
            regrNm = "";        
        }

    }
}

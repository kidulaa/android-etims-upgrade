using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerBhfCustRecord.
    /// </summary>
    public class TaxpayerBhfCustRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string CustNo { get; set; }              // Customer No.
        public string CustTin { get; set; }             // Customer Taxpayer Identification Number(TIN)
        public string CustBhfId { get; set; }           // Customer Branch ID
        public string CustNid { get; set; }             // Customer National Idetification
        public string CustNm { get; set; }              // Customer Name
        public string TelNo { get; set; }               // Telephone Number
        public string Adrs { get; set; }                // Address
        public string UseYn { get; set; }               // Use(Y/N)
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
        public string NationCd { get; set; }            
        public string ChargerNm { get; set; }          
        public string Contact1 { get; set; }           
        public string Contact2 { get; set; }           
        public string Email { get; set; }               
        public string Fax { get; set; }                 
        public string Rm { get; set; }                  
        public double InitlUnclamt { get; set; }        
        public double InitlNpyamt { get; set; }         
        public double Unclamt { get; set; }             
        public double Npyamt { get; set; }             
        public string BcncType { get; set; }          
        public string Bank { get; set; }                
        public string Account { get; set; }            
        public string Depositor { get; set; }          
        public string CustGroup { get; set; } 

        public string BcncTypeName { get; set; }           
        public string NationName { get; set; }           

        public TaxpayerBhfCustRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.CustNo = string.Empty;                 // Customer No.
            this.CustTin = string.Empty;                // Customer Taxpayer Identification Number(TIN)
            this.CustBhfId = string.Empty;              // Customer Branch ID
            this.CustNid = string.Empty;                // Customer National Idetification
            this.CustNm = string.Empty;                 // Customer Name
            this.TelNo = string.Empty;                  // Telephone Number
            this.Adrs = string.Empty;                   // Address
            this.UseYn = "Y";                           // Use(Y/N)
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date

            this.NationCd = string.Empty;               
            this.ChargerNm = string.Empty;              
            this.Contact1 = string.Empty;               
            this.Contact2 = string.Empty;               
            this.Email = string.Empty;                  
            this.Fax = string.Empty;                    
            this.Rm = string.Empty;                     
            this.InitlUnclamt = 0;                      
            this.InitlNpyamt = 0;                       
            this.Unclamt = 0;                           
            this.Npyamt = 0;                            
            this.BcncType = "01";                       
            this.Bank = string.Empty;                  
            this.Account = string.Empty;                
            this.Depositor = string.Empty;              

            this.CustGroup = "DF";
        }
    }
}

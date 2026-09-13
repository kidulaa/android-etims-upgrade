using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class RegiDetail
    {
        public UnitTotal TotalAmount { get; set; }       
        public UnitTotal DiscountPartAmount { get; set; } 
        public UnitTotal DiscountPartRate { get; set; }  
        public UnitTotal DiscountAmount { get; set; }     
        public UnitTotal PureAmount { get; set; }       
        //----------------------------------------------------------------------
        public UnitTotal VoidAmount { get; set; }         // VOID
        //----------------------------------------------------------------------
        public UnitTotal CashSale { get; set; }           
        public UnitTotal CashReturn { get; set; }         
        //----------------------------------------------------------------------
        public UnitTotal Credit { get; set; }             
        public UnitTotal Debit { get; set; }              
        public UnitTotal Gift { get; set; }              
        public UnitTotal MobileWallet { get; set; }       // Mobile Wallet
        public UnitTotal Cutting { get; set; }            
        //----------------------------------------------------------------------
        public UnitTotal CreditHomeIlsibul { get; set; }  
        public UnitTotal CreditHomeHalbu { get; set; }    
        //----------------------------------------------------------------------
        public UnitTotal ReservedFund { get; set; }       
        public UnitTotal MidStockCash1 { get; set; }      
        public UnitTotal MidStockCash2 { get; set; }      
        public UnitTotal MidStockCash3 { get; set; }      
        public UnitTotal MidStockCash4 { get; set; }      
        public UnitTotal MidStockGift { get; set; }       
        public UnitTotal CashStock { get; set; }          
        //----------------------------------------------------------------------
        public UnitTotal NormalSale { get; set; }         
        public UnitTotal ReturnSale { get; set; }         
        public UnitTotal Vat { get; set; }                
        public UnitTotal CustomerNumber { get; set; }     

        public ClassTotalList ClassTotalList { get; set; }

        public RegiDetail()
        {
            TotalAmount = new UnitTotal("TotalAmount", "total amount");         
            DiscountAmount = new UnitTotal("DiscountAmount", "DiscountAmount");   
            PureAmount = new UnitTotal("PureAmount", "net sales");             
            //----------------------------------------------------------------------
            VoidAmount = new UnitTotal("VoidAmount", "VOID");                 // VOID
            //----------------------------------------------------------------------
            CashSale = new UnitTotal("CashSale", "CashSale");                 
            CashReturn = new UnitTotal("CashReturn", "CashReturn");         
            //----------------------------------------------------------------------
            Credit = new UnitTotal("Credit", "Credit");                     
            Debit = new UnitTotal("Debit", "Debit");                       
            Gift = new UnitTotal("Gift", "Gift");                           
            MobileWallet = new UnitTotal("MobileWallet", "Mobile Wallet");    // MobileWallet
            Cutting = new UnitTotal("Cutting", "Cutting");                       
            //----------------------------------------------------------------------
            CreditHomeIlsibul = new UnitTotal("CreditHomeIlsibul", "CreditHomeIlsibul"); 
            CreditHomeHalbu = new UnitTotal("CreditHomeHalbu", "CreditHomeHalbu");       
            //----------------------------------------------------------------------
            ReservedFund = new UnitTotal("ReservedFund", "ReservedFund");           
            MidStockCash1 = new UnitTotal("MidStockCash1", "MidStockCash1");  
            MidStockCash2 = new UnitTotal("MidStockCash2", "MidStockCash2");  
            MidStockCash3 = new UnitTotal("MidStockCash3", "MidStockCash3"); 
            MidStockCash4 = new UnitTotal("MidStockCash4", "MidStockCash4"); 
            MidStockGift = new UnitTotal("MidStockGift", "MidStockGift");   
            CashStock = new UnitTotal("CashStock", "CashStock");          
            //----------------------------------------------------------------------
            NormalSale = new UnitTotal("NormalSale", "NormalSale");             
            ReturnSale = new UnitTotal("ReturnSale", "ReturnSale");             
            Vat = new UnitTotal("Vat", "부가세");                            
            CustomerNumber = new UnitTotal("CustomerNumber", "CustomerNumber");       
            //----------------------------------------------------------------------

            ClassTotalList = new ClassTotalList();
        }

        public void Clear()
        {
            TotalAmount.Clear();           
            DiscountAmount.Clear();        
            PureAmount.Clear();          
            //----------------------------------------------------------------------
            VoidAmount.Clear();            // VOID
            //----------------------------------------------------------------------
            CashSale.Clear();              
            CashReturn.Clear();           
            //----------------------------------------------------------------------
            Credit.Clear();                
            Debit.Clear();                 
            Gift.Clear();                  
            MobileWallet.Clear();          
            Cutting.Clear();              
            //----------------------------------------------------------------------
            CreditHomeIlsibul.Clear();     
            CreditHomeHalbu.Clear();       
            //----------------------------------------------------------------------
            ReservedFund.Clear();         
            MidStockCash1.Clear();         
            MidStockCash2.Clear();        
            MidStockCash3.Clear();         
            MidStockCash4.Clear();         
            MidStockGift.Clear();          
            CashStock.Clear();             
            //----------------------------------------------------------------------
            NormalSale.Clear();            
            ReturnSale.Clear();            
            Vat.Clear();                   
            CustomerNumber.Clear();        
            //----------------------------------------------------------------------

            ClassTotalList.Clear();
        }

        public double GetSubstituteTotalAmount()
        {
            double amount = 0;
            amount = Credit.Amount                      
                + Debit.Amount                         
                + Gift.Amount                          
                + MobileWallet.Amount;                  // MobileWallet

            return amount;
        }

        public int GetSubstituteTotalCount()
        {
            int count = 0;
            count = Credit.Count                       
                + Debit.Count                          
                + Gift.Count                           
                + MobileWallet.Count;                  // MobileWallet
            return count;
        }
        public double GetCashTotalAmount()
        {
            double amount = 0;
            amount = CashSale.Amount + CashReturn.Amount + ReservedFund.Amount;
            return amount;
        }

        public int GetCashTotalCount()
        {
            int count = 0;
            count = CashSale.Count + CashReturn.Count + ReservedFund.Count;
            return count;
        }
 
        public double GetOutputCashAmount()
        {
            double amount = 0;

            amount = MidStockCash1.Amount
                + MidStockCash2.Amount
                + MidStockCash3.Amount
                + MidStockCash4.Amount;

            return amount;
        }
        public int GetOutputCashCount()
        {
            int count = 0;
            count = MidStockCash1.Count
                + MidStockCash2.Count
                + MidStockCash3.Count
                + MidStockCash4.Count;
            return count;
        }
        public double GetCashBalanceAmount()
        {
            double amount = 0;
            amount = GetCashTotalAmount() - GetOutputCashAmount();
            return amount;
        }
        public double SumCashOverShortAmount()
        {
            double amount = 0;
            amount = CashStock.Amount - GetCashBalanceAmount();
            return amount;
        }
    }
}

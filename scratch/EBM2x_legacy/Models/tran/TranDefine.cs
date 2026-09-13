using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.tran
{
    public sealed class TranDefine
    {
        public const string LOG_OPEN = "OpenNode";                  
        public const string LOG_CLOSE = "CloseNode";                
        public const string LOG_TRAN = "TranNode";                  
        public const string LOG_SIGN_ON = "SignOnNode";             
        public const string LOG_SIGN_OFF = "SignOffNode";           
        public const string LOG_VOID = "VOID";                      
        public const string LOG_RESERVE = "ReserveNode";            
        public const string LOG_MIDSTOCK = "MidstockNode";          
        public const string LOG_CASHSTOCK = "CashstockNode";        
        public const string LOG_CARDPAY = "CardPayNode";           
        public const string LOG_INOUTPAY = "InoutPayNode";          
        public const string LOG_BUYING = "BUYING";                  
        public const string LOG_ORDER = "ORDER";                    
        public const string LOG_STOCK = "STOCK";			        
        public const string LOG_CHANGE_CLOSE = "ChangeCloseNode";       // 2015.01.22

        //2010.06.18 JCNA
        public const string LOG_TMONEY_FILL = "TMONEY_FILL";        // TMONEY

        public const string LOG_WORK_STATUS = "WORK_STATUS";        
                                                                    // 2010.04.11 KSM

        public const string LOG_POST_CASHBILL = "POST_CASHBILL";    
        public const string LOG_POST_POINTSAVE = "POST_POINTSAVE";  
        public const string LOG_POST_CASHBAGSAVE = "POST_CASHBAGSAVE"; 

        public const string LOG_TAXPAYMENT = "TaxPayMentNode";      

        public const string LOG_BOKGWON = "BokgwonNode";            
        public const string LOG_TRUST_REPAY = "TrustRepayNode";    

        public const string TRAN_NORMAL = "N";      
        public const string TRAN_RETURN = "R";      
        public const string TRAN_MISTAKE = "M";     
        public const string TRAN_BOTTLE = "B";      
        public const string TRAN_SHOPPING = "S";    
        public const string TRAN_EXAMP = "E";      

        public const string ITEM_NORMAL = "0";     
        public const string ITEM_MODIFY = "1";     
        public const string ITEM_CANCEL = "2";     
        public const string ITEM_BOTTLE = "3";      
        public const string ITEM_RTNBOTTLE = "4";   
        public const string ITEM_RTNSHOPPING = "5"; 



        public TranDefine()
        {
        }

        public static int sign(string vMode)
        {
            if (vMode.Equals(TRAN_NORMAL)) return 1;
            else if (vMode.Equals(TRAN_RETURN)) return -1;
            //			else if (vMode.Equals(TRAN_MISTAKE)) return -1;
            //			else if (vMode.Equals(TRAN_BOTTLE))  return -1;
            else return -1;
        }
        
        public static string getTranName(string vMode)
        {
            if (vMode.Equals(TRAN_NORMAL)) return "normal transaction";
            else if (vMode.Equals(TRAN_RETURN)) return "return transaction";
            else if (vMode.Equals(TRAN_MISTAKE)) return "return transaction";
            else if (vMode.Equals(TRAN_BOTTLE)) return "return transaction";
            else if (vMode.Equals(TRAN_EXAMP)) return "practice trading";
            else return string.Empty;
        }
    
        public static string getItemState(string vMode)
        {
            if (vMode.Equals(ITEM_MODIFY)) return "correction";
            else if (vMode.Equals(ITEM_CANCEL)) return "correction";
            else if (vMode.Equals(ITEM_BOTTLE)) return "correction";
            else if (vMode.Equals(ITEM_RTNBOTTLE)) return "correction";
            else if (vMode.Equals(ITEM_RTNSHOPPING)) return "correction";
            else return string.Empty;
        }
    }
}

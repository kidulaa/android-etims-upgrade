using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class RegiHeader
    {
        public string StoreCode { get; set; }      
        public string StoreName { get; set; }      
        public string RegiNo { get; set; }         
        public string RegiName { get; set; }       
        public bool OpenFlag { get; set; }         
        public string OpenDate { get; set; }       
        public string OpenTime { get; set; }       
        public string OpenDay365 { get; set; }     
        public int OpenSequence { get; set; }     

        public bool CloseFlag { get; set; }       
        public string CloseDate { get; set; }      
        public string CloseTime { get; set; }      
        public int CloseSequence { get; set; }    

        public string UserID { get; set; }         
        public string UserName { get; set; }       
        public string UpdateDate { get; set; }     
        public string UpdateTime { get; set; }        

        
        public int CreditHomeSlipNo { get; set; }        // CreditHomeSlipNo
        public int CreditOtherSlipNo { get; set; }       // CreditOtherSlipNo
        public int GiftSlipNo { get; set; }              //  GiftSlipNo
        public int DebitCardSlipNO { get; set; }         // DebitCardSlipNO
        public int CashCardSlipNo { get; set; }          // CashCardSlipNo
        public int NormalCreditSlipNo { get; set; }      // NormalCreditSlipNo
        public int PrepaidSlipNo { get; set; }           // SlipNo
        public int CardPaySlipNo { get; set; }           // SlipNo

        public int GiftExSlipNo { get; set; }            // SlipNo

        public int ItemQuerySeq { get; set; }            //  SEQ

        public int HoldCount { get; set; }              			   
        public int HoldNo { get; set; }                  		

        public int BuyHoldNo { get; set; }              		

        public int SignCount { get; set; }              

        public bool TRSend { get; set; }                
        public int ManualCreditHomeSlipNo { get; set; }  // CreditHomeSlipNo
        public int ManualCreditOtherSlipNo { get; set; } // CreditOtherSlipNo
        public int ManualDebitCardSlipNo { get; set; }   // DebitCardSlipNO
        public int ManualCashCardSlipNo { get; set; }    // CashCardSlipNo

        public int TRLossCount { get; set; }             
        public int SignLossCount { get; set; }           

        public bool InquiryNoticeFlag { get; set; }	     
        public string DownTime { get; set; }
        public string DownDate { get; set; }

        public RegiHeader()
        {
            StoreCode = string.Empty;      
            StoreName = string.Empty;      
            RegiNo = string.Empty;         
            RegiName = string.Empty;       

            Clear();
        }

        public void Clear()
        {
            OpenFlag = false;              
            OpenDate = string.Empty;       
            OpenTime = string.Empty;       
            OpenDay365 = string.Empty;     // 365
            OpenSequence = 1;             

            CloseFlag = true;              
            CloseDate = string.Empty;      
            CloseTime = string.Empty;      
            CloseSequence = 0;            

            UserID = string.Empty;         
            UserName = string.Empty;       
            UpdateDate = string.Empty;     
            UpdateTime = string.Empty;      

            //20200226 JCNA : ReceiptNo = 1;                 

            CreditHomeSlipNo = 1;      // CreditHomeSlipNo
            CreditOtherSlipNo = 1;     // CreditOtherSlipNo
            GiftSlipNo = 1;            // GiftSlipNo
            DebitCardSlipNO = 1;       // DebitCardSlipNO
            CashCardSlipNo = 1;        // CashCardSlipNo
            NormalCreditSlipNo = 1;    // NormalCreditSlipNo
            PrepaidSlipNo = 1;         // SlipNo
            CardPaySlipNo = 3001;      // SlipNo
            GiftExSlipNo = 5001;       // SlipNo

            ItemQuerySeq = 0;          // SEQ

            HoldCount = 0;             			   
            HoldNo = 0;                	

            BuyHoldNo = 0;             	

            SignCount = 0;             

            TRSend = false;            

            ManualCreditHomeSlipNo = 1;     // CreditHomeSlipNo
            ManualCreditOtherSlipNo = 1;    // CreditOtherSlipNo
            ManualDebitCardSlipNo = 1;      // DebitCardSlipNO
            ManualCashCardSlipNo = 1;       // CashCardSlipNo

            TRLossCount = 0;                // TR
            SignLossCount = 0;              // SIGN

            InquiryNoticeFlag = false;     
            DownTime = string.Empty;
            DownDate = string.Empty;
        }

        public void UpdateDateTime()
        {
            UpdateDate = System.DateTime.Now.ToString("yyyyMMdd");
            UpdateTime = System.DateTime.Now.ToString("HHmmss");
        }

        // (ADD) 
        public void addBuyHoldNo()
        {
            BuyHoldNo = BuyHoldNo + 1;
        }

        //(ADD) 
        public void addHoldNo()
        {
            HoldNo = HoldNo + 1;
        }

        public void addSignCount()
        {
            SignCount = SignCount + 1;
        }

       public void addCreditHomeSlipNo()
        {
            CreditHomeSlipNo = CreditHomeSlipNo + 1;
        }

        public void addCreditOtherSlipNo()
        {
            CreditOtherSlipNo = CreditOtherSlipNo + 1;
        }

        public void addGiftSlipNo()
        {
            GiftSlipNo = GiftSlipNo + 1;
        }

        public void addDebitCardSlipNO()
        {
            DebitCardSlipNO = DebitCardSlipNO + 1;
        }

        public void addCashCardSlipNo()
        {
            CashCardSlipNo = CashCardSlipNo + 1;
        }

        public void addNormalCreditSlipNo()
        {
            NormalCreditSlipNo = NormalCreditSlipNo + 1;
        }

        public void addPrepaidSlipNo()
        {
            PrepaidSlipNo = PrepaidSlipNo + 1;
        }

        public void addCardPaySlipNo()
        {
            CardPaySlipNo = CardPaySlipNo + 1;
        }

        public void addGiftExSlipNo()
        {
            GiftExSlipNo = GiftExSlipNo + 1;
        }

        

        
        public void setUpdateDateTime()
        {
            UpdateDate = System.DateTime.Now.ToString("yyyyMMdd");
            UpdateTime = System.DateTime.Now.ToString("HHmmss");
            OpenDay365 = "000"; //df000.format(calendar.get(java.util.Calendar.DAY_OF_YEAR));
        }
        public void setCloseDate()
        {
            CloseDate = System.DateTime.Now.ToString("yyyyMMdd");
        }
        public void setCloseTime()
        {
            CloseTime = System.DateTime.Now.ToString("HHmmss");
        }
        public void setNextOpenDate()
        {
            System.DateTime dOpen = System.DateTime.Now.AddDays(1);
            OpenDate = dOpen.ToString("yyyyMMdd");
        }
        public void setOpenDate()
        {
            OpenDate = System.DateTime.Now.ToString("yyyyMMdd");
        }
        public void setOpenTime()
        {
            OpenTime = System.DateTime.Now.ToString("HHmmss");
        }

        public string getBarcodeText(long receiptNo)
        {
            string barcode = string.Empty;

            if (OpenDate != null && !OpenDate.Equals(string.Empty))
            {
                barcode = "0" + OpenDate + RegiNo.ToString() + receiptNo.ToString("0000#");
            }
            else
            {
                barcode = "0" + "00000000" + "0000" + receiptNo.ToString("0000#");
            }
            return barcode;
        }

        
        public bool IsOpened()
        {
            return OpenFlag;
        }
       
        public bool IsClosed()
        {
            return CloseFlag;
        }
    }
}

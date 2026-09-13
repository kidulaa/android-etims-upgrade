namespace EBM2x.Models.config
{
    public class EnvFunctionNode
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public string UseGoodsInq { get; set; }                
        public string UseNotInGoods { get; set; }              
        public string UseGoodsFieldInput { get; set; }         
        //public string UseRescanning { get; set; } 		 
        //public string UsePoint { get; set; } 				  
        public string UseSettleNotice { get; set; }              
        public string UsePrintReceipt { get; set; }             
        //public string UseRePrintReceipt { get; set; } 	    
        public string UsePrintCashReceipt { get; set; }         
        public string UsePrintBarcode { get; set; }             
        public string UsePrintLogo { get; set; }               
        public string UsePrintCreditSlipForStore { get; set; }  
        public string UseSubViewEcho { get; set; }             
        public string UseAddItemNoCondition { get; set; }      


        // 2010.04.20 KSM NONP
        public string UseNonPLU { get; set; }                   // NON PLU 
        public string NonPLUPrefix { get; set; }                // NON PLU 
        public int NonPLUAmountPos { get; set; }                // NON PLU
        public int NonPLUAmountLength { get; set; }             // NON PLU 
        public int NonPLUAmountUnit { get; set; }               // NON PLU

        public string NonPLUAmountSetType { get; set; }         // NON PLU 
        public string NonPLUCDType { get; set; }                //  C/D 
                                                                // KSM NONPLU

        // NONPLU
        public string UseNonPLU_2 { get; set; }                 // NON PLU 
        public string NonPLUPrefix_2 { get; set; }              // NON PLU 
        public int NonPLUAmountPos_2 { get; set; }              // NON PLU 
        public int NonPLUAmountLength_2 { get; set; }           // NON PLU 
        public int NonPLUAmountUnit_2 { get; set; }             // NON PLU 
        public string NonPLUAmountSetType_2 { get; set; }       // NON PLU 
        public string NonPLUCDType_2 { get; set; }              // 바코드 C/D 

        
        public string UseNonPLU_3 { get; set; }                 
        public string NonPLUPrefix_3 { get; set; }              
        public int NonPLUCodePos_3 { get; set; }                
        public int NonPLUCodeLength_3 { get; set; }            
        public int NonPLUAmountPos_3 { get; set; }              
        public int NonPLUAmountLength_3 { get; set; }           
        public int NonPLUAmountUnit_3 { get; set; }             
        public string NonPLUAmountSetType_3 { get; set; }       
        public string NonPLUCDType_3 { get; set; }            

        //		public string NonpluType { get; set; } 			
        public string DcSection { get; set; }                    

        public string LabelPrinterSpace { get; set; }           
        public string LabelPrinterGoodsNameSize { get; set; }   
        public string LabelPrinterGoodsNameAlign { get; set; }  
        public string LabelPrinterPriceSize { get; set; }       
        public string LabelPrinterPriceAlign { get; set; }     
        public string ItemPrintLine { get; set; }              
        public string UseHoldPrinter { get; set; }              
        public string UseClosePrinter { get; set; }           

        public string OrderNoIncrease { get; set; }             
        public string OrderNoEachIncrease { get; set; }         
        public string OrderNo { get; set; }                     

        public string PrinterWithKitchen { get; set; }         

        
        public int InputIdx { get; set; }

        public string WonAmtNoSale { get; set; }               
        public string UseNoCvm { get; set; }                   
        public string UseAutoRecovery { get; set; }            

        public string TenderCuttingYn { get; set; }           

        public string DiscountCuttingYn { get; set; }         
        public string DiscountAll25 { get; set; }              
        public string NoPrintItemList { get; set; }           
        public string CancelTranNotPrintFlag { get; set; }      
        public string UseQuickClose { get; set; }              
        public string UseTotalRegiPrinter { get; set; }        
        public string UseTotalOperPrinter { get; set; }       
        public string UseTotalCardPrinter { get; set; }         
        public string UseTotalItemPrinter { get; set; }         
        public string UseTotalClassPrinter { get; set; }      
        public string UseCashStockPrinter { get; set; }      
        public string UseCardDrawerClose { get; set; }         
        public string UseNoticeShow { get; set; }              
        public string UseDrawerOpen { get; set; }              
        public string UseDual { get; set; }                 


        public EnvFunctionNode()
        {
            UseGoodsInq = string.Empty;
            UseNotInGoods = string.Empty;
            UseGoodsFieldInput = string.Empty;
            //setUseRescanning(string.Empty);
            //setUsePoint(string.Empty);
            UseSettleNotice = string.Empty;
            UsePrintReceipt = string.Empty;
            //setUseRePrintReceipt(string.Empty);
            UsePrintCashReceipt = string.Empty;
            UsePrintBarcode = string.Empty;
            UsePrintLogo = string.Empty;
            UsePrintCreditSlipForStore = string.Empty;
            UseSubViewEcho = string.Empty;

            UseAddItemNoCondition = string.Empty;
            // 2010.04.20 KSM NONPLU옵션화 -->
            UseNonPLU = string.Empty;
            NonPLUPrefix = string.Empty;
            NonPLUAmountPos = 0;
            NonPLUAmountLength = 0;
            NonPLUAmountUnit = 0;

            NonPLUAmountSetType = string.Empty;
            NonPLUCDType = string.Empty;
            // 2010.04.20 KSM NONPLU옵션화 <--

            // v2.2_NONPLU처리-2
            UseNonPLU_2 = string.Empty;
            NonPLUPrefix_2 = string.Empty;
            NonPLUAmountPos_2 = 0;
            NonPLUAmountLength_2 = 0;
            NonPLUAmountUnit_2 = 0;

            NonPLUAmountSetType_2 = string.Empty;
            NonPLUCDType_2 = string.Empty;

            // v4.1.0_NONPLU처리-3(중량저울바코드 처리)
            UseNonPLU_3 = string.Empty;
            NonPLUPrefix_3 = string.Empty;
            NonPLUCodePos_3 = 0;
            NonPLUCodeLength_3 = 0;
            NonPLUAmountPos_3 = 0;
            NonPLUAmountLength_3 = 0;
            NonPLUAmountUnit_3 = 0;
            NonPLUAmountSetType_3 = string.Empty;
            NonPLUCDType_3 = string.Empty;

            //			setNonpluType(string.Empty);
            DcSection = string.Empty;

            OrderNoIncrease = string.Empty;
            OrderNoEachIncrease = string.Empty;
            PrinterWithKitchen = string.Empty;
            WonAmtNoSale = string.Empty;
            //5만원이하 무서명 @pwb 20170214
            UseNoCvm = string.Empty;
            UseAutoRecovery = string.Empty;  //정산자료 오류 복구 알림기능
            InputIdx = 0;
        }
    }
}

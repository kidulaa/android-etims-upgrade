using EBM2x.Models;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.open
{
    public class StartFlow
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;

            posModel.TranInformation.SaleDate = regiHeader.OpenDate; 

            bool nextOpenFlag = false;
            if (regiHeader.OpenDate.Equals(System.DateTime.Now.ToString("yyyyMMdd")))
                nextOpenFlag = true;

            regiHeader.Clear();

            if (nextOpenFlag) regiHeader.setNextOpenDate(); 
            else regiHeader.setOpenDate();     

            regiHeader.OpenFlag = true;               
            regiHeader.setOpenTime();                 
            regiHeader.OpenSequence = 1;              
            regiHeader.CloseFlag = false;             
            regiHeader.CloseSequence = 0;             
            //regiHeader.SendReceiptNo = 0;           
            //20200226 JCNA : regiHeader.ReceiptNo = 1;            

            //regiHeader.SendReceiptNo = 0;           // TR 
            regiHeader.CreditHomeSlipNo = 1;          // CreditHomeSlipNo
            regiHeader.CreditOtherSlipNo = 5001;      // CreditOtherSlipNo
            regiHeader.GiftSlipNo = 1;                // GiftSlipNo
            regiHeader.DebitCardSlipNO = 1;           // DebitCardSlipNO
            regiHeader.CashCardSlipNo = 1;            // CashCardSlipNo
            regiHeader.NormalCreditSlipNo = 1;        // NormalCreditSlipNo
            regiHeader.PrepaidSlipNo = 1;             // NormalCreditSlipNo
            regiHeader.ItemQuerySeq = 0;              // SEQ
            regiHeader.CardPaySlipNo = 3001;          // SlipNo
            regiHeader.ManualCreditHomeSlipNo = 0;    // CreditHomeSlipNo
            regiHeader.ManualCreditOtherSlipNo = 0;   // CreditOtherSlipNo
            regiHeader.ManualDebitCardSlipNo = 0;     // DebitCardSlipNO
            regiHeader.ManualCashCardSlipNo = 0;      // CashCardSlipNo
            regiHeader.TRSend = false;  

            OperHeader operHeader = posModel.OperTotal.OperHeader;
            operHeader.Clear();

            operHeader.StoreCode = regiHeader.StoreCode;    
            operHeader.RegiNo = regiHeader.RegiNo;        
            operHeader.OpenDate = regiHeader.OpenDate;      
            operHeader.OpenFlag = false;                   
            operHeader.OpenSequence = 1;                   
            operHeader.CloseFlag = true;                  
            operHeader.CloseSequence = 0;                  
            //20200226 JCNA : operHeader.ReceiptNo = regiHeader.ReceiptNo;
            operHeader.PreOperCashStock = 0;               

            posModel.InitailizeTran();

            return StateModel.OP_EXIT;
        }
    }
}

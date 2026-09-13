using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;
using EBM2x.Services;

namespace EBM2x.Process.signon
{
    public class SignOnProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            
            StoreRecord storerecord = StoreService.Load(posModel.Environment.EnvPosNode.StoreCode);
            
            posModel.RegiTotal.RegiHeader.StoreName = storerecord.StoreName;

            posModel.ReceiptInfor.PosRcptMsg1 = storerecord.NameplateLine1;              
            posModel.ReceiptInfor.PosRcptMsg2 = storerecord.NameplateLine2;            
            posModel.ReceiptInfor.PosRcptMsg3 = storerecord.NameplateLine3;             
            posModel.ReceiptInfor.PosRcptMsg4 = storerecord.NameplateLine4;         

            posModel.ReceiptInfor.PosLocMsg1 = storerecord.StoreMessage1; 
            posModel.ReceiptInfor.PosLocMsg2 = storerecord.StoreMessage2; 

            posModel.ReceiptInfor.PosInfoMsg1 = storerecord.Message1;    
            posModel.ReceiptInfor.PosInfoMsg2 = storerecord.Message2;    
            posModel.ReceiptInfor.PosInfoMsg3 = storerecord.Message3;   

            if (posModel.TranModel.SignOnNode != null)
            {
                
                Journal.signon.SignOnJournal journalCreate = new Journal.signon.SignOnJournal();
                journalCreate.create(posModel);

                OtherEndOfTransaction.excuteProcess(posModel, inputModel, informationModel);

                // TranNode 
                PosModelInitialize.excuteProcess(posModel, inputModel, informationModel);

                posModel.TranModel.Clear();

                return StateModel.OP_NEXT;
            }
            else
            {
                return StateModel.OP_EXIT;
            }
        }
    }
}

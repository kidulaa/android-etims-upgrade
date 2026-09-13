using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Services;

namespace EBM2x.Process.start
{
    public class OpenCheck
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel InformationModel)
        {
         
            StoreRecord record = StoreService.Load(posModel.Environment.EnvPosNode.StoreCode);
            posModel.ReceiptInfor.PosRcptMsg1 = record.NameplateLine1;
            posModel.ReceiptInfor.PosRcptMsg2 = record.NameplateLine2;
            posModel.ReceiptInfor.PosRcptMsg3 = record.NameplateLine3;
            posModel.ReceiptInfor.PosRcptMsg4 = record.NameplateLine4;

            posModel.ReceiptInfor.PosLocMsg1 = record.StoreMessage1;
            posModel.ReceiptInfor.PosLocMsg2 = record.StoreMessage2;

            posModel.ReceiptInfor.PosInfoMsg1 = record.Message1;
            posModel.ReceiptInfor.PosInfoMsg2 = record.Message2;
            posModel.ReceiptInfor.PosInfoMsg3 = record.Message3;

            string toDay = System.DateTime.Now.ToString("yyyyMMdd");

            // POS
            /*
            if (posModel.RegiTotal.RegiHeader.IsOpened() && posModel.RegiTotal.RegiHeader.IsClosed())
            {
                if (posModel.RegiTotal.RegiHeader.OpenDate.Equals(toDay))
                {
                    return StateModel.OP_FAR;
                }
                else
                {
                    return StateModel.OP_NEXT;
                }
            }
            */
            if (posModel.RegiTotal.RegiHeader.IsClosed())
            {
                /*
                
                if (posModel.RegiTotal.RegiHeader.OpenDate.Equals(toDay))
                {
                    
                    return StateModel.OP_FAR;
                }
                
                else
                {
                    // SIGN ON
                    return StateModel.OP_NEXT;
                }
                */
                return StateModel.OP_FAR;
            }

            /* 
             *
            else if (posModel.RegiTotal.RegiHeader.OpenDate.CompareTo(toDay) == 0)
            {
                return StateModel.OP_NEXT;
            }
            else
            {
                return StateModel.OP_FAR;
            }
            */

            return StateModel.OP_NEXT;
        }
    }
}

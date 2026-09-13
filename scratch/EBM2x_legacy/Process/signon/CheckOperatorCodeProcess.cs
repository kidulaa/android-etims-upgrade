using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.signon
{
    public class CheckOperatorCodeProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.SignOnNode != null)
            {
                
                RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;
                OperHeader operHeader = posModel.OperTotal.OperHeader;
                //if (operHeader.OpenFlag && !operHeader.CloseFlag && operHeader.ReceiptNo + 1 < regiHeader.ReceiptNo &&
                //   !operHeader.UserID.Equals(posModel.TranModel.SignOnNode.CashierID))

                if (operHeader.OpenFlag && !operHeader.CloseFlag &&
                   !operHeader.UserID.Equals(posModel.TranModel.SignOnNode.CashierID))
                {
                    string strMessage = posModel.OperTotal.OperHeader.UserID + "(" + posModel.OperTotal.OperHeader.UserName + ")";

                    informationModel.SetAlertMessage("It is different from the previous cashier.", strMessage);
                    informationModel.AlertTitle = "";
                    informationModel.AlertMessage = "It is different from the previous cashier."; 
                    return StateModel.OP_ALERT;
                }
                return StateModel.OP_NEXT;
            }
            else
            {
                informationModel.SetWarningMessage("Please restart the program!");
                return StateModel.OP_RETRY;
            }
        }
    }
}

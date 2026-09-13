using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.signon
{
    public class InputAdminOperatorCodeProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.SignOnNode != null)
            {
                RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;

               
                //if (posModel.Environment.EnvPosSetup.PosAdminCode.Equals(inputModel.EnteredText))
                if (inputModel.EnteredText == "99999")
                {
                    //posModel.TranModel.SignOnNode.CashierID = posModel.Environment.EnvPosSetup.PosAdminCode;
                    posModel.TranModel.SignOnNode.CashierID = "99999";
                    posModel.TranModel.SignOnNode.CashierName = "administrator";

                    return StateModel.OP_NEXT;
                }
                else
                {
                    informationModel.SetWarningMessage("Unregistered USER. Please check the USER ID!");
                    return StateModel.OP_RETRY;
                }
            }
            else
            {
                informationModel.SetWarningMessage("Please restart the program!");
                return StateModel.OP_RETRY;
            }
        }
        public static string excuteProcessII(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.SignOnNode != null)
            {
                RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;

                if (inputModel.EnteredText == posModel.Environment.EnvPosSetup.GblTaxIdNo)
                {
                    //posModel.TranModel.SignOnNode.CashierID = posModel.Environment.EnvPosSetup.PosAdminCode;
                    posModel.TranModel.SignOnNode.CashierID = posModel.Environment.EnvPosSetup.GblTaxIdNo;
                    posModel.TranModel.SignOnNode.CashierName =posModel.Environment.EnvPosSetup.GblBrcNam;

                    return StateModel.OP_NEXT;
                }
                else
                {
                    informationModel.SetWarningMessage("Unregistered USER. Please check the USER ID!");
                    return StateModel.OP_RETRY;
                }
            }
            else
            {
                informationModel.SetWarningMessage("Please restart the program!");
                return StateModel.OP_RETRY;
            }
        }
    }
}

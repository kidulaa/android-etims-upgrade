using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.regitotal;
using EBM2x.Models.tran;

namespace EBM2x.Process.signon
{
    public class InputAdminPasswordProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.SignOnNode != null)
            {
                // JINIT ID를 "11111"
                /*
                if (posModel.Environment.EnvPosSetup.PosAdminPassword.Equals(inputModel.EnteredText))
                {
                    return StateModel.OP_NEXT;
                }
                else
                {
                    informationModel.SetWarningMessage("Password is wrong. Please check again..");
                    return StateModel.OP_RETRY;
                }
                */

                if (inputModel.EnteredText == Utils.Common.getAdminPass())
                {
                    return StateModel.OP_NEXT;
                }
                else
                {
                    informationModel.SetWarningMessage("Password is wrong. Please check again..");
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
                if (inputModel.EnteredText == Utils.Common.getAdminPassII())
                {
                    return StateModel.OP_NEXT;
                }
                else
                {
                    informationModel.SetWarningMessage("Password is wrong. Please check again..");
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

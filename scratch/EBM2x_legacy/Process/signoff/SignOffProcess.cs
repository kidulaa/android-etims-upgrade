using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.signoff
{
    public class SignOffProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            posModel.RegiTotal.RegiHeader.UserID = string.Empty;
            posModel.RegiTotal.RegiHeader.UserName = string.Empty;

            OtherEndOfTransaction.excuteProcess(posModel, inputModel, informationModel);

            posModel.TranModel.Clear();

            return StateModel.OP_NEXT;
        }
    }
}

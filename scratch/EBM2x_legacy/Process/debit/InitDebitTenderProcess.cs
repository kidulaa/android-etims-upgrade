using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.debit
{
    public class InitDebitTenderProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            return StateModel.OP_NEXT;
        }
    }
}

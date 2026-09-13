using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.tran
{
    public class CancelPaymentProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.TenderList.CurrentLineNumber < 1) return StateModel.OP_RETRY;

            posModel.TranModel.TranNode.TenderList.DeleteTender();
            posModel.TranModel.TranNode.TenderList.CurrentLineNumber = -1;

            posModel.TranModel.TranNode.CalculateTenderList();

            return StateModel.OP_NEXT;
        }
    }
}

using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.mobilewallet
{
    public class MobileWalletTenderProcess
    {
        public static string excuteProcess(TenderNode tenderNode, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            posModel.TranModel.TranNode.TenderList.Add(tenderNode);
            posModel.TranModel.TranNode.CalculateTenderList();

            if (posModel.TranModel.TranNode.AmountToReceive() == 0)
            {
                return StateModel.OP_TRANSACTION_END;
            }
            return StateModel.OP_NEXT;
        }
    }
}

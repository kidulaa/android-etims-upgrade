using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.tran
{
    public class CancelSalesProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            posModel.TranModel.TranNode.ItemList.Clear();
            posModel.TranModel.TranNode.TenderList.Clear();
            posModel.TranModel.TranNode.CalculateItemList();
            posModel.TranModel.TranNode.CalculateTenderList();

            return StateModel.OP_NEXT;
        }
    }
}

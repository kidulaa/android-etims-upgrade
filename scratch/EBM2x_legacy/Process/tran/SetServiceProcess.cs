using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.tran
{
    public class SetServiceProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return StateModel.OP_RETRY;

            double price = 0;

            posModel.TranModel.TranNode.ItemList.ChangePrice(true, price);
            posModel.TranModel.TranNode.CalculateItemList();

            return StateModel.OP_NEXT;
        }
    }
}

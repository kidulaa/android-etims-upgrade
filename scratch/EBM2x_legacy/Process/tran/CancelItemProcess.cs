using EBM2x.Models;

namespace EBM2x.Process.tran
{
    public class CancelItemProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return StateModel.OP_RETRY;

            bool IsDeleteItem = true;
            if (IsDeleteItem)
            {
                posModel.TranModel.TranNode.ItemList.DeleteItem();
                posModel.TranModel.TranNode.ItemList.CurrentLineNumber = -1;
            }
            else
            {
                posModel.TranModel.TranNode.ItemList.CancelItem();
            }

            posModel.TranModel.TranNode.CalculateItemList();

            return StateModel.OP_NEXT;
        }
    }
}

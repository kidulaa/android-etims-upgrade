using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.tran
{
    public class ChangePriceProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return StateModel.OP_RETRY;

            double price = 0;

            if (!string.IsNullOrEmpty(inputModel.EnteredText))
            {
                if (inputModel.EnteredText.Length > 8)
                {
                    informationModel.SetWarningMessage("Please enter a price between 1 and 99,999,999.");
                    return StateModel.OP_RETRY;
                }
                price = double.Parse(inputModel.EnteredText);

                if(price <= 0)
                {
                    informationModel.SetWarningMessage("Please enter a price between 1 and 99,999,999.");
                    return StateModel.OP_RETRY;
                }
            }
            else
            {
                informationModel.SetWarningMessage("Please enter a price value.");
                return StateModel.OP_RETRY;
            }

            posModel.TranModel.TranNode.ItemList.ChangePrice(price);
            posModel.TranModel.TranNode.CalculateItemList();

            return StateModel.OP_NEXT;
        }
    }
}

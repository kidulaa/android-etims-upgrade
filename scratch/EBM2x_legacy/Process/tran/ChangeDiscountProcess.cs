using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.tran
{
    public class ChangeDiscountProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (posModel.TranModel.TranNode.ItemList.CurrentLineNumber < 1) return StateModel.OP_RETRY;


            int discount = 0;

            if (!string.IsNullOrEmpty(inputModel.EnteredText))
            {
                if(inputModel.EnteredText.Length > 2)
                {
                    informationModel.SetWarningMessage("Please enter a discount rate between 0 and 99.");
                    return StateModel.OP_RETRY;
                }
                discount = int.Parse(inputModel.EnteredText);
            }
            else
            {
                informationModel.SetWarningMessage("Please enter a discount rate.");
                return StateModel.OP_RETRY;
            }


            posModel.TranModel.TranNode.ItemList.ChangeDiscount(discount);
            posModel.TranModel.TranNode.CalculateItemList();

            return StateModel.OP_NEXT;
        }
    }
}

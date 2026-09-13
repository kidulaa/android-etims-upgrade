using EBM2x.Datafile.regitotal;
using EBM2x.Models;
using EBM2x.Models.ReadyMoney;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.eot
{
    public class CloaseEndOfTransaction
    {
        public static string excuteProcess(ReadyMoneyList list, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (!posModel.RegiTotal.RegiHeader.OpenDate.Equals(string.Empty) && list != null)
            {
                updateCloase(posModel.RegiTotal.RegiDetail, list);
                updateCloase(posModel.OperTotal.RegiDetail, list);
            }

         
            posModel.RegiTotal.RegiHeader.UpdateDateTime();

            
            RegiTotalWriter.write(posModel);
            // OperTotal json 
            OperTotalWriter.write(posModel);
            //posModel.RegiTotal.RegiHeader.increaseReceiptNo();

            return StateModel.OP_FAR;
        }

        public static void updateCloase(RegiDetail regiDetail, ReadyMoneyList list)
        {
            regiDetail.CashStock.Count += 1;
            regiDetail.CashStock.Amount = list.GetTotal();
            regiDetail.CashStock.SubtotalAmount = list.GetTotal();
            regiDetail.CashStock.DiscountAmount = 0;
        }
    }
}

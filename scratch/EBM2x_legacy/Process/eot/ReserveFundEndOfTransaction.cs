using EBM2x.Datafile.regitotal;
using EBM2x.Models;
using EBM2x.Models.ReadyMoney;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.eot
{
    public class ReserveFundEndOfTransaction
    {
        public static string excuteProcess(ReadyMoneyList list, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (!posModel.RegiTotal.RegiHeader.OpenDate.Equals(string.Empty) && list != null)
            {
                updateReserveFund(posModel.RegiTotal.RegiDetail, list);
            }

            
            posModel.RegiTotal.RegiHeader.UpdateDateTime();

            ////JCNA 20191203
            //posModel.RegiTotal.RegiHeader.decreaseReceiptNo();
            // RegiTotal json 
            RegiTotalWriter.write(posModel);
            // OperTotal json
            OperTotalWriter.write(posModel);
            //posModel.RegiTotal.RegiHeader.increaseReceiptNo();

            return StateModel.OP_FAR;
        }

        public static void updateReserveFund(RegiDetail regiDetail, ReadyMoneyList list)
        {
            regiDetail.ReservedFund.Count += 1;
            regiDetail.ReservedFund.Amount = list.GetTotal();
            regiDetail.ReservedFund.SubtotalAmount = list.GetTotal();
            regiDetail.ReservedFund.DiscountAmount += 0;
        }
    }
}

using EBM2x.Datafile.regitotal;
using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Models.ReadyMoney;
using EBM2x.Models.regitotal;

namespace EBM2x.Process.eot
{
    public class IntermediateDepositEndOfTransaction
    {
        public static string excuteProcess(ReadyMoneyList list, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (!posModel.RegiTotal.RegiHeader.OpenDate.Equals(string.Empty) && list != null)
            {
                updateIntermediateDeposit(posModel.RegiTotal.RegiDetail, list);
            }

         
            posModel.RegiTotal.RegiHeader.UpdateDateTime();

            
            RegiTotalWriter.write(posModel);
            // OperTotal json 
            OperTotalWriter.write(posModel);
            //posModel.RegiTotal.RegiHeader.increaseReceiptNo();

            return StateModel.OP_FAR;
        }

        public static void updateIntermediateDeposit(RegiDetail regiDetail, ReadyMoneyList list)
        {
            if (regiDetail.MidStockCash1.Count == 0)
            {
                regiDetail.MidStockCash1.Count += 1;
                regiDetail.MidStockCash1.Amount = list.GetTotal();
                regiDetail.MidStockCash1.SubtotalAmount = list.GetTotal();
                regiDetail.MidStockCash1.DiscountAmount += 0;
            }
            else if (regiDetail.MidStockCash2.Count == 0)
            {
                regiDetail.MidStockCash2.Count += 1;
                regiDetail.MidStockCash2.Amount = list.GetTotal();
                regiDetail.MidStockCash2.SubtotalAmount = list.GetTotal();
                regiDetail.MidStockCash2.DiscountAmount += 0;
            }
            else if (regiDetail.MidStockCash3.Count == 0)
            {
                regiDetail.MidStockCash3.Count += 1;
                regiDetail.MidStockCash3.Amount = list.GetTotal();
                regiDetail.MidStockCash3.SubtotalAmount = list.GetTotal();
                regiDetail.MidStockCash3.DiscountAmount += 0;
            }
            else if (regiDetail.MidStockCash4.Count == 0)
            {
                regiDetail.MidStockCash4.Count += 1;
                regiDetail.MidStockCash4.Amount = list.GetTotal();
                regiDetail.MidStockCash4.SubtotalAmount = list.GetTotal();
                regiDetail.MidStockCash4.DiscountAmount += 0;
            }
            else
            {
                regiDetail.MidStockCash4.Count += 1;
                regiDetail.MidStockCash4.Amount += list.GetTotal();
                regiDetail.MidStockCash4.SubtotalAmount += list.GetTotal();
                regiDetail.MidStockCash4.DiscountAmount += 0;
            }
        }
    }
}

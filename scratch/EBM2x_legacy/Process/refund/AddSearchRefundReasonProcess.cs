using EBM2x.Models;
using EBM2x.Models.ListView;
using EBM2x.Models.tran;

namespace EBM2x.Process.refund
{
    public class AddSearchRefundReasonProcess
    {
        public static string excuteProcess(SearchRefundReasonNode searchRefundReasonNode, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            posModel.TranModel.TranNode.RefundReasonNode.ReasonCode = searchRefundReasonNode.ReasonCode;
            posModel.TranModel.TranNode.RefundReasonNode.ReasonText = searchRefundReasonNode.ReasonText;

            posModel.TranModel.TranNode.TranFlag = TranDefine.TRAN_RETURN;
            posModel.TranModel.TranNode.Sign = -1;

            posModel.TranModel.TranNode.CalculateItemList();
            return StateModel.OP_NEXT;
        }
        public static string excuteProcess(SearchRefundReasonNode searchRefundReasonNode, TranModel tranModel, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            tranModel.TranNode.RefundReasonNode.ReasonCode = searchRefundReasonNode.ReasonCode;
            tranModel.TranNode.RefundReasonNode.ReasonText = searchRefundReasonNode.ReasonText;

           
            tranModel.TranNode.RefundReasonNode.OrgBarcodeNo = 
                "0" + tranModel.TranInformation.SaleDate + tranModel.TranInformation.PosNumber + tranModel.TranInformation.ReceiptNumber.ToString("0000#");
            tranModel.TranNode.RefundReasonNode.OrgInvoiceNo = tranModel.TranInformation.InvoiceN0.ToString("#");

      
            tranModel.TranNode.TranFlag = TranDefine.TRAN_RETURN;
            tranModel.TranNode.Sign = -1;

            
            for (int i = 0; i < tranModel.TranNode.ItemList.Count(); i++)
            {
                tranModel.TranNode.ItemList.Get(i).TranFlag = "R";
            }

            tranModel.TranNode.CalculateItemList();
            return StateModel.OP_NEXT;
        }

    }
}

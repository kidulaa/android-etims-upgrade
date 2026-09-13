using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Models.hold;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.tran
{
    public class ReloadTranProcess
    {
        public static string excuteProcess(TranModel tranModel, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            TranModel oldTranModel = posModel.TranModel;

            posModel.TranModel = tranModel;
            posModel.TranModel.TranInformation.initialize(posModel.TranInformation);
            posModel.TranModel.TranNode.ItemList.CountOfItemsToDisplayOnOnePage = oldTranModel.ItemListCountOfItemsToDisplayOnOnePage;
            posModel.TranModel.TranNode.TenderList.CountOfItemsToDisplayOnOnePage = oldTranModel.TenderListCountOfItemsToDisplayOnOnePage;

            tranModel.TranNode.CalculateItemList();

            posModel.TranModel.TranNode.Receive = 0;
            posModel.TranModel.TranNode.TenderList.Clear();

            return StateModel.OP_NEXT;
        }
    }
}

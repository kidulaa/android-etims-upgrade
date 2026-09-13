using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Models.hold;
using EBM2x.Models.signon;
using EBM2x.Models.tran;
using EBM2x.Process.eot;

namespace EBM2x.Process.hold
{
    public class RestoreHoldTranProcess
    {
        public static string excuteProcess(HoldNode holdNode, PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
           
            posModel.TranModel = holdNode.TranModel;
            posModel.TranModel.TranInformation.initialize(posModel.TranInformation);

            TrHoldWriter.DeleteFile(holdNode.HoldFileName);

            posModel.RegiTotal.RegiHeader.HoldCount = TrHoldReader.GetCount();

            return StateModel.OP_NEXT;
        }
    }
}

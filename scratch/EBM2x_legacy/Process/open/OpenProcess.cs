using EBM2x.Datafile;
using EBM2x.Models;
using EBM2x.Models.open;
using EBM2x.Models.tran;
using EBM2x.Process.dining;
using EBM2x.Process.eot;
using EBM2x.Process.preset;
using EBM2x.UI;

namespace EBM2x.Process.open
{
    public class OpenProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            //SetStartTime
            posModel.OpenTimeNode.MasterStartTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");

            // start flow
            StartFlow.excuteProcess(posModel, inputModel, informationModel);

            // OpenNode 
            posModel.TranInformation.LogFlag = TranDefine.LOG_OPEN;
            posModel.TranModel.OpenNode = new OpenNode();

            posModel.TranModel.OpenNode.OpenDate = posModel.RegiTotal.RegiHeader.OpenDate;
            posModel.TranModel.OpenNode.OpenTime = posModel.RegiTotal.RegiHeader.OpenTime;
            posModel.TranModel.OpenNode.OpenSequence = posModel.RegiTotal.RegiHeader.OpenSequence;

            // JINIT_clear data file
            Datafile.DatafileService.DeleteFiles("tran/fund");
            Datafile.DatafileService.DeleteFiles("tran/regitotal");
            Datafile.DatafileService.DeleteFiles("tran/receipt");
            Datafile.DatafileService.DeleteFiles("tran/receipt/send");
            Datafile.DatafileService.DeleteFiles("tran/hold");

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Restaurant"))
            {
                // JINIT_201911, Dining
                Datafile.DatafileService.DeleteFiles("DiningTableOrder");
                dining.DiningRoomNodeProcess.InitDiningTable();
                posModel.DiningTableModel.DiningRoomList = DiningRoomNodeProcess.LoadDiningTable(5, 20);
            }

            // end flow
            EndFlow.excuteProcess(posModel, inputModel, informationModel);

            // SetEndTime
            posModel.OpenTimeNode.MasterEndTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");

            // end open
            EndOpen.excuteProcess(posModel, inputModel, informationModel);

            // End Of Transaction
            posModel.RegiTotal.RegiHeader.UpdateDate = string.Empty;

            
            Journal.open.OpenJournal journalCreate = new Journal.open.OpenJournal();
            journalCreate.create(posModel);

          
            OtherEndOfTransaction.excuteProcess(posModel, inputModel, informationModel);

            // TranNode
            PosModelInitialize.excuteProcess(posModel, inputModel, informationModel);

            posModel.TranModel.Clear();

            return StateModel.OP_NEXT;
        }
    }
}

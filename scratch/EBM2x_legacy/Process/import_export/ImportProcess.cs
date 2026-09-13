using EBM2x.Database.Master;
using EBM2x.Models;

namespace EBM2x.Process.import_export
{
    public class ImportProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            ItemClassRecord record = new ItemClassRecord();

            //ItmitemclsMaster db = new ItmitemclsMaster();
            //db.ToRecord(record, "45111603");

            return StateModel.OP_NEXT;
        }
    }
}

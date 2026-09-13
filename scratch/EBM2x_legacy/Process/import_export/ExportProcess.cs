using EBM2x.Models;

namespace EBM2x.Process.import_export
{
    public class ExportProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            return StateModel.OP_NEXT;
        }
    }
}

using EBM2x.Models;

namespace EBM2x.Process.import_export
{
    public class CreateDBProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            //MsSQLiteEBM2xClientProvider.getInstance().CreateDatabase();

            return StateModel.OP_NEXT;
        }
    }
}

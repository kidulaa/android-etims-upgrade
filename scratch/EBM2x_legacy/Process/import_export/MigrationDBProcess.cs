using EBM2x.Database;
using EBM2x.Database.Master;
using EBM2x.Models;

namespace EBM2x.Process.import_export
{
    public class MigrationDBProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            //Ebm2xMigrationManager.MigrationTables();

            return StateModel.OP_NEXT;
        }
    }
}

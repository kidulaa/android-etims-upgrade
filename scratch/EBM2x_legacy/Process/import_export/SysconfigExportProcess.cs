using EBM2x.Database.Master;
using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Services;
using System.Collections.Generic;

namespace EBM2x.Process.import_export
{
    public class SysconfigExportProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            SysconfigMaster sysconfigMaster = new SysconfigMaster();
            List<SysconfigRecord> list = sysconfigMaster.getSysconfigTable();

            SysconfigService.Save(list);

            return StateModel.OP_NEXT;
        }
    }
}

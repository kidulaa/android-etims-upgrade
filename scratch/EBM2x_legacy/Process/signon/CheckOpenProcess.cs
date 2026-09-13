using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.regitotal;
using EBM2x.Services;

namespace EBM2x.Process.signon
{
    public class CheckOpenProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            StoreRecord storerecord = StoreService.Load(posModel.Environment.EnvPosNode.StoreCode);
            if (string.IsNullOrEmpty(storerecord.StoreCode))
            {
                informationModel.SetAlertMessage("POS not available", "Please contact your administrator.");
                return StateModel.OP_ALERT_YES_NO;
            }
            return StateModel.OP_NEXT;
        }
    }
}

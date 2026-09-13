using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.open;
using EBM2x.Models.tran;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.open
{
    public class EndOpen
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            posModel.RegiTotal.RegiHeader.TRSend = true;

            OpenNode openNode = posModel.TranModel.OpenNode;

            //openNode.SwVer = Model.MPosInformation.getInstance().version();
            //openNode.VanFlag = environmentNode.EnvVanNode.VanModel;
            //openNode.PluFlag = environmentNode.EnvDeviceNode.PLUKeyboard;
            //openNode.IpAddr = Beans.util.NetPing.getLocalIP().ToString();
            openNode.UseFlag = "Y";

            // v5.9.3_opendate, opentime 
            if (openNode.OpenDate.Length == 0)
            {
                openNode.OpenDate = System.DateTime.Now.ToString("yyyyMMdd"); 
                openNode.OpenTime = System.DateTime.Now.ToString("HHmmss");  
            }

            return StateModel.OP_TRANSACTION_END;
        }
    }
}

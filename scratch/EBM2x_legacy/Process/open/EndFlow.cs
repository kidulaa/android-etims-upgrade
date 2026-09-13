using EBM2x.Models;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.open
{
    public class EndFlow
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            string posNum = posModel.RegiTotal.RegiHeader.RegiNo.ToString();
            try
            {
              
                posModel.RegiTotal.RegiDetail.Clear();
                posModel.OperTotal.RegiDetail.Clear();
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }

            posModel.OpenTimeNode.MasterEndTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");

            return StateModel.OP_NEXT;
        }
    }
}

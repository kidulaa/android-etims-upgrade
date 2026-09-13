using EBM2x.Models;
using EBM2x.Models.hold;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Datafile.trlog
{
    public class TrHoldWriter :  DatafileService
    {
        public static string GetFileName(string tranName)
        {
            return GetFileName("tran/hold", tranName + ".json");
        }
        public static bool DeleteFile(string strFileName)
        {
            return DeleteFile("tran/hold", strFileName + ".json");
        }
        public static bool write(PosModel posModel)
        {
            string strFileName = string.Format("hold_{0:000}_{1}", posModel.RegiTotal.RegiHeader.HoldNo, posModel.RegiTotal.RegiHeader.UpdateTime); // 보류번호

            HoldNode holdNode = new HoldNode();

            holdNode.HoldFileName = strFileName;
            holdNode.UpdateTime = posModel.RegiTotal.RegiHeader.UpdateTime;
            holdNode.TranModel = posModel.TranModel;

            return write(holdNode, strFileName);
        }
        public static bool write(HoldNode holdNode, string tranName)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(holdNode, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(tranName), false))
                {
                    string jsonRequestPassword = Common.AESEncrypt256(Common.AES256Password(), jsonRequest);
                    file.Write(jsonRequestPassword);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

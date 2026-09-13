using EBM2x.Models;
using EBM2x.Models.regitotal;
using Newtonsoft.Json;

namespace EBM2x.Datafile.regitotal
{
    public class OperTotalWriter : DatafileService
    {
        public static string GetOperFileName(string operSeq)
        {
            return GetFileName("tran/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static string GetOperFileName(string saledate, string operSeq)
        {
            return GetFileName("backup/" + saledate + "/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static bool DeleteFile(PosModel posModel)
        {
            string operSeq = posModel.RegiTotal.RegiHeader.UserID;

            return DeleteFile("tran/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static bool write(PosModel posModel)
        {
            string operSeq = posModel.RegiTotal.RegiHeader.UserID;

            return write(posModel.OperTotal, operSeq);
        }
        public static bool write(OperTotal opertotal, string operSeq)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(opertotal, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetOperFileName(operSeq), false))
                {
                    file.Write(jsonRequest);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool write(OperTotal opertotal, string saledate, string operSeq)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(opertotal, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetOperFileName(saledate, operSeq), false))
                {
                    file.Write(jsonRequest);
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

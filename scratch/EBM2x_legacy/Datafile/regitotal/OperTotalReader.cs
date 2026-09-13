using EBM2x.Models;
using EBM2x.Models.regitotal;
using Newtonsoft.Json;

namespace EBM2x.Datafile.regitotal
{
    public class OperTotalReader : DatafileService
    {
        public static string GetOperFileName(string operSeq)
        {
            return GetFileName("tran/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static string GetOperFileName(string saledate, string operSeq)
        {
            return GetFileName("backup/" + saledate + "/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }

        public static bool IsOperTotal(string operSeq)
        {
            return IsFileExist("tran/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static bool IsOperTotal(string saledate, string operSeq)
        {
            return IsFileExist("backup/" + saledate + "/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        
        public static void read(PosModel posModel)
        {
            string operSeq = posModel.RegiTotal.RegiHeader.UserID;

            OperTotal operTotal = read(operSeq);
            posModel.OperTotal = operTotal;
        }

        public static OperTotal read(string operSeq)
        {
            OperTotal opertotal = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetOperFileName(operSeq)))
                {
                    opertotal = JsonConvert.DeserializeObject<OperTotal>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return opertotal;
        }
        public static OperTotal read(string saledate, string operSeq)
        {
            OperTotal opertotal = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetOperFileName(saledate, operSeq)))
                {
                    opertotal = JsonConvert.DeserializeObject<OperTotal>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return opertotal;
        }
    }
}

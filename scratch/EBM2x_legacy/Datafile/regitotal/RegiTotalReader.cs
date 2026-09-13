using EBM2x.Models;
using EBM2x.Models.regitotal;
using Newtonsoft.Json;

namespace EBM2x.Datafile.regitotal
{
    public class RegiTotalReader : DatafileService
    {
        public static string GetFileName()
        {
            return GetFileName("tran/regitotal", "RegiTotal.json");
        }
        public static string GetFileName(string saledate)
        {
            return GetFileName("backup/" + saledate + "/regitotal", "RegiTotal.json");
        }

        public static bool IsRegiTotal()
        {
            return IsFileExist("tran/regitotal", "RegiTotal.json");
        }
        public static bool IsRegiTotal(string saledate)
        {
            return IsFileExist("backup/" + saledate + "/regitotal", "RegiTotal.json");
        }

        public static void read(PosModel posModel)
        {
            RegiTotal regiTotal = read();
            if (regiTotal != null) posModel.RegiTotal = regiTotal;
        }

        public static RegiTotal read()
        {
            RegiTotal regiTotal = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    regiTotal = JsonConvert.DeserializeObject<RegiTotal>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return regiTotal;
        }
        public static RegiTotal read(string saledate)
        {
            RegiTotal regiTotal = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(saledate)))
                {
                    regiTotal = JsonConvert.DeserializeObject<RegiTotal>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return regiTotal;
        }
    }
}

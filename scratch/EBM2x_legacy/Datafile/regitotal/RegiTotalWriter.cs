using EBM2x.Models;
using EBM2x.Models.regitotal;
using Newtonsoft.Json;

namespace EBM2x.Datafile.regitotal
{
    public class RegiTotalWriter : DatafileService
    {
        public static string GetFileName()
        {
            return GetFileName("tran/regitotal", "RegiTotal.json");
        }
        public static string GetFileName(string saledate)
        {
            return GetFileName("backup/" + saledate + "/regitotal", "RegiTotal.json");
        }
        public static bool DeleteFile()
        {
            return DeleteFile("tran/regitotal", "RegiTotal.json");
        }
        public static bool DeleteFileAll()
        {
            return DeleteFiles("tran/regitotal");
        }

        public static bool write(PosModel posModel)
        {
            return write(posModel.RegiTotal);
        }

        public static bool write(RegiTotal regitotal)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(regitotal, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
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
        public static bool write(RegiTotal regitotal, string saledate)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(regitotal, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(saledate), false))
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

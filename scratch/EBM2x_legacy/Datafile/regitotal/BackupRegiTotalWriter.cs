using EBM2x.Models;
using EBM2x.Models.regitotal;

namespace EBM2x.Datafile.regitotal
{
    public class BackupRegiTotalWriter : DatafileService
    {
        public static string GetFileName()
        {
            return GetFileName("tran/regitotal", "RegiTotal.json");
        }
        public static string GetFileName(string saledate)
        {
            // JINIT_경로변경, return GetFileName("backup/" + saledate + "/regitotal", "RegiTotal.json");
            return GetFileName("backup/" + saledate + "/tran/regitotal", "RegiTotal.json");
        }
        public static bool write(PosModel posModel, string filedate)
        {
            RegiTotal regiTotal = RegiTotalReader.read();
            if (regiTotal != null)
            {
                return RegiTotalWriter.write(regiTotal, filedate);
            }
            else
            {
                return false;
            }
        }
    }
}

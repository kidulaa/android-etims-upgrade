using EBM2x.Models;
using EBM2x.Models.regitotal;

namespace EBM2x.Datafile.regitotal
{
    public class BackupOperTotalWriter : DatafileService
    {
        public static string GetFileName(string operSeq)
        {
            return GetFileName("tran/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static string GetFileName(string saledate, int operSeq)
        {
            return GetFileName("backup/" + saledate + "/regitotal", string.Format("OperTotal{0}.json", operSeq));
        }
        public static bool write(PosModel posModel, string filedate, string operSeq)
        {
            OperTotal operTotal = OperTotalReader.read(operSeq);
            if (operTotal != null)
            {
                return OperTotalWriter.write(operTotal, filedate, operSeq);
            }
            else
            {
                return false;
            }
        }
    }
}

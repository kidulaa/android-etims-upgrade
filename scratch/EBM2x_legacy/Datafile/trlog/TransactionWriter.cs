using EBM2x.Models;
using EBM2x.Utils;
using Newtonsoft.Json;

namespace EBM2x.Datafile.trlog
{
    public class TransactionWriter : DatafileService
    {
        public static string GetFileName(string tranName)
        {
            return GetFileName("tran/receipt", tranName +".json");
        }
        public static string GetBackupFileName(string saledate, string tranName)
        {
            return GetFileName("backup/" + saledate + "/receipt", tranName + ".json");
        }
        public static bool DeleteFile(PosModel posModel)
        {
            string tranName = posModel.RegiTotal.RegiHeader.getBarcodeText(posModel.TranModel.TranInformation.ReceiptNumber);

            return DeleteFile("tran/receipt", tranName + ".json");
        }
        public static bool DeleteFile(TranModel tranModel)
        {
            string tranName = string.Empty;

            if (tranModel.TranInformation.SaleDate != null && !tranModel.TranInformation.SaleDate.Equals(string.Empty))
            {
                //18  //--2--------8---------------4------------------------4-----//
                tranName = "0" + tranModel.TranInformation.SaleDate + tranModel.TranInformation.PosNumber + tranModel.TranInformation.ReceiptNumber.ToString("0000#");
            }
            else
            {
                //18 //--2--------8-------2------1-------4-----//
                tranName = "0" + "00000000" + "0000" + tranModel.TranInformation.ReceiptNumber.ToString("0000#");
            }

            return DeleteFile("tran/receipt", tranName + ".json");
        }
        public static bool DeleteFileAll()
        {
            DeleteFiles("tran/fund");
            DeleteFiles("tran/hold");
            return DeleteFiles("tran/receipt");
        }
        public static bool write(PosModel posModel)
        {
            //string tranName = posModel.RegiTotal.RegiHeader.getBarcodeText(posModel.TranModel.TranInformation.ReceiptNumber);
            //return write(posModel.TranModel, tranName);
            return write(posModel.TranModel);
        }

        public static bool write(TranModel tranModel)
        {
            string tranName = string.Empty;

            if (tranModel.TranInformation.SaleDate != null && !tranModel.TranInformation.SaleDate.Equals(string.Empty))
            {
                //18 //--2--------8---------------4------------------------4-----//
                tranName = "0" + tranModel.TranInformation.SaleDate + tranModel.TranInformation.PosNumber + tranModel.TranInformation.ReceiptNumber.ToString("0000#");
            }
            else
            {
                //18  //--2--------8-------2------1-------4-----//
                tranName = "0" + "00000000" + "0000" + tranModel.TranInformation.ReceiptNumber.ToString("0000#");
            }
            return write(tranModel, tranName);
        }

        public static bool write(TranModel tranModel, string tranName)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(tranModel, Formatting.Indented);

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
        public static bool write(TranModel tranModel, string saledate, string tranName)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(tranModel, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetBackupFileName(saledate, tranName), false))
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

        
        public static bool overwrite(TranModel tranModel, string tranName)
        {
            if (! TransactionReader.IsTranNode(tranName)) return false;

            try
            {
                var jsonRequest = JsonConvert.SerializeObject(tranModel, Formatting.Indented);
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

        public static bool overwrite(TranModel tranModel, string saledate, string tranName)
        {
            try
            {
                if (!TransactionReader.IsBackupTranNode(tranName.Substring(1, 8), tranName)) return false;

                var jsonRequest = JsonConvert.SerializeObject(tranModel, Formatting.Indented);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetBackupFileName(saledate, tranName), false))
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

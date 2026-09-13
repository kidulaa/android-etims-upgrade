using EBM2x.Models;
using EBM2x.Models.ListView;
using EBM2x.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EBM2x.Datafile.trlog
{
    public class TransactionReader : DatafileService
    {
        public static string GetPathName()
        {
            return GetPathName("tran/receipt"); 
        }
        public static string GetFileName(string tranName)
        {
            return GetFileName("tran/receipt", tranName + ".json");
        }
        public static string GetBackupFileName(string saledate, string tranName)
        {
            return GetFileName("backup/" + saledate + "/receipt", tranName + ".json");
        }
        // JINIT_backup폴더추가
        public static string GetBackupPathName(string saledate)
        {
            return GetPathName("backup/" + saledate + "/receipt");
        }

        public static bool IsTranNode(string tranName)
        {
            return IsFileExist("tran/receipt", tranName + ".json");
        }
        public static bool IsBackupTranNode(string saledate, string tranName)
        {
            return IsFileExist("backup/" + saledate + "/receipt", tranName + ".json");
        }

        public static TranModel read(string tranName)
        {
            TranModel tranModel = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(tranName)))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                    tranModel = JsonConvert.DeserializeObject<TranModel>(buffer);
                }
            }
            catch
            {
            }

            return tranModel;
        }
        public static TranModel read(string saledate, string tranName)
        {
            TranModel tranModel = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetBackupFileName(saledate, tranName)))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword); 
                    tranModel = JsonConvert.DeserializeObject<TranModel>(buffer);
                }
            }
            catch
            {
            }

            return tranModel;
        }
        public static TranModel readSystempath(string tranName)
        {
            TranModel tranModel = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(tranName))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                    tranModel = JsonConvert.DeserializeObject<TranModel>(buffer);
                }
            }
            catch
            {
            }

            return tranModel;
        }
        public static List<TranModel> GetTransactionList()
        {
            List<TranModel> list = new List<TranModel>();

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        string bufferPassword = reader.ReadToEnd();
                        string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                        TranModel tranModel = JsonConvert.DeserializeObject<TranModel>(buffer);
                        list.Add(tranModel);
                    }
                }
            }
            catch
            {
            }

            return list;
        }

        // JINIT
        public static List<SearchReceiptNode> GetSearchReceiptList(string saledate)
        {
            List<SearchReceiptNode> list = new List<SearchReceiptNode>();
            string[] filenames = null;
            try
            {
                if (saledate == "")
                {
                    // tran/receipt 
                    filenames = System.IO.Directory.GetFiles(GetPathName());
                }
                else
                {
                    // backup/receipt
                    filenames = System.IO.Directory.GetFiles(GetBackupPathName(saledate));
                }

                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        string bufferPassword = reader.ReadToEnd();
                        string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                        TranModel tranModel = JsonConvert.DeserializeObject<TranModel>(buffer);

                        SearchReceiptNode searchReceiptNode = new SearchReceiptNode();

                        searchReceiptNode.SalesDate = tranModel.TranInformation.SaleDate;
                        searchReceiptNode.ReceiptNo = string.Format("{0:0000}", tranModel.TranInformation.ReceiptNumber);
                        searchReceiptNode.InvoiceNum = string.Format("{0:#}", tranModel.TranInformation.InvoiceN0);
                        searchReceiptNode.SalesType = tranModel.TranInformation.LogFlag;

                        if (tranModel.TranNode != null)
                        {
                            searchReceiptNode.Amount = tranModel.TranNode.Subtotal;
                            searchReceiptNode.Sign = tranModel.TranNode.Sign;
                        }
                        else
                        {
                            searchReceiptNode.Amount = 0;
                            searchReceiptNode.Sign = 1;
                        }
                        searchReceiptNode.SalesFilename = file;

                        list.Add(searchReceiptNode);
                    }
                }
            }
            catch
            {
            }

            return list;
        }

        
        public static int GetCount(string saledate)
        {
            int count = 0;
            string[] filenames = null;
            try
            {
                if (saledate == "")
                {
                    // tran/receipt 
                    filenames = System.IO.Directory.GetFiles(GetPathName());
                }
                else
                {
                    // backup/receipt 
                    filenames = System.IO.Directory.GetFiles(GetBackupPathName(saledate));
                }

                foreach (string file in filenames)
                {
                    count = count + 1;
                }
            }
            catch
            {
            }

            return count;
        }

        /*
        public static List<SearchReceiptNode> GetSearchReceiptList()
        {
            List<SearchReceiptNode> list = new List<SearchReceiptNode>();

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        TranModel tranModel = JsonConvert.DeserializeObject<TranModel>(reader.ReadToEnd());

                        SearchReceiptNode searchReceiptNode = new SearchReceiptNode();

                        searchReceiptNode.SalesDate = tranModel.TranInformation.SaleDate;
                        searchReceiptNode.ReceiptNo = string.Format("{0:0000}",tranModel.TranInformation.ReceiptNumber);
                        searchReceiptNode.InvoiceNum = "0000000000";
                        searchReceiptNode.SalesType = tranModel.TranInformation.LogFlag;

                        if(tranModel.TranNode != null)
                        {
                            searchReceiptNode.Amount = tranModel.TranNode.Subtotal;
                        }
                        else
                        {
                            searchReceiptNode.Amount = 0;
                        }
                        searchReceiptNode.SalesFilename = file;

                        list.Add(searchReceiptNode);
                    }
                }
            }
            catch
            {
            }

            return list;
        }
        */

        /*
        public static int GetCount()
        {
            int count = 0;
            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    count = count + 1;
                }
            }
            catch
            {
            }

            return count;
        }
        */
    }
}

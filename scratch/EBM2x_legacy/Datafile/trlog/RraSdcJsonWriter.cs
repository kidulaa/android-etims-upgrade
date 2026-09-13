using EBM2x.Models;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.UI;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace EBM2x.Datafile.trlog
{
    public class RraSdcJsonWriter : DatafileService
    {
        public static string GetPathName()
        {
            return GetPathName("rrasdc/transaction"); ;
        }
        public static string GetFileName(string tranName)
        {
            return GetFileName("rrasdc/transaction", tranName + ".json");
        }
        public static bool DeleteFileAll()
        {
            return DeleteFiles("rrasdc/transaction");
        }
        public static bool BackupSuccessFile(RraSdcUploadModel rraSdcUploadModel)
        {
            string filename = GetFileName(rraSdcUploadModel.FileName);

            //string saledate = DateTime.Now.ToString("yyyyMMdd");
            //string backupFilename = GetFileName("rrasdc/" + saledate + "/success", rraSdcUploadModel.FileName + ".json");
            //WriteTransaction(backupFilename, rraSdcUploadModel);

            if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);

            if (System.IO.File.Exists(filename)) return false;
            else return true;
        }
        public static bool BackupErrorFile(RraSdcUploadModel rraSdcUploadModel)
        {
            string filename = GetFileName(rraSdcUploadModel.FileName);
            string saledate = DateTime.Now.ToString("yyyyMMdd");
            string backupFilename = GetFileName("rrasdc/" + saledate + "/error", rraSdcUploadModel.FileName + ".json");
            WriteTransaction(backupFilename, rraSdcUploadModel);

            if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);

            if (System.IO.File.Exists(filename)) return false;
            else return true;
        }
        public static bool WriteTransaction(RraSdcUploadModel rraSdcUploadModel)
        {
            string tranName = DateTime.Now.ToString("yyyyMMddHHmmssffff_") + rraSdcUploadModel.FileType;
            rraSdcUploadModel.FileName = tranName;
            string filename = GetFileName(tranName);

            return WriteTransaction(filename, rraSdcUploadModel);
        }
        public static bool WriteTransaction(string filename, RraSdcUploadModel rraSdcUploadModel)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false))
                {
                    var jsonRequest = JsonConvert.SerializeObject(rraSdcUploadModel, Formatting.Indented);
                    string jsonRequestPassword = Common.AESEncrypt256(Common.AES256Password(), jsonRequest);
                    //v20201012.NEW.0119
                    if (!string.IsNullOrEmpty(jsonRequestPassword))
                    {
                        file.Write(jsonRequestPassword);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<RraSdcUploadModel> GetTransactionList()
        {
            List<RraSdcUploadModel> list = new List<RraSdcUploadModel>();

            List<string> deletelist = new List<string>();

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        try
                        {
                            string bufferPassword = reader.ReadToEnd();
                            string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                            RraSdcUploadModel tranModel = JsonConvert.DeserializeObject<RraSdcUploadModel>(buffer);
                            list.Add(tranModel);
                        }
                        catch
                        {
                            //v20201012.NEW.0119
                            deletelist.Add(file);
                        }
                    }
                }
            }
            catch
            {
            }

            foreach (string file in deletelist)
            {
                try
                {
                    //v20201012.NEW.0119
                    System.IO.File.Delete(file);
                }
                catch
                {
                }
            }

            return list;
        }
        public static int GetTransactionOldCount(string currentDate)
        {
            int count = 0;
            int OfflineDays = 3;
            //UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {

                        string bufferPassword = reader.ReadToEnd();
                        string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                        RraSdcUploadModel tranModel = JsonConvert.DeserializeObject<RraSdcUploadModel>(buffer);
                        //if (!string.IsNullOrEmpty(tranModel.FileName) && tranModel.FileName.Length >= 8 && (tranModel.FunctionName == "saveTrnsSales" || tranModel.FunctionName == "saveTrnsSalesReceipt"))
                        if (!string.IsNullOrEmpty(tranModel.FileName) && tranModel.FileName.Length >= 8)
                        {
                            //int iDate = int.Parse(tranModel.FileName.Substring(0, 8));
                            string iDate = tranModel.FileName.Substring(0, 8);
                            DateTime icurDate = DateTime.ParseExact(iDate, "yyyyMMdd", null);
                            DateTime DateNow = DateTime.ParseExact(currentDate, "yyyyMMdd", null);
                            var diffDays = (DateNow - icurDate).TotalDays;
                            //int diffDays = (int.Parse(currentDate)- iDate);
                            //Console.WriteLine("different niyingiyi" + diffDays);
                            //convert into int 
                            //int diffDays = int.Parse(diffDay.Substring(0,1));
                            if (diffDays >= OfflineDays)
                            {
                                count = count + 1;
                            }
                            //end
                        }
                    }
                }
            }
            catch
            {
            }
            return count;
        }
        //Add Method Of get count of  mobile information
            public static int GetTransactionMobileCount(string currentDate)
        {
            int count = 0;
            int OfflineDays = 1;
            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        string bufferPassword = reader.ReadToEnd();
                        string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                        RraSdcUploadModel tranModel = JsonConvert.DeserializeObject<RraSdcUploadModel>(buffer);
                        //if (!string.IsNullOrEmpty(tranModel.FileName) && tranModel.FileName.Length >= 8 && (tranModel.FunctionName == "saveTrnsSales" || tranModel.FunctionName == "saveTrnsSalesReceipt"))
                        if (!string.IsNullOrEmpty(tranModel.FileName) && tranModel.FileName.Length >= 8)
                        {
                            string iDate = tranModel.FileName.Substring(0, 8);
                            //DateTime icurDate = DateTime.ParseExact(currentDate.ToString(), "yyyyMMdd", null);
                            DateTime icurDate = DateTime.ParseExact(iDate, "yyyyMMdd", null);
                            DateTime DateNow = DateTime.ParseExact(currentDate, "yyyyMMdd", null);
                            var diffDays = (DateNow - icurDate).TotalDays;
                            //int diffDays = (int.Parse(currentDate) - iDate);
                            //convert into int 
                            //int diffDays = int.Parse(diffDay.Substring(0,1));
                            if (diffDays >= OfflineDays)
                            {
                                count = count + 1;
                            }
                            //end
                        }
                    }
                }
            }
            catch
            {
            }
            return count;
        }
        //end iyo method
        public static double GetTransactionSalesReceipt()
        {
            double amount = 0;
            string dateManagement = null;

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());

                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        string bufferPassword = reader.ReadToEnd();
                        string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                        RraSdcUploadModel tranModel = JsonConvert.DeserializeObject<RraSdcUploadModel>(buffer);
                        if (tranModel.FunctionName == "saveTrnsSales")
                        {
                            try
                            {
                                TrnsSalesSaveReq trnsSalesSaveReq = JsonConvert.DeserializeObject<TrnsSalesSaveReq>(tranModel.JsonRequest);
                                Console.WriteLine("---------------sale---json" + JsonConvert.SerializeObject(trnsSalesSaveReq));
                                amount = amount + trnsSalesSaveReq.totAmt;
                                dateManagement = tranModel.RequestDt;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return amount;
        }
        public static int GetCount()
        {
            int count = 0;

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    count++;
                }
            }
            catch
            {
            }

            return count;
        }
        public static int GetErrCount()
        {
            int count = 0;

            //try
            //{
            //    string[] filenames = System.IO.Directory.GetFiles(GetPathName());
            //    foreach (string file in filenames)
            //    {
            //        count++;
            //    }
            //}
            //catch
            //{
            //}

            return count;
        }

        public static void RestoreErrorFile()
        {
            string sourceDirectory = null;
            try
            {
                sourceDirectory = GetPathName("rrasdc");
                if (Directory.Exists(sourceDirectory))
                {
                    string targetDirectory = GetPathName("rrasdc/transaction");

                    try
                    {
                        DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                        foreach (DirectoryInfo dir in diSource.GetDirectories())
                        {
                            if (dir.Name.Length > 2 && dir.Name.Substring(0, 2).Equals("20"))
                            {
                                string sourceErrDirectory = GetPathName("rrasdc/" + dir.Name + "/error");
                                if (Directory.Exists(sourceErrDirectory))
                                {
                                    DirectoryInfo diErrorSource = new DirectoryInfo(sourceErrDirectory);
                                    foreach (FileInfo file in diErrorSource.GetFiles())
                                    {
                                        if (!(file.Name.IndexOf("_Sales") > 0)) continue;

                                        using (System.IO.StreamReader reader = new System.IO.StreamReader(file.FullName))
                                        {
                                            try
                                            {
                                                string bufferPassword = reader.ReadToEnd();
                                                string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                                                RraSdcUploadModel tranModel = JsonConvert.DeserializeObject<RraSdcUploadModel>(buffer);
                                                if (string.IsNullOrEmpty(tranModel.ResultCd))
                                                {
                                                    if (WriteTransaction(targetDirectory + "/" + file.Name, tranModel))
                                                    {
                                                        reader.Close();
                                                        System.IO.File.Delete(file.FullName);
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                string aaa = e.Message;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string aaa = e.Message;
                    }
                }
            }
            catch (Exception e)
            {
                string aa = e.Message;
            }
        }

    }
}

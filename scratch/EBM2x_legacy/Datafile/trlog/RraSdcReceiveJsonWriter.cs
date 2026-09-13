using EBM2x.Models;
using EBM2x.RraSdc;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EBM2x.Datafile.trlog
{
    public class RraSdcReceiveJsonWriter : DatafileService
    {
        public static string GetPathName()
        {
            string tranDate = DateTime.Now.ToString("yyyyMMdd");
            return GetPathName("rrasdc/receive/" + tranDate);
        }
        //added by Aime
        public static string GetFiles(string path)
        {
            var file = new DirectoryInfo(path).GetFiles().OrderByDescending(o => o.LastWriteTime).First();
            return file.Name;
        }

        public static string GetFileName()
        {
            return GetFileName("rrasdc/requestDate", "requestDate.json");
        }

        public static string readFileDecrpt(string tranName)
        {
            string buffer = "";
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    string bufferPassword = reader.ReadToEnd();
                    buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                }
            }
            catch
            {
            }

            return buffer;
        }

        //end by Aime

        public static string GetFileName(string tranName)
        {
            string tranDate = DateTime.Now.ToString("yyyyMMdd");
            return GetFileName("rrasdc/receive/" + tranDate, tranName +".json");
        }
        public static bool DeleteFileAll(string tranDate)
        {
            return DeleteFiles("rrasdc/receive/" + tranDate);
        }
        public static string read(string tranName)
        {
            string buffer = "";
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(tranName)))
                {
                    string bufferPassword = reader.ReadToEnd();
                    buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                }
            }
            catch
            {
            }

            return buffer;
        }
        public static bool WriteTransactionREQ(string filetype, string jsonString)
        {
            string tranName = DateTime.Now.ToString("yyyyMMddHHmmssffff_") + "req_" + filetype;
            string filename = GetFileName(tranName);

            return WriteTransaction(filename, jsonString, true);
        }
        public static bool WriteTransactionSCC(string filetype, string jsonString)
        {

            string tranName = DateTime.Now.ToString("yyyyMMddHHmmssffff_") + "scc_" + filetype;
            string filename = GetFileName(tranName);

            return WriteTransaction(filename, jsonString, true);
        }
        public static bool WriteTransactionERR(string filetype, string jsonString)
        {
            string tranName = DateTime.Now.ToString("yyyyMMddHHmmssffff_") + "err_" + filetype;
            string filename = GetFileName(tranName);

            return WriteTransaction(filename, jsonString, true);
        }
        public static bool WriteTransaction(string filename, string jsonString, bool flag)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false))
                {
                    string jsonStringPassword = Common.AESEncrypt256(Common.AES256Password(), jsonString);
                    file.Write(jsonStringPassword);
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

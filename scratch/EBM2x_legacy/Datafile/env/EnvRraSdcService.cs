using EBM2x.Database;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace EBM2x.Datafile.env
{
    public class EnvRraSdcService : DatafileService
    {
        public static string GetFileName()
        {
            EBM2xMsSQLiteClientProvider provider = new EBM2xMsSQLiteClientProvider();
            return provider.GetDatabaseName("RRA_EBM2x.db.RraSdc");
        }

        public static bool IsEnvRraSdc()
        {
            InitInfoVO node = LoadEnvRraSdc();
            if(string.IsNullOrEmpty(node.tin)) return false;
            else return true;
        }

        public static void SaveEnvRraSdc(InitInfoVO node)
        {
            var jsonRequest = JsonConvert.SerializeObject(node, Formatting.Indented);
            jsonRequest = HttpUtility.UrlEncode(jsonRequest, Encoding.GetEncoding("UTF-8"));
            //jsonRequest = Common.AESEncrypt256(Common.AES256Password(), jsonRequest);
            byte[] bytesJsonRequest = Encoding.ASCII.GetBytes(jsonRequest);

            string size = string.Format("{0:00000000000000000000}", bytesJsonRequest.Length);          
            byte[] bytesSize = Encoding.ASCII.GetBytes(size);

            int ss = bytesSize.Length;

            using (FileStream fs = File.OpenWrite(GetFileName()))
            {
                fs.Seek(100, SeekOrigin.Begin);
                fs.Write(bytesSize, 0, bytesSize.Length);
                fs.Seek(200, SeekOrigin.Begin);
                fs.Write(bytesJsonRequest, 0, bytesJsonRequest.Length);
            }
        }

        public static InitInfoVO LoadEnvRraSdc()
        {
            InitInfoVO node = null;

            try
            {
                byte[] bytesJsonRequest = new byte[4096];
                byte[] bytesSize = new byte[1024];

                using (FileStream fs = File.OpenRead(GetFileName()))
                {
                    fs.Seek(100, SeekOrigin.Begin);
                    fs.Read(bytesSize, 0, 20);

                    string sizeString = Encoding.ASCII.GetString(bytesSize);
                    int offset = int.Parse(sizeString);

                    fs.Seek(200, SeekOrigin.Begin);
                    fs.Read(bytesJsonRequest, 0, offset);

                    string jsonString = Encoding.ASCII.GetString(bytesJsonRequest);
                    //jsonString = Common.AESDecrypte256(Common.AES256Password(), jsonString);
                    jsonString = HttpUtility.UrlDecode(jsonString, Encoding.GetEncoding("UTF-8"));
                    node = JsonConvert.DeserializeObject<InitInfoVO>(jsonString);
                }

                return node;
            }
            catch(Exception ex)
            {
                return new InitInfoVO();
            }
        }
    }
}

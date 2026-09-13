using EBM2x.Database;
using EBM2x.Models.config;
using EBM2x.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace EBM2x.Datafile.env
{
    public class EnvPosSetupService : DatafileService
    {
        public static string GetFileName()
        {
            EBM2xMsSQLiteClientProvider provider = new EBM2xMsSQLiteClientProvider();
            return provider.GetDatabaseName("RRA_EBM2x.db.RraSdc");
        }

        public static bool IsEnvPosSetup()
        {
            EnvPosSetup node = LoadEnvPosSetup();
            if (string.IsNullOrEmpty(node.GblTaxIdNo)) return false;
            else return true;
        }

        public static void SaveEnvPosSetup(EnvPosSetup node)
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
                fs.Seek(6100, SeekOrigin.Begin);
                fs.Write(bytesSize, 0, bytesSize.Length);
                fs.Seek(6200, SeekOrigin.Begin);
                fs.Write(bytesJsonRequest, 0, bytesJsonRequest.Length);
            }
        }

        public static EnvPosSetup LoadEnvPosSetup()
        {
            EnvPosSetup node = null;

            try
            {
                byte[] bytesJsonRequest = new byte[4096];
                byte[] bytesSize = new byte[1024];

                using (FileStream fs = File.OpenRead(GetFileName()))
                {
                    fs.Seek(6100, SeekOrigin.Begin);
                    fs.Read(bytesSize, 0, 20);

                    string sizeString = Encoding.ASCII.GetString(bytesSize);
                    int offset = int.Parse(sizeString);

                    fs.Seek(6200, SeekOrigin.Begin);
                    fs.Read(bytesJsonRequest, 0, offset);

                    string jsonString = Encoding.ASCII.GetString(bytesJsonRequest);
                    //jsonString = Common.AESDecrypte256(Common.AES256Password(), jsonString);
                    jsonString = HttpUtility.UrlDecode(jsonString, Encoding.GetEncoding("UTF-8"));
                    node = JsonConvert.DeserializeObject<EnvPosSetup>(jsonString);
                }

                // 2020/05/20 : 르와다 요청으로 OfflineDays를 1일 에서 7일로 변경 
                if (string.IsNullOrEmpty(node.UpdateFlagOfflineDays) && !node.UpdateFlagOfflineDays.Equals("YES"))
                {
                    node.UpdateFlagOfflineDays = "YES";
                    node.OfflineDays = 3;
                }

                return node;
            }
            catch(Exception ex)
            {
                return new EnvPosSetup();
            }
        }
    }
}

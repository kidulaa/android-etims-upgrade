using EBM2x.Models.config;
using EBM2x.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EBM2x.Services
{
    public class OperatorService : JsonService
    {
        public static string GetFileName(string operatorCode)
        {
            return GetFileName("master/operator", operatorCode + ".json");
        }

        public static string GetPathName()
        {
            return GetPathName("master/operator","");
        }

        public static void Save(OperatorRecord record)
        {
            // 암호화 처리
            string oldPassword = record.Password;
            string newPassword = "{{#}}" + Common.AESEncrypt256(Common.AES256Password(), record.Password);
            record.Password = newPassword;

            var jsonRequest = JsonConvert.SerializeObject(record, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(record.OperatorCode), false))
            {
                file.Write(jsonRequest);
            }

            record.Password = oldPassword;
        }

        public static OperatorRecord Load(string operatorCode)
        {
            OperatorRecord record = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(operatorCode)))
                {
                    record = JsonConvert.DeserializeObject<OperatorRecord>(reader.ReadToEnd());

                    if (!string.IsNullOrEmpty(record.Password) && record.Password.Length > 5 && record.Password.Substring(0, 5).Equals("{{#}}"))
                    {
                        string password = record.Password.Substring(5);
                        record.Password = Common.AESDecrypt256(Common.AES256Password(), password);
                    }
                }
            }
            catch
            {
                record = new OperatorRecord();
            }

            return record;
        }

        public static List<OperatorRecord> GetOperatorList()
        {
            List<OperatorRecord> list = new List<OperatorRecord>();

            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        OperatorRecord record = JsonConvert.DeserializeObject<OperatorRecord>(reader.ReadToEnd());
                    
                        if (!string.IsNullOrEmpty(record.Password) && record.Password.Length > 5 && record.Password.Substring(0, 5).Equals("{{#}}"))
                        {
                            string password = record.Password.Substring(5);
                            record.Password = Common.AESDecrypt256(Common.AES256Password(), password);
                        }

                        list.Add(record);
                    }
                }
            }
            catch
            {
            }

            return list;
        }
    } 
}

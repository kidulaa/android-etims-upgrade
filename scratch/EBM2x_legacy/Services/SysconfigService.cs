using EBM2x.Database.Master;
using EBM2x.Models.config;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EBM2x.Services
{
    public class SysconfigService : JsonService
    {
        public static string GetFileName()
        {
            return GetFileName("master/sysconfig", "Sysconfig.json");
        }

        public static void Save(List<SysconfigRecord> records)
        {
            var jsonRequest = JsonConvert.SerializeObject(records, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }

        public static List<SysconfigRecord> Load()
        {
            List<SysconfigRecord> records = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    records = JsonConvert.DeserializeObject<List<SysconfigRecord>>(reader.ReadToEnd());
                }
            }
            catch
            {
                records = new List<SysconfigRecord>();
            }

            return records;
        }
    } 
}

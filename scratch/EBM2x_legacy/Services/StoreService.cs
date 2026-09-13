using EBM2x.Models.config;
using Newtonsoft.Json;

namespace EBM2x.Services
{
    public class StoreService : JsonService
    {
        public static string GetFileName(string storeCode)
        {
            return GetFileName("master/store", storeCode + ".json");
        }

        public static void Save(StoreRecord record)
        {
            var jsonRequest = JsonConvert.SerializeObject(record, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(record.StoreCode), false))
            {
                file.Write(jsonRequest);
            }
        }

        public static StoreRecord Load(string storeCode)
        {
            StoreRecord record = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(storeCode)))
                {
                    record = JsonConvert.DeserializeObject<StoreRecord>(reader.ReadToEnd());
                }
            }
            catch
            {
                record = new StoreRecord();
            }

            return record;
        }
    } 
}

using EBM2x.Models.Preset;
using Newtonsoft.Json;

namespace EBM2x.Services
{
    public class RequestResponService : JsonService
    {
        public static string GetFileName()
        {
            return GetFileName("RraSdc", "RequestRespon.json");
        }

        public static void SaveList(PresetGroupList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }

        public static PresetGroupList LoadList()
        {
            PresetGroupList list = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    list = JsonConvert.DeserializeObject<PresetGroupList>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return list;
        }
    } 
}

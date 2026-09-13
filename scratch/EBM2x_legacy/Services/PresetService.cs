using EBM2x.Models.Preset;
using Newtonsoft.Json;
using System;

namespace EBM2x.Services
{
    public class PresetService : JsonService
    {
        public static string GetFileName()
        {
            return GetFileName("master", "Preset.json");
        }
        public static string GetHistoryFileName()
        {
            string dt = DateTime.Now.ToString("yyyyMMdd");
            return GetFileName("master/preset", "Preset_" + dt + ".json");
        }

        public static void SaveList(PresetGroupList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }
        public static void SaveHistoryList(PresetGroupList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetHistoryFileName(), false))
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

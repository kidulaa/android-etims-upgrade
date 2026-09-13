using EBM2x.Models.DiningTable;
using Newtonsoft.Json;
using System;

namespace EBM2x.Services
{
    public class DiningTableService : JsonService
    {
        public static string GetFileName()
        {
            return GetFileName("master", "DiningTable.json");
        }
        public static string GetHistoryFileName()
        {
            string dt = DateTime.Now.ToString("yyyyMMdd");
            return GetFileName("master/restaurant", "DiningTable_" + dt + ".json");
        }

        public static void SaveList(DiningRoomList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }
        public static void SaveHistoryList(DiningRoomList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetHistoryFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }


        public static DiningRoomList LoadList()
        {
            DiningRoomList list = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    list = JsonConvert.DeserializeObject<DiningRoomList>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return list;
        }
    } 
}

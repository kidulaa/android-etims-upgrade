using EBM2x.Models.HotelRoom;
using Newtonsoft.Json;
using System;

namespace EBM2x.Services
{
    public class HotelRoomService : JsonService
    {
        public static string GetFileName()
        {
            return GetFileName("master", "HotelRoom.json");
        }
        public static string GetHistoryFileName()
        {
            string dt = DateTime.Now.ToString("yyyyMMdd");
            return GetFileName("master/hotel", "HotelRoom_" + dt + ".json");
        }


        public static void SaveList(HotelFloorList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }
        public static void SaveHistoryList(HotelFloorList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetHistoryFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }


        public static HotelFloorList LoadList()
        {
            HotelFloorList list = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    list = JsonConvert.DeserializeObject<HotelFloorList>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return list;
        }
    } 
}

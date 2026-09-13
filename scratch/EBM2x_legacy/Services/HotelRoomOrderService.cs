using EBM2x.Models.tran;
using Newtonsoft.Json;

namespace EBM2x.Services
{
    public class HotelRoomOrderService : JsonService
    {
        public static string GetFileName(string hotelRoomCode)
        {
            return GetFileName("HotelRoomOrder", hotelRoomCode + ".json");
        }

        public static void Move(string hotelRoomCodeFrom, string hotelRoomCodeTo)
        {
            System.IO.File.Delete(GetFileName(hotelRoomCodeTo));
            System.IO.File.Move(GetFileName(hotelRoomCodeFrom), GetFileName(hotelRoomCodeTo));
        }

        public static void Delete(string hotelRoomCode)
        {
            System.IO.File.Delete(GetFileName(hotelRoomCode));
        }
        public static void Save(string hotelRoomCode, TranNode tranNode)
        {
            var jsonRequest = JsonConvert.SerializeObject(tranNode, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(hotelRoomCode), false))
            {
                file.Write(jsonRequest);
            }
        }

        public static TranNode Load(string hotelRoomCode)
        {
            TranNode tranNode = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(hotelRoomCode)))
                {
                    tranNode = JsonConvert.DeserializeObject<TranNode>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return tranNode;
        }
    } 
}

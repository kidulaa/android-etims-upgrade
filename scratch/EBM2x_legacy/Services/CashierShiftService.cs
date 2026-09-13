using EBM2x.Models.ReadyMoney;
using Newtonsoft.Json;

namespace EBM2x.Services
{
    public class CashierShiftService : JsonService
    {
        public static string GetFileName()
        {
            return GetFileName("tran/fund", "CashierShift.json");
        }

        public static void SaveList(ReadyMoneyList list)
        {
            var jsonRequest = JsonConvert.SerializeObject(list, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                file.Write(jsonRequest);
            }
        }

        public static ReadyMoneyList LoadList()
        {
            ReadyMoneyList list = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    list = JsonConvert.DeserializeObject<ReadyMoneyList>(reader.ReadToEnd());
                }
            }
            catch
            {
            }

            return list;
        }
    }
}

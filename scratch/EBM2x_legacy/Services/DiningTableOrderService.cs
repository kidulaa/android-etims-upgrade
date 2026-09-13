using EBM2x.Models.tran;
using Newtonsoft.Json;

namespace EBM2x.Services
{
    public class DiningTableOrderService : JsonService
    {
        public static string GetFileName(string diningTableCode)
        {
            return GetFileName("DiningTableOrder", diningTableCode + ".json");
        }

        public static void Move(string diningTableCodeFrom, string diningTableCodeTo)
        {
            System.IO.File.Delete(GetFileName(diningTableCodeTo));
            System.IO.File.Move(GetFileName(diningTableCodeFrom), GetFileName(diningTableCodeTo));
        }
        public static void Copy(string diningTableCodeFrom, string diningTableCodeTo)
        {
            System.IO.File.Copy(GetFileName(diningTableCodeFrom), GetFileName(diningTableCodeTo));
        }

        public static void Delete(string diningTableCode)
        {
            System.IO.File.Delete(GetFileName(diningTableCode));
        }
        public static void Save(string diningTableCode, TranNode tranNode)
        {
            var jsonRequest = JsonConvert.SerializeObject(tranNode, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(diningTableCode), false))
            {
                file.Write(jsonRequest);
            }
        }

        public static TranNode Load(string diningTableCode)
        {
            TranNode tranNode = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(diningTableCode)))
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

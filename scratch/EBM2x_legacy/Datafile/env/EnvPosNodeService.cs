using EBM2x.Models.config;
using EBM2x.Utils;
using Newtonsoft.Json;

namespace EBM2x.Datafile.env
{
    public class EnvPosNodeService : DatafileService
    {
        public static string GetFileName()
        {
            return GetFileName("environment", "EnvPosNode.json");
        }

        public static bool IsEnvPosNode()
        {
            return IsFileExist("environment", "EnvPosNode.json");
        }

        public static void SaveEnvPosNode(EnvPosNode node)
        {
            var jsonRequest = JsonConvert.SerializeObject(node, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                string jsonRequestPassword = Common.AESEncrypt256(Common.AES256Password(), jsonRequest);
                file.Write(jsonRequestPassword);
            }
        }

        public static EnvPosNode LoadEnvPosNode()
        {
            EnvPosNode node = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                    node = JsonConvert.DeserializeObject<EnvPosNode>(buffer);
                }
                return node;
            }
            catch
            {
                return new EnvPosNode();
            }
        }
    }
}

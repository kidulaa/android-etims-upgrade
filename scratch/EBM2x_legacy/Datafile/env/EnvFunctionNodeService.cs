using EBM2x.Models.config;
using EBM2x.Utils;
using Newtonsoft.Json;

namespace EBM2x.Datafile.env
{
    public class EnvFunctionNodeService : DatafileService
    {
        public static string GetFileName()
        {
            return GetFileName("environment", "EnvFunctionNode.json");
        }

        public static bool IsEnvPosSetup()
        {
            return IsFileExist("environment", "EnvFunctionNode.json");
        }

        public static void SaveEnvFunctionNode(EnvFunctionNode node)
        {
            var jsonRequest = JsonConvert.SerializeObject(node, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                string jsonRequestPassword = Common.AESEncrypt256(Common.AES256Password(), jsonRequest);
                file.Write(jsonRequestPassword);
            }
        }

        public static EnvFunctionNode LoadEnvFunctionNode()
        {
            EnvFunctionNode node = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                    node = JsonConvert.DeserializeObject<EnvFunctionNode>(buffer);
                }
                return node;
            }
            catch
            {
                return new EnvFunctionNode();
            }
        }
    }
}

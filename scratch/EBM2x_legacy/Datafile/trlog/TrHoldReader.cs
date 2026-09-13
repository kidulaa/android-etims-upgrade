using EBM2x.Models.hold;
using EBM2x.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EBM2x.Datafile.trlog
{
    public class TrHoldReader : DatafileService
    {
        public static string GetPathName()
        {
            return GetPathName("tran/hold"); ;
        }
        public static string GetFileName(string tranName)
        {
            return GetFileName("tran/hold", tranName + ".json");
        }
        public static bool IsHoldTranNode(string tranName)
        {
            return IsFileExist("tran/hold", tranName + ".json");
        }
        public static HoldNode read(string tranName)
        {
            HoldNode holdNode = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName(tranName)))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                    holdNode = JsonConvert.DeserializeObject<HoldNode>(buffer);
                }
            }
            catch
            {
            }

            return holdNode;
        }
        public static List<HoldNode> GetHoldingList()
        {
            List<HoldNode> list = new List<HoldNode>();
            
            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    string filename = file;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
                    {
                        string bufferPassword = reader.ReadToEnd();
                        string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                        HoldNode holdNode = JsonConvert.DeserializeObject<HoldNode>(buffer);
                        list.Add(holdNode); 
                    }
                }
            }
            catch
            {
            }

            return list;
        }
        public static int GetCount()
        {
            int count = 0;
            try
            {
                string[] filenames = System.IO.Directory.GetFiles(GetPathName());
                foreach (string file in filenames)
                {
                    count = count + 1;
                }
            }
            catch
            {
            }

            return count;
        }
    }
}

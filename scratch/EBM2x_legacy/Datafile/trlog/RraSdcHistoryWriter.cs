using EBM2x.RraSdc;
using EBM2x.Utils;
using Newtonsoft.Json;

namespace EBM2x.Datafile.trlog
{
    public class RraSdcHistoryWriter : DatafileService
    {
        public static string GetFileName()
        {
            return GetFileName("rrasdc/history", "RraSdcDownload.json");
        }
        public static void WriteTransaction(RequestResponList requestResponList)
        {
            var jsonRequest = JsonConvert.SerializeObject(requestResponList, Formatting.Indented);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFileName(), false))
            {
                string jsonRequestPassword = Common.AESEncrypt256(Common.AES256Password(), jsonRequest);
                file.Write(jsonRequestPassword);
            }
        }
        public static RequestResponList ReadTransaction()
        {
            RequestResponList requestResponList = null;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFileName()))
                {
                    string bufferPassword = reader.ReadToEnd();
                    string buffer = Common.AESDecrypt256(Common.AES256Password(), bufferPassword);
                    requestResponList = JsonConvert.DeserializeObject<RequestResponList>(buffer);

                    for(int i = 0; i < requestResponList.NodeList.Count; i++)
                    {
                        if (requestResponList.NodeList[i].LastDate.Equals("20150101000000") && !requestResponList.NodeList[i].ProcessName.Equals(RraSdcService.URL_CODE_SEARCH))
                        {
                            requestResponList.NodeList[i].LastDate = "20200218000000";
                        }
                    }                    
                }
            }
            catch
            {
            }

            return requestResponList;
        }
    }
}

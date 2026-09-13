using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.MobileWallet
{
    public class MobileWalletProcess
    {
        public async void GetMobileWalletProcess(DebitReqRecord debitReqRecord)
        {
            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    string jsonRequest = JsonConvert.SerializeObject(debitReqRecord);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    string url = "" + "/" + "";
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    DebitResRecord debitResRecord = JsonConvert.DeserializeObject<DebitResRecord>(jsonResponse);
                    //if (debitResRecord..Msgid.Equals("000")) {
                    //}
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}

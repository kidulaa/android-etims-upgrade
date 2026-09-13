using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Xamarin.Essentials;

namespace EBM2x.RraSdc.process
{
    public class TaxpayerInfoProcess
    {
        public void TaxpayerInfoDownloadProcess()
        {
            using (HttpClient client = new HttpClient())
            {              
                try
                {
                   //'TIN'  'BHFID'RequestHeader,  'cmcKey'.
                    RraSdcService.SetDefaultRequestHeaders(client);

                    TaxpayerInfoReq taxpayerInfoReq = RraSdcService.GetTaxpayerInfoRequestHeaders();
                    // POST 
                    NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                    outgoingQueryString.Add("tin", taxpayerInfoReq.tin);
                    outgoingQueryString.Add("bhfId", taxpayerInfoReq.bhfId);
                    outgoingQueryString.Add("lastReqDt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    string postData = outgoingQueryString.ToString();

                    ASCIIEncoding ascii = new ASCIIEncoding();
                    string aaa = postData.ToString();
                    byte[] sendByte = ascii.GetBytes(postData.ToString());

                    string url = RraSdcService.EXTERNAL_URL + "/" + "selectTaxpayerInfo";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "POST"; //POST
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    httpWebRequest.ContentLength = sendByte.Length;

                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(sendByte, 0, sendByte.Length);
                    requestStream.Flush();
                    requestStream.Close();

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(responseStream, Encoding.UTF8);

                    string jsonResponse = readerPost.ReadToEnd().Trim();
                    //HttpResponseMessage response = await client.PostAsync(url, content);
                    //string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if(string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        TaxpayerInfoRes taxpayerInfoRes = JsonConvert.DeserializeObject<TaxpayerInfoRes>(jsonResponse);
                        if (taxpayerInfoRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC("selectTaxpayerInfo", jsonResponse);
                            TaxpayerInfo taxpayerInfo = taxpayerInfoRes.data.info;

                        }
                        else if (taxpayerInfoRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR("selectTaxpayerInfo", jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}

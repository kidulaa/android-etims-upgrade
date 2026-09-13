using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class BhfProcess
    {
        public async void BhfDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_BHF_SEARCH);

            BhfReq bhfReq = new BhfReq
            {
                lastReqDt = requestResponNode.LastDate    
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    
                    string jsonRequest = JsonConvert.SerializeObject(bhfReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_BHF_SEARCH;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if(string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        BhfRes bhfRes = JsonConvert.DeserializeObject<BhfRes>(jsonResponse);
                        if (bhfRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_BHF_SEARCH, jsonResponse);

                            int processCount = UpdateTable(bhfRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = bhfRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (bhfRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_BHF_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(BhfRes bhfRes)
        {
            int processCount = 0;
            TaxpayerBhfMaster taxpayerBhfMaster = new TaxpayerBhfMaster();

            TaxpayerBhfRecord record2 = new TaxpayerBhfRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            BhfResData bhfData = bhfRes.data;
            for (int i = 0; i < bhfData.bhfList.Count; i++)
            {
                Bhf bhf = bhfData.bhfList[i];
                // Save bhf
                taxpayerBhfMaster.ToTableSDC(bhf, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

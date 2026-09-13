using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class TrnsPurchaseProcess
    {
        public async void TrnsPurchaseDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_TRNS_PURCHASE_SEARCH);

            TrnsPurchaseReq trnsPurchaseReq = new TrnsPurchaseReq
            {
                lastReqDt = requestResponNode.LastDate     
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_TRNS_PURCHASE_SEARCH;
                    // 'TIN' 'BHFID' RequestHeader,  'cmcKey'.
                    RraSdcService.SetDefaultRequestHeaders(client);
                    string jsonRequest = JsonConvert.SerializeObject(trnsPurchaseReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        TrnsPurchaseRes trnsPurchaseRes = JsonConvert.DeserializeObject<TrnsPurchaseRes>(jsonResponse);
                        if (trnsPurchaseRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_TRNS_PURCHASE_SEARCH, jsonResponse);

                            int processCount = UpdateTable(trnsPurchaseRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = trnsPurchaseRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (trnsPurchaseRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_TRNS_PURCHASE_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            //return rraSdcResult;
        }
        public int UpdateTable(TrnsPurchaseRes trnsPurchaseRes)
        {
            int processCount = 0;
            TrnsPurchaseMaster trnsPurchaseMaster = new TrnsPurchaseMaster();

            TrnsPurchaseRecord record2 = new TrnsPurchaseRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            TrnsPurchaseResData trnsPurchaseResData = trnsPurchaseRes.data;
            for (int i = 0; i < trnsPurchaseResData.pchsList.Count; i++)
            {
                TrnsPurchase trnsPurchase = trnsPurchaseResData.pchsList[i];
                //// Save bhf
                //trnsPurchaseMaster.ToTableSDC(trnsPurchase, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class TrnsSalesReceiptProcess
    {
        public async void TrnsSalesReceiptDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_TRNS_SALES_SEARCH);

            TrnsSalesReq trnsSalesReq = new TrnsSalesReq
            {
                lastReqDt = requestResponNode.LastDate   
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_TRNS_SALES_SEARCH;
                    //  'TIN' 'BHFID' RequestHeader,  'cmcKey'.
                    RraSdcService.SetDefaultRequestHeaders(client);

                    string jsonRequest = JsonConvert.SerializeObject(trnsSalesReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        TrnsSalesRes trnsSalesRes = JsonConvert.DeserializeObject<TrnsSalesRes>(jsonResponse);
                        if (trnsSalesRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_TRNS_SALES_SEARCH, jsonResponse);

                            int processCount = UpdateTable(trnsSalesRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = trnsSalesRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (trnsSalesRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_TRNS_SALES_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public int UpdateTable(TrnsSalesRes trnsSalesRes)
        {
            int processCount = 0;
            TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();

            TrnsSaleRecord record2 = new TrnsSaleRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            TrnsSalesResData trnsSalesResData = trnsSalesRes.data;
            for (int i = 0; i < trnsSalesResData.saleList.Count; i++)
            {
                TrnsSales trnsSales = trnsSalesResData.saleList[i];
                //// Save bhf
                //trnsSaleMaster.ToTableSDC(trnsSalesRes, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

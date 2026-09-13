using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class StockMasterProcess
    {
        public async void StockMasterDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_STOCK_MASTER_SEARCH);

            StockMasterReq stockMasterReq = new StockMasterReq
            {
                lastReqDt = requestResponNode.LastDate   
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    string jsonRequest = JsonConvert.SerializeObject(stockMasterReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_STOCK_MASTER_SEARCH;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        StockMasterRes stockMasterRes = JsonConvert.DeserializeObject<StockMasterRes>(jsonResponse);
                        if (stockMasterRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_STOCK_MASTER_SEARCH, jsonResponse);

                            int processCount = UpdateTable(stockMasterRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = stockMasterRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (stockMasterRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_STOCK_MASTER_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(StockMasterRes stockMasterRes)
        {
            int processCount = 0;
            StockMasterMaster stockMasterMaster = new StockMasterMaster();

            StockMasterRecord record2 = new StockMasterRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            StockMasterData stockMasterData = stockMasterRes.data;
            for (int i = 0; i < stockMasterData.itemList.Count; i++)
            {
                StockMaster stockMaster = stockMasterData.itemList[i];
                //// Save bhf
                //stockMasterMaster.ToTableSDC(notice, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

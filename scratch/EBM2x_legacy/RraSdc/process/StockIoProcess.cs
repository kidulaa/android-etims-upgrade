using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class StockIoProcess
    {
        public async void StockIoDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_STOCK_IO_SEARCH);

            StockIoReq stockIoReq = new StockIoReq
            {
                lastReqDt = requestResponNode.LastDate    
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_STOCK_IO_SEARCH;
                    RraSdcService.SetDefaultRequestHeaders(client);

                    string jsonRequest = JsonConvert.SerializeObject(stockIoReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        StockIoRes stockIoRes = JsonConvert.DeserializeObject<StockIoRes>(jsonResponse);
                        if (stockIoRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_STOCK_IO_SEARCH, jsonResponse);

                            int processCount = UpdateTable(stockIoRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = stockIoRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (stockIoRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_STOCK_IO_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public int UpdateTable(StockIoRes stockIoRes)
        {
            int processCount = 0;
            StockIoMaster stockIoMaster = new StockIoMaster();

            StockIoRecord record2 = new StockIoRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            StockIoResData stockIoResData = stockIoRes.data;
            for (int i = 0; i < stockIoResData.stockList.Count; i++)
            {
                StockIo stockIo = stockIoResData.stockList[i];
                //// Save bhf
                //stockIoMaster.ToTableSDC(notice, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

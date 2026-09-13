using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class ClassificationProcess
    {
        public async void ClassificationDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_ITEM_CLASS_SEARCH);

            ItemClsReq itemClsReq = new ItemClsReq
            {
                lastReqDt = requestResponNode.LastDate   
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    string jsonRequest = JsonConvert.SerializeObject(itemClsReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_ITEM_CLASS_SEARCH;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    //string jsonResponse = RraSdcReceiveJsonWriter.read("201912270318156862_scc_selectItemClsList");

                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        ItemClsRes itemClsRes = JsonConvert.DeserializeObject<ItemClsRes>(jsonResponse);
                        if (itemClsRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_ITEM_CLASS_SEARCH, jsonResponse);

                            int processCount = UpdateTable(itemClsRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = itemClsRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (itemClsRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_ITEM_CLASS_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
        }

        public int UpdateTable(ItemClsRes itemClsRes)
        {
            int processCount = 0;
            ItemClassMaster itemClassMaster = new ItemClassMaster();

            ItemClassRecord record2 = new ItemClassRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            ItemClsResData itemClsResData = itemClsRes.data;
            for (int i = 0; i < itemClsResData.itemClsList.Count; i++)
            {
                ItemClassLVO itemClass = itemClsResData.itemClsList[i];
                // Save
                itemClassMaster.ToTableSDC(itemClass, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

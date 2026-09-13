using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class NoticeProcess
    {
        public async void NoticeDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_NOTICE_SEARCH);

            NoticeReq noticeReq = new NoticeReq
            {
                lastReqDt = requestResponNode.LastDate     
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    
                    string jsonRequest = JsonConvert.SerializeObject(noticeReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_NOTICE_SEARCH;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        NoticeRes noticeRes = JsonConvert.DeserializeObject<NoticeRes>(jsonResponse);
                        if (noticeRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_NOTICE_SEARCH, jsonResponse);

                            int processCount = UpdateTable(noticeRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = noticeRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (noticeRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_NOTICE_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(NoticeRes noticeRes)
        {
            int processCount = 0;
            BbsNoticeMaster bbsNoticeMaster = new BbsNoticeMaster();

            BbsNoticeRecord record2 = new BbsNoticeRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            NoticeResData noticeResData = noticeRes.data;
            for (int i = 0; i < noticeResData.noticeList.Count; i++)
            {
                Notice notice = noticeResData.noticeList[i];
                // Save
                bbsNoticeMaster.ToTableSDC(notice, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

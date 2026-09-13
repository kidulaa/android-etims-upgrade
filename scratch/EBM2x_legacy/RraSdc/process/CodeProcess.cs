using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class CodeProcess
    {
        public async void CodeDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_CODE_SEARCH);

            CodeReq codeReq = new CodeReq
            {
                lastReqDt = requestResponNode.LastDate    
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    
                    string jsonRequest = JsonConvert.SerializeObject(codeReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_CODE_SEARCH;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        CodeRes codeRes = JsonConvert.DeserializeObject<CodeRes>(jsonResponse);
                        if (codeRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_CODE_SEARCH, jsonResponse);

                            int processCount = UpdateTable(codeRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = codeRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (codeRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_CODE_SEARCH, jsonResponse);

                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(CodeRes codeRes)
        {
            int processCount = 0;
            CodeClassMaster codeClassMaster = new CodeClassMaster();
            CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
            CodeClassRecord record2 = new CodeClassRecord();
            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            CodeResData codeData = codeRes.data;
            for (int i = 0; i < codeData.clsList.Count; i++)
            {
                CodeClassLVO codeCls = codeData.clsList[i];
                // Save CodeCls
                codeClassMaster.ToTableSDC(codeCls, record2);
                for (int j = 0; j < codeCls.dtlList.Count; j++)
                {
                    CodeDtlLVO code = codeCls.dtlList[j];
                    // Save Code
                    codeDtlMaster.ToTableSDC(code, codeCls, record2);
                    processCount++;
                }
                processCount++;
            }

            return processCount;
        }
    }
}

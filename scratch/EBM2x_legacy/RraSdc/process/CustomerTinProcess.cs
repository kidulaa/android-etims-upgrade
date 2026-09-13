using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class CustomerTinProcess
    {
        public async void CustomerTinDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_CUSTOMER_SEARCH);

            CustomerTinReq customerTinReq = new CustomerTinReq
            {
                lastReqDt = requestResponNode.LastDate  
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    string jsonRequest = JsonConvert.SerializeObject(customerTinReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_CUSTOMER_SEARCH;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if(string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        CustomerTinRes customerTinRes = JsonConvert.DeserializeObject<CustomerTinRes>(jsonResponse);
                        if (customerTinRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_CUSTOMER_SEARCH, jsonResponse);

                            int processCount = UpdateTable(customerTinRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = customerTinRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (customerTinRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_CUSTOMER_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(CustomerTinRes customerTinRes)
        {
            int processCount = 0;
            TaxpayerBaseMaster taxpayerBaseMaster = new TaxpayerBaseMaster();

            TaxpayerBaseRecord record2 = new TaxpayerBaseRecord();

            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            CustomerTinResData customerTinData = customerTinRes.data;
            for (int i = 0; i < customerTinData.custList.Count; i++)
            {
                CustomerTin customerTin = customerTinData.custList[i];
                // Save taxpayer
                taxpayerBaseMaster.ToTableSDC(customerTin, record2);
                processCount++;
            }

            return processCount;
        }
    }
}

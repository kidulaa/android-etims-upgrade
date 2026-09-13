using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using EBM2x.UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class ImportItemProcess
    {
        public async void ImportItemDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_IMPORT_ITEM_SEARCH);

            //read file from json file by Aime Irak 19/04/2022 11:00 PM
            string dataJsonFile = RraSdcReceiveJsonWriter.readFileDecrpt(RraSdcReceiveJsonWriter.GetFileName());
            TrnsPurchaseSalesRes trnsPurchaseSalesJson = new TrnsPurchaseSalesRes();
            if (RraSdcService.IsValidJson(dataJsonFile))
            {
                trnsPurchaseSalesJson = JsonConvert.DeserializeObject<TrnsPurchaseSalesRes>(dataJsonFile);
            }
            else
            {
                trnsPurchaseSalesJson.resultDt = requestResponNode.LastDate;
            }

            //TrnsPurchaseSalesRes trnsPurchaseSalesJson = JsonConvert.DeserializeObject<TrnsPurchaseSalesRes>(dataJsonFile);
            string lastDtRqst = null;
            if (trnsPurchaseSalesJson != null)
            {
                lastDtRqst = trnsPurchaseSalesJson.resultDt;
            }
            else
            {

                lastDtRqst = requestResponNode.LastDate;
            }

            ImportItemReq importItemReq = new ImportItemReq
            {
                //lastReqDt = requestResponNode.LastDate    
                lastReqDt = lastDtRqst                      // get last date request 
            };
            //END read json file by Aime Irak

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_IMPORT_ITEM_SEARCH;
                    RraSdcService.SetDefaultRequestHeaders(client);

                    string jsonRequest = JsonConvert.SerializeObject(importItemReq);
                    RraSdcReceiveJsonWriter.WriteTransactionREQ(RraSdcService.URL_IMPORT_ITEM_SEARCH, jsonRequest);

                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("RESPONSE >>>>>>>> " + jsonResponse);
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    //RraSdcReceiveJsonWriter.WriteTransaction(RraSdcReceiveJsonWriter.GetFileName(), jsonResponse, true);
                    //create file  with json data  by Aime  Irak 20/04/2022 05:00 AM
                    if (File.Exists(RraSdcReceiveJsonWriter.GetFileName()))
                    {
                        File.Delete(RraSdcReceiveJsonWriter.GetFileName());
                        RraSdcReceiveJsonWriter.WriteTransaction(RraSdcReceiveJsonWriter.GetFileName(), jsonResponse, true);
                    }
                    else if (!File.Exists(RraSdcReceiveJsonWriter.GetFileName()))
                    {
                        RraSdcReceiveJsonWriter.WriteTransaction(RraSdcReceiveJsonWriter.GetFileName(), jsonResponse, true);
                    }
                    //end Create File

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        ImportItemRes importItemRes = JsonConvert.DeserializeObject<ImportItemRes>(jsonResponse);
                        if (importItemRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_IMPORT_ITEM_SEARCH, jsonResponse);

                            int processCount = UpdateTable(importItemRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = importItemRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (importItemRes.resultCd.Equals("001"))
                        {
                            
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_IMPORT_ITEM_SEARCH, jsonResponse);

                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(ImportItemRes importItemRes)
        {
            int processCount = 0;

            ImportItemMaster importItemMaster = new ImportItemMaster();

            ImportItemRecord record2 = new ImportItemRecord();

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            //string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;

            record2.Tin = Tin;
            record2.TaxprNm = "";
            record2.ItemCd = "";
            record2.ItemClsCd = "";
            record2.DclTaxofcCd = "";
            record2.TrffAmt = 0;
            record2.VatAmt = 0;
            record2.Remark = "";
            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            ImportItemResData itemData = importItemRes.data;
            for (int i = 0; i < itemData.itemList.Count; i++)
            {
                ImportItem importItem = itemData.itemList[i];
                importItemMaster.ToTableSDC(importItem, record2);

                processCount++;
            }

            return processCount;
        }
    }
}


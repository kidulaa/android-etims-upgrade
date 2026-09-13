using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using EBM2x.UI;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class ItemProcess
    {
        public async void ItemDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_ITEM_SEARCH);

            ItemReq itemReq = new ItemReq
            {
                lastReqDt = requestResponNode.LastDate    
            };

            using (HttpClient client = new HttpClient())
            {              
                try
                {
                    
                    string jsonRequest = JsonConvert.SerializeObject(itemReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    RraSdcService.SetDefaultRequestHeaders(client);

                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_ITEM_SEARCH;
                    Console.WriteLine("item url mfite niyingiyi" + url);
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    //jsonResponse = RraSdcReceiveJsonWriter.read("201912130703450273_scc_selectItemList");

                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        ItemRes itemRes = JsonConvert.DeserializeObject<ItemRes>(jsonResponse);
                        if (itemRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_ITEM_SEARCH, jsonResponse);

                            int processCount = UpdateTable(itemRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = itemRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (itemRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_ITEM_SEARCH, jsonResponse);

                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int UpdateTable(ItemRes itemRes)
        {
            int processCount = 0;
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();

            TaxpayerItemRecord record = new TaxpayerItemRecord();
            TaxpayerItemRecord record2 = new TaxpayerItemRecord();

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;

            record2.Tin = Tin;
            record2.RegBhfId = BhfId;
            record2.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.RegrId = "";
            record2.RegrNm = "";
            record2.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            record2.ModrId = "";
            record2.ModrNm = "";

            ItemResData itemData = itemRes.data;
            for (int i = 0; i < itemData.itemList.Count; i++)
            {
                Item item = itemData.itemList[i];

                bool IsExist = taxpayerItemMaster.GetItemCode(item.tin, item.itemCd);
                // Save bhf
                bool ret = taxpayerItemMaster.ToTableSDC(item, record2);
                if(ret)
                {
                    //============ 20191210 JCNA
                    if (!IsExist)
                    {
                        taxpayerItemMaster.ToRecord(record, item.tin, item.itemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);

                        //  STOCK_MASTER
                        StockMasterRecord stockRecord = new StockMasterRecord();
                        stockRecord.Tin = record.Tin;
                        stockRecord.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                        stockRecord.ItemCd = record.ItemCd;
                        stockRecord.RsdQty = 0;
                        stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                        stockRecord.ModrId = "";
                        stockRecord.ModrNm = "";
                        stockRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                        stockRecord.RegrId = "";
                        stockRecord.RegrNm = "";

                        //JCNA 20191204
                        StockMasterMaster stockMasterMaster = new StockMasterMaster();
                        stockMasterMaster.InsertTable(stockRecord);

                        //JCNA 20191204
                        StockMasterRraSdcUpload stockMasterRraSdcUpload = new StockMasterRraSdcUpload();
                        stockMasterRraSdcUpload.SendStockMasterSave(stockRecord.Tin, stockRecord.BhfId, stockRecord.ItemCd);
                    }

                    //===>>>>>>>>>
                    //JCNA 20191204
                    ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
                    itemRraSdcUpload.SendItemSave(record.Tin, record.ItemCd);

                    //===>>>>>>>>>
                    // JCNA 20191204 TR 
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();

                    processCount++;
                }
            }

            return processCount;
        }
    }
}

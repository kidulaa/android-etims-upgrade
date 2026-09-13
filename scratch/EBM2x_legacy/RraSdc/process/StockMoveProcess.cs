using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using EBM2x.UI;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EBM2x.RraSdc.process
{
    public class StockMoveProcess
    {
        public async void StockMoveDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }
            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_STOCK_MOVE_SEARCH);

            StockIoReq stockIoReq = new StockIoReq
            {
                lastReqDt = requestResponNode.LastDate    
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_STOCK_MOVE_SEARCH;
                    Console.WriteLine("stock url" + url);
                    // 'TIN' 'BHFID'RequestHeader 'cmcKey'
                    RraSdcService.SetDefaultRequestHeaders(client);
                    string jsonRequest = JsonConvert.SerializeObject(stockIoReq);
                    RraSdcReceiveJsonWriter.WriteTransactionREQ(RraSdcService.URL_STOCK_MOVE_SEARCH, jsonRequest);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        StockIoRes stockIoRes = JsonConvert.DeserializeObject<StockIoRes>(jsonResponse);
                        if (stockIoRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_STOCK_MOVE_SEARCH, jsonResponse);

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
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_STOCK_MOVE_SEARCH, jsonResponse);
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

            TransactionStockInOutModel stockInOutModel;
            StockIoMaster stockIoMaster = new StockIoMaster();
            StockIoItemMaster stockIoItemMaster = new StockIoItemMaster();

            TaxpayerItemMaster ItemMaster = new TaxpayerItemMaster();

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            long SarNo = stockIoMaster.GetStockIoSeq();
            string RegTyCd = "A"; 

            StockIoResData stockIoResData = stockIoRes.data;
            for (int i = 0; i < stockIoResData.stockList.Count; i++)
            {
                StockIo stockIo = stockIoResData.stockList[i];

                stockInOutModel = new TransactionStockInOutModel();
                stockInOutModel.CurrentItemRecord = null;
                stockInOutModel.InitModel(Tin, BhfId, SarNo, stockIo.ocrnDt, RegTyCd, "", "");
                stockInOutModel.TranRecord.SarTyCd = "04"; 
                stockInOutModel.TranRecord.CustTin = stockIo.custTin;
                stockInOutModel.TranRecord.CustBhfId = stockIo.custBhfId;
                stockInOutModel.TranRecord.OrgSarNo = stockIo.sarNo;

                if(stockIoMaster.IsStockIoTable("04", stockIo.custTin, stockIo.custBhfId, stockIo.sarNo))
                {
                    continue;
                }

                for (int j = 0; j < stockIo.itemList.Count; j++)
                {
                    StockIoItem stockIoItem = stockIo.itemList[j];
                    TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                    bool retSelect = ItemMaster.ToRecord(itemRecord, Tin, stockIoItem.itemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                    if (retSelect)
                    {
                        stockInOutModel.SetCurrentItem(itemRecord);
                        stockInOutModel.CurrentItemRecord.Qty = stockIoItem.qty;
                        stockInOutModel.CurrentItemRecord.AfterQty = stockInOutModel.CurrentItemRecord.RdsQty - stockInOutModel.CurrentItemRecord.Qty;
                        stockInOutModel.CalculateCurrentItem();
                        stockInOutModel.ConfirmCurrentItem();
                    }
                }
                // IN/OUT  STOCK_IO, STOCK_IO_ITEM
                stockInOutModel.TranRecord.SarNo = stockIoMaster.GetStockIoSeq();
                stockIoMaster.InsertTable(stockInOutModel.TranRecord);
                stockIoItemMaster.InsertTable(1, stockInOutModel.ItemRecords, stockInOutModel.TranRecord.SarNo);

                //===>>>>>>>>>
                //JCNA 20191204
                StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
                stockIoRraSdcUpload.SendStockIoSave(stockInOutModel.TranRecord.Tin, stockInOutModel.TranRecord.BhfId, stockInOutModel.TranRecord.SarNo);

                processCount++;
            }

            //===>>>>>>>>>
            // JCNA 20191204 TR
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            return processCount;
        }
    }
}

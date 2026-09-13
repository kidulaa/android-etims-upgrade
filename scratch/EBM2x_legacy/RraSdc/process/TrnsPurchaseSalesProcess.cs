using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Datafile.trlog;
using EBM2x.RraSdc.model;
using EBM2x.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static EBM2x.RraSdc.model.NonReportingReceiptsRes;

namespace EBM2x.RraSdc.process
{
    public class TrnsPurchaseSalesProcess
    {
        public async void TrnsPurchaseSalesDownloadProcess()
        {
            RequestResponList requestResponList = RraSdcHistoryWriter.ReadTransaction();
            if (requestResponList == null || requestResponList.NodeList.Count == 0)
            {
                requestResponList = new RequestResponList();
                requestResponList.InitRequestRespon();
            }

            RequestResponNode requestResponNode = requestResponList.GetRequestResponNode(RraSdcService.URL_TRNS_PURCHASE_SALES_SEARCH);



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
                //lastDtRqst = trnsPurchaseSalesJson.resultDt;
                string input = requestResponNode.LastDate;
                DateTime date = DateTime.ParseExact(input, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                DateTime newDate = date.AddDays(-5);
                string result = newDate.ToString("yyyyMMddHHmmss");
                lastDtRqst = result;
                Console.WriteLine("murife date1" + result);
            }
            else
            {
                string input = requestResponNode.LastDate;
                DateTime date = DateTime.ParseExact(input, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                DateTime newDate = date.AddDays(-5);
                string result = newDate.ToString("yyyyMMddHHmmss");
                lastDtRqst = result;
                Console.WriteLine("murife date2" + result);
                //lastDtRqst = requestResponNode.LastDate;
            }


            TrnsPurchaseSalesReq trnsPurchaseReq = new TrnsPurchaseSalesReq
            {
                //lastReqDt = requestResponNode.LastDate   
                lastReqDt = lastDtRqst
            };
            //end by Aime Irak

            /*
             * 
            TrnsPurchaseSalesReq trnsPurchaseReq = new TrnsPurchaseSalesReq
            {
                lastReqDt = requestResponNode.LastDate     
            };
            */
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + RraSdcService.URL_TRNS_PURCHASE_SALES_SEARCH;
                    
                    // 'TIN' 'BHFID' RequestHeader, 'cmcKey'.
                    //RraSdcService.SetDefaultRequestHeaders(client, "211111113", "00", "43513fbb4e8e49a8851aabd38fa4eb63");
                    RraSdcService.SetDefaultRequestHeaders(client);

                    string jsonRequest = JsonConvert.SerializeObject(trnsPurchaseReq);
                    //Console.WriteLine(jsonRequest)
                    RraSdcReceiveJsonWriter.WriteTransactionREQ(RraSdcService.URL_TRNS_PURCHASE_SALES_SEARCH, jsonRequest);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    //string jsonResponse = RraSdcReceiveJsonWriter.read("201912131100129515_scc_selectTrnsPurchaseSalesList");

                    Console.WriteLine("jsonRequest mfite2" + jsonResponse);
                    if (string.IsNullOrEmpty(jsonResponse)) jsonResponse = jsonResponse.Replace(":null,", ":\"\",");

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        TrnsPurchaseSalesRes trnsPurchaseSalesRes = JsonConvert.DeserializeObject<TrnsPurchaseSalesRes>(jsonResponse);
                        
                        if (trnsPurchaseSalesRes.resultCd.Equals("000"))
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionSCC(RraSdcService.URL_TRNS_PURCHASE_SALES_SEARCH, jsonResponse);
                            int processCount = UpdateTable(trnsPurchaseSalesRes);
                            requestResponNode.ProcessCount = processCount;
                            requestResponNode.LastDate = trnsPurchaseSalesRes.resultDt;
                            RraSdcHistoryWriter.WriteTransaction(requestResponList);
                        }
                        else if (trnsPurchaseSalesRes.resultCd.Equals("001"))
                        {
                        }
                        else
                        {
                            RraSdcReceiveJsonWriter.WriteTransactionERR(RraSdcService.URL_TRNS_PURCHASE_SALES_SEARCH, jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            //return rraSdcResult;
        }

       
        public int UpdateTable(TrnsPurchaseSalesRes trnsPurchaseSalesRes)
        {
            int processCount = 0;
            TrnsPurchaseMaster trnsPurchaseMaster = new TrnsPurchaseMaster();
            TransactionPurchaseModel PurchaseModel = new TransactionPurchaseModel();
            TrnsPurchaseSalesResData trnsPurchaseSalesResData = trnsPurchaseSalesRes.data;
            for (int i = 0; i < trnsPurchaseSalesResData.saleList.Count; i++)
            {
                TrnsPurchaseSales trnsPurchaseSales = trnsPurchaseSalesResData.saleList[i];
                //trnsPurchaseSales.spplrTin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                //trnsPurchaseSales.spplrNm = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNm;

                bool isExist = trnsPurchaseMaster.GetPurchaseInvoice(trnsPurchaseSales.spplrTin, trnsPurchaseSales.spplrInvcNo);
                if (isExist) continue;
                // Save bhf
                CreateTrnsPurchaseSales(trnsPurchaseSales);

                processCount++;
            }

            return processCount;
        }
        public bool CreateTrnsPurchaseSales(TrnsPurchaseSales trnsPurchaseSales)
        {
            TransactionPurchaseModel PurchaseModel = new TransactionPurchaseModel();
            TrnsPurchaseMaster PurchaseMaster = new TrnsPurchaseMaster();
            TaxpayerItemMaster ItemMaster = new TaxpayerItemMaster();

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            //JCNA 202001 DELETE string DvcId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblDvcId;
            long InvcNo = PurchaseMaster.GetPurchaseSeq();
            string PchsDt = trnsPurchaseSales.salesDt;  // DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "A";
            string UserId = "";
            string UserNm = "";

            PurchaseModel.CurrentItemRecord = null;
            PurchaseModel.InitModel(Tin, BhfId, InvcNo, PchsDt, RegTyCd, UserId, UserNm);
            PurchaseModel.TranRecord.OrgInvcNo = 0;

            PurchaseModel.TranRecord.TaxprNm = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNm;
            PurchaseModel.TranRecord.SpplrTin = trnsPurchaseSales.spplrTin;
            PurchaseModel.TranRecord.SpplrNm = trnsPurchaseSales.spplrNm;
            PurchaseModel.TranRecord.SpplrBhfId = trnsPurchaseSales.spplrBhfId;
            PurchaseModel.TranRecord.SpplrInvcNo = trnsPurchaseSales.spplrInvcNo;

            //Added By Bright 1.5.2022 --Issue Purchase with Zero Amount and No Items
            PurchaseModel.TranRecord.TotItemCnt = trnsPurchaseSales.totItemCnt;
            PurchaseModel.TranRecord.TaxblAmtA = trnsPurchaseSales.taxblAmtA;
            PurchaseModel.TranRecord.TaxblAmtB = trnsPurchaseSales.taxblAmtB;
            PurchaseModel.TranRecord.TaxblAmtC = trnsPurchaseSales.taxblAmtC;
            PurchaseModel.TranRecord.TaxblAmtD = trnsPurchaseSales.taxblAmtD;
            PurchaseModel.TranRecord.TaxblAmtE = trnsPurchaseSales.taxblAmtE;
            PurchaseModel.TranRecord.TaxRtA = trnsPurchaseSales.taxRtA;
            PurchaseModel.TranRecord.TaxRtB = trnsPurchaseSales.taxRtB;
            PurchaseModel.TranRecord.TaxRtC = trnsPurchaseSales.taxRtC;
            PurchaseModel.TranRecord.TaxRtD = trnsPurchaseSales.taxRtD;
            PurchaseModel.TranRecord.TaxRtE = trnsPurchaseSales.taxRtE;
            PurchaseModel.TranRecord.TaxAmtA = trnsPurchaseSales.taxAmtA;
            PurchaseModel.TranRecord.TaxAmtB = trnsPurchaseSales.taxAmtB;
            PurchaseModel.TranRecord.TaxAmtC = trnsPurchaseSales.taxAmtC;
            PurchaseModel.TranRecord.TaxAmtD = trnsPurchaseSales.taxAmtD;
            PurchaseModel.TranRecord.TaxAmtE = trnsPurchaseSales.taxAmtE;
            PurchaseModel.TranRecord.TotTaxblAmt = trnsPurchaseSales.totTaxblAmt;
            PurchaseModel.TranRecord.TotTaxAmt = trnsPurchaseSales.totTaxAmt;
            PurchaseModel.TranRecord.TotAmt = trnsPurchaseSales.totAmt;
            //END

            for (int i = 0; i < trnsPurchaseSales.itemList.Count; i++)
            {
                TrnsPurchaseSalesItem salesItem = trnsPurchaseSales.itemList[i];

                TaxpayerItemRecord itemRecord = new TaxpayerItemRecord();
                /*
                 * Comment By Bright Issue Purchase with Zero Amount and No Items (This will always give null)
                bool retSelect = ItemMaster.ToRecord(itemRecord, Tin, salesItem.itemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                if (retSelect)
                  if (true)
                */
 
                if (true)
                {
                    PurchaseModel.SetCurrentItem(itemRecord);
                    PurchaseModel.CurrentItemRecord.SpplrTin = PurchaseModel.TranRecord.SpplrTin;

                    PurchaseModel.CurrentItemRecord.TaxTyCd = salesItem.taxTyCd;
                    PurchaseModel.CurrentItemRecord.Prc = salesItem.prc;
                    PurchaseModel.CurrentItemRecord.Qty = salesItem.qty;
                    PurchaseModel.CurrentItemRecord.DcRt = salesItem.dcRt;

                    PurchaseModel.CurrentItemRecord.SpplrItemCd = salesItem.itemCd;
                    PurchaseModel.CurrentItemRecord.SpplrItemClsCd = salesItem.itemClsCd;
                    PurchaseModel.CurrentItemRecord.SpplrItemNm = salesItem.itemNm;
                    PurchaseModel.CurrentItemRecord.ItemExprDt = DateTime.Now.ToString("yyyyMMdd");
                    //Added By Bright 1.5.2022 --Issue Purchase with Zero Amount and No Items
                    PurchaseModel.CurrentItemRecord.ItemCd = salesItem.itemCd;
                    PurchaseModel.CurrentItemRecord.ItemNm = salesItem.itemNm;
                    PurchaseModel.CurrentItemRecord.ItemClsCd = salesItem.itemClsCd;

                    //Added by Aime 10/22/2022 -- Issue Purchase for PkgUnitCd and QtyUnityCd
                    PurchaseModel.CurrentItemRecord.PkgUnitCd = salesItem.pkgUnitCd;
                    PurchaseModel.CurrentItemRecord.QtyUnitCd = salesItem.qtyUnitCd;
       
                    //END
                    PurchaseModel.CalculateCurrentItem();
                    PurchaseModel.ConfirmCurrentItem();
                }
            }

            TrnsPurchaseMaster TrnsPurchaseMaster = new TrnsPurchaseMaster();
            TrnsPurchaseItemMaster TrnsPurchaseItemMaster = new TrnsPurchaseItemMaster();

            // Header, Items
            TrnsPurchaseMaster.InsertTable(PurchaseModel.TranRecord);
            TrnsPurchaseItemMaster.InsertTable(PurchaseModel.ItemRecords);
  
            //===>>>>>>>>>
            //JCNA 20191204
            TrnsPurchaseRraSdcUpload trnsPurchaseRraSdcUpload = new TrnsPurchaseRraSdcUpload();
            trnsPurchaseRraSdcUpload.SendTranPurchaseSave(PurchaseModel.TranRecord.Tin, PurchaseModel.TranRecord.BhfId, PurchaseModel.TranRecord.SpplrTin, PurchaseModel.TranRecord.InvcNo);
            //===>>>>>>>>>
            // JCNA 20191204 TR 
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();
            return true;
        }

        public bool UpdateItemTable(TrnsPurchaseSalesItem trnsPurchaseSalesItem)
        {
            string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string bhfid = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            TaxpayerItemMaster master = new TaxpayerItemMaster();
            TaxpayerItemRecord custRecord = new TaxpayerItemRecord();

            bool IsExist = master.GetItemCode(tin, trnsPurchaseSalesItem.itemCd);
            if (IsExist) return true;

            custRecord.Tin = tin;                  // Taxpayer Identification Number(TIN)
            custRecord.ItemCd = trnsPurchaseSalesItem.itemCd;               // Item Code
            custRecord.ItemClsCd = trnsPurchaseSalesItem.itemClsCd;            // Item Classification Code (RRA)
            custRecord.ItemTyCd = trnsPurchaseSalesItem.itemCd.Substring(2,1);             // Item Type Code
            custRecord.ItemNm = trnsPurchaseSalesItem.itemNm;               // Item Name
            custRecord.ItemStdNm = trnsPurchaseSalesItem.itemNm;            // Item Stand Name
            custRecord.OrgnNatCd = trnsPurchaseSalesItem.itemCd.Substring(0, 2);            // Origin National Code
            custRecord.PkgUnitCd = trnsPurchaseSalesItem.pkgUnitCd;            // Package Unit Code
            custRecord.QtyUnitCd = trnsPurchaseSalesItem.qtyUnitCd;            // Quantity Unit Code
            custRecord.TaxTyCd = trnsPurchaseSalesItem.taxTyCd;              // Taxation Type Code
            custRecord.Bcd = trnsPurchaseSalesItem.bcd;                // Barcode
            custRecord.RegBhfId = bhfid;        // Branch Office ID
            custRecord.UseYn = "Y";             // Use(Y/N)
            custRecord.RraModYn = "Y";          // RRA Modified(Y/N)
            custRecord.AddInfo = "";            // Additional Information
            custRecord.SftyQty = 0;             // Safety Quantity
            custRecord.IsrcAplcbYn = "N";        // Insurance Appicable(Y/N)
            custRecord.DftPrc = 0;              // Default Price
            custRecord.GrpPrcL1 = 0;            // Group Default Price L1
            custRecord.GrpPrcL2 = 0;            // Group Default Price L2
            custRecord.GrpPrcL3 = 0;            // Group Default Price L3
            custRecord.GrpPrcL4 = 0;            // Group Default Price L4
            custRecord.GrpPrcL5 = 0;            // Group Default Price L5
            custRecord.RegrId = "";     // Registrant ID
            custRecord.RegrNm = "";       // Registrant Name
            custRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");      // Registered Date
            custRecord.ModrId = "";     // Modifier ID
            custRecord.ModrNm = "";       // Modifier Name
            custRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");      // Modified Date
            custRecord.InitlWhUntpc = trnsPurchaseSalesItem.prc;            
            custRecord.InitlQty = 0;            
            custRecord.Rm = "";                
            custRecord.UseBarcode = string.IsNullOrEmpty(trnsPurchaseSalesItem.bcd) ? "N" : "Y";    
            custRecord.UseAdiYn = "N";         
            custRecord.BatchNum = "";           // BatchNum
            //JCNA 202001 DELETE custRecord.ExpirationDtUse = "N";   // Expiration Dt Use

            StockMasterRecord stockRecord = new StockMasterRecord();
            stockRecord.Tin = custRecord.Tin;
            stockRecord.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            stockRecord.ItemCd = custRecord.ItemCd;
            stockRecord.RsdQty = 0;
            stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.ModrId = "";
            stockRecord.ModrNm = "";
            stockRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.RegrId = "";
            stockRecord.RegrNm = "";

            custRecord.UpdateNull();
            stockRecord.UpdateNull();
            bool ret = master.ToTable(custRecord, stockRecord);
            if (ret)
            {
                //JCNA 20191204
                StockMasterRraSdcUpload stockMasterRraSdcUpload = new StockMasterRraSdcUpload();
                stockMasterRraSdcUpload.SendStockMasterSave(stockRecord.Tin, stockRecord.BhfId, stockRecord.ItemCd);

                //===>>>>>>>>>
                //JCNA 20191204
                ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
                itemRraSdcUpload.SendItemSave(custRecord.Tin, custRecord.ItemCd);

                //===>>>>>>>>>
                // JCNA 20191204 TR 
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();
            }

            return true;
        }

        private void OnFunctionSendTranSales(long curRcptNo)
        {
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            List<TrnsSalesSaveReq> sendList = trnsSaleRraSdcUpload.getNonTrnsSaleTable(curRcptNo);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Sales";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                if (sendList[i].itemList.Count > 0)
                {
                    RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
                }
            }
        }
        private void OnFunctionSendTranSalesRcpt(long curRcptNo)
        {
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            List<TrnsSalesRcptSaveReq> sendListRcpt = trnsSaleRcptRraSdcUpload.getNonReportingTrnsSaleRcptTable(curRcptNo);
            for (int i = 0; i < sendListRcpt.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "SalesReceipt";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_RECEIPT_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendListRcpt[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }
    }
}

/*
TAXPAYER_ITEM (
    TIN               CHAR (9)        NOT NULL,                  TIN         CHAR (9)        NOT NULL,   
    ITEM_CD           VARCHAR (20)    NOT NULL,                  ITEM_CD     VARCHAR (20),                public string itemCd { get; set; }     // Item Code
    ITEM_CLS_CD       VARCHAR (10)    NOT NULL,                  ITEM_CLS_CD VARCHAR (10),                public string itemClsCd { get; set; }  // Item Classification Code
    ITEM_TY_CD        VARCHAR (5)     NOT NULL,                  ITEM_CD.SUBSTRING(2,1)
    ITEM_NM           VARCHAR (200)   NOT NULL,                  ITEM_NM     VARCHAR (200),               public string itemNm { get; set; }     // Item Name
    ITEM_STD_NM       VARCHAR (200),                             ITEM_NM     VARCHAR (200),               public string itemNm { get; set; }     // Item Name
    ORGN_NAT_CD       VARCHAR (5)     NOT NULL,                  ITEM_CD.SUBSTRING(0,2)
    PKG_UNIT_CD       VARCHAR (5)     NOT NULL,                  PKG_UNIT_CD VARCHAR (5),                 public string pkgUnitCd { get; set; }  // Packaging Unit Code
    QTY_UNIT_CD       VARCHAR (5)     NOT NULL,                  QTY_UNIT_CD VARCHAR (5),                 public string qtyUnitCd { get; set; }  // Quantity Unit Code
    TAX_TY_CD         VARCHAR (5)     NOT NULL,                                                           public string taxTyCd { get; set; }    // Taxation Type Code
    BCD               VARCHAR (20),                              BCD         VARCHAR (20),                public string bcd { get; set; }        // Bar code
    REG_BHF_ID        CHAR (2),                                  "00"
    USE_YN            CHAR (1),                                  "Y"
    RRA_MOD_YN        CHAR (1)        NOT NULL,                  "Y"
    ADD_INFO          VARCHAR (7),                               ""
    SFTY_QTY          DECIMAL (13, 2) NOT NULL,                  0
    ISRC_APLCB_YN     CHAR (1)        NOT NULL,                  "N"
    DFT_PRC           DECIMAL (13, 2),                           "0"
    GRP_PRC_L1        DECIMAL (13, 2),                           "0"
    GRP_PRC_L2        DECIMAL (13, 2),                           "0"
    GRP_PRC_L3        DECIMAL (13, 2),                           "0"
    GRP_PRC_L4        DECIMAL (13, 2),                           "0"
    GRP_PRC_L5        DECIMAL (13, 2),                           "0"
    REGR_ID           VARCHAR (20),                              ""
    REGR_NM           VARCHAR (60),                              ""
    REG_DT            VARCHAR (14),                              DateTime.now
    MODR_ID           VARCHAR (20),                              ""
    MODR_NM           VARCHAR (60),                              ""
    MOD_DT            VARCHAR (14),                              DateTime.now
    INITL_WH_UNTPC    DECIMAL (18, 2) NOT NULL,                  SPLY_AMT    DECIMAL (18, 2),            public double prc { get; set; }  // Unit Price
    INITL_QTY         DECIMAL (13, 2) NOT NULL,                  0, 
    RM                VARCHAR (400),                             ""
    USE_BARCODE       CHAR (1)        NOT NULL,                  "Y"
    USE_ADI_YN        CHAR (1)        NOT NULL,                  "N"
    BATCH_NUM         VARCHAR (20),                              ""
    Expiration_Dt_Use CHAR (1),                                  "N"
    PRIMARY KEY (
        TIN,
        ITEM_CD
    )
);
 */

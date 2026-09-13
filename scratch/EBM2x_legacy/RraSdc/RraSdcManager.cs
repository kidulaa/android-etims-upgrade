using System;
using EBM2x.Dependency;
using Xamarin.Forms;

namespace EBM2x.RraSdc
{
    public class RraSdcManager
    {
        private IRraSdc _rraSdcService;

        // EBM Client 
        public string InitializeExcute_selectInitInfo(string jsonInitReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonInitRes = _rraSdcService.InitializeExcute_selectInitInfo(jsonInitReq);
            return jsonInitRes;
        }
        //Key 
        public string KeyExcute_setKey(string CmcKey, string IntrlKey, string SignKey)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonInitRes = _rraSdcService.KeyExcute_setKey(CmcKey, IntrlKey, SignKey);
            return jsonInitRes;
        }

        //Key
        public string KeyExcute_getKey()
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonKeyInfo = _rraSdcService.KeyExcute_getKey();
            return jsonKeyInfo;
        }

        //Key 
        //SkmmConst 
        public string KeyExcute_getKey(string keyId)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonKeyInfo = _rraSdcService.KeyExcute_getKey(keyId);
            return jsonKeyInfo;
        }

        
        public string CodeExcute_selectCodeList(string jsonCodeReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonCodeRes = _rraSdcService.CodeExcute_selectCodeList(jsonCodeReq);
            return jsonCodeRes;
        }

       
        public string ItemClsExcute_selectItemClsList(string jsonItemClsReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonItemClsRes = _rraSdcService.ItemClsExcute_selectItemClsList(jsonItemClsReq);
            return jsonItemClsRes;
        }

        
        public string SoftwareExcute_selectSwVersion(string jsonSwVerReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonSwVerRes = _rraSdcService.SoftwareExcute_selectSwVersion(jsonSwVerReq);
            return jsonSwVerRes;
        }

       
        public string BhfExcute_selectCustomer(string jsonCustReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonCustRes = _rraSdcService.BhfExcute_selectCustomer(jsonCustReq);
            return jsonCustRes;
        }

        
        public string BhfExcute_saveCustomer(string jsonCustSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonCustSaveRes = _rraSdcService.BhfExcute_saveCustomer(jsonCustSaveReq);
            return jsonCustSaveRes;
        }

       
        public string BhfExcute_selectBhfList(string jsonBhfReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonBhfRes = _rraSdcService.BhfExcute_selectBhfList(jsonBhfReq);
            return jsonBhfRes;
        }

        public string BhfExcute_saveBhfUser(string jsonBhfUserSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonBhfUserSaveRes = _rraSdcService.BhfExcute_saveBhfUser(jsonBhfUserSaveReq);
            return jsonBhfUserSaveRes;
        }

        
        public string BhfExcute_saveBhfInsurance(string jsonBhfInsuranceSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonBhfInsuranceSaveRes = _rraSdcService.BhfExcute_saveBhfInsurance(jsonBhfInsuranceSaveReq);
            return jsonBhfInsuranceSaveRes;
        }

        public string ItemExcute_selectItemList(string jsonItemReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonItemRes = _rraSdcService.ItemExcute_selectItemList(jsonItemReq);
            return jsonItemRes;
        }

        
        public string ItemExcute_saveItem(string jsonItemSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonItemSaveRes = _rraSdcService.ItemExcute_saveItem(jsonItemSaveReq);
            return jsonItemSaveRes;
        }

        
        public string ItemExcute_saveItemComposition(string jsonItemCpstSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonItemCpstSaveRes = _rraSdcService.ItemExcute_saveItemComposition(jsonItemCpstSaveReq);
            return jsonItemCpstSaveRes;
        }

       
        public string ImportItemExcute_selectImportItemList(string jsonImptItemReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonImptItemRes = _rraSdcService.ImportItemExcute_selectImportItemList(jsonImptItemReq);
            return jsonImptItemRes;
        }

        
        public string ImportItemExcute_updateImportItem(string jsonImptItemSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonImptItemSaveRes = _rraSdcService.ImportItemExcute_updateImportItem(jsonImptItemSaveReq);
            return jsonImptItemSaveRes;
        }

        
        public string TrnsSalesExcute_selectTrnsSalesList(string jsonTrnsSalesReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonTrnsSalesRes = _rraSdcService.TrnsSalesExcute_selectTrnsSalesList(jsonTrnsSalesReq);
            Console.WriteLine("---------------sale-json---" + jsonTrnsSalesReq);
            return jsonTrnsSalesRes;
        }

        
        public string TrnsSalesExcute_saveTrnsSales(string jsonTrnsSalesSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonTrnsSalesSaveRes = _rraSdcService.TrnsSalesExcute_saveTrnsSales(jsonTrnsSalesSaveReq);
            Console.WriteLine("---------------sale-json---" + jsonTrnsSalesSaveReq);
            return jsonTrnsSalesSaveRes;
        }

        public string TrnsSalesExcute_saveTrnsSalesReceipt(string jsonTrnsSalesRcptSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonTrnsSalesRcptSaveRes = _rraSdcService.TrnsSalesExcute_saveTrnsSalesReceipt(jsonTrnsSalesRcptSaveReq);

            Console.WriteLine("---------------sale-json---" + jsonTrnsSalesRcptSaveReq);
            return jsonTrnsSalesRcptSaveRes;
        }

       
        public string TrnsPurchaseExcute_selectTrnsPurchaseSalesList(string jsonTrnsPurchaseSalesReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonTrnsPurchaseSalesRes = _rraSdcService.TrnsPurchaseExcute_selectTrnsPurchaseSalesList(jsonTrnsPurchaseSalesReq);
            return jsonTrnsPurchaseSalesRes;
        }

        
        public string TrnsPurchaseExcute_selectTrnsPurchaseList(string jsonTrnsPurchaseReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonTrnsPurchaseRes = _rraSdcService.TrnsPurchaseExcute_selectTrnsPurchaseList(jsonTrnsPurchaseReq);
            return jsonTrnsPurchaseRes;
        }

        
        public string TrnsPurchaseExcute_insertTrnsPurchase(string jsonTrnsPurchaseSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonTrnsPurchaseSaveRes = _rraSdcService.TrnsPurchaseExcute_insertTrnsPurchase(jsonTrnsPurchaseSaveReq);
            return jsonTrnsPurchaseSaveRes;
        }

       
        public string StockMasterExcute_selectStockMasterList(string jsonStockMstReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonStockMstRes = _rraSdcService.StockMasterExcute_selectStockMasterList(jsonStockMstReq);
            return jsonStockMstRes;
        }

      
        public string StockMasterExcute_saveStockMaster(string jsonStockMstSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonStockMstSaveRes = _rraSdcService.StockMasterExcute_saveStockMaster(jsonStockMstSaveReq);
            return jsonStockMstSaveRes;
        }

        
        public string StockIoExcute_selectStockIOList(string jsonStockIOReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonStockIORes = _rraSdcService.StockIoExcute_selectStockIOList(jsonStockIOReq);
            return jsonStockIORes;
        }

        
        public string StockIoExcute_insertStockIO(string jsonStockIOSaveReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonStockIOSaveRes = _rraSdcService.StockIoExcute_insertStockIO(jsonStockIOSaveReq);
            return jsonStockIOSaveRes;
        }

        
        public string NoticeExcute_selectNoticeList(string jsonNoticeReq)
        {
            _rraSdcService = DependencyService.Get<IRraSdc>();
            string jsonNoticeRes = _rraSdcService.NoticeExcute_selectNoticeList(jsonNoticeReq);
            return jsonNoticeRes;
        }
    }
}

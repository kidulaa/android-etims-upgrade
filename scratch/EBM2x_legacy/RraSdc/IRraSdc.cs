namespace EBM2x.RraSdc
{
    public interface IRraSdc
    {
        // EBM Client 
        string InitializeExcute_selectInitInfo(string jsonInitReq);   // return string jsonInitRes

        //Key 
        string KeyExcute_setKey(string CmcKey, string IntrlKey, string SignKey);
        
        //Key
        string KeyExcute_getKey();  // return string jsonKeyInfo
        
        //Key 
        //SkmmConst 
        string KeyExcute_getKey(string keyId);  // return string jsonKeyInfo

        
        string CodeExcute_selectCodeList(string jsonCodeReq);

        string ItemClsExcute_selectItemClsList(string jsonItemClsReq);

        
        string SoftwareExcute_selectSwVersion(string jsonSwVerReq);

       
        string BhfExcute_selectCustomer(string jsonCustReq);

        
        string BhfExcute_saveCustomer(string jsonCustSaveReq);

        
        string BhfExcute_selectBhfList(string jsonBhfReq);

        string BhfExcute_saveBhfUser(string jsonBhfUserSaveReq);

        string BhfExcute_saveBhfInsurance(string jsonBhfInsuranceSaveReq);

        string ItemExcute_selectItemList(string jsonItemReq);

        string ItemExcute_saveItem(string jsonItemSaveReq);

        string ItemExcute_saveItemComposition(string jsonItemCpstSaveReq);

        string ImportItemExcute_selectImportItemList(string jsonImptItemReq);

        
        string ImportItemExcute_updateImportItem(string jsonImptItemSaveReq);

        
        string TrnsSalesExcute_selectTrnsSalesList(string jsonTrnsSalesReq);

        
        string TrnsSalesExcute_saveTrnsSales(string jsonTrnsSalesSaveReq);

       
        string TrnsSalesExcute_saveTrnsSalesReceipt(string jsonTrnsSalesRcptSaveReq);

        
        string TrnsPurchaseExcute_selectTrnsPurchaseSalesList(string jsonTrnsPurchaseSalesReq);

      
        string TrnsPurchaseExcute_selectTrnsPurchaseList(string jsonTrnsPurchaseReq);

       
        string TrnsPurchaseExcute_insertTrnsPurchase(string jsonTrnsPurchaseSaveReq);

        
        string StockMasterExcute_selectStockMasterList(string jsonStockMstReq);

       
        string StockMasterExcute_saveStockMaster(string jsonStockMstSaveReq);

        
        string StockIoExcute_selectStockIOList(string jsonStockIOReq);

        
        string StockIoExcute_insertStockIO(string jsonStockIOSaveReq);

        
        string NoticeExcute_selectNoticeList(string jsonNoticeReq);

    }
}

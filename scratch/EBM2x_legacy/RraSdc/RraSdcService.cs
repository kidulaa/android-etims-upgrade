using EBM2x.Datafile.env;
using EBM2x.Models.config;
using EBM2x.RraSdc.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace EBM2x.RraSdc
{
    public class RraSdcService
    {
        public static string cosebase = "November7";
        //rwanda Local (test)/ if it is test you can replace it with production api

        //public static string APPLICATION_NAME = "KENYA DEV LOCAL";
        //public static string EXTERNAL_URL = "https://eis-dev-api.kra.go.ke/etims-api";
        //public static string INTERNAL_URL = "https://eis-dev-api.kra.go.ke/etims-api";
        //public static string RECEIPT_URL = "https://eis-dev-api.kra.go.ke";


        //public static string APPLICATION_NAME = "KENYA TEST LOCAL V0.6";
        //public static string EXTERNAL_URL = "https://etims-test-api.kra.go.ke/etims-api";
        //public static string INTERNAL_URL = "https://etims-test-api.kra.go.ke/etims-api";
        //public static string RECEIPT_URL = "https://etims-portal-test.kra.go.ke";


        //public static string APPLICATION_NAME = "KRA ETIMS V1.12";
        //public static string EXTERNAL_URL = "https://etims-api.kra.go.ke/etims-api";
        //public static string INTERNAL_URL = "https://etims-api.kra.go.ke/etims-api";
        //public static string RECEIPT_URL = "https://etims.kra.go.ke";

        //public static string APPLICATION_NAME = "KRA KIRUI V1.00";
        //public static string EXTERNAL_URL = "http://192.168.27.168:8080/etims-api";
        //public static string INTERNAL_URL = "http://192.168.27.168:8080/etims-api";
        //public static string RECEIPT_URL = "https://etims-api.kra.go.ke";

        //QA

        public static string APPLICATION_NAME = "KENYA QA V1.26.0";
        public static string EXTERNAL_URL = "https://etims-api-test.kra.go.ke/etims-api";
        public static string INTERNAL_URL = "https://etims-api-test.kra.go.ke/etims-api";
        public static string RECEIPT_URL = "https://etims-portal-test.kra.go.ke";


        //public static string APPLICATION_NAME = "KENYA SANDBOX V0.5.0";
        //public static string EXTERNAL_URL = "https://etims-api-sbx.kra.go.ke/etims-api";
        //public static string INTERNAL_URL = "https://etims-api-sbx.kra.go.ke/etims-api";
        //public static string RECEIPT_URL = "https://etims-sbx.kra.go.ke";

        // pharis
        //public static string APPLICATION_NAME = "LOCAL PHARIS";
        //public static string EXTERNAL_URL = "http://192.168.135.142:8080/etims-api/";
        //public static string INTERNAL_URL = "http://192.168.135.142:8080/etims-api/";
        //public static string RECEIPT_URL = "http://192.168.135.142:8080";


        //
        public static string HEADER_TIN = "tin";
	    public static string HEADER_BHF_ID = "bhfId";
	    public static string HEADER_CMC_KEY = "cmcKey";

        //*************************************
        //* API Internal URL
        //*************************************
        public static string URL_INIT_INFO = "selectInitInfo";
            
        //*************************************
        //* API External URL
        //*************************************
        public static string URL_TEST_ECHO = "selectTestEcho";
        public static string URL_MAIN_SERVERTIME = "selectServerTime";

        public static string URL_CODE_SEARCH = "selectCodeList";
        public static string URL_ITEM_CLASS_SEARCH = "selectItemClsList";
        public static string URL_ITEM_SEARCH = "selectItemList";
        public static string URL_ITEM_SAVE = "saveItem";
        public static string URL_ITEM_COMPOSITION_SAVE = "saveItemComposition";
        public static string URL_CUSTOMER_SEARCH = "selectCustomerList";
        public static string URL_BHF_SEARCH = "selectBhfList";
        public static string URL_BHF_USER_SAVE = "saveBhfUser";
        public static string URL_BHF_INSURANCE_SAVE = "saveBhfInsurance";
        public static string URL_BHF_CUST_SEARCH = "selectCustomer";
        public static string URL_BHF_CUST_SAVE = "saveBhfCustomer";
        public static string URL_TRNS_SALES_SEARCH = "selectTrnsSalesList";
        public static string URL_TRNS_SALES_SAVE = "saveTrnsSales";
        public static string URL_TRNS_SALES_RECEIPT_SAVE = "saveTrnsSalesReceipt";
        public static string URL_TRNS_PURCHASE_SALES_SEARCH = "selectTrnsPurchaseSalesList";
        public static string URL_TRNS_PURCHASE_SEARCH = "selectTrnsPurchaseList";
        public static string URL_TRNS_PURCHASE_SAVE = "insertTrnsPurchase";
        public static string URL_IMPORT_ITEM_SEARCH = "selectImportItemList";
        public static string URL_IMPORT_ITEM_UPDATE = "updateImportItem";
        public static string URL_STOCK_MASTER_SEARCH = "selectStockMasterList";
        public static string URL_STOCK_MASTER_SAVE = "saveStockMaster";
        public static string URL_STOCK_IO_SEARCH = "selectStockIOList";
        public static string URL_STOCK_MOVE_SEARCH = "selectStockMoveList";
        public static string URL_STOCK_IO_SAVE = "insertStockIO";
        public static string URL_NOTICE_SEARCH = "selectNoticeList";

        public static string URL_REPORT_Z_SAVE = "saveReportZ";

        public static string URL_TAXPAYER_INFO_SELECT = "selectTaxpayerInfo";
        public static string URL_TRNS_NON_REPORTING_SALES = "selectNonReportingInvoiceList";

        public static bool SetDefaultRequestHeaders(HttpClient client)
        {
            InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
           
            if (initInfoVO != null && !string.IsNullOrEmpty(initInfoVO.tin))
            { 
                client.DefaultRequestHeaders.Add(HEADER_TIN, initInfoVO.tin);
                client.DefaultRequestHeaders.Add(HEADER_BHF_ID, initInfoVO.bhfId);
                client.DefaultRequestHeaders.Add(HEADER_CMC_KEY, initInfoVO.cmcKey);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static SwVersionReq GetDefaultRequestHeaders()
        {
            //InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
            EnvPosSetup envPosSetup = EnvPosSetupService.LoadEnvPosSetup();

            SwVersionReq swVersionReq = new SwVersionReq();

            swVersionReq.tin = envPosSetup.GblTaxIdNo;
            swVersionReq.bhfId = envPosSetup.GblBrcCod;
            swVersionReq.dvcSrlNo = envPosSetup.GblSerialNo;

            return swVersionReq;
        }
        public static TaxpayerInfoReq GetTaxpayerInfoRequestHeaders()
        {
            //InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
            EnvPosSetup envPosSetup = EnvPosSetupService.LoadEnvPosSetup();

            TaxpayerInfoReq taxpayerInfoReq = new TaxpayerInfoReq();

            taxpayerInfoReq.tin = envPosSetup.GblTaxIdNo;
            taxpayerInfoReq.bhfId = envPosSetup.GblBrcCod;

            return taxpayerInfoReq;
        }
        public static void SetDefaultRequestHeaders(HttpClient client, string tin, string bhfid)
        {
            InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
            if (initInfoVO != null && initInfoVO.tin.Equals(tin) && initInfoVO.bhfId.Equals(bhfid))
            {
                client.DefaultRequestHeaders.Add(HEADER_TIN, initInfoVO.tin);
                client.DefaultRequestHeaders.Add(HEADER_BHF_ID, initInfoVO.bhfId);
                client.DefaultRequestHeaders.Add(HEADER_CMC_KEY, initInfoVO.cmcKey);
            }
        }
        public static void SetDefaultRequestHeaders(HttpClient client, string tin, string bhfid, string cmcKey)
        {
            client.DefaultRequestHeaders.Add(HEADER_TIN, tin);
            client.DefaultRequestHeaders.Add(HEADER_BHF_ID, bhfid);
            client.DefaultRequestHeaders.Add(HEADER_CMC_KEY, cmcKey);
        }

        public static bool IsValidJson(string input)
        {
            try
            {
                JToken.Parse(input);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.Models.regitotal;

namespace EBM2x.Models.config
{
    public class TranInformation
    {
        public string StoreCode { get; set; }
        public string SaleDate { get; set; } 
        public string PosNumber { get; set; } 
        public long   InvoiceN0 { get; set; } 
        public int    ReceiptNumber { get; set; } 
        public string UserCode { get; set; } 
        public string UserName { get; set; } 
        public string DateTime { get; set; }  
        public string LogFlag { get; set; }  
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string VibrationBellNum { get; set; } 
        public string OrderNoIncrease { get; set; }
        public string RefundBarcodeNo { get; set; }
        public void initialize(TranInformation tranInformation)
        {
            StoreCode = tranInformation.StoreCode;
            SaleDate = tranInformation.SaleDate;
            PosNumber = tranInformation.PosNumber;
            InvoiceN0 = tranInformation.InvoiceN0;
            ReceiptNumber = tranInformation.ReceiptNumber;
            UserCode = tranInformation.UserCode;
            DateTime = tranInformation.DateTime;
            SupplierCode = SupplierCode;
            SupplierName = SupplierName;
            VibrationBellNum = VibrationBellNum;
            RefundBarcodeNo = RefundBarcodeNo;
        }
        public void initialize(RegiTotal regiInfor)
        {
            StoreCode = regiInfor.RegiHeader.StoreCode;
            SaleDate = regiInfor.RegiHeader.OpenDate;
            PosNumber = regiInfor.RegiHeader.RegiNo;
            ReceiptNumber = 0;
            UserCode = regiInfor.RegiHeader.UserID;
            UserName = regiInfor.RegiHeader.UserName; // JINIT_20191201
            DateTime = regiInfor.RegiHeader.UpdateDate + regiInfor.RegiHeader.UpdateTime;
            SupplierCode = string.Empty;
            SupplierName = string.Empty;
            VibrationBellNum = string.Empty;
            RefundBarcodeNo = string.Empty;
        }

        public void clear()
        {
            StoreCode = string.Empty;  			
            SaleDate = System.DateTime.Now.ToString("yyyyMMdd");
            PosNumber = string.Empty;     
            InvoiceN0 = 0;                
            ReceiptNumber = 0;            
            UserCode = string.Empty; 
            UserName = string.Empty;
            DateTime = string.Empty;
            LogFlag = string.Empty;
            RefundBarcodeNo = string.Empty;
        }
    }
}

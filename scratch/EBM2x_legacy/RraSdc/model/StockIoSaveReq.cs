using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class StockIoSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public long sarNo { get; set; }  // Stored and released Number
	    public long orgSarNo { get; set; }  // Original Stored and released Number
	    public string regTyCd { get; set; }  // Registration Type Code
	    public string custTin { get; set; }  // Customer TIN
	    public string custBhfId { get; set; }  // Customer Branch ID
	    public string custNm { get; set; }  // Customer Name
	    public string sarTyCd { get; set; }  // Stored and released Type Code
	    //public string sarRsnCd { get; set; }  // Stored and released Reason Code
	    public string ocrnDt { get; set; }  // Occurred Date time
	    public int totItemCnt { get; set; }  // Total Item Count
	    public double totTaxblAmt { get; set; }  // Total Supply Price
	    public double totTaxAmt { get; set; }  // Total VAT
	    public double totAmt { get; set; }  // Total Amount
	    public string remark { get; set; }  // Remark
        public string regrId { get; set; }  
        public string regrNm { get; set; }  
        public string modrId { get; set; }  
        public string modrNm { get; set; } 
        public List<StockIoItemSaveReq> itemList { get; set; }  // Item List
    }
}

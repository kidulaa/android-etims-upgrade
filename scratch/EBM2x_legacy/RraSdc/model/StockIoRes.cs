using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class StockIoRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public StockIoResData data { get; set; }  // Result Date
    }
	public class StockIoResData {
		public List<StockIo> stockList { get; set; }  // StockIo List
	}

	public class StockIo {
		public long sarNo { get; set; }  // Stored and released Number
		public long orgSarNo { get; set; }  // Original Stored and released Number
		public string regTyCd { get; set; }  // Registration Type Code
		public string custTin { get; set; }  // Customer TIN
		public string custBhfId { get; set; }  // Customer Branch ID
		public string custNm { get; set; }  // Customer Name
		//public int invcNo { get; set; }  // Invoice Number
		public string sarTyCd { get; set; }  // Stored and released Type Code
		//public string sarRsnCd { get; set; }  // Stored and released Reason Code
		public string ocrnDt { get; set; }  // Occurred Date time : 'yyyyMMddHHmmss'
		public int totItemCnt { get; set; }  // Total Item Count
		public double totTaxblAmt { get; set; }  // Total Supply Price
		public double totTaxAmt { get; set; }  // Total VAT
		public double totAmt { get; set; }  // Total Amount
		public string remark { get; set; }  // Remark
		public List<StockIoItem> itemList { get; set; }  // Item List
	}

	public class StockIoItem {
		public int itemSeq { get; set; }  // Item Sequence
		public string itemCd { get; set; }  // Item Code
		public string itemClsCd { get; set; }  // Item Class Code
		public string itemNm { get; set; }  // Item Name
		public string bcd { get; set; }  // Barcode
		public string pkgUnitCd { get; set; }  // Package unit code
		public double pkg { get; set; }  // Package Quantity
		public string qtyUnitCd { get; set; }  // Unit Quantity Code
		public double qty { get; set; }  // Unit Quantity
		public string itemExprDt { get; set; }  // ItemExprDt
		public double prc { get; set; }  // Unit Price
		public double splyAmt { get; set; }  // Supply Amount
		public double totDcAmt { get; set; }  // Discount Rate
		public double taxblAmt { get; set; }  // Taxable Amount
		public string taxTyCd { get; set; }  // Taxation Type Code
		public double taxAmt { get; set; }  // Tax Amount
		public double totAmt { get; set; }  // Total Amount
	}
}

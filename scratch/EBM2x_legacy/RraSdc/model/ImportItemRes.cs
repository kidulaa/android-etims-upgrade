using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class ImportItemRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public ImportItemResData data { get; set; }  // Result Date
    }
	public class ImportItemResData {
		public List<ImportItem> itemList { get; set; }  // Item List
	}

	public class ImportItem {
		public string taskCd { get; set; }  // Task Code
		public string dclDe { get; set; }  // Declaration Date
		public int itemSeq { get; set; }  // Item Sequence
		public string dclNo { get; set; }  // Declaration Number
		public string hsCd { get; set; }  // HS Code
		public string itemNm { get; set; }  // Item Name
		public string imptItemsttsCd { get; set; }  // Import Item Status Code
		public string orgnNatCd { get; set; }  // Origin Nation Code
		public string exptNatCd { get; set; }  // Export Nation Code
		public double pkg { get; set; }  // Package
		public string pkgUnitCd { get; set; }  // Packaging Unit Code
		public double qty { get; set; }  // Quantity
		public string qtyUnitCd { get; set; }  // Quantity Unit Code
		public double totWt { get; set; }  // Gross Weight
		public double netWt { get; set; }  // Net Weight
		public string spplrNm { get; set; }  // Supplier's name
		public string agntNm { get; set; }  // Agent name
		public double invcFcurAmt { get; set; }  // Invoice Foreign Currency Amount
		public string invcFcurCd { get; set; }  // Invoice Foreign Currency
		public double invcFcurExcrt { get; set; }  // Invoice Foreign Currency Rate
	}
}

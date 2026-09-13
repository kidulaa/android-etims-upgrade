using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class StockMasterRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public StockMasterData data { get; set; }  // Result Date
    }
	public class StockMasterData {
		public List<StockMaster> itemList { get; set; }  // Item List
	}

	public class StockMaster {
		public string bhfId { get; set; }  // Branch ID
		public string itemCd { get; set; }  // Item Code
		public double rsdQty { get; set; }  // Remain Quantity
	}
}

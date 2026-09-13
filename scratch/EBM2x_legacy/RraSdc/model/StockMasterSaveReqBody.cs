using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class StockMasterSaveReqBody {
	    public string itemCd { get; set; }  // Item Code
	    public double rsdQty { get; set; }  // Remain Quantity
    }
}

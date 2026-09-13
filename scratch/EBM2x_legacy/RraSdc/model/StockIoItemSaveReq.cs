using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class StockIoItemSaveReq {
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
	    public double totDcAmt { get; set; }  // Discount Amount
	    public double taxblAmt { get; set; }  // Taxable Amount
	    public string taxTyCd { get; set; }  // Taxation Type Code
	    public double taxAmt { get; set; }  // Tax Amount
	    public double totAmt { get; set; }  // Total Amount
    }
}

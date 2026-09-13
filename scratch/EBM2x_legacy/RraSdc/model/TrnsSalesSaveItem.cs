using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsSalesSaveItem {
	    public int itemSeq { get; set; }  // Item Sequence Number
	    public string itemCd { get; set; }  // Item Code
	    public string itemClsCd { get; set; }  // Item Classification Code
	    public string itemNm { get; set; }  // Item Name
	    public string bcd { get; set; }  // Bar code
	    public string pkgUnitCd { get; set; }  // Packaging Unit Code
	    public double pkg { get; set; }  // Package
	    public string qtyUnitCd { get; set; }  // Quantity Unit Code
	    public double qty { get; set; }  // Quantity
	    public double prc { get; set; }  // Unit Price
	    public double splyAmt { get; set; }  // Supply Amount
	    public double dcRt { get; set; }  // Discount Rate
	    public double dcAmt { get; set; }  // Discount Amount
	    public string isrccCd { get; set; }  // Insurance Company Code
	    public string isrccNm { get; set; }  // Insurance Company Name
	    public double isrcRt { get; set; }  // Insurance Rate
	    public double isrcAmt { get; set; }  // Insurance Amount
	    public string taxTyCd { get; set; }  // Taxation Type Code
	    public double taxblAmt { get; set; }  // Taxable Amount
	    public double taxAmt { get; set; }  // Tax Amount
	    public double totAmt { get; set; }  // Total Amount
    }
}

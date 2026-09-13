using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsPurchaseSaveItem {
	    public int itemSeq { get; set; }  // Item Sequence Number
	    public string itemCd { get; set; }  // Item Code
	    public string itemClsCd { get; set; }  // Item Classification Code
	    public string itemNm { get; set; }  // Item Name
	    public string bcd { get; set; }  // Barcode
	    public string spplrItemClsCd { get; set; }  // Supplier Item Classification Code
	    public string spplrItemCd { get; set; }  // Supplier Item Code
	    public string spplrItemNm { get; set; }  // Supplier Item Name
	    public string pkgUnitCd { get; set; }  // Packaging Unit Code
	    public double pkg { get; set; }  // Package
	    public string qtyUnitCd { get; set; }  // Quantity Unit Code
	    public double qty { get; set; }  // Quantity
	    public double prc { get; set; }  // Unit Price
	    public double splyAmt { get; set; }  // Supply Price
	    public double dcRt { get; set; }  // Discount Rate
	    public double dcAmt { get; set; }  // Discount Amount
	    public double taxblAmt { get; set; }  // Taxable Amount
	    public string taxTyCd { get; set; }  // Taxation Type Code
	    public double taxAmt { get; set; }  // Tax Amount
	    public double totAmt { get; set; }  // Total Amount
	    public string itemExprDt { get; set; }  // Item Expiration Date
    }
}

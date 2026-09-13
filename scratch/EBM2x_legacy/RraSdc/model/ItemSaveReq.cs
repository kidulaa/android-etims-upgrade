using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class ItemSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public string itemCd { get; set; }  // Item Code
	    public string itemClsCd { get; set; }  // Item Classification Code
	    public string itemTyCd { get; set; }  // item Type Code
	    public string itemNm { get; set; }  // item Name
	    public string itemStdNm { get; set; }  // Item Standard Name
	    public string orgnNatCd { get; set; }  // Origin Place Code (Nation)
	    public string pkgUnitCd { get; set; }  // Packaging Unit Code
	    public string qtyUnitCd { get; set; }  // Quantity Unit Code
	    public string taxTyCd { get; set; }  // Taxation Type Code
	    public string btchNo { get; set; }  // Batch Number
	    public string bcd { get; set; }  // Barcode
	    public double dftPrc { get; set; }  // Default Unit Price
	    public double grpPrcL1 { get; set; }  // Group1 Unit Price
	    public double grpPrcL2 { get; set; }  // Group2 Unit Price
	    public double grpPrcL3 { get; set; }  // Group3 Unit Price
	    public double grpPrcL4 { get; set; }  // Group4 Unit Price
	    public double grpPrcL5 { get; set; }  // Group5 Unit Price
	    public string addInfo { get; set; }  // Add Information
	    public double sftyQty { get; set; }  // Safty Quantity
	    public string useYn { get; set; }  // Used / UnUsed
	    public string regBhfId { get; set; }  // Registrant Branch ID
        public string rraModYn { get; set; }  //RraModYn
        public string isrcAplcbYn { get; set; }  //IsrcAplcbYn
        public string batchNum { get; set; }  // BatchNum
        public string regrId { get; set; }  //   등록자 아이디
        public string regrNm { get; set; }  //    등록장 명
        public string modrId { get; set; }  //   수정자 아이디
        public string modrNm { get; set; }  //    수정자 명

    }
}

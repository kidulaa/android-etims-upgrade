using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsPurchaseSaveReqBody {
	    public string spplrTin { get; set; }  // Supplier TIN
	    public int invcNo { get; set; }  // Invoice Number
	    public int orgInvcNo { get; set; }  // Original Invoice Number
	    public string taxprNm { get; set; }  // Taxpayer Name
	    public string dvcId { get; set; }  // DVC ID
	    public string spplrBhfId { get; set; }  // Customer Branch ID
	    public string spplrNm { get; set; }  // Supplier Name
	    public string spplrDvcId { get; set; }  // Supplier DVC ID
	    public int spplrRcptNo { get; set; }  // Supplier Receipt Number
	    public string regTyCd { get; set; }  // Registration Type Code
	    public string pchsTyCd { get; set; }  // Purchase Type Code
	    public string rcptTyCd { get; set; }  // Receipt Type Code
	    public string pmtTyCd { get; set; }  // Payment Type Code
	    public string pchsSttsCd { get; set; }  // Purchase Status Code
	    public string cfmDt { get; set; }  // Validated Date
	    public string pchsDt { get; set; }  // Purchase Date
	    public string wrhsDt { get; set; }  // Warehousing Date
	    public string cnclReqDt { get; set; }  // Cancel Requested Date
	    public string cnclDt { get; set; }  // Canceled Date
	    public string rfdDt { get; set; }  // Refunded Date
	    public int totItemCnt { get; set; }  // Total Item Count
	    public int taxblAmtA { get; set; }  // Taxable Amount A
	    public int taxblAmtB { get; set; }  // Taxable Amount B
	    public int taxblAmtC { get; set; }  // Taxable Amount C
	    public int taxblAmtD { get; set; }  // Taxable Amount D
		public int taxblAmtE { get; set; }  // Taxable Amount E
		public int taxRtA { get; set; }  // Tax Rate A
	    public int taxRtB { get; set; }  // Tax Rate B
	    public int taxRtC { get; set; }  // Tax Rate C
	    public int taxRtD { get; set; }  // Tax Rate D
		public int taxRtE { get; set; }  // Tax Rate E
		public int taxAmtA { get; set; }  // Tax Amount A
	    public int taxAmtB { get; set; }  // Tax Amount B
	    public int taxAmtC { get; set; }  // Tax Amount C
	    public int taxAmtD { get; set; }  // Tax Amount D
		public int taxAmtE { get; set; }  // Tax Amount E
		public int totTaxblAmt { get; set; }  // Total Taxable Amount
	    public int totTaxAmt { get; set; }  // Total Tax Amount
	    public int totAmt { get; set; }  // Total Amount
	    public string remark { get; set; }  // Remark
	    public List<TrnsPurchaseSaveItem> itemList { get; set; }  // Item List
    }
}

using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsPurchaseRes {
	    public string resultCd { get; set; }  // Result Code
	    public string resultMsg { get; set; }  // Result Message
	    public string resultDt { get; set; }  // Result Date
	    public TrnsPurchaseResData data { get; set; }  // Result Date
    }
	public class TrnsPurchaseResData {
		public List<TrnsPurchase> pchsList { get; set; }  // Purchase List
	}

	public class TrnsPurchase {
		public string spplrTin { get; set; }   
		public long invcNo { get; set; }   
		public long orgInvcNo { get; set; }   
		public string spplrBhfId { get; set; }  
		public string spplrNm { get; set; }   
		public int spplrInvcNo { get; set; }   
		public string regTyCd { get; set; }   
		public string pchsTyCd { get; set; }   
		public string rcptTyCd { get; set; }   
		public string pmtTyCd { get; set; }  
		public string pchsSttsCd { get; set; }   
		public string cfmDt { get; set; }   
		public string pchsDt { get; set; }  
		public string wrhsDt { get; set; }   
		public string cnclReqDt { get; set; }   
		public string cnclDt { get; set; }   
		public string rfdDt { get; set; }   
		public double totItemCnt { get; set; }   
		public double taxblAmtA { get; set; }   
		public double taxblAmtB { get; set; }   
		public double taxblAmtC { get; set; }   
		public double taxblAmtD { get; set; }
		public double taxblAmtE { get; set; }
		public int taxRtA { get; set; }   
		public int taxRtB { get; set; }   
		public int taxRtC { get; set; }   
		public int taxRtD { get; set; }
		public int taxRtE { get; set; }
		public double taxAmtA { get; set; }   
		public double taxAmtB { get; set; }   
		public double taxAmtC { get; set; }   
		public double taxAmtD { get; set; }
		public double taxAmtE { get; set; }
		public double totTaxblAmt { get; set; } 
		public double totTaxAmt { get; set; } 
		public double totAmt { get; set; }   
		public string remark { get; set; }  
		public List<TrnsPurchaseItem> itemList { get; set; }  
	}

	public class TrnsPurchaseItem {
		public int itemSeq { get; set; }  // 
		public string itemCd { get; set; }  // 
		public string itemClsCd { get; set; }  // 
		public string itemNm { get; set; }  // 
		public string bcd { get; set; }  // 
		public string spplrItemClsCd { get; set; }  // 
		public string spplrItemCd { get; set; }  // 
		public string spplrItemNm { get; set; }  // 
		public string pkgUnitCd { get; set; }  // 
		public double pkg { get; set; }  // 
		public string qtyUnitCd { get; set; }  // 
		public double qty { get; set; }  // 
		public double prc { get; set; }  // 
		public double splyAmt { get; set; }  // 
		public double dcRt { get; set; }  // 
		public double dcAmt { get; set; }  // 
		public double taxblAmt { get; set; }  // 
		public string taxTyCd { get; set; }  // 
		public double taxAmt { get; set; }  // 
		public double totAmt { get; set; }  // 
		public string itemExprDt { get; set; }  // 
	}
}

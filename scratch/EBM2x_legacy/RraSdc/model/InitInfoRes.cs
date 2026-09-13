using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class InitInfoRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public InitInfoResData data { get; set; }  // Result Date
    }
	public class InitInfoResData {
		public InitInfoVO info { get; set; }  // Initialize Information
	}

	public class InitInfoVO {
		public string tin { get; set; }  // TIN
		public string taxprNm { get; set; }  // Taxpayer Name
		public string bsnsActv { get; set; }  // Business Activity
		public string bhfId { get; set; }  // Branch ID
		public string bhfNm { get; set; }  // Branch Name
		public string prvncNm { get; set; }  // Province Name
		public string dstrtNm { get; set; }  // District Name
		public string sctrNm { get; set; }  // Sector Name
		public string locDesc { get; set; }  // Location Description
		public string hqYn { get; set; }  // Head Quarter Yes/No
		public string mgrNm { get; set; }  // Manager Name
		public string mgrTelNo { get; set; }  // Manager Contact
		public string mgrEmail { get; set; }  // Manager Email
        public string dvcId { get; set; }  // Device ID
        public string sdcId { get; set; }  // Sdc ID
        public string mrcNo { get; set; }  // Mrc No
		public string intrlKey { get; set; }  // Internal Key
		public string signKey { get; set; }  // Signature Key
		public string cmcKey { get; set; }  // Communication Key
        public long lastSaleInvcNo { get; set; }  // lastSaleInvcNo
        public long lastPchsInvcNo { get; set; }  // lastPchsInvcNo
        public long lastRcptNo { get; set; }  // lastRcptNo
        public long lastZreportRptNo { get; set; }  // lastZreportRptNo
        public string vatTyCd { get; set; }  // Business Activity (1: VAT,2 : NON-VAT) //2021.02.02 Ãß°¡
    }
}

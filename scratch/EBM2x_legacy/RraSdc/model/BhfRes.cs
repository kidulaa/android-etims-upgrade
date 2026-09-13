using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public BhfResData data { get; set; }  // Result Date
    }
	public class BhfResData
    {
		public List<Bhf> bhfList { get; set; }  // Bhf List
	}

	public class Bhf {
        public string tin { get; set; }  // Branch ID
        public string bhfId { get; set; }  // Branch ID
		public string bhfNm { get; set; }  // Branch Name
        public string bhfSttsCd { get; set; }  // BhfSttsCd
        public string prvncNm { get; set; }  // Province Name
		public string dstrtNm { get; set; }  // District Name
		public string sctrNm { get; set; }  // Sector Name
		public string locDesc { get; set; }  // Location Description
		public string mgrNm { get; set; }  // Manager Name
		public string mgrTelNo { get; set; }  // Manager Contact
		public string mgrEmail { get; set; }  // Manager Email
		public string hqYn { get; set; }  // Head Quarter Yes/No
	}
}

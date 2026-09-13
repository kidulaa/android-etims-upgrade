using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class CustomerTinRes
    {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public CustomerTinResData data { get; set; }  // Result Date
    }
	public class CustomerTinResData
    {
		public List<CustomerTin> custList { get; set; }  // cust List
	}

	public class CustomerTin
    {
        public string tin { get; set; }  // tin ID
		public string taxprNm { get; set; }  // taxpr Name
        public string taxprSttsCd { get; set; }  // taxprSttsCd
        public string prvncNm { get; set; }  // Province Name
		public string dstrtNm { get; set; }  // District Name
		public string sctrNm { get; set; }  // Sector Name
		public string locDesc { get; set; }  // Location Description
	}
}

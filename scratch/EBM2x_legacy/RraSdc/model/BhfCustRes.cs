using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfCustRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public CustResData data { get; set; }  // Result Date
    }
	
    public class CustResData {
		public Taxpayer taxpayer { get; set; }  // Bhf List
	}

	public class Taxpayer {
		public string tin { get; set; }  // TIN
		public string taxprNm { get; set; }  // Taxpayer Name
		public List<Bhf> bhfList { get; set; }  // Bhf List
	}
}

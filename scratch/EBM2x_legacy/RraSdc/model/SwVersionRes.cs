using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class SwVersionRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public SwVersionResData data { get; set; }  // Result Date
    }
	public class SwVersionResData {
		public long 	swNo { get; set; }  // Software Number
		public string 	ver { get; set; }  // Software Version
		public string 	dnUrl { get; set; }  // Software Update URL
		public long	fileSize { get; set; }  // Software Version File Size
		public string	rlsDt { get; set; }  // Software Release Date
	}
}

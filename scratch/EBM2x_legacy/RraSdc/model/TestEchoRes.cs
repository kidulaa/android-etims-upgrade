using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TestEchoRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public TestEchoResData data { get; set; }  // Result Date
    }
	public class TestEchoResData {
		public string testStr { get; set; }  // Code Class List
	}
}

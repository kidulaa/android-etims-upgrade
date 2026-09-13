using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class NoticeRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public NoticeResData data { get; set; }  // Result Date
    }
	public class NoticeResData {
		public List<Notice> noticeList { get; set; }  // Notice List
	}

	public class Notice {
		public int noticeNo { get; set; }  // Notice Number
		public string title { get; set; }  // Title
		public string cont { get; set; }  // Content
		public string dtlUrl { get; set; }  // Notice Detail URL
		public string regrNm { get; set; }  // Registrator Name
		public string regDt { get; set; }  // Registrator Date
	}
}

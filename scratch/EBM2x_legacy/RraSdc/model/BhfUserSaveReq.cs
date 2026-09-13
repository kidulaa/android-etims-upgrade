using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfUserSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public string userId { get; set; }  // User ID
	    public string userNm { get; set; }  // User Name
	    public string pwd { get; set; }  // Password
	    public string adrs { get; set; }  // Address
	    public string cntc { get; set; }  // Contact
	    public string authCd { get; set; }  // Authority Code
	    public string remark { get; set; }  // Remark
	    public string useYn { get; set; }  // Used / UnUsed
        public string regrId { get; set; }  //   등록자 아이디
        public string regrNm { get; set; }  //    등록장 명
        public string modrId { get; set; }  //   수정자 아이디
        public string modrNm { get; set; }  //    수정자 명
    }
}

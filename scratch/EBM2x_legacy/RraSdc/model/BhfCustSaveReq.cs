using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfCustSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public string custNo { get; set; }  // Customer Number
	    public string custTin { get; set; }  // Customer TIN
	    public string custNid { get; set; }  // Customer National ID
	    public string custNm { get; set; }  // Customer Name
	    public string custBhfId { get; set; }  // Customer Branch Office ID
	    public string adrs { get; set; }  // Address
	    public string telNo { get; set; }  // Contact
        public string email { get; set; }  // 이메일
        public string faxNo { get; set; }  // 팩스 번호
        public string useYn { get; set; }  // 사용 여부
        public string remark { get; set; }  //  비고

        public string regrId { get; set; }  //   등록자 아이디
        public string regrNm { get; set; }  //    등록장 명
        public string modrId { get; set; }  //   수정자 아이디
        public string modrNm { get; set; }  //    수정자 명

    }
}

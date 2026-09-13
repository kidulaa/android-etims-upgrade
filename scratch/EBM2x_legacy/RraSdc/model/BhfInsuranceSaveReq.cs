using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfInsuranceSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public string isrccCd { get; set; }  // Insurance Code
	    public string isrccNm { get; set; }  // Insurance Name
	    public int isrcRt { get; set; }  // Premium Rate
	    public string useYn { get; set; }  // Used / UnUsed
        public string regrId { get; set; }  //   등록자 아이디
        public string regrNm { get; set; }  //    등록장 명
        public string modrId { get; set; }  //   수정자 아이디
        public string modrNm { get; set; }  //    수정자 명
    }
}

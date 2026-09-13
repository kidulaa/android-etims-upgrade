using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class CodeRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public CodeResData data { get; set; }  // Result Date
    }
	public class CodeResData {
		public List<CodeClassLVO> clsList { get; set; }  // Code Class List
	}

	public class CodeClassLVO {
		public string	cdCls { get; set; }  // Code Class
		public string	cdClsNm { get; set; }  // Code Class Name
		public string	cdClsDesc { get; set; }  // Code Class Description
		public string	useYn { get; set; }  // Code Class Used/Unused
		public string	userDfnNm1 { get; set; }  // User Define Name 1
		public string	userDfnNm2 { get; set; }  // User Define Name 2
		public string	userDfnNm3 { get; set; }  // User Define Name 3
		public List<CodeDtlLVO> dtlList { get; set; }
    }

	public class CodeDtlLVO {
		public string	cd { get; set; }  // Code
		public string	cdNm { get; set; }  // Code Name
		public string	cdDesc { get; set; }  // Code Description
		public string	useYn { get; set; }  // Code Used/Unused
		public int	srtOrd { get; set; }  // Sort Order
		public string	userDfnCd1 { get; set; }  // User Define Code 1
		public string	userDfnCd2 { get; set; }  // User Define Code 2
		public string	userDfnCd3 { get; set; }  // User Define Code 3
	}
}

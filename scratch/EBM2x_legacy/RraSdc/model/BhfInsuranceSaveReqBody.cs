using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfInsuranceSaveReqBody {
	    public string isrccCd { get; set; }  // Insurance Code
	    public string isrccNm { get; set; }  // Insurance Name
	    public string isrcRt { get; set; }  // Premium Rate
	    public string useYn { get; set; }  // Used / UnUsed
    }
}

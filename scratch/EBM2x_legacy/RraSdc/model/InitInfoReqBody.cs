using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class InitInfoReqBody {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public string dvcSrlNo { get; set; }  // Device Serial Number
    }
}

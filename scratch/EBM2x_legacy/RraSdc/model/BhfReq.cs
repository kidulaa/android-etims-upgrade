using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public string lastReqDt { get; set; }  // Last Request Dt
    }
}

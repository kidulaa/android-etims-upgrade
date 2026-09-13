using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class ImportItemUpdateRes {
	    public string resultCd { get; set; }  // Result Code
	    public string resultMsg { get; set; }  // Result Message
	    public string resultDt { get; set; }  // Result Date
	    public object	data { get; set; }  // Result Data
    }
}

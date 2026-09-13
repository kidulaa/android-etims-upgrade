using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class BhfCustSaveReqBody {
	    public int custNo { get; set; }  // Customer Number
	    public string custTin { get; set; }  // Customer TIN
	    public string custNid { get; set; }  // Customer National ID
	    public string custNm { get; set; }  // Customer Name
	    public string custBhfId { get; set; }  // Customer Branch Office ID
	    public string adrs { get; set; }  // Address
	    public string telNo { get; set; }  // Contact
    }
}

using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsSalesRcptSaveReqBody {
	    public long invcNo { get; set; }  // Invoice Number
	    public long orgInvcNo { get; set; }  // Original Invoice Number
	    public long curRcptNo { get; set; }  // Current Receipt Number
	    public long totRcptNo { get; set; }  // Total Receipt Number
	    public string rcptPbctDt { get; set; }  // Receipt Published Date
	    public string intrlData { get; set; }  // Internal Data
	    public string rcptSign { get; set; }  // Receipt Signature
	    public string jrnl { get; set; }  // Journal
	    public string trdeNm { get; set; }  // Trade name
	    public string adrs { get; set; }  // Address
	    public string topMsg { get; set; }  // TOp Message
	    public string btmMsg { get; set; }  // Bottom Message
	    public string prchrAcptcYn { get; set; }  // Purchase Accept Yes/No
    }
}

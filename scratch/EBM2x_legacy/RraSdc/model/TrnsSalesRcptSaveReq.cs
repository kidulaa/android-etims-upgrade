using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsSalesRcptSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public long invcNo { get; set; }  // Invoice Number
	    public long orgInvcNo { get; set; }  // Original Invoice Number
	    public int curRcptNo { get; set; }  // Current Receipt Number
	    public int totRcptNo { get; set; }  // Total Receipt Number

        public string custTin { get; set; }  // Receipt Published Date
        public string custMblNo { get; set; }  // Receipt Published Date
        
        public int rptNo { get; set; }  // rptNo

        public string rcptPbctDt { get; set; }  // Receipt Published Date
	    public string intrlData { get; set; }  // Internal Data
	    public string rcptSign { get; set; }  // Receipt Signature
	    public string jrnl { get; set; }  // Journal
	    public string trdeNm { get; set; }  // Trade name
	    public string adrs { get; set; }  // Address
	    public string topMsg { get; set; }  // TOp Message
	    public string btmMsg { get; set; }  // Bottom Message
	    public string prchrAcptcYn { get; set; }  // Purchase Accept Yes/No
        public string regrId { get; set; }  //   등록자 아이디
        public string regrNm { get; set; }  //    등록장 명
        public string modrId { get; set; }  //   수정자 아이디
        public string modrNm { get; set; }  //    수정자 명
    }
}

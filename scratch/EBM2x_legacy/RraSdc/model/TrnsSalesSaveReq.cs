using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TrnsSalesSaveReq {
	    public string tin { get; set; }  // TIN
	    public string bhfId { get; set; }  // Branch ID
	    public long invcNo { get; set; }  // Invoice Number
	    public long orgInvcNo { get; set; }  // Original Invoice Number
	    //public long custNo { get; set; }  // Customer Number
	    public string custTin { get; set; }  // Customer TIN
	    //public string custBhfId { get; set; }  // Customer Branch ID
	    public string custNm { get; set; }  // Customer Name
	    public string salesTyCd { get; set; }  // Sales Type Code
	    public string rcptTyCd { get; set; }  // Receipt Type Code
	    public string pmtTyCd { get; set; }  // Payment Type Code
        public string rfdRsnCd { get; set; }  // Refund Reason Type Code       
        public string salesSttsCd { get; set; }  // Invoice Status Code
	    public string cfmDt { get; set; }  // Validated Date
	    public string salesDt { get; set; }  // Sale Date
	    public string stockRlsDt { get; set; }  // Stock Released Date
	    public string cnclReqDt { get; set; }  // Cancel Requested Date
	    public string cnclDt { get; set; }  // Canceled Date
	    public string rfdDt { get; set; }  // Refunded Date
	    public double totItemCnt { get; set; }  // Total Item Count
	    public double taxblAmtA { get; set; }  // Taxable Amount A
	    public double taxblAmtB { get; set; }  // Taxable Amount B
	    public double taxblAmtC { get; set; }  // Taxable Amount C
	    public double taxblAmtD { get; set; }  // Taxable Amount D
		public double taxblAmtE { get; set; }  // Taxable Amount E
		public int taxRtA { get; set; }  // Tax Rate A
	    public int taxRtB { get; set; }  // Tax Rate B
	    public int taxRtC { get; set; }  // Tax Rate C
	    public int taxRtD { get; set; }  // Tax Rate D
		public int taxRtE { get; set; }  // Tax Rate E
		public double taxAmtA { get; set; }  // Tax Amount A
	    public double taxAmtB { get; set; }  // Tax Amount B
	    public double taxAmtC { get; set; }  // Tax Amount C
	    public double taxAmtD { get; set; }  // Tax Amount D
		public double taxAmtE { get; set; }  // Tax Amount E
		public double totTaxblAmt { get; set; }  // Total Taxable Amount
	    public double totTaxAmt { get; set; }  // Total Tax Amount
	    public double totAmt { get; set; }  // Total Amount
	    public string prchrAcptcYn { get; set; }  // Purchase Accept Yes/No
	    public string remark { get; set; }  // remark
        public string regrId { get; set; }  //   ����� ���̵�
        public string regrNm { get; set; }  //    ����� ��
        public string modrId { get; set; }  //   ������ ���̵�
        public string modrNm { get; set; }  //    ������ ��
        public List<TrnsSalesSaveItem> itemList { get; set; }  // Item List
    }
}

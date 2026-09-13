using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of SalesReportRecord.
    /// </summary>
    public class SalesReportRecord
    {
        public string ItemCd { get; set; }
        public string ItemNm { get; set; }
        public double Qty { get; set; }
        public double Prc { get; set; }
        public string CustTin { get; set; }             // Customer Taxpayer Identification Number(TIN)
        public long InvcNo { get; set; }                // Invoice No.
        public string SdcId { get; set; }               // SdcId
        public long CurRcptNo { get; set; }             // Current Receipt No. 
        public string Oper { get; set; }
        public string SalesDt { get; set; }             // Sale Date
        public double VatAmt { get; set; }
        public double TotAmt { get; set; }
        public SalesReportRecord()
        {
        }
    }
}

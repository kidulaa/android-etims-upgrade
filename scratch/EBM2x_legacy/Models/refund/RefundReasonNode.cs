using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.refund
{
    public class RefundReasonNode
    {
        public string ReasonCode { get; set; }
        public string ReasonText { get; set; }
        public string OrgBarcodeNo { get; set; }
        public string OrgInvoiceNo { get; set; }

        public RefundReasonNode()
        {
            ReasonCode = string.Empty;
            ReasonText = string.Empty;
            OrgBarcodeNo = string.Empty;
            OrgInvoiceNo = string.Empty;
        }
        public RefundReasonNode(string reasonCode, string reasonText, string orgBarcodeNo, string orgInvoiceNo)
        {
            ReasonCode = reasonCode;
            ReasonText = reasonText;
            OrgBarcodeNo = orgBarcodeNo;
            OrgInvoiceNo = orgInvoiceNo;
        }
    }
}

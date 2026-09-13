using EBM2x.Models.journal;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.ListView
{
    public class SearchReceiptNode
    {
        public string SalesDate { get; set; }
        public string ReceiptNo { get; set; }
        public string SalesType { get; set; }
        public string InvoiceNum { get; set; }
        public double Amount { get; set; }
        public string SalesFilename { get; set; }
        public int Sign { get; set; } 
    }
}

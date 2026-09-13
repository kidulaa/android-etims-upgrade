using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.ListView
{
    public class SearchItemNode
    {
        public string ItemCode { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string BarchNumber { get; set; }
        public double Price { get; set; }
        public int StockQty { get; set; }
        public string ExpirationDate { get; set; }
    }
}

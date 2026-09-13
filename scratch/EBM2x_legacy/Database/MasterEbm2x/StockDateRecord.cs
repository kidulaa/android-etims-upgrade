using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of StockDateRecord.
    /// </summary>
    public class StockDateRecord
    {
        public string ItemCd { get; set; }
        public string ItemClsCd { get; set; }
        public string ItemNm { get; set; }
        public string ItemTyCd { get; set; }
        public double InitlQty { get; set; }
        public double InitlWhPrc { get; set; }
        public double Qty { get; set; }
        public double Prc { get; set; }
        public StockDateRecord()
        {
        }
    }
}

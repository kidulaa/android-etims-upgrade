using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class OpeningCloseingStockRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string BhfId { get; set; }               // Branch Office ID
        public string ItemCd { get; set; }              // Item Code
        public string ItemNm { get; set; }              // Item Name
        public double OpeningQty { get; set; }          // Quantity
        public double ClosingQty { get; set; }          // Quantity
        public double RsdQty { get; set; }              // Resodual Quantity
        public double Prc { get; set; }                 // Price
        public double TotAmt { get; set; }              // Total Amount

        public double SftyQty { get; set; }             // Safety Quantity
        public string TextColor { get; set; }           


        // sale status
        // JINIT_20191210, double로 변경, public string FinishedProduct { get; set; }    // FinishedProduct
        public double FinishedProduct { get; set; }    // FinishedProduct
        public double RawMaterial { get; set; }        // RawMaterial

        public string ExpirDt { get; set; }            // ExpirDt
        public double RdsQty { get; set; }              

        // stock in history
        public double PurchaseQty { get; set; }
        public double PurchasePrice { get; set; }
        public double PurchaseTotalAmount { get; set; }

        public double ImportationQty { get; set; }
        public double ImportationPrice { get; set; }
        public double ImportationTotalAmount { get; set; }

        public double AdjusmentInQty { get; set; }
        public double ProcessingInQty { get; set; }
        public double ShipmentReceivedQty { get; set; }


        // stock out history
        public double SalesQty { get; set; }
        public double SalesPrice { get; set; }
        public double SalesTotalAmount { get; set; }

        public double ShipmentOutQty { get; set; }
        public double AdjustmentOutQty { get; set; }
        public double DammagedExpiredQty { get; set; }
        public double ProcessingOutQty { get; set; }

        public OpeningCloseingStockRecord()
        {
            clear();
        }

        public void clear()
        {
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.BhfId = string.Empty;                  // Branch Office ID
            this.ItemCd = string.Empty;                 // Item Code
            this.ItemNm = string.Empty;                 // Item Name
            this.OpeningQty = 0;                        // Quantity
            this.ClosingQty = 0;                        // Quantity
            this.RsdQty = 0;                            // Resodual Quantity
            this.Prc = 0;                               // Price
            this.TotAmt = 0;                            // Total Amount

            this.SftyQty = 0;
            this.TextColor = "000000";
        }
    }
}

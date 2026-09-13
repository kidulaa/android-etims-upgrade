using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of ImportItemRecord.
    /// </summary>
    public class ImportItemRecord
    {
        public string TaskCd { get; set; }              // Operation Code
        public string DclDe { get; set; }               // Declaration Date
        public int ItemSeq { get; set; }                // Item Sequence
        public string DclNo { get; set; }               // Declaration Number
        public string ImptItemSttsCd { get; set; }      // Import Item Status Status Code
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string TaxprNm { get; set; }             // Taxpayer's Name
        public string ItemCd { get; set; }              // Item Code
        public string ItemClsCd { get; set; }           // Item Classification Code
        public string HsCd { get; set; }                // HS Code
        public string ItemNm { get; set; }              // ItemName
        public string OrgnNatCd { get; set; }           // Country Code of Origin
        public string ExptNatCd { get; set; }           // Country Code of Export
        public double Pkg { get; set; }                 // Packing
        public string PkgUnitCd { get; set; }           // Packing Unit Code
        public double Qty { get; set; }                 // Quantity
        public string QtyUnitCd { get; set; }           // Quantity Unit Code
        public double TotWt { get; set; }               // Gross Weight
        public double NetWt { get; set; }               // Net Weight
        public string SpplrNm { get; set; }             // Supplier's name
        public string AgntNm { get; set; }              // Agent's Name
        public double InvcFcurAmt { get; set; }         // Invoice Foreign Currency Amount
        public string InvcFcurCd { get; set; }          // Invoice Foreign Currency Code
        public double InvcFcurExcrt { get; set; }       // Invoice Foreign Currency Rate
        public string DclTaxofcCd { get; set; }         // Declaration Tax Office Code
        public double TrffAmt { get; set; }             // Tariff Amount
        public double VatAmt { get; set; }              // VAT
        public string Remark { get; set; }              // Remark
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date

        public string ImptItemSttsNm { get; set; }
        public string OrgnNatNm { get; set; }
        public string ExptNatNm { get; set; }
        public string QtyUnitNm { get; set; }
        public string InvcFcurNm { get; set; }
        public string ItemClsNm { get; set; }
        public string SupplierItemNm { get; set; }
        public string SupplierItemCd { get; set; }
        public string ProcessDate { get; set; }
        public string DclrNo { get; set; }
        public string AgentNm { get; set; }

        public ImportItemRecord()
        {
            clear();
        }

        public void clear()
        {
            this.TaskCd = string.Empty;                 // Operation Code
            this.DclDe = string.Empty;                  // Declaration Date
            this.ItemSeq = 0;                           // Item Sequence
            this.DclNo = string.Empty;                  // Declaration Number
            this.ImptItemSttsCd = string.Empty;         // Import Item Status Status Code
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.TaxprNm = string.Empty;                // Taxpayer's Name
            this.ItemCd = string.Empty;                 // Item Code
            this.ItemClsCd = string.Empty;              // Item Classification Code
            this.HsCd = string.Empty;                   // HS Code
            this.ItemNm = string.Empty;                 // ItemName
            this.OrgnNatCd = string.Empty;              // Country Code of Origin
            this.ExptNatCd = string.Empty;              // Country Code of Export
            this.Pkg = 0;                               // Packing
            this.PkgUnitCd = string.Empty;              // Packing Unit Code
            this.Qty = 0;                               // Quantity
            this.QtyUnitCd = string.Empty;              // Quantity Unit Code
            this.TotWt = 0;                             // Gross Weight
            this.NetWt = 0;                             // Net Weight
            this.SpplrNm = string.Empty;                // Supplier's name
            this.AgntNm = string.Empty;                 // Agent's Name
            this.InvcFcurAmt = 0;                       // Invoice Foreign Currency Amount
            this.InvcFcurCd = string.Empty;             // Invoice Foreign Currency Code
            this.InvcFcurExcrt = 0;                     // Invoice Foreign Currency Rate
            this.DclTaxofcCd = string.Empty;            // Declaration Tax Office Code
            this.TrffAmt = 0;                           // Tariff Amount
            this.VatAmt = 0;                            // VAT
            this.Remark = string.Empty;                 // Remark
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss"); ;                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

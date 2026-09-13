using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of TaxpayerItemRecord.
    /// </summary>
    public class TaxpayerItemRecord
    {
        public string Tin { get; set; }                 // Taxpayer Identification Number(TIN)
        public string ItemCd { get; set; }              // Item Code
        public string ItemClsCd { get; set; }           // Item Classification Code (RRA)
        public string ItemTyCd { get; set; }            // Item Type Code
        public string ItemNm { get; set; }              // Item Name
        public string ItemStdNm { get; set; }           // Item Stand Name
        public string OrgnNatCd { get; set; }           // Origin National Code
        public string PkgUnitCd { get; set; }           // Package Unit Code
        public string QtyUnitCd { get; set; }           // Quantity Unit Code
        public string TaxTyCd { get; set; }             // Taxation Type Code
        public string Bcd { get; set; }                 // Barcode
        public string RegBhfId { get; set; }            // Branch Office ID
        public string UseYn { get; set; }               // Use(Y/N)
        public string RraModYn { get; set; }            // RRA Modified(Y/N)
        public string AddInfo { get; set; }             // Additional Information
        public double SftyQty { get; set; }             // Safety Quantity
        public string IsrcAplcbYn { get; set; }         // Insurance Appicable(Y/N)
        public double DftPrc { get; set; }              // Default Price
        public double GrpPrcL1 { get; set; }            // Group Default Price L1
        public double GrpPrcL2 { get; set; }            // Group Default Price L2
        public double GrpPrcL3 { get; set; }            // Group Default Price L3
        public double GrpPrcL4 { get; set; }            // Group Default Price L4
        public double GrpPrcL5 { get; set; }            // Group Default Price L5
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }             // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }             // Modified Date

        public double InitlWhUntpc { get; set; }        // 초기 입고단가
        public double InitlQty { get; set; }            // 초기 입고수량
        public string Rm { get; set; }                  // 비고
        public string UseBarcode { get; set; }          // 바코드사용여부
        public string UseAdiYn { get; set; }            // 부가정보사용여부
        
        // 2019.09.29 르완다에서 추가
        public string BatchNum { get; set; }            // Batch number
        //JCNA 202001 DELETE
        //public string ExpirationDtUse { get; set; }     // Expiration Date Use
        //public DateTime ExpirationDt { get; set; }      // Expiration Date

        public string OrgnNatName { get; set; }           // Origin National Namee
        public string ItemTyName { get; set; }            // Item Type Name
        public string PkgUnitName { get; set; }           // Package Unit Name
        public string QtyUnitName { get; set; }           // Quantity Unit Name
        public string TaxTyName { get; set; }             // Taxation Type Name
        public string ItemClsName { get; set; }           // Item Classification Name (RRA)
        public double RdsQty { get; set; }                // 현재고 수량

        public double InputQty { get; set; }              // 등록 수량
        public double AfterQty { get; set; }              // 잔여 수량

        public string UseExpiration { get; set; }          // 만료일자사용여부
        public string ExpirationDate { get; set; }          // 만료일자


        public TaxpayerItemRecord()
		{
			clear();
		}

		public void clear()
		{
            this.Tin = string.Empty;                    // Taxpayer Identification Number(TIN)
            this.ItemCd = string.Empty;                 // Item Code
            this.ItemClsCd = string.Empty;              // Item Classification Code (RRA)
            this.ItemTyCd = string.Empty;               // Item Type Code
            this.ItemNm = string.Empty;                 // Item Name
            this.ItemStdNm = string.Empty;              // Item Stand Name
            this.OrgnNatCd = string.Empty;              // Origin National Code
            this.PkgUnitCd = string.Empty;              // Package Unit Code
            this.QtyUnitCd = string.Empty;              // Quantity Unit Code
            this.TaxTyCd = string.Empty;                // Taxation Type Code
            this.Bcd = string.Empty;                    // Barcode
            this.RegBhfId = string.Empty;               // Branch Office ID
            this.UseYn = "Y";                           // Use(Y/N)
            this.RraModYn = string.Empty;               // RRA Modified(Y/N)
            this.AddInfo = string.Empty;                // Additional Information
            this.SftyQty = 0d;                          // Safety Quantity
            this.IsrcAplcbYn = "N";                     // Insurance Appicable(Y/N)
            this.DftPrc = 0d;                           // Default Price
            this.GrpPrcL1 = 0d;                         // Group Default Price L1
            this.GrpPrcL2 = 0d;                         // Group Default Price L2
            this.GrpPrcL3 = 0d;                         // Group Default Price L3
            this.GrpPrcL4 = 0d;                         // Group Default Price L4
            this.GrpPrcL5 = 0d;                         // Group Default Price L5
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date

            this.InitlWhUntpc = 0;                      // 초기 입고단가
            this.InitlQty = 0;                          // 초기 입고수량
            this.Rm = string.Empty;                     // 비고
            this.UseBarcode = "N";                      // 바코드사용여부
            this.UseAdiYn = "Y";                        // 부가정보사용여부

            this.BatchNum = string.Empty;               // Batch number
            //JCNA 202001 DELETE
            //this.ExpirationDtUse = "N";               // Expiration Date Use
            //this.ExpirationDt = DateTime.Now;         // Expiration Date

            this.RdsQty = 0;                            // 현재고 수량
            this.ItemClsName = "";
            this.OrgnNatName = "";

            this.UseExpiration = "N";                   // 만료일자사용여부
            this.ExpirationDate = "";                   // 만료일자
        }
        public void UpdateNull()
        {
            if (string.IsNullOrEmpty(this.Tin)) this.Tin = "";                    // Taxpayer Identification Number(TIN)
            if (string.IsNullOrEmpty(this.ItemCd)) this.ItemCd = "";                 // Item Code
            if (string.IsNullOrEmpty(this.ItemClsCd)) this.ItemClsCd = "";              // Item Classification Code (RRA)
            if (string.IsNullOrEmpty(this.ItemTyCd)) this.ItemTyCd = "";               // Item Type Code
            if (string.IsNullOrEmpty(this.ItemNm)) this.ItemNm = "";                 // Item Name
            if (string.IsNullOrEmpty(this.ItemStdNm)) this.ItemStdNm = "";              // Item Stand Name
            if (string.IsNullOrEmpty(this.OrgnNatCd)) this.OrgnNatCd = "";              // Origin National Code
            if (string.IsNullOrEmpty(this.PkgUnitCd)) this.PkgUnitCd = "";              // Package Unit Code
            if (string.IsNullOrEmpty(this.QtyUnitCd)) this.QtyUnitCd = "";              // Quantity Unit Code
            if (string.IsNullOrEmpty(this.TaxTyCd)) this.TaxTyCd = "";                // Taxation Type Code
            if (string.IsNullOrEmpty(this.Bcd)) this.Bcd = "";                    // Barcode
            if (string.IsNullOrEmpty(this.RegBhfId)) this.RegBhfId = "";               // Branch Office ID
            if (string.IsNullOrEmpty(this.UseYn)) this.UseYn = "Y";                  // Use(Y/N)
            if (string.IsNullOrEmpty(this.RraModYn)) this.RraModYn = "";               // RRA Modified(Y/N)
            if (string.IsNullOrEmpty(this.AddInfo)) this.AddInfo = "";                // Additional Information
            //this.SftyQty = 0d;                          // Safety Quantity
            if (string.IsNullOrEmpty(this.IsrcAplcbYn)) this.IsrcAplcbYn = "N";            // Insurance Appicable(Y/N)
            //this.DftPrc = 0d;                           // Default Price
            //this.GrpPrcL1 = 0d;                         // Group Default Price L1
            //this.GrpPrcL2 = 0d;                         // Group Default Price L2
            //this.GrpPrcL3 = 0d;                         // Group Default Price L3
            //this.GrpPrcL4 = 0d;                         // Group Default Price L4
            //this.GrpPrcL5 = 0d;                         // Group Default Price L5
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date

            //this.InitlWhUntpc = 0;                      // 초기 입고단가
            //this.InitlQty = 0;                          // 초기 입고수량
            if (string.IsNullOrEmpty(this.Rm)) this.Rm = "";                     // 비고
            if (string.IsNullOrEmpty(this.UseBarcode)) this.UseBarcode = "N";             // 바코드사용여부
            if (string.IsNullOrEmpty(this.UseAdiYn)) this.UseAdiYn = "Y";               // 부가정보사용여부

            if (string.IsNullOrEmpty(this.BatchNum)) this.BatchNum = "";               // Batch number
                                                                                       //JCNA 202001 DELETE
                                                                                       //if (string.IsNullOrEmpty(this.ExpirationDtUse)) this.ExpirationDtUse = "N";                 // Expiration Date Use
                                                                                       //this.ExpirationDt = DateTime.Now;           // Expiration Date

            //this.RdsQty = 0;                            // 현재고 수량
            if (string.IsNullOrEmpty(this.UseExpiration)) this.UseExpiration = "N";    // 만료일자사용여부
            if (string.IsNullOrEmpty(this.ExpirationDate)) this.ExpirationDate = "";   // 만료일자
        }
    }
}

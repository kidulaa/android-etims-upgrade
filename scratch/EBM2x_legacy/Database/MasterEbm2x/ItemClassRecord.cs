using EBM2x.Database.Master;
using System;

namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of ItemClassRecord.
    /// </summary>
    public class ItemClassRecord
    {
  
        public string ItemClsCd { get; set; }           // Item Classification Code (RRA)
        //public string ItemClsCd { get; set; }           // 품목분류코드
        public string ItemClsNm { get; set; }           // Item Classification Name
        //public string ItemClsNm { get; set; }           // 품목분류명
        public long ItemClsLvl { get; set; }          // Item Category Code (UN/SPSC Code)
        public int ItemCtgyLvl { get; set; }            // Item Category level
        //public int CategoryLv { get; set; }             // 분류레벨은 ItemCategory
        public string TaxTyCd { get; set; }             // Taxation Type Code
        //public string TaxTyCd { get; set; }             // 과세유형코드
        public string MjrTgYn { get; set; }             // Major Taget(Y/N)
        //public string ManageYn { get; set; }            // 집중관리대상여부
        public string UseYn { get; set; }               // Use(Y/N)
        //public string UseYn { get; set; }               // 사용여부
        public string Remark { get; set; }              // Remark
        public string RegrId { get; set; }              // Registrant ID
        public string RegrNm { get; set; }              // Registrant Name
        public string RegDt { get; set; }               // Registered Date
        public string ModrId { get; set; }              // Modifier ID
        public string ModrNm { get; set; }              // Modifier Name
        public string ModDt { get; set; }               // Modified Date
                                                        
        //public DateTime CreateDt { get; set; }          // 생성일시
        //public string CommF { get; set; }               // 송수신구분
        //public DateTime SndDt { get; set; }             // 송신일시
        //public DateTime RcvDt { get; set; }             // 수신일시

        public ItemClassRecord()
		{
			clear();
		}

		public void clear()
		{
            this.ItemClsCd = string.Empty;              // Item Classification Code (RRA)
		    this.ItemClsNm = string.Empty;              // Item Classification Name
            this.ItemClsLvl = 0;             // Item Category Code (UN / SPSC Code)
            this.ItemCtgyLvl = 0;                       // Item Category level
            this.TaxTyCd = string.Empty;                // Taxation Type Code
            this.MjrTgYn = string.Empty;                // Major Taget(Y/N)
            this.UseYn = string.Empty;                  // Use(Y/N)
            this.Remark = string.Empty;                 // Remark
            this.RegrId = "system";                 // Registrant ID
            this.RegrNm = "system";                 // Registrant Name
            this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            this.ModrId = "system";                 // Modifier ID
            this.ModrNm = "system";                 // Modifier Name
            this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
        public void UpdateNull()
        {
            if (string.IsNullOrEmpty(this.ItemClsCd)) this.ItemClsCd = string.Empty;              // Item Classification Code (RRA)
            if (string.IsNullOrEmpty(this.ItemClsNm)) this.ItemClsNm = string.Empty;              // Item Classification Name
            //this.ItemCtgyCd = 0;             // Item Category Code (UN / SPSC Code)
            //this.ItemCtgyLvl = 0;                       // Item Category level
            if (string.IsNullOrEmpty(this.TaxTyCd)) this.TaxTyCd = string.Empty;                // Taxation Type Code
            if (string.IsNullOrEmpty(this.MjrTgYn)) this.MjrTgYn = string.Empty;                // Major Taget(Y/N)
            if (string.IsNullOrEmpty(this.UseYn)) this.UseYn = string.Empty;                  // Use(Y/N)
            if (string.IsNullOrEmpty(this.Remark)) this.Remark = string.Empty;                 // Remark
            if (string.IsNullOrEmpty(this.RegrId)) this.RegrId = "system";                 // Registrant ID
            if (string.IsNullOrEmpty(this.RegrNm)) this.RegrNm = "system";                 // Registrant Name
            if (string.IsNullOrEmpty(this.RegDt)) this.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Registered Date
            if (string.IsNullOrEmpty(this.ModrId)) this.ModrId = "system";                 // Modifier ID
            if (string.IsNullOrEmpty(this.ModrNm)) this.ModrNm = "system";                 // Modifier Name
            if (string.IsNullOrEmpty(this.ModDt)) this.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // Modified Date
        }
    }
}

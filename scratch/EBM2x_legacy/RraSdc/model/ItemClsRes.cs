using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class ItemClsRes {
	    public string	resultCd { get; set; }  // Result Code
	    public string	resultMsg { get; set; }  // Result Message
	    public string	resultDt { get; set; }  // Result Date
	    public ItemClsResData data { get; set; }  // Result Date
    }
	public class ItemClsResData {
		public List<ItemClassLVO> itemClsList { get; set; }  // Code Class List
	}

	public class ItemClassLVO {
		public string	itemClsCd { get; set; }  // Item Class Code
		public string	itemClsNm { get; set; }  // Item Class Name
        public long itemClsLvl { get; set; }  // Item Class Long
        public string	taxTyCd { get; set; }  // Taxation Type Code
		public string	mjrTgYn { get; set; }  // Whether it is Major Taget
		public string	useYn { get; set; }  // Whether to Use
	}
}

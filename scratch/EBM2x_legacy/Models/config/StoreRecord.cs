namespace EBM2x.Models.config
{
    /// <summary>
    /// Description of StoreRecord.
    /// </summary>
    public class StoreRecord
	{
		public string StoreCode { get; set; }           
        public string StoreName { get; set; } 
        public string NameplateLine1 { get; set; }
        public string NameplateLine2 { get; set; }  
        public string NameplateLine3 { get; set; } 
        public string NameplateLine4 { get; set; } 
        public string StoreMessage1 { get; set; }   
        public string StoreMessage2 { get; set; } 
        public string Message1 { get; set; }      
        public string Message2 { get; set; }   
        public string Message3 { get; set; }  
        public string TargetType { get; set; }

        public StoreRecord()
		{
			clear();
		}

		public void clear()
		{
			this.StoreCode = string.Empty; 			// 가맹점 코드
			this.StoreName = string.Empty; 			// 가맹점 명
			this.NameplateLine1 =string.Empty;		// 명판메시지1
			this.NameplateLine2 = string.Empty; 	// 명판메시지2
			this.NameplateLine3 = string.Empty; 	// 명판메시지3
			this.NameplateLine4 = string.Empty; 	// 명판메시지4
			this.StoreMessage1 = string.Empty; 		// 영수증상단메시지 1
			this.StoreMessage2 = string.Empty; 		// 영수증상단메시지 2
			this.Message1 = string.Empty; 			// 영수증하단메시지 1
			this.Message2 = string.Empty; 			// 영수증하단 2
			this.Message3 = string.Empty; 			// 영수증하단 3
			this.TargetType = string.Empty;         // 소속구분
		}
    }
}

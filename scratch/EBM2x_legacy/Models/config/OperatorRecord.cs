namespace EBM2x.Models.config
{
    /// <summary>
    /// Description of OperatorRecord.
    /// </summary>
    public class OperatorRecord
	{
		public string OperatorCode { get; set; }     
        public string OperatorName { get; set; }    
        public string Password { get; set; }         
        public string TelNo { get; set; }            
        public string Permission { get; set; }       

        public OperatorRecord() 
		{
			clear();
		}

		public void clear()
		{
			this.OperatorCode = string.Empty;		
			this.OperatorName = string.Empty;		
			this.Password = string.Empty;			
			this.TelNo = string.Empty;			    
			this.Permission = string.Empty;		   
		}
	}
}

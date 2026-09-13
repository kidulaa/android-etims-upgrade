namespace EBM2x.Database.Master
{
    /// <summary>
    /// Description of SysconfigRecord.
    /// </summary>
    public class SysconfigRecord
    {
        public string ConfigCd { get; set; }            
        public string ConfigValue { get; set; }         
        public string UpdTyCd { get; set; }           

        public SysconfigRecord()
        {
            clear();
        }

        public void clear()
        {
            this.ConfigCd = string.Empty;               
            this.ConfigValue = string.Empty;            
            this.UpdTyCd = string.Empty;               
        }
    }
}

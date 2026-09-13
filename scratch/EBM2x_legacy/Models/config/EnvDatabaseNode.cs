namespace EBM2x.Models.config
{
    public class EnvDatabaseNode
    {
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public bool InitDatabaseStatus { get; set; }        
        public bool InitTableStatus { get; set; }           
        public bool DeleteBackupStatus { get; set; }        
        public bool DeleteLogStatus { get; set; }           
        public bool DeleteSignStatus { get; set; }          
        public bool DeleteHoldStatus { get; set; }          
        public bool DeleteTrStatus { get; set; }            
        public bool DeleteRegiStatus { get; set; }          
        public bool DeleteEnvironmentStatus { get; set; }   
        public bool DeletePosEnvStatus { get; set; }        

        public bool InitTranDatabaseStatus { get; set; }    
        public bool InitTranTableStatus { get; set; }	

        public EnvDatabaseNode()
        {
            InitDatabaseStatus = false;
            InitTableStatus = false;
            DeleteBackupStatus = false;
            DeleteLogStatus = false;
            DeleteSignStatus = false;
            DeleteHoldStatus = false;
            DeleteTrStatus = false;
            DeleteRegiStatus = false;
            DeleteEnvironmentStatus = false;
            DeletePosEnvStatus = false;

            InitTranDatabaseStatus = false;
            InitTranTableStatus = false;
        }
    }
}

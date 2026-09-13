using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{
    public class EnvFoodNode
    {
        
        public const string PRT_NAME_ONE = "PRT1";
        public const string PRT_NAME_TWO = "PRT2";
        public const string PRT_NAME_THREE = "PRT3";
        public const string PRT_NAME_FOUR = "PRT4";
        public const string PRT_NAME_FIVE = "PRT5";
        public const string PRT_NAME_SIX = "PRT6";
        public const string PRT_NAME_SEVEN = "PRT7";
        public const string PRT_NAME_EIGHT = "PRT8";
        public const string PRT_NAME_NINE = "PRT9";
        public const string PRT_NAME_TEN = "PRT10";
        public const string PRT_NAME_ELEVEN = "PRT11";
        public const string PRT_NAME_TWELVE = "PRT12";
        public const string PRT_NAME_THIRTEEN = "PRT13";
        public const string PRT_NAME_FOURTEEN = "PRT14";
        public const string PRT_NAME_FIFTTEEN = "PRT15";
        public const string PRT_NAME_SIXTEEN = "PRT16";
        public const string PRT_NAME_SEVENTEEN = "PRT17";
        public const string PRT_NAME_EIGHTEEN = "PRT18";
        public const string PRT_NAME_NINETEEN = "PRT19";
        public const string PRT_NAME_TWENLY = "PRT20";

        public const string FOOD_PAYMODE_PAYPOS = "1";
        
        public const string FOOD_PAYMODE_ORDERPOS = "0";

        
        public const string FOOD_ORDERDB_SERVER = "1";
        
        public const string FOOD_ORDERDB_CLIENT = "0";

        public const string FOOD_SYNC_SERVER = "1";
       
        public const string FOOD_SYNC_CLIENT = "0";

        public string PayPOS { get; set; }
        
        public string HasOrderDB { get; set; }
      
        public string SyncServer { get; set; }

        public string SeatNo { get; set; }

        public string DbHostIP { get; set; }
       
        public int DbHostPort { get; set; }

        
        public string SyncHostIP { get; set; }
        
        public int SyncHostPort { get; set; }

        public string PrintServerIP { get; set; }
       
        public int PrintServerPort { get; set; }


        public int ClientNum { get; set; }

        public EnvFoodNode()
        {
            PayPOS = string.Empty;
            HasOrderDB = string.Empty;
            SyncServer = string.Empty;
            SeatNo = string.Empty;

            DbHostIP = string.Empty;
            DbHostPort = 0;
            SyncHostIP = string.Empty;
            SyncHostPort = 0;
            PrintServerIP = string.Empty;
            PrintServerPort = 0;


            ClientNum = 0;
        }

    }
}

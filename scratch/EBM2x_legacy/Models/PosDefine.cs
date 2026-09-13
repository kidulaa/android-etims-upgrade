using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models
{
    /// <summary>
	/// Description of PosDefine.
	/// </summary>
	public class PosDefine
    {
        //----------------------------------------------------------------------------
        // VAN define
        //----------------------------------------------------------------------------
        public enum VAN_NAME
        {
            NOTUSE = 0,
            KSNET = 1,
            KFTC = 2,
            KCP = 3,
            STARVAN = 4,
            KICC = 5,
            NICE = 6,
            FDIK = 7,
            SMARTRO = 8,
            KIS = 9,
            JTNET = 10,
            KOCES = 11,
            KOVAN = 12,
            NICE_S = 13,
            SPC = 14,
            CATTERMINAL = 15
        };

        public enum GREEN_VAN_NAME
        {
#if (DEBUG)
            NICE = 6,
            NICE_S = 13
            //JTNET   = 10,
            //KIS     = 9
#else
			NICE    = 6, 
			NICE_S  = 13
#endif
        };

        /// (01:KSNET, 02:KFTC, 03:KCP, 04:STARVAN, 05:KICC, 06:NICE, 07:FDIK, 08:SMARTRO, 09:KIS, 10:JTNET, 11:KOCES, 12:KOVAN, 12:NICE()
        public static string[] getVanNames()
        {
            return Enum.GetNames(typeof(VAN_NAME));
        }

        public static string getVanName(VAN_NAME vanName)
        {
            if (Enum.IsDefined(typeof(VAN_NAME), vanName) == false)
                return Enum.GetName(typeof(VAN_NAME), VAN_NAME.NOTUSE);
            return Enum.GetName(typeof(VAN_NAME), vanName);
        }

        /// (01:KSNET, 02:KFTC, 03:KCP, 04:STARVAN, 05:KICC, 06:NICE, 07:FDIK, 08:SMARTRO, 09:KIS, 10:JTNET, 11:KOCES, 12:KOVAN))
        public static string getVanCodeString(string vanName)
        {
            int ret = (int)getVanCode(vanName);
            return ret.ToString().PadLeft(2, '0');
        }

        public static VAN_NAME getVanCode(string vanName)
        {
            try
            {
                return (VAN_NAME)Enum.Parse(typeof(VAN_NAME), vanName.ToUpper());
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog("getVanCode " + e.ToString());
                return VAN_NAME.NOTUSE;
            }
        }

        public const string POS_FLAG_CUSTOMER = "For membership POS";
        public const string POS_FLAG_NORMAL   = "for general supermarkets";

        public const string NOTICE_TYPE_NORMAL  = "1";  
        public const string NOTICE_TYPE_SURVEY  = "2";  
        public const string NOTICE_TYPE_CONSENT = "3";  

       
        public const string STORE_TYPE_ASSCIATION  = "1";
        public const string STORE_TYPE_CHAIN       = "2";
        public const string STORE_TYPE_ETC_CENTER  = "3";
        public const string STORE_TYPE_INDEPENDENT = "4";

        public const string APP_TRAN_WRITE = "TranSummaryWrite";
        public const Int32 WM_PROC = 0x0401;
        public const Int32 WM_CLOSE = 0x0402;

                      
        public const string POS_REMOTE_101 = "101";          		

       
        public const string PROMO_TYPE_THRESHOLD = "1";
       
        public const string PROMO_TYPE_MIXMATCH = "2";
        
        public const string PROMO_TYPE_THRESHOLD_QTY = "3";

        
        public const string MIXMATCH_TYPE_BUY = "1";
        
        public const string MIXMATCH_TYPE_GET = "2";
        
        public const string MIXMATCH_TYPE_ALL = "3";
        
       
        public const string APPLY_TO_COMPOUND = "C";
        
        public const string APPLY_TO_EXCLUSIVE = "E";
        
        public const string DC_TYPE_PERCENT = "1";
       
        public const string DC_TYPE_AMOUNT = "2";
        
        public const string DC_TYPE_FIXAMOUNT = "3";
        
        public const string THRE_TYPE_QTY = "U";
        
        public const string THRE_TYPE_AMOUNT = "A";
        
        public const string BUY_TYPE_ANY = "U";
      
        public const string BUY_TYPE_ALL = "A";

        public const string TEMPLET_GROCERY_STORE = "Grocery Store";
        public const string TEMPLET_PHARMACY = "Pharmacy";
        public const string TEMPLET_SPECIAL_STORE = "Specialty Store";
        public const string TEMPLET_RESTAURANT = "Restaurant";
        public const string TEMPLET_HOTEL = "Hotel";

    }
}
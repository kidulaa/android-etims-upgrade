using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Modules
{
    public static class Session
    {
        //JCNA public static string Connection = "";
        //JCNA public static string transactionTyCd = "";

        public static string GblSysUserID;
        public static string GblSysPassword;

        public static string GblUsrcod;
        public static string GblUserid;           
        public static string GblUsrNam;
        public static string GblUsrPwd;
        public static string GblUsrRole;
        public static string GblUsrAuth;
        public static string GblUsrUse;

        public static string GblSvrDat;           
        public static string GblCutDat;           

        public static string GblstrSvrYer;        
        public static string GblstrSvrMon;        
        public static string GblstrSvrDay;        

        public static int GblLocSysVer;           
        public static int GblScmSysVer;           

        public static string GblSdcSysNum;        
        public static string GblMrcSysCod;        
        public static string GblTaxIdNo;          
        public static string GblSerialNo;

        public static string GblBrcCod;           
        public static string GblBrcNam;           
        public static string GblBrcMan;           
        public static string GblBrcAdr;           
        public static string GblBrcSim;           // SIMCODENO
        public static string GblurlSender;
        public static string GblBhfItm;
        public static string GblDeviceGuid;

        public static string GblSysCode;          
        public static string GblSysName;          
        public static string GblDbKey = "EBMDBPASSWORD";

        // Keys for encryption
        public static string GblKeySign;          // Signature key
        public static string GblKeyInternal;      // Internal key
        public static string GblKeyComm;          // Communication key
                                                  // ------------------------Login var from scm-------------------------------------
        public static DateTime ScmServerTime;
        public static string Wis_lastVer;
        public static string current_version = "1.0.17";
        public static string db_version;
        public static string urlVersion;
        public static string rcpt_mode;
        public static string Gbl_maxTime;
        public static string SQLTimeCol = "DATETIME('NOW','LOCALTIME')";
    }

}

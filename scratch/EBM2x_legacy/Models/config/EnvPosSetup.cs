using EBM2x.Datafile.env;
using EBM2x.RraSdc.model;
using System;

namespace EBM2x.Models.config
{
    public class EnvPosSetup
    {
        //----------------------------------------------------------------------------
        // Non VAT
        //----------------------------------------------------------------------------
        public bool NonVAT { get; set; }
        public bool ChangeNonVAT { get; set; }

        //----------------------------------------------------------------------------
        // UpdateFlagOfflineDays :
        //----------------------------------------------------------------------------
        public string UpdateFlagOfflineDays { get; set; }

        //----------------------------------------------------------------------------
        // PRINTER 58mm or 80mm
        //----------------------------------------------------------------------------
        public string PrintingLogo { get; set; }
        public string PrinterPort { get; set; }
        public int PrinterBaudRate { get; set; }
        public string PrinterPaperSize { get; set; }

        public long LastSaleInvcNo { get; set; }
        public long LastPchsInvcNo { get; set; }

        public long LastRcptNo { get; set; }
        public long LastZreportRptNo { get; set; }

        //----------------------------------------------------------------------------
        // MySQL
        //----------------------------------------------------------------------------
        public string MySQLServerType { get; set; }
        public string MySQLServer { get; set; }
        public string MySQLDatabase { get; set; }
        public string MySQLUid { get; set; }
        public string MySQLPwd { get; set; }
        //----------------------------------------------------------------------------
        // Variables declaration
        //----------------------------------------------------------------------------
        public string TempletType { get; set; }        // Templet Type
        public string LocationType { get; set; }       // Location Type

        public int OfflineDays { get; set; }           // OfflineDays
        public double OfflineAmount { get; set; }      // OfflineAmount

        public int GblLocSysVer { get; set; }         
        public int GblScmSysVer { get; set; }          // SCM

        public string GblTaxIdNo { get; set; }         //  (Tax ID Numbers)
        public string GblTaxIdNm { get; set; }         //  (Tax ID Numbers)
        public string GblSerialNo { get; set; }

        public string GblBrcCod { get; set; }          
        public string GblBrcNam { get; set; }         
        public string GblBrcMan { get; set; }          
        public string GblBrcAdr { get; set; }          
        public string GblBrcTel { get; set; }          // Tel
        public string GblBrcEmail { get; set; }        // Email
        public string GblBrcTyCd { get; set; }   //added by Aime
        public string GblBrcHQYn { get; set; }   //added by Aime

        public string GblDvcId { get; set; }
        public string GblSdcSysNum { get; set; }       // SDC
        public string GblMrcSysCod { get; set; }       // (MRC)

        // Keys for encryption
        public string GblKeySign { get; set; }         // Signature key
        public string GblKeyInternal { get; set; }     // Internal key
        public string GblKeyComm { get; set; }         // Communication key

        public string StoreCode { get; set; } 
        public string PosNumber { get; set; }
        public string PosAdminCode { get; set; }
        public string Url0001 { get; set; }
        public string Url0002 { get; set; }
        public string PosInstallDate { get; set; }

        public EnvPosSetup()
        {
            // Non VAT
            ChangeNonVAT = false;
            NonVAT = false;

            UpdateFlagOfflineDays = "";

            PrintingLogo = "Y";
            PrinterPort = "";
            PrinterBaudRate = 19200;
            PrinterPaperSize = "80mm";

            LastSaleInvcNo = 0;
            LastPchsInvcNo = 0;

            LastRcptNo = 0;
            LastZreportRptNo = 0;

            MySQLServerType = "Master";
            MySQLServer = "";
            MySQLDatabase = "";
            MySQLUid = "";
            MySQLPwd = "";

            TempletType = "Grocery Store";
            LocationType = "en";
            OfflineDays = 365;
            /*Added By Bright 1.5.2022  
             *  OfflineAmount = 100000000;
             */
            OfflineAmount = 20000000;
            //END
            GblLocSysVer = 0;
            GblScmSysVer = 0;

            GblSdcSysNum = string.Empty;  
            GblMrcSysCod = string.Empty;      
            GblTaxIdNo = string.Empty;         
            GblTaxIdNm = string.Empty;     
            GblSerialNo = string.Empty;
            //added by Aime
            GblBrcTyCd = string.Empty;

            GblBrcCod = string.Empty;          
            GblBrcNam = string.Empty;           
            GblBrcMan = string.Empty;           
            GblBrcAdr = string.Empty;           
            GblBrcTel = string.Empty;           
            GblBrcEmail = string.Empty; 

            GblDvcId = string.Empty;
            GblKeySign = string.Empty;          // Signature key
            GblKeyInternal = string.Empty;      // Internal key
            GblKeyComm = string.Empty;          // Communication key

            StoreCode = "20001";           
            PosNumber = "0001";            
            PosAdminCode = "11111";
            PosInstallDate = string.Empty;

            Url0001 = string.Empty;
            Url0002 = string.Empty;

            Init();
        }

        public void Init()
        {
            InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
            if (initInfoVO != null)
            {
                GblTaxIdNo = initInfoVO.tin;               
                GblTaxIdNm = initInfoVO.taxprNm;          
                GblBrcTyCd = initInfoVO.vatTyCd;           // added by Aime
                GblBrcHQYn = initInfoVO.hqYn;              // added by Aime
                GblBrcCod = initInfoVO.bhfId;             
                GblBrcNam = initInfoVO.bhfNm;           
                GblBrcMan = initInfoVO.mgrNm;
                if (string.IsNullOrEmpty(GblBrcAdr))
                {
                    GblBrcAdr = "";
                    if (initInfoVO.prvncNm != null) GblBrcAdr = initInfoVO.prvncNm;           // 주소
                    if (initInfoVO.dstrtNm != null) GblBrcAdr = GblBrcAdr + " " + initInfoVO.dstrtNm;           // 주소
                    if (initInfoVO.sctrNm != null) GblBrcAdr = GblBrcAdr + " " + initInfoVO.sctrNm;             // 주소
                    if (initInfoVO.locDesc != null) GblBrcAdr = GblBrcAdr + " " + initInfoVO.locDesc;           // 주소
                }
                GblBrcTel = initInfoVO.mgrTelNo;           
                GblBrcEmail = initInfoVO.mgrEmail;        

                GblDvcId = initInfoVO.dvcId;               
                GblSdcSysNum = initInfoVO.sdcId;           
                GblMrcSysCod = initInfoVO.mrcNo;       

                // Keys for encryption
                GblKeySign = initInfoVO.signKey;           // Signature key
                GblKeyInternal = initInfoVO.intrlKey;      // Internal key
                GblKeyComm = initInfoVO.cmcKey;            // Communication key
            }
        }

        public int GetDayCount()
        {
            int dayCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(PosInstallDate) && PosInstallDate.Length >= 8)
                {
                    DateTime oDate = DateTime.ParseExact(PosInstallDate.Substring(0, 8), "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                    System.TimeSpan diff = DateTime.Now.Subtract(oDate);
                    dayCount = diff.Days + 1;
                }
            }
            catch (Exception ex)
            {
                dayCount = 0;
            }

            return dayCount;
        }
        public int GetDayCount(DateTime dateTime)
        {
            int dayCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(PosInstallDate) && PosInstallDate.Length >= 8)
                {
                    DateTime oDate = DateTime.ParseExact(PosInstallDate.Substring(0, 8), "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                    System.TimeSpan diff = dateTime.Subtract(oDate);
                    dayCount = diff.Days + 1;
                }
            }
            catch (Exception ex)
            {
                dayCount = 0;
            }

            return dayCount;
        }
        public void ChangePosInstallDate(string newdate)
        {
            try
            {
                if (!string.IsNullOrEmpty(newdate) && newdate.Length >= 8)
                {
                    DateTime nDate = DateTime.ParseExact(newdate.Substring(0, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    PosInstallDate = string.Format("{0:ddMMyyyy hh:mm}", nDate);
                }
            }
            catch (Exception ex)
            {
                PosInstallDate = string.Format("{0:ddMMyyyy hh:mm}", DateTime.Now);
            }
        }
    }
}

using EBM2x.Models.ReadyMoney;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Services
{
    public class JsonService
    {
        public static string GetPathName(string directory, string name)
        {
            //string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ISave iSave = DependencyService.Get<ISave>();
            if (iSave == null)
            {
            }
            string localApplicationData = iSave.GetEBM2xDataFolderPath();
            string pathName = Path.Combine(localApplicationData, "EBM2x/" + directory);
            return pathName;
        }
        public static string GetFileName(string directory, string name)
        {
            string pathName = GetPathName(directory, name);
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }

            string fileName = Path.Combine(pathName, name);

            return fileName;
        }
        public static bool IsFileExist(string directory, string name)
        {
            string pathName = GetPathName(directory, name);
            if (!Directory.Exists(pathName))
            {
                return false;
            }

            string fileName = Path.Combine(pathName, name);
            if (!File.Exists(fileName))
            {
                return false;
            }

            return true;
        }
        public static bool DeleteFile(string directory, string name)
        {
            string pathName = GetPathName(directory, name);
            if (!Directory.Exists(pathName))
            {
                return true;
            }

            string fileName = Path.Combine(pathName, name);
            if (!File.Exists(fileName))
            {
                return true;
            }

            File.Delete(fileName);
            return true;
        }
        /*
         *   [LocalApplicationData] -+- [environment] -+- Environment.json
         *                           |              
         *                           +- [master] -+- Store.json
         *                           |            | 
         *                           |            +- Cashier.json
         *                           |            | 
         *                           |            +- Preset.json
         *                           |            | 
         *                           |            +- DiningTable.json
         *                           |            | 
         *                           |            +- HotemRoom.json
         *                           |              
         *                           +- [image]  -+- itemcode*.png 
         *                           |              
         *                           +- [order]  -+- DiningTableId.json   
         *                           |              
         *                           +- [stay]   -+- HotemRoomNumber.json   
         *                           |              
         *                           +- [tran]   -+- [fund]    -+- ReserveFund.json
         *                           |            |             | 
         *                           |            |             +- ReserveFund.json   
         *                           |            |             | 
         *                           |            |             +- ReserveFund.json   
         *                           |            |             | 
         *                           |            |             +- ReserveFund.json   
         *                           |            | 
         *                           |            +- [total]   -+- ReceiptTotal.json   
         *                           |            |             | 
         *                           |            |             +- CashierReceiptTotalSeq.json   
         *                           |            | 
         *                           |            +- [receipt] -+- ReceiptSeq.json   
         *                           |            |             | 
         *                           |            +             +- [sent] -+- ReceiptSeq.json   
         *                           |            | 
         *                           |            +- [holding] -+- HodingReceiptSeq.json   
         *                           |              
         *                           +- [backup] -+- TranSalesDate.zip   
         *                           |             
         *                           |             
         *                           +- [sqlite] -+- EBM2xClient.db
         *                           |             
         *                           |             
         *                           +- [deploy] -+- EBM2xClientAppVersion.zip
         * 
         */
    }
}

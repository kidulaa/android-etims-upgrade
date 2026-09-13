using EBM2x.Models;
using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
//using System.Printing;

namespace EBM2x.Journal
{
    public class JournalFooter
    {
        string line = "", line2 = "";

        public void create(PosModel posModel)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
                line2 = "================================"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
                line2 = "==================================="; //35Byte
            }
            try
            {
                if (UIManager.Instance().Is58mmPrinter)
                {
                    posModel.Journal.Add("".PadLeft(9) + "SCU INFORMATION" + "".PadLeft(8));
                    posModel.Journal.Add("Date: " + DateTime.Now.ToString("dd-MM-yyyy") + " " + "Time:" + DateTime.Now.ToString(" HH:mm:ss"));
                    posModel.Journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(17, posModel.Environment.EnvPosSetup.GblSdcSysNum));

                    string flag = "";
                    if (posModel.TranModel.TranNode.TranFlag == "N") flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NS";
                    else flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NR";
                    //posModel.Journal.AddFormat("RECEIPT NUMBER : {0, 18}", flag);
                    posModel.Journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));

                    posModel.Journal.Add("".PadLeft(9) + "Internal Data :");
                    posModel.Journal.Add(GetFormatData(posModel.TranModel.TranNode.IntrlData));
                    //posModel.Journal.Add("2ZQS-U6NW-7NYF-MLWN-ZFHR-6FF5-AQ");
                    posModel.Journal.Add("".PadLeft(8) + "Receipt Signature :");
                    posModel.Journal.Add(GetFormatData(posModel.TranModel.TranNode.RcptSign));
                    //posModel.Journal.Add("PE5K-66C7-RE3L-BZCB");
                    posModel.Journal.Add("", line);
                   
                    //posModel.Journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, posModel.TranInformation.ReceiptNumber));
                    posModel.Journal.Add(Journal.JournalUtil.lpad(17, "INVOICE NUMBER :") + Journal.JournalUtil.lpad(15, posModel.Environment.EnvPosSetup.GblSdcSysNum )+"/" + posModel.TranInformation.InvoiceN0);
                    posModel.Journal.Add("Date: " + DateTime.Now.ToString("dd-MM-yyyy") + "  " + "Time:" + DateTime.Now.ToString(" HH:mm:ss"));
                   // posModel.Journal.Add("MRC : " + posModel.Environment.EnvPosSetup.GblMrcSysCod);
                    posModel.Journal.Add("", line);

                    // clinton custom  qr print
                    Models.config.EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;

                    string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + posModel.TranModel.TranNode.RcptSign;

                    posModel.Journal.Add("qrcode", qrcode);
                   // Qrcoded(qrcode);

                    posModel.Journal.Add("", line);
                    // clint


                    posModel.Journal.Add("End of Legal Receipt");
                    posModel.Journal.Add("Powered by ETIMS v1");
                    //posModel.Journal.Add("", line2);
                }
                else
                {
                    posModel.Journal.Add("".PadLeft(10) + "SCU INFORMATION" + "".PadLeft(10));
                    posModel.Journal.Add("Date : " + DateTime.Now.ToString("dd-MM-yyyy") + "   " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
                    posModel.Journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(20, posModel.Environment.EnvPosSetup.GblSdcSysNum));

                    string flag = "";
                    if (posModel.TranModel.TranNode.TranFlag == "N") flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NS";
                    else flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NR";
                    //posModel.Journal.AddFormat("RECEIPT NUMBER : {0, 18}", flag);
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));

                    posModel.Journal.Add("".PadLeft(10) + "Internal Data :");
                    posModel.Journal.Add(GetFormatData(posModel.TranModel.TranNode.IntrlData));
                    //posModel.Journal.Add("2ZQS-U6NW-7NYF-MLWN-ZFHR-6FF5-AQ");
                    posModel.Journal.Add("".PadLeft(8) + "Receipt Signature :");
                    posModel.Journal.Add(GetFormatData(posModel.TranModel.TranNode.RcptSign));
                    //posModel.Journal.Add("PE5K-66C7-RE3L-BZCB");
                    posModel.Journal.Add("", line);
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "INVOICE NUMBER :") + Journal.JournalUtil.lpad(15, posModel.Environment.EnvPosSetup.GblSdcSysNum) + "/" + posModel.TranInformation.InvoiceN0);
                    posModel.Journal.Add("Date : " + DateTime.Now.ToString("dd-MM-yyyy") + "   " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
                    //posModel.Journal.Add("MRC : " + posModel.Environment.EnvPosSetup.GblMrcSysCod);
                    posModel.Journal.Add("", line);

                    // clinton custom  qr print
                    Models.config.EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;

                    string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + posModel.TranModel.TranNode.RcptSign;

                    posModel.Journal.Add("qrcode", qrcode);

                    //Qrcoded(qrcode);

                    posModel.Journal.Add("", line);
                    // clint

                    posModel.Journal.Add("End of Legal Receipt");
                    posModel.Journal.Add("Powered by ETIMS v1");
                   }
                
                if (posModel.TranModel.TranNode.TranFlag.Equals("N"))
                {
                    if (posModel.Environment.EnvFunctionNode.UsePrintBarcode.Equals("Y"))
                    {
                        posModel.Journal.Add("barcode", posModel.RegiTotal.RegiHeader.getBarcodeText(posModel.TranModel.TranInformation.ReceiptNumber));
                    }
                    Models.config.EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;

                   string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + posModel.TranModel.TranNode.RcptSign;
                 
                    posModel.Journal.Add("qrcode", qrcode);
                    posModel.Journal.Add("barcode", posModel.RegiTotal.RegiHeader.getBarcodeText(posModel.TranModel.TranInformation.ReceiptNumber));

                }
                else
                {
                    
                    posModel.Journal.Add(string.Empty);
                    posModel.Journal.Add("barcode", posModel.RegiTotal.RegiHeader.getBarcodeText(posModel.TranModel.TranInformation.ReceiptNumber));
                    Models.config.EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;
                    string qrcode = envPosSetup.GblTaxIdNo + envPosSetup.GblBrcCod + posModel.TranModel.TranNode.RcptSign;
                    posModel.Journal.Add("qrcode", qrcode);

                }

                posModel.Journal.Add("cutpaper", string.Empty);  

            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }
        public void createWithoutPayment(PosModel posModel)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
                line2 = "================================"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
                line2 = "==================================="; //35Byte
            }

            try
            {
                if (UIManager.Instance().Is58mmPrinter)
                {
                    posModel.Journal.Add("".PadLeft(9) + "SDC INFORMATION" + "".PadLeft(8));
                    posModel.Journal.Add("Date: " + DateTime.Now.ToString("dd-MM-yyyy") + " " + "Time:" + DateTime.Now.ToString(" HH:mm:ss"));
                    posModel.Journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(17, posModel.Environment.EnvPosSetup.GblSdcSysNum));

                    string flag = "";
                    if (posModel.TranModel.TranNode.TranFlag == "N") flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NS";
                    else flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NR";
                    //posModel.Journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.rpad(15, flag));

                    //posModel.Journal.Add(Journal.JournalUtil.lpad(17, "RECEIPT NUMBER :") + Journal.JournalUtil.lpad(15, posModel.TranInformation.InvoiceN0));
                    //posModel.Journal.Add("Date: " + DateTime.Now.ToString("dd-MM-yyyy") + "  " + "Time:" + DateTime.Now.ToString(" HH:mm:ss"));
                    //posModel.Journal.Add("MRC : " + posModel.Environment.EnvPosSetup.GblMrcSysCod);
                    posModel.Journal.Add("", line);
                    
                    posModel.Journal.Add("End of Order list");
                    //posModel.Journal.Add("", line2);
                }
                else
                {
                    posModel.Journal.Add("".PadLeft(10) + "SCU INFORMATION" + "".PadLeft(10));
                    posModel.Journal.Add("Date : " + DateTime.Now.ToString("dd-MM-yyyy") + "   " + "Time :" + DateTime.Now.ToString(" HH:mm:ss"));
                    posModel.Journal.Add("SCU ID :  " + Journal.JournalUtil.lpad(20, posModel.Environment.EnvPosSetup.GblSdcSysNum));

                    string flag = "";
                    if (posModel.TranModel.TranNode.TranFlag == "N") flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NS";
                    else flag = posModel.TranInformation.ReceiptNumber + "/" + posModel.TranInformation.ReceiptNumber + "NR";
                    //posModel.Journal.Add("MRC : " + posModel.Environment.EnvPosSetup.GblMrcSysCod);
                    posModel.Journal.Add("", line);
                    posModel.Journal.Add("End of Order list");
                    posModel.Journal.Add("", line2);
                }

                posModel.Journal.Add("cutpaper", string.Empty); 

            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

        public string GetFormatData(string data)
        {
            string buffer = "";
            for(int i = 1; i <= data.Length;i++)
            {
                buffer = buffer + data.Substring(i-1, 1);
                if (i % 4 == 0 && i < data.Length) buffer = buffer + "-";
            }
            return buffer;
        }
        public void Qrcoded(string data)
        {
            var qrCodeWriter = new BarcodeWriter();
            qrCodeWriter.Format = BarcodeFormat.QR_CODE;
            qrCodeWriter.Options = new EncodingOptions
            {
                Width = 100,
                Height = 100,
                Margin = 0
            };

            var qrCode = qrCodeWriter.Write(data);
            Console.WriteLine(qrCode);
        }

    }

    internal class BarcodeWriter
    {
        public BarcodeFormat Format { get; set; }
        public EncodingOptions Options { get; set; }

        internal object Write(string qrcode)
        {
            throw new NotImplementedException();
        }
    }
}

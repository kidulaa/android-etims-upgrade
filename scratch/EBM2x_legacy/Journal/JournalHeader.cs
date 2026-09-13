using EBM2x.Models;
using EBM2x.Models.regitotal;
using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal
{
    public class JournalHeader
    {
        public void create(PosModel posModel)
        {
            create(posModel, posModel.RegiTotal);
        }

        public void create(PosModel posModel, RegiTotal regiTotal)
        {
            try
            {
                string line = "-----------------------------------"; //35Byte
                if (UIManager.Instance().Is58mmPrinter)
                {
                    line = "--------------------------------"; //32Byte
                }

                Models.config.EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;

                // Header
                posModel.Journal.Add("", "");
                posModel.Journal.Add("", envPosSetup.GblTaxIdNm);
                posModel.Journal.Add("", envPosSetup.GblBrcAdr);
                //posModel.Journal.Add("", "TEL: " + envPosSetup.GblBrcTel);
                //posModel.Journal.Add("", "EMAIL: " + envPosSetup.GblBrcEmail);
                posModel.Journal.Add("", "PIN: " + envPosSetup.GblTaxIdNo);
                posModel.Journal.Add("", "CASHIER: " + posModel.RegiTotal.RegiHeader.UserName + "(" + posModel.RegiTotal.RegiHeader.UserID + ")");
                posModel.Journal.Add("", line);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

        public void create(PosModel posModel, OperTotal operTotal)
        {
            try
            {
                string line = "-----------------------------------"; //35Byte
                if (UIManager.Instance().Is58mmPrinter)
                {
                    line = "--------------------------------"; //32Byte
                }

                Models.config.EnvPosSetup envPosSetup = posModel.Environment.EnvPosSetup;

                // Header
                posModel.Journal.Add("", "");
                posModel.Journal.Add("", envPosSetup.GblTaxIdNm);
                posModel.Journal.Add("", envPosSetup.GblBrcAdr);
                //posModel.Journal.Add("", "TEL : " + envPosSetup.GblBrcTel);
               // posModel.Journal.Add("", "EMAIL : " + envPosSetup.GblBrcEmail);
                posModel.Journal.Add("", "PIN : " + envPosSetup.GblTaxIdNo);
                posModel.Journal.Add("", line);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }
    }
}

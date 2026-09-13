using EBM2x.Models;
using EBM2x.Models.regitotal;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal
{
    public class JournalHeaderEtc
    {
        public void create(PosModel posModel)
        {
            try
            {
                string text;

                //--------------------------------------------------------------------------
             
                // posModel.getReceiptInfor().getPosImageYn()                  //--------------------------------------------------------------------------
                if (posModel.Environment.EnvFunctionNode.UsePrintLogo.Equals("Y"))
                {
                   // posModel.Journal.Add("logo", string.Empty);
                }
                else
                {
                    string PosRcptMsg1 = posModel.ReceiptInfor.PosRcptMsg1;
                    string PosRcptMsg2 = posModel.ReceiptInfor.PosRcptMsg2;
                    string PosRcptMsg3 = posModel.ReceiptInfor.PosRcptMsg3;
                    string PosRcptMsg4 = posModel.ReceiptInfor.PosRcptMsg4;

                    if (!PosRcptMsg1.Equals(string.Empty)) posModel.Journal.Add("wide", PosRcptMsg1);
                    if (!PosRcptMsg2.Equals(string.Empty)) posModel.Journal.Add("bold", PosRcptMsg2);
                    if (!PosRcptMsg3.Equals(string.Empty)) posModel.Journal.Add("bold", PosRcptMsg3);
                    if (!PosRcptMsg4.Equals(string.Empty)) posModel.Journal.Add("bold", PosRcptMsg4);
                    posModel.Journal.Add(string.Empty);
                }

                text = posModel.ReceiptInfor.PosLocMsg1;
                if (!text.Equals(string.Empty)) posModel.Journal.Add("", text);
                text = posModel.ReceiptInfor.PosLocMsg2;
                if (!text.Equals(string.Empty)) posModel.Journal.Add("", text);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }
    }
}

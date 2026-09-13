using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal.signon
{
    public class SignOnJournal
    {
        public void create(PosModel posModel)
        {
            try
            {
                Journal.JournalHeader header = new Journal.JournalHeader();
                header.create(posModel);

                create(posModel, posModel.TranModel.SignOnNode);

                Journal.JournalFooterEtc footer = new Journal.JournalFooterEtc();
                footer.create(posModel);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

        public void create(PosModel posModel, SignOnNode node)
        {
            string text;

            //posModel.Journal.AddDoubleLine();
            //posModel.Journal.AddFormat("          POS : {0}-{1}", posModel.RegiTotal.RegiHeader.RegiNo.ToString(), 
            //    String.Format("{0:0000}", posModel.RegiTotal.RegiHeader.ReceiptNo));
            //     0---------1---------2---------3---------4---------5
            text = "       #####    LOGIN    #####       ";
            posModel.Journal.Add("hide", text);
            posModel.Journal.AddFormat("manager : {0}", posModel.RegiTotal.RegiHeader.UserName);
        }
    }
}

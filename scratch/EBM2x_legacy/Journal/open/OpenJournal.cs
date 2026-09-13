using EBM2x.Models;
using EBM2x.Models.inforlist;
using EBM2x.Models.open;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal.open
{
    public class OpenJournal
    {
        public void create(PosModel posModel)
        {
            try
            {
                Journal.JournalHeader header = new Journal.JournalHeader();
                header.create(posModel);

                create(posModel, posModel.TranModel.OpenNode);

                Journal.JournalFooterEtc footer = new Journal.JournalFooterEtc();
                footer.create(posModel);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

        public void create(PosModel posModel, OpenNode node)
        {
            string text;

            posModel.Journal.AddDoubleLine();
            text = "       Report on opening preparation results       ";
            posModel.Journal.Add("hide", text);
            posModel.Journal.AddDoubleLine();
            posModel.Journal.Add("  T[Number of total cases] S[Number of success cases] F[Number of failure cases]   ");
            posModel.Journal.AddLine();

            for (int i = 0; i < node.InforNodeList.Count; i++)
            {
                InforNode infor = node.InforNodeList[i];
                if (infor != null && infor.Type.Equals("print"))
                {
                    posModel.Journal.Add(string.Empty, infor.Message);
                }
            }
            posModel.Journal.Add(" ");
        }
    }
}

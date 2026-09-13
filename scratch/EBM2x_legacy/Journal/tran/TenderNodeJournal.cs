using EBM2x.Models;
using EBM2x.Models.tran;
using EBM2x.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal.tran
{
    public class TenderNodeJournal
    {
        public void getTenderListString(PosModel posModel, TenderList tenderList)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                // 0---------1---------2---------3---------4---------5
                //  ----------------------------------------
                //       -9,999,999,990
                //       -9,999,999,990
                //      -9,999,999,990
                //                -99,999,990
                // 0---------1---------2---------3---------4---------5
            }
            else
            {
                // 0---------1---------2---------3---------4---------5
                //  ----------------------------------------
                //                 -9,999,999,990
                //               -9,999,999,990
                //                 -9,999,999,990
                //                    -99,999,990
                // 0---------1---------2---------3---------4---------5
            }

            double Amount = 0;
            int sign = 1;
            string nodeData = string.Empty;
            TenderNode tenderNode = null;

            for (int i = 0; i < tenderList.Count(); i++)
            {
                tenderNode = tenderList.Get(i);
                Amount = tenderNode.SubtotalAmount;
                sign = tenderNode.Sign;

                if (Amount != 0)
                {
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        posModel.Journal.Add(Journal.JournalUtil.lpad(17, tenderNode.TenderName.ToUpper()) + Journal.JournalUtil.lpad(15, (Amount * sign)));
                    }
                    else
                    {
                        posModel.Journal.Add(Journal.JournalUtil.lpad(20, tenderNode.TenderName.ToUpper()) + Journal.JournalUtil.lpad(15, (Amount * sign)));
                    }
                }
            }
        }

        public void getTenderNodeStringCash(PosModel posModel, TenderList tenderList)
        {
        }
    }
}

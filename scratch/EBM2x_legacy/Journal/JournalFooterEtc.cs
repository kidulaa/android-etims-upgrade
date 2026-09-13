using EBM2x.Models;
using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal
{
    public class JournalFooterEtc
    {
        public void create(PosModel posModel)
        {
            try
            {
                string text;
                if (UIManager.Instance().Is58mmPrinter)
                {
                    posModel.Journal.AddFormat("=== {0} ===", DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                }
                else
                {
                    posModel.Journal.AddFormat("========= {0} ==========", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                }

                posModel.Journal.Add("cutpaper", string.Empty); 
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

    }
}

using EBM2x.Models.regitotal;
using EBM2x.UI;
using System;
using System.Collections.Generic;

namespace EBM2x.Models.journal
{
    public class JournalModel
    {
        public bool Barcode { get; set; }
        public bool Logo { get; set; }
        public bool PrintFlag { get; set; } 

        public int PageStart { get; set; }
        public int PageOffset = 7;


        public bool BarcodePrint { get; set; }
        public string BarcodeText { get; set; }    
        public string Port { get; set; }

        public string JournalName { get; set; }

        public List<JournalString> JournalStringList { get; set; }

        public JournalModel()
        {
            JournalStringList = new List<JournalString>();

            Barcode = true;
            Logo = true;

            PageStart = 0;
            PageOffset = 7;

            BarcodePrint = false;
            BarcodeText = string.Empty;  
            Port = "printer";
            JournalName = string.Empty;
        }

        public int Count()
        {
            return JournalStringList.Count;
        }

        /**
		 * Clear value.
		 */
        public void Clear()
        {
            JournalStringList.Clear();
            BarcodePrint = false;
            Barcode = true;
            Logo = true;
            PrintFlag = true;
            PageStart = 0;
        }

        public JournalString Get(int valIndex)
        {
            if (valIndex < 0 || valIndex >= JournalStringList.Count)
            {
                return null;
            }
            else
            {
                return JournalStringList[valIndex];
            }
        }

        public void Add(string type, string value)
        {
            JournalString journalString = new JournalString();
            journalString.Type = type;
            journalString.Data = value;
            JournalStringList.Add(journalString);
        }
        public void Add(string value)
        {
            Add(string.Empty, value);
        }

        public void AddRegiFormat(string title, UnitTotal unitTotal)
        {
            try
            {
                if (UIManager.Instance().Is58mmPrinter)
                {
                    if (unitTotal != null)
                    {
                       Add(string.Format("{0}({1, 4}) {2, 11}", title, unitTotal.Count.ToString("#,##0"), unitTotal.Amount.ToString("#,##0")));
                    }
                    else
                    {
                        Add(string.Format("{0}({1, 4}) {2, 11}", title, "0", "0"));
                    }
                }
                else
                {
                    if (unitTotal != null)
                    {
                        Add(string.Format(" {0}({1, 6})  {2, 13}", title, unitTotal.Count.ToString("#,##0"), unitTotal.Amount.ToString("#,##0")));
                    }
                    else
                    {
                        Add(string.Format(" {0}({1, 6})  {2, 13}", title, "0", "0"));
                    }
                }
            }
            catch(Exception e)
            {
                string message = e.Message;
            }
        }
        public void AddRegiFormatSubtotal(string title, UnitTotal unitTotal)
        {
            try
            {
                if (UIManager.Instance().Is58mmPrinter)
                {
                    if (unitTotal != null)
                    {
                        Add(string.Format("{0}({1, 4}) {2, 11}", title, unitTotal.Count.ToString("#,##0"), unitTotal.SubtotalAmount.ToString("#,##0")));
                    }
                    else
                    {
                        Add(string.Format("{0}({1, 4}) {2, 11}", title, "0", "0"));
                    }
                }
                else
                {
                    if (unitTotal != null)
                    {
                       Add(string.Format(" {0}({1, 6})  {2, 13}", title, unitTotal.Count.ToString("#,##0"), unitTotal.SubtotalAmount.ToString("#,##0")));
                    }
                    else
                    {
                        Add(string.Format(" {0}({1, 6})  {2, 13}", title, "0", "0"));
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }
        public void AddRegiFormat(string title, int count, double amount)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                Add(string.Format("{0}({1, 4}) {2, 11}", title, count.ToString("#,##0"), amount.ToString("#,##0")));
            }
            else
            {
                Add(string.Format(" {0}({1, 6})  {2, 13}", title, count.ToString("#,##0"), amount.ToString("#,##0")));
            }
        }
        public void AddRegiFormat(string title, double amount)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                Add(string.Format("{0} {1, 17}", title, amount.ToString("#,##0")));
            }
            else
            {
                Add(string.Format(" {0} {1, 23}", title, amount.ToString("#,##0")));
            }
        }

        public void AddFormat(string format, params object[] args)
        {
            Add(string.Format(format, args));
        }

        public void AddLine()
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                Add(string.Empty, new string('-', 32));
            }
            else
            {
                Add(string.Empty, new string('-', 35));
            }
        }

        public void AddBoldLine()
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                Add("bold", new string('-', 32));
            }
            else
            {
                Add("bold", new string('-', 35));
            }
        }

        public void AddDoubleLine()
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                Add(string.Empty, new string('=', 32));
            }
            else
            {
                Add(string.Empty, new string('=', 35));
            }
        }

        // JINIT_20191201, 
        public string SetJournal()
        {
            string jrnl = string.Empty;
            for (int i = 0; i < Count(); i++)
            {
                jrnl += JournalStringList[i].Data;

                if (Count() - 1 != i)
                    jrnl += "\r\n";
            }

            return jrnl;
        }
    }
}

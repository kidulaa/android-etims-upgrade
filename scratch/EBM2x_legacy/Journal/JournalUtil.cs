using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal
{
    public class JournalUtil
    {
        public static string rpad(int size, string str)
        {
            int strLeng = 0;
            string strTemp = string.Empty;

            foreach (char ch in str)
            {
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    if ((strLeng + 2) <= size)
                    {
                        strTemp += ch;
                        strLeng += 2;
                    }
                    else
                        break;
                }
                else
                {
                    if ((strLeng + 1) <= size)
                    {
                        strTemp += ch;
                        strLeng += 1;
                    }
                    else
                        break;
                }
            }

            string spacebuff = string.Empty;
            for (int i = 0; i < size - strLeng; i++) spacebuff += ' ';

            return spacebuff + strTemp;
        }

        public static string lpad(int size, string str)
        {
            int strLeng = 0;
            string strTemp = string.Empty;

            foreach (char ch in str)
            {
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    if ((strLeng + 2) <= size)
                    {
                        strTemp += ch;
                        strLeng += 2;
                    }
                    else
                        break;
                }
                else
                {
                    if ((strLeng + 1) <= size)
                    {
                        strTemp += ch;
                        strLeng += 1;
                    }
                    else
                        break;
                }
            }

            string spacebuff = string.Empty;
            for (int i = 0; i < size - strLeng; i++) spacebuff += ' ';

            return strTemp + spacebuff;
        }

        public static string lpad(int size, int val)
        {
            return string.Format("{0:#,##0}", val).PadLeft(size, ' ');
        }
        public static string lpad(int size, long val)
        {
            return string.Format("{0:#,##0}", val).PadLeft(size, ' ');
        }
        public static string lpad(int size, double val)
        {
            return string.Format("{0:#,##0.00}", val).PadLeft(size, ' ');
        }
        public static string lpad(int size, float val)
        {
            return string.Format("{0:#,##0.00}", val).PadLeft(size, ' ');
        }
    }
}

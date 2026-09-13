using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Datafile.trlog
{
    public class MoveTranWriter : DatafileService
    {
        public static bool FileMove(string strTrFile)
        {
            return true;
        }
        public static bool JsonFileMove(string strTrFile)
        {
            return true;
        }
        public static bool ErrorFileMove(string strTrFile)
        {
            return true;
        }
        public static bool JsonErrorFileMove(string strTrFile)
        {
            return true;
        }
    }
}

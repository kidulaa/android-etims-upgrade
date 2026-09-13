using EBM2x.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Datafile.upload
{
    public class TrlogUpload
    {
        public static bool upload(PosModel posModel, bool eotFlag)
        {
            bool ret = false;
            return ret;
        }
        public static bool uploadHttp(PosModel posModel, System.Xml.XmlDocument document)
        {
            return false;
        }
        public static bool uploadMuscatHttp(PosModel posModel, TranModel tm)
        {
            return false;
        }
    }
}

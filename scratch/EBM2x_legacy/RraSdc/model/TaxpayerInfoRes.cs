using System;
using System.Collections.Generic;

namespace EBM2x.RraSdc.model
{
    public class TaxpayerInfoRes
    {
        public string resultCd { get; set; }  // Result Code
        public string resultMsg { get; set; }  // Result Message
        public string resultDt { get; set; }  // Result Date
        public TaxpayerInfoResData data { get; set; }  // Result Date
    }
    public class TaxpayerInfoResData
    {
        public TaxpayerInfo info { get; set; }  // Initialize Information
    }

    public class TaxpayerInfo
    {
        public string taxprNm { get; set; }  // Taxpayer Name
        public string vatTyCd { get; set; }  // Business Activity (1: VAT,2 : NON-VAT) //2021.02.02 Ãß°¡
    }
}

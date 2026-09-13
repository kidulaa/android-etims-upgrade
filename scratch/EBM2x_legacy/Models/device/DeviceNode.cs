using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.device
{
    public class DeviceNode
    {
        public bool MSRFlag { get; set; }
        public bool HandScannerFlag { get; set; }
        public bool FixScannerFlag { get; set; }
        public bool KeyboardFlag { get; set; }

        public bool CDPFlag { get; set; }
        public bool CashDrawerFlag { get; set; }
        public bool ToneFlag { get; set; }
        public bool PrinterFlag { get; set; }
        public bool SerialFlag { get; set; }

        public void clear()
        {
            MSRFlag = true;
            HandScannerFlag = true;
            FixScannerFlag = true;
            KeyboardFlag = true;

            CDPFlag = false;
            CashDrawerFlag = false;
            ToneFlag = false;
            PrinterFlag = false;
            SerialFlag = false;
        }
    }
}

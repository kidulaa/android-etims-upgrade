using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models
{
    public class StateModel
    {
        public const string OP_ERROR = "ERROR";
        public const string OP_POS_SETUP = "POS_SETUP";
        public const string OP_EXIT = "EXIT";
        public const string OP_NEXT = "NEXT";
        public const string OP_FAR = "FAR";
        public const string OP_PRE = "PRE";
        public const string OP_RETRY = "RETRY";
        public const string OP_ALERT = "ALERT";
        public const string OP_ALERT_YES_NO = "ALERT_YES_NO";
        public const string OP_TRANSACTION_END = "TRANSACTION_END";
        public const string OP_TRANSACTION_FAR = "TRANSACTION_FAR";

        public const string OP_TRANSACTION_HOLD = "TRANSACTION_HOLD";
        public const string OP_BUY_TRANSACTION_HOLD = "BUY_TRANSACTION_HOLD"; 

        public static bool IsIt(string opmode, string resultmode)
        {
            return opmode.Equals(resultmode);
        }
        public static bool IsIt_OP_ERROR(string opmode)
        {
            return IsIt(OP_ERROR, opmode);
        }
        public static bool IsIt_OP_ALERT(string opmode)
        {
            return IsIt(OP_ALERT, opmode);
        }
        public static bool IsIt_OP_ALERT_YES_NO(string opmode)
        {
            return IsIt(OP_ALERT_YES_NO, opmode);
        }
        public static bool IsIt_OP_POS_SETUP(string opmode)
        {
            return IsIt(OP_POS_SETUP, opmode);
        }
        public static bool IsIt_OP_EXIT(string opmode)
        {
            return IsIt(OP_EXIT, opmode);
        }
        public static bool IsIt_OP_NEXT(string opmode)
        {
            return IsIt(OP_NEXT, opmode);
        }
        public static bool IsIt_OP_FAR(string opmode)
        {
            return IsIt(OP_FAR, opmode);
        }
        public static bool IsIt_OP_PRE(string opmode)
        {
            return IsIt(OP_PRE, opmode);
        }
        public static bool IsIt_OP_RETRY(string opmode)
        {
            return IsIt(OP_RETRY, opmode);
        }
        public static bool IsIt_OP_TRANSACTION_END(string opmode)
        {
            return IsIt(OP_TRANSACTION_END, opmode);
        }
        public static bool IsIt_OP_TRANSACTION_FAR(string opmode)
        {
            return IsIt(OP_TRANSACTION_FAR, opmode);
        }
        public static bool IsIt_OP_TRANSACTION_HOLD(string opmode)
        {
            return IsIt(OP_TRANSACTION_HOLD, opmode);
        }
        public static bool IsIt_OP_BUY_TRANSACTION_HOLD(string opmode)
        {
            return IsIt(OP_BUY_TRANSACTION_HOLD, opmode);
        }
    }
}

using EBM2x.Models.ReadyMoney;
using EBM2x.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.money
{
    public class ReadyMoneyProcess
    {
        public static void SaveReserveFund(ReadyMoneyList list)
        {
            ReserveFundService.SaveList(list);
        }

        public static ReadyMoneyList LoadReserveFund()
        {
            return ReserveFundService.LoadList();
        }

        public static void SaveIntermediateDeposit(ReadyMoneyList list)
        {
            IntermediateDepositService.SaveList(list);
        }

        public static ReadyMoneyList LoadIntermediateDeposit()
        {
            return IntermediateDepositService.LoadList();
        }

        public static void SaveEndOfDay(ReadyMoneyList list)
        {
            EndOfDayService.SaveList(list);
        }

        public static ReadyMoneyList LoadEndOfDay()
        {
            return EndOfDayService.LoadList();
        }

        public static void SaveCashierShift(ReadyMoneyList list)
        {
            EndOfDayService.SaveList(list);
        }

        public static ReadyMoneyList LoadCashierShift()
        {
            return EndOfDayService.LoadList();
        }
    }
}

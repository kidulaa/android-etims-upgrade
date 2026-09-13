using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.DiningTable
{
    public class DiningTableNode
    {
        public bool IsVisible { get; set; }
        public bool IsOwnerVisible { get; set; }

        public string DiningTableCode { get; set; }
        
        public bool IsOrdered { get; set; }
        public DateTime FirstOrderTime { get; set; }

        public bool IsGroup { get; set; }
        public string GroupCode { get; set; }

        public double Amount { get; set; }

        public DiningTableNode()
        {
            IsVisible = true;

            DiningTableCode = string.Empty;
            
            initDiningTable();
        }

        public void initDiningTable()
        {
            IsOwnerVisible = false;

            IsOrdered = false;
            FirstOrderTime = DateTime.MinValue;

            IsGroup = false;
            GroupCode = "";

            Amount = 0;
        }

        public string GetFirstOrder()
        {
            if (IsOrdered)
            {
                return string.Format("{0:HH:mm}", FirstOrderTime);
            }
            else
            {
                return "00:00";
            }
        }

        public string GetDurationTime()
        {
            if(IsOrdered)
            {
                TimeSpan timeDiff = DateTime.Now - FirstOrderTime;
                return timeDiff.Hours + "h " + timeDiff.Minutes + "m";
            }
            else
            {
                return "00h 00m";
            }
        }
    }
}

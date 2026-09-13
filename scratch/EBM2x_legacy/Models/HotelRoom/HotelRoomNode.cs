using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.HotelRoom
{
    public class HotelRoomNode
    {
        public bool IsVisible { get; set; }
        public bool IsOwnerVisible { get; set; }

        public string HotelRoomCode { get; set; }
        //public string HotelRoomName { get; set; }

        public bool IsOrdered { get; set; }

        public bool IsGroup { get; set; }
        public string GroupCode { get; set; }

        public double Amount { get; set; }

        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int NumberOfNights { get; set; }
        public double PriceOfNight { get; set; }
        public string GuestName { get; set; }
        public string OperatorName { get; set; }
        public string Memo { get; set; }


        public HotelRoomNode()
        {
            IsVisible = true;

            HotelRoomCode = string.Empty;
            //HotelRoomName = string.Empty;

            initDiningTable();
        }
        public void initDiningTable()
        {
            IsOwnerVisible = false;

            IsOrdered = false;

            IsGroup = false;
            GroupCode = "";

            Amount = 0;
            NumberOfNights = 0;
            PriceOfNight = 0;
            GuestName = string.Empty;
            OperatorName = string.Empty;
            Memo = string.Empty;

            ArrivalDate = DateTime.MinValue;
            DepartureDate = DateTime.MinValue;
        }

        public void Copy(HotelRoomNode nodeFrom)
        {
            IsOrdered = nodeFrom.IsOrdered;

            IsGroup = nodeFrom.IsGroup;
            GroupCode = nodeFrom.GroupCode;

            Amount = nodeFrom.Amount;
            NumberOfNights = nodeFrom.NumberOfNights;
            PriceOfNight = nodeFrom.PriceOfNight;
            GuestName = nodeFrom.GuestName;
            OperatorName = nodeFrom.OperatorName;
            Memo = nodeFrom.Memo;

            ArrivalDate = nodeFrom.ArrivalDate;
            DepartureDate = nodeFrom.DepartureDate;
        }



        public string GetArrivalDateText()
        {
            if (IsOrdered && ArrivalDate > DateTime.MinValue)
            {
                return string.Format("{0:d/M/yyyy}", ArrivalDate);
            }
            else
            {
                return "";
            }
        }
        public string GetDepartureDateText()
        {
            if (IsOrdered && DepartureDate > DateTime.MinValue)
            {
                return string.Format("{0:d/M/yyyy}", DepartureDate);
            }
            else
            {
                return "";
            }
        }
    }
}

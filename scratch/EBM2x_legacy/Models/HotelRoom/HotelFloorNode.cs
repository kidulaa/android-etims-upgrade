using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.HotelRoom
{
    public class HotelFloorNode
    {
        public bool IsVisible { get; set; }
        public bool IsOwnerVisible { get; set; }
        public string HotelFloorName { get; set; }

        public HotelRoomList HotelRoomList { get; set; }              // HotelRoomList

        public HotelFloorNode()
        {
            IsVisible = true;
            IsOwnerVisible = false;
            HotelFloorName = string.Empty;

            HotelRoomList = new HotelRoomList();
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, HotelFloorNode>(this, "Hotel Room Node", this);
        }
    }
}

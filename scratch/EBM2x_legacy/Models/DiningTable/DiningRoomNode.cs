using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.DiningTable
{
    public class DiningRoomNode
    {
        public bool IsVisible { get; set; }
        public bool IsOwnerVisible { get; set; }
        public string RoomName { get; set; }



        public DiningTableList DiningTableList { get; set; }              // DiningTable LIST

        public DiningRoomNode()
        {
            IsVisible = true;
            IsOwnerVisible = false;
            RoomName = string.Empty;

            DiningTableList = new DiningTableList();
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, DiningRoomNode>(this, "Dining Room Node", this);
        }
    }
}

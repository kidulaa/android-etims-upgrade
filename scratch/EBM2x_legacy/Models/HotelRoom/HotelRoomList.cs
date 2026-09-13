using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.HotelRoom
{
    public class HotelRoomList
    {
        public List<HotelRoomNode> List = new List<HotelRoomNode>();                    // HotelRoomNode

        public int CurrentLineNumber { get; set; }                  
        public int LinesAtWhichPageBegins { get; set; }             
        public int CountOfItemsToDisplayOnOnePage { get; set; } 

        public HotelRoomList()
        {
            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;
            CountOfItemsToDisplayOnOnePage = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, HotelRoomList>(this, "Hotel Room Node", this); 
        }

        public int Count()
        {
            return List.Count;
        }

        public void SetCurrent(int index)
        {
            if ((LinesAtWhichPageBegins + index) <= List.Count)
            {
                CurrentLineNumber = LinesAtWhichPageBegins + index;

                MessagingCenter.Send<Object, HotelRoomList>(this, "Hotel Room Node", this);
            }
        }

        public void PageUp()
        {
            if (LinesAtWhichPageBegins <= CountOfItemsToDisplayOnOnePage) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins - CountOfItemsToDisplayOnOnePage;

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, HotelRoomList>(this, "Hotel Room Node", this);
        }

        public void PageDown()
        {
            if ((LinesAtWhichPageBegins + CountOfItemsToDisplayOnOnePage) > List.Count) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins + CountOfItemsToDisplayOnOnePage;

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, HotelRoomList>(this, "Hotel Room Node", this);
        }

        public HotelRoomNode Get(int index)
        {
            return List[index];
        }

        public void Add(HotelRoomNode itemNode)
        {
            List.Add(itemNode);

            CurrentLineNumber = List.Count;

            int pageCount = List.Count - ((List.Count - 1) % CountOfItemsToDisplayOnOnePage);
            LinesAtWhichPageBegins = pageCount;

            MessagingCenter.Send<Object, HotelRoomList>(this, "Hotel Room Node", this);
        }

        public void Clear()
        {
            List.Clear();

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;

            MessagingCenter.Send<Object, HotelRoomList>(this, "Hotel Room Node", this);
        }
    }
}

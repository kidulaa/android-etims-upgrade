using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.DiningTable
{
    public class DiningRoomList
    {
        public List<DiningRoomNode> List = new List<DiningRoomNode>();  

        public int CurrentLineNumber { get; set; }                  
        public int LinesAtWhichPageBegins { get; set; }             
        public int CountOfGroupsToDisplayOnOnePage { get; set; }     
        public int CountOfItemsToDisplayOnOnePage { get; set; } 

        public DiningRoomList()
        {
            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;
            CountOfGroupsToDisplayOnOnePage = 0;
            CountOfItemsToDisplayOnOnePage = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, DiningRoomList>(this, "Dining Room Node", this); 
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

                MessagingCenter.Send<Object, DiningRoomList>(this, "Dining Room Node", this);
            }
        }

        public void PageUp()
        {
            if (LinesAtWhichPageBegins <= CountOfGroupsToDisplayOnOnePage) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins - CountOfGroupsToDisplayOnOnePage;

            CurrentLineNumber = LinesAtWhichPageBegins;

            MessagingCenter.Send<Object, DiningRoomList>(this, "Dining Room Node", this);
        }

        public void PageDown()
        {
            if ((LinesAtWhichPageBegins + CountOfGroupsToDisplayOnOnePage) > List.Count) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins + CountOfGroupsToDisplayOnOnePage;

            CurrentLineNumber = LinesAtWhichPageBegins;

            MessagingCenter.Send<Object, DiningRoomList>(this, "Dining Room Node", this);
        }

        public DiningRoomNode Get(int index)
        {
            return List[index];
        }

        public void Add(DiningRoomNode itemNode)
        {
            itemNode.DiningTableList.CountOfItemsToDisplayOnOnePage = CountOfItemsToDisplayOnOnePage;
            List.Add(itemNode);

            CurrentLineNumber = List.Count;

            int pageCount = List.Count - ((List.Count - 1) % CountOfGroupsToDisplayOnOnePage);
            LinesAtWhichPageBegins = pageCount;

            MessagingCenter.Send<Object, DiningRoomList>(this, "Dining Room Node", this);
        }

        public void Clear()
        {
            List.Clear();

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;

            MessagingCenter.Send<Object, DiningRoomList>(this, "Dining Room Node", this);
        }
    }
}

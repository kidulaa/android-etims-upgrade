using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.tran
{
    public class TenderList
    {
        public List<TenderNode> List { get; set; }                    

        public int CurrentLineNumber { get; set; }                  
        public int LinesAtWhichPageBegins { get; set; }             
        public int CountOfItemsToDisplayOnOnePage { get; set; }     

        public TenderList()
        {
            List = new List<TenderNode>();          

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;
            CountOfItemsToDisplayOnOnePage = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
        }

        //public List<TenderNode> Lists()
        //{
        //    return arrayList;
        //}

        public int Count()
        {
            return List.Count;
        }
        public void DeleteTender()
        {
            TenderNode tenderNode = Get(CurrentLineNumber - 1);

            List.Remove(tenderNode);

            MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
        }

        public void SetCurrent(int index)
        {
            if ((LinesAtWhichPageBegins + index) <= List.Count)
            {
                CurrentLineNumber = LinesAtWhichPageBegins + index;

                MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
            }
        }

        public void PageUp()
        {
            if (LinesAtWhichPageBegins <= CountOfItemsToDisplayOnOnePage) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins - CountOfItemsToDisplayOnOnePage;

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
        }

        public void PageDown()
        {
            if ((LinesAtWhichPageBegins + CountOfItemsToDisplayOnOnePage) > List.Count) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins + CountOfItemsToDisplayOnOnePage;

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
        }

        public TenderNode Get(int index)
        {
            return List[index];
        }

        public void Add(TenderNode tenderNode)
        {
            List.Add(tenderNode);

            CurrentLineNumber = List.Count;

            int pageCount = List.Count - ((List.Count - 1) % CountOfItemsToDisplayOnOnePage);
            LinesAtWhichPageBegins = pageCount;

            MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
        }

        public void Clear()
        {
            List.Clear();

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;

            MessagingCenter.Send<Object, TenderList>(this, "Tender Node", this);
        }
        public void CalculateTenderList(TranNode tranNode)
        {
            tranNode.Receive = 0;  
            tranNode.Change = 0; 

            for (int i = 0; i < List.Count; i++)
            {
                tranNode.Receive = tranNode.Receive + List[i].ReceiveAmount;
            }

            if (tranNode.Receive > tranNode.Subtotal)
            {
                tranNode.Change = tranNode.Receive - tranNode.Subtotal;
            }
        }

        public IEnumerator<TenderNode> GetEnumerator()
        {
            for (int i = 0; i < List.Count; i++)
            {
                yield return List[i];
            }
        }
    }
}

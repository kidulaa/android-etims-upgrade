using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.State
{
    public class StateNodeList
    {
        private List<StateNode> arrayList = null;                    // ItemNode

        public int CurrentLineNumber { get; set; }                

        public StateNodeList()
        {
            arrayList = new List<StateNode>();                       // ItemNode

            CurrentLineNumber = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, StateNodeList>(this, "State Node", this); 
        }

        public int Count()
        {
            return arrayList.Count;
        }

        public void SetCurrent(int index)
        {
            if (index >= 1 && index <= arrayList.Count)
            {
                CurrentLineNumber = index;

                MessagingCenter.Send<Object, StateNodeList>(this, "State Node", this);
            }
        }

        public void PageUp()
        {
            if (CurrentLineNumber <= 1) return;

            CurrentLineNumber = CurrentLineNumber - 1;

            MessagingCenter.Send<Object, StateNodeList>(this, "State Node", this);
        }

        public void PageDown()
        {
            if ((CurrentLineNumber + 1) > arrayList.Count) return;

            CurrentLineNumber = CurrentLineNumber + 1;

            MessagingCenter.Send<Object, StateNodeList>(this, "State Node", this);
        }

        public StateNode Get(int index)
        {
            return arrayList[index];
        }

        public void Add(StateNode itemNode)
        {
            arrayList.Add(itemNode);

            CurrentLineNumber = arrayList.Count;

            MessagingCenter.Send<Object, StateNodeList>(this, "State Node", this);
        }

        public void Clear()
        {
            arrayList.Clear();

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, StateNodeList>(this, "State Node", this);
        }
    }
}

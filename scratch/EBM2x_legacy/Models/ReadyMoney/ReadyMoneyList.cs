using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.ReadyMoney
{
    public class ReadyMoneyList
    {        
       public List<ReadyMoneyNode> List = new List<ReadyMoneyNode>();  // ReadyMoneyNode

        public int CurrentLineNumber { get; set; }                 

        public ReadyMoneyList()
        {
            CurrentLineNumber = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, ReadyMoneyList>(this, "Ready Money Node", this);
        }

        public int Count()
        {
            return List.Count;
        }

        public void SetCurrent(int index)
        {
            if (index <= List.Count)
            {
                CurrentLineNumber = index;
                MessagingCenter.Send<Object, ReadyMoneyList>(this, "Ready Money Node", this);
            }
        }

        public void Up()
        {
            if (CurrentLineNumber > 1)
            {
                CurrentLineNumber -= 1;
                MessagingCenter.Send<Object, ReadyMoneyList>(this, "Ready Money Node", this);
            }
        }

        public void Down()
        {
            if (CurrentLineNumber < Count())
            {
                CurrentLineNumber += 1;
                MessagingCenter.Send<Object, ReadyMoneyList>(this, "Ready Money Node", this);
            }
        }

        public ReadyMoneyNode Get(int index)
        {
            return List[index];
        }

        public void Add(ReadyMoneyNode itemNode)
        {
            List.Add(itemNode);

            CurrentLineNumber = List.Count;

            MessagingCenter.Send<Object, ReadyMoneyList>(this, "Ready Money Node", this);
        }

        public double GetTotal()
        {
            double total = 0;

            for (int i = 0; i < List.Count; i++)
            {
                total = total + List[i].Amount;
            }

            return total;
        }


        public void Clear()
        {
            List.Clear();

            CurrentLineNumber = 0;

            MessagingCenter.Send<Object, ReadyMoneyList>(this, "Ready Money Node", this);
        }
    }
}

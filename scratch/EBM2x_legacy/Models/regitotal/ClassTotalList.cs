using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.regitotal
{
    public class ClassTotalList
    {
        public List<ClassTotal> List { get; set; }                    // ClassTotal 
        public ClassTotalList()
        {
            List = new List<ClassTotal>();                            // ClassTotal
        }

        public int Count()
        {
            return List.Count;
        }

        public ClassTotal Get(int index)
        {
            return List[index];
        }

        public void Add(ClassTotal classTotal)
        {
            List.Add(classTotal);
        }

        public void Clear()
        {
            List.Clear();
        }
    }
}

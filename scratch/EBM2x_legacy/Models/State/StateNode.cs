using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.State
{
    public class StateNode
    {
        public string Information { get; set; }
        public ContentView ContentView { get; set; }
        public int MinLength { get; set; }

        public StateNode()
        {
            Information = string.Empty;
            ContentView = null;
            MinLength = 0;
        }
    }
}

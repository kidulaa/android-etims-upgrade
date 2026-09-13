using EBM2x.Models.tran;
using System;

namespace EBM2x.Models.Event
{
    public class TenderEventArgs : EventArgs
    {
        public string TenderID { get; set; }
        public TenderNode TenderNode { get; set; }

        public TenderEventArgs(string id, TenderNode tenderNode)
        {
            TenderID = id;
            TenderNode = tenderNode;
        }
    }
}

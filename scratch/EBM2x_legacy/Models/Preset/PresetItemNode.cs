using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.Preset
{
    public class PresetItemNode
    {
        public bool IsVisible { get; set; }
        public bool IsOwnerVisible { get; set; }

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public string ItemImage { get; set; }

        public PresetItemNode()
        {
            IsVisible = true;
            IsOwnerVisible = false;

            Clear();
        }
        public void Clear()
        {
            ItemCode = string.Empty;
            ItemName = string.Empty;
            Barcode = string.Empty;
            Price = 0;
            Quantity = 0;
            Amount = 0;
            ItemImage = string.Empty;
        }
    }
}

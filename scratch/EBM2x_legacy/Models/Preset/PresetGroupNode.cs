using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.Preset
{
    public class PresetGroupNode
    {
        public bool IsVisible { get; set; }
        public bool IsOwnerVisible { get; set; }
        public string GroupName { get; set; }
        public string GroupImage { get; set; }

        public PresetItemList PresetItemList { get; set; }              // Preset ITEM LIST

        public PresetGroupNode()
        {
            IsVisible = true;
            IsOwnerVisible = false;

            PresetItemList = new PresetItemList();

            Clear();
        }

        public void Clear()
        {
            GroupName = string.Empty;
            GroupImage = string.Empty;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, PresetGroupNode>(this, "Preset Group Node", this);
        }
    }
}

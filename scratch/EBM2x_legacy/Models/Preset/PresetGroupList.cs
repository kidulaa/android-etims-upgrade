using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models.Preset
{
    public class PresetGroupList
    {
        public List<PresetGroupNode> List = new List<PresetGroupNode>();

        public int CurrentLineNumber { get; set; }                  
        public int LinesAtWhichPageBegins { get; set; }            
        public int CountOfGroupsToDisplayOnOnePage { get; set; }    
        public int CountOfItemsToDisplayOnOnePage { get; set; }    

        public PresetGroupList()
        {
            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;
            CountOfGroupsToDisplayOnOnePage = 0;
            CountOfItemsToDisplayOnOnePage = 0;
        }

        public void InvalidateSurface()
        {
            MessagingCenter.Send<Object, PresetGroupList>(this, "Preset Group Node", this); 
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

                MessagingCenter.Send<Object, PresetGroupList>(this, "Preset Group Node", this);
            }
        }

        public void PageUp()
        {
            if (LinesAtWhichPageBegins <= CountOfGroupsToDisplayOnOnePage) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins - CountOfGroupsToDisplayOnOnePage;

            CurrentLineNumber = LinesAtWhichPageBegins;

            MessagingCenter.Send<Object, PresetGroupList>(this, "Preset Group Node", this);
        }

        public void PageDown()
        {
            if ((LinesAtWhichPageBegins + CountOfGroupsToDisplayOnOnePage) > List.Count) return;

            LinesAtWhichPageBegins = LinesAtWhichPageBegins + CountOfGroupsToDisplayOnOnePage;

            CurrentLineNumber = LinesAtWhichPageBegins;

            MessagingCenter.Send<Object, PresetGroupList>(this, "Preset Group Node", this);
        }

        public PresetGroupNode Get(int index)
        {
            if (index >= List.Count) index = List.Count - 1;
            return List[index];
        }

        public void Add(PresetGroupNode itemNode)
        {
            itemNode.PresetItemList.CountOfItemsToDisplayOnOnePage = CountOfItemsToDisplayOnOnePage;
            List.Add(itemNode);

            CurrentLineNumber = List.Count;

            int pageCount = List.Count - ((List.Count - 1) % CountOfGroupsToDisplayOnOnePage);
            LinesAtWhichPageBegins = pageCount;

            MessagingCenter.Send<Object, PresetGroupList>(this, "Preset Group Node", this);
        }

        public void Clear()
        {
            List.Clear();

            CurrentLineNumber = 0;
            LinesAtWhichPageBegins = 0;

            MessagingCenter.Send<Object, PresetGroupList>(this, "Preset Group Node", this);
        }
    }
}

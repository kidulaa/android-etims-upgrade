using EBM2x.Models.Preset;
using EBM2x.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.preset
{
    public class PresetGroupNodeProcess
    {
        public static void SavePreset(PresetGroupList list)
        {
            PresetService.SaveList(list);
        }

        public static PresetGroupList LoadPreset(int countOfGroupsToDisplayOnOnePage, int countOfItemsToDisplayOnOnePage)
        {
            PresetGroupList presetGroupList = PresetService.LoadList();
            if (presetGroupList != null)
            {
                presetGroupList.CountOfGroupsToDisplayOnOnePage = countOfGroupsToDisplayOnOnePage;
                presetGroupList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;
                for (int i = 0; i < presetGroupList.List.Count; i++)
                {
                    presetGroupList.List[i].PresetItemList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;
                }
            }
            return presetGroupList;
        }

        public static void CreatePreset(int countOfGroupsToDisplayOnOnePage, int countOfItemsToDisplayOnOnePage)
        {
            PresetGroupList presetGroupList = new PresetGroupList();
            presetGroupList.CountOfGroupsToDisplayOnOnePage = countOfGroupsToDisplayOnOnePage;
            presetGroupList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;

            for (int i = 0; i < countOfGroupsToDisplayOnOnePage; i++)
            {
                PresetGroupNode groupNode = new PresetGroupNode();

                groupNode.PresetItemList.CountOfItemsToDisplayOnOnePage = presetGroupList.CountOfItemsToDisplayOnOnePage;

                for (int j = 0; j < countOfItemsToDisplayOnOnePage; j++)
                {
                    PresetItemNode itemNode = new PresetItemNode();
                    itemNode.IsVisible = false;
                    groupNode.PresetItemList.Add(itemNode);
                }
                groupNode.PresetItemList.LinesAtWhichPageBegins = 1;
                presetGroupList.Add(groupNode);
            }
            presetGroupList.LinesAtWhichPageBegins = 1;
            presetGroupList.CurrentLineNumber = 1;

            PresetService.SaveList(presetGroupList);
        }
        public static void RecreatePreset(PresetGroupList presetGroupList)
        {
            presetGroupList.Clear();

            for (int i = 0; i < presetGroupList.CountOfGroupsToDisplayOnOnePage; i++)
            {
                PresetGroupNode groupNode = new PresetGroupNode();

                groupNode.PresetItemList.CountOfItemsToDisplayOnOnePage = presetGroupList.CountOfItemsToDisplayOnOnePage;

                for (int j = 0; j < presetGroupList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    PresetItemNode itemNode = new PresetItemNode();
                    itemNode.IsVisible = false;
                    groupNode.PresetItemList.Add(itemNode);
                }
                groupNode.PresetItemList.LinesAtWhichPageBegins = 1;
                presetGroupList.Add(groupNode);
            }
            presetGroupList.LinesAtWhichPageBegins = 1;
            presetGroupList.CurrentLineNumber = 1;

            PresetService.SaveList(presetGroupList);
        }
        public static void AddPresetGroup(PresetGroupList presetGroupList, int groupCount)
        {
            int index = groupCount;
            if (presetGroupList.Count()%groupCount != 0)
            {
                index = groupCount - (presetGroupList.Count() % groupCount);
            }
            
            for (int i = 0; i < index; i++)
            {
                PresetGroupNode groupNode = new PresetGroupNode();

                groupNode.PresetItemList.CountOfItemsToDisplayOnOnePage = presetGroupList.CountOfItemsToDisplayOnOnePage;

                for (int j = 0; j < presetGroupList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    PresetItemNode itemNode = new PresetItemNode();
                    itemNode.IsVisible = false;
                    groupNode.PresetItemList.Add(itemNode);
                }
                presetGroupList.Add(groupNode);
            }
            
            PresetService.SaveList(presetGroupList);
        }
        public static void DeletePresetGroup(PresetGroupList presetGroupList, int groupCount)
        {
         
            if ((presetGroupList.Count() - groupCount) < groupCount) return;

            presetGroupList.PageUp();

            int index = groupCount;
            if (presetGroupList.Count() % groupCount != 0)
            {
                index = groupCount - (presetGroupList.Count() % groupCount);
            }

            for (int i = 0; i < index; i++)
            {
                presetGroupList.List.RemoveAt(presetGroupList.Count() -1);
            }

            PresetService.SaveList(presetGroupList);
        }
        public static void AddPresetItem(PresetGroupList presetGroupList, int itemCount)
        {
            PresetGroupNode PresetGroupNode = presetGroupList.Get(presetGroupList.CurrentLineNumber - 1);
            int index = itemCount;
            if (PresetGroupNode.PresetItemList.Count() % itemCount != 0)
            {
                index = itemCount - (PresetGroupNode.PresetItemList.Count() % itemCount);
            }

            for (int i = 0; i < index; i++)
            {
                PresetItemNode itemNode = new PresetItemNode();
                itemNode.IsVisible = false;

                PresetGroupNode.PresetItemList.Add(itemNode);
            }

            PresetService.SaveList(presetGroupList);
        }
        public static void DeletePresetItem(PresetGroupList presetGroupList, int itemCount)
        {
            PresetGroupNode PresetGroupNode = presetGroupList.Get(presetGroupList.CurrentLineNumber - 1);
            
            if ((PresetGroupNode.PresetItemList.Count() - itemCount) < itemCount) return;

            PresetGroupNode.PresetItemList.PageUp();

            int index = itemCount;
            if (PresetGroupNode.PresetItemList.Count() % itemCount != 0)
            {
                index = itemCount - (PresetGroupNode.PresetItemList.Count() % itemCount);
            }

            for (int i = 0; i < index; i++)
            {
                PresetGroupNode.PresetItemList.List.RemoveAt(PresetGroupNode.PresetItemList.Count() - 1);
            }

            PresetService.SaveList(presetGroupList);
        }
    }
}

using EBM2x.Models.DiningTable;
using EBM2x.Services;

namespace EBM2x.Process.dining
{
    public class DiningRoomNodeProcess
    {
        public static void SaveDiningTable(DiningRoomList list)
        {
            DiningTableService.SaveList(list);
        }

        public static DiningRoomList LoadDiningTable(int countOfGroupsToDisplayOnOnePage, int countOfItemsToDisplayOnOnePage)
        {
            DiningRoomList presetGroupList = DiningTableService.LoadList();
            if (presetGroupList != null)
            {
                presetGroupList.CountOfGroupsToDisplayOnOnePage = countOfGroupsToDisplayOnOnePage;
                presetGroupList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;
            }
            return presetGroupList;
        }

        public static void CreateDiningTable(int countOfGroupsToDisplayOnOnePage, int countOfItemsToDisplayOnOnePage)
        {
            DiningRoomList diningRoomList = new DiningRoomList();
            diningRoomList.CountOfGroupsToDisplayOnOnePage = countOfGroupsToDisplayOnOnePage;
            diningRoomList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;

            for (int i = 0; i < countOfGroupsToDisplayOnOnePage; i++)
            {
                DiningRoomNode groupNode = new DiningRoomNode();

                groupNode.DiningTableList.CountOfItemsToDisplayOnOnePage = diningRoomList.CountOfItemsToDisplayOnOnePage;
                for (int j = 0; j < countOfItemsToDisplayOnOnePage; j++)
                {
                    DiningTableNode itemNode = new DiningTableNode();
                    itemNode.IsVisible = false;

                    groupNode.DiningTableList.Add(itemNode);
                }
                groupNode.DiningTableList.LinesAtWhichPageBegins = 1;
                diningRoomList.Add(groupNode);
            }
            diningRoomList.LinesAtWhichPageBegins = 1;
            diningRoomList.CurrentLineNumber = 1;

            DiningTableService.SaveList(diningRoomList);
        }

        // JINIT_201911, DiningTable 
        public static void InitDiningTable()
        {
            DiningRoomList diningRoomList = DiningTableService.LoadList();
            if (diningRoomList == null) return;

            for (int i = 0; i < diningRoomList.Count(); i++)
            {
                DiningRoomNode diningRoomNode = (DiningRoomNode)diningRoomList.Get(i);

                for (int j = 0; j < diningRoomNode.DiningTableList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    DiningTableNode diningTableNode = diningRoomNode.DiningTableList.Get(j);
                    //DiningTableOrderProcess.DeleteOrder(diningTableNode.DiningTableCode);
                    diningTableNode.initDiningTable();
                }
            }
            DiningTableService.SaveList(diningRoomList);
        }

        // JINIT_201911,
        public static bool DupCheckDiningTableNo(string tableNo)
        {
            //bool flag = false;
            DiningRoomList diningRoomList = DiningTableService.LoadList();
            if (diningRoomList == null) return false;

            for (int i = 0; i < diningRoomList.Count(); i++)
            {
                DiningRoomNode diningRoomNode = (DiningRoomNode)diningRoomList.Get(i);

                for (int j = 0; j < diningRoomNode.DiningTableList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    DiningTableNode diningTableNode = diningRoomNode.DiningTableList.Get(j);
                    if (diningTableNode.DiningTableCode == tableNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static void RecreateDiningTable(DiningRoomList diningRoomList)
        {
            diningRoomList.Clear();

            for (int i = 0; i < diningRoomList.CountOfGroupsToDisplayOnOnePage; i++)
            {
                DiningRoomNode groupNode = new DiningRoomNode();

                groupNode.DiningTableList.CountOfItemsToDisplayOnOnePage = diningRoomList.CountOfItemsToDisplayOnOnePage;
                for (int j = 0; j < diningRoomList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    DiningTableNode itemNode = new DiningTableNode();
                    itemNode.IsVisible = false;

                    groupNode.DiningTableList.Add(itemNode);
                }
                groupNode.DiningTableList.LinesAtWhichPageBegins = 1;
                diningRoomList.Add(groupNode);
            }
            diningRoomList.LinesAtWhichPageBegins = 1;
            diningRoomList.CurrentLineNumber = 1;

            DiningTableService.SaveList(diningRoomList);
        }
        public static void AddDiningRoom(DiningRoomList diningRoomList, int groupCount)
        {
            int index = groupCount;
            if (diningRoomList.Count() % groupCount != 0)
            {
                index = groupCount - (diningRoomList.Count() % groupCount);
            }

            for (int i = 0; i < index; i++)
            {
                DiningRoomNode groupNode = new DiningRoomNode();

                groupNode.DiningTableList.CountOfItemsToDisplayOnOnePage = diningRoomList.CountOfItemsToDisplayOnOnePage;

                for (int j = 0; j < diningRoomList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    DiningTableNode itemNode = new DiningTableNode();
                    itemNode.IsVisible = false;
                    groupNode.DiningTableList.Add(itemNode);
                }
                diningRoomList.Add(groupNode);
            }

            DiningTableService.SaveList(diningRoomList);
        }
        public static void DeleteDiningRoom(DiningRoomList diningRoomList, int groupCount)
        {
           
            if ((diningRoomList.Count() - groupCount) < groupCount) return;

            diningRoomList.PageUp();

            int index = groupCount;
            if (diningRoomList.Count() % groupCount != 0)
            {
                index = groupCount - (diningRoomList.Count() % groupCount);
            }

            for (int i = 0; i < index; i++)
            {
                diningRoomList.List.RemoveAt(diningRoomList.Count() - 1);
            }

            DiningTableService.SaveList(diningRoomList);
        }
        public static void AddDiningTable(DiningRoomList diningRoomList, int itemCount)
        {
            DiningRoomNode DiningRoomNode = diningRoomList.Get(diningRoomList.CurrentLineNumber - 1);
            int index = itemCount;
            if (DiningRoomNode.DiningTableList.Count() % itemCount != 0)
            {
                index = itemCount - (DiningRoomNode.DiningTableList.Count() % itemCount);
            }

            for (int i = 0; i < index; i++)
            {
                DiningTableNode itemNode = new DiningTableNode();
                itemNode.IsVisible = false;

                DiningRoomNode.DiningTableList.Add(itemNode);
            }

            DiningTableService.SaveList(diningRoomList);
        }
        public static void DeleteDiningTable(DiningRoomList diningRoomList, int itemCount)
        {
            DiningRoomNode DiningRoomNode = diningRoomList.Get(diningRoomList.CurrentLineNumber - 1);
            
            if ((DiningRoomNode.DiningTableList.Count() - itemCount) < itemCount) return;

            DiningRoomNode.DiningTableList.PageUp();

            int index = itemCount;
            if (DiningRoomNode.DiningTableList.Count() % itemCount != 0)
            {
                index = itemCount - (DiningRoomNode.DiningTableList.Count() % itemCount);
            }

            for (int i = 0; i < index; i++)
            {
                DiningRoomNode.DiningTableList.List.RemoveAt(DiningRoomNode.DiningTableList.Count() - 1);
            }

            DiningTableService.SaveList(diningRoomList);
        }

    }
}

using EBM2x.Models.DiningTable;
using EBM2x.Models.ReadyMoney;
using EBM2x.Models.tran;
using EBM2x.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.dining
{
    public class DiningTableOrderProcess
    {
        public static void MoveOrder(string diningTableCodeFrom, string diningTableCodeTo)
        {
            DiningTableOrderService.Move(diningTableCodeFrom, diningTableCodeTo);
        }
        public static void CopyOrder(string diningTableCodeFrom, string diningTableCodeTo)
        {
            DiningTableOrderService.Copy(diningTableCodeFrom, diningTableCodeTo);
        }

        public static void MergeOrder(string diningTableCodeFrom, string diningTableCodeTo)
        {
            TranNode tranNodeFrom = DiningTableOrderService.Load(diningTableCodeFrom); 
            TranNode tranNodeTo = DiningTableOrderService.Load(diningTableCodeTo);

            for(int i = 0; i < tranNodeFrom.ItemList.Count();i++)
            {
                ItemNode itemNode = tranNodeFrom.ItemList.Get(i);
                tranNodeTo.ItemList.Add(itemNode);
            }
            tranNodeTo.CalculateItemList();

            DiningTableOrderService.Save(diningTableCodeTo, tranNodeTo);
            DiningTableOrderService.Delete(diningTableCodeFrom);
        }

        public static void SaveOrder(string diningTableCode, TranNode tranNode)
        {
            DiningTableOrderService.Save(diningTableCode, tranNode);
        }

        // JINIT_201911, DeleteOrder 
        public static void DeleteOrder(string diningTableCode)
        {
            DiningTableOrderService.Delete(diningTableCode);
        }

        public static TranNode LoadOrder(string diningTableCode)
        {
            return DiningTableOrderService.Load(diningTableCode);
        }

        public static TranNode LoadGroupOrder(DiningRoomList diningRoomList, DiningTableNode diningTableNode)
        {
            for(int i = 0; i < diningRoomList.Count(); i++)
            {
                DiningRoomNode diningRoom = diningRoomList.Get(i);
                if (diningRoom == null) continue;
                for(int j = 0; j < diningRoom.DiningTableList.Count(); j++)
                {
                    DiningTableNode diningTable = diningRoom.DiningTableList.Get(j);
                    if(diningTable == null || string.IsNullOrEmpty(diningTable.GroupCode)) continue;
                    if (diningTable != null && diningTableNode.DiningTableCode.Equals(diningTable.DiningTableCode)) continue;
                    if (diningTable != null && diningTableNode.GroupCode.Equals(diningTable.GroupCode))
                    {
                        MergeOrder(diningTable.DiningTableCode, diningTableNode.DiningTableCode);
                        diningTableNode.Amount = diningTableNode.Amount + diningTable.Amount;
                        diningTable.initDiningTable();
                    }
                }
            }
            DiningRoomNodeProcess.SaveDiningTable(diningRoomList);
            return DiningTableOrderService.Load(diningTableNode.DiningTableCode);
        }

        // JINIT_201911,
        public static bool CheckOrder(TranNode tranNode)
        {
           
            if (tranNode.ItemList.Count() < 1) return false;

            double qty = 0;
            
            for (int i = 0; i < tranNode.ItemList.Count(); i++)
            {
                ItemNode itemNode = tranNode.ItemList.List[i];
                qty += itemNode.Quantity;
            }
            if (qty == 0) return false;

            return true;

        }

    }
}

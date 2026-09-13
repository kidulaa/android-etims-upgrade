using EBM2x.Models.HotelRoom;
using EBM2x.Models.ReadyMoney;
using EBM2x.Models.tran;
using EBM2x.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.hotel
{
    public class HotelRoomOrderProcess
    {
        public static void MoveOrder(string hotelRoomCodeFrom, string hotelRoomCodeTo)
        {
            HotelRoomOrderService.Move(hotelRoomCodeFrom, hotelRoomCodeTo);
        }

        public static void MergeOrder(string hotelRoomCodeFrom, string hotelRoomCodeTo)
        {
            TranNode tranNodeFrom = HotelRoomOrderService.Load(hotelRoomCodeFrom);
            TranNode tranNodeTo = HotelRoomOrderService.Load(hotelRoomCodeTo);

            for (int i = 0; i < tranNodeFrom.ItemList.Count(); i++)
            {
                ItemNode itemNode = tranNodeFrom.ItemList.Get(i);
                tranNodeTo.ItemList.Add(itemNode);
            }
            tranNodeTo.CalculateItemList();

            HotelRoomOrderService.Save(hotelRoomCodeTo, tranNodeTo);
            HotelRoomOrderService.Delete(hotelRoomCodeFrom);
        }

        public static void SaveOrder(string hotelRoomCode, TranNode tranNode)
        {
            HotelRoomOrderService.Save(hotelRoomCode, tranNode);
        }

        // JINIT_201911, DeleteOrder 
        public static void DeleteOrder(string hotelRoomCode)
        {
            HotelRoomOrderService.Delete(hotelRoomCode);
        }

        public static TranNode LoadOrder(string hotelRoomCode)
        {
            return HotelRoomOrderService.Load(hotelRoomCode);
        }
        public static TranNode LoadGroupOrder(HotelFloorList hotelFloorList, HotelRoomNode hotelRoomNode)
        {
            for (int i = 0; i < hotelFloorList.Count(); i++)
            {
                HotelFloorNode hotelFloor = hotelFloorList.Get(i);
                if (hotelFloor == null) continue;
                for (int j = 0; j < hotelFloor.HotelRoomList.Count(); j++)
                {
                    HotelRoomNode hotelRoom = hotelFloor.HotelRoomList.Get(j);
                    if (hotelRoom == null || string.IsNullOrEmpty(hotelRoom.GroupCode)) continue;
                    if (hotelRoom != null && hotelRoomNode.HotelRoomCode.Equals(hotelRoom.HotelRoomCode)) continue;
                    if (hotelRoom != null && hotelRoomNode.GroupCode.Equals(hotelRoom.GroupCode))
                    {
                        MergeOrder(hotelRoom.HotelRoomCode, hotelRoomNode.HotelRoomCode);
                        hotelRoomNode.Amount = hotelRoomNode.Amount + hotelRoom.Amount;
                        hotelRoom.initDiningTable();
                    }
                }
            }

            HotelFloorNodeProcess.SaveHotelFloor(hotelFloorList);

            return HotelRoomOrderService.Load(hotelRoomNode.HotelRoomCode);
        }
    }
}

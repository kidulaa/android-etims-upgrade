using EBM2x.Models.HotelRoom;
using EBM2x.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Process.hotel
{
    public class HotelFloorNodeProcess
    {

        public static void SaveHotelFloor(HotelFloorList list)
        {
            HotelRoomService.SaveList(list);
        }

        public static HotelFloorList LoadHotelFloor(int countOfGroupsToDisplayOnOnePage, int countOfItemsToDisplayOnOnePage)
        {
            HotelFloorList hotelFloorList = HotelRoomService.LoadList();
            if (hotelFloorList != null)
            {
                hotelFloorList.CountOfGroupsToDisplayOnOnePage = countOfGroupsToDisplayOnOnePage;
                hotelFloorList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;
            }
            return hotelFloorList;
        }

        public static void CreateHotelFloor(int countOfGroupsToDisplayOnOnePage, int countOfItemsToDisplayOnOnePage)
        {
            HotelFloorList hotelFloorList = new HotelFloorList();
            hotelFloorList.CountOfGroupsToDisplayOnOnePage = countOfGroupsToDisplayOnOnePage;
            hotelFloorList.CountOfItemsToDisplayOnOnePage = countOfItemsToDisplayOnOnePage;

            for (int i = 0; i < countOfGroupsToDisplayOnOnePage; i++)
            {
                HotelFloorNode groupNode = new HotelFloorNode();
                groupNode.HotelRoomList.CountOfItemsToDisplayOnOnePage = hotelFloorList.CountOfItemsToDisplayOnOnePage;
                for (int j = 0; j < countOfItemsToDisplayOnOnePage; j++)
                {
                    HotelRoomNode itemNode = new HotelRoomNode();
                    itemNode.IsVisible = false;
                    groupNode.HotelRoomList.Add(itemNode);
                }
                groupNode.HotelRoomList.LinesAtWhichPageBegins = 1;
                hotelFloorList.Add(groupNode);
            }
            hotelFloorList.LinesAtWhichPageBegins = 1;
            hotelFloorList.CurrentLineNumber = 1;

            HotelRoomService.SaveList(hotelFloorList);
        }
        public static void RecreateHotelFloor(HotelFloorList hotelFloorList)
        {
            hotelFloorList.Clear();

            for (int i = 0; i < hotelFloorList.CountOfGroupsToDisplayOnOnePage; i++)
            {
                HotelFloorNode groupNode = new HotelFloorNode();
                groupNode.HotelRoomList.CountOfItemsToDisplayOnOnePage = hotelFloorList.CountOfItemsToDisplayOnOnePage;
                for (int j = 0; j < hotelFloorList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    HotelRoomNode itemNode = new HotelRoomNode();
                    itemNode.IsVisible = false;
                    groupNode.HotelRoomList.Add(itemNode);
                }
                groupNode.HotelRoomList.LinesAtWhichPageBegins = 1;
                hotelFloorList.Add(groupNode);
            }
            hotelFloorList.LinesAtWhichPageBegins = 1;
            hotelFloorList.CurrentLineNumber = 1;

            HotelRoomService.SaveList(hotelFloorList);
        }
        public static void AddHotelFloor(HotelFloorList hotelFloorList, int groupCount)
        {
            int index = groupCount;
            if (hotelFloorList.Count() % groupCount != 0)
            {
                index = groupCount - (hotelFloorList.Count() % groupCount);
            }

            for (int i = 0; i < index; i++)
            {
                HotelFloorNode groupNode = new HotelFloorNode();

                groupNode.HotelRoomList.CountOfItemsToDisplayOnOnePage = hotelFloorList.CountOfItemsToDisplayOnOnePage;

                for (int j = 0; j < hotelFloorList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    HotelRoomNode itemNode = new HotelRoomNode();
                    itemNode.IsVisible = false;
                    groupNode.HotelRoomList.Add(itemNode);
                }
                hotelFloorList.Add(groupNode);
            }

            HotelRoomService.SaveList(hotelFloorList);
        }
        public static void DeleteHotelFloor(HotelFloorList hotelFloorList, int groupCount)
        {
           
            if ((hotelFloorList.Count() - groupCount) < groupCount) return;

            hotelFloorList.PageUp();

            int index = groupCount;
            if (hotelFloorList.Count() % groupCount != 0)
            {
                index = groupCount - (hotelFloorList.Count() % groupCount);
            }

            for (int i = 0; i < index; i++)
            {
                hotelFloorList.List.RemoveAt(hotelFloorList.Count() - 1);
            }

            HotelRoomService.SaveList(hotelFloorList);
        }
        public static void AddHotelRoom(HotelFloorList hotelFloorList, int itemCount)
        {
            HotelFloorNode HotelFloorNode = hotelFloorList.Get(hotelFloorList.CurrentLineNumber - 1);
            int index = itemCount;
            if (HotelFloorNode.HotelRoomList.Count() % itemCount != 0)
            {
                index = itemCount - (HotelFloorNode.HotelRoomList.Count() % itemCount);
            }

            for (int i = 0; i < index; i++)
            {
                HotelRoomNode itemNode = new HotelRoomNode();
                itemNode.IsVisible = false;

                HotelFloorNode.HotelRoomList.Add(itemNode);
            }

            HotelRoomService.SaveList(hotelFloorList);
        }
        public static void DeleteHotelRoom(HotelFloorList hotelFloorList, int itemCount)
        {
            HotelFloorNode HotelFloorNode = hotelFloorList.Get(hotelFloorList.CurrentLineNumber - 1);
            
            if ((HotelFloorNode.HotelRoomList.Count() - itemCount) < itemCount) return;

            HotelFloorNode.HotelRoomList.PageUp();

            int index = itemCount;
            if (HotelFloorNode.HotelRoomList.Count() % itemCount != 0)
            {
                index = itemCount - (HotelFloorNode.HotelRoomList.Count() % itemCount);
            }

            for (int i = 0; i < index; i++)
            {
                HotelFloorNode.HotelRoomList.List.RemoveAt(HotelFloorNode.HotelRoomList.Count() - 1);
            }

            HotelRoomService.SaveList(hotelFloorList);
        }

        // JINIT_201911
        public static bool DupCheckHotelRoomNo(string roomNo)
        {
            //bool flag = false;
            HotelFloorList hotelFloorList = HotelRoomService.LoadList();
            if (hotelFloorList == null) return false;

            for (int i = 0; i < hotelFloorList.Count(); i++)
            {
                HotelFloorNode hotelFloorNode = (HotelFloorNode)hotelFloorList.Get(i);

                for (int j = 0; j < hotelFloorNode.HotelRoomList.CountOfItemsToDisplayOnOnePage; j++)
                {
                    HotelRoomNode hotelRoomNode = hotelFloorNode.HotelRoomList.Get(j);
                    if (hotelRoomNode.HotelRoomCode == roomNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

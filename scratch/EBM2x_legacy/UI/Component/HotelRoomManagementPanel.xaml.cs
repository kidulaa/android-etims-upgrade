using EBM2x.Models.HotelRoom;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotelRoomManagementPanel : ContentView
    {
        public event EventHandler FunctionCalled;
        public static readonly BindableProperty FunctionCalledProperty = BindableProperty.Create(
                                                         propertyName: "FunctionCalled",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(HotelRoomManagementPanel),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        List<HotelFloorButton> HotelFloorButtonList = null;
        List<HotelRoomButton> HotelRoomButtonList = null;

        public HotelRoomManagementPanel()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();

            HotelFloorButtonList = new List<HotelFloorButton>();
            for (int i = 0; i < 5; i++)
            {
                HotelFloorButton hotelFloorButton = new HotelFloorButton
                {
                    FunctionID = string.Format("{0}", i)
                };
                hotelFloorButton.ButtonClicked += OnGroupButtonClicked;

                fixedGrid.Children.Add(hotelFloorButton, 0 + (i * 4), 4 + (i * 4), 0, 2);
                HotelFloorButtonList.Add(hotelFloorButton);
            }

            HotelRoomButtonList = new List<HotelRoomButton>();
            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i < 5; i++)
                {
                    HotelRoomButton hotelRoomButton = new HotelRoomButton
                    {
                        FunctionID = string.Format("{0}", i + +(k * 5))
                    };
                    hotelRoomButton.ButtonClicked += OnItemButtonClicked;

                    fixedGrid.Children.Add(hotelRoomButton, 0 + (i * 4), 4 + (i * 4), 2 + (k * 2), 4 + (k * 2));
                    HotelRoomButtonList.Add(hotelRoomButton);
                }
            }

            EBM2x.UI.Draw.ImageButton imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "leftpage.png"
            };
            imageButton.ButtonClicked += OnGroupUpButtonClicked;
            fixedGrid.Children.Add(imageButton, 0, 1, 0, 2);

            imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "rightpage.png"
            };
            imageButton.ButtonClicked += OnGroupDownButtonClicked;
            fixedGrid.Children.Add(imageButton, 19, 20, 0, 2);

            imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "leftpage.png"
            };
            imageButton.ButtonClicked += OnItemUpButtonClicked;
            fixedGrid.Children.Add(imageButton, 0, 1, 3, 9);

            imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "rightpage.png"
            };
            imageButton.ButtonClicked += OnItemDownButtonClicked;
            fixedGrid.Children.Add(imageButton, 19, 20, 3, 9);
        }

        public void HotelFloorListInvalidateSurface(HotelFloorList arg, bool useButtonEvent, bool editMode)
        {
            if (arg == null || arg.LinesAtWhichPageBegins == 0)
            {
                HotelFloorNode groupNode = null;
                for (int i = 0; i < arg.CountOfGroupsToDisplayOnOnePage; i++)
                {
                    HotelFloorButtonList[i].InvalidateSurface(groupNode, i + 1, false, useButtonEvent);
                }

                HotelRoomListInvalidateSurface(null, useButtonEvent, editMode);
            }
            else
            {
                HotelFloorNode groupNode = null;
                bool isCurrent = false;

                for (int i = 0; i < arg.CountOfGroupsToDisplayOnOnePage; i++)
                {
                    if (arg.LinesAtWhichPageBegins + i <= arg.Count())
                    {
                        if (arg.CurrentLineNumber == arg.LinesAtWhichPageBegins + i) isCurrent = true;
                        groupNode = arg.Get(arg.LinesAtWhichPageBegins + (i - 1));
                    }
                    else
                    {
                        groupNode = null;
                    }
                    HotelFloorButtonList[i].InvalidateSurface(groupNode, arg.LinesAtWhichPageBegins + i, isCurrent, useButtonEvent);
                    isCurrent = false;
                }

                groupNode = arg.Get(arg.CurrentLineNumber - 1);
                HotelRoomListInvalidateSurface(groupNode.HotelRoomList, useButtonEvent, editMode);
            }
        }

        public void HotelRoomListInvalidateSurface(HotelRoomList arg, bool useButtonEvent, bool editMode)
        {
            if (arg == null || arg.LinesAtWhichPageBegins == 0)
            {
                HotelRoomNode itemNode = null;
                for (int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    HotelRoomButtonList[i].InvalidateSurface(itemNode, i + 1, false, useButtonEvent, editMode);
                }
            }
            else
            {
                HotelRoomNode itemNode = null;
                bool isCurrent = false;

                for (int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    if (arg.LinesAtWhichPageBegins + i <= arg.Count())
                    {
                        if (arg.CurrentLineNumber == arg.LinesAtWhichPageBegins + i) isCurrent = true;
                        itemNode = arg.Get(arg.LinesAtWhichPageBegins + (i - 1));
                    }
                    else
                    {
                        itemNode = null;
                    }
                    HotelRoomButtonList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + i, isCurrent, useButtonEvent, editMode);
                    isCurrent = false;
                }
            }
        }

        void OnGroupUpButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList.PageUp();
        }

        void OnGroupDownButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList.PageDown();
        }

        void OnItemUpButtonClicked(object sender, EventArgs e)
        {
            HotelFloorList groupList = UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList;
            groupList.Get(groupList.CurrentLineNumber-1).HotelRoomList.PageUp();
        }

        void OnItemDownButtonClicked(object sender, EventArgs e)
        {
            HotelFloorList groupList = UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList;
            groupList.Get(groupList.CurrentLineNumber - 1).HotelRoomList.PageDown();
        }

        void OnGroupButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            HotelFloorList groupList = UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList;
            groupList.SetCurrent(index);

            if (FunctionCalled != null)
            {
                HotelFloorNode groupNode = groupList.Get((groupList.CurrentLineNumber - 1));
                FunctionCalled?.Invoke(this, new ExtEventArgs("HotelFloor", groupNode));
            }
        }

        void OnItemButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);

            HotelFloorList groupList = UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList;
            HotelRoomList itemList = groupList.Get(groupList.CurrentLineNumber - 1).HotelRoomList;

            itemList.SetCurrent(index);

            if (FunctionCalled != null)
            {
                HotelRoomNode itemNode = itemList.Get((itemList.CurrentLineNumber - 1));
                FunctionCalled?.Invoke(this, new ExtEventArgs("HotelRoom", itemNode));
            }
        }
    }
}

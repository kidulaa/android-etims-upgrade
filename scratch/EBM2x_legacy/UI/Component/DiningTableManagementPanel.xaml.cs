using EBM2x.Models.DiningTable;
using EBM2x.UI.Draw;
using SkiaSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiningTableManagementPanel : ContentView
    {
        public event EventHandler FunctionCalled;
        public static readonly BindableProperty FunctionCalledProperty = BindableProperty.Create(
                                                         propertyName: "FunctionCalled",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(DiningTableManagementPanel),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        List<DiningRoomButton> DiningRoomButtonList = null;
        List<DiningTableButton> DiningTableButtonList = null;

        public DiningTableManagementPanel()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();

            DiningRoomButtonList = new List<DiningRoomButton>();
            for (int i = 0; i < 5; i++)
            {
                DiningRoomButton diningRoomButton = new DiningRoomButton
                {
                    FunctionID = string.Format("{0}", i)
                };
                diningRoomButton.ButtonClicked += OnGroupButtonClicked;

                fixedGrid.Children.Add(diningRoomButton, 0 + (i * 4), 4 + (i * 4), 0, 2);
                DiningRoomButtonList.Add(diningRoomButton);
            }

            DiningTableButtonList = new List<DiningTableButton>();
            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i < 5; i++)
                {
                    DiningTableButton diningTableButton = new DiningTableButton
                    {
                        FunctionID = string.Format("{0}", i + +(k * 5))
                    };
                    diningTableButton.ButtonClicked += OnItemButtonClicked;

                    fixedGrid.Children.Add(diningTableButton, 0 + (i * 4), 4 + (i * 4), 2 + (k * 2), 4 + (k * 2));
                    DiningTableButtonList.Add(diningTableButton);
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

        public void DiningRoomListInvalidateSurface(DiningRoomList arg, bool useButtonEvent, bool editMode)
        {
            if (arg == null || arg.LinesAtWhichPageBegins == 0)
            {
                DiningRoomNode groupNode = null;
                for (int i = 0; i < arg.CountOfGroupsToDisplayOnOnePage; i++)
                {
                    DiningRoomButtonList[i].InvalidateSurface(groupNode, i + 1, false, useButtonEvent);
                }

                DiningTableListInvalidateSurface(null, useButtonEvent, editMode);
            }
            else
            {
                DiningRoomNode groupNode = null;
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
                    DiningRoomButtonList[i].InvalidateSurface(groupNode, arg.LinesAtWhichPageBegins + i, isCurrent, useButtonEvent);
                    isCurrent = false;
                }

                groupNode = arg.Get(arg.CurrentLineNumber - 1);
                //groupNode.DiningTableList.Clear();
                groupNode.DiningTableList.LinesAtWhichPageBegins = 1;
                DiningTableListInvalidateSurface(groupNode.DiningTableList, useButtonEvent, editMode);
            }
        }

        public void DiningTableListInvalidateSurface(DiningTableList arg, bool useButtonEvent, bool editMode)
        {
            if (arg == null || arg.LinesAtWhichPageBegins == 0)
            {
                DiningTableNode itemNode = null;
                for (int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    DiningTableButtonList[i].InvalidateSurface(itemNode, i + 1, false, useButtonEvent, editMode);
                }
            }
            else
            {
                DiningTableNode itemNode = null;
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
                    DiningTableButtonList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + i, isCurrent, useButtonEvent, editMode);
                    isCurrent = false;
                }
            }
        }

        void OnGroupUpButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.DiningTableModel.DiningRoomList.PageUp();
        }

        void OnGroupDownButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.DiningTableModel.DiningRoomList.PageDown();
        }

        void OnItemUpButtonClicked(object sender, EventArgs e)
        {
            DiningRoomList groupList = UIManager.Instance().PosModel.DiningTableModel.DiningRoomList;
            groupList.Get(groupList.CurrentLineNumber-1).DiningTableList.PageUp();
        }

        void OnItemDownButtonClicked(object sender, EventArgs e)
        {
            DiningRoomList groupList = UIManager.Instance().PosModel.DiningTableModel.DiningRoomList;
            groupList.Get(groupList.CurrentLineNumber - 1).DiningTableList.PageDown();
        }

        void OnGroupButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            DiningRoomList groupList = UIManager.Instance().PosModel.DiningTableModel.DiningRoomList;
            groupList.SetCurrent(index);

            if (FunctionCalled != null)
            {
                DiningRoomNode groupNode = groupList.Get((groupList.CurrentLineNumber - 1));
                FunctionCalled?.Invoke(this, new ExtEventArgs("DiningRoom", groupNode));
            }
        }

        void OnItemButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            DiningRoomList groupList = UIManager.Instance().PosModel.DiningTableModel.DiningRoomList;
            DiningTableList itemList = groupList.Get(groupList.CurrentLineNumber - 1).DiningTableList;
            itemList.SetCurrent(index);

            if (FunctionCalled != null)
            {
                DiningTableNode itemNode = itemList.Get((itemList.CurrentLineNumber - 1));
                FunctionCalled?.Invoke(this, new ExtEventArgs("DiningTable", itemNode));
            }
        }
    }
}

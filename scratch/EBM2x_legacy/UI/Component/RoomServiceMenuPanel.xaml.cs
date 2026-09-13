using EBM2x.Models.Preset;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomServiceMenuPanel : ContentView
    {
        public event EventHandler FunctionCalled;
        public static readonly BindableProperty FunctionCalledProperty = BindableProperty.Create(
                                                         propertyName: "FunctionCalled",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(RoomServiceMenuPanel),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        List<ImagePresetGroupButton> PresetGroupButtonList = null;
        List<ImagePresetItemButton> PresetItemButtonList = null;

        public RoomServiceMenuPanel()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();

            PresetGroupButtonList = new List<ImagePresetGroupButton>();
            for (int i = 0; i < 5; i++)
            {
                ImagePresetGroupButton presetGroupButton = new ImagePresetGroupButton
                {
                    FunctionID = string.Format("{0}", i)
                };
                presetGroupButton.ButtonClicked += OnGroupButtonClicked;

                fixedGrid.Children.Add(presetGroupButton, 0 + (i * 4), 4 + (i * 4), 0, 3);
                PresetGroupButtonList.Add(presetGroupButton);
            }

            PresetItemButtonList = new List<ImagePresetItemButton>();
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < 5; i++)
                {
                    ImagePresetItemButton presetItemButton = new ImagePresetItemButton
                    {
                        FunctionID = string.Format("{0}", i + +(k * 5))
                    };
                    presetItemButton.ButtonClicked += OnItemButtonClicked;

                    fixedGrid.Children.Add(presetItemButton, 0 + (i * 4), 4 + (i * 4), 3 + (k * 3), 6 + (k * 3));
                    PresetItemButtonList.Add(presetItemButton);
                }
            }

            EBM2x.UI.Draw.ImageButton imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "leftpage.png"
            };
            imageButton.ButtonClicked += OnGroupUpButtonClicked;
            fixedGrid.Children.Add(imageButton, 0, 1, 0, 3);

            imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "rightpage.png"
            };
            imageButton.ButtonClicked += OnGroupDownButtonClicked;
            fixedGrid.Children.Add(imageButton, 19, 20, 0, 3);

            imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "leftpage.png"
            };
            imageButton.ButtonClicked += OnItemUpButtonClicked;
            fixedGrid.Children.Add(imageButton, 0, 1, 5, 15);

            imageButton = new EBM2x.UI.Draw.ImageButton
            {
                Icon = "rightpage.png"
            };
            imageButton.ButtonClicked += OnItemDownButtonClicked;
            fixedGrid.Children.Add(imageButton, 19, 20, 5, 15);
        }

        public void PresetGroupListInvalidateSurface(PresetGroupList arg, bool useButtonEvent, bool editMode)
        {
            if (arg == null || arg.LinesAtWhichPageBegins == 0)
            {
                PresetGroupNode groupNode = null;
                for (int i = 0; i < arg.CountOfGroupsToDisplayOnOnePage; i++)
                {
                    PresetGroupButtonList[i].InvalidateSurface(groupNode, i + 1, false, useButtonEvent);
                }

                PresetItemListInvalidateSurface(null, useButtonEvent, editMode);
            }
            else
            {
                PresetGroupNode groupNode = null;
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
                    PresetGroupButtonList[i].InvalidateSurface(groupNode, arg.LinesAtWhichPageBegins + i, isCurrent, useButtonEvent);
                    isCurrent = false;
                }

                groupNode = arg.Get(arg.CurrentLineNumber - 1);
                PresetItemListInvalidateSurface(groupNode.PresetItemList, useButtonEvent, editMode);
            }
        }

        public void PresetItemListInvalidateSurface(PresetItemList arg, bool useButtonEvent, bool editMode)
        {
            if (arg == null || arg.LinesAtWhichPageBegins == 0)
            {
                PresetItemNode itemNode = null;
                for (int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    PresetItemButtonList[i].InvalidateSurface(itemNode, i + 1, false, useButtonEvent, editMode);
                }
            }
            else
            {
                PresetItemNode itemNode = null;
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
                    PresetItemButtonList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + i, isCurrent, useButtonEvent, editMode);
                    isCurrent = false;
                }
            }
        }

        void OnGroupUpButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.PresetModel.PresetGroupList.PageUp();
        }

        void OnGroupDownButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.PresetModel.PresetGroupList.PageDown();
        }

        void OnItemUpButtonClicked(object sender, EventArgs e)
        {
            PresetGroupList groupList = UIManager.Instance().PosModel.PresetModel.PresetGroupList;
            groupList.Get(groupList.CurrentLineNumber-1).PresetItemList.PageUp();
        }

        void OnItemDownButtonClicked(object sender, EventArgs e)
        {
            PresetGroupList groupList = UIManager.Instance().PosModel.PresetModel.PresetGroupList;
            groupList.Get(groupList.CurrentLineNumber - 1).PresetItemList.PageDown();
        }

        void OnGroupButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            PresetGroupList groupList = UIManager.Instance().PosModel.PresetModel.PresetGroupList;
            groupList.SetCurrent(index);

            if (FunctionCalled != null)
            {
                PresetGroupNode groupNode = groupList.Get((groupList.CurrentLineNumber - 1));
                FunctionCalled?.Invoke(this, new ExtEventArgs("PresetGroup", groupNode));
            }
        }

        void OnItemButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);

            PresetGroupList groupList = UIManager.Instance().PosModel.PresetModel.PresetGroupList;
            PresetItemList itemList = groupList.Get(groupList.CurrentLineNumber - 1).PresetItemList;

            itemList.SetCurrent(index);

            PresetItemNode itemNode = itemList.Get((itemList.CurrentLineNumber - 1));

            if (FunctionCalled != null)
            {
                FunctionCalled?.Invoke(this, new ExtEventArgs("Preset", itemNode));
            }
        }
    }
}

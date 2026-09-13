using EBM2x.Models.DiningTable;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiningTableButton : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(DiningTableButton),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);
        public string FunctionID
        {
            get { return base.GetValue(FunctionIDProperty).ToString(); }
            set { base.SetValue(FunctionIDProperty, value); }
        }
        public static readonly BindableProperty FunctionIDProperty = BindableProperty.Create(
                                                         propertyName: "FunctionID",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DiningTableButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(DiningTableButton),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        public string BoxColor
        {
            get { return base.GetValue(BoxColorProperty).ToString(); }
            set { base.SetValue(BoxColorProperty, value); }
        }
        public static readonly BindableProperty BoxColorProperty = BindableProperty.Create(
                                                         propertyName: "BoxColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DiningTableButton),
                                                         defaultValue: "ecb7d1",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsCurrent
        {
            get { return (bool)base.GetValue(IsCurrentProperty); }
            set { base.SetValue(IsCurrentProperty, value); }
        }
        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(
                                                         propertyName: "IsCurrent",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(DiningTableButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DiningTableButton()
        {
            InitializeComponent();

            groupBox.IsVisible = false;
            groupNum.IsVisible = false;
        }

        public void InvalidateSurface(DiningTableNode itemNode, int no, bool isCurrent, bool useButtonEvent, bool editMode)
        {
            IsCurrent = isCurrent;

            backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null && itemNode.IsVisible)
            {
                tableNumber.InvalidateSurface(itemNode.DiningTableCode);

                if(itemNode.IsGroup)
                {
                    groupBox.IsVisible = true;
                    groupNum.IsVisible = true;
                    groupBox.InvalidateSurface(itemNode.GroupCode);
                    groupNum.InvalidateSurface(itemNode.GroupCode);
                }
                else
                {
                    groupNum.InvalidateSurface("");
                    groupBox.IsVisible = false;
                    groupNum.IsVisible = false;
                }

                if (itemNode.IsOrdered)
                {
                    backgroungBox.InvalidateSurface("9ef7c9");

                    noNumber.InvalidateSurface(no, true);
                    firstOrderText.InvalidateSurface(" " + itemNode.GetFirstOrder() + " (since then)");
                    durationText.InvalidateSurface(" " + itemNode.GetDurationTime() + " (duration of stay)");
                    totalTitle.InvalidateSurface("Total:");
                    totalNumber.InvalidateSurface(itemNode.Amount, true);
                }
                else
                {
                    backgroungBox.InvalidateSurface("daaaf0");

                    noNumber.InvalidateSurface(no, true);
                    firstOrderText.InvalidateSurface("");
                    durationText.InvalidateSurface("");
                    totalTitle.InvalidateSurface("");
                    totalNumber.InvalidateSurface(0, false);
                }

                selectOwner.IsVisible = itemNode.IsOwnerVisible;
                buttonEvent.IsVisible = true;
            }
            else if (itemNode != null)
            {
                backgroungBox.InvalidateSurface("9b989c");

                selectOwner.IsVisible = itemNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);

                if (editMode && !string.IsNullOrEmpty(itemNode.DiningTableCode))
                {
                    tableNumber.InvalidateSurface(itemNode.DiningTableCode);
                }
                else
                {
                    tableNumber.InvalidateSurface(""); // JINIT_TableNumber Clear
                }

                groupNum.InvalidateSurface("");
                groupBox.IsVisible = false;
                groupNum.IsVisible = false;
                
                firstOrderText.InvalidateSurface("");
                durationText.InvalidateSurface("");
                totalTitle.InvalidateSurface("");
                totalNumber.InvalidateSurface(0, false);

                buttonEvent.IsVisible = useButtonEvent;
            }
            else
            {
                backgroungBox.InvalidateSurface("9b989c");
                selectOwner.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                tableNumber.InvalidateSurface("");
                
                groupNum.InvalidateSurface("");
                groupBox.IsVisible = false;
                groupNum.IsVisible = false;
                
                firstOrderText.InvalidateSurface("");
                durationText.InvalidateSurface("");
                totalTitle.InvalidateSurface("");
                totalNumber.InvalidateSurface(0, false);

                buttonEvent.IsVisible = useButtonEvent;
            }
        }

        void OnBoxButtonClicked(object sender, EventArgs e)
        {
            if (ButtonClicked != null) ButtonClicked?.Invoke(this, new ExtEventArgs(FunctionID,""));
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}

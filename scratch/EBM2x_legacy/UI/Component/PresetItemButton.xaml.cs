using EBM2x.Models.Preset;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PresetItemButton : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PresetItemButton),
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
                                                         declaringType: typeof(PresetItemButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PresetItemButton),
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
                                                         declaringType: typeof(PresetItemButton),
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
                                                         declaringType: typeof(PresetItemButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public PresetItemButton()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(PresetItemNode itemNode, int no, bool isCurrent, bool useButtonEvent, bool editMode)
        {
            IsCurrent = isCurrent;


            if (itemNode != null && itemNode.IsVisible)
            {
                backgroungBox.InvalidateSurface(BoxColor);

                selectOwner.IsVisible = itemNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface(itemNode.ItemName);
                priceNumber.InvalidateSurface(itemNode.Price, true);

                buttonEvent.IsVisible = true;
            }
            else if (itemNode != null)
            {
                backgroungBox.InvalidateSurface("9b989c");

                selectOwner.IsVisible = itemNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);

                if(editMode && !string.IsNullOrEmpty(itemNode.ItemCode))
                {
                    itemNameText.InvalidateSurface(itemNode.ItemName);
                    priceNumber.InvalidateSurface(itemNode.Price, true);
                }
                else
                {
                    itemNameText.InvalidateSurface("");
                    priceNumber.InvalidateSurface(0, false);
                }
                buttonEvent.IsVisible = useButtonEvent;
            }
            else
            {
                backgroungBox.InvalidateSurface("9b989c");

                selectOwner.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
                priceNumber.InvalidateSurface(0, false);
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

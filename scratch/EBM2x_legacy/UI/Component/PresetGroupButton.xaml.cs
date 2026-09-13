using EBM2x.Models.Preset;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PresetGroupButton : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PresetGroupButton),
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
                                                         declaringType: typeof(PresetGroupButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PresetGroupButton),
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
                                                         declaringType: typeof(PresetGroupButton),
                                                         defaultValue: "c3e0bc",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsCurrent
        {
            get { return (bool)base.GetValue(IsCurrentProperty); }
            set { base.SetValue(IsCurrentProperty, value); }
        }
        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(
                                                         propertyName: "IsCurrent",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(PresetGroupButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public PresetGroupButton()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(PresetGroupNode groupNode, int no, bool isCurrent, bool useButtonEvent)
        {
            IsCurrent = isCurrent;

            if (groupNode != null && groupNode.IsVisible)
            {
                if (IsCurrent) backgroungBox.InvalidateSurface("FFCF17");
                else backgroungBox.InvalidateSurface(BoxColor);

                selectOwner.IsVisible = groupNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface(groupNode.GroupName);

                buttonEvent.IsVisible = true;
            }
            else if (groupNode != null)
            {
                backgroungBox.InvalidateSurface("9b989c");

                selectOwner.IsVisible = groupNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
                buttonEvent.IsVisible = useButtonEvent;
            }
            else
            {
                backgroungBox.InvalidateSurface("9b989c");

                selectOwner.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
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

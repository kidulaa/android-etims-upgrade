using EBM2x.Models.Preset;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePresetGroupButton : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ImagePresetGroupButton),
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
                                                         declaringType: typeof(ImagePresetGroupButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ImagePresetGroupButton),
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
                                                         declaringType: typeof(ImagePresetGroupButton),
                                                         defaultValue: "6f7ec0",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsCurrent
        {
            get { return (bool)base.GetValue(IsCurrentProperty); }
            set { base.SetValue(IsCurrentProperty, value); }
        }
        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(
                                                         propertyName: "IsCurrent",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(ImagePresetGroupButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public ImagePresetGroupButton()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(PresetGroupNode groupNode, int no, bool isCurrent, bool useButtonEvent)
        {
            IsCurrent = isCurrent;

            if (groupNode != null && groupNode.IsVisible)
            {
                if (IsCurrent) backgroungBox.InvalidateSurface("FFCF17");
                else backgroungBox.InvalidateSurface("c3e0bc");

                selectOwner.IsVisible = groupNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface(groupNode.GroupName);
                presetImage.InvalidateSurface(groupNode.GroupImage);
                presetImage.IsVisible = true;
                buttonEvent.IsVisible = true;
            }
            else if (groupNode != null)
            {
                backgroungBox.InvalidateSurface("9b989c");
                selectOwner.IsVisible = groupNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
                presetImage.InvalidateSurface("");
                presetImage.IsVisible = false;
                buttonEvent.IsVisible = useButtonEvent;
            }
            else
            {
                backgroungBox.InvalidateSurface("9b989c");

                selectOwner.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
                presetImage.InvalidateSurface("");
                presetImage.IsVisible = false;
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

using EBM2x.Models.Preset;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePresetItemButton : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ImagePresetItemButton),
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
                                                         declaringType: typeof(ImagePresetItemButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ImagePresetItemButton),
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
                                                         declaringType: typeof(ImagePresetItemButton),
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
                                                         declaringType: typeof(ImagePresetItemButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public ImagePresetItemButton()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(PresetItemNode itemNode, int no, bool isCurrent, bool useButtonEvent, bool editMode)
        {
            IsCurrent = isCurrent;

            backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null && itemNode.IsVisible)
            {
                noNumberBack.IsVisible = true;
                itemNameTextBack.IsVisible = true;
                priceNumberBack.IsVisible = true;

                backgroungBox.InvalidateSurface("ecb7d1");
                selectOwner.IsVisible = itemNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface(itemNode.ItemName);
                priceNumber.InvalidateSurface(itemNode.Price, true);
                presetImage.InvalidateSurface(itemNode.ItemImage); 
                buttonEvent.IsVisible = true;
            }
            else if (itemNode != null)
            {
                backgroungBox.InvalidateSurface("9b989c");
                selectOwner.IsVisible = itemNode.IsOwnerVisible;

                noNumber.InvalidateSurface(no, true);
                if (editMode && !string.IsNullOrEmpty(itemNode.ItemCode))
                {
                    noNumberBack.IsVisible = true;
                    itemNameTextBack.IsVisible = true;
                    priceNumberBack.IsVisible = true;

                    itemNameText.InvalidateSurface(itemNode.ItemName);
                    priceNumber.InvalidateSurface(itemNode.Price, true);
                    presetImage.InvalidateSurface(itemNode.ItemImage);
                }
                else
                {
                    noNumberBack.IsVisible = false;
                    itemNameTextBack.IsVisible = false;
                    priceNumberBack.IsVisible = false;

                    itemNameText.InvalidateSurface("");
                    priceNumber.InvalidateSurface(0, false);
                    presetImage.InvalidateSurface("");
                }

                buttonEvent.IsVisible = useButtonEvent;
            }
            else
            {
                noNumberBack.IsVisible = false;
                itemNameTextBack.IsVisible = false;
                priceNumberBack.IsVisible = false;

                backgroungBox.InvalidateSurface("9b989c");
                selectOwner.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
                priceNumber.InvalidateSurface(0, false);
                presetImage.InvalidateSurface("");
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

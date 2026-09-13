using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneSalesItemView : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PhoneSalesItemView),
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
                                                         declaringType: typeof(PhoneSalesItemView),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PhoneSalesItemView),
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
                                                         declaringType: typeof(PhoneSalesItemView),
                                                         defaultValue: "FFFFFF",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsCurrent
        {
            get { return (bool)base.GetValue(IsCurrentProperty); }
            set { base.SetValue(IsCurrentProperty, value); }
        }
        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(
                                                         propertyName: "IsCurrent",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(PhoneSalesItemView),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public PhoneSalesItemView()
        {
            InitializeComponent();
        }
        public void InvalidateSurface(string color)
        {
            BoxColor = color;
        }

        public void InvalidateSurface(ItemNode itemNode, int no, bool isCurrent)
        {
            IsCurrent = isCurrent;

            if(IsCurrent) backgroungBox.InvalidateSurface("FFCF17");
            else backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null)
            {
                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface(itemNode.ItemName);
                itemCodeText.InvalidateSurface("");
                //itemCodeText.InvalidateSurface(itemNode.ItemCode);
                priceNumber.InvalidateSurface(itemNode.Price, true);
                qtyNumber.InvalidateSurface(itemNode.Quantity * itemNode.Sign, true);
                if(itemNode.DiscountAmount != 0) discountNumber.InvalidateSurface(itemNode.DiscountAmount, true);
                else discountNumber.InvalidateSurface(0, false);
                amountNumber.InvalidateSurface(itemNode.Subtotal * itemNode.Sign, true);
            }
            else
            {
                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
                itemCodeText.InvalidateSurface("");
                priceNumber.InvalidateSurface(0, false);
                qtyNumber.InvalidateSurface(0, false);
                discountNumber.InvalidateSurface(0, false);
                amountNumber.InvalidateSurface(0, false);
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

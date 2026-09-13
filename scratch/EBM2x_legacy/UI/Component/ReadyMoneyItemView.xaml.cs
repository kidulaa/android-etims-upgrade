using EBM2x.Models.ReadyMoney;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadyMoneyItemView : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ReadyMoneyItemView),
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
                                                         declaringType: typeof(ReadyMoneyItemView),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ReadyMoneyItemView),
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
                                                         declaringType: typeof(ReadyMoneyItemView),
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
                                                         declaringType: typeof(ReadyMoneyItemView),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public ReadyMoneyItemView()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(ReadyMoneyNode itemNode, bool isCurrent)
        {
            IsCurrent = isCurrent;

            if(IsCurrent) backgroungBox.InvalidateSurface("#fad0cf");
            else backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null)
            {
                priceNumber.InvalidateSurface(itemNode.Price, true);
                qtyNumber.InvalidateSurface(itemNode.Quantity, true);
                amountNumber.InvalidateSurface(itemNode.Amount, true);
            }
            else
            {
                priceNumber.InvalidateSurface(0, false);
                qtyNumber.InvalidateSurface(0, false);
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

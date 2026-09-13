using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentTenderView : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PaymentTenderView),
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
                                                         declaringType: typeof(PaymentTenderView),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler TenderSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "TenderSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(PaymentTenderView),
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
                                                         declaringType: typeof(PaymentTenderView),
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
                                                         declaringType: typeof(PaymentTenderView),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public PaymentTenderView()
        {
            InitializeComponent();
        }
        public void InvalidateSurface(string color)
        {
            BoxColor = color;
        }
        public void InvalidateSurface(TenderNode itemNode, int no, bool isCurrent)
        {
            IsCurrent = isCurrent;

            if(IsCurrent) backgroungBox.InvalidateSurface("FFCF17");
            else backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null)
            {
                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface(itemNode.TenderName);
                amountNumber.InvalidateSurface(itemNode.ReceiveAmount * itemNode.Sign, true);
            }
            else
            {
                noNumber.InvalidateSurface(no, true);
                itemNameText.InvalidateSurface("");
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

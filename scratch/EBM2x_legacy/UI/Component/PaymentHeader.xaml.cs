using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentHeader : ContentView
    {
        public PaymentHeader()
        {
            InitializeComponent();
        }
        public void InvalidateSurface(string boxColor)
        {
            drawBox.InvalidateSurface(boxColor);
        }
    }
}

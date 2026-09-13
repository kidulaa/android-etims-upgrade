using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneSalesHeader : ContentView
    {
        public PhoneSalesHeader()
        {
            InitializeComponent();
        }
        public void InvalidateSurface(string boxColor)
        {
            drawBox.InvalidateSurface(boxColor);
        }
    }
}

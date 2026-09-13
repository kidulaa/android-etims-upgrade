using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadyMoneyHeader : ContentView
    {
        public ReadyMoneyHeader()
        {
            InitializeComponent();
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}

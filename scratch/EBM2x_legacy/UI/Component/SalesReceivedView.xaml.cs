using EBM2x.Models.tran;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesReceivedView : ContentView
    {
        public SalesReceivedView()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(TranNode tranNode)
        {
            receivedNumber.InvalidateSurface(tranNode.Receive * tranNode.Sign, true);
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}

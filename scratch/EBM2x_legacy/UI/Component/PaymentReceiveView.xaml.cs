using EBM2x.Models.tran;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentReceiveView : ContentView
    {
        public PaymentReceiveView()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(TranNode tranNode)
        {
            receiveNumber.InvalidateSurface(tranNode.Receive * tranNode.Sign, true);
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}

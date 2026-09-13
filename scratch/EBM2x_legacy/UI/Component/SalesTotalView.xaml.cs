using EBM2x.Models.tran;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesTotalView : ContentView
    {
        public SalesTotalView()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(TranNode tranNode)
        {
            totalNumber.InvalidateSurface(tranNode.Subtotal * tranNode.Sign, true);
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}

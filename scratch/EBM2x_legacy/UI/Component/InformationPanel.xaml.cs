using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InformationPanel : ContentView
    {
        public InformationPanel()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<Object, string>(this, "Warning Message", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    warningText.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "Information Message", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    informationText.InvalidateSurface(arg);
                });
            });
        }
    }
}

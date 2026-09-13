using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EchoPanel : ContentView
    {
        public EchoPanel()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<Object, string>(this, "It was entered", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    enteredText.InvalidateSurface(arg);
                    enteredLength.InvalidateSurface(arg.Length);
                });
            });
        }
    }
}

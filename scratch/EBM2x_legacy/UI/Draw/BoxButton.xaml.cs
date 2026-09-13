using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BoxButton : ContentView
	{
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(BoxButton),
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
                                                         declaringType: typeof(BoxButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);


        public BoxButton()
        {
            InitializeComponent();

            skiaButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    Command = new Command(() => {
                        try
                        {
                            var duration = TimeSpan.FromSeconds(0.1);
                            Vibration.Vibrate(duration);
                        }
                        catch (FeatureNotSupportedException ex)
                        {
                        }
                        catch (Exception ex)
                        {
                        }
                        // Handle the click here
                        if (ButtonClicked != null) ButtonClicked?.Invoke(this, new ExtEventArgs(FunctionID,""));
                    })
                }
            );
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
        }
    }
}
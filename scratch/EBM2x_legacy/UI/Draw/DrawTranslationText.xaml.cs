using EBM2x.UI.i18n;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawTranslationText : ContentView
	{
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawTranslationText),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string TextAlign
        {
            get { return base.GetValue(TextAlignProperty).ToString(); }
            set { base.SetValue(TextAlignProperty, value); }
        }
        public static readonly BindableProperty TextAlignProperty = BindableProperty.Create(
                                                         propertyName: "TextAlign",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawTranslationText),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawTranslationText),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public float FillRate
        {
            get { return (float)(base.GetValue(FillRateProperty)); }
            set { base.SetValue(FillRateProperty, value); }
        }
        public static readonly BindableProperty FillRateProperty = BindableProperty.Create(
                                                         propertyName: "FillRate",
                                                         returnType: typeof(float),
                                                         declaringType: typeof(DrawTranslationText),
                                                         defaultValue: 0.6f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawTranslationText()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<Object, string>(this, "Location", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    canvasView.InvalidateSurface();
                });
            });
        }

        public void InvalidateSurface(string text)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            canvasView.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Text, FillRate, TextAlign, SKColor.Parse(TextColor));
        }
    }
}
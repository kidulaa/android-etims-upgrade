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
	public partial class DrawEnteredText : ContentView
	{
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawEnteredText),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawEnteredText()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(string text)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            canvasView.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void PaintSurface(SKCanvas canvas, float width, float height)
        {
            // clear the surface
            canvas.Clear(SKColors.Transparent);

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.TextAlign = SKTextAlign.Right;
                paint.TextSize = height * 0.5f;

                var xOffset = width;
                var yOffset = height - (height * 0.2f);

                canvas.DrawText(Text, xOffset, yOffset, paint);
            }
        }
    }
}
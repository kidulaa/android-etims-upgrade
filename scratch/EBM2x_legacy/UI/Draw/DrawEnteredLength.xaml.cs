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
	public partial class DrawEnteredLength : ContentView
	{
        public int Length
        {
            get { return (int)(base.GetValue(LengthProperty)); }
            set { base.SetValue(LengthProperty, value); }
        }
        public static readonly BindableProperty LengthProperty = BindableProperty.Create(
                                                         propertyName: "Length",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(DrawEnteredLength),
                                                         defaultValue: 0,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public DrawEnteredLength()
        {
            InitializeComponent();
        }
        public void InvalidateSurface(int length)
        {
            Length = length;
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
                //paint.TextAlign = SKTextAlign.Left;
                paint.TextAlign = SKTextAlign.Right;
                paint.TextSize = height * 0.3f;

                float xOffset = width - paint.TextSize;
                float yOffset = paint.TextSize;

                string Text = string.Format("[{0:00}]", Length);

                canvas.DrawText(Text, xOffset, yOffset, paint);
            }
        }
    }
}
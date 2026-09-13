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
	public partial class DrawCursor : ContentView
	{
        bool cursorActive = true;
        bool pageIsActive = false;
        Stopwatch stopwatch = new Stopwatch();

        public string Cursor
        {
            get { return base.GetValue(CursorProperty).ToString(); }
            set { base.SetValue(CursorProperty, value); }
        }
        public static readonly BindableProperty CursorProperty = BindableProperty.Create(
                                                         propertyName: "Cursor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawCursor),
                                                         defaultValue: "|",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawCursor()
        {
            InitializeComponent();

            Start();
        }

        public void Start()
        {
            pageIsActive = true;
            AnimationLoop();
        }

        public void Stop()
        {
            pageIsActive = false;
        }

        async void AnimationLoop()
        {
            // Start animation
            stopwatch.Start();

            while (pageIsActive)
            {
                canvasView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }

            // Stop animation
            stopwatch.Stop();
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
                paint.TextAlign = SKTextAlign.Left;
                paint.TextSize = height * 0.5f;

                string text = Cursor;
                if(cursorActive)
                {
                    cursorActive = false;
                    text = " ";
                }
                else
                {
                    cursorActive = true;
                    text = Cursor;
                }

                var xOffset = 0;
                var yOffset = height - (height * 0.2f);

                canvas.DrawText(text, xOffset, yOffset, paint);
            }
        }
    }
}
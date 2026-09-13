using EBM2x.UI.Resource;
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
	public partial class UploadAnimationView : ContentView
	{
        SKBitmap bitmapEveryN = null;
        bool pageIsActive;
        Stopwatch stopwatch = new Stopwatch();
        float angle;

        public UploadAnimationView()
        {
            InitializeComponent();

            using (var stream = ResourceUtil.GetImageStream("gear.png"))
            {
                bitmapEveryN = SKBitmap.Decode(stream);
            }

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
                // Determine the rotation angle.
                float tAngle = stopwatch.ElapsedMilliseconds % 3000 / 3000f;
                angle = 360 * tAngle;

                canvasView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 36));
            }

            // Stop animation
            stopwatch.Stop();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, angle, e.Info.Width, e.Info.Height, bitmapEveryN);
        }

        public void PaintSurface(SKCanvas canvas, float angle, float width, float height, SKBitmap bitmap)
        {
            // clear the surface
            canvas.Clear(SKColors.Transparent);

            //float scale = 1.5f;
            //canvas.Scale(scale);

            // Set the rotate transform
            float xRadius = width / 2;
            float yRadius = height / 2;
            canvas.RotateDegrees(angle, xRadius, yRadius);

            float xOffset = (width - bitmap.Width) / 2;
            float yOffset = (height - bitmap.Height) / 2;
            canvas.DrawBitmap(bitmap, xOffset, yOffset);
        }
    }
}
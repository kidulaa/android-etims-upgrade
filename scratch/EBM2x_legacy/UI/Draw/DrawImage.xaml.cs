using EBM2x.UI.Resource;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawImage : ContentView
	{
        SKBitmap bitmap = null;
        public string Icon
        {
            get { return base.GetValue(IconProperty).ToString(); }
            set { base.SetValue(IconProperty, value); }
        }
        public static readonly BindableProperty IconProperty = BindableProperty.Create(
                                                        propertyName: "Icon",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(DrawImage),
                                                        defaultValue: string.Empty,
                                                        defaultBindingMode: BindingMode.TwoWay);
        public DrawImage()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(string icon)
        {
            Icon = icon;
            canvasView.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            if (bitmap == null && !Icon.Equals(""))
            {              
                using (var stream = ResourceUtil.GetImageStream(Icon))
                {
                    bitmap = SKBitmap.Decode(stream);

                    SKImageInfo info = e.Info;
                    if (bitmap.Height > (info.Height * 0.8f) || bitmap.Height < (info.Height * 0.7f))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Height * 0.8f), (int)(info.Height * 0.8f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }

                    if ((info.Width < info.Height) && (bitmap.Width > (info.Width * 0.8f) || bitmap.Width < (info.Width * 0.8f)))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Width * 0.8f), (int)(info.Width * 0.8f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }
                }
            }

            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, bitmap);
        }

        public void PaintSurface(SKCanvas canvas, float width, float height, SKBitmap bitmap)
        {
            // clear the surface
            canvas.Clear(SKColors.Transparent);

            if (bitmap != null)
            {
                float xOffset = (width - bitmap.Width) / 2;
                float yOffset = (height - bitmap.Height) / 2;
                canvas.DrawBitmap(bitmap, xOffset, yOffset);

            }
        }
    }
}
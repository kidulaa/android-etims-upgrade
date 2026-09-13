using EBM2x.UI.Resource;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawResizeImage : ContentView
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
                                                        declaringType: typeof(DrawResizeImage),
                                                        defaultValue: string.Empty,
                                                        defaultBindingMode: BindingMode.TwoWay);
        public DrawResizeImage()
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
                    SKImageInfo resizeInfo = new SKImageInfo((int)info.Width, (int)info.Height, info.ColorType, info.AlphaType, info.ColorSpace);
                    bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
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
                float xOffset = 0;
                float yOffset = 0;
                canvas.DrawBitmap(bitmap, xOffset, yOffset);
            }
        }
    }
}
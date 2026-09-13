using EBM2x.UI.PresetImage;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawSystemImage : ContentView
	{
        // JINIT_20191210, 
        //SKBitmap bitmap = null;
        public SKBitmap bitmap = null;
        public string Icon
        {
            get { return base.GetValue(IconProperty).ToString(); }
            set { base.SetValue(IconProperty, value); }
        }
        public static readonly BindableProperty IconProperty = BindableProperty.Create(
                                                        propertyName: "Icon",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(DrawSystemImage),
                                                        defaultValue: string.Empty,
                                                        defaultBindingMode: BindingMode.TwoWay);
        public DrawSystemImage()
        {
            InitializeComponent();
        }

        public bool SavePngImage(string filename)
        {
            if(bitmap != null)
            {
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
                {
                    // save the data to a stream
                    using (var stream = File.OpenWrite(PresetUtil.GetImageName(filename)))
                    {
                        data.SaveTo(stream);
                    }
                }
                return true;
            }
            return false;
        }

        public void InvalidateSurface(string icon)
        {
            Icon = icon;

            if (!string.IsNullOrEmpty(Icon))
            {
                if (Icon.LastIndexOf("/") > 0 || Icon.LastIndexOf("\\") > 0)
                {
                    byte[] buff = System.IO.File.ReadAllBytes(Icon);
                    System.IO.MemoryStream stream = new System.IO.MemoryStream(buff);

                    bitmap = SKBitmap.Decode(stream);
                }
                else
                {
                    using (var stream = PresetUtil.GetImageStream(Icon))
                    {
                        bitmap = SKBitmap.Decode(stream);
                    }
                }
            }
            else
            {
                bitmap = null;
            }

            canvasView.InvalidateSurface();
        }

        // JINIT_20191210, 
        public void InvalidateSurface(byte[] buff)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream(buff);
                bitmap = SKBitmap.Decode(stream);
                canvasView.InvalidateSurface();
            }
            catch
            {
            }
        }

        // JINIT_20191210,
        public void clear()
        {
            bitmap = null;
            canvasView.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            if (bitmap != null)
            {
                SKImageInfo info = e.Info;

                int bWidth = (int)bitmap.Width;
                int bHeight = (int)bitmap.Height;

                float rate = ((float)info.Height / (float)bitmap.Height);

                int rWidth = (int)(bWidth * rate);
                int rHeight = (int)info.Height;

                SKImageInfo resizeInfo = new SKImageInfo(rWidth, rHeight, info.ColorType, info.AlphaType, info.ColorSpace);
                bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
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
                if (width < bitmap.Width)
                {
                    xOffset -= (bitmap.Width - width) / 2;
                }
                else
                {
                    xOffset = (width - bitmap.Width) / 2;
                }
                float yOffset = 0;
                if (height < bitmap.Height)
                {
                    yOffset -= (bitmap.Height - height) / 2;
                }
                canvas.DrawBitmap(bitmap, xOffset, yOffset);
            }
        }
    }
}
using EBM2x.UI.Draw;
using EBM2x.UI.Resource;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogoBackground : ContentView
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
                                                        declaringType: typeof(LogoBackground),
                                                        defaultValue: "EBM2X.png",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string Color
        {
            get { return base.GetValue(ColorProperty).ToString(); }
            set { base.SetValue(ColorProperty, value); }
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
                                                         propertyName: "Color",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(LogoBackground),
                                                         defaultValue: "ffffff",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string Align
        {
            get { return base.GetValue(AlignProperty).ToString(); }
            set { base.SetValue(AlignProperty, value); }
        }
        public static readonly BindableProperty AlignProperty = BindableProperty.Create(
                                                         propertyName: "Align",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(LogoBackground),
                                                         defaultValue: "RIGHT",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public LogoBackground()
        {
            InitializeComponent();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            if (bitmap == null && !Icon.Equals(""))
            {
                using (var stream = ResourceUtil.GetImageStream(Icon))
                {
                    bitmap = SKBitmap.Decode(stream);
                    SKImageInfo info = e.Info;
                    if (bitmap.Height > (info.Height * 0.5f))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Height * 0.5f), (int)(info.Height * 0.5f), info.ColorType, info.AlphaType, info.ColorSpace);
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

            SKRect rect = new SKRect(0, 0, width, height);
            
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;
                paint.Color = SKColor.Parse(Color);

                canvas.DrawRect(rect, paint);
            }

            //float scale = 1.5f;
            //canvas.Scale(scale);

            if (bitmap != null)
            {
                if (Align != null && Align.ToUpper().Equals("CENTER"))
                {
                    // Set the rotate transform
                    float xOffset = (width - bitmap.Width) / 2;
                    float yOffset = (height - bitmap.Height) / 2;
                    canvas.DrawBitmap(bitmap, xOffset, yOffset);
                }
                else if (Align != null && Align.ToUpper().Equals("BOTTOM_LEFT"))
                {
                    // Set the rotate transform
                    float xOffset = 16;
                    float yOffset = height - bitmap.Height - 16;
                    canvas.DrawBitmap(bitmap, xOffset, yOffset);
                }
                else if (Align != null && Align.ToUpper().Equals("BOTTOM_RIGHT"))
                {
                    // Set the rotate transform
                    float xOffset = width - bitmap.Width - 16;
                    float yOffset = height - bitmap.Height - 16;
                    canvas.DrawBitmap(bitmap, xOffset, yOffset);
                }
                else if (Align != null && Align.ToUpper().Equals("LEFT"))
                {
                    // Set the rotate transform
                    float xOffset = 16;
                    float yOffset = 16;
                    canvas.DrawBitmap(bitmap, xOffset, yOffset);
                }
                else
                {
                    // Set the rotate transform
                    float xOffset = width - bitmap.Width - 16;
                    float yOffset = 16;
                    canvas.DrawBitmap(bitmap, xOffset, yOffset);
                }
            }
        }
    }
}
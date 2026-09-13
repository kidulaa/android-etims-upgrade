using EBM2x.UI.Resource;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExtMenuButton : ContentView
	{
        private bool IsExtend = false;
        SKBitmap bitmap = null;
        SKBitmap extBitmap = null;
        private bool IsPressed = false;

        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ExtMenuButton),
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
                                                         declaringType: typeof(ExtMenuButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string Icon
        {
            get { return base.GetValue(IconProperty).ToString(); }
            set { base.SetValue(IconProperty, value); }
        }
        public static readonly BindableProperty IconProperty = BindableProperty.Create(
                                                        propertyName: "Icon",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(ExtMenuButton),
                                                        defaultValue: "funcmenu.png",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string ExtIcon
        {
            get { return base.GetValue(ExtIconProperty).ToString(); }
            set { base.SetValue(ExtIconProperty, value); }
        }
        public static readonly BindableProperty ExtIconProperty = BindableProperty.Create(
                                                        propertyName: "ExtIcon",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(ExtMenuButton),
                                                        defaultValue: "extfuncmenu.png",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string ButtonColor
        {
            get { return base.GetValue(ButtonColorProperty).ToString(); }
            set { base.SetValue(ButtonColorProperty, value); }
        }
        public static readonly BindableProperty ButtonColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(ExtMenuButton),
                                                        defaultValue: "3f922b",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string ButtonPressedColor
        {
            get { return base.GetValue(ButtonPressedColorProperty).ToString(); }
            set { base.SetValue(ButtonPressedColorProperty, value); }
        }
        public static readonly BindableProperty ButtonPressedColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonPressedColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(ExtMenuButton),
                                                        defaultValue: "98e785",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                        propertyName: "BorderColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(ExtMenuButton),
                                                        defaultValue: "93bf89",
                                                        defaultBindingMode: BindingMode.TwoWay);
        
        public ExtMenuButton()
        {
            InitializeComponent();

            skiaButton.GestureRecognizers.Add(
                    new TapGestureRecognizer()
                    {
                        Command = new Command(() => {
                            if (IsPressed == false)
                            {
                                if (ButtonClicked != null)
                                {
                                    AnimationLoop();
                                    ButtonClicked?.Invoke(this, new ExtEventArgs(FunctionID, ""));
                                }
                            }
                        })
                    }
                );
        }
        async void AnimationLoop()
        {
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
            IsPressed = true;
            while (IsPressed)
            {
                skiaButton.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(0.15));

                IsPressed = false;
            }
            skiaButton.InvalidateSurface();
        }

        public void InvalidateSurface(bool isExtend)
        {
            IsExtend = isExtend;
            skiaButton.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            if (bitmap == null && !Icon.Equals(""))
            {
                using (var stream = ResourceUtil.GetImageStream(Icon))
                {
                    bitmap = SKBitmap.Decode(stream);
                    SKImageInfo info = e.Info;
                    if (bitmap.Height > (info.Height * 0.6f) || bitmap.Height < (info.Height * 0.6f))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Height * 0.6f), (int)(info.Height * 0.6f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }

                    if ((info.Width < info.Height) && (bitmap.Width > (info.Width * 0.8f) || bitmap.Width < (info.Width * 0.8f)))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Width * 0.8f), (int)(info.Width * 0.8f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }
                }
            }
            if (extBitmap == null && !ExtIcon.Equals(""))
            {
                using (var stream = ResourceUtil.GetImageStream(ExtIcon))
                {
                    extBitmap = SKBitmap.Decode(stream);
                    SKImageInfo info = e.Info;
                    if (extBitmap.Height > (info.Height * 0.6f) || extBitmap.Height < (info.Height * 0.6f))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Height * 0.6f), (int)(info.Height * 0.6f), info.ColorType, info.AlphaType, info.ColorSpace);
                        extBitmap = extBitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }

                    if ((info.Width < info.Height) && (bitmap.Width > (info.Width * 0.8f) || bitmap.Width < (info.Width * 0.8f)))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Width * 0.8f), (int)(info.Width * 0.8f), info.ColorType, info.AlphaType, info.ColorSpace);
                        extBitmap = extBitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }
                }
            }

            if(IsExtend) PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, extBitmap);
            else PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, bitmap);
        }

        private void PaintSurface(SKCanvas canvas, float width, float height, SKBitmap bitmap)
        {
            canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(1, 1, width - 2, height - 2);
            SKSize radius = new SKSize(10, 10);
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;
                if (IsPressed) paint.Color = SKColor.Parse(ButtonPressedColor);
                else paint.Color = SKColor.Parse(ButtonColor);

                canvas.DrawRoundRect(rect, radius, paint);
            }

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 1f;
                paint.Color = SKColor.Parse(BorderColor);

                canvas.DrawRoundRect(rect, radius, paint);
            }

            if (bitmap != null)
            {
                float xOffset = (width - bitmap.Width) / 2;
                float yOffset = (height - bitmap.Height) / 2;
                canvas.DrawBitmap(bitmap, xOffset, yOffset);
            }
        }
    }
}
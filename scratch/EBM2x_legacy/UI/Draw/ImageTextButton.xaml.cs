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
	public partial class ImageTextButton : ContentView
	{
        SKBitmap bitmap = null;

        private bool IsPressed = false;

        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ImageTextButton),
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
                                                         declaringType: typeof(ImageTextButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ImageTextButton),
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
                                                        declaringType: typeof(ImageTextButton),
                                                        defaultValue: string.Empty,
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string ButtonPressedColor
        {
            get { return base.GetValue(ButtonPressedColorProperty).ToString(); }
            set { base.SetValue(ButtonPressedColorProperty, value); }
        }
        public static readonly BindableProperty ButtonPressedColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonPressedColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(ImageTextButton),
                                                        defaultValue: "98e785",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public ImageTextButton()
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

        public void InvalidateSurfaceIcon(string icon)
        {
            Icon = icon;
            skiaButton.InvalidateSurface();
        }

        public void InvalidateSurfaceText(string text)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
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
                    if (bitmap.Height > (info.Height * 0.7f) || bitmap.Height < (info.Height * 0.7f))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Height * 0.7f), (int)(info.Height * 0.7f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }

                    if ((info.Width < info.Height) && (bitmap.Width > (info.Width * 0.7f) || bitmap.Width < (info.Width * 0.7f)))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Width * 0.7f), (int)(info.Width * 0.7f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }
                }
            }

            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, bitmap);
        }

        private void PaintSurface(SKCanvas canvas, float width, float height, SKBitmap bitmap)
        {
            canvas.Clear(SKColors.Transparent);

            if (IsPressed)
            {
                SKRect rect = new SKRect(1, 1, width - 2, height - 2);
                SKSize radius = new SKSize(10, 10);
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = SKColor.Parse(ButtonPressedColor);

                    canvas.DrawRoundRect(rect, radius, paint);
                }
            }

            if (bitmap != null)
            {
                float xOffset = (width - bitmap.Width) / 2;
                float yOffset = (height - bitmap.Height) / 2;
                canvas.DrawBitmap(bitmap, xOffset, yOffset);
            }

            if (!string.IsNullOrEmpty(Text)) {
                using (var paint = new SKPaint())
                {
                    paint.TextSize = height * 0.3f;
                    paint.Color = SKColor.Parse("991f00");

                    float xOffset = 0;
                    float yOffset = (height / 2) + (paint.TextSize / 2) - (paint.TextSize * 0.15f);

                    paint.IsAntialias = true;
                    paint.TextAlign = SKTextAlign.Center;
                    xOffset = width / 2;

                    var yOffset2 = (height / 2) - (paint.TextSize / 2) + paint.TextSize - (paint.TextSize * 0.15f); ;

                    // 기본 출력 함수
                    canvas.DrawText(Text, xOffset, yOffset2, paint);
                }
            }
        }
    }
}
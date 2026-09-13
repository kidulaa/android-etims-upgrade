using EBM2x.UI.i18n;
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
	public partial class IconTextButton : ContentView
	{
        SKBitmap bitmap = null;

        private bool IsPressed = false;

        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(IconTextButton),
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
                                                         declaringType: typeof(IconTextButton),
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
                                                         declaringType: typeof(IconTextButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string ButtonColor
        {
            get { return base.GetValue(ButtonColorProperty).ToString(); }
            set { base.SetValue(ButtonColorProperty, value); }
        }
        public static readonly BindableProperty ButtonColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(IconTextButton),
                                                        defaultValue: "000000",  // a55e99
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                        propertyName: "BorderColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(IconTextButton),
                                                        defaultValue: "FFFFFF",
                                                        defaultBindingMode: BindingMode.TwoWay);
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                        propertyName: "TextColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(IconTextButton),
                                                        defaultValue: "FFFFFF",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string TextAlign
        {
            get { return base.GetValue(TextAlignProperty).ToString(); }
            set { base.SetValue(TextAlignProperty, value); }
        }
        public static readonly BindableProperty TextAlignProperty = BindableProperty.Create(
                                                         propertyName: "TextAlign",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(IconTextButton),
                                                         defaultValue: "CENTER",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public float FillRate
        {
            get { return (float)(base.GetValue(FillRateProperty)); }
            set { base.SetValue(FillRateProperty, value); }
        }
        public static readonly BindableProperty FillRateProperty = BindableProperty.Create(
                                                         propertyName: "FillRate",
                                                         returnType: typeof(float),
                                                         declaringType: typeof(IconTextButton),
                                                         defaultValue: 0.6f,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string Icon
        {
            get { return base.GetValue(IconProperty).ToString(); }
            set { base.SetValue(IconProperty, value); }
        }
        public static readonly BindableProperty IconProperty = BindableProperty.Create(
                                                        propertyName: "Icon",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(IconTextButton),
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
                                                        declaringType: typeof(IconTextButton),
                                                        defaultValue: "#fad0cf",
                                                        defaultBindingMode: BindingMode.TwoWay);
        public string ButtonDisabledColor
        {
            get { return base.GetValue(ButtonDisabledColorProperty).ToString(); }
            set { base.SetValue(ButtonDisabledColorProperty, value); }
        }
        public static readonly BindableProperty ButtonDisabledColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonDisabledColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(IconTextButton),
                                                        defaultValue: "808080",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public bool IsDisabled
        {
            get { return (bool)base.GetValue(IsDisabledProperty); }
            set { base.SetValue(IsDisabledProperty, value); }
        }
        public static readonly BindableProperty IsDisabledProperty = BindableProperty.Create(
                                                         propertyName: "IsDisabled",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(IconTextButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);


        public IconTextButton()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();

            skiaButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    Command = new Command(() => {
                        if (IsPressed == false)
                        {
                            if (ButtonClicked != null && !IsDisabled)
                            {
                                AnimationLoop();
                                if(ButtonClicked != null && !IsDisabled) ButtonClicked?.Invoke(this, new ExtEventArgs(FunctionID, ""));
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
        public void InvalidateSurface(string text)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            skiaButton.InvalidateSurface();
        }
        public void InvalidateSurfaceSetDisabled(bool isDisabled)
        {
            IsDisabled = isDisabled;
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
                    if (bitmap.Height > (info.Height * 0.3f) || bitmap.Height < (info.Height * 0.3f))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Height * 0.3f), (int)(info.Height * 0.3f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }

                    if ((info.Width < info.Height) && (bitmap.Width > (info.Width * 0.3f) || bitmap.Width < (info.Width * 0.3f)))
                    {
                        SKImageInfo resizeInfo = new SKImageInfo((int)(info.Width * 0.3f), (int)(info.Width * 0.3f), info.ColorType, info.AlphaType, info.ColorSpace);
                        bitmap = bitmap.Resize(resizeInfo, SKFilterQuality.Medium);
                    }
                }
            }

            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, bitmap);
        }

        private void PaintSurface(SKCanvas canvas, float width, float height, SKBitmap bitmap)
        {
            canvas.Clear(SKColors.Transparent);

            SKSize radius = new SKSize(6, 6);
            SKRect rect = new SKRect(0, 0, width, height);
            if (IsPressed)
            {
                rect = new SKRect(1, 1, width - 2, height - 2);
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;

                    if(IsDisabled) paint.Color = SKColor.Parse(ButtonDisabledColor);
                    else paint.Color = SKColor.Parse(ButtonPressedColor);

                    canvas.DrawRoundRect(rect, radius, paint);
                }
            }

            rect = new SKRect(0, 0, width, height);
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;
                if (IsDisabled)
                {
                    paint.Color = SKColor.Parse(ButtonDisabledColor);
                }
                else paint.Color = SKColor.Parse(ButtonColor);

                canvas.DrawRoundRect(rect, radius, paint);
                //canvas.DrawRect(rect, paint);
            }

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 1f;
                if (IsDisabled)
                {
                    paint.Color = SKColor.Parse(ButtonDisabledColor);
                }
                else paint.Color = SKColor.Parse(BorderColor);

                canvas.DrawRoundRect(rect, radius, paint);
                //canvas.DrawRect(rect, paint);
            }

            if (bitmap != null)
            {
                float xOffset = 6;
                float yOffset = (height - bitmap.Height) / 2;
                canvas.DrawBitmap(bitmap, xOffset, yOffset);
            }

            using (var paint = new SKPaint())
            {
                paint.TextSize = height * FillRate;
                paint.Color = SKColor.Parse(TextColor);

                float xOffset = 0;
                float yOffset = (height / 2) + (paint.TextSize / 2) - (paint.TextSize * 0.15f);

                paint.IsAntialias = true;
                if (TextAlign != null && TextAlign.ToUpper().Equals("CENTER"))
                {
                    paint.TextAlign = SKTextAlign.Center;
                    xOffset = width / 2;
                }
                else if (TextAlign != null && TextAlign.ToUpper().Equals("RIGHT"))
                {
                    paint.TextAlign = SKTextAlign.Right;
                    xOffset = width - (paint.TextSize / 3);
                }
                else
                {
                    paint.TextAlign = SKTextAlign.Left;
                    xOffset = bitmap.Width + 12;
                }

                // 다국어 처리
                string locationText = UILocation.Instance().GetLocationText(Text);
                string[] result = locationText.Split('|');

                var yOffset2 = (height / 2) - ((result.Length * paint.TextSize) / 2) + paint.TextSize - (paint.TextSize * 0.15f); ;

                foreach (string s in result)
                {
                    // 기본 출력 함수
                    canvas.DrawText(s, xOffset, yOffset2, paint);

                    //// 한글 출력 함수
                    //// DrawTools.DrawString(canvas, s, (int)(height * 0.7f), SKColors.Black, xOffset, yOffset);

                    yOffset2 += paint.TextSize;
                }
            }
        }
    }
}
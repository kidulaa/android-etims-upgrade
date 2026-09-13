using EBM2x.UI.i18n;
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
	public partial class TextButton : ContentView
	{
        private bool IsPressed = false;

        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(TextButton),
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
                                                         declaringType: typeof(TextButton),
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
                                                         declaringType: typeof(TextButton),
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
                                                        declaringType: typeof(TextButton),
                                                        defaultValue: "62b64e",  // a55e99
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                        propertyName: "BorderColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(TextButton),
                                                        defaultValue: "a55e00",
                                                        defaultBindingMode: BindingMode.TwoWay);
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                        propertyName: "TextColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(TextButton),
                                                        defaultValue: "FFFF00",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string TextAlign
        {
            get { return base.GetValue(TextAlignProperty).ToString(); }
            set { base.SetValue(TextAlignProperty, value); }
        }
        public static readonly BindableProperty TextAlignProperty = BindableProperty.Create(
                                                         propertyName: "TextAlign",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(TextButton),
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
                                                         declaringType: typeof(TextButton),
                                                         defaultValue: 0.6f,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string ButtonPressedColor
        {
            get { return base.GetValue(ButtonPressedColorProperty).ToString(); }
            set { base.SetValue(ButtonPressedColorProperty, value); }
        }
        public static readonly BindableProperty ButtonPressedColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonPressedColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(TextButton),
                                                        defaultValue: "FF6699",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public bool IsEditable { get; set; }

        public TextButton()
        {
            InitializeComponent();

            IsEditable = true;

            skiaButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    Command = new Command(() => {
                        if (IsPressed == false)
                        {
                            if (ButtonClicked != null)
                            {
                                if (IsEditable)
                                {
                                    AnimationLoop();
                                    ButtonClicked?.Invoke(this, new ExtEventArgs(FunctionID, ""));
                                }
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

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void PaintSurface(SKCanvas canvas, float width, float height)
        {
            canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(0, 0, width, height);
            if (IsPressed)
            {
                rect = new SKRect(1, 1, width - 2, height - 2);
                SKSize radius = new SKSize(10, 10);
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = SKColor.Parse(ButtonPressedColor);

                    canvas.DrawRoundRect(rect, radius, paint);
                }
            }

            rect = new SKRect(0, 0, width, height);
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;
                if (IsEditable)
                {
                    paint.Color = SKColor.Parse(ButtonColor);
                }
                else
                {
                    paint.Color = SKColor.Parse("A9A9A9");
                }

                canvas.DrawRect(rect, paint);
            }

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 1f;
                paint.Color = SKColor.Parse(BorderColor);

                canvas.DrawRect(rect, paint);
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
                    xOffset = 0;
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
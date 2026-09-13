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
	public partial class NumberButton : ContentView
	{
        private bool IsPressed = false;

        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(NumberButton),
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
                                                         declaringType: typeof(NumberButton),
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
                                                         declaringType: typeof(NumberButton),
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
                                                        declaringType: typeof(NumberButton),
                                                        defaultValue: "4e4e4e",
                                                        defaultBindingMode: BindingMode.TwoWay);
        public string ButtonPressedColor
        {
            get { return base.GetValue(ButtonPressedColorProperty).ToString(); }
            set { base.SetValue(ButtonPressedColorProperty, value); }
        }
        public static readonly BindableProperty ButtonPressedColorProperty = BindableProperty.Create(
                                                        propertyName: "ButtonPressedColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(NumberButton),
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
                                                        declaringType: typeof(NumberButton),
                                                        defaultValue: "9E9E9E",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                        propertyName: "TextColor",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(NumberButton),
                                                        defaultValue: "FFFFFF",
                                                        defaultBindingMode: BindingMode.TwoWay);

        public float FillRate
        {
            get { return (float)(base.GetValue(FillRateProperty)); }
            set { base.SetValue(FillRateProperty, value); }
        }
        public static readonly BindableProperty FillRateProperty = BindableProperty.Create(
                                                         propertyName: "FillRate",
                                                         returnType: typeof(float),
                                                         declaringType: typeof(NumberButton),
                                                         defaultValue: 0.7f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public NumberButton()
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

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void PaintSurface(SKCanvas canvas, float width, float height)
        {
            canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(1, 1, width - 2, height - 2);
            SKSize radius = new SKSize(10, 10);
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;
                if(IsPressed) paint.Color = SKColor.Parse(ButtonPressedColor);
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

            if (!string.IsNullOrEmpty(Text))
            {
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.TextAlign = SKTextAlign.Center;
                    paint.TextSize = height * FillRate;
                    paint.Color = SKColor.Parse(TextColor);

                    var xOffset = width / 2;
                    var yOffset = (height / 2) + (paint.TextSize / 2) - (paint.TextSize * 0.15f);

                    // 다국어 처리
                    //string locationText = UILocation.Instance().GetLocationText(Text);
                    canvas.DrawText(Text, xOffset, yOffset, paint);
                }
            }
        }
    }
}
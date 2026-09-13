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
	public partial class DrawNumber : ContentView
	{
        private bool IsNumberVisible = false;

        public double Number
        {
            get { return (double)(base.GetValue(NumberProperty)); }
            set { base.SetValue(NumberProperty, value); }
        }
        public static readonly BindableProperty NumberProperty = BindableProperty.Create(
                                                         propertyName: "Number",
                                                         returnType: typeof(double),
                                                         declaringType: typeof(DrawNumber),
                                                         defaultValue: 0.0d,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string TextAlign
        {
            get { return base.GetValue(TextAlignProperty).ToString(); }
            set { base.SetValue(TextAlignProperty, value); }
        }
        public static readonly BindableProperty TextAlignProperty = BindableProperty.Create(
                                                         propertyName: "TextAlign",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawNumber),
                                                         defaultValue: "RIGHT",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string Format
        {
            get { return base.GetValue(FormatProperty).ToString(); }
            set { base.SetValue(FormatProperty, value); }
        }
        public static readonly BindableProperty FormatProperty = BindableProperty.Create(
                                                         propertyName: "Format",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawNumber),
                                                         defaultValue: "#,##0.00",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawNumber),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public float FillRate
        {
            get { return (float)(base.GetValue(FillRateProperty)); }
            set { base.SetValue(FillRateProperty, value); }
        }
        public static readonly BindableProperty FillRateProperty = BindableProperty.Create(
                                                         propertyName: "FillRate",
                                                         returnType: typeof(float),
                                                         declaringType: typeof(DrawNumber),
                                                         defaultValue: 0.6f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawNumber()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(double number, bool isVisible)
        {
            IsNumberVisible = isVisible;
            Number = number;
            canvasView.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void PaintSurface(SKCanvas canvas, float width, float height)
        {
            // clear the surface
            canvas.Clear(SKColors.Transparent);

            if(IsNumberVisible)
            {
                string Text = string.Format("{0:" + Format + "}", Number);
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

                    // 기본 출력 함수
                    canvas.DrawText(Text, xOffset, yOffset, paint);
                }
            }
        }
    }
}
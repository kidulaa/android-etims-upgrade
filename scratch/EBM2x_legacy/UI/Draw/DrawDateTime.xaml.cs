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
	public partial class DrawDateTime : ContentView
	{
        bool pageIsActive;
        //Stopwatch stopwatch = new Stopwatch();

        public string TextAlign
        {
            get { return base.GetValue(TextAlignProperty).ToString(); }
            set { base.SetValue(TextAlignProperty, value); }
        }
        public static readonly BindableProperty TextAlignProperty = BindableProperty.Create(
                                                         propertyName: "TextAlign",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawDateTime),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public float FillRate
        {
            get { return (float)(base.GetValue(FillRateProperty)); }
            set { base.SetValue(FillRateProperty, value); }
        }
        public static readonly BindableProperty FillRateProperty = BindableProperty.Create(
                                                         propertyName: "FillRate",
                                                         returnType: typeof(float),
                                                         declaringType: typeof(DrawDateTime),
                                                         defaultValue: 0.6f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string Format
        {
            get { return base.GetValue(FormatProperty).ToString(); }
            set { base.SetValue(FormatProperty, value); }
        }
        public static readonly BindableProperty FormatProperty = BindableProperty.Create(
                                                         propertyName: "Format",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawDateTime),
                                                         defaultValue: "dd/MM/yyyy HH:mm:ss",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawDateTime),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawDateTime()
        {
            InitializeComponent();

            Start();
        }

        public void Start()
        {
            pageIsActive = true;
            AnimationLoop();
        }

        public void Stop()
        {
            pageIsActive = false;
        }

        async void AnimationLoop()
        {
            //// Start animation
            //stopwatch.Start();

            while (true)
            {
                if(pageIsActive) canvasView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(1.0));
            }

            //// Stop animation
            //stopwatch.Stop();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void PaintSurface(SKCanvas canvas, float width, float height)
        {
            string text = DateTime.Now.ToString(Format);

            // clear the surface
            canvas.Clear(SKColors.Transparent);

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

                canvas.DrawText(text, xOffset, yOffset, paint);
            }
        }
    }
}
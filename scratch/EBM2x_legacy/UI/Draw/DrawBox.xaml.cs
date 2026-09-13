using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawBox : ContentView
	{
        public string Color
        {
            get { return base.GetValue(ColorProperty).ToString(); }
            set { base.SetValue(ColorProperty, value); }
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
                                                         propertyName: "Color",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawBox),
                                                         defaultValue: "FFFFFF",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string StrokeColor
        {
            get { return base.GetValue(StrokeColorProperty).ToString(); }
            set { base.SetValue(StrokeColorProperty, value); }
        }
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
                                                         propertyName: "StrokeColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawBox),
                                                         defaultValue: "FFFFFF",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsStroke
        {
            get { return (bool)base.GetValue(IsStrokeProperty); }
            set { base.SetValue(IsStrokeProperty, value); }
        }
        public static readonly BindableProperty IsStrokeProperty = BindableProperty.Create(
                                                         propertyName: "IsStroke",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(DrawBox),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public bool IsRound
        {
            get { return (bool)base.GetValue(IsRoundProperty); }
            set { base.SetValue(IsRoundProperty, value); }
        }
        public static readonly BindableProperty IsRoundProperty = BindableProperty.Create(
                                                         propertyName: "IsRound",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(DrawBox),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawBox()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(string color)
        {
            Color = color;
            canvasView.InvalidateSurface();
        }

        public void InvalidateSurface(bool isStroke)
        {
            IsStroke = isStroke;
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

            SKSize radius = new SKSize(6, 6);
            SKRect rect = new SKRect(0, 0, width, height);

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;
                paint.Color = SKColor.Parse(Color);

                if(IsRound) canvas.DrawRoundRect(rect, radius, paint);
                else canvas.DrawRect(rect, paint);

                if (IsStroke)
                {
                    paint.Style = SKPaintStyle.Stroke;
                    if (StrokeColor.Equals("000000")) paint.StrokeWidth = 1f;
                    else paint.StrokeWidth = 3f;
                    paint.Color = SKColor.Parse(StrokeColor);

                    if (IsRound) canvas.DrawRoundRect(rect, radius, paint);
                    else canvas.DrawRect(rect, paint);
                }
            }
        }
    }
}
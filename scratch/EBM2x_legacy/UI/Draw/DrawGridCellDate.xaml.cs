using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Draw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DrawGridCellDate : ContentView
	{
        public string Text
        {
            get {
                //if (base.GetValue(TextProperty) == null) return "0";
                return base.GetValue(TextProperty).ToString(); 
            }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawGridCellDate),
                                                         defaultValue: "",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string TextAlign
        {
            get { return base.GetValue(TextAlignProperty).ToString(); }
            set { base.SetValue(TextAlignProperty, value); }
        }
        public static readonly BindableProperty TextAlignProperty = BindableProperty.Create(
                                                         propertyName: "TextAlign",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawGridCellDate),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawGridCellDate),
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
                                                         declaringType: typeof(DrawGridCellDate),
                                                         defaultValue: 0.3f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public string BoxColor
        {
            get { return base.GetValue(BoxColorProperty).ToString(); }
            set { base.SetValue(BoxColorProperty, value); }
        }
        public static readonly BindableProperty BoxColorProperty = BindableProperty.Create(
                                                         propertyName: "BoxColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawGridCellDate),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string StrokeColor
        {
            get { return base.GetValue(StrokeColorProperty).ToString(); }
            set { base.SetValue(StrokeColorProperty, value); }
        }
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
                                                         propertyName: "StrokeColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DrawGridCellDate),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsStroke
        {
            get { return (bool)base.GetValue(IsStrokeProperty); }
            set { base.SetValue(IsStrokeProperty, value); }
        }
        public static readonly BindableProperty IsStrokeProperty = BindableProperty.Create(
                                                         propertyName: "IsStroke",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(DrawGridCellDate),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public DrawGridCellDate()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(string text)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            canvasView.InvalidateSurface();
        }
        public void InvalidateSurface(string text, string textAlign, string textColor, float fillRate)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            TextAlign = textAlign;
            TextColor = textColor;
            FillRate = fillRate;
            canvasView.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(0, 0, e.Info.Width-1, e.Info.Height-1);

            using (var paint = new SKPaint())
            {
                if (!string.IsNullOrEmpty(BoxColor))
                { 
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = SKColor.Parse(BoxColor);

                    e.Surface.Canvas.DrawRect(rect, paint);
                }

                if (IsStroke)
                {
                    paint.Style = SKPaintStyle.Stroke;
                    if(StrokeColor.Equals("000000")) paint.StrokeWidth = 1f;
                    else paint.StrokeWidth = 3f;
                    paint.Color = SKColor.Parse(StrokeColor);

                    e.Surface.Canvas.DrawRect(rect, paint);
                }
            }

            try
            {
                if(Text.Length == 8)
                {
                    DrawTools.PaintSurfaceAppend(e.Surface.Canvas, e.Info.Width, e.Info.Height, DateFormat(Text), FillRate, TextAlign, SKColor.Parse(TextColor));
                }
                else if (Text.Length == 14)
                {
                    DrawTools.PaintSurfaceAppend(e.Surface.Canvas, e.Info.Width, e.Info.Height, DateTimeFormat(Text), FillRate, TextAlign, SKColor.Parse(TextColor));
                }
                else
                {
                    DrawTools.PaintSurfaceAppend(e.Surface.Canvas, e.Info.Width, e.Info.Height, Text, FillRate, TextAlign, SKColor.Parse(TextColor));
                }
            }
            catch
            {
                DrawTools.PaintSurfaceAppend(e.Surface.Canvas, e.Info.Width, e.Info.Height, "", FillRate, TextAlign, SKColor.Parse(TextColor));
            }

        }

        private string DateFormat(string date)
        {
            string result = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");
            return result;
        }
        private string DateTimeFormat(string date)
        {
            string result = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm:ss");
            return result;
        }
    }
}
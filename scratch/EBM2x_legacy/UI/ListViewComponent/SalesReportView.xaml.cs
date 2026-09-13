using EBM2x.Models.ListView;
using EBM2x.Models.regitotal;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.ListViewComponent
{ 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesReportView : ContentView
    {
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(SalesReportView),
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
                                                         declaringType: typeof(SalesReportView),
                                                         defaultValue: 0.4f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public ClassTotal Node
        {
            get { return (ClassTotal)base.GetValue(NodeProperty); }
            set { base.SetValue(NodeProperty, value); }
        }
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(
            propertyName: "Node",
            returnType: typeof(ClassTotal),
            declaringType: typeof(SalesReportView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public SalesReportView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }

        private void OnPaintingClassCode(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.ClassCode, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingClassName(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.ClassName, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingQuantity(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, string.Format("{0:#,##0}", Node.Count), FillRate, "RIGHT", SKColor.Parse(TextColor));
        }
        private void OnPaintingAmount(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, string.Format("{0:#,##0.00}",Node.SubtotalAmount), FillRate, "RIGHT", SKColor.Parse(TextColor));
        }
    }
}

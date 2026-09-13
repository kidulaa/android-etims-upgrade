using EBM2x.Models.ListView;
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
    public partial class SearchReceiptView : ContentView
    {
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(SearchReceiptView),
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
                                                         declaringType: typeof(SearchReceiptView),
                                                         defaultValue: 0.3f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public SearchReceiptNode Node
        {
            get { return (SearchReceiptNode)base.GetValue(NodeProperty); }
            set { base.SetValue(NodeProperty, value); }
        }
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(
            propertyName: "Node",
            returnType: typeof(SearchReceiptNode),
            declaringType: typeof(SearchReceiptView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public SearchReceiptView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }

        private void OnPaintingSalesDate(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.SalesDate, FillRate, "CENTER", SKColor.Parse(TextColor));
        }
        private void OnPaintingReceiptNo(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.ReceiptNo, FillRate, "CENTER", SKColor.Parse(TextColor));
        }
        private void OnPaintingSalesType(object sender, SKPaintSurfaceEventArgs e)
        {
            string text = "Normal";
            if (Node.Sign < 0) text = "Refund";
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, text, FillRate, "CENTER", SKColor.Parse(TextColor));
            //DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.SalesType, FillRate, "CENTER", SKColor.Parse(TextColor));
        }
        private void OnPaintingInvoiceNum(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.InvoiceNum, FillRate, "CENTER", SKColor.Parse(TextColor));
        }
        private void OnPaintingAmount(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, string.Format("{0:#,##0.00}",Node.Amount), FillRate, "RIGHT", SKColor.Parse(TextColor));
        }
    }
}

using EBM2x.Models.ListView;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using EBM2x.Utils;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.ListViewComponent
{ 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchItemView : ContentView
    {
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(SearchItemView),
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
                                                         declaringType: typeof(SearchItemView),
                                                         defaultValue: 0.5f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public SearchItemNode Node
        {
            get { return (SearchItemNode)base.GetValue(NodeProperty); }
            set { base.SetValue(NodeProperty, value); }
        }
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(
            propertyName: "Node",
            returnType: typeof(SearchItemNode),
            declaringType: typeof(SearchItemView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public SearchItemView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }

        private void OnPaintingItemCode(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.ItemCode, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingItemName(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.ItemName, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingBarcode(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.Barcode, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingBatchNumber(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.BarchNumber, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingExpirationDate(object sender, SKPaintSurfaceEventArgs e)
        {
            string textPrice = "";
            if (!string.IsNullOrEmpty(Node.ExpirationDate))
            {
                textPrice = Common.DateFormat(Node.ExpirationDate).ToString("dd-MM-yyyy");
            }
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, textPrice, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingStockQuantity(object sender, SKPaintSurfaceEventArgs e)
        {
            var textPrice = String.Format("{0:#,##0} ", Node.StockQty);
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, textPrice, FillRate, "RIGHT", SKColor.Parse(TextColor));
        }
        private void OnPaintingPrice(object sender, SKPaintSurfaceEventArgs e)
        {
            var textPrice = String.Format("{0:#,##0} ", Node.Price);
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, textPrice, FillRate, "RIGHT", SKColor.Parse(TextColor));
        }

    }
}

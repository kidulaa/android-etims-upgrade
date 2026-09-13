using EBM2x.Models.journal;
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
    public partial class SearchReceiptDetailView : ContentView
    {
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(SearchReceiptDetailView),
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
                                                         declaringType: typeof(SearchReceiptDetailView),
                                                         defaultValue: 0.4f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public JournalString Node
        {
            get { return (JournalString)base.GetValue(NodeProperty); }
            set { base.SetValue(NodeProperty, value); }
        }
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(
            propertyName: "Node",
            returnType: typeof(JournalString),
            declaringType: typeof(SearchReceiptDetailView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public SearchReceiptDetailView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }

        private void OnPaintingJournalString(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.Data, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
    }
}

using EBM2x.Models.config;
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
    public partial class SearchUserView : ContentView
    {
        public string TextColor
        {
            get { return base.GetValue(TextColorProperty).ToString(); }
            set { base.SetValue(TextColorProperty, value); }
        }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                         propertyName: "TextColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(SearchUserView),
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
                                                         declaringType: typeof(SearchUserView),
                                                         defaultValue: 0.7f,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public OperatorRecord Node
        {
            get { return (OperatorRecord)base.GetValue(NodeProperty); }
            set { base.SetValue(NodeProperty, value); }
        }
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(
            propertyName: "Node",
            returnType: typeof(OperatorRecord),
            declaringType: typeof(SearchUserView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public SearchUserView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }

        private void OnPaintingTin(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.OperatorCode, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
        private void OnPaintingCustomerName(object sender, SKPaintSurfaceEventArgs e)
        {
            DrawTools.PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height, Node.OperatorName, FillRate, "LEFT", SKColor.Parse(TextColor));
        }
    }
}

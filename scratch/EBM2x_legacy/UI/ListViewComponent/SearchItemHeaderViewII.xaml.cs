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
    public partial class SearchItemHeaderViewII : ContentView
    {
        public SearchItemHeaderViewII()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }
    }
}

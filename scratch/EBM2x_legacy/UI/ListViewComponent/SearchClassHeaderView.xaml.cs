using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.ListViewComponent
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchClassHeaderView : ContentView
    {
        public SearchClassHeaderView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }
    }
}

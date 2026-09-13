using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.ListViewComponent
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchTinHeaderView : ContentView
    {
        public SearchTinHeaderView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }
    }
}

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.ListViewComponent
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchUserHeaderView : ContentView
    {
        public SearchUserHeaderView()
        {
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }
    }
}

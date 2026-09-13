using Xamarin.Forms;

namespace EBM2x.Dependency
{
    public class Toast
    {
        public static void Show(string message)
        {
            IToast toast = DependencyService.Get<IToast>();
            if(toast != null) toast.Show(message);
        }
    }
}

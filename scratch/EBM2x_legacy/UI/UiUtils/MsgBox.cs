using EBM2x.UI.i18n;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EBM2x.UI.UiUtils
{
    public class MsgBox
    {

        public static async void DisplayAlert(ContentPage page, string title, string message, string button1)
        {
            string locationTitle = UILocation.Instance().GetLocationText(title);
            string locationMessage = UILocation.Instance().GetLocationText(message);
            string locationButton1 = UILocation.Instance().GetLocationText(button1);

            await page.DisplayAlert(locationTitle, locationMessage, locationButton1);
        }
    }
}

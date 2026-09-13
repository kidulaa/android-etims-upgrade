using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.UI.Web
{
    public class NoticeWebView : ContentPage
    {
        public NoticeWebView(string uri)
        {
            var browser = new WebView();
            browser.Source = uri;
            Content = browser;
        }
    }
}

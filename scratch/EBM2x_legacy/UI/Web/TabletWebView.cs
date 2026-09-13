using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.UI.Web
{
    public class TabletWebView : ContentPage
    {
        public TabletWebView(string uri)
        {
            var browser = new WebView();
            browser.Source = uri;

            //var htmlSource = new HtmlWebViewSource();
            //htmlSource.Html = @"<html><body>
            //                    <h1>Xamarin.Forms</h1>
            //                    <p>Welcome to WebView.</p>
            //                    </body>
            //                    </html>";
            //browser.Source = htmlSource;
            Content = browser;
        }
    }
}

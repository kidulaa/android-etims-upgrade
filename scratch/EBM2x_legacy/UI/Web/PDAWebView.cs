using EBM2x.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.UI.Web
{
    public class PDAWebView : ContentPage
    {
        public PDAWebView()
        {
            var browser = new WebView();
            browser.Source = "http://192.168.24.43:9220/indexSwDownloadAndroid?swNo=2&ver=1.0&tin=999992114&bhfId=00&dvcSrlNo=dvc211400";

            //var htmlSource = new HtmlWebViewSource();
            //htmlSource.Html = @"<html><body>
            //                    <h1>Xamarin.Forms</h1>
            //                    <p>Welcome to WebView.</p>
            //                    </body>
            //                    </html>";
            //browser.Source = htmlSource; 
            Content = browser;

            browser.Navigating += (object sender, WebNavigatingEventArgs e) => {
                if (e.Url.ToLower().Contains("forcesave=true"))
                {
                    var downloadProcessor = DependencyService.Get<IDownloadProcessor>();
                    downloadProcessor.DownloadFromUrl(e.Url);
                }
            };
        }
    }
}

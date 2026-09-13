using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneScanningPopupPage : ContentPage
    {
        ZXingScannerView zxing;

        public static readonly BindableProperty IsTorchOnProperty = BindableProperty.Create(
            nameof(IsTorchOn), 
            typeof(bool), 
            typeof(PhoneScanningPopupPage), 
            false);
        public bool IsTorchOn
        {
            get { return (bool)GetValue(IsTorchOnProperty); }
            set { SetValue(IsTorchOnProperty, value); }
        }

        public static readonly BindableProperty IsAnalyzingProperty = BindableProperty.Create(
            nameof(IsAnalyzing), 
            typeof(bool), 
            typeof(PhoneScanningPopupPage), 
            false);
        public bool IsAnalyzing
        {
            get { return (bool)GetValue(IsAnalyzingProperty); }
            set { SetValue(IsAnalyzingProperty, value); }
        }

        public static readonly BindableProperty IsScanningProperty = BindableProperty.Create(
            nameof(IsScanning), 
            typeof(bool), 
            typeof(ZXingScannerPage), 
            false);
        public bool IsScanning
        {
            get { return (bool)GetValue(IsScanningProperty); }
            set { SetValue(IsScanningProperty, value); }
        }

        public static readonly BindableProperty HasTorchProperty = BindableProperty.Create(
            nameof(HasTorch), 
            typeof(bool), 
            typeof(PhoneScanningPopupPage), 
            false);
        public bool HasTorch
        {
            get { return (bool)GetValue(HasTorchProperty); }
            set { SetValue(HasTorchProperty, value); }
        }

        public static readonly BindableProperty ResultProperty = BindableProperty.Create(
            nameof(Result), 
            typeof(Result), 
            typeof(PhoneScanningPopupPage), 
            default(Result));
        public Result Result
        {
            get { return (Result)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public PhoneScanningPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                // Options = null,
                AutomationId = "zxingScannerView"
            };

            zxing.SetBinding(ZXingScannerView.IsTorchOnProperty, new Binding(nameof(IsTorchOn)));
            zxing.SetBinding(ZXingScannerView.IsAnalyzingProperty, new Binding(nameof(IsAnalyzing)));
            zxing.SetBinding(ZXingScannerView.IsScanningProperty, new Binding(nameof(IsScanning)));
            zxing.SetBinding(ZXingScannerView.HasTorchProperty, new Binding(nameof(HasTorch)));
            zxing.SetBinding(ZXingScannerView.ResultProperty, new Binding(nameof(Result)));

            zxing.OnScanResult += (result) => {
                this.OnScanResult?.Invoke(result);
                //Device.BeginInvokeOnMainThread (() => eh (result));
            };

            var gridOverlay = new Grid()
            {
                AutomationId = "gridOverlay",
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            gridOverlay.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            gridOverlay.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            gridOverlay.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            gridOverlay.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            gridOverlay.Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Overlay = gridOverlay;

            ScannerView.Children.Add(zxing);
            ScannerView.Children.Add(Overlay);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Scanning");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Scan the barcode");

            zxing.IsScanning = true;
            
            etBarcodeEntry.Focus();
            etBarcodeEntry.Completed += (sender, e) => {
                if (!string.IsNullOrEmpty(etBarcodeEntry.Text))
                {
                    ZXing.Result result = new ZXing.Result(etBarcodeEntry.Text, null, null, BarcodeFormat.EAN_13);
                    this.OnScanResult?.Invoke(result);
                    //if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
                }
            };
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();
        }

        public delegate void ScanResultDelegate(ZXing.Result result);
        public event ScanResultDelegate OnScanResult;
        public event EventHandler OnCanceled;

        public View Overlay
        {
            get;
            private set;
        }

        #region Functions

        public void ToggleTorch()
        {
            if (zxing != null)
                zxing.ToggleTorch();
        }

        public void PauseAnalysis()
        {
            if (zxing != null)
                zxing.IsAnalyzing = false;
        }

        public void ResumeAnalysis()
        {
            if (zxing != null)
                zxing.IsAnalyzing = true;
        }

        public void AutoFocus()
        {
            if (zxing != null)
                zxing.AutoFocus();
        }

        public void AutoFocus(int x, int y)
        {
            if (zxing != null)
                zxing.AutoFocus(x, y);
        }

        #endregion

        void OnCancelButtonClicked(object sender, EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

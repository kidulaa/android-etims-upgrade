using EBM2x.Database.Master;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.CustomerManagement;
using EBM2x.UI.Backoffice.Environment;
using EBM2x.UI.Backoffice.ImportManagement;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Backoffice.PurchaseManagement;
using EBM2x.UI.Backoffice.SalesManagement;
using EBM2x.UI.Backoffice.StockManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using EBM2x.UI.Tablet;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackofficeMenuPage : ContentPage
    {
        TrnsSaleMaster trnsSaleMaster = null;
        TrnsPurchaseMaster trnsPurchaseMaster = null;
        ImportItemMaster importItemMaster = null;

        bool pageIsActive;
        public BackofficeMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            fixedGridEnvironment.InitializeComponent();
            fixedGridItem.InitializeComponent();
            fixedGridCustomer.InitializeComponent();
            fixedGridStock.InitializeComponent();
            fixedGridAdmin.InitializeComponent();

            fixedGridAdmin.IsVisible = false;

            string AuthCd = UIManager.Instance().UserModel.AuthCd;
            if (UIManager.Instance().UserModel.RoleCd != "1")
            {
                if (!AuthCd.Contains("SETTING;"))
                {
                    textButtonSetting.IsEditable = false;
                }
                if (!AuthCd.Contains("USERMGT;"))
                {
                    textButtonUser.IsEditable = false;
                }

                if (!AuthCd.Contains("CUSTOMER;"))
                {
                    textButtonCustomer.IsEditable = false;
                }

                if (!AuthCd.Contains("STOCK;"))
                {
                    textButtonStock.IsEditable = false;
                }
                if (!AuthCd.Contains("IMPORT;"))
                {
                    textButtonImport.IsEditable = false;
                }
                if (!AuthCd.Contains("PURCHASE;"))
                {
                    textButtonPurchase.IsEditable = false;
                }

                //if (!AuthCd.Contains("REFUND;") ? true : false);
                //if(!AuthCd.Contains("PRICE;") ? true : false);
                //if(!AuthCd.Contains("PROFORMA;") ? true : false);

                //if(!AuthCd.Contains("SALERPT;") ? true : false);
                //if (!AuthCd.Contains("ZREPORT;") ? true : false) ;
                //if (!AuthCd.Contains("ADJUST;") ? true : false) ;
            }

            trnsSaleMaster = new TrnsSaleMaster();
            trnsPurchaseMaster = new TrnsPurchaseMaster();
            importItemMaster = new ImportItemMaster();

            textUserName.InvalidateSurface(UIManager.Instance().UserModel.UserNm);
        }
        public BackofficeMenuPage(bool adminMode)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            fixedGridEnvironment.InitializeComponent();
            fixedGridItem.InitializeComponent();
            fixedGridCustomer.InitializeComponent();
            fixedGridStock.InitializeComponent();
            fixedGridAdmin.InitializeComponent();

            fixedGridAdmin.IsVisible = adminMode;

            trnsSaleMaster = new TrnsSaleMaster();
            trnsPurchaseMaster = new TrnsPurchaseMaster();
            importItemMaster = new ImportItemMaster();

            textUserName.InvalidateSurface(UIManager.Instance().UserModel.UserNm);

            pageIsActive = true;
            AnimationLoop();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetSubMenuVisible(null);
            pageIsActive = true;

            long countWaitSales = trnsSaleMaster.GetWaitCount();
            long countWaitPurchase = trnsPurchaseMaster.GetWaitCount();
            long countWaitImport = importItemMaster.GetWaitCount();
            textWaitSales.InvalidateSurface(countWaitSales.ToString("#,##0"));
            textWaitPurchase.InvalidateSurface(countWaitPurchase.ToString("#,##0"));
            textWaitImport.InvalidateSurface(countWaitImport.ToString("#,##0"));
        }

        protected override void OnDisappearing()
        {
            pageIsActive = false;
            base.OnDisappearing();
        }

        async void AnimationLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(300.0));
                if (pageIsActive)
                {
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();
                }
            }
        }

        void OnBoxButtonClicked(object sender, EventArgs e)
        {
            SetSubMenuVisible(null);
        }

        async void OnFunctionLogout(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to Logout?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;
            
            Navigation.InsertPageBefore(new SalesMenuPage(), this);
            await Navigation.PopAsync();
        }
        void OnFunctionSubMenuVisible(object sender, EventArgs e)
        {
            SetSubMenuVisible(((ExtEventArgs)e).FunctionID); 
        }

        void SetSubMenuVisible(string mainMenu)
        {
            fixedGridEnvironment.IsVisible = false;
            if (!string.IsNullOrEmpty(mainMenu) && mainMenu.Equals("Environment")) fixedGridEnvironment.IsVisible = true;

            fixedGridItem.IsVisible = false;
            if (!string.IsNullOrEmpty(mainMenu) && mainMenu.Equals("Item")) fixedGridItem.IsVisible = true;

            fixedGridCustomer.IsVisible = false;
            if (!string.IsNullOrEmpty(mainMenu) && mainMenu.Equals("Customer")) fixedGridCustomer.IsVisible = true;

            fixedGridStock.IsVisible = false;
            if (!string.IsNullOrEmpty(mainMenu) && mainMenu.Equals("Stock")) fixedGridStock.IsVisible = true;
        }
        async void OnFunctionSubMenu(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "UserManagement":
                    await Navigation.PushAsync(new UserManagementPage());
                    break;
                case "ChangePassword":
                    await Navigation.PushAsync(new ChangePasswordPage());
                    break;
                case "SystemSetting":
                    await Navigation.PushAsync(new SystemSettingPage());
                    break;
                case "ItemManagement":
                    await Navigation.PushAsync(new ItemManagementPage());
                    break;
                case "CustomerManagement":
                    await Navigation.PushAsync(new CustomerManagementPage());
                    break;
                case "InsurerManagement":
                    await Navigation.PushAsync(new InsurerManagementPage());
                    break;
                case "SalesManagement":
                    await Navigation.PushAsync(new SalesManagementPage());
                    break;
                case "PurchaseManagement":
                    await Navigation.PushAsync(new PurchaseManagementPage());
                    break;
                case "ImportManagement":
                    await Navigation.PushAsync(new ImportManagementPage());
                    break;
                case "OpeningClosingStock":
                    await Navigation.PushAsync(new OpeningClosingStockPage());
                    break;
                case "StockStatus":
                    await Navigation.PushAsync(new StockStatusPage());
                    break;
                case "StockInHistory":
                    await Navigation.PushAsync(new StockInHistoryPage());
                    break;
                case "StockOutHistory":
                    await Navigation.PushAsync(new StockOutHistoryPage());
                    break;
                default:
                    break;
            }
        }

        async void OnLocationButtonClicked(object sender, EventArgs e)
        {
            var locationPopupPage = new TabletLocationPopupPage();
            locationPopupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            locationPopupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UILocation.Instance().SetLocation(((ExtEventArgs)ex).EnteredText);
                    //MessagingCenter.Send<Object, string>(this, "Location", ((FunctionEventArgs)e).EnteredText);

                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            locationPopupPage.OnCanceled += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our location page
            await Navigation.PushAsync(locationPopupPage);
        }
    }
}
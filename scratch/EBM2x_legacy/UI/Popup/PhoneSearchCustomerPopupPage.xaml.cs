using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Process;
using EBM2x.Process.search;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneSearchCustomerPopupPage : ContentPage
    {
        bool TinMode = false;
        public ObservableCollection<SearchCustomerListViewModel> SearchCustomerList { get; set; }
        List<SearchCustomerNode> listSearchCustomerNode;

        public PhoneSearchCustomerPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SearchCustomerList = new ObservableCollection<SearchCustomerListViewModel>();
            listView.ItemsSource = SearchCustomerList;
            searchEntry.GetEntry().Completed += (sender, e) => {
                OnQueryButtonClicked(sender, e);
            };
        }
        public PhoneSearchCustomerPopupPage(bool tinMode)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            TinMode = tinMode;
            btnConfirm.IsVisible = false;

            SearchCustomerList = new ObservableCollection<SearchCustomerListViewModel>();
            listView.ItemsSource = SearchCustomerList;
            searchEntry.GetEntry().Completed += (sender, e) => {
                OnQueryButtonClicked(sender, e);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            searchEntry.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Search customer");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Customer");
            searchEntry.GetEntry().Focus();

            if (!TinMode)
            {
                OnQueryButtonClicked(this, null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchEntry.GetEntry().Text)) searchEntry.GetEntry().Text = "";

            UIManager.Instance().InformationModel.SetWarningMessage("");
            SearchCustomerList.Clear();

            EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
            if (TinMode)
            {
                if (string.IsNullOrEmpty(searchEntry.GetEntry().Text) || searchEntry.GetEntry().Text.Length < 3)
                {
                    UIManager.Instance().InformationModel.SetWarningMessage("Please enter at least 3 characters.");
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "Please enter at least 3 characters.", "Ok");
                }

                listSearchCustomerNode = SearchCustomerNodeProcess.QuerydSearchTinCustomer(envPosSetup.GblTaxIdNo, envPosSetup.GblBrcCod, searchEntry.GetEntry().Text);
                // JINIT_메시지 표시
                if (listSearchCustomerNode.Count == 0)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                    return;
                }

                foreach (SearchCustomerNode node in listSearchCustomerNode)
                {
                    SearchCustomerList.Add(new SearchCustomerListViewModel { Node = node });
                }
            }
            else
            {
                listSearchCustomerNode = SearchCustomerNodeProcess.QuerydSearchCustomer(envPosSetup.GblTaxIdNo, envPosSetup.GblBrcCod, searchEntry.GetEntry().Text);
                // JINIT_메시지 표시
                if (listSearchCustomerNode.Count == 0)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                    return;
                }

                foreach (SearchCustomerNode node in listSearchCustomerNode)
                {
                    SearchCustomerList.Add(new SearchCustomerListViewModel { Node = node });
                }
            }
        }
        async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchEntry.GetEntry().Text) && searchEntry.GetEntry().Text.Length > 4)
            {
                if (searchEntry.GetEntry().Text.Length > 9)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter 9 characters or less.", "Ok");
                    return;
                }
                string locationTitle2 = UILocation.Instance().GetLocationText("TIN?");
                string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure? [" + searchEntry.GetEntry().Text + "]");
                var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                if (result)
                {
                    SearchCustomerNode node = new SearchCustomerNode();
                    node.Tin = searchEntry.GetEntry().Text;
                    node.CustomerCode = searchEntry.GetEntry().Text;
                    node.CustomerName = "";
                    ExtEventArgs extEventArgs = new ExtEventArgs("Select", node);
                    OnResult?.Invoke(this, extEventArgs);
                }
            }
            else
            {
                UIManager.Instance().InputModel.Clear();
                if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
            }
        }
        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                SearchCustomerNode node = new SearchCustomerNode();
                node.Tin = "";
                node.CustomerCode = "";
                node.CustomerName = "";
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }
        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (OnResult != null && e.SelectedItem != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", ((SearchCustomerListViewModel)e.SelectedItem).Node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

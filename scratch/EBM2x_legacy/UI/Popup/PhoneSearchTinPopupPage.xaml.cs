using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Process;
using EBM2x.Process.search;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneSearchTinPopupPage : ContentPage
    {
        public ObservableCollection<SearchTinListViewModel> SearchTinList { get; set; }

        public PhoneSearchTinPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SearchTinList = new ObservableCollection<SearchTinListViewModel>();
            listView.ItemsSource = SearchTinList;

            searchEntry.GetEntry().Completed += (sender, e) => {
                OnQueryButtonClicked(sender, e);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            searchEntry.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Search Tin");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Tin");
            searchEntry.GetEntry().Focus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchEntry.GetEntry().Text) || searchEntry.GetEntry().Text.Length < 9)
            {
                UIManager.Instance().InformationModel.SetWarningMessage("Please enter at least 9 characters.");
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "Please enter at least 9 characters.", "Ok");
            }
            else {
                UIManager.Instance().InformationModel.SetWarningMessage("");
                SearchTinList.Clear();

                EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
                List<SearchTinNode> list = SearchTinNodeProcess.QuerydSearchTin(searchEntry.GetEntry().Text);
                // JINIT_메시지 표시
                if (list.Count == 0)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                    return;
                }

                foreach (SearchTinNode node in list)
                {
                    SearchTinList.Add(new SearchTinListViewModel { Node = node });
                }
            }
        }
        void OnCancelButtonClicked(object sender, EventArgs e)
        {
                UIManager.Instance().InputModel.Clear();
                if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }
        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (OnResult != null && e.SelectedItem != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", ((SearchTinListViewModel)e.SelectedItem).Node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

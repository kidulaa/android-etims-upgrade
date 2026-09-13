using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Process.search;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletSearchItemPopupPage : ContentPage
    {
        public ObservableCollection<SearchItemListViewModel> SearchItemList { get; set; }
        List<SearchItemNode> listSearchItemNode;

        public TabletSearchItemPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchItemList = new ObservableCollection<SearchItemListViewModel>();
            listView.ItemsSource = SearchItemList;

            searchEntry.GetEntry().Completed += (sender, e) => {
                OnQueryButtonClicked(sender, e);
            };
        }
        public TabletSearchItemPopupPage(List<SearchItemNode> list)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchItemList = new ObservableCollection<SearchItemListViewModel>();
            listView.ItemsSource = SearchItemList;

            listSearchItemNode = list;
            AnimationLoop(list);
        }

        async void AnimationLoop(List<SearchItemNode> list)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            if (list != null && list.Count > 0)
            {
                SearchItemList.Clear();
                foreach (SearchItemNode node in list)
                {
                    SearchItemList.Add(new SearchItemListViewModel { Node = node });
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


            UIManager.Instance().PosModel.SetSalesTitleText("Search item");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Item");

            searchEntry.GetEntry().Focus(); 
            
            if (listSearchItemNode == null || listSearchItemNode.Count <= 0)
            {
                AnimationLoop();
            }
        }

        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
            listSearchItemNode = SearchItemProcess.QuerydSearchItem(envPosSetup.GblTaxIdNo, searchEntry.GetEntryValue());
            foreach (SearchItemNode node in listSearchItemNode)
            {
                SearchItemList.Add(new SearchItemListViewModel { Node = node });
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
            //if (string.IsNullOrEmpty(searchEntry.GetEntry().Text) || searchEntry.GetEntry().Text.Length < 3)
            //{
            //    UIManager.Instance().InformationModel.SetWarningMessage("Please enter at least 3 characters.");
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "Please enter at least 3 characters.", "Ok");
            //}

            UIManager.Instance().InformationModel.SetWarningMessage("");
            SearchItemList.Clear();

            EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
            listSearchItemNode = SearchItemProcess.QuerydSearchItem(envPosSetup.GblTaxIdNo, searchEntry.GetEntry().Text);

            // JINIT_메시지 표시
            if (listSearchItemNode.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                return;
            }

            foreach (SearchItemNode node in listSearchItemNode)
            {
                SearchItemList.Add(new SearchItemListViewModel { Node = node });
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
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", ((SearchItemListViewModel)e.SelectedItem).Node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

using EBM2x.Models.ListView;
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
    public partial class TabletRefundReasonPopupPage : ContentPage
    {
        public ObservableCollection<SearchRefundReasonListViewModel> SearchRefundReasonList { get; set; }

        public TabletRefundReasonPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SearchRefundReasonList = new ObservableCollection<SearchRefundReasonListViewModel>();
            listView.ItemsSource = SearchRefundReasonList;
        }

        void GetSearchItemList()
        {
            SearchRefundReasonList.Clear();

            List<SearchRefundReasonNode> list = SearchRefundReasonProcess.QuerydSearchRefundReason();
            foreach (SearchRefundReasonNode node in list)
            {
                SearchRefundReasonList.Add(new SearchRefundReasonListViewModel { Node = node });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetSearchItemList();

            UIManager.Instance().PosModel.SetSalesTitleText("Refund|Reason");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Item");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (OnResult != null && e.SelectedItem != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", ((SearchRefundReasonListViewModel)e.SelectedItem).Node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

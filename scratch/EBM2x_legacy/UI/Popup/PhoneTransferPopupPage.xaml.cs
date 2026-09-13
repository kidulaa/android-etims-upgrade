using EBM2x.Models.ListView;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneTransferPopupPage : ContentPage
    {
        public ObservableCollection<SearchItemListViewModel> SearchItemList { get; set; }

        public PhoneTransferPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchItemList = new ObservableCollection<SearchItemListViewModel>();
            listView.ItemsSource = SearchItemList;

            GetSearchItemList();
        }

        void GetSearchItemList()
        {
            SearchItemList.Clear();

            for (int i = 0; i < 45; i++)
            {
                SearchItemNode node = new SearchItemNode
                {
                    ItemCode = string.Format("88012345678{0:00}", i),
                    ItemName = "item name " + i,
                    Price = 100 * i
                };
                SearchItemList.Add(new SearchItemListViewModel { Node = node });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("Search item");
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

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            //if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }
        void OnSelectButtonClicked(object sender, EventArgs e)
        {
            if (OnResult != null) OnResult?.Invoke(this, new ExtEventArgs("Select", ((ExtEventArgs)e).FunctionID, null));
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

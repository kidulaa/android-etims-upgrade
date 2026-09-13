using EBM2x.Datafile.trlog;
using EBM2x.Models.hold;
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
    public partial class TabletRestoreHoldPopupPage : ContentPage
    {
        public ObservableCollection<HoldNodeListViewModel> SearchItemList { get; set; }

        public TabletRestoreHoldPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SearchItemList = new ObservableCollection<HoldNodeListViewModel>();
            listView.ItemsSource = SearchItemList;
        }

        void GetSearchItemList()
        {
            SearchItemList.Clear();

            List<HoldNode> list = TrHoldReader.GetHoldingList();

            foreach (HoldNode node in list)
            {
                // JINIT_시간표시형식 변경(99:99:99)
                try
                {
                    node.UpdateTime = node.UpdateTime.Substring(0, 2) + ":" + node.UpdateTime.Substring(2, 2) + ":" + node.UpdateTime.Substring(4, 2);
                }
                catch { }
                
                SearchItemList.Add(new HoldNodeListViewModel { Node = node });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetSearchItemList();

            UIManager.Instance().PosModel.SetSalesTitleText("Restore Hold");
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
            //JINIT_, if (OnResult != null)
            if (OnResult != null && e.SelectedItem != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", ((HoldNodeListViewModel)e.SelectedItem).Node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

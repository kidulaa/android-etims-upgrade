using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Process;
using EBM2x.Process.search;
using EBM2x.Services;
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
    public partial class PhoneSearchUserPopupPage : ContentPage
    {
        public ObservableCollection<SearchUserListViewModel> SearchUserList { get; set; }

        public PhoneSearchUserPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            SearchUserList = new ObservableCollection<SearchUserListViewModel>();
            listView.ItemsSource = SearchUserList;
            searchEntry.GetEntry().Completed += (sender, e) => {
                OnQueryButtonClicked(sender, e);
            };

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            searchEntry.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Search User");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a User");
            
            //searchEntry.GetEntry().Focus();
            AnimationLoop();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            List<OperatorRecord> list = OperatorService.GetOperatorList();
            foreach (OperatorRecord node in list)
            {
                SearchUserList.Add(new SearchUserListViewModel { Node = node });
            }
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InformationModel.SetWarningMessage("");
            SearchUserList.Clear();

            List<OperatorRecord> list = OperatorService.GetOperatorList();
            // JINIT_메시지 표시
            if (list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Data not found.", "OK");
                return;
            }

            foreach (OperatorRecord node in list)
            {
                SearchUserList.Add(new SearchUserListViewModel { Node = node });
            }
        }
        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                OperatorRecord node = new OperatorRecord();
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }
        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (OnResult != null && e.SelectedItem != null)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", ((SearchUserListViewModel)e.SelectedItem).Node);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

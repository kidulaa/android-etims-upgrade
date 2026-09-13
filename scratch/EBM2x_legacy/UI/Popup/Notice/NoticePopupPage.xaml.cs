using EBM2x.Database.Master;
using EBM2x.UI.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup.Notice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoticePopupPage : ContentPage
    {
 
        public ObservableCollection<BbsNoticeRecord> SearchNoticeList { get; set; }
        BbsNoticeMaster master = new BbsNoticeMaster();
        string curDate = ""; // JINIT_당일영업일자

        public NoticePopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();

            // JINIT_영업일자설정
            curDate = UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate;
            //salesDateEntry.SetDateTime(DateTime.ParseExact(curDate, "yyyyMMdd", null));
            //salesDateEntry.TitleInvalidateSurface("Notice");

            UIManager.Instance().PosModel.SetSalesTitleText("Notice");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please enter");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AnimationLoop();
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            List<BbsNoticeRecord> list = master.getBbsNoticeTable(curDate);
            SetList(list);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        async void OnSearch(object sender, EventArgs e)
        {
            //curDate = salesDateEntry.GetDateTime().ToString("yyyyMMdd");
            List<BbsNoticeRecord> list = master.getBbsNoticeTable(curDate);
            if(list == null)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                return;
            }
            SetList(list);
            if (list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<BbsNoticeRecord> datas)
        {
            try
            {
                SearchNoticeList = new ObservableCollection<BbsNoticeRecord>();

                listView.ItemsSource = SearchNoticeList;

                for (int i = 0; i < datas.Count; i++)
                {
                    SearchNoticeList.Add(datas[i]);
                }
            }
            catch
            {
            }
        }
        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            await Navigation.PopAsync(); 
        }
        //public void OpenBrowser(Uri uri)
        //{
        //    //await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        //}
        async private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                BbsNoticeRecord record = (BbsNoticeRecord)e.SelectedItem;
                if (!string.IsNullOrEmpty(record.DtlUrl)) {
                    if (UIManager.Instance().IsWindows)
                    {
                        // Windows 인 경우
                        NoticeWebView noticeWebView = new NoticeWebView(record.DtlUrl);
                        await Navigation.PushAsync(noticeWebView);
                    }
                    else
                    {
                        // Tablet 인 경우 
                        await Browser.OpenAsync(record.DtlUrl);
                    }
                }
            }
        }

        void OnQueryButtonClicked(object sender, EventArgs e)
        {
            OnSearch(this, null);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

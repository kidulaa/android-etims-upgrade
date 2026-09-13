using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Process;
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
    public partial class TabletSearchOriginPopupPage : ContentPage
    {
        public ObservableCollection<CodeDtlRecord> lvOriginManagement { get; set; }
        List<CodeDtlRecord> listCodeDtlRecord;
        CodeDtlMaster master = null;
        CodeDtlRecord record = null;
        public TabletSearchOriginPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            master = new CodeDtlMaster();
            record = new CodeDtlRecord();

            searchEntry.GetEntry().Completed += (sender, e) => {
                OnQueryButtonClicked(sender, e);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            searchEntry.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Search Class");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Class");

            searchEntry.GetEntry().Focus();

            if(listCodeDtlRecord == null || listCodeDtlRecord.Count <= 0)
            {
                AnimationLoop();
            }
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            listCodeDtlRecord = master.getCodeDtlTable("05", "");
            SetList(listCodeDtlRecord);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            string valueLike = searchEntry.GetEntryValue();
            //if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 2)
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 2 characters.", "OK");
            //    return;
            //}
            listCodeDtlRecord = master.getCodeDtlTable("05", valueLike);
            SetList(listCodeDtlRecord);
            if (listCodeDtlRecord.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<CodeDtlRecord> datas)
        {
            try
            {
                lvOriginManagement = new ObservableCollection<CodeDtlRecord>();
                listView.ItemsSource = lvOriginManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvOriginManagement.Add(datas[i]);
                }
            }
            catch
            {
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
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", (CodeDtlRecord)e.SelectedItem);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

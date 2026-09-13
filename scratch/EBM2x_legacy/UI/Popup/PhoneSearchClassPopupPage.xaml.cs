using EBM2x.Database.Master;
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
    public partial class PhoneSearchClassPopupPage : ContentPage
    {
        public ObservableCollection<ItemClassRecord> lvCustManagement { get; set; }
        bool IsSelected = false;
        ItemClassMaster master = null;
        ItemClassRecord record = null;

        public PhoneSearchClassPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            master = new ItemClassMaster();
            record = new ItemClassRecord();

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

            etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "3", Name = "" });

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
            string valueLike = searchEntry.GetEntryValue();
            if (string.IsNullOrEmpty(valueLike)) valueLike = "";
            string valueUseYn = "";
            if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 2)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 2 characters.", "OK");
                return;
            }
            List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, etClassificationLvlEntryPanel.GetSelectedItem().Id);
            SetList(list);
            if (list.Count == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
            }
        }
        void SetList(List<ItemClassRecord> datas)
        {
            try
            {
                lvCustManagement = new ObservableCollection<ItemClassRecord>();
                listView.ItemsSource = lvCustManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvCustManagement.Add(datas[i]);
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
        async void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (OnResult != null && e.SelectedItem != null)
            {
                record = (ItemClassRecord)e.SelectedItem;

                if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("1"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 2);
                    string valueUseYn = "";
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "2");

                    if (list == null || list.Count <= 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                    }
                    else
                    {
                        SetList(list);
                        searchEntry.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "2", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("2"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 4);
                    string valueUseYn = "";
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "3");

                    if (list == null || list.Count <= 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                    }
                    else
                    {
                        SetList(list);
                        searchEntry.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "3", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("3"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 6);
                    string valueUseYn = "";
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "4");

                    if (list == null || list.Count <= 0)
                    {
                        //Added by Aime
                        //IsSelected = true;
                        SetList(list);
                        searchEntry.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "4", Name = "" });

                        //END
                        /*
                         * Commented By Bright when Incorporating Aime Code
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                        */
                    }
                    else
                    {
                        SetList(list);
                        searchEntry.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "4", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("4"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 8);
                    string valueUseYn = "";
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "5");

                    if (list == null || list.Count <= 0)
                    {
                        SetList(list);
                        searchEntry.SetEntryValue(record.ItemClsNm);
                        //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                    }
                    else
                    {
                        SetList(list);
                        searchEntry.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "5", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("5") && record.ItemClsCd.Length == 10)
                {
                    ExtEventArgs extEventArgs = new ExtEventArgs("Select", (ItemClassRecord)e.SelectedItem);
                    OnResult?.Invoke(this, extEventArgs);
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }


    }
}

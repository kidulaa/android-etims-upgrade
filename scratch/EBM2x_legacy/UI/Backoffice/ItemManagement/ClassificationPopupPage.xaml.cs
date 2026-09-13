using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.ItemManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassificationPopupPage : ContentPage
    {
        public ObservableCollection<ItemClassRecord> ItemClassRecords { get; set; }

        bool IsSelected = false;
        ItemClassMaster master = null;
        ItemClassRecord record = null;

        public ClassificationPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new ItemClassMaster();
            record = new ItemClassRecord();

            etSearchUsable.SetSelecteItem(new SystemCode() { Id = "Y", Name = "" });

            etLikeValue.GetEntry().Completed += (sender, e) => {
                OnSearch(sender, e);
            };

            etSearchUsable.SetReadOnly(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "3", Name = "" });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 
        }
        async void OnSearch(object sender, EventArgs e)
        {
            string valueLike = etLikeValue.GetEntryValue();
            if (string.IsNullOrEmpty(valueLike)) valueLike = "";
            string valueUseYn = etSearchUsable.GetSelectedItem().Id;
            if (string.IsNullOrEmpty(valueLike) || valueLike.Length < 2)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "Please enter at least 2 characters.", "OK");
                return;
            }

            IsSelected = false;
            SetEntityData(new ItemClassRecord(), true);

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
                ItemClassRecords = new ObservableCollection<ItemClassRecord>();
                listView.ItemsSource = ItemClassRecords;

                for (int i = 0; i < datas.Count; i++)
                {
                    record = datas[i];
                    ItemClassRecords.Add(record);
                }
            }
            catch
            {
            }
        }

        void OnFunctionCancel(object sender, EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        void OnFunctionConfirm(object sender, EventArgs e)
        {
            if (OnResult != null && IsSelected)
            {
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", record);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        private async void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                IsSelected = false;
                SetEntityData(new ItemClassRecord(), true);

                record = (ItemClassRecord)e.SelectedItem;

                if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("1"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 2);
                    string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "2");

                    if(list == null || list.Count <= 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                    }
                    else
                    {
                        SetList(list);
                        etLikeValue.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "2", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("2"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 4);
                    string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "3");

                    if (list == null || list.Count <= 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                    }
                    else
                    {
                        SetList(list);
                        etLikeValue.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "3", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("3"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 6);
                    string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "4");

                    if (list == null || list.Count <= 0)
                    {

                        //Added by Aime
                        IsSelected = true;
                        SetEntityData(record, true);
                        //END
                        /*
                         * Commented By Bright when Incorporating Aime Code
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                        */
                    }
                    else
                    {
                        SetList(list);
                        etLikeValue.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "4", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("4"))
                {
                    string valueLike = record.ItemClsCd.Substring(0, 8);
                    string valueUseYn = etSearchUsable.GetSelectedItem().Id;
                    List<ItemClassRecord> list = master.getItemClassTable(valueLike, valueUseYn, "5");

                    if (list == null || list.Count <= 0)
                    {
                        //Added by Aime
                        IsSelected = true;
                        SetEntityData(record, true);
                        //END
                        /*
                         * 
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "There are no results for the query. Please try again.", "OK");
                        */
                    }
                    else
                    {
                        SetList(list);
                        etLikeValue.SetEntryValue(record.ItemClsNm);
                        etClassificationLvlEntryPanel.SetSelecteItem(new SystemCode() { Id = "5", Name = "" });
                    }
                }
                else if (etClassificationLvlEntryPanel.GetSelectedItem().Id.Equals("5") && record.ItemClsCd.Length == 10)
                {
                    IsSelected = true;
                    SetEntityData(record, true);
                }
            }
            else
            {
                IsSelected = false;
                SetEntityData(new ItemClassRecord(), true);
            }
        }

        private void SetEntityData(ItemClassRecord record, bool readOnly)
        {
            etItemClsCd.SetEntryValue(record.ItemClsCd);
            etItemClsCd.SetReadOnly(readOnly);
            etItemClsNm.SetEntryValue(record.ItemClsNm);
            etItemClsNm.SetReadOnly(readOnly);
        }

        private void OnClearUse(object sender, EventArgs e)
        {
            etSearchUsable.SetSelecteItem(new SystemCode());
        }
    }
}

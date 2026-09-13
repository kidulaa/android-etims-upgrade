using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.Services;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletStoreManagementPopupPage : ContentPage
    {

        public TabletStoreManagementPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            entityStoreCode.TitleInvalidateSurface();           // 가맹점 코드
            entityStoreName.TitleInvalidateSurface();           // 가맹점 명
            entityNameplateLine1.TitleInvalidateSurface();      // 명판메시지1
            entityNameplateLine2.TitleInvalidateSurface();      // 명판메시지2
            entityNameplateLine3.TitleInvalidateSurface();      // 명판메시지3
            entityNameplateLine4.TitleInvalidateSurface();      // 명판메시지4
            entityStoreMessage1.TitleInvalidateSurface();       // 영수증상단메시지 1
            entityStoreMessage2.TitleInvalidateSurface();       // 영수증상단메시지 2
            entityMessage1.TitleInvalidateSurface();            // 영수증하단메시지 1
            entityMessage2.TitleInvalidateSurface();            // 영수증하단 2
            entityMessage3.TitleInvalidateSurface(); 			// 영수증하단 3

            UIManager.Instance().PosModel.SetSalesTitleText("Store|Management");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Item");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            StoreRecord record = StoreService.Load(entityStoreCode.GetEntry().Text);

            entityStoreName.GetEntry().Text = record.StoreName;

            entityNameplateLine1.GetEntry().Text = record.NameplateLine1;
            entityNameplateLine2.GetEntry().Text = record.NameplateLine2;
            entityNameplateLine3.GetEntry().Text = record.NameplateLine3;
            entityNameplateLine4.GetEntry().Text = record.NameplateLine4;

            entityStoreMessage1.GetEntry().Text = record.StoreMessage1;
            entityStoreMessage2.GetEntry().Text = record.StoreMessage2;

            entityMessage1.GetEntry().Text = record.Message1;
            entityMessage2.GetEntry().Text = record.Message2;
            entityMessage3.GetEntry().Text = record.Message3;

            if (string.IsNullOrEmpty(record.StoreCode))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "There is no data.", "Ok");
            }
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entityStoreCode.GetEntryValue()) || entityStoreCode.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the StoreCode.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityStoreName.GetEntryValue()) || entityStoreName.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the StoreName.", "Ok");
                return;
            }
            string locationTitle2 = UILocation.Instance().GetLocationText("Save?");
            string locationMessage2 = UILocation.Instance().GetLocationText("Do you want to save data?");
            var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
            if (result)
            {

                StoreRecord record = new StoreRecord();

                record.StoreCode = entityStoreCode.GetEntry().Text;
                record.StoreName = entityStoreName.GetEntry().Text;

                record.NameplateLine1 = entityNameplateLine1.GetEntry().Text;
                record.NameplateLine2 = entityNameplateLine2.GetEntry().Text;
                record.NameplateLine3 = entityNameplateLine3.GetEntry().Text;
                record.NameplateLine4 = entityNameplateLine4.GetEntry().Text;

                record.StoreMessage1 = entityStoreMessage1.GetEntry().Text;
                record.StoreMessage2 = entityStoreMessage2.GetEntry().Text;

                record.Message1 = entityMessage1.GetEntry().Text;
                record.Message2 = entityMessage2.GetEntry().Text;
                record.Message3 = entityMessage3.GetEntry().Text;

                StoreService.Save(record);
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully.", "Ok");

                record.clear();
                entityStoreCode.GetEntry().Text = record.StoreCode;
                entityStoreName.GetEntry().Text = record.StoreName;

                entityNameplateLine1.GetEntry().Text = record.NameplateLine1;
                entityNameplateLine2.GetEntry().Text = record.NameplateLine2;
                entityNameplateLine3.GetEntry().Text = record.NameplateLine3;
                entityNameplateLine4.GetEntry().Text = record.NameplateLine4;

                entityStoreMessage1.GetEntry().Text = record.StoreMessage1;
                entityStoreMessage2.GetEntry().Text = record.StoreMessage2;

                entityMessage1.GetEntry().Text = record.Message1;
                entityMessage2.GetEntry().Text = record.Message2;
                entityMessage3.GetEntry().Text = record.Message3;
            }

            UIManager.Instance().InputModel.Clear();
        }
        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
        }
        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            await Navigation.PopAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

using EBM2x.Models.config;
using EBM2x.Services;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneUserManagementPopupPage : ContentPage
    {
        bool NewFlag = true;

        public PhoneUserManagementPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            entityPassword.GetEntry().IsPassword = true;
            entityUserName.GetEntry().MaxLength = 60;

            entityUserCode.SetReadOnly(!NewFlag);
            entityUserName.SetReadOnly(!NewFlag);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

		    entityUserCode.TitleInvalidateSurface(); 	    // 담당자코드
		    entityUserName.TitleInvalidateSurface(); 	    // 담당자명

		    entityPassword.TitleInvalidateSurface(); 		// 비밀번호
		    entityTelNo.TitleInvalidateSurface(); 			// 전화번호
		    entityPermission.TitleInvalidateSurface();    	// 권한등급            

            UIManager.Instance().PosModel.SetSalesTitleText("User|Management");
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

            if (!string.IsNullOrEmpty(entityUserCode.GetEntryValue()))
            {
                OperatorRecord record = OperatorService.Load(entityUserCode.GetEntry().Text);

                entityUserCode.SetEntryValue(record.OperatorCode);        // 담당자명
                entityUserName.SetEntryValue(record.OperatorName);        // 담당자명

                entityPassword.SetEntryValue(record.Password);            // 비밀번호
                entityTelNo.SetEntryValue(record.TelNo);                  // 전화번호
                entityPermission.SetSelecteItem(record.Permission);       // 권한등급       

                if (string.IsNullOrEmpty(record.OperatorCode))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "There is no data.", "Ok");
                    NewFlag = true;
                }
                else
                {
                    NewFlag = false;
                    entityUserCode.SetReadOnly(!NewFlag);
                    entityUserName.SetReadOnly(!NewFlag);
                }
            }
            else
            {
                var popupPage = new PhoneSearchUserPopupPage();
                popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

                popupPage.OnResult += (popup, ex) => {
                    Device.BeginInvokeOnMainThread(() => {
                        OperatorRecord record = (OperatorRecord)((ExtEventArgs)ex).EnteredObject;

                        entityUserCode.SetEntryValue(record.OperatorCode);        // 담당자명
                        entityUserName.SetEntryValue(record.OperatorName);        // 담당자명

                        entityPassword.SetEntryValue(record.Password);            // 비밀번호
                        entityTelNo.SetEntryValue(record.TelNo);                  // 전화번호
                        entityPermission.SetSelecteItem(record.Permission);       // 권한등급      

                        NewFlag = false;
                        entityUserCode.SetReadOnly(!NewFlag);
                        entityUserName.SetReadOnly(!NewFlag);

                        Navigation.PopAsync();
                    });
                };
                popupPage.OnCanceled += (senderX, eX) => {
                    Device.BeginInvokeOnMainThread(() => {
                        UIManager.Instance().InputModel.Clear();
                        Navigation.PopAsync();
                    });
                };
                await Navigation.PushAsync(popupPage);
            }
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entityUserCode.GetEntryValue()) || entityUserCode.GetEntryValue().Length < 4)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the User Code.", "Ok");
                return;
            }
            if (entityUserCode.GetEntryValue().Length > 20) // 2021.7.24
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter up to 20 characters.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityUserName.GetEntryValue()) || entityUserName.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the User Name.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityPassword.GetEntryValue()) || entityPassword.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Password.", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(entityPermission.GetSelectedItem()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please select the Permission.", "Ok");
                return;
            }
            string locationTitle2 = UILocation.Instance().GetLocationText("Save?");
            string locationMessage2 = UILocation.Instance().GetLocationText("Do you want to save data?");
            var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
            if (result)
            {
                OperatorRecord record = new OperatorRecord();

                record.OperatorCode = entityUserCode.GetEntry().Text;        // 담당자코드
                record.OperatorName = entityUserName.GetEntry().Text;        // 담당자명

                record.Password = entityPassword.GetEntry().Text;            // 비밀번호
                record.TelNo = entityTelNo.GetEntry().Text;                  // 전화번호
                record.Permission = entityPermission.GetSelectedItem();      // 권한등급     

                if (NewFlag)
                {
                    OperatorRecord recordNew = OperatorService.Load(entityUserCode.GetEntry().Text);
                    if (NewFlag && !string.IsNullOrEmpty(recordNew.OperatorCode))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This code already exists.", "Ok");
                        return;
                    }
                }

                OperatorService.Save(record);

                // JCNA : CLEAR 202001
                record = new OperatorRecord();

                entityUserCode.SetEntryValue(record.OperatorCode);        // 담당자명
                entityUserName.SetEntryValue(record.OperatorName);        // 담당자명

                entityPassword.SetEntryValue(record.Password);            // 비밀번호
                entityTelNo.SetEntryValue(record.TelNo);                  // 전화번호
                entityPermission.SetSelecteItem(record.Permission);       // 권한등급   

                NewFlag = true;
                entityUserCode.SetReadOnly(!NewFlag);
                entityUserName.SetReadOnly(!NewFlag);

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully.", "Ok");
            }
            UIManager.Instance().InputModel.Clear();
        }
        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            NewFlag = true;
            entityUserCode.SetReadOnly(!NewFlag);
            entityUserName.SetReadOnly(!NewFlag);
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

        private void OnNewButtonClicked(object sender, EventArgs e)
        {
            NewFlag = true;
            entityUserCode.SetReadOnly(!NewFlag);
            entityUserName.SetReadOnly(!NewFlag);

            OperatorRecord record = new OperatorRecord();

            entityUserCode.SetEntryValue(record.OperatorCode);        // 담당자명
            entityUserName.SetEntryValue(record.OperatorName);        // 담당자명

            entityPassword.SetEntryValue(record.Password);            // 비밀번호
            entityTelNo.SetEntryValue(record.TelNo);                  // 전화번호
            entityPermission.SetSelecteItem(record.Permission);       // 권한등급   
        }
    }
}

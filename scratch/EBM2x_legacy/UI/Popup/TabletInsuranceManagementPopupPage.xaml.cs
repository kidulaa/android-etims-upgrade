using EBM2x.Database.Master;
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
    public partial class TabletInsuranceManagementPopupPage : ContentPage
    {

        public TabletInsuranceManagementPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

		    entityUserCode.TitleInvalidateSurface(); 	    // 담당자코드
		    entityUserName.TitleInvalidateSurface(); 	    // 담당자명

		    entityPassword.TitleInvalidateSurface(); 		// 비밀번호
		    entityTelNo.TitleInvalidateSurface(); 			// 전화번호
		    entityPermission.TitleInvalidateSurface();    	// 권한등급            

            UIManager.Instance().PosModel.SetSalesTitleText("Insurance|Management");
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
            TaxpayerBhfInsuranceRecord record = new TaxpayerBhfInsuranceRecord();

            EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;

            //bool ret = TaxpayerBhfInsuranceMaster.ToRecord(record, entityUserCode.GetEntry().Text);

            //entityUserName.GetEntry().Text = record.OperatorName;        // 담당자명

            //entityPassword.GetEntry().Text = record.Password;            // 비밀번호
            //entityTelNo.GetEntry().Text = record.TelNo;                  // 전화번호
            ////entityPermission.GetEntry().Text = record.Permission;    	 // 권한등급            

            if (string.IsNullOrEmpty(record.IssrccCd))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "There is no data.", "Ok");
            }

            UIManager.Instance().InputModel.Clear();
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
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
                                                                             //record.Permission = entityPermission.GetEntry().Text;    	 // 권한등급     

                OperatorService.Save(record);
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

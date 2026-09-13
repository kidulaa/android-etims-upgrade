using EBM2x.Database.Master;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.Environment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePasswordPage : ContentPage
    {
        TaxpayerBhfDeviceUserMaster master = null;
        TaxpayerBhfDeviceUserRecord record = null;

        public ChangePasswordPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBhfDeviceUserMaster();
            record = new TaxpayerBhfDeviceUserRecord();

            etCurrentPassword.SetEntryValue("");
            etNewPassword.SetEntryValue("");
            etConfirmPassword.SetEntryValue("");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            etCurrentPassword.SetFocus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton , true: BackButton
        }

        async void OnFunctionSave(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(etCurrentPassword.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the old password.", "Ok");
                etCurrentPassword.SetFocus();
                return;
            }
            if (string.IsNullOrEmpty(etNewPassword.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the New password.", "Ok");
                etNewPassword.SetFocus();
                return;
            }
            if (string.IsNullOrEmpty(etConfirmPassword.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please Confirm the new password.", "Ok");
                etConfirmPassword.SetFocus();
                return;
            }

            TaxpayerBhfDeviceUserRecord userRecord = new TaxpayerBhfDeviceUserRecord();
            TaxpayerBhfDeviceUserMaster userMaster = new TaxpayerBhfDeviceUserMaster();
            bool ret = userMaster.ToRecord(userRecord, UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo,
                UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod,
                UIManager.Instance().UserModel.UserId);
            // CurrentPassword 
            if (ret && etCurrentPassword.GetEntryValue().Equals(userRecord.Pwd)) 
            {
                // NewPassword ConfirmPassword 
                if (etNewPassword.GetEntryValue().Equals(etConfirmPassword.GetEntryValue()))
                {                    
                    userRecord.Pwd = etNewPassword.GetEntryValue();

                    userRecord.RegrId = UIManager.Instance().UserModel.UserId;
                    userRecord.RegrNm = UIManager.Instance().UserModel.UserNm;
                    userRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    
                    userMaster.ChangePassword(userRecord);

                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Password changed successfully.", "Ok");
                    
                    etCurrentPassword.SetEntryValue("");
                    etNewPassword.SetEntryValue("");
                    etConfirmPassword.SetEntryValue("");
                    etCurrentPassword.SetFocus();
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The Confirm password must match the New Password entry.", "Ok");
                }
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Current password is incorrect.", "Ok");
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            await Navigation.PopAsync();
        }
    }
}

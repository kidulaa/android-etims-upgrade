using EBM2x.Datafile.env;
using EBM2x.Models.config;
using EBM2x.RraSdc.model;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.Environment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SystemSettingPage : ContentPage
    {
        public SystemSettingPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
            InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
            if (initInfoVO != null)
            {
                etCompanyName.SetEntryValue(initInfoVO.taxprNm);
                etCompanyName.SetReadOnly(true);
                etPresidentName.SetEntryValue(initInfoVO.bsnsActv);
                etPresidentName.SetReadOnly(true);
                etEmailAddress.SetEntryValue(initInfoVO.mgrEmail);
                etPhoneNo.SetEntryValue(initInfoVO.mgrTelNo);
                etBranchName.SetEntryValue(initInfoVO.bhfNm);
                etBranchName.SetReadOnly(true);
                etManagerName.SetEntryValue(initInfoVO.mgrNm);
                etManagerName.SetReadOnly(true);
                etAddress.SetEntryValue(envPosSetup.GblBrcAdr);  // envPosSetup.GblBrcAdr  // initInfoVO.locDesc
                etWorkMode.SetEntryValue("");
                etWorkMode.SetReadOnly(true);
                //etWelcomeMessageOnTheSale.SetEntryValue("");
                //etBottomMessageOnTheSale.SetEntryValue("");
                //etWelcomeMessageOnTheRefund.SetEntryValue("");
                //etBottomMessageOnTheRefund.SetEntryValue("");
                etTINNumber.SetEntryValue(initInfoVO.tin);
                etTINNumber.SetReadOnly(true);
                etSDCID.SetEntryValue(initInfoVO.sdcId);
                etSDCID.SetReadOnly(true);
                etAdminAccount.SetEntryValue("");
                etAdminAccount.SetReadOnly(true);
                //etSCMUrl.SetEntryValue(envPosSetup.Url0001);
                //etGenDBTable.SetEntryValue("");
                etBranchCode.SetEntryValue(initInfoVO.bhfId);
                etBranchCode.SetReadOnly(true);
                etMRCNo.SetEntryValue(initInfoVO.mrcNo);
                etMRCNo.SetReadOnly(true);
                //etAdminPassword.SetEntryValue("");
                //etGenBasicCode.SetEntryValue("");
                etDataChanged.SetEntryValue(envPosSetup.PosInstallDate);
                etDataChanged.SetReadOnly(true);

                etCompanyName.SetReadOnly(true);
                etPresidentName.SetReadOnly(true);
                etManagerName.SetReadOnly(true);

                etEmailAddress.SetReadOnly(false);
                etPhoneNo.SetReadOnly(false);
                etAddress.SetReadOnly(false);

                entryPort.SetEntryValue(envPosSetup.PrinterPort);
                etBaudRate.SetSelecteItem(envPosSetup.PrinterBaudRate);
                etPaperSize.SetSelecteItem(envPosSetup.PrinterPaperSize);


                etNonVATChanged.SetReadOnly(true);
                etNonVATChanged.SetSelecteItem(new SystemBoolCode() { Id = envPosSetup.NonVAT, Name = ""});
            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton, true: BackButton
        }

        async void OnFunctionSave(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Save");
            string locationMessage21 = UILocation.Instance().GetLocationText("Do you want to save?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if(result) { 
                EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
                InitInfoVO initInfoVO = EnvRraSdcService.LoadEnvRraSdc();
                if (initInfoVO != null)
                {
                    initInfoVO.taxprNm = etCompanyName.GetEntryValue();
                    initInfoVO.prvncNm = etPresidentName.GetEntryValue();
                    initInfoVO.mgrEmail = etEmailAddress.GetEntryValue();
                    initInfoVO.mgrTelNo = etPhoneNo.GetEntryValue();
                    initInfoVO.mgrNm = etManagerName.GetEntryValue();

                    EnvRraSdcService.SaveEnvRraSdc(initInfoVO);

                    envPosSetup.Init();
                    envPosSetup.GblBrcAdr = etAddress.GetEntryValue(); //initInfoVO.locDesc = etAddress.GetEntryValue();

                    envPosSetup.PrinterPort = entryPort.GetEntryValue();
                    envPosSetup.PrinterBaudRate = etBaudRate.GetSelectedItem();
                    envPosSetup.PrinterPaperSize = etPaperSize.GetSelectedItem();

                    if (string.IsNullOrEmpty(envPosSetup.PosInstallDate))
                    {
                        envPosSetup.PosInstallDate = string.Format("{0:ddMMyyyy hh:mm}", DateTime.Now);
                    }

                    envPosSetup.ChangeNonVAT = true;
                    envPosSetup.NonVAT = etNonVATChanged.GetSelectedItem().Id;

                    EnvPosSetupService.SaveEnvPosSetup(envPosSetup);
                    // 2021.03.04
                    UIManager.Instance().PosModel.Environment.EnvPosSetup = envPosSetup;
                    // 2021.03.15
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.PrinterPaperSize.Equals("80mm"))
                    {
                        UIManager.Instance().Is58mmPrinter = false;
                    }
                    else
                    {
                        UIManager.Instance().Is58mmPrinter = true;
                    }

                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "User saved successfully.", "Ok");
                }
            }
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

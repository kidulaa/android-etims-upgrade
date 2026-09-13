using EBM2x.Datafile.env;
using EBM2x.Models.config;
using EBM2x.UI.Tablet;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Setup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EBM2xPhoneDeviceSetupPage : ContentPage
    {
        EnvPosSetup envPosSetup = null;

        public EBM2xPhoneDeviceSetupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            entryTin.SetReadOnly(true);
            entryBhfId.SetReadOnly(true);

            entryOfflineDays.SetReadOnly(true);
            entryOfflineAmount.SetReadOnly(true);

            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
            {
            }
            else
            {
                textMySQLServerType.IsVisible = false;
                entryMySQLServerType.IsVisible = false; 
                textMySQLServer.IsVisible = false;
                entryMySQLServer.IsVisible = false;
                textMySQLDatabase.IsVisible = false;
                entryMySQLDatabase.IsVisible = false;
                textMySQLUid.IsVisible = false;
                entryMySQLUid.IsVisible = false;
                textMySQLPwd.IsVisible = false;
                entryMySQLPwd.IsVisible = false;
            }

            entryMySQLPwd.GetEntry().IsPassword = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UIManager.Instance().PosModel.SetSalesTitleText("POS Setup");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please enter your Information");

            envPosSetup = EnvPosSetupService.LoadEnvPosSetup();
            if (envPosSetup == null)
            {
                DisplayAlert("Error", "The device cannot be set up.", "OK");

                Navigation.InsertPageBefore(new SalesMenuPage(), this);
                Navigation.PopAsync();
            }
            else
            {
                entryTin.SetEntryValue(envPosSetup.GblTaxIdNo);
                entryBhfId.SetEntryValue(envPosSetup.GblBrcCod);

                //entryUrl0001.SetEntryValue(envPosSetup.Url0001);
                //entryUrl0002.SetEntryValue(envPosSetup.Url0002);

                // LOGO 출력 설정
                entryPrintingLogo.SetSelecteItem(new SystemCode() { Id = envPosSetup.PrintingLogo });

                entryPort.SetEntryValue(envPosSetup.PrinterPort);
                etBaudRate.SetSelecteItem(envPosSetup.PrinterBaudRate);
                etPaperSize.SetSelecteItem(envPosSetup.PrinterPaperSize);

                entryLocation.SetSelecteItem(new SystemCode() { Id = envPosSetup.LocationType, Name = "" });
                entryOfflineDays.SetEntryValue(envPosSetup.OfflineDays);
                entryOfflineAmount.SetEntryValue(envPosSetup.OfflineAmount);

                entryMySQLServerType.SetSelecteItem(envPosSetup.MySQLServerType);
                entryMySQLServer.SetEntryValue(envPosSetup.MySQLServer);
                entryMySQLDatabase.SetEntryValue(envPosSetup.MySQLDatabase);
                entryMySQLUid.SetEntryValue(envPosSetup.MySQLUid);
                entryMySQLPwd.SetEntryValue(envPosSetup.MySQLPwd);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionInitializeRraSdc(object sender, EventArgs e)
        {
            envPosSetup.GblTaxIdNo = entryTin.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.GblTaxIdNo))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryTin.SetFocus();
                return;
            }
            envPosSetup.GblBrcCod = entryBhfId.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.GblBrcCod))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryBhfId.SetFocus();
                return;
            }
            //envPosSetup.Url0001 = entryUrl0002.GetEntryValue();
            //envPosSetup.Url0002 = entryUrl0002.GetEntryValue();

            // LOGO 출력 설정
            envPosSetup.PrintingLogo = entryPrintingLogo.GetSelectedItem().Id;

            envPosSetup.PrinterPort = entryPort.GetEntryValue();
            envPosSetup.PrinterBaudRate = etBaudRate.GetSelectedItem();
            envPosSetup.PrinterPaperSize = etPaperSize.GetSelectedItem();

            envPosSetup.LocationType = entryLocation.GetSelectedItem().Id;

            envPosSetup.OfflineDays = entryOfflineDays.GetEntryValue();
            if (envPosSetup.OfflineDays > 3) envPosSetup.OfflineDays = 3;
            envPosSetup.OfflineAmount = entryOfflineAmount.GetEntryValue();

            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
            {
                envPosSetup.MySQLServerType = entryMySQLServerType.GetSelectedItem();

                envPosSetup.MySQLServer = entryMySQLServer.GetEntryValue();
                if (string.IsNullOrEmpty(envPosSetup.MySQLServer))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                    entryMySQLServer.SetFocus();
                    return;
                }
                envPosSetup.MySQLDatabase = entryMySQLDatabase.GetEntryValue();
                if (string.IsNullOrEmpty(envPosSetup.MySQLDatabase))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                    entryMySQLDatabase.SetFocus();
                    return;
                }
                envPosSetup.MySQLUid = entryMySQLUid.GetEntryValue();
                if (string.IsNullOrEmpty(envPosSetup.MySQLUid))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                    entryMySQLUid.SetFocus();
                    return;
                }
                envPosSetup.MySQLPwd = entryMySQLPwd.GetEntryValue();
                if (string.IsNullOrEmpty(envPosSetup.MySQLPwd))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                    entryMySQLPwd.SetFocus();
                    return;
                }
            }

            if (string.IsNullOrEmpty(envPosSetup.PosInstallDate))
            {
                // JINIT_일/월/년 형식으로 변경, envPosSetup.PosInstallDate = string.Format("{0:MMddyyyy hh:mm}",DateTime.Now);
                envPosSetup.PosInstallDate = string.Format("{0:ddMMyyyy hh:mm}", DateTime.Now);
            }

            EnvPosSetupService.SaveEnvPosSetup(envPosSetup);
            // 2021.03.04
            UIManager.Instance().PosModel.Environment.EnvPosSetup = envPosSetup;
            // 2021.03.15
            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.PrinterPaperSize.Equals("80mm"))
            {
                // 영수증 사이즈를 80mm로 설정
                UIManager.Instance().Is58mmPrinter = false;
            }
            else
            {
                // 영수증 사이즈를 58mm로 설정
                UIManager.Instance().Is58mmPrinter = true;
            }


            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "The device is set up.", "OK");
                
            await Navigation.PopAsync();
        }
         void OnFunctionPrinterTest(object sender, EventArgs e)
        {
            envPosSetup.PrinterPort = entryPort.GetEntryValue();
            envPosSetup.PrinterBaudRate = etBaudRate.GetSelectedItem();
            envPosSetup.PrinterPaperSize = etPaperSize.GetSelectedItem();

            //Test
        }
        async void OnFunctionMySQLTest(object sender, EventArgs e)
        {
            envPosSetup.MySQLServerType = entryMySQLServerType.GetSelectedItem();

            envPosSetup.MySQLServer = entryMySQLServer.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.MySQLServer))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryMySQLServer.SetFocus();
                return;
            }
            envPosSetup.MySQLDatabase = entryMySQLDatabase.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.MySQLDatabase))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryMySQLDatabase.SetFocus();
                return;
            }
            envPosSetup.MySQLUid = entryMySQLUid.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.MySQLUid))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryMySQLUid.SetFocus();
                return;
            }
            envPosSetup.MySQLPwd = entryMySQLPwd.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.MySQLPwd))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryMySQLPwd.SetFocus();
                return;
            }

            // Test

        }
        async void OnFunctionBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

using EBM2x.Database;
using EBM2x.Database.InitSQLite;
using EBM2x.Datafile.env;
using EBM2x.Models.config;
using EBM2x.Process.start;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.UI.Phone;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Setup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EBM2xPhoneSetupPage : ContentPage
    {
        EnvPosSetup envPosSetup = null;
        bool IsResetEnvPosSetup = false;

        public EBM2xPhoneSetupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            entryOfflineDays.SetReadOnly(true);
            entryOfflineAmount.SetReadOnly(true);

            textAppVersion.InvalidateSurface(RraSdcService.APPLICATION_NAME + " / " + UIManager.AppVersion);
        }

        public EBM2xPhoneSetupPage(bool isResetEnvPosSetup)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            IsResetEnvPosSetup = isResetEnvPosSetup;

            entryOfflineDays.SetReadOnly(true);
            entryOfflineAmount.SetReadOnly(true);

            textAppVersion.InvalidateSurface(RraSdcService.APPLICATION_NAME + " / " + UIManager.AppVersion);
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
                envPosSetup = new EnvPosSetup();

                //entryStoreCode.SetEntryValue(envPosSetup.StoreCode);
                //entryPosNumber.SetEntryValue(envPosSetup.PosNumber);
                entryOfflineDays.SetEntryValue(envPosSetup.OfflineDays);
                entryOfflineAmount.SetEntryValue(envPosSetup.OfflineAmount);
            }
            else
            {
                entryTin.SetEntryValue(envPosSetup.GblTaxIdNo);
                entryBhfId.SetEntryValue(envPosSetup.GblBrcCod);
                entryGblSerialNo.SetEntryValue(envPosSetup.GblSerialNo);

                //entryStoreCode.SetEntryValue(envPosSetup.StoreCode);
                //entryPosNumber.SetEntryValue(envPosSetup.PosNumber);
                //entryUrl0001.SetEntryValue(envPosSetup.Url0001);
                //entryUrl0002.SetEntryValue(envPosSetup.Url0002);
                //entryTemplet.SetSelecteItem(envPosSetup.TempletType);
                entryLocation.SetSelecteItem(new SystemCode() { Id = envPosSetup.LocationType, Name = "" });
                entryOfflineDays.SetEntryValue(envPosSetup.OfflineDays);
                entryOfflineAmount.SetEntryValue(envPosSetup.OfflineAmount);
            }

            //if (string.IsNullOrEmpty(entryStoreCode.GetEntryValue())) entryStoreCode.SetEntryValue("20001");
            //if (string.IsNullOrEmpty(entryPosNumber.GetEntryValue())) entryPosNumber.SetEntryValue("0001");
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
            envPosSetup.GblSerialNo = entryGblSerialNo.GetEntryValue();
            if (string.IsNullOrEmpty(envPosSetup.GblSerialNo))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
                entryGblSerialNo.SetFocus();
                return;
            }
            envPosSetup.StoreCode = "20001";
            envPosSetup.PosNumber = "0001";
            //envPosSetup.StoreCode = entryStoreCode.GetEntryValue();
            //if (string.IsNullOrEmpty(envPosSetup.StoreCode))
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
            //    entryStoreCode.SetFocus();
            //    return;
            //}
            //envPosSetup.PosNumber = entryPosNumber.GetEntryValue();
            //if (string.IsNullOrEmpty(envPosSetup.PosNumber))
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please enter a value", "Ok");
            //    entryPosNumber.SetFocus();
            //    return;
            //}
            //envPosSetup.Url0001 = entryUrl0002.GetEntryValue();
            //envPosSetup.Url0002 = entryUrl0002.GetEntryValue();
            //envPosSetup.TempletType = entryTemplet.GetSelectedItem();
            envPosSetup.LocationType = entryLocation.GetSelectedItem().Id;

            envPosSetup.OfflineDays = entryOfflineDays.GetEntryValue();
            envPosSetup.OfflineAmount = entryOfflineAmount.GetEntryValue();

            if (string.IsNullOrEmpty(envPosSetup.PosInstallDate))
            {
                // JINIT_일/월/년 형식으로 변경, envPosSetup.PosInstallDate = string.Format("{0:MMddyyyy hh:mm}",DateTime.Now);
                envPosSetup.PosInstallDate = string.Format("{0:ddMMyyyy hh:mm}", DateTime.Now);
            }

            if (IsResetEnvPosSetup)
            {
                EnvPosSetupService.SaveEnvPosSetup(envPosSetup);
                string result = "";
                result = Initialize.RegiClearProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                //UIManager.Instance().PosModel.RegiTotal.RegiHeader.CloseFlag = true;
                //UIManager.Instance().PosModel.RegiTotal.RegiHeader.CloseDate = "20191201";

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Initialize", "OK");

                result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                await Navigation.PopAsync();
            }

            InitInfoReq initInfoReq = new InitInfoReq
            {
                tin = envPosSetup.GblTaxIdNo,           // TIN
                bhfId = envPosSetup.GblBrcCod,          // 지점코드
                dvcSrlNo = envPosSetup.GblSerialNo      // 최종 수신일자 
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //// JSON의 형식으로 자료를 서버에 전송한다.
                    string jsonRequest = JsonConvert.SerializeObject(initInfoReq);
                    HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    string url = RraSdcService.INTERNAL_URL + "/" + RraSdcService.URL_INIT_INFO;
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response != null)
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        InitInfoRes initInfoRes = JsonConvert.DeserializeObject<InitInfoRes>(jsonResponse);
                        if (initInfoRes.resultCd.Equals("000"))
                        {
                            try
                            {
                                EBM2xMsSQLiteClientProvider providerCopyDatabase = new EBM2xMsSQLiteClientProvider();
                                bool ret = providerCopyDatabase.CopySQLiteDatabase();
                                if (ret)
                                {
                                    //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "CopySQLiteDatabase", "OK");

                                    InitInfoVO initInfoVO = initInfoRes.data.info;
                                    EnvRraSdcService.SaveEnvRraSdc(initInfoVO);
                                    envPosSetup.Init();

                                    envPosSetup.LastSaleInvcNo = initInfoRes.data.info.lastSaleInvcNo;
                                    envPosSetup.LastPchsInvcNo = initInfoRes.data.info.lastPchsInvcNo;

                                    try
                                    {
                                        envPosSetup.ChangeNonVAT = true;
                                        // ChangeNonVAT 변경되 이력이 없는 경우에만 장비인증 값을 사용한다.
                                        if (initInfoVO.vatTyCd != null && initInfoVO.vatTyCd.Equals("2"))
                                        {
                                            envPosSetup.NonVAT = true;
                                        }
                                        else
                                        {
                                            envPosSetup.NonVAT = false;
                                        }
                                    }
                                    catch (Exception ve)
                                    {
                                        envPosSetup.NonVAT = false;
                                    }

                                    EnvPosSetupService.SaveEnvPosSetup(envPosSetup);

                                    //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", initInfoRes.resultMsg, "Ok");

                                    SQLiteDatabaseInit sQLiteDatabaseInit = new SQLiteDatabaseInit();
                                    sQLiteDatabaseInit.UpdateTable(entryTin.GetEntryValue(), entryBhfId.GetEntryValue());

                                    //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "UpdateTable", "OK");

                                    string result = "";
                                    result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                                    result = Initialize.RegiClearProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Initialize", "OK");

                                    result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                                    Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                                    await Navigation.PopAsync();
                                }
                                else
                                {
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error cp", "CopySQLiteDatabase", "OK");
                                }
                            }
                            catch (InvalidOperationException ex)
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error op", ex.ToString(), "OK");
                            }
                        }
                        else
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error ir", initInfoRes.resultMsg, "Ok");
                        }
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error cm", "Communication error, please try again.", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    string resultMsg = ex.Message;
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error ex", resultMsg, "Ok");
                }
            }
        }
    }
}

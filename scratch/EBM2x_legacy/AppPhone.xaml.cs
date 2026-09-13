using EBM2x.Database;
using EBM2x.Database.Master;
using EBM2x.Datafile.env;
using EBM2x.Process.start;
using EBM2x.UI;
using EBM2x.UI.Phone;
using EBM2x.UI.Setup;
using System;
using Xamarin.Forms;

namespace EBM2x
{
    public partial class AppPhone : Application
    {
        public UIManager __UIManager = null;

        public AppPhone(bool isWindows, bool isMobile)
        {
            __UIManager = UIManager.Instance();
            __UIManager.IsWindows = isWindows;
            __UIManager.Is58mmPrinter = true;

            // 2021.01 Mobile
            __UIManager.IsMobile = isMobile;

            UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 5;
            UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

            //UIManager.Instance().PresetModel.PresetGroupList.CountOfGroupsToDisplayOnOnePage = 0;
            //UIManager.Instance().PresetModel.PresetGroupList.CountOfItemsToDisplayOnOnePage = 0;

            InitializeComponent();

            MessagingCenter.Subscribe<Object, string>(this, "It was entered Number", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Append(arg);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "It was entered Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    if (arg.Equals("Backspace"))
                    {
                        UIManager.Instance().InputModel.DeleteLastOne();
                    }
                    else
                    {
                        MessagingCenter.Send<Object, string>(this, "Function", arg);
                    }
                });
            });

            if (!EnvRraSdcService.IsEnvRraSdc() || !EnvPosSetupService.IsEnvPosSetup())
            {
                // 환경설정파일이 존재하지 않습니다
                MainPage = new NavigationPage(new EBM2xPhoneSetupPage());
            }
            else
            {
                string result = "";
                result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                UIManager.Instance().PosModel.Environment.EnvPosSetup.Init();
                EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);

                int days = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount();
                if (days == 0)
                {
                    StockIoMaster stockIoMaster = new StockIoMaster();
                    UIManager.Instance().PosModel.Environment.EnvPosSetup.ChangePosInstallDate(stockIoMaster.GetStockIoDate());
                    EnvPosSetupService.SaveEnvPosSetup(UIManager.Instance().PosModel.Environment.EnvPosSetup);
                }

                //=========================================================================================
                // Z-REPORT Table 생성
                Ebm2xCreateTables.CreateZreportTables();
                
                //=========================================================================================

                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.PrinterPaperSize.Equals("80mm"))
                {
                    // 영수증 사이즈를 80mm로 설정
                    __UIManager.Is58mmPrinter = false;
                }
                else
                {
                    // 영수증 사이즈를 58mm로 설정
                    __UIManager.Is58mmPrinter = true;
                }

                MainPage = new NavigationPage(new PersonalShopStartPage());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

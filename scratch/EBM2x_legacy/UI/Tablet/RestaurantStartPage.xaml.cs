using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Process.start;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Setup;
using EBM2x.UI.Tablet.Menu;
using EBM2x.UI.Tablet.Open;
using EBM2x.UI.Tablet.SignOn;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantStartPage : ContentPage
    {
        public RestaurantStartPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.SetSalesTitleText("Start");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Start pos program.");
            excuteProcess();
        }

        async void excuteProcess()
        {
            bool IsPressed = true;
            string result = "";

            if (IsPressed)
            {
                result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                if (StateModel.IsIt_OP_POS_SETUP(result))
                {
                    IsPressed = false;
                }
            }
            if (IsPressed)
            {
                result = OpenCheck.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
            }

            if (StateModel.IsIt_OP_POS_SETUP(result))
            {
                await Navigation.PushAsync(new EBM2xSetupPage());
            }
            else if (StateModel.IsIt_OP_NEXT(result))
            {
                // JINIT_전일자를 마감하지 않은 상태면 확인메시지를 표시함
                if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate != System.DateTime.Now.ToString("yyyyMMdd"))
                {
                    string locationTitle3 = UILocation.Instance().GetLocationText("Warning");
                    string locationMessage3 = UILocation.Instance().GetLocationText("The previous date is not close.Should we go to the menu?");
                    var retClose = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                    if (retClose)
                    {
                        Navigation.InsertPageBefore(new EndOfDayMainPage(), this);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        Navigation.InsertPageBefore(new RestaurantSignOnPage(), this);
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    Navigation.InsertPageBefore(new RestaurantSignOnPage(), this);
                    await Navigation.PopAsync();
                }
            }
            else if (StateModel.IsIt_OP_FAR(result))
            {
                Navigation.InsertPageBefore(new RestaurantOpenPage(), this);
                await Navigation.PopAsync();
            }
            else if (StateModel.IsIt_OP_EXIT(result))
            {
                var closer = DependencyService.Get<ICloseApplication>();
                closer?.closeApplication();
            }
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Start":
                    Navigation.InsertPageBefore(new RestaurantOpenPage(), this);
                    await Navigation.PopAsync();
                    break;
                case "Exit":
                    var closer = DependencyService.Get<ICloseApplication>();
                    closer?.closeApplication();
                    break;
                default:
                    UIManager.Instance().InputModel.Clear();
                    break;
            }
        }
    }
}

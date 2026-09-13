using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Models.signon;
using EBM2x.Models.State;
using EBM2x.Models.tran;
using EBM2x.Process.signon;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Tablet.Menu;
using EBM2x.UI.Tablet.Order;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.SignOn
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantSignOnPage : ContentPage
    {
        bool pageIsActive;
        SignOnNode signOnNode = null;
        StateNodeList stateNodeList = null;

        public RestaurantSignOnPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();

            signOnNode = new SignOnNode();
            signOnNode.CashierID = "";
            signOnNode.CashierName = "";

            stateNodeList = new StateNodeList();

            stateNodeList.Add(new StateNode
            {
                Information = "Please enter User ID",
                ContentView = userIdEntry
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter Password",
                ContentView = passwordEntry
            });
            stateNodeList.SetCurrent(1);
            UIManager.Instance().InputModel.IsPassword = false;

            // DATA 초기화
            UIManager.Instance().PosModel.TranModel.SignOnNode = signOnNode;
            UIManager.Instance().PosModel.TranModel.TranInformation.LogFlag = TranDefine.LOG_SIGN_ON;

            UIManager.Instance().InputModel.Clear(); // JINIT_InputData 초기화
            pageIsActive = true;
            AnimationLoop();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.SetSalesTitleText("Sign On");
            UIManager.Instance().PosModel.InvalidateSurface();

            InvalidateSurface(signOnNode);
            pageIsActive = true;
        }

        protected override void OnDisappearing()
        {
            pageIsActive = false;
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(300.0));
                if (pageIsActive)
                {
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();
                }
            }
        }
        public void InvalidateSurface(SignOnNode node)
        {
            userIdEntry.InvalidateSurface(node.CashierID);
            userNameEntry.InvalidateSurface(node.CashierName);
            if (!string.IsNullOrEmpty(node.Password))
                passwordEntry.InvalidateSurface("********************".Substring(0, node.Password.Length));
            else
                passwordEntry.InvalidateSurface("");

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            string result = "";
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "SignOn":
                case "Enter":
                    // Navigation.InsertPageBefore(new DiningTableArrangementPage(), this);
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                        result = InputOperatorCodeProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        if (StateModel.IsIt_OP_NEXT(result))
                        {
                            FocuePassword();
                            InvalidateSurface(signOnNode);
                            UIManager.Instance().InputModel.Clear();
                        }
                        else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                        {
                            string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                            string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                            var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                            if (ret1)
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else if (StateModel.IsIt_OP_ALERT(result))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                            UIManager.Instance().InputModel.Clear();
                        }
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        result = InputPasswordProcess.excuteProcess("user", UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        if (StateModel.IsIt_OP_NEXT(result))
                        {
                            result = SignOnProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                UIManager.Instance().InputModel.IsPassword = false;
                                UIManager.Instance().InputModel.Clear();

                                Navigation.InsertPageBefore(new DiningTableArrangementPage(), this);
                                await Navigation.PopAsync();
                            }
                        }
                        else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                        {
                            string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                            string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                            var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                            if (ret1)
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else if (StateModel.IsIt_OP_ALERT(result))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                            UIManager.Instance().InputModel.Clear();
                        }
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "Menu":
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                        result = InputOperatorCodeProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        if (StateModel.IsIt_OP_NEXT(result))
                        {
                            FocuePassword();
                            InvalidateSurface(signOnNode);
                            UIManager.Instance().InputModel.Clear();
                        }
                        else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                        {
                            string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                            string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                            var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                            if (ret1)
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else if (StateModel.IsIt_OP_ALERT(result))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                            UIManager.Instance().InputModel.Clear();
                        }
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        result = InputPasswordProcess.excuteProcess("admin", UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        if (StateModel.IsIt_OP_NEXT(result))
                        {
                            UIManager.Instance().InputModel.IsPassword = false;
                            UIManager.Instance().InputModel.Clear();

                            Navigation.InsertPageBefore(new RestaurantMenuPage(), this);
                            await Navigation.PopAsync();
                        }
                        else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                        {
                            string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                            string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                            var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                            if (ret1)
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else if (StateModel.IsIt_OP_ALERT(result))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                            UIManager.Instance().InputModel.Clear();
                        }
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "AdminMenu":
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                        if (UIManager.Instance().InputModel.EnteredText == "99999")
                        {
                            result = InputAdminOperatorCodeProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                            else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                            {
                                string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                                string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                                var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                                if (ret1)
                                {
                                    FocuePassword();
                                    InvalidateSurface(signOnNode);
                                    UIManager.Instance().InputModel.Clear();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT(result))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else if (UIManager.Instance().InputModel.EnteredText == UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo)
                        {
                            result = InputAdminOperatorCodeProcess.excuteProcessII(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                            else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                            {
                                string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                                string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                                var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                                if (ret1)
                                {
                                    FocuePassword();
                                    InvalidateSurface(signOnNode);
                                    UIManager.Instance().InputModel.Clear();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT(result))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else
                        {
                            result = InputOperatorCodeProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                FocuePassword();
                                InvalidateSurface(signOnNode);
                                UIManager.Instance().InputModel.Clear();
                            }
                            else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                            {
                                string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                                string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                                var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                                if (ret1)
                                {
                                    FocuePassword();
                                    InvalidateSurface(signOnNode);
                                    UIManager.Instance().InputModel.Clear();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT(result))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        if (signOnNode.CashierID == "99999")
                        {
                            result = InputAdminPasswordProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                UIManager.Instance().InputModel.IsPassword = false;
                                UIManager.Instance().InputModel.Clear();

                                Navigation.InsertPageBefore(new RestaurantAdminMenuPage(), this);
                                await Navigation.PopAsync();
                            }
                            else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                            {
                                string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                                string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                                var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                                if (ret1)
                                {
                                    FocuePassword();
                                    InvalidateSurface(signOnNode);
                                    UIManager.Instance().InputModel.Clear();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT(result))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else if (signOnNode.CashierID == UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo)
                        {
                            result = InputAdminPasswordProcess.excuteProcessII(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                UIManager.Instance().InputModel.IsPassword = false;
                                UIManager.Instance().InputModel.Clear();

                                Navigation.InsertPageBefore(new RestaurantAdminMenuPage(), this);
                                await Navigation.PopAsync();
                            }
                            else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                            {
                                string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                                string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                                var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                                if (ret1)
                                {
                                    FocuePassword();
                                    InvalidateSurface(signOnNode);
                                    UIManager.Instance().InputModel.Clear();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT(result))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                        else
                        {
                            result = InputPasswordProcess.excuteProcess("admin", UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            if (StateModel.IsIt_OP_NEXT(result))
                            {
                                result = SignOnProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                                if (StateModel.IsIt_OP_NEXT(result))
                                {
                                    UIManager.Instance().InputModel.IsPassword = false;
                                    UIManager.Instance().InputModel.Clear();

                                    Navigation.InsertPageBefore(new RestaurantAdminMenuPage(), this);
                                    await Navigation.PopAsync();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT_YES_NO(result))
                            {
                                string locationTitle = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertTitle);
                                string locationMessage = UILocation.Instance().GetLocationText(UIManager.Instance().InformationModel.AlertMessage);
                                var ret1 = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                                if (ret1)
                                {
                                    FocuePassword();
                                    InvalidateSurface(signOnNode);
                                    UIManager.Instance().InputModel.Clear();
                                }
                            }
                            else if (StateModel.IsIt_OP_ALERT(result))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, UIManager.Instance().InformationModel.AlertTitle, UIManager.Instance().InformationModel.AlertMessage, "Yes");
                                UIManager.Instance().InputModel.Clear();
                            }
                        }
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "Exit":
                    Navigation.InsertPageBefore(new SalesMenuPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "Backspace":
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                        signOnNode.CashierID = "";
                        signOnNode.CashierName = "";
                        signOnNode.Password = "";
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        signOnNode.CashierID = "";
                        signOnNode.CashierName = "";
                        signOnNode.Password = "";
                        FocusUserID();
                    }
                    InvalidateSurface(signOnNode);
                    break;

                case "FocueUserId":
                    signOnNode.CashierID = "";
                    signOnNode.CashierName = "";
                    signOnNode.Password = "";
                    FocusUserID();
                    InvalidateSurface(signOnNode);
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                case "FocuePassword":
                    signOnNode.Password = "";
                    FocuePassword();
                    InvalidateSurface(signOnNode);
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                default:
                    break;
            }
        }

        void FocusUserID()
        {
            userIdBox.InvalidateSurface(true);
            passwordBox.InvalidateSurface(false);

            stateNodeList.SetCurrent(1);
            UIManager.Instance().InputModel.IsPassword = false;
        }

        void FocuePassword()
        {
            userIdBox.InvalidateSurface(false);
            passwordBox.InvalidateSurface(true);

            stateNodeList.SetCurrent(2);
            UIManager.Instance().InputModel.IsPassword = true;
        }
    }
}

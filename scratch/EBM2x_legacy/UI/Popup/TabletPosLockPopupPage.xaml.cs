using EBM2x.Dependency;
using EBM2x.Models.signon;
using EBM2x.Models.State;
using EBM2x.UI.Draw;
using EBM2x.UI.Tablet.Menu;
using EBM2x.UI.Tablet.Sales;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletPosLockPopupPage : ContentPage
    {
        SignOnNode signOnNode = null;
        StateNodeList stateNodeList = null;

        public TabletPosLockPopupPage()
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

            UIManager.Instance().InputModel.Clear(); // JINIT InputData 초기화
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.SetSalesTitleText("POS Lock");
            UIManager.Instance().PosModel.InvalidateSurface();

            InvalidateSurface(signOnNode);
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
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

        public event EventHandler OnSignOffResult;

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                /* JINIT_[ENTER]처리와 동일하게 처리하게 변경
                case "Unlock":
                    if(UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserID.Equals(signOnNode.CashierID))
                    {
                        signOnNode.Password = ((ExtEventArgs)e).EnteredText;
                        UIManager.Instance().InputModel.IsPassword = false;

                        await Navigation.PopAsync();
                    }
                    else
                    {
                        UIManager.Instance().InformationModel.SetWarningMessage("Unregistered USER. Please check the USER ID!");
                    }
                    //UIManager.Instance().InputModel.Clear();
                    break;
                    */

                case "Backspace":
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                        signOnNode.CashierID = "";
                        signOnNode.CashierName = "";
                        signOnNode.Password = "";
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        //FocusUserID();
                        //UIManager.Instance().InputModel.Append(signOnNode.CashierID);
                        signOnNode.CashierID = "";
                        signOnNode.CashierName = "";
                        signOnNode.Password = "";
                        FocusUserID();
                    }
                    InvalidateSurface(signOnNode);
                    break;

                case "Unlock":
                case "Enter":
                    if (stateNodeList.CurrentLineNumber == 1) // 사용자ID 입력
                    {
                        // JINIT_관리자ID인 경우 처리
                        //string userId = ((ExtEventArgs)e).EnteredText;
                        string userId = UIManager.Instance().InputModel.EnteredText;
                        if (userId == "99999")
                        {
                            signOnNode.CashierID = userId;
                            signOnNode.CashierName = "administrator";
                            InvalidateSurface(signOnNode);
                            FocuePassword();
                        }
                        else
                        {
                            if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserID.Equals(userId))
                            {
                                signOnNode.CashierID = userId;
                                signOnNode.CashierName = UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserName;
                                InvalidateSurface(signOnNode);
                                FocuePassword();
                            }
                            else
                            {
                                // JINIT_사용자ID가 틀릴 경우 메시지 표시
                                UIManager.Instance().InformationModel.SetWarningMessage("User does not match !");
                            }
                        }
                    }
                    else if (stateNodeList.CurrentLineNumber == 2) // 비밀번호 입력
                    {
                        // JINIT_ string pass = ((ExtEventArgs)e).EnteredText;
                        string pass = UIManager.Instance().InputModel.EnteredText;
                        // JINIT_비밀번호 체크 추가
                        /*
                        signOnNode.Password = ((ExtEventArgs)e).EnteredText;
                        UIManager.Instance().InputModel.IsPassword = false;
                        UIManager.Instance().InputModel.Clear();
                        */

                        if (signOnNode.CashierID == "99999")
                        {
                            if (Utils.Common.getAdminPass() == pass)
                            {
                                UIManager.Instance().InputModel.IsPassword = false;
                                InvalidateSurface(signOnNode);
                                await Navigation.PopAsync();
                            }
                            else
                            {
                                UIManager.Instance().InformationModel.SetWarningMessage("Administrator password does not match !");
                            }
                        }
                        else
                        {
                            Models.config.OperatorRecord record = Services.OperatorService.Load(signOnNode.CashierID);
                            if (record.Password == pass)
                            {
                                signOnNode.Password = pass;
                                UIManager.Instance().InputModel.IsPassword = false;
                                //UIManager.Instance().InputModel.Clear();

                                InvalidateSurface(signOnNode);
                                await Navigation.PopAsync();
                            }
                            else
                            {
                                UIManager.Instance().InformationModel.SetWarningMessage("Password does not match !");
                            }
                        }
                    }
                    break;

                case "FocueUserId":
                    signOnNode.CashierID = "";
                    signOnNode.CashierName = "";
                    signOnNode.Password = "";
                    FocusUserID();
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                case "FocuePassword":
                    //signOnNode.CashierID = "";
                    //signOnNode.CashierName = "";
                    signOnNode.Password = "";
                    FocuePassword();
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                default:
                    break;
            }
            UIManager.Instance().InputModel.Clear();
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

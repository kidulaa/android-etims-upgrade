using EBM2x.Models.Event;
using EBM2x.Models.State;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletMobileWalletPage : ContentPage
    {
        public TenderNode TenderNode { get; set; }
        StateNodeList stateNodeList = null;

        public TabletMobileWalletPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();

            TenderNode = new TenderNode();
            TenderNode.TenderFlag = "Mobile Wallet";
            TenderNode.TenderName = "Mobile Wallet";
            TenderNode.Sign = UIManager.Instance().PosModel.TranModel.TranNode.Sign;
            TenderNode.ReceiveAmount = 0;

            stateNodeList = new StateNodeList();

            stateNodeList.Add(new StateNode
            {
                Information = "Please enter the amount received",
                ContentView = payNowEntry
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter Phone number",
                ContentView = phoneNumberBox
            });
            stateNodeList.SetCurrent(1);

            if (!string.IsNullOrEmpty(UIManager.Instance().InputModel.EnteredText))
            {
                TenderNode.ReceiveAmount = double.Parse(UIManager.Instance().InputModel.EnteredText);
                UIManager.Instance().InputModel.Clear();
                if (TenderNode.ReceiveAmount > UIManager.Instance().PosModel.TranModel.TranNode.AmountToReceive())
                {
                    TenderNode.ReceiveAmount = 0;
                    UIManager.Instance().InformationModel.SetWarningMessage("The amount is too large.");
                }
                else
                {
                    TenderNode.SubtotalAmount = TenderNode.ReceiveAmount;
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    FocuePhoneNumber();
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.SetSalesTitleText("Mobile payment");
            UIManager.Instance().PosModel.InvalidateSurface();

            InvalidateSurface();
            InvalidateSurface(TenderNode);

            UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }

        public void InvalidateSurface()
        {
            salesReceive.InvalidateSurface(UIManager.Instance().PosModel.TranModel.TranNode);
            salesChange.InvalidateSurface(UIManager.Instance().PosModel.TranModel.TranNode);
            salesTotal.InvalidateSurface(UIManager.Instance().PosModel.TranModel.TranNode);
            paymentPanel.SalesTenderListInvalidateSurface(UIManager.Instance().PosModel.TranModel.TranNode.TenderList);

            amountToReceive.InvalidateSurface(UIManager.Instance().PosModel.TranModel.TranNode.AmountToReceive(), true);
        }

        public void InvalidateSurface(TenderNode node)
        {
            payNowEntry.InvalidateSurface(TenderNode.ReceiveAmount, true);
            phoneNumberEntry.InvalidateSurface(TenderNode.PhoneNumber);
        }

        public event EventHandler OnConfirmed;
        public event EventHandler OnCanceled;

        void OnCancelButtonClicked(object sender, EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }
        async void OnApprovedButtonClicked(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(TenderNode.PhoneNumber) || TenderNode.PhoneNumber.Length < 8)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the PhoneNumber.", "Ok");
                return;
            }

            fixedGrid02.IsVisible = false;
        }
        void OnApprovedCancelButtonClicked(object sender, EventArgs e)
        {
            fixedGrid02.IsVisible = true;
        }
        void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            if (TenderNode.SubtotalAmount == 0)
            {
                UIManager.Instance().InformationModel.SetWarningMessage("Please enter an amount.");
            }
            else
            {
                if (OnConfirmed != null) OnConfirmed?.Invoke(this, new TenderEventArgs(TenderNode.TenderName, TenderNode));
            }
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
                case "Backspace":
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        TenderNode.ReceiveAmount = 0; // JINIT_입력금액 초기화
                        FocusPayNow();
                    }
                    InvalidateSurface(TenderNode);
                    break;

                case "Enter":
                    if (stateNodeList.CurrentLineNumber == 1)
                    {
                        if (!string.IsNullOrEmpty(UIManager.Instance().InputModel.EnteredText))
                        {
                            double receiveAmount = double.Parse(UIManager.Instance().InputModel.EnteredText);

                            // JINIT_반품인 경우 거스름돈이 발생되지 않도록 처리
                            if (UIManager.Instance().PosModel.TranModel.TranNode.Sign == -1)
                            {
                                // 반품인경우 받은돈이 합계금액보다 크면 오류 
                                if (receiveAmount + UIManager.Instance().PosModel.TranModel.TranNode.Receive > UIManager.Instance().PosModel.TranModel.TranNode.Subtotal)
                                {
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The amount is too large.", "OK");
                                    break;
                                }
                            }

                            TenderNode.ReceiveAmount = receiveAmount;
                            if (TenderNode.ReceiveAmount > UIManager.Instance().PosModel.TranModel.TranNode.AmountToReceive())
                            {
                                TenderNode.ReceiveAmount = 0;
                                UIManager.Instance().InformationModel.SetWarningMessage("The amount is too large.");
                                break;
                            }
                            TenderNode.SubtotalAmount = TenderNode.ReceiveAmount;
                        }
                        else
                        {
                            TenderNode.ReceiveAmount = UIManager.Instance().PosModel.TranModel.TranNode.AmountToReceive();
                            TenderNode.SubtotalAmount = TenderNode.ReceiveAmount;
                        }
                        FocuePhoneNumber();
                    }
                    else if (stateNodeList.CurrentLineNumber == 2)
                    {
                        TenderNode.PhoneNumber = UIManager.Instance().InputModel.EnteredText;
                    }
                    InvalidateSurface(TenderNode);
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                case "FocuePayNow":
                    FocusPayNow();
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                case "FocuePhoneNumber":
                    FocuePhoneNumber();
                    UIManager.Instance().InformationModel.SetWarningMessage("");
                    UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
                    break;

                default:
                    break;
            }
        }

        void FocusPayNow()
        {
            payNowBox.InvalidateSurface(true);
            phoneNumberBox.InvalidateSurface(false);

            stateNodeList.SetCurrent(1);
        }

        void FocuePhoneNumber()
        {
            payNowBox.InvalidateSurface(false);
            phoneNumberBox.InvalidateSurface(true);

            stateNodeList.SetCurrent(2);
        }
    }
}

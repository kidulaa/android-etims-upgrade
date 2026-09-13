using EBM2x.Dependency;
using EBM2x.Models.ReadyMoney;
using EBM2x.Process;
using EBM2x.Process.eot;
using EBM2x.Process.money;
using EBM2x.Services;
using EBM2x.UI.Draw;
using EBM2x.UI.Popup;
using EBM2x.UI.Tablet.Sales;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntermediateDepositPage : ContentPage
    {
        ReadyMoneyList readyMoneyList = null;

        public IntermediateDepositPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();

            /* JINIT_이전에 등록된 자료를 LOAD하지 않고 신규로 등록하게끔 처리함.
            readyMoneyList = ReadyMoneyProcess.LoadIntermediateDeposit();
            if (readyMoneyList == null)
            {
                readyMoneyList = new ReadyMoneyList();
                readyMoneyList.Add(new ReadyMoneyNode { Price = 10000 });
                readyMoneyList.Add(new ReadyMoneyNode { Price = 5000 });
                readyMoneyList.Add(new ReadyMoneyNode { Price = 2000 });
                readyMoneyList.Add(new ReadyMoneyNode { Price = 1000 });
                readyMoneyList.Add(new ReadyMoneyNode { Price = 500 });
                readyMoneyList.Add(new ReadyMoneyNode { Price = 100 });
                readyMoneyList.Add(new ReadyMoneyNode { Price = 50 });
                //            readyMoneyList.Add(new ReadyMoneyNode { Price = 10 });
                //            readyMoneyList.Add(new ReadyMoneyNode { Price = 1 });

                readyMoneyList.SetCurrent(1);

            }
            */
            
            readyMoneyList = new ReadyMoneyList();
            readyMoneyList.Add(new ReadyMoneyNode { Price = 5000 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 2000 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 1000 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 500 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 100 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 50 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 20 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 10 });
            readyMoneyList.Add(new ReadyMoneyNode { Price = 5 });

            readyMoneyList.SetCurrent(1);
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

            UIManager.Instance().PosModel.SetSalesTitleText("Intermediate|Deposit");
            UIManager.Instance().PosModel.InvalidateSurface();

            readyMoneyPanel.InvalidateSurface(readyMoneyList);
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

        void OnSelectMoneyType(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가

            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            readyMoneyList.SetCurrent(index);
            readyMoneyPanel.InvalidateSurface(readyMoneyList);
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Back":
                    UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                    await Navigation.PopAsync();
                    break;

                case "Confirm":
                    // JINIT_금액이 0원이면 오류처리
                    if (readyMoneyList.GetTotal() == 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "No amount entered.", "OK");
                        UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                        break;
                    }

                    // 저장 및 영수증 출력
                    ReadyMoneyProcess.SaveIntermediateDeposit(readyMoneyList);
                    IntermediateDepositEndOfTransaction.excuteProcess(readyMoneyList, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                    await Navigation.PopAsync();
                    break;

                case "Up":
                    readyMoneyList.Up();
                    readyMoneyPanel.InvalidateSurface(readyMoneyList);
                    UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                    break;

                case "Down":
                    readyMoneyList.Down();
                    readyMoneyPanel.InvalidateSurface(readyMoneyList);
                    UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                    break;

                case "Enter":
                    if (!string.IsNullOrEmpty(UIManager.Instance().InputModel.EnteredText))
                    {
                        int count = int.Parse(UIManager.Instance().InputModel.EnteredText);
                        readyMoneyList.Get(readyMoneyList.CurrentLineNumber - 1).SetQuantity(count);
                        readyMoneyList.Down();
                        readyMoneyPanel.InvalidateSurface(readyMoneyList);
                        UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                    }
                    else
                    {
                        readyMoneyList.Down();
                        readyMoneyPanel.InvalidateSurface(readyMoneyList);
                        UIManager.Instance().InputModel.Clear(); // JINIT_Input Clear 추가
                    }
                    break;

                default:
                    break;
            }
        }
    }
}

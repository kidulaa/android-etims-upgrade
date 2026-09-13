using EBM2x.Models.ReadyMoney;
using EBM2x.Process.eot;
using EBM2x.Process.money;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopEndOfDayPage : ContentPage
    {
        ReadyMoneyList readyMoneyList = null;

        public PersonalShopEndOfDayPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            readyMoneyList = ReadyMoneyProcess.LoadEndOfDay();
            if (readyMoneyList == null)
            {
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
                    // 저장 및 영수증 출력
                    ReadyMoneyProcess.SaveEndOfDay(readyMoneyList);
                    CloaseEndOfTransaction.excuteProcess(readyMoneyList, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    Navigation.InsertPageBefore(new PersonalShopEndOfDayMainPage(), this);
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
                        readyMoneyList.Get(readyMoneyList.CurrentLineNumber-1).SetQuantity(count);
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

using EBM2x.Models;
using EBM2x.Models.DiningTable;
using EBM2x.Process.signoff;
using EBM2x.Process.tran;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using EBM2x.UI.Tablet.Sales;
using EBM2x.UI.Tablet.SignOn;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Order
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiningTableArrangementPage : ContentPage
    {
        bool pageIsActive;

        public DiningTableArrangementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Restaurant/Bar|Arrangement");
            UIManager.Instance().PosModel.InvalidateSurface();

            diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, false, false);

            pageIsActive = true;
            AnimationLoop();
        }

        protected override void OnDisappearing()
        {
            pageIsActive = false;

            Unsubscribe();

            base.OnDisappearing();
        }

        async void AnimationLoop()
        {
            while (true)
            {
                if (pageIsActive)
                {
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, false, false);
                }
                await Task.Delay(TimeSpan.FromSeconds(60));
                if (pageIsActive)
                {
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();
                }
            }
        }

        void Subscribe()
        {
            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            MessagingCenter.Subscribe<Object, DiningRoomList>(this, "Dining Room Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(arg, false, false);
                });
            });

            MessagingCenter.Subscribe<Object, DiningTableList>(this, "Dining Table Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningTableManagementPanel.DiningTableListInvalidateSurface(arg, false, false);
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, DiningRoomList>(this, "Dining Room Node");
            MessagingCenter.Unsubscribe<Object, DiningTableList>(this, "Dining Table Node");
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnMoveButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            await Navigation.PushAsync(new MoveSeatPage());
        }
        async void OnMergeBillButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            await Navigation.PushAsync(new MergeBillSeatPage());
        }
        async void OnDivideBillButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            await Navigation.PushAsync(new DivideBillSeatPage());
        }

        async void OnCombineButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            await Navigation.PushAsync(new CombineSeatPage());
        }

        async void OnCancelCombineButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            await Navigation.PushAsync(new CancelCombineSeatPage());
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
           
            if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate != System.DateTime.Now.ToString("yyyyMMdd"))
            {
                string locationTitle2 = UILocation.Instance().GetLocationText("Warning");
                string locationMessage2 = UILocation.Instance().GetLocationText("The previous date is not close.Should we go to the menu?");
                var retClose = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                if (retClose)
                {
                    SignOffProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    Navigation.InsertPageBefore(new SalesMenuPage(), this);
                    await Navigation.PopAsync();
                }
            }

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "SignOff":
                    SignOffProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    Navigation.InsertPageBefore(new RestaurantSignOnPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "DiningTable":
                    if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate != System.DateTime.Now.ToString("yyyyMMdd"))
                    {
                        //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning", "The previous date is not close.", "OK");
                        //SignOffProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        //Navigation.InsertPageBefore(new RestaurantSignOnPage(), this);
                        //await Navigation.PopAsync();
                    }

                    DiningRoomList diningRoomList = UIManager.Instance().PosModel.DiningTableModel.DiningRoomList;
                    DiningTableList diningTableList = diningRoomList.List[diningRoomList.CurrentLineNumber - 1].DiningTableList;
                    DiningTableNode diningTableNode = diningTableList.List[diningTableList.CurrentLineNumber -1];
                    await Navigation.PushAsync(new DiningTableOrderPage(diningTableNode));
                    break;
                case "SearchReceipt":
                    OnSearchReceiptTapped();
                    //await Navigation.PushAsync(new TabletQueryReceiptPopupPage());
                    break;

                default:
                    break;
            }
        }

        async void OnSearchReceiptTapped()
        {
            var popupPage = new TabletQueryReceiptPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    TranModel tranModel = (TranModel)((ExtEventArgs)ex).EnteredObject;
                    ReloadTranProcess.excuteProcess(tranModel, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    
                    Navigation.PopAsync();
                    Navigation.PushAsync(new RestaurantRefundSalesPage());
                });
            };

            popupPage.OnCanceled += (sender, e) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }
    }
}

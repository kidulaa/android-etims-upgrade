using EBM2x.Models;
using EBM2x.Models.Event;
using EBM2x.Models.ListView;
using EBM2x.Models.tran;
using EBM2x.Process.cash;
using EBM2x.Process.credit;
using EBM2x.Process.customer;
using EBM2x.Process.debit;
using EBM2x.Process.eot;
using EBM2x.Process.mobilewallet;
using EBM2x.Process.tran;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopRefundPaymentPage : ContentPage
    {

        public PersonalShopRefundPaymentPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            functionPanel.IsVisible = true;
            extFunctionPanel.IsVisible = false;
            finishedFunctionPanel.IsVisible = false;

            // DATA 초기화
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.Clear();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Get a Payment");

            if (UIManager.Instance().PosModel.TranModel.TranNode.AmountToReceive() == 0)
            {
                functionPanel.IsVisible = false;
                extFunctionPanel.IsVisible = false;
                finishedFunctionPanel.IsVisible = true;
            }
        }

        protected override void OnDisappearing()
        {
            Unsubscribe();
            base.OnDisappearing();
        }

        void Subscribe()
        {
            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            MessagingCenter.Subscribe<Object, TranNode>(this, "Tran Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesReceive.InvalidateSurface(arg);
                    salesChange.InvalidateSurface(arg);
                    salesTotal.InvalidateSurface(arg);
                    membersPanel.InvalidateSurface(arg);
                    paymentPanel.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, TenderList>(this, "Tender Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    paymentPanel.SalesTenderListInvalidateSurface(arg);
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, TranNode>(this, "Tran Node");
            MessagingCenter.Unsubscribe<Object, TenderList>(this, "Tender Node");
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
                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Cash":
                    //UIManager.Instance().InputModel.Clear(); // JINIT_201911, 결제금액 + [현금]처리 막음
                    if (UIManager.Instance().InputModel.EnteredText.Length > 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Do not enter the amount.", "Ok");// JINIT_201911, 결제금액 + [현금]처리 막음
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    OnCashTapped(UIManager.Instance().PosModel.TranModel.TranNode);
                    break;

                case "MobileWallet":
                    //UIManager.Instance().InputModel.Clear(); // JINIT_201911, 결제금액 + [현금]처리 막음
                    if (UIManager.Instance().InputModel.EnteredText.Length > 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Do not enter the amount.", "Ok");// JINIT_201911, 결제금액 + [현금]처리 막음
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    OnMobileWalletTapped(UIManager.Instance().PosModel.TranModel.TranNode);
                    break;

                case "Credit":
                    //UIManager.Instance().InputModel.Clear(); // JINIT_201911, 결제금액 + [현금]처리 막음
                    if (UIManager.Instance().InputModel.EnteredText.Length > 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Do not enter the amount.", "Ok");// JINIT_201911, 결제금액 + [현금]처리 막음
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    OnCreditTapped(UIManager.Instance().PosModel.TranModel.TranNode);
                    break;

                case "Debit":
                    //UIManager.Instance().InputModel.Clear(); // JINIT_201911, 결제금액 + [현금]처리 막음
                    if (UIManager.Instance().InputModel.EnteredText.Length > 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Do not enter the amount.", "Ok");// JINIT_201911, 결제금액 + [현금]처리 막음
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    OnDebitTapped(UIManager.Instance().PosModel.TranModel.TranNode);
                    break;

                case "TIN":
                    OnSearchCustomerTapped();
                    break;

                case "CalcelPayment":
                    string locationTitle2 = UILocation.Instance().GetLocationText("Cancel?");
                    string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure to Cancel");
                    var cancel = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (cancel)
                    {
                        if (UIManager.Instance().PosModel.TranModel.TranNode.TenderList.Count() > 0)
                        {
                            CancelPaymentProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                            UIManager.Instance().InputModel.Clear();
                        }
                        else
                        {
                            UIManager.Instance().InputModel.Clear();
                            await Navigation.PopAsync();
                        }
                    }
                    break;

                case "FinishedEJournal":
                    string locationTitle = UILocation.Instance().GetLocationText("EJournal");
                    string locationMessage = UILocation.Instance().GetLocationText("Do not issue a receipt.");
                    var ret = await DisplayAlert(locationTitle, locationMessage, "Yes", "No");
                    if (!ret) break;

                    UIManager.Instance().InputModel.Clear();
                    // 거래 마감 (영수증 출력하지 않음.)
                    UIManager.Instance().PosModel.Journal.PrintFlag = false; // JINIT_EJournal인 경우 출력하지 않도록 설정.
                    TransactionEndProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    // TranNode 초기화
                    PosModelInitialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    // 반품 모드를 해제한다.
                    UIManager.Instance().PosModel.TranModel.TranNode.TranFlag = TranDefine.TRAN_NORMAL;
                    await Navigation.PopAsync();
                    break;

                case "FinishedSales":
                    UIManager.Instance().InputModel.Clear();
                    // 거래 마감
                    TransactionEndProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    // TranNode 초기화
                    PosModelInitialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    // 반품 모드를 해제한다.
                    UIManager.Instance().PosModel.TranModel.TranNode.TranFlag = TranDefine.TRAN_NORMAL;
                    await Navigation.PopAsync();
                    break;

                case "RegisterCustomer":
                    await Navigation.PushAsync(new PhoneCustomerManagementPopupPage());
                    break;

                default:
                    break;
            }
        }

        async void OnSearchCustomerTapped()
        {
            var popupPage = new PhoneSearchCustomerPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    SearchCustomerNode searchCustomerNode = (SearchCustomerNode)((ExtEventArgs)ex).EnteredObject;

                    AddSearchCustomerProcess.excuteProcess(searchCustomerNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    Navigation.PopAsync();
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

        async void OnCashTapped(TranNode tranNode)
        {
            var popupPage = new PersonalShopCashPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnConfirmed += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string result = CashTenderProcess.excuteProcess(((TenderEventArgs)ex).TenderNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //if (StateModel.IsIt_OP_TRANSACTION_END(result))
                    //{
                    //    TransactionEndProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //}
                    UIManager.Instance().InputModel.Clear();

                    Navigation.PopAsync();
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

        async void OnCreditTapped(TranNode tranNode)
        {
            var popupPage = new PersonalShopCreditPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnConfirmed += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string result = CreditTenderProcess.excuteProcess(((TenderEventArgs)ex).TenderNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //if (StateModel.IsIt_OP_TRANSACTION_END(result))
                    //{
                    //    TransactionEndProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //}
                    UIManager.Instance().InputModel.Clear();

                    Navigation.PopAsync();
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
        async void OnDebitTapped(TranNode tranNode)
        {
            var popupPage = new PersonalShopDebitPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnConfirmed += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string result = DebitTenderProcess.excuteProcess(((TenderEventArgs)ex).TenderNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //if (StateModel.IsIt_OP_TRANSACTION_END(result))
                    //{
                    //    TransactionEndProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //}
                    UIManager.Instance().InputModel.Clear();

                    Navigation.PopAsync();
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
        async void OnMobileWalletTapped(TranNode tranNode)
        {
            var popupPage = new PersonalShopMobileWalletPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnConfirmed += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string result = MobileWalletTenderProcess.excuteProcess(((TenderEventArgs)ex).TenderNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //if (StateModel.IsIt_OP_TRANSACTION_END(result))
                    //{
                    //    TransactionEndProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    //}
                    UIManager.Instance().InputModel.Clear();

                    Navigation.PopAsync();
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

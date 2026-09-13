using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Models.Event;
using EBM2x.Models.journal;
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
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopReprintPage : ContentPage
    {
        TranModel SelectedTranModel = null;
        public ObservableCollection<SearchReceiptDetailListViewModel> SearchReceiptDetailList { get; set; }

        public PersonalShopReprintPage(TranModel _SelectedTranModel)
        {
            this.SelectedTranModel = _SelectedTranModel;

            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchReceiptDetailList = new ObservableCollection<SearchReceiptDetailListViewModel>();
            foreach (JournalString node in SelectedTranModel.Journal.JournalStringList)
            {
                SearchReceiptDetailList.Add(new SearchReceiptDetailListViewModel { Node = node });
            }
            detailListView.ItemsSource = SearchReceiptDetailList;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Enter the phone number");
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
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
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
                case "Back":
                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;

                case "FinishedSendSMS":
                    if (UIManager.Instance().InputModel.EnteredText.Length < 10)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Enter the phone number to receive.", "Ok"); // 전화번호를 입력하세요.
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }

                    string phoneNumber = UIManager.Instance().InputModel.EnteredText;
                    string viewMessage = "Do you send SMS to this number? [" + phoneNumber + "]";
                    string locationTitle = UILocation.Instance().GetLocationText("Send SMS");
                    var ret3 = await DisplayAlert(locationTitle, viewMessage, "Yes", "No");
                    if (!ret3)
                    {
                        break;
                    }

                    try
                    {
                        PrintingService printingServiceSMS = new PrintingService();

                        string smsMessage1 = "PIN:" + UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo + "\n";
                        smsMessage1 += "Amount:" + SelectedTranModel.TranNode.Subtotal.ToString("#,##0.##") + "\n";
                        smsMessage1 += "Receipt num:" + UIManager.Instance().PosModel.Environment.EnvPosSetup.GblSdcSysNum;
                        smsMessage1 += "/" + SelectedTranModel.TranInformation.ReceiptNumber + "\n";

                        string smsMessage2 = printingServiceSMS.GetSmsMessage(SelectedTranModel.Journal);
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "To:" + phoneNumber, smsMessage1 + smsMessage2, "Ok");

                        // 영수증을 문자로 보냄
                        printingServiceSMS.SendSMS(phoneNumber, smsMessage1 + smsMessage2);
                    }
                    catch (Exception ex)
                    {

                    }

                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;

                case "FinishedSales":
                    PrintingService printingService = new PrintingService();
                    // JINIT_파리미터를 List에서 JournalModel로 변경,
                    //printingService.writeJurnal(SelectedTranModel.Journal.JournalStringList, true);
                    printingService.writeJurnal(SelectedTranModel.Journal, true);

                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;
                default:
                    break;
            }
        }
        // JINIT_201911,
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

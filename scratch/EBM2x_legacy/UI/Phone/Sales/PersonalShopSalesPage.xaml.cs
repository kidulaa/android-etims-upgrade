using EBM2x.Models;
using EBM2x.Models.ListView;
using EBM2x.Models.tran;
using EBM2x.Process;
using EBM2x.Process.customer;
using EBM2x.Process.refund;
using EBM2x.Process.signoff;
using EBM2x.Process.tran;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Phone.SignOn;
using EBM2x.UI.Popup;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Phone.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalShopSalesPage : ContentPage
    {
        bool IsEnteredItem = false;

        public PersonalShopSalesPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            if (!UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
            {
                nonVat01.IsVisible = false;
                nonVat02.IsVisible = false;
                nonVat03.IsVisible = false;
            }

            // DATA 초기화
            UIManager.Instance().PosModel.TranModel.TranNode = new TranNode();
            UIManager.Instance().PosModel.TranModel.TranInformation.LogFlag = TranDefine.LOG_TRAN;
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.Clear();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage;
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage;
            UIManager.Instance().PosModel.TranModel.TranNode.CustomerNode.clear();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // 반품이면 반품화면으로 이동한다.
            if (UIManager.Instance().PosModel.TranModel.TranNode.TranFlag.Equals(TranDefine.TRAN_RETURN))
            {
                Navigation.InsertPageBefore(new PersonalShopRefundSalesPage(), this);
                Navigation.PopAsync();
            }

            // 상품 등록 모드 처리
            ModeCheck();

            Subscribe();

            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Item");
        }

        protected override void OnDisappearing()
        {
            Unsubscribe();
            base.OnDisappearing();
        }
        void ModeCheck()
        {
            if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Count() == 0)
            {
                IsEnteredItem = false;
                readyFunctionPanel.IsVisible = true;
                readyExtFunctionPanel.IsVisible = false;
                enteredItemFunctionPanel.IsVisible = false;
                enteredItemExtFunctionPanel.IsVisible = false;

                inputPriceFunctionPanel.IsVisible = false;
            }
            else
            {
                readyFunctionPanel.IsVisible = false;
                readyExtFunctionPanel.IsVisible = false;
                enteredItemFunctionPanel.IsVisible = true;
                enteredItemExtFunctionPanel.IsVisible = false;
                IsEnteredItem = true;

                inputPriceFunctionPanel.IsVisible = false;
                if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.GetLast().Price == 0)
                {
                    enteredItemFunctionPanel.IsVisible = false;
                    inputPriceFunctionPanel.IsVisible = true;
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

            MessagingCenter.Subscribe<Object, ItemList>(this, "Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesPanel.SalesItemListInvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, TranNode>(this, "Tran Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesTotal.InvalidateSurface(arg);
                    membersPanel.InvalidateSurface(arg);
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, ItemList>(this, "Item Node");
            MessagingCenter.Unsubscribe<Object, TranNode>(this, "Tran Node");
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Count() == 0)
            {
                if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate != System.DateTime.Now.ToString("yyyyMMdd"))
                {
                    string locationTitle2 = UILocation.Instance().GetLocationText("Warning");
                    string locationMessage2 = UILocation.Instance().GetLocationText("The previous date is not close.Should we go to the menu?");
                    var retClose = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (retClose)
                    {
                        SignOffProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                        await Navigation.PopAsync();
                    }
                }
            }

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "ExtMenu":
                    if (IsEnteredItem)
                    {
                        // Extend the menu.
                        enteredItemExtFunctionPanel.IsVisible = !enteredItemExtFunctionPanel.IsVisible;
                        enteredItemExtMenuButton.InvalidateSurface(enteredItemExtFunctionPanel.IsVisible);
                    }
                    else
                    {
                        // Extend the menu.
                        readyExtFunctionPanel.IsVisible = !readyExtFunctionPanel.IsVisible;
                        readyExtMenuButton.InvalidateSurface(readyExtFunctionPanel.IsVisible);
                    }
                    break;

                case "SignOff":
                    SignOffProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    Navigation.InsertPageBefore(new PersonalShopSignOnPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "Enter":
                case "PLU":
                    string resultPlu = AddBarcodeItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    if (StateModel.IsIt_OP_NEXT(resultPlu))
                    {
                        bool stockCheck1 = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, 0);
                        if (!stockCheck1)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
                        UIManager.Instance().InformationModel.SetWarningMessage("");
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }

                    break;

                case "Camera":
                    OnScanTapped();
                    break;

                case "SearchItem":
                    OnSearchItemTapped();
                    break;

                case "TIN":
                    OnSearchCustomerTapped();
                    break;

                case "Quantity":
                    double qty = ChangeQuantityProcess.Quantity(UIManager.Instance().InputModel);
                    bool stockCheck2 = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, qty);
                    if (!stockCheck2)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                        UIManager.Instance().InputModel.Clear();
                    }
                    else
                    {
                        ChangeQuantityProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        UIManager.Instance().InputModel.Clear();
                    }
                    break;

                case "ChangePrice":
                    ChangePriceProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    // 상품 등록 모드 처리
                    ModeCheck();
                    break;

                case "Discount"://JCAN 2020.1.20
                    if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.IsItemDiscountInsurers())
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This product cannot be discounted.", "Ok"); 
                        UIManager.Instance().InputModel.Clear();
                        break;
                    }
                    ChangeDiscountProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "CancelItem":
                    CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    // 상품 등록 모드 처리
                    ModeCheck();
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "CancelSales":
                    string locationTitle2 = UILocation.Instance().GetLocationText("Cancel?");
                    string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure to Cancel");
                    var cancel = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (cancel)
                    {
                        CancelSalesProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        UIManager.Instance().InputModel.Clear();
                        OnAppearing();
                    }
                    break;

                case "Payment":
                    await Navigation.PushAsync(new PersonalShopPaymentPage());
                    break;

                case "Refund":
                    OnRefundSalesTapped();
                    break;

                case "SearchReceipt":
                    OnSearchReceiptTapped();
                    break;

                case "RegisterCustomer":
                    await Navigation.PushAsync(new PhoneCustomerManagementPopupPage());
                    break;
                case "RegisterItem":
                    await Navigation.PushAsync(new TabletItemManagementPopupPage());
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
                Device.BeginInvokeOnMainThread(() => {
                    SearchCustomerNode searchCustomerNode = (SearchCustomerNode)((ExtEventArgs)ex).EnteredObject;
                    AddSearchCustomerProcess.excuteProcess(searchCustomerNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            popupPage.OnCanceled += (sender, e) => {
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        async void OnScanTapped()
        {
            var popupPage = new PhoneScanningPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnScanResult += (result) => {
                popupPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    UIManager.Instance().InputModel.EnteredText = result.Text;
                    //DisplayAlert("Barcode", "[" + UIManager.Instance().InputModel.EnteredText + "]", "OK");
                    string resultPlu = AddBarcodeItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    if (StateModel.IsIt_OP_NEXT(resultPlu))
                    {
                        bool stockCheck1 = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, 0);
                        if (!stockCheck1)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This barcode is not registered." + "[" + result.Text + "]", "Ok");
                    }
                    UIManager.Instance().InputModel.Clear();
                });
            };

            popupPage.OnCanceled += (sender, e) => {
                popupPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        async void OnSearchItemTapped()
        {
            var popupPage = new PhoneSearchItemPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    SearchItemNode searchItemNode = (SearchItemNode)((ExtEventArgs)ex).EnteredObject;
                    string resultPlu = AddSearchItemProcess.excuteProcess(searchItemNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    if (StateModel.IsIt_OP_NEXT(resultPlu))
                    {
                        bool stockCheck = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, 0);
                        if (!stockCheck)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }
                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (sender, e) => {
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        async void OnSearchReceiptTapped()
        {
            var popupPage = new PhoneSearchReceiptPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TranModel tranModel = (TranModel)((ExtEventArgs)ex).EnteredObject;
                    if (((ExtEventArgs)ex).FunctionID.Equals("RefundTranModel"))
                    {
                        ReloadTranProcess.excuteProcess(tranModel, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    }
                    else if (((ExtEventArgs)ex).FunctionID.Equals("ReloadTranModel"))
                    {
                        ReloadTranProcess.excuteProcess(tranModel, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    }

                    // 상품 등록 모드 처리
                    ModeCheck();

                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (sender, e) => {
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        async void OnRefundSalesTapped()
        {
            var popupPage = new PhoneRefundReasonPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    SearchRefundReasonNode searchRefundReasonNode = (SearchRefundReasonNode)((ExtEventArgs)ex).EnteredObject;
                    AddSearchRefundReasonProcess.excuteProcess(searchRefundReasonNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            popupPage.OnCanceled += (sender, e) => {
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }
    }
}

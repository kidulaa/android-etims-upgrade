using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.Models.config;
using EBM2x.Models.hold;
using EBM2x.Models.ListView;
using EBM2x.Models.Preset;
using EBM2x.Models.tran;
using EBM2x.Process;
using EBM2x.Process.customer;
using EBM2x.Process.hold;
using EBM2x.Process.refund;
using EBM2x.Process.search;
using EBM2x.Process.signoff;
using EBM2x.Process.tran;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using EBM2x.UI.Tablet.SignOn;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PharmacySalesPage : ContentPage
    {
        bool IsEnteredItem = false;

        public PharmacySalesPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            // DATA 초기화
            UIManager.Instance().PosModel.TranModel.TranNode = new TranNode();
            UIManager.Instance().PosModel.TranModel.TranInformation.LogFlag = TranDefine.LOG_TRAN;
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.Clear();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage;
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage;
            UIManager.Instance().PosModel.TranModel.TranNode.CustomerNode.clear();
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // 반품이면 반품화면으로 이동한다.
            if (UIManager.Instance().PosModel.TranModel.TranNode.TranFlag.Equals(TranDefine.TRAN_RETURN))
            {
                Navigation.InsertPageBefore(new PharmacyRefundSalesPage(), this);
                Navigation.PopAsync();
            }

            // 상품 등록 모드 처리
            ModeCheck();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Pharmacy|Sales");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Item");

            UIManager.Instance().PosModel.TranModel.TranNode.InsurerNode = new SearchInsurerNode();

            presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, false, false);
            holdTextButton.InvalidateSurface(string.Format("Hold[{0}]", UIManager.Instance().PosModel.RegiTotal.RegiHeader.HoldCount));
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

            MessagingCenter.Subscribe<Object, TranNode>(this, "Tran Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesReceive.InvalidateSurface(arg);
                    salesChange.InvalidateSurface(arg);
                    salesTotal.InvalidateSurface(arg);
                    membersPanel.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, ItemList>(this, "Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesPanel.SalesItemListInvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, PresetGroupList>(this, "Preset Group Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    presetMenuPanel.PresetGroupListInvalidateSurface(arg, false, false);
                });
            });

            MessagingCenter.Subscribe<Object, PresetItemList>(this, "Preset Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    presetMenuPanel.PresetItemListInvalidateSurface(arg, false, false);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "Hold Count", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    holdTextButton.InvalidateSurface(string.Format("Hold[{0}]", arg));
                });
            });
            MessagingCenter.Subscribe<Object, string>(this, "Wait Count", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    waitTextButton.InvalidateSurface(string.Format("Wait[{0}]", arg));
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, TranNode>(this, "Tran Node");
            MessagingCenter.Unsubscribe<Object, ItemList>(this, "Item Node");
            MessagingCenter.Unsubscribe<Object, PresetGroupList>(this, "Preset Group Node");
            MessagingCenter.Unsubscribe<Object, PresetItemList>(this, "Preset Item Node");
            MessagingCenter.Unsubscribe<Object, PosModel>(this, "Hold Count");
            MessagingCenter.Unsubscribe<Object, PosModel>(this, "Wait Count");
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            //Added By Bright 18.4.2022 start
            //Check Date Time of the last invoice

            TrnsSaleReceiptMaster receiptMaster = new TrnsSaleReceiptMaster();
            string LAST_INVC_TIME = receiptMaster.GetLastReceiptSeqTime();

            int result1 = DateTime.Compare(DateTime.ParseExact(LAST_INVC_TIME, "yyyyMMddHHmmss", null), DateTime.Now);

            if (result1 > 0)
            {
   

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please check the system time", "OK");
                return;
            }

            //Added By Bright 18.4.2022 End

            if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Count() == 0)
            {
                if (UIManager.Instance().PosModel.RegiTotal.RegiHeader.OpenDate != System.DateTime.Now.ToString("yyyyMMdd"))
                {
                    string locationTitle3 = UILocation.Instance().GetLocationText("Warning");
                    string locationMessage3 = UILocation.Instance().GetLocationText("The previous date is not close.Should we go to the menu?");
                    var retClose = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                    if (retClose)
                    {
                        SignOffProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        Navigation.InsertPageBefore(new SalesMenuPage(), this);
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
                    Navigation.InsertPageBefore(new PharmacySignOnPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "Lock":
                    OnLockTapped();
                    break;

                case "Enter":
                case "PLU":
                    EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
                    List<SearchItemNode> list = SearchItemProcess.QuerydSearchItemBarcode(envPosSetup.GblTaxIdNo, UIManager.Instance().InputModel.EnteredText);
                    //List<SearchItemNode> list = SearchItemProcess.QuerydSearchItem(envPosSetup.GblTaxIdNo, UIManager.Instance().InputModel.EnteredText);
                    if (list != null && list.Count > 1)
                    {
                        OnSearchItemBarcodeTapped(list);
                    }
                    else
                    {
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
                    }

                    break;

                case "Preset":
                    PresetItemNode itemNode = (PresetItemNode)((ExtEventArgs)e).EnteredObject;
                    string resultPreset = AddPresetItemProcess.excuteProcess(itemNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    if (StateModel.IsIt_OP_NEXT(resultPreset))
                    {
                        bool stockCheck2 = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, 0);
                        if (!stockCheck2)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
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
                    bool stockCheck3 = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, qty);
                    if (!stockCheck3)
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
                        UIManager.Instance().PosModel.TinInitailizeTran();
                        UIManager.Instance().InputModel.Clear();
                        OnAppearing();
                    }
                    break;

                case "HoldProcessing":
                    string locationTitle3 = UILocation.Instance().GetLocationText("Hold?");
                    string locationMessage3 = UILocation.Instance().GetLocationText("Do you want to hold it?");
                    var hold = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                    if (hold)
                    {
                        string result = HoldTranProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        UIManager.Instance().PosModel.TinInitailizeTran();
                        UIManager.Instance().InputModel.Clear();
                        UIManager.Instance().PosModel.InvalidateSurfaceHoldWait();
                        OnAppearing();
                    }
                    break;

                case "RestoreHold":
                    OnRestoreHoldTapped();
                    break;

                case "Payment":
                    if(UIManager.Instance().PosModel.TranModel.TranNode.ItemList.IsDiscountInsurers())
                    {
                        await Navigation.PushAsync(new PharmacyInsurerPage());
                    }
                    else
                    {
                        await Navigation.PushAsync(new TabletPaymentPage());
                    }
                    break;

                case "Refund":
                    OnRefundSalesTapped();
                    break;

                case "SearchReceipt":
                    OnSearchReceiptTapped();
                    break;

                case "RegisterCustomer":
                    await Navigation.PushAsync(new TabletCustomerManagementPopupPage());
                    break;
                case "RegisterItem":
                    await Navigation.PushAsync(new TabletItemManagementPopupPage());
                    break;

                default:
                    break;
            }
        }
        async void OnLockTapped()
        {
            var localPage = new TabletPosLockPopupPage();
            localPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            localPage.OnSignOffResult += (result, e) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    MessagingCenter.Send<Object, string>(this, "Function", "SignOff");
                });
            };
            await Navigation.PushAsync(localPage);
        }
        async void OnScanTapped()
        {
            var scanPage = new TabletScanningPopupPage();
            scanPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            scanPage.OnScanResult += (result) => {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string resultPlu = AddBarcodeItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    if (StateModel.IsIt_OP_NEXT(resultPlu))
                    {
                        bool stockCheck1 = ChangeQuantityProcess.checkStockProcess(UIManager.Instance().PosModel, 0);
                        if (!stockCheck1)
                        {
                            DisplayAlert("Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        DisplayAlert("Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }
                    Navigation.PopAsync();
                });
            };

            scanPage.OnCanceled += (sender, e) => {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(scanPage);
        }

        async void OnSearchCustomerTapped()
        {
            var popupPage = new TabletSearchCustomerPopupPage();
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

        async void OnSearchItemTapped()
        {
            var popupPage = new TabletSearchItemPopupPage();
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
                            DisplayAlert("Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        DisplayAlert("Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }

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
        async void OnSearchItemBarcodeTapped(List<SearchItemNode> list)
        {
            var popupPage = new TabletSearchItemPopupPage(list);
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
                            DisplayAlert("Confirm", "Sales quantity is greater than current stock quantity.", "Ok");
                            CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        }
                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        DisplayAlert("Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }

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
        async void OnRestoreHoldTapped()
        {
            var popupPage = new TabletRestoreHoldPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    HoldNode holdNode = (HoldNode)((ExtEventArgs)ex).EnteredObject;
                    RestoreHoldTranProcess.excuteProcess(holdNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    // 상품 등록 모드 처리
                    ModeCheck();
                    UIManager.Instance().PosModel.InvalidateSurfaceHoldWait();

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

        async void OnSearchReceiptTapped()
        {
            var popupPage = new TabletSearchReceiptPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    /* JINIT
                    AddBarcodeItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    // 상품 등록 모드 처리
                    ModeCheck();
                    Navigation.PopAsync();
                    */

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
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }
        async void OnRefundSalesTapped()
        {
            var popupPage = new TabletRefundReasonPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    SearchRefundReasonNode searchRefundReasonNode = (SearchRefundReasonNode)((ExtEventArgs)ex).EnteredObject;
                    AddSearchRefundReasonProcess.excuteProcess(searchRefundReasonNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

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

using EBM2x.Dependency;
using EBM2x.Models;
using EBM2x.Models.DiningTable;
using EBM2x.Models.ListView;
using EBM2x.Models.Preset;
using EBM2x.Models.tran;
using EBM2x.Process.customer;
using EBM2x.Process.dining;
using EBM2x.Process.tran;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using EBM2x.UI.Tablet.Sales;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Order
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiningTableOrderPage : ContentPage
    {
        DiningTableNode diningTableNode = null;
        bool IsEnteredItem = false;

        public DiningTableOrderPage(DiningTableNode node)
        {
            this.diningTableNode = node;

            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            // DATA 초기화
            TranNode oldTranNode = UIManager.Instance().PosModel.TranModel.TranNode;

            TranNode tranNode = DiningTableOrderProcess.LoadOrder(diningTableNode.DiningTableCode);
            if(tranNode != null)
            {
                this.diningTableNode.IsOrdered = true;
                UIManager.Instance().PosModel.TranModel.TranNode = tranNode;
                UIManager.Instance().PosModel.TranModel.TranInformation.LogFlag = TranDefine.LOG_TRAN;
            }
            else
            {
                UIManager.Instance().PosModel.TranModel.TranNode = new TranNode();
                UIManager.Instance().PosModel.TranModel.TranInformation.LogFlag = TranDefine.LOG_TRAN;
                //UIManager.Instance().PosModel.TranModel.TranNode.TranFlag = TranDefine.TRAN_NORMAL;
                UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();
                UIManager.Instance().PosModel.TranModel.TranNode.TenderList.Clear();
                UIManager.Instance().PosModel.TranModel.TranNode.ItemList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage;
                UIManager.Instance().PosModel.TranModel.TranNode.TenderList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage;
                UIManager.Instance().PosModel.TranModel.TranNode.CustomerNode.clear();

                this.diningTableNode.IsOrdered = true;
                this.diningTableNode.FirstOrderTime = DateTime.Now;
                UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // 상품 등록 모드 처리
            ModeCheck();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Restaurant/Bar|Order");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Item");

            diningMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, false, false);
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
                // 서비스 처리 추가
                if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.GetLast().Price == 0 
                    && !UIManager.Instance().PosModel.TranModel.TranNode.ItemList.GetLast().IsService)
                {
                    enteredItemFunctionPanel.IsVisible = false;
                    inputPriceFunctionPanel.IsVisible = true;
                }
            }

            diningTablePanel.InvalidateSurface(diningTableNode);
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
                    membersPanel.InvalidateSurface(arg);
                    salesTotal.InvalidateSurface(arg);
                    //                    membersPanel.InvalidateSurface(arg);
                    diningTablePanel.InvalidateSurface(diningTableNode);
                });
            });

            MessagingCenter.Subscribe<Object, PresetGroupList>(this, "Preset Group Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningMenuPanel.PresetGroupListInvalidateSurface(arg, false, false);
                });
            });

            MessagingCenter.Subscribe<Object, PresetItemList>(this, "Preset Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningMenuPanel.PresetItemListInvalidateSurface(arg, false, false);
                });
            });

        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, ItemList>(this, "Item Node");
            MessagingCenter.Unsubscribe<Object, TranNode>(this, "Tran Node");
            MessagingCenter.Unsubscribe<Object, PresetGroupList>(this, "Preset Group Node");
            MessagingCenter.Unsubscribe<Object, PresetItemList>(this, "Preset Item Node");
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
                        else if (StateModel.IsIt_OP_ALERT(resultPlu))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                        }

                        DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                        diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        // 상품 등록 모드 처리
                        ModeCheck();
                        UIManager.Instance().InformationModel.SetWarningMessage("");
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

                        DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                        diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        // 상품 등록 모드 처리
                        ModeCheck();
                        UIManager.Instance().InformationModel.SetWarningMessage("");
                    }

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

                    DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                    diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                    DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    // 상품 등록 모드 처리
                    ModeCheck();
                    break;

                case "SearchItem":
                    OnSearchItemTapped();
                    break;

                case "TIN":
                    OnSearchCustomerTapped();
                    break;

                case "ChangePrice":
                    ChangePriceProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                    diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                    DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    // 상품 등록 모드 처리
                    ModeCheck();
                    break;

                case "Service":
                    SetServiceProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                    diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                    DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

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

                    DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                    diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                    DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    // 상품 등록 모드 처리
                    ModeCheck();

                    break;

                case "CancelItem":
                    CancelItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

                    DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                    diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                    DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    // 상품 등록 모드 처리
                    ModeCheck();
                    UIManager.Instance().InputModel.Clear();

                    // JINIT_201911, 등록된 상품이 없으면
                    if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Count() < 1)
                    {
                        // JINIT_201911, DiningTable 초기화처리
                        DiningTableOrderProcess.DeleteOrder(diningTableNode.DiningTableCode);
                        this.diningTableNode.initDiningTable();

                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        this.diningTableNode.IsOrdered = false;
                        this.diningTableNode.FirstOrderTime = DateTime.MinValue;

                        await Navigation.PopAsync();
                    }

                    break;

                case "CancelSales":
                    string locationTitle2 = UILocation.Instance().GetLocationText("Cancel?");
                    string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure to Cancel");
                    var cancel = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (cancel)
                    {
                        CancelSalesProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        UIManager.Instance().InputModel.Clear();

                        // JINIT_201911, SaveOrder를 DeleteOrder로 변경
                        DiningTableOrderProcess.DeleteOrder(diningTableNode.DiningTableCode);
                        // JINIT_201911, DiningTable 초기화처리
                        this.diningTableNode.initDiningTable();

                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        this.diningTableNode.IsOrdered = false;
                        this.diningTableNode.FirstOrderTime = DateTime.MinValue;
                            
                        await Navigation.PopAsync();
                    }

                    break;

                case "GroupPayment":
                    if(diningTableNode == null || diningTableNode.IsGroup != true)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Group?", "No item specified..", "OK");
                        break;
                    }

                    string locationTitle21 = UILocation.Instance().GetLocationText("Payment?");
                    string locationMessage21 = UILocation.Instance().GetLocationText("All are summed and processed.");
                    var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
                    if (result)
                    {
                        DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                        diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        TranNode tranNode = DiningTableOrderProcess.LoadGroupOrder(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, diningTableNode);
                        UIManager.Instance().PosModel.TranModel.TranNode = tranNode;
                        UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
                        UIManager.Instance().PosModel.TranModel.TranNode.ItemList.InvalidateSurface();
                        diningTablePanel.InvalidateSurface(diningTableNode);

                        // JINIT_201911, 그룹후 결제로 이동
                        var popupPageGroup = new TabletPaymentPage();
                        popupPageGroup.SetValue(NavigationPage.HasNavigationBarProperty, false);
                        popupPageGroup.OnConfirmed += (popup, ex) => {
                            Device.BeginInvokeOnMainThread(() => {
                                UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();

                                Navigation.PopAsync();

                                // JINIT_201911, DiningTable 초기화처리
                                DiningTableOrderProcess.DeleteOrder(diningTableNode.DiningTableCode);
                                this.diningTableNode.initDiningTable();
                                DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                                Navigation.PopAsync();
                                //Navigation.PopToRootAsync();
                            });
                        };
                        await Navigation.PushAsync(popupPageGroup);
                        break;
                    }
                    break;

                case "Payment":
                    var popupPage = new TabletPaymentPage();
                    popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);
                    popupPage.OnConfirmed += (popup, ex) => {
                        Device.BeginInvokeOnMainThread(() => {
                            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();

                            Navigation.PopAsync();

                            // JINIT_201911, DiningTable 초기화처리
                            DiningTableOrderProcess.DeleteOrder(diningTableNode.DiningTableCode);
                            this.diningTableNode.initDiningTable();
                            DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                            Navigation.PopAsync();
                            //Navigation.PopToRootAsync();
                        });
                    };
                    await Navigation.PushAsync(popupPage);
                    break;

                case "PrintBill":
                    UIManager.Instance().PosModel.Journal.Clear();
                    Journal.tran.TranNodeJournal journalCreate = new Journal.tran.TranNodeJournal();
                    journalCreate.createToCustWithoutPayment(UIManager.Instance().PosModel);

                    PrintingService printingService = new PrintingService();
                    printingService.writeJurnal(UIManager.Instance().PosModel.Journal, false);

                    UIManager.Instance().PosModel.Journal.Clear();
                    break;

                case "Arrangement": // JINIT_201911, 사용하지 않음(버튼삭제)
                    // JINIT_201911, CANCEL과 동일하게 처리
                    if (UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Count() == 0)
                    {
                        this.diningTableNode.initDiningTable();
                        this.diningTableNode.IsOrdered = false;
                        this.diningTableNode.FirstOrderTime = DateTime.MinValue;
                    }

                    await Navigation.PopAsync();
                    break;
                case "Cancel":
                    if(UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Count() == 0)
                    {
                        this.diningTableNode.initDiningTable();
                        this.diningTableNode.IsOrdered = false;
                        this.diningTableNode.FirstOrderTime = DateTime.MinValue;
                    }

                    await Navigation.PopAsync();
                    break;

                case "Ordered":
                    // JINIT_201911, 등록된 상품이 없으면 오류처리
                    if (! DiningTableOrderProcess.CheckOrder(UIManager.Instance().PosModel.TranModel.TranNode))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please input item first.", "OK");
                        break;
                    }

                    DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                    diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                    DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);
                    UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();
                    await Navigation.PopAsync();
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

        async void OnScanTapped()
        {
            var popupPage = new PhoneScanningPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnScanResult += (result) => {
                // Stop scanning
                popupPage.IsScanning = false;

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

                        DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                        diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        // 상품 등록 모드 처리
                        ModeCheck();
                        UIManager.Instance().InformationModel.SetWarningMessage("");
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        DisplayAlert("Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }
                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (sender, e) => {
                // Stop scanning
                popupPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    UIManager.Instance().InputModel.Clear();
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
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

                        DiningTableOrderProcess.SaveOrder(diningTableNode.DiningTableCode, UIManager.Instance().PosModel.TranModel.TranNode);
                        diningTableNode.Amount = UIManager.Instance().PosModel.TranModel.TranNode.Subtotal;
                        DiningRoomNodeProcess.SaveDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
                    else if (StateModel.IsIt_OP_ALERT(resultPlu))
                    {
                        DisplayAlert("Confirm", UIManager.Instance().InformationModel.AlertMessage, "Ok");
                    }
                    if (!IsEnteredItem)
                    {
                        readyFunctionPanel.IsVisible = false;
                        readyExtFunctionPanel.IsVisible = false;
                        enteredItemFunctionPanel.IsVisible = true;
                        enteredItemExtFunctionPanel.IsVisible = false;
                        IsEnteredItem = true;
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

    }
}

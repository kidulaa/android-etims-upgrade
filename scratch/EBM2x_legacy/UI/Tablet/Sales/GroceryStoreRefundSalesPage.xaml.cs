using EBM2x.Models;
using EBM2x.Models.ListView;
using EBM2x.Models.Preset;
using EBM2x.Models.tran;
using EBM2x.Process.customer;
using EBM2x.Process.tran;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroceryStoreRefundSalesPage : ContentPage
    {
        bool IsEnteredItem = false;

        public GroceryStoreRefundSalesPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            // DATA 초기화
            //            UIManager.Instance().PosModel.TranModel.TranNode = new TranNode();
            // 반품 모드로 설정 한다.
            //            UIManager.Instance().PosModel.TranModel.TranNode.TranFlag = TranDefine.TRAN_RETURN;
            // 
            //            UIManager.Instance().PosModel.TranModel.TranInformation.LogFlag = TranDefine.LOG_TRAN;
            //            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.Clear();
            UIManager.Instance().PosModel.TranModel.TranNode.Receive = 0;
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.Clear();
            //            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage;
            //            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.CountOfItemsToDisplayOnOnePage = UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage;
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // 정상이면 정상화면으로 이동한다.
            if (UIManager.Instance().PosModel.TranModel.TranNode.TranFlag.Equals(TranDefine.TRAN_NORMAL))
            {
                Navigation.InsertPageBefore(new GroceryStoreSalesPage(), this);
                Navigation.PopAsync();
            }

            // 상품 등록 모드 처리
            ModeCheck();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Refund");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Item");

            //presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, false, false);
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
                    salesPanel.InvalidateSurface(arg);
                    membersPanel.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, ItemList>(this, "Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesPanel.SalesItemListInvalidateSurface(arg);
                });
            });

            //MessagingCenter.Subscribe<Object, PresetGroupList>(this, "Preset Group Node", (sender, arg) => {
            //    Device.BeginInvokeOnMainThread(() => {
            //        presetMenuPanel.PresetGroupListInvalidateSurface(arg, false, false);
            //    });
            //});

            //MessagingCenter.Subscribe<Object, PresetItemList>(this, "Preset Item Node", (sender, arg) => {
            //    Device.BeginInvokeOnMainThread(() => {
            //        presetMenuPanel.PresetItemListInvalidateSurface(arg, false, false);
            //    });
            //});

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
            //MessagingCenter.Unsubscribe<Object, PresetGroupList>(this, "Preset Group Node");
            //MessagingCenter.Unsubscribe<Object, PresetItemList>(this, "Preset Item Node");
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

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "ExtMenu":
                    if(IsEnteredItem)
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
                    string result = AddBarcodeItemProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    if (StateModel.IsIt_OP_NEXT(result))
                    {
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
                        // 상품 등록 모드 처리
                        ModeCheck();
                        UIManager.Instance().InformationModel.SetWarningMessage("");
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
                    ChangeQuantityProcess.excuteProcessRefund(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "ChangePrice":
                    InformationModel informationModel = new InformationModel();
                    Double price = 0.00;
                    if (!string.IsNullOrEmpty(UIManager.Instance().InputModel.EnteredText))
                    {
                        price = UIManager.Instance().PosModel.TranModel.TranNode.ItemList.GetCurrentItem().Price;
                        //Console.WriteLine("here------------------");
                    }
                    else
                    {
                        informationModel.SetWarningMessage("Price has not been changed");
                        break;
                    }
                    // Console.WriteLine("price------------------" + price);
                    string enteredText = UIManager.Instance().InputModel.EnteredText;
                    if (price < Double.Parse(enteredText))
                    {
                        informationModel.SetWarningMessage("Please enter a price below the original Price!!!");
                        break;
                    }
                    else
                    {
                        ChangePriceProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                        UIManager.Instance().InputModel.Clear();
                        // 상품 등록 모드 처리
                        ModeCheck();
                    }
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
                    await Navigation.PushAsync(new TabletRefundPaymentPage());
                    break;

                case "RegisterCustomer":
                    await Navigation.PushAsync(new TabletCustomerManagementPopupPage());
                    break;

                case "CancelRefund":
                    UIManager.Instance().PosModel.TranModel.TranNode.TranFlag = TranDefine.TRAN_NORMAL;
                    Navigation.InsertPageBefore(new GroceryStoreSalesPage(), this);
                    await Navigation.PopAsync();
                    break;                 

                default:
                    break;
            }
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
                        // 상품 등록 모드 처리
                        ModeCheck();
                        UIManager.Instance().InformationModel.SetWarningMessage("");
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
                    AddSearchItemProcess.excuteProcess(searchItemNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    UIManager.Instance().InputModel.Clear();

                    // 상품 등록 모드 처리
                    ModeCheck();
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

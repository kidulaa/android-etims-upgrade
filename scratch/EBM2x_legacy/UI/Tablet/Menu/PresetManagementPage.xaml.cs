using EBM2x.Models.ListView;
using EBM2x.Models.Preset;
using EBM2x.Process.preset;
using EBM2x.Services;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PresetManagementPage : ContentPage
    {
        PresetItemNode PresetItemNode = null;
        PresetGroupNode PresetGroupNode = null;

        public PresetManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGridGroup.IsVisible = false;
            fixedGridItem.IsVisible = false;

            itemCode.TitleInvalidateSurface("Code :");
            itemCode.GetEntry().IsReadOnly = true;

            itemName.TitleInvalidateSurface("Name :");
            itemPrice.TitleInvalidateSurface("Price :");

            groupName.TitleInvalidateSurface("Name :");
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Preset|Management");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Item");

            presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
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

            MessagingCenter.Subscribe<Object, PresetGroupList>(this, "Preset Group Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    presetMenuPanel.PresetGroupListInvalidateSurface(arg, true, true);
                });
            });

            MessagingCenter.Subscribe<Object, PresetItemList>(this, "Preset Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    presetMenuPanel.PresetItemListInvalidateSurface(arg, true, true);
                });
            });

        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
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
                case "PresetGroup":
                    if (PresetGroupNode != null)
                    {
                        PresetGroupNode.IsOwnerVisible = false;
                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }

                    PresetGroupNode = (PresetGroupNode)((ExtEventArgs)e).EnteredObject;
                    //PresetGroupNode.IsVisible = true;
                    PresetGroupNode.IsOwnerVisible = true;
                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    fixedGridGroup.IsVisible = true;
                    fixedGridItem.IsVisible = false;

                    groupName.GetEntry().Text = PresetGroupNode.GroupName;

                    UIManager.Instance().InputModel.Clear();

                    break;

                case "Preset":
                    if (PresetGroupNode != null)
                    {
                        PresetGroupNode.IsOwnerVisible = false;
                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }

                    PresetItemNode = (PresetItemNode)((ExtEventArgs)e).EnteredObject;
                    //PresetItemNode.IsVisible = true;
                    PresetItemNode.IsOwnerVisible = true;
                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    if (PresetItemNode.IsVisible)
                    {
                        uncheckedButton.IsVisible = false;
                        checkedButton.IsVisible = true;
                    }
                    else
                    {
                        uncheckedButton.IsVisible = true;
                        checkedButton.IsVisible = false;
                    }

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = true;

                    itemCode.GetEntry().Text = PresetItemNode.ItemCode;
                    itemName.GetEntry().Text = PresetItemNode.ItemName;
                    itemPrice.GetEntry().Text = "" + PresetItemNode.Price;

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Check":
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsVisible = true;
                        if (PresetItemNode.IsVisible)
                        {
                            uncheckedButton.IsVisible = false;
                            checkedButton.IsVisible = true;
                        }
                        else
                        {
                            uncheckedButton.IsVisible = true;
                            checkedButton.IsVisible = false;
                        }

                    }

                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Uncheck":
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsVisible = false;
                        if (PresetItemNode.IsVisible)
                        {
                            uncheckedButton.IsVisible = false;
                            checkedButton.IsVisible = true;
                        }
                        else
                        {
                            uncheckedButton.IsVisible = true;
                            checkedButton.IsVisible = false;
                        }
                    }

                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "ConfirmGroup":
                    if (PresetGroupNode != null)
                    {
                        if (PresetGroupNode.IsVisible)
                        {
                            if (string.IsNullOrEmpty(groupName.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid GroupName.", "OK");
                                return;
                            }
                            PresetGroupNode.GroupName = groupName.GetEntry().Text;
                        }
                        else
                        {
                            PresetGroupNode.GroupName = "";
                        }


                        PresetGroupNode.IsOwnerVisible = false;

                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }

                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save preset file
                    PresetService.SaveList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);
                    PresetService.SaveHistoryList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "ConfirmItem":
                    if (PresetGroupNode != null)
                    {
                        PresetGroupNode.IsOwnerVisible = false;
                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        if(PresetItemNode.IsVisible)
                        {
                            if(string.IsNullOrEmpty(itemCode.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid ItemCode.", "OK");
                                return;
                            }
                            if (string.IsNullOrEmpty(itemName.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid ItemName.", "OK");
                                return;
                            }
                            if (string.IsNullOrEmpty(itemPrice.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid price.", "OK");
                                return;
                            }
                            else
                            {
                                try
                                {
                                    double price = double.Parse(itemPrice.GetEntry().Text);
                                    if(price <= 0)
                                    {
                                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid price.", "OK");
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid price.", "OK");
                                    return;
                                }
                            }

                            PresetItemNode.ItemCode = itemCode.GetEntry().Text;
                            PresetItemNode.ItemName = itemName.GetEntry().Text;
                            PresetItemNode.Price = double.Parse(itemPrice.GetEntry().Text);
                        }
                        else
                        {
                            PresetItemNode.ItemCode = "";
                            PresetItemNode.ItemName = "";
                            PresetItemNode.Price = 0;
                        }

                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }

                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save preset file
                    PresetService.SaveList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);
                    PresetService.SaveHistoryList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Cancel":
                    if (PresetGroupNode != null)
                    {
                        PresetGroupNode.IsOwnerVisible = false;
                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }

                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Save":
                    if (PresetGroupNode != null)
                    {
                        PresetGroupNode.IsOwnerVisible = false;
                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }
                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save preset file
                    PresetService.SaveList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);
                    PresetService.SaveHistoryList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);

                    UIManager.Instance().InputModel.Clear();
                    break;
                case "Back":
                    if (PresetGroupNode != null)
                    {
                        PresetGroupNode.IsOwnerVisible = false;
                        PresetGroupNode = null;
                    }
                    if (PresetItemNode != null)
                    {
                        PresetItemNode.IsOwnerVisible = false;
                        PresetItemNode = null;
                    }
                    presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;
                case "Query":
                    OnSearchItemTapped();
                    break;

                case "AddGroup":
                    string locationTitle2 = UILocation.Instance().GetLocationText("Add?");
                    string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultAdd = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (resultAdd)
                    {
                        if (PresetGroupNode != null)
                        {
                            PresetGroupNode.IsOwnerVisible = false;
                            PresetGroupNode = null;
                        }
                        if (PresetItemNode != null)
                        {
                            PresetItemNode.IsOwnerVisible = false;
                            PresetItemNode = null;
                        }
                        PresetGroupNodeProcess.AddPresetGroup(UIManager.Instance().PosModel.PresetModel.PresetGroupList, 5);
                        presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "DeleteGroup":
                    string locationTitle3 = UILocation.Instance().GetLocationText("Delete?");
                    string locationMessage3 = UILocation.Instance().GetLocationText("Are you sure?");
                    var result = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                    if (result)
                    {
                        if (PresetGroupNode != null)
                        {
                            PresetGroupNode.IsOwnerVisible = false;
                            PresetGroupNode = null;
                        }
                        if (PresetItemNode != null)
                        {
                            PresetItemNode.IsOwnerVisible = false;
                            PresetItemNode = null;
                        }
                        PresetGroupNodeProcess.DeletePresetGroup(UIManager.Instance().PosModel.PresetModel.PresetGroupList,5);
                        presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "AddItem":
                    string locationTitle4 = UILocation.Instance().GetLocationText("Add?");
                    string locationMessage4 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultAddItem = await DisplayAlert(locationTitle4, locationMessage4, "Yes", "No");
                    if (resultAddItem)
                    {
                        if (PresetGroupNode != null)
                        {
                            PresetGroupNode.IsOwnerVisible = false;
                            PresetGroupNode = null;
                        }
                        if (PresetItemNode != null)
                        {
                            PresetItemNode.IsOwnerVisible = false;
                            PresetItemNode = null;
                        }
                        PresetGroupNodeProcess.AddPresetItem(UIManager.Instance().PosModel.PresetModel.PresetGroupList, 15);
                        presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "DeleteItem":
                    string locationTitle14 = UILocation.Instance().GetLocationText("Delete?");
                    string locationMessage14 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultDeleteItem = await DisplayAlert(locationTitle14, locationMessage14, "Yes", "No");
                    if (resultDeleteItem)
                    {
                        if (PresetGroupNode != null)
                        {
                            PresetGroupNode.IsOwnerVisible = false;
                            PresetGroupNode = null;
                        }
                        if (PresetItemNode != null)
                        {
                            PresetItemNode.IsOwnerVisible = false;
                            PresetItemNode = null;
                        }
                        PresetGroupNodeProcess.DeletePresetItem(UIManager.Instance().PosModel.PresetModel.PresetGroupList, 15);
                        presetMenuPanel.PresetGroupListInvalidateSurface(UIManager.Instance().PosModel.PresetModel.PresetGroupList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;

                default:
                    break;
            }
        }

        async void OnSearchItemTapped()
        {
            var popupPage = new TabletSearchItemPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    SearchItemNode searchItemNode = (SearchItemNode)((ExtEventArgs)ex).EnteredObject;

                    itemCode.GetEntry().Text = searchItemNode.ItemCode;
                    itemName.GetEntry().Text = searchItemNode.ItemName;
                    itemPrice.GetEntry().Text = "" + searchItemNode.Price;

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

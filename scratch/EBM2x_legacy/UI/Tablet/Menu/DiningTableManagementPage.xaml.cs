using EBM2x.Models.DiningTable;
using EBM2x.Process.dining;
using EBM2x.Services;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiningTableManagementPage : ContentPage
    {
        DiningTableNode DiningTableNode = null;
        DiningRoomNode DiningRoomNode = null;

        string diningTableNo = "";

        public DiningTableManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGridGroup.IsVisible = false;
            fixedGridItem.IsVisible = false;

            itemCode.TitleInvalidateSurface("Name :");
            //itemName.TitleInvalidateSurface("Name :");

            groupName.TitleInvalidateSurface("Name :");
            // JINIT_201911, 
            UIManager.Instance().InputModel.Clear();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Dining table|Management");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Table");

            diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
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

            MessagingCenter.Subscribe<Object, DiningRoomList>(this, "Dining Room Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(arg, true, true);
                });
            });

            MessagingCenter.Subscribe<Object, DiningTableList>(this, "Dining Table Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    diningTableManagementPanel.DiningTableListInvalidateSurface(arg, true, true);
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

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "DiningRoom":
                    if (DiningRoomNode != null)
                    {
                        DiningRoomNode.IsOwnerVisible = false;
                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsOwnerVisible = false;
                        DiningTableNode = null;
                    }

                    DiningRoomNode = (DiningRoomNode)((ExtEventArgs)e).EnteredObject;
                    //DiningRoomNode.IsVisible = true;
                    DiningRoomNode.IsOwnerVisible = true;
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    fixedGridGroup.IsVisible = true;
                    fixedGridItem.IsVisible = false;

                    groupName.GetEntry().Text = DiningRoomNode.RoomName;

                    UIManager.Instance().InputModel.Clear();

                    break;

                case "DiningTable":
                    if (DiningRoomNode != null)
                    {
                        DiningRoomNode.IsOwnerVisible = false;
                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsOwnerVisible = false;
                        DiningTableNode = null;
                    }

                    DiningTableNode = (DiningTableNode)((ExtEventArgs)e).EnteredObject;
                    //DiningTableNode.IsVisible = true;
                    DiningTableNode.IsOwnerVisible = true;
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    if (DiningTableNode.IsVisible)
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

                    itemCode.GetEntry().Text = DiningTableNode.DiningTableCode;

                    // JINIT_201911, DiningTableCode 중복체크를 위해서 원코드번호를 저장함
                    diningTableNo = DiningTableNode.DiningTableCode;

                    //itemName.GetEntry().Text = DiningTableNode.DiningTableName;

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Check":
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsVisible = true;
                        if (DiningTableNode.IsVisible)
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

                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Uncheck":
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsVisible = false;
                        if (DiningTableNode.IsVisible)
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

                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "ConfirmGroup":
                    if (DiningRoomNode != null)
                    {
                        if (DiningRoomNode.IsVisible)
                        {
                            if (string.IsNullOrEmpty(groupName.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid GroupName.", "OK");
                                return;
                            }
                            DiningRoomNode.RoomName = groupName.GetEntry().Text;
                        }
                        else
                        {
                            DiningRoomNode.RoomName = "";
                        }
                        DiningRoomNode.IsOwnerVisible = false;

                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsOwnerVisible = false;
                        DiningTableNode = null;
                    }

                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save preset file
                    //JINIT_201911, PresetService.SaveList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);
                    DiningTableService.SaveList(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);
                    DiningTableService.SaveHistoryList(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "ConfirmItem":

                    if (DiningRoomNode != null)
                    {
                        DiningRoomNode.IsOwnerVisible = false;
                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        if (DiningTableNode.IsVisible)
                        {
                            if (string.IsNullOrEmpty(itemCode.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid Dining Table No.", "OK");
                                return;
                            }
                            //if (string.IsNullOrEmpty(itemName.GetEntry().Text))
                            //{
                            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid Dining Table Name.", "OK");
                            //    return;
                            //}
                            // JINIT_201911, 테이블번호를 수정했으면 중복된 테이블번호가 있는지 체크
                            if (diningTableNo != itemCode.GetEntry().Text)
                            {
                                if (DiningRoomNodeProcess.DupCheckDiningTableNo(itemCode.GetEntry().Text))
                                {
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a duplicate Dining Table No.", "Ok");
                                    return;
                                }
                                //if (DiningRoomNodeProcess.DupCheckDiningTableName(itemName.GetEntry().Text))
                                //{
                                //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a duplicate Dining Table Name.", "Ok");
                                //    return;
                                //}
                            }
                            DiningTableNode.DiningTableCode = itemCode.GetEntry().Text;
                            //DiningTableNode.DiningTableName = itemName.GetEntry().Text;
                        }
                        else
                        {
                            DiningTableNode.DiningTableCode = "";
                            //DiningTableNode.DiningTableName = "";
                        }
                        DiningTableNode.IsOwnerVisible = false;


                        DiningTableNode = null;
                    }

                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save preset file
                    //JINIT_201911, PresetService.SaveList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);
                    DiningTableService.SaveList(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);
                    DiningTableService.SaveHistoryList(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Cancel":
                    if (DiningRoomNode != null)
                    {
                        DiningRoomNode.IsOwnerVisible = false;
                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsOwnerVisible = false;
                        DiningTableNode = null;
                    }

                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Save":
                    if (DiningRoomNode != null)
                    {
                        DiningRoomNode.IsOwnerVisible = false;
                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsOwnerVisible = false;
                        DiningTableNode = null;
                    }
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save preset file
                    // JINIT_201911, PresetService.SaveList(UIManager.Instance().PosModel.PresetModel.PresetGroupList);
                    DiningTableService.SaveList(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);
                    DiningTableService.SaveHistoryList(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList);

                    UIManager.Instance().InputModel.Clear();
                    break;
                case "Back":
                    if (DiningRoomNode != null)
                    {
                        DiningRoomNode.IsOwnerVisible = false;
                        DiningRoomNode = null;
                    }
                    if (DiningTableNode != null)
                    {
                        DiningTableNode.IsOwnerVisible = false;
                        DiningTableNode = null;
                    }
                    diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;

                case "AddGroup":
                    string locationTitle2 = UILocation.Instance().GetLocationText("Add?");
                    string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultAdd = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (resultAdd)
                    {
                        if (DiningRoomNode != null)
                        {
                            DiningRoomNode.IsOwnerVisible = false;
                            DiningRoomNode = null;
                        }
                        if (DiningTableNode != null)
                        {
                            DiningTableNode.IsOwnerVisible = false;
                            DiningTableNode = null;
                        }
                        DiningRoomNodeProcess.AddDiningRoom(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, 5);
                        diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
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
                        if (DiningRoomNode != null)
                        {
                            DiningRoomNode.IsOwnerVisible = false;
                            DiningRoomNode = null;
                        }
                        if (DiningTableNode != null)
                        {
                            DiningTableNode.IsOwnerVisible = false;
                            DiningTableNode = null;
                        }
                        DiningRoomNodeProcess.DeleteDiningRoom(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, 5);
                        diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
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
                        if (DiningRoomNode != null)
                        {
                            DiningRoomNode.IsOwnerVisible = false;
                            DiningRoomNode = null;
                        }
                        if (DiningTableNode != null)
                        {
                            DiningTableNode.IsOwnerVisible = false;
                            DiningTableNode = null;
                        }
                        DiningRoomNodeProcess.AddDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, 20);
                        diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "DeleteItem":
                    string locationTitle34 = UILocation.Instance().GetLocationText("Delete?");
                    string locationMessage34 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultDeleteItem = await DisplayAlert(locationTitle34, locationMessage34, "Yes", "No");
                    if (resultDeleteItem)
                    {
                        if (DiningRoomNode != null)
                        {
                            DiningRoomNode.IsOwnerVisible = false;
                            DiningRoomNode = null;
                        }
                        if (DiningTableNode != null)
                        {
                            DiningTableNode.IsOwnerVisible = false;
                            DiningTableNode = null;
                        }
                        DiningRoomNodeProcess.DeleteDiningTable(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, 20);
                        diningTableManagementPanel.DiningRoomListInvalidateSurface(UIManager.Instance().PosModel.DiningTableModel.DiningRoomList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;

                default:
                    break;
            }
        }
    }
}

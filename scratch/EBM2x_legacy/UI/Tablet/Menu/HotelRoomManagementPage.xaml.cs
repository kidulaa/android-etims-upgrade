using EBM2x.Models.HotelRoom;
using EBM2x.Process.hotel;
using EBM2x.Services;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotelRoomManagementPage : ContentPage
    {
        HotelFloorNode HotelFloorNode = null;
        HotelRoomNode HotelRoomNode = null;

        string hotelRoomNo = "";

        public HotelRoomManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGridGroup.IsVisible = false;
            fixedGridItem.IsVisible = false;

            itemCode.TitleInvalidateSurface("Name :");
            //itemName.TitleInvalidateSurface("Name :");

            groupName.TitleInvalidateSurface("Name :");
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Hotel room|Management");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Register the Room");

            hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
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

            MessagingCenter.Subscribe<Object, HotelFloorList>(this, "Hotel Floor Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(arg, true, true);
                });
            });

            MessagingCenter.Subscribe<Object, HotelRoomList>(this, "Hotel Room Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    hotelRoomManagementPanel.HotelRoomListInvalidateSurface(arg, true, true);
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, HotelFloorList>(this, "Hotel Floor Node");
            MessagingCenter.Unsubscribe<Object, HotelRoomList>(this, "Hotel Room Node");
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
                case "HotelFloor":
                    if (HotelFloorNode != null)
                    {
                        HotelFloorNode.IsOwnerVisible = false;
                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsOwnerVisible = false;
                        HotelRoomNode = null;
                    }

                    HotelFloorNode = (HotelFloorNode)((ExtEventArgs)e).EnteredObject;
                    //PresetGroupNode.IsVisible = true;
                    HotelFloorNode.IsOwnerVisible = true;
                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    fixedGridGroup.IsVisible = true;
                    fixedGridItem.IsVisible = false;

                    groupName.GetEntry().Text = HotelFloorNode.HotelFloorName;

                    UIManager.Instance().InputModel.Clear();

                    break;

                case "HotelRoom":
                    if (HotelFloorNode != null)
                    {
                        HotelFloorNode.IsOwnerVisible = false;
                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsOwnerVisible = false;
                        HotelRoomNode = null;
                    }

                    HotelRoomNode = (HotelRoomNode)((ExtEventArgs)e).EnteredObject;
                    //PresetItemNode.IsVisible = true;
                    HotelRoomNode.IsOwnerVisible = true;
                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    if (HotelRoomNode.IsVisible)
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

                    itemCode.GetEntry().Text = HotelRoomNode.HotelRoomCode;
                    // JINIT_201911, HotelRoomCode 중복체크를 위해서 원코드번호를 저장함
                    hotelRoomNo = HotelRoomNode.HotelRoomCode;
                    //itemName.GetEntry().Text = HotelRoomNode.HotelRoomName;

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Check":
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsVisible = true;
                        if (HotelRoomNode.IsVisible)
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

                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Uncheck":
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsVisible = false;
                        if (HotelRoomNode.IsVisible)
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

                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "ConfirmGroup":
                    if (HotelFloorNode != null)
                    {
                        if (HotelFloorNode.IsVisible)
                        {
                            if (string.IsNullOrEmpty(groupName.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid GroupName.", "OK");
                                return;
                            }
                            HotelFloorNode.HotelFloorName = groupName.GetEntry().Text;
                        }
                        else
                        {
                            HotelFloorNode.HotelFloorName = "";
                        }
                        HotelFloorNode.IsOwnerVisible = false;

                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsOwnerVisible = false;
                        HotelRoomNode = null;
                    }

                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save HotelRoom file
                    HotelRoomService.SaveList(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList);
                    HotelRoomService.SaveHistoryList(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "ConfirmItem":
                    if (HotelFloorNode != null)
                    {
                        HotelFloorNode.IsOwnerVisible = false;
                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        if (HotelRoomNode.IsVisible)
                        {
                            if (string.IsNullOrEmpty(itemCode.GetEntry().Text))
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid Hotel Room No.", "OK");
                                return;
                            }
                            //if (string.IsNullOrEmpty(itemName.GetEntry().Text))
                            //{
                            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter a valid Hotel Room Name.", "OK");
                            //    return;
                            //}
                            // JINIT_201911, 룸번호를 수정했으면 중복된 룸번호가 있는지 체크
                            if (hotelRoomNo != itemCode.GetEntry().Text)
                            {
                                if (HotelFloorNodeProcess.DupCheckHotelRoomNo(itemCode.GetEntry().Text))
                                {
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a duplicate Hotel Room No.", "Ok");
                                    return;
                                }
                                //if (HotelFloorNodeProcess.DupCheckHotelRoomName(itemName.GetEntry().Text))
                                //{
                                //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a duplicate Hotel Room Name.", "Ok");
                                //    return;
                                //}
                            }
                            HotelRoomNode.HotelRoomCode = itemCode.GetEntry().Text;
                            //HotelRoomNode.HotelRoomName = itemName.GetEntry().Text;
                        }
                        else
                        {
                            HotelRoomNode.HotelRoomCode = "";
                            //HotelRoomNode.HotelRoomName = "";
                        }
                        HotelRoomNode.IsOwnerVisible = false;


                        HotelRoomNode = null;
                    }

                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save HotelRoom file
                    HotelRoomService.SaveList(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList);
                    HotelRoomService.SaveHistoryList(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList);

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Cancel":
                    if (HotelFloorNode != null)
                    {
                        HotelFloorNode.IsOwnerVisible = false;
                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsOwnerVisible = false;
                        HotelRoomNode = null;
                    }

                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);

                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    UIManager.Instance().InputModel.Clear();

                    break;
                case "Save":
                    if (HotelFloorNode != null)
                    {
                        HotelFloorNode.IsOwnerVisible = false;
                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsOwnerVisible = false;
                        HotelRoomNode = null;
                    }
                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
                    fixedGridGroup.IsVisible = false;
                    fixedGridItem.IsVisible = false;

                    // save HotelRoom file
                    HotelRoomService.SaveList(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList);

                    UIManager.Instance().InputModel.Clear();
                    break;
                case "Back":
                    if (HotelFloorNode != null)
                    {
                        HotelFloorNode.IsOwnerVisible = false;
                        HotelFloorNode = null;
                    }
                    if (HotelRoomNode != null)
                    {
                        HotelRoomNode.IsOwnerVisible = false;
                        HotelRoomNode = null;
                    }
                    hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
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
                        if (HotelFloorNode != null)
                        {
                            HotelFloorNode.IsOwnerVisible = false;
                            HotelFloorNode = null;
                        }
                        if (HotelRoomNode != null)
                        {
                            HotelRoomNode.IsOwnerVisible = false;
                            HotelRoomNode = null;
                        }
                        HotelFloorNodeProcess.AddHotelFloor(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, 5);
                        hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
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
                        if (HotelFloorNode != null)
                        {
                            HotelFloorNode.IsOwnerVisible = false;
                            HotelFloorNode = null;
                        }
                        if (HotelRoomNode != null)
                        {
                            HotelRoomNode.IsOwnerVisible = false;
                            HotelRoomNode = null;
                        }
                        HotelFloorNodeProcess.DeleteHotelFloor(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, 5);
                        hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
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
                        if (HotelFloorNode != null)
                        {
                            HotelFloorNode.IsOwnerVisible = false;
                            HotelFloorNode = null;
                        }
                        if (HotelRoomNode != null)
                        {
                            HotelRoomNode.IsOwnerVisible = false;
                            HotelRoomNode = null;
                        }
                        HotelFloorNodeProcess.AddHotelRoom(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, 20);
                        hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
                        fixedGridGroup.IsVisible = false;
                        fixedGridItem.IsVisible = false;
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "DeleteItem":
                    string locationTitle24 = UILocation.Instance().GetLocationText("Delete?");
                    string locationMessage24 = UILocation.Instance().GetLocationText("Are you sure?");
                    var resultDeleteItem = await DisplayAlert(locationTitle24, locationMessage24, "Yes", "No");
                    if (resultDeleteItem)
                    {
                        if (HotelFloorNode != null)
                        {
                            HotelFloorNode.IsOwnerVisible = false;
                            HotelFloorNode = null;
                        }
                        if (HotelRoomNode != null)
                        {
                            HotelRoomNode.IsOwnerVisible = false;
                            HotelRoomNode = null;
                        }
                        HotelFloorNodeProcess.DeleteHotelRoom(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, 20);
                        hotelRoomManagementPanel.HotelFloorListInvalidateSurface(UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList, true, true);
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

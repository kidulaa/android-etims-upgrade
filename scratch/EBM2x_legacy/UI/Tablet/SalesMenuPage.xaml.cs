using EBM2x.Database;
using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.Dependency;
using EBM2x.Process.dining;
using EBM2x.Process.hotel;
using EBM2x.Process.preset;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.Login;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Phone;
using EBM2x.UI.RraSdc;
using EBM2x.UI.Setup;
using EBM2x.UI.UiUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using System.Management;
using System.Linq;

namespace EBM2x.UI.Tablet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesMenuPage : ExtContentPage
    {

        bool pageIsActive;
        public SalesMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가

            pageIsActive = true;

            Toast.Show("Sw Version");
            UIManager.SwVersionDownloadProcessRunFlag = false;
            SwVersion swVersion = new SwVersion();
            swVersion.SwVersionDownloadProcess(this);
            swVersion.TaxpayerInfoDownloadProcess(this);

            AnimationLoop();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pageIsActive = true;

            UIManager.Instance().PosModel.SetSalesTitleText("Store Menu");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a function.");

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().InputModel.Clear(); // JINIT_InputData 초기화

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Grocery Store"))
            {
                txtGroceryStore.IsVisible = true;
                btnGroceryStore.IsVisible = true;
            }
            else
            {
                txtGroceryStore.IsVisible = false;
                btnGroceryStore.IsVisible = false;
            }

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Pharmacy"))
            {
                txtPharmacy.IsVisible = true;
                btnPharmacy.IsVisible = true;
            }
            else
            {
                txtPharmacy.IsVisible = false;
                btnPharmacy.IsVisible = false;
            }

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Specialty Store"))
            {
                txtSpecialStore.IsVisible = true;
                btnSpecialStore.IsVisible = true;
            }
            else
            {
                txtSpecialStore.IsVisible = false;
                btnSpecialStore.IsVisible = false;
            }

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Restaurant"))
            {
                txtRestaurant.IsVisible = true;
                btnRestaurant.IsVisible = true;
            }
            else
            {
                txtRestaurant.IsVisible = false;
                btnRestaurant.IsVisible = false;
            }
            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Hotel"))
            {
                txtHotel.IsVisible = true;
                btnHotel.IsVisible = true;
            }
            else
            {
                txtHotel.IsVisible = false;
                btnHotel.IsVisible = false;
            }

            textWaitCount.InvalidateSurface(RraSdcJsonWriter.GetCount().ToString("#,##0"));
            textErrCount.InvalidateSurface(UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays.ToString("##0"));
            textWaitAmount.InvalidateSurface(RraSdcJsonWriter.GetTransactionSalesReceipt().ToString("#,##0"));
            textAppVersion.InvalidateSurface(RraSdcService.APPLICATION_NAME + " / " + UIManager.AppVersion);

            //=========================================================================================
            // Z-REPORT Data 생성
            ZreportMaster zreportMaster = new ZreportMaster();
            zreportMaster.UpdateZreportTable();

            //=========================================================================================

            //===================update pkg_unit_cd and qty_unit_cd==================================== added by Aime 20221101
            string valpkgUnitCd = "NT", valqtyUnitCd = "NO";
            TrnsPurchaseItemMaster trnsPurchaseItemMaster = new TrnsPurchaseItemMaster();
            TrnsSaleItemMaster trnsSaleItemMaster = new TrnsSaleItemMaster();
            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            StockIoItemMaster stockIoItemMaster = new StockIoItemMaster();
            ImportItemMaster importItemMaster = new ImportItemMaster();
            trnsPurchaseItemMaster.UpdateItemPkgQty(valpkgUnitCd,valqtyUnitCd);
            trnsSaleItemMaster.UpdateItemPkgQty(valpkgUnitCd, valqtyUnitCd);
            taxpayerItemMaster.UpdateItemPkgQty(valpkgUnitCd, valqtyUnitCd);
            stockIoItemMaster.UpdateItemPkgQty(valpkgUnitCd, valqtyUnitCd);
            importItemMaster.UpdateItemPkgQty(valpkgUnitCd, valqtyUnitCd);

            //====================end update pkg_unit_cd and qty_unit_cd===============================
            return;
        }

        public async void SelectServerTime()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //Start Serial
                    //ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                    //ManagementObject obj = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                    //Console.WriteLine((string)obj["SerialNumber"]);
                    //Console.WriteLine("iyi ni serial number");

                    //End Get Serial Number
                    string url = RraSdcService.EXTERNAL_URL + "/" + "selectServerTime";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Timeout = UIManager.TIME_OUT;
                    httpWebRequest.Method = "POST"; //POST
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(responseStream, Encoding.UTF8);

                    string jsonResponse = readerPost.ReadToEnd().Trim();
                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse.Length == 14)
                    {
                        string NowDate = DateTime.Now.ToString("yyyyMMdd");
                        string ServerDate = jsonResponse.Substring(0, 8);

                        if (NowDate != ServerDate)
                        {
                            // 시간 변경
                            string locationTitle3 = UILocation.Instance().GetLocationText("Confirm");
                            string locationMessage3 = UILocation.Instance().GetLocationText("Check the system time. [" + jsonResponse + "]");
                            var ret = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                            if (ret && UIManager.Instance().IsWindows)
                            {
                                Navigation.InsertPageBefore(new SetDateTimeMenuPage(), this);
                                await Navigation.PopAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public async void CheckWaitTransaction()
        {
          
            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays > 100)
            {
                
                DateTime dateTime = DateTime.Now;
                dateTime.AddDays(UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays * (-1));
                int count = RraSdcJsonWriter.GetTransactionOldCount(dateTime.ToString("yyyyMMdd"));
                if (count > 100)
                {
                    pageIsActive = false;
                    //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the number of failed transfers. [" + count + "]", "Ok");
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your computer was not connected to internet for more than 3 days/ please connect it to internet to be synchronized and wait for synchronization to complete.", "Ok");
                    Navigation.InsertPageBefore(new SendMenuPage(), this);
                    await Navigation.PopAsync();
                    return;
                }
            }

            if (UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineAmount > 0)
            {
                double offlineAmtWindows = 1000000000;
                double offlineTablet = 1000000000;
                double offlineMobile = 1000000000;
                double amount = RraSdcJsonWriter.GetTransactionSalesReceipt();
                if (UIManager.Instance().IsWindows || UIManager.Instance().IsMySQL)
                {
                    if (amount >= offlineAmtWindows)
                    {
                        pageIsActive = false;

                        //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Check the amount of failed transfers. [" + amount + "]", "Ok");
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your Invoices was not reported to KRA because your computer is not connected to internet. Please reconnect it and wait for synchronization to complete.", "Ok");
                        return;
                    }

                }
                else {
                    if (amount >= offlineTablet)
                    {
                        pageIsActive = false;
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Your Invoices was not reported to KRA because your Device is not connected to internet. Please reconnect it and wait for synchronization to complete.", "Ok");
                        Navigation.InsertPageBefore(new SendMenuPage(), this);
                        await Navigation.PopAsync();
                        return;
                    }
                }
               


            }

            textWaitCount.InvalidateSurface(RraSdcJsonWriter.GetCount().ToString("#,##0"));
            textErrCount.InvalidateSurface(UIManager.Instance().PosModel.Environment.EnvPosSetup.OfflineDays.ToString("##0"));
            textWaitAmount.InvalidateSurface(RraSdcJsonWriter.GetTransactionSalesReceipt().ToString("#,##0"));
        }

        protected override void OnDisappearing()
        {
            pageIsActive = false;
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(1.0));

            //if (!UIManager.Instance().IsWindows && UIManager.SwVersionDownloadProcessRunFlag)
            //{
            //    Toast.Show("Sw Version");
            //    UIManager.SwVersionDownloadProcessRunFlag = false;
            //    SwVersionDownloadProcess();
            //}

            Toast.Show("Get Server Time");
            SelectServerTime();

            Toast.Show("Check transaction");
            CheckWaitTransaction();

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(300.0)); //300.0
                if (pageIsActive)
                {
                    Toast.Show("Upload");
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            // 20210710
            if (UIManager.Instance().IsValidDevice == false)
            {
                string message = "[901] It is not valid device.\n";
                message += "[" + UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo + "],[" + UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod + "]";
                await DisplayAlert("Info", message, "Ok");
                return;
            }

            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            ItemProcess itemProcess = new ItemProcess();
            NoticeProcess noticeProcess = new NoticeProcess();

            //switch ("Restaurant")
            switch (((ExtEventArgs)e).FunctionID)
            {
                case "BackOffice":
                    UIManager.Instance().InputModel.Clear();
                    //Navigation.InsertPageBefore(new PageMain(), this);
                    Navigation.InsertPageBefore(new BackofficeLoginPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "GroceryStore":
                    UIManager.Instance().InputModel.Clear();
                    UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 10;
                    UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

                    UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 15);
                    if (UIManager.Instance().PosModel.PresetModel.PresetGroupList == null)
                    {
                        PresetGroupNodeProcess.CreatePreset(5, 15);
                        UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 15);
                    }

                    itemProcess.ItemDownloadProcess();
                    noticeProcess.NoticeDownloadProcess();

                    Navigation.InsertPageBefore(new GroceryStoreStartPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "Pharmacy":
                    UIManager.Instance().InputModel.Clear();
                    UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 10;
                    UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

                    UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 15);
                    if (UIManager.Instance().PosModel.PresetModel.PresetGroupList == null)
                    {
                        PresetGroupNodeProcess.CreatePreset(5, 15);
                        UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 15);
                    }

                    itemProcess.ItemDownloadProcess();
                    noticeProcess.NoticeDownloadProcess();

                    Navigation.InsertPageBefore(new PharmacyStartPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "SpecialtyStore":
                    UIManager.Instance().InputModel.Clear();
                    UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 5;
                    UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

                    UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 15);
                    if (UIManager.Instance().PosModel.PresetModel.PresetGroupList == null)
                    {
                        PresetGroupNodeProcess.CreatePreset(5, 15);
                        UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 15);
                    }

                    itemProcess.ItemDownloadProcess();
                    noticeProcess.NoticeDownloadProcess();

                    Navigation.InsertPageBefore(new SpecialStoreStartPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "Restaurant":
                    UIManager.Instance().InputModel.Clear();
                    UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 5;
                    UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

                    UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 25);
                    if (UIManager.Instance().PosModel.PresetModel.PresetGroupList == null)
                    {
                        PresetGroupNodeProcess.CreatePreset(5, 25);
                        UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 25);
                    }

                    UIManager.Instance().PosModel.DiningTableModel.DiningRoomList = DiningRoomNodeProcess.LoadDiningTable(5, 20);
                    if (UIManager.Instance().PosModel.DiningTableModel.DiningRoomList == null)
                    {
                        DiningRoomNodeProcess.CreateDiningTable(5, 20);
                        UIManager.Instance().PosModel.DiningTableModel.DiningRoomList = DiningRoomNodeProcess.LoadDiningTable(5, 20);
                    }

                    itemProcess.ItemDownloadProcess();
                    noticeProcess.NoticeDownloadProcess();

                    Navigation.InsertPageBefore(new RestaurantStartPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "Hotel":
                    UIManager.Instance().InputModel.Clear();
                    UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 5;
                    UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

                    UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 25);
                    if (UIManager.Instance().PosModel.PresetModel.PresetGroupList == null)
                    {
                        PresetGroupNodeProcess.CreatePreset(5, 25);
                        UIManager.Instance().PosModel.PresetModel.PresetGroupList = PresetGroupNodeProcess.LoadPreset(5, 25);
                    }

                    UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList = HotelFloorNodeProcess.LoadHotelFloor(5, 20);
                    if (UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList == null)
                    {
                        HotelFloorNodeProcess.CreateHotelFloor(5, 20);
                        UIManager.Instance().PosModel.HotelRoomModel.HotelFloorList = HotelFloorNodeProcess.LoadHotelFloor(5, 20);
                    }

                    itemProcess.ItemDownloadProcess();
                    noticeProcess.NoticeDownloadProcess();

                    Navigation.InsertPageBefore(new HotelStartPage(), this);
                    await Navigation.PopAsync();
                    break;

                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Phone":
                    string adminPwd1 = Utils.Common.getAdminPass();
                    if (UIManager.Instance().InputModel.EnteredText.Equals(adminPwd1))
                    {
                        UIManager.Instance().PosModel.TranModel.ItemListCountOfItemsToDisplayOnOnePage = 5;
                        UIManager.Instance().PosModel.TranModel.TenderListCountOfItemsToDisplayOnOnePage = 5;

                        UIManager.Instance().InputModel.Clear();
                        Navigation.InsertPageBefore(new PersonalShopStartPage(), this);
                        await Navigation.PopAsync();
                    }
                    break;
                case "Setting":
                    string adminPwd3 = Utils.Common.getAdminPass();
                    if (UIManager.Instance().InputModel.EnteredText.Equals(adminPwd3))
                    {
                        UIManager.Instance().InputModel.Clear();
                        await Navigation.PushAsync(new EBM2xDeviceSetupPage());
                    }
                    break;
                case "Admin":
                    string adminPwd2 = Utils.Common.getAdminPass();
                    if (UIManager.Instance().InputModel.EnteredText.Equals(adminPwd2))
                    {
                        UIManager.Instance().InputModel.Clear();
                        await Navigation.PushAsync(new AdminMenuPage());
                    }
                    break;
                case "RraSdcSend":
                    // 2021.06.09
                    Navigation.InsertPageBefore(new SendMenuPage(), this);
                    await Navigation.PopAsync();
                    /*
                    // 복구
                    RraSdcJsonWriter.RestoreErrorFile();

                    if (UIManager.Instance().InputModel.EnteredText.Length == 8)
                    {
                        string startDate = UIManager.Instance().InputModel.EnteredText;
                        if (startDate.Substring(0, 3).Equals("202"))
                        {
                            OnFunctionSendItem();
                            OnFunctionSendTranSales(startDate);
                            OnFunctionSendTranSalesRcpt(startDate);
                            OnFunctionSendStockIoSave(startDate);

                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Sales, SalesReceipt Json file.", "OK");
                        }
                    }

                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    if (UIManager.Instance().InputModel.EnteredText.Equals("107"))
                    {
                        //rraSdcUploadProcess.UploadProcess(true);

                        ////string startDate = "20200513";
                        ////OnUpdateClassification();
                        ////OnFunctionSendItem();
                        ////OnFunctionSendTranSales(startDate);
                        ////OnFunctionSendTranSalesRcpt(startDate);
                        ////OnFunctionSendStockIoSave(startDate);

                        ////rraSdcUploadProcess.UploadProcess(true);
                    }
                    //else if (UIManager.Instance().InputModel.EnteredText.Equals("901"))
                    //{
                    //    rraSdcUploadProcess.UploadProcess(true);
                    //}
                    //else if (UIManager.Instance().InputModel.EnteredText.Equals("999"))
                    //{
                    //    rraSdcUploadProcess.UploadClearProcess(true);
                    //}
                    else
                    {
                        rraSdcUploadProcess.UploadProcess();
                    }
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "SendTran", "This operation was called asynchronously.", "OK");

                    UIManager.Instance().InputModel.Clear();
                    */
                    break;
                case "RraSdcReceive":
                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PushAsync(new EBM2xSdcPDAManagementPage());
                    break;
                default:
                    break;
            }
        }

        private void OnFunctionSendItem()
        {
            ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
            List<ItemSaveReq> sendList = itemRraSdcUpload.getItemTable();
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Item";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_ITEM_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        private void OnFunctionSendTranSales(string startDate)
        {
            string endDate = DateTime.Now.ToString("yyyyMMdd");
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            List<TrnsSalesSaveReq> sendList = trnsSaleRraSdcUpload.getTrnsSaleTable(startDate, endDate);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Sales";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                if (sendList[i].itemList.Count > 0)
                {
                    RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
                }
            }
        }

        //판매 영수증 정보 JSON 생성
        private void OnFunctionSendTranSalesRcpt(string startDate)
        {
            string endDate = DateTime.Now.ToString("yyyyMMdd");
            TrnsSaleRcptRraSdcUpload trnsSaleRcptRraSdcUpload = new TrnsSaleRcptRraSdcUpload();
            List<TrnsSalesRcptSaveReq> sendList = trnsSaleRcptRraSdcUpload.getTrnsSaleRcptTable(startDate, endDate);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "SalesReceipt";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_RECEIPT_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        private void OnFunctionSendStockIoSave(string startDate)
        {
            string endDate = DateTime.Now.ToString("yyyyMMdd");
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            List<StockIoSaveReq> sendList = stockIoRraSdcUpload.getStockIoTable(startDate, endDate);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "StockIo";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_STOCK_IO_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        private void OnUpdateClassification()
        {
            // DELETE RRASDC ITEM : OK

            try
            {
                EBM2xDBClientProvider bBM2xDBClientProvider = EBM2xDBClientProvider.getInstance();
                if (bBM2xDBClientProvider != null)
                {
                    if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
                    {
                        string DBServer = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer;
                        string Database = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLDatabase;
                        string DBUid = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLUid;
                        string DBPwd = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLPwd;
                        bBM2xDBClientProvider.OpenConnection(DBServer, Database, DBUid, DBPwd);
                    }
                    else
                    {
                        bBM2xDBClientProvider.OpenConnection("", "", "", "");
                    }
                    //bBM2xDBClientProvider.OpenConnection();
                    using (var command = bBM2xDBClientProvider.GetDbCommand())
                    {
                        string buffer = "";
                        // UPDATE TAXPAYER_ITEM 
                        buffer = "UPDATE TAXPAYER_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                        // UPDATE TRNS_SALE_ITEM 
                        buffer = "UPDATE TRNS_SALE_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                        // UPDATE TRNS_PURCHASE_ITEM 
                        buffer = "UPDATE TRNS_PURCHASE_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                        // UPDATE STOCK_IO_ITEM 
                        buffer = "UPDATE STOCK_IO_ITEM SET ITEM_CLS_CD = ITEM_CLS_CD || '00' WHERE length(ITEM_CLS_CD) = 8;";
                        try
                        {
                            command.CommandText = buffer;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                //EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", ex.ToString(), "OK");
            }
        }
    }
}


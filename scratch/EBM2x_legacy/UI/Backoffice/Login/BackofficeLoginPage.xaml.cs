using EBM2x.Database.Master;
using EBM2x.Datafile.trlog;
using EBM2x.Process.start;
using EBM2x.RraSdc;
using EBM2x.RraSdc.model;
using EBM2x.RraSdc.process;
using EBM2x.UI.Tablet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackofficeLoginPage : ContentPage
    {
        public BackofficeLoginPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            string result = "";
            result = Initialize.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);

            //entityTINNumber.TitleInvalidateSurface();
            //entityUserID.TitleInvalidateSurface();
            //entityPassword.TitleInvalidateSurface();

            entityTINNumber.SetEntryValue(UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo);
            entityTINNumber.SetReadOnly(true);
            entityUserID.SetFocus();

            // Added By Aime
            TrnsSalesProcess trnsSalesProcess = new TrnsSalesProcess();
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionLogin(object sender, EventArgs e)
        {
            if(!entityUserID.IsValid())
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Attention", "Please Check the UserID.", "OK");
                entityUserID.SetFocus();
                return;
            }
            if (!entityPassword.IsValid())
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Attention", "Please Check the Password.", "OK");
                entityPassword.SetFocus();
                return;
            }

            if (entityUserID.GetEntryValue().ToLower() == "99999") 
            {
                string adminPwd = Utils.Common.getAdminPass();
                if (entityPassword.GetEntryValue().Equals(adminPwd))
                {
                    Navigation.InsertPageBefore(new BackofficeMenuPage(true), this);
                    await Navigation.PopAsync();
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Attention", "The administrator's password is incorrect.", "OK");
                    entityPassword.SetFocus();
                    return;
                }
            }
            else
            {
                // DB Check
                TaxpayerBhfDeviceUserRecord userRecord = new TaxpayerBhfDeviceUserRecord();
                TaxpayerBhfDeviceUserMaster userMaster = new TaxpayerBhfDeviceUserMaster();
                bool ret = userMaster.ToRecord(userRecord, UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo,
                    UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod,
                    entityUserID.GetEntryValue());
                if (ret && userRecord.UseYn.ToUpper().Equals("Y") && entityPassword.GetEntryValue().Equals(userRecord.Pwd))
                {
                    // 로인 정보 보관
                    UIManager.Instance().UserModel = userRecord;

                    ItemProcess itemProcess = new ItemProcess();
                    itemProcess.ItemDownloadProcess();
                    //DisplayAlert("ReceiveItem", "This operation was called asynchronously.", "OK");

                    StockMoveProcess stockMoveProcess = new StockMoveProcess();
                    stockMoveProcess.StockMoveDownloadProcess();
                    //DisplayAlert("ReceiveStockMove", "This operation was called asynchronously.", "OK");
                    TrnsPurchaseSalesProcess trnsPurchaseSalesProcess = new TrnsPurchaseSalesProcess();
                    trnsPurchaseSalesProcess.TrnsPurchaseSalesDownloadProcess();
                    //DisplayAlert("ReceiveTranPurchase", "This operation was called asynchronously.", "OK");
                    
                    ImportItemProcess importItemProcess = new ImportItemProcess();
                    importItemProcess.ImportItemDownloadProcess();
                    //DisplayAlert("ReceiveImportItem", "This operation was called asynchronously.", "OK");

                    //Added By  Aime for Data Sychronization
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();

                    Navigation.InsertPageBefore(new BackofficeMenuPage(), this);
                    await Navigation.PopAsync();
                }
                else if (ret && userRecord.UseYn.ToUpper().Equals("Y") && !entityPassword.GetEntryValue().Equals(userRecord.Pwd))
                {
                    // 2021.05.24
                    string adminPwd = Utils.Common.getAdminPass();
                    if (entityPassword.GetEntryValue().Equals(adminPwd))
                    {
                        Navigation.InsertPageBefore(new BackofficeMenuPage(true), this);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Attention", "The user's password is incorrect.", "OK");
                        entityPassword.SetFocus();
                    }
                }
                else if (entityUserID.GetEntryValue().ToLower() == UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo.ToLower())
                {
                    // 2021.03.15 -> 2021.05.24
                    string adminPwd = Utils.Common.getAdminPassII();
                    if (entityPassword.GetEntryValue().Equals(adminPwd))
                    {
                        Navigation.InsertPageBefore(new BackofficeMenuPage(true), this);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Attention", "The administrator's password is incorrect.", "OK");
                        entityPassword.SetFocus();
                    }
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Attention", "Please check your user ID and password.", "OK");
                    entityPassword.SetFocus();
                }
            }
        }
        private void OnFunctionSendTranSales(long curRcptNo)
        {
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            List<TrnsSalesSaveReq> sendList = trnsSaleRraSdcUpload.getNonTrnsSaleTable(curRcptNo);
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
        async void OnFunctionCancel(object sender, EventArgs e)
        {
            Navigation.InsertPageBefore(new SalesMenuPage(), this);
            await Navigation.PopAsync();
        }
    }
}

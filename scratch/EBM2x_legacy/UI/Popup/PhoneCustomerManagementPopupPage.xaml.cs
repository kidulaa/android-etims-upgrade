using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.Models.ListView;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneCustomerManagementPopupPage : ContentPage
    {
        TaxpayerBhfCustMaster master = null;
        TaxpayerBhfCustRecord record = null;

        bool NewFlag = true;

        public PhoneCustomerManagementPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            master = new TaxpayerBhfCustMaster();
            record = new TaxpayerBhfCustRecord();

            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            entityCustType.SetSelecteItem(new SystemCode() { Id = record.BcncType, Name = "" });

            entityCustName.GetEntry().MaxLength = 100;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            entityCustType.TitleInvalidateSurface();

            entityTinCode.TitleInvalidateSurface();
            entityCustName.TitleInvalidateSurface();

            entityPhone.TitleInvalidateSurface();
            entityEMail.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Customer|Management");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a Item");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void OnQueryButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();

            if (!string.IsNullOrEmpty(entityTinCode.GetEntryValue()))
            {
                string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                string bhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                string custNo = entityTinCode.GetEntryValue();

                bool ret = master.ToRecord(record, tin, bhfId, custNo);
                if (ret)
                {
                    NewFlag = false;
                    entityTinCode.SetReadOnly(!NewFlag);
                    entityCustName.SetReadOnly(!NewFlag);
                }
                else
                {
                    record = new TaxpayerBhfCustRecord();
                    NewFlag = true;
                    entityTinCode.SetReadOnly(!NewFlag);
                    entityCustName.SetReadOnly(!NewFlag);
                }

                entityCustName.SetEntryValue(record.CustNm);
                entityCustType.SetSelecteItem(new SystemCode() { Id = record.BcncType, Name = "" });
                entityPhone.SetEntryValue(record.Contact1);
                entityEMail.SetEntryValue(record.Email);
            }
            else
            {
                var popupPage = new PhoneSearchCustomerPopupPage(true);
                popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

                popupPage.OnResult += (popup, ex) => {
                    Device.BeginInvokeOnMainThread(() => {
                        SearchCustomerNode searchCustomerNode = (SearchCustomerNode)((ExtEventArgs)ex).EnteredObject;

                        string tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                        string bhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                        string custNo = searchCustomerNode.CustomerCode;

                        bool ret = master.ToRecord(record, tin, bhfId, custNo);
                        if (ret)
                        {
                            entityTinCode.SetEntryValue(record.CustTin);
                            entityCustName.SetEntryValue(record.CustNm);
                            entityCustType.SetSelecteItem(new SystemCode() { Id = record.BcncType, Name = "" });
                            entityPhone.SetEntryValue(record.Contact1);
                            entityEMail.SetEntryValue(record.Email);
                            NewFlag = false;
                            entityTinCode.SetReadOnly(!NewFlag);
                            entityCustName.SetReadOnly(!NewFlag);
                        }
                        else
                        {
                            entityTinCode.SetEntryValue(searchCustomerNode.CustomerCode);
                            entityCustName.SetEntryValue(searchCustomerNode.CustomerName);
                            NewFlag = true;
                            entityTinCode.SetReadOnly(!NewFlag);
                            entityCustName.SetReadOnly(!NewFlag);
                        }

                        Navigation.PopAsync();
                    });
                };
                popupPage.OnCanceled += (senderX, eX) => {
                    Device.BeginInvokeOnMainThread(() => {
                        UIManager.Instance().InputModel.Clear();
                        Navigation.PopAsync();
                    });
                };
                await Navigation.PushAsync(popupPage);
            }
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();

            if (string.IsNullOrEmpty(entityCustType.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the CustType.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityTinCode.GetEntryValue()) || entityTinCode.GetEntryValue().Length < 5)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the PIN.", "Ok");
                return;
            }
            if (entityTinCode.GetEntryValue().Length != 11)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the PIN [11 byte].", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityCustName.GetEntryValue()) || entityCustName.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Cust Name.", "Ok");
                return;
            }

            string locationTitle2 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage2 = UILocation.Instance().GetLocationText("Do you want to save this customer account?");
            var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
            if (result)
            {
                record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                record.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;

                record.CustNo = entityTinCode.GetEntryValue();
                record.CustTin = entityTinCode.GetEntryValue();
                record.CustNm = entityCustName.GetEntryValue();

                record.BcncType = entityCustType.GetSelectedItem().Id;

                record.ChargerNm = "";
                record.NationName = "";
                record.Contact1 = entityPhone.GetEntryValue();
                record.Contact2 = "";
                record.Email = entityEMail.GetEntryValue();
                record.Fax = "";
                record.Adrs = "";

                record.Bank = "";
                record.Account = "";
                record.Depositor = "";

                record.Rm = "";
                record.UseYn = "Y";
                record.CustGroup = "DF";

                string UserId = UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserID;
                string UserNm = UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserName;
                record.RegrId = UserId;
                record.RegrNm = UserNm;
                record.ModrId = UserId;
                record.ModrNm = UserNm;

                if (NewFlag)
                {
                    TaxpayerBhfCustRecord recordNew = new TaxpayerBhfCustRecord();
                    bool exist = master.ToRecord(recordNew, record.Tin, record.BhfId, record.CustNo);
                    if (exist)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This code already exists.", "Ok");
                        return;
                    }
                }

                bool ret = master.ToTable(record);
                if (ret)
                {
                    //===>>>>>>>>>
                    //JCNA 20191204
                    BhfCustRraSdcUpload bhfCustRraSdcUpload = new BhfCustRraSdcUpload();
                    bhfCustRraSdcUpload.SendBhfCustSave(record.Tin, record.BhfId, record.CustNo);

                    //===>>>>>>>>>
                    // JCNA 20191204 TR 전송 명령 실행
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();

                    // JCNA : CLEAR 202001
                    record = new TaxpayerBhfCustRecord();
                    entityTinCode.SetEntryValue(record.CustNo);
                    entityCustName.SetEntryValue(record.CustNm);
                    entityCustType.SetSelecteItem(new SystemCode() { Id = record.BcncType, Name = "" });
                    entityPhone.SetEntryValue(record.Contact1);
                    entityEMail.SetEntryValue(record.Email);

                    NewFlag = true;
                    entityTinCode.SetReadOnly(!NewFlag);
                    entityCustName.SetReadOnly(!NewFlag);

                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully.", "Ok");
                }
                else EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to save.", "Ok");
            }
        }
        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            NewFlag = true;
            entityTinCode.SetReadOnly(!NewFlag);
            entityCustName.SetReadOnly(!NewFlag);
            UIManager.Instance().InputModel.Clear();
        }
        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            await Navigation.PopAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        private void OnNewButtonClicked(object sender, EventArgs e)
        {
            NewFlag = true;
            entityTinCode.SetReadOnly(!NewFlag);
            entityCustName.SetReadOnly(!NewFlag);

            record = new TaxpayerBhfCustRecord();

            entityTinCode.SetEntryValue(record.CustNo);
            entityCustName.SetEntryValue(record.CustNm);
            entityCustType.SetSelecteItem(new SystemCode() { Id = record.BcncType, Name = "" });
            entityPhone.SetEntryValue(record.Contact1);
            entityEMail.SetEntryValue(record.Email);
        }
    }
}

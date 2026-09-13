using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.Models.config;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.CustomerManagement;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.SalesManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaleRegistrationPage : ContentPage
    {
        bool ModifyFlag = false;
        TransactionSalesModel SalesModel { get; set; }

        TrnsSaleItemRecord SelectedItem;
        TaxpayerBhfCustRecord CustomerRecord;

        public SaleRegistrationPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            ModifyFlag = false;
            Init();

            etUnitPrice.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etSalesQty.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etDCRate.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etTaxType.GetPicker().SelectedIndexChanged += (sender, e) => {
                UpdateItemView();
            };

            etRemark.GetEntry().MaxLength = 400;
        }
        public SaleRegistrationPage(TrnsSaleRecord recordTran, List<TrnsSaleItemRecord> listTrnsSaleItemRecord)
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            ModifyFlag = true;
            Init(recordTran, listTrnsSaleItemRecord);

            etUnitPrice.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etSalesQty.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etDCRate.GetEntry().Completed += (sender, e) => {
                UpdateItemView();
            };
            etTaxType.GetPicker().SelectedIndexChanged += (sender, e) => {
                UpdateItemView();
            };

            etRemark.GetEntry().MaxLength = 400;
        }

        public void Init()
        {
            TrnsSaleMaster TrnsSaleMaster = new TrnsSaleMaster();
            // 초기화
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            //JCNA 202001 DELETE string DvcId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblDvcId;
            long InvcNo = TrnsSaleMaster.GetSalesSeq();
            string SalesDt = DateTime.Now.ToString("yyyyMMdd");
            string SalesTyCd = "N";
            string RcptTyCd = "S";             // S : Sale, R : Refund  
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            SalesModel = new TransactionSalesModel();
            SalesModel.CurrentItemRecord = null;
            SalesModel.InitModel(Tin, BhfId, InvcNo, SalesDt, SalesTyCd, RcptTyCd, UserId, UserNm);

            UpdateHeaderView();
            UpdateItemView(new TrnsSaleItemRecord());

            etUnitPrice.SetReadOnly(true);
            etSalesQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }
        public void Init(TrnsSaleRecord recordTran, List<TrnsSaleItemRecord> listTrnsSaleItemRecord)
        {
            SalesModel = new TransactionSalesModel();
            SalesModel.TranRecord = recordTran;
            SalesModel.ItemRecords = listTrnsSaleItemRecord;

            SalesModel.CurrentItemRecord = null;

            TaxpayerBhfCustMaster CustMaster = new TaxpayerBhfCustMaster();
            CustomerRecord = new TaxpayerBhfCustRecord();
            CustMaster.ToRecord(CustomerRecord, recordTran.Tin, recordTran.CustBhfId, recordTran.CustTin);

            UpdateHeaderView();
            UpdateItemView(new TrnsSaleItemRecord());
            listView.ItemsSource = SalesModel.GetObservableCollection();

            etUnitPrice.SetReadOnly(true);
            etSalesQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderView();

            if (SalesModel.CurrentItemRecord != null)
            {
                UpdateItemView(SalesModel.CurrentItemRecord);
                etUnitPrice.SetReadOnly(false);
                etSalesQty.SetReadOnly(false);
                etTaxType.SetReadOnly(false);
                etDCRate.SetReadOnly(false);
                etSalesQty.SetFocus();
            }
            else
            {
                UpdateItemView(new TrnsSaleItemRecord());
                etUnitPrice.SetReadOnly(true);
                etSalesQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        public void UpdateHeaderView()
        {
            etSale.SetReadOnly(true);
            etInvoiceID.SetReadOnly(true);
            etCustomerID.SetReadOnly(true);
            etCustomerName.SetReadOnly(true);
            etSaleDate.SetReadOnly(true);
            etReleaseDate.SetReadOnly(true);
            etTotalAmount.SetReadOnly(true);
            etVAT.SetReadOnly(true);
            etRemark.SetReadOnly(false);

            if(SalesModel.TranRecord.RcptTyCd.Equals("S")) etSale.SetEntryValue("Sale");
            else if (SalesModel.TranRecord.RcptTyCd.Equals("R")) etSale.SetEntryValue("Refund");

            etInvoiceID.SetEntryValue(SalesModel.TranRecord.InvcNo.ToString("#,##0"));
            etCustomerID.SetEntryValue(SalesModel.TranRecord.CustTin);
            etCustomerName.SetEntryValue(SalesModel.TranRecord.CustNm);
            etSaleDate.SetEntryValue(SalesModel.TranRecord.SalesDt);
            etReleaseDate.SetEntryValue("");
            etTotalAmount.SetEntryValue(SalesModel.TranRecord.TotAmt.ToString("#,##0.00"));
            etVAT.SetEntryValue(SalesModel.TranRecord.TotTaxAmt.ToString("#,##0.00"));
            etRemark.SetEntryValue(SalesModel.TranRecord.Remark);
        }
        public void UpdateItemView(TrnsSaleItemRecord itemRecord)
        {
            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            etUnitPrice.SetReadOnly(false);
            etSalesQty.SetReadOnly(false);
            etTaxType.SetReadOnly(false);
            etVat.SetReadOnly(true);
            etDCRate.SetReadOnly(false);
            etDCAmount.SetReadOnly(true);
            etSalesPrice.SetReadOnly(true);
            etTotalPrice.SetReadOnly(true);

            etItemCode.SetEntryValue(itemRecord.ItemCd);
            etItemName.SetEntryValue(itemRecord.ItemNm);
            etClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etClassName.SetEntryValue(itemRecord.ItemClsNm);
            etUnitPrice.SetEntryValue(itemRecord.Prc);
            etSalesQty.SetEntryValue(itemRecord.Qty);
            etTaxType.SetSelecteItem(new SystemCode() { Id = itemRecord.TaxTyCd, Name = "" });
            etVat.SetEntryValue(itemRecord.TaxAmt.ToString("#,##0.00"));
            etDCRate.SetEntryValue((int)itemRecord.DcRt);
            etDCAmount.SetEntryValue(itemRecord.DcAmt.ToString("#,##0.00"));
            etSalesPrice.SetEntryValue(itemRecord.SplyAmt.ToString("#,##0.00"));
            etTotalPrice.SetEntryValue(itemRecord.TotAmt.ToString("#,##0.00"));
        }
        public void UpdateItemView()
        {
            if (SalesModel.CurrentItemRecord != null)
            {
                SalesModel.CurrentItemRecord.TaxTyCd = etTaxType.GetSelectedItem().Id;
                SalesModel.CurrentItemRecord.Prc = etUnitPrice.GetEntryValue();
                SalesModel.CurrentItemRecord.Qty = etSalesQty.GetEntryValue();
                SalesModel.CurrentItemRecord.DcRt = etDCRate.GetEntryValue();

                SalesModel.CurrentItemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                SalesModel.CurrentItemRecord.ModrId = UIManager.Instance().UserModel.UserId;
                SalesModel.CurrentItemRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
                SalesModel.CurrentItemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                SalesModel.CurrentItemRecord.RegrId = UIManager.Instance().UserModel.UserId;
                SalesModel.CurrentItemRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

                SalesModel.CalculateCurrentItem();

                etVat.SetEntryValue(SalesModel.CurrentItemRecord.TaxAmt.ToString("#,##0.00"));
                etDCAmount.SetEntryValue(SalesModel.CurrentItemRecord.DcAmt.ToString("#,##0.00"));
                etSalesPrice.SetEntryValue(SalesModel.CurrentItemRecord.SplyAmt.ToString("#,##0.00"));
                etTotalPrice.SetEntryValue(SalesModel.CurrentItemRecord.TotAmt.ToString("#,##0.00"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionPrintReceipt(object sender, EventArgs e)
        {
            //=======================================================
            // JINIT_20191208, Check항목 추가
            //=======================================================
            // 거래처가 선택되어있지 않으면 오류
            if (string.IsNullOrEmpty(etCustomerID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Customer.", "OK");
                return;
            }
            // 등록된 ITEM이 없으면 오류
            if (SalesModel.GetItemCount() == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please confirm a Item.", "OK");
                return;
            }
            //=======================================================
            // PRINTING
            var popupPage = new PrintReceiptPage(SalesModel, false);
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    string paymentName = (string)((ExtEventArgs)ex).EnteredObject;
                    Navigation.PopAsync();
                });
            };
            popupPage.OnCanceled += (senderx, ex) => {
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };

            // Navigate to our scanner page
            await Navigation.PushAsync(popupPage);
        }

        async void OnFunctionSave(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(etCustomerID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Customer.", "OK");
                return;
            }
            if (SalesModel.GetItemCount() == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please confirm a Item.", "OK");
                return;
            }


            //Added By Bright 4.4.2022 start
            //Check Date Time of the last invoice

            TrnsSaleReceiptMaster receiptMaster = new TrnsSaleReceiptMaster();
            string LAST_INVC_TIME = receiptMaster.GetLastReceiptSeqTime();
            int result1 = 0;

            if (!LAST_INVC_TIME.Equals(""))
            {
                result1 = DateTime.Compare(DateTime.ParseExact(LAST_INVC_TIME, "yyyyMMddHHmmss", null), DateTime.Now);
            }
            
            
           
            if (result1 > 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please check the system time", "OK");
                return;
            }
    
            //Added By Bright 4.4.2022 End

            //=======================================================

            SalesModel.TranRecord.Remark = etRemark.GetEntryValue();

            SalesModel.TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.ModrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.ModrNm = UIManager.Instance().UserModel.UserNm;
            SalesModel.TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            SalesModel.TranRecord.RegrId = UIManager.Instance().UserModel.UserId;
            SalesModel.TranRecord.RegrNm = UIManager.Instance().UserModel.UserNm;

            TrnsSaleMaster TrnsSaleMaster = new TrnsSaleMaster();
            TrnsSaleItemMaster TrnsSaleItemMaster = new TrnsSaleItemMaster();

            if (!ModifyFlag)
            {
                SalesModel.TranRecord.TaxprNm = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNm;
                SalesModel.TranRecord.SalesDt = DateTime.Now.ToString("yyyyMMdd");
                SalesModel.TranRecord.SalesTyCd = "N";
                SalesModel.TranRecord.RcptTyCd = "S";
            }

            if(ModifyFlag)
            {
                TrnsSaleMaster.DeleteTable(SalesModel.TranRecord);
                TrnsSaleItemMaster.DeleteTable(SalesModel.TranRecord);
            }

            //  Header, Items
            TrnsSaleMaster.InsertTable(SalesModel.TranRecord);

            // JINIT_20191208,  SalesModel.TranRecord 
            //TrnsSaleItemMaster.InsertTable(SalesModel.ItemRecords);
            TrnsSaleItemMaster.InsertTable(SalesModel.TranRecord, SalesModel.ItemRecords);

            //Commented By Bright on 24.4.2022
           //Trnsale File has to be generated after invoice number is approved
           //This was done to fix the issue caused by Modify Button


            //===>>>>>>>>>
            //JCNA 20191204
            
            /**
             * 
            TrnsSaleRraSdcUpload trnsSaleRraSdcUpload = new TrnsSaleRraSdcUpload();
            trnsSaleRraSdcUpload.SendTranSalesSave(SalesModel.TranRecord.Tin, SalesModel.TranRecord.BhfId, SalesModel.TranRecord.InvcNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();
           
             
             **/
            //End of Comment

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            if (OnResult != null) OnResult?.Invoke(this, new EventArgs());
        }

        async void OnFunctionClose(object sender, EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnFunctionConfirm(object sender, System.EventArgs e)
        {
            if (SalesModel.CurrentItemRecord != null)
            {
                if(etUnitPrice.GetEntryValue() > 99999999999)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid UnitPrice.", "OK");
                    return;
                }
                if (etSalesQty.GetEntryValue() > 999999)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid SalesQty.", "OK");
                    return;
                }
                if ((etUnitPrice.GetEntryValue() * etSalesQty.GetEntryValue()) > 99999999999)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid UnitPrice or SalesQty.", "OK");
                    return;
                }
                if ((etUnitPrice.GetEntryValue() * etSalesQty.GetEntryValue()) > 10000000)
                {
                    string locationTitle21 = UILocation.Instance().GetLocationText("Info");
                    string locationMessage21 = UILocation.Instance().GetLocationText("The amount exceeds 10,000,000.00. Would you like to register?");
                    var checkAmt = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
                    if(!checkAmt) return;
                }

                SalesModel.CurrentItemRecord.TaxTyCd = etTaxType.GetSelectedItem().Id;
                SalesModel.CurrentItemRecord.Prc = etUnitPrice.GetEntryValue();
                SalesModel.CurrentItemRecord.Qty = etSalesQty.GetEntryValue();
                SalesModel.CurrentItemRecord.DcRt = etDCRate.GetEntryValue();

                if ((etUnitPrice.GetEntryValue() * etSalesQty.GetEntryValue()) != 0)
                {
                    SalesModel.CalculateCurrentItem();
                    SalesModel.ConfirmCurrentItem();

                    listView.ItemsSource = SalesModel.GetObservableCollection();

                    UpdateHeaderView();
                    UpdateItemView(new TrnsSaleItemRecord());

                    etUnitPrice.SetReadOnly(true);
                    etSalesQty.SetReadOnly(true);
                    etTaxType.SetReadOnly(true);
                    etDCRate.SetReadOnly(true);
                }
                else
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
                }
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a product.", "OK");
            }
        }

        private void OnFunctionClear(object sender, EventArgs e)
        {
            SalesModel.CurrentItemRecord = null;

            UpdateItemView(new TrnsSaleItemRecord());
            etUnitPrice.SetReadOnly(true);
            etSalesQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = (TrnsSaleItemRecord)e.SelectedItem;
            }
            else
            {
                SelectedItem = null;
            }
        }
        private void OnRemove(object sender, System.EventArgs e)
        {
            if (SelectedItem != null)
            {
                SalesModel.Delete(SelectedItem);
                SalesModel.CalculateTran();
                listView.ItemsSource = SalesModel.GetObservableCollection();

                UpdateHeaderView();
                UpdateItemView(new TrnsSaleItemRecord());
                etUnitPrice.SetReadOnly(true);
                etSalesQty.SetReadOnly(true);
                etTaxType.SetReadOnly(true);
                etDCRate.SetReadOnly(true);
            }
        }

        private void OnEmpty(object sender, System.EventArgs e)
        {
            SalesModel.DeleteAll();
            SalesModel.CalculateTran();
            listView.ItemsSource = SalesModel.GetObservableCollection();

            UpdateHeaderView();
            UpdateItemView(new TrnsSaleItemRecord());
            etUnitPrice.SetReadOnly(true);
            etSalesQty.SetReadOnly(true);
            etTaxType.SetReadOnly(true);
            etDCRate.SetReadOnly(true);
        }

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            if(CustomerRecord == null || string.IsNullOrEmpty(etCustomerID.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Customer.", "OK");
                return;
            }

            var popupPage = new ItemPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;

                    if (SalesModel.IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {

                        if (CustomerRecord != null)
                        {
                            if (CustomerRecord.CustGroup.Equals("L1"))
                            {
                                if (popupRecord.GrpPrcL1 != 0)
                                {
                                    popupRecord.DftPrc = popupRecord.GrpPrcL1;
                                }
                            }
                            else if (CustomerRecord.CustGroup.Equals("L2"))
                            {
                                if (popupRecord.GrpPrcL2 != 0)
                                {
                                    popupRecord.DftPrc = popupRecord.GrpPrcL2;
                                }
                            }
                            else if (CustomerRecord.CustGroup.Equals("L3"))
                            {
                                if (popupRecord.GrpPrcL3 != 0)
                                {
                                    popupRecord.DftPrc = popupRecord.GrpPrcL3;
                                }
                            }
                            else if (CustomerRecord.CustGroup.Equals("L4"))
                            {
                                if (popupRecord.GrpPrcL4 != 0)
                                {
                                    popupRecord.DftPrc = popupRecord.GrpPrcL4;
                                }
                            }
                            else if (CustomerRecord.CustGroup.Equals("L5"))
                            {
                                if (popupRecord.GrpPrcL5 != 0)
                                {
                                    popupRecord.DftPrc = popupRecord.GrpPrcL5;
                                }
                            }
                        }

                        SalesModel.SetCurrentItem(popupRecord);
                        UpdateItemView(SalesModel.CurrentItemRecord);

                        string AuthCd = UIManager.Instance().UserModel.AuthCd;
                        if (UIManager.Instance().UserModel.RoleCd != "1")
                        {
                            if(!AuthCd.Contains("PRICE;") && popupRecord.DftPrc > 0)
                            {
                                etUnitPrice.SetReadOnly(true);
                            }
                            else
                            {
                                etUnitPrice.SetReadOnly(false);
                            }
                        }
                        etSalesQty.SetReadOnly(false);
                        etTaxType.SetReadOnly(false);
                        etDCRate.SetReadOnly(false);
                    }

                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

        async void OnFunctionCustomerID(object sender, EventArgs e)
        {
            var popupPage = new CustomerPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    CustomerRecord = (TaxpayerBhfCustRecord)((ExtEventArgs)ex).EnteredObject;
                    //JCNA 202001 DELETESalesModel.TranRecord.CustNo = CustomerRecord.CustNo;
                    SalesModel.TranRecord.CustTin = CustomerRecord.CustTin;
                    SalesModel.TranRecord.CustNm = CustomerRecord.CustNm;
                    SalesModel.TranRecord.CustBhfId = "00";
                    UpdateHeaderView();

                    Navigation.PopAsync();
                });
            };

            popupPage.OnCanceled += (senderx, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
            };
            await Navigation.PushAsync(popupPage);
        }

    }
}

using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
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
    public partial class PhoneItemManagementPopupPage : ContentPage
    {
        TaxpayerItemMaster master = null;
        TaxpayerItemRecord record = null;

        bool NewFlag = true;

        public PhoneItemManagementPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();

            master = new TaxpayerItemMaster();
            record = new TaxpayerItemRecord();
            
            entityItemName.GetEntry().MaxLength = 200;

            SetData(record, true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            entityBarcode.TitleInvalidateSurface();

            entityItemCode.TitleInvalidateSurface();
            entityItemName.TitleInvalidateSurface();

            entitySalePrice.TitleInvalidateSurface();
            entityCurrentStock.TitleInvalidateSurface();

            entityClassCode.TitleInvalidateSurface();
            entityClassName.TitleInvalidateSurface();
            entityClassCode.SetReadOnly(true);
            entityClassName.SetReadOnly(true);

            entityOrigin.TitleInvalidateSurface();
            entityOrigin.SetReadOnly(true);

            entityPkgUnit.TitleInvalidateSurface();
            entityItemType.TitleInvalidateSurface();

            entityQtyUnit.TitleInvalidateSurface();
            entityTaxType.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Item|Management");
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
            if (!string.IsNullOrEmpty(entityItemCode.GetEntryValue()))
            {
                EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
                LoadItemInfor(record, envPosSetup.GblTaxIdNo, entityItemCode.GetEntryValue());
            }
            else
            {
                var popupPage = new PhoneSearchItemPopupPage();
                popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

                popupPage.OnResult += (popup, ex) => {
                    Device.BeginInvokeOnMainThread(() => {
                        SearchItemNode searchItemNode = (SearchItemNode)((ExtEventArgs)ex).EnteredObject;
                        EnvPosSetup envPosSetup = UIManager.Instance().PosModel.Environment.EnvPosSetup;
                        LoadItemInfor(record, envPosSetup.GblTaxIdNo, searchItemNode.ItemCode);
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

        async void LoadItemInfor(TaxpayerItemRecord record, string tin, string itemCode)
        {
            bool ret = master.ToRecord(record, tin, itemCode, false);
            if (!ret)
            {
                // JINIT_201911, 데이터Clear추가
                record = new TaxpayerItemRecord();
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Warning?", "There is no data.", "Ok");

                SetData(record, true);
            }
            else
            {
                SetData(record, false);
                NewFlag = false;
            }
        }

        public void SetData(TaxpayerItemRecord record, bool newFlag)
        { 
            entityBarcode.SetEntryValue(record.Bcd);

            entityItemCode.SetEntryValue(record.ItemCd);
            entityItemCode.SetReadOnly(!newFlag);
            entityItemCodeIcon.IsVisible = newFlag;
            entityItemName.SetEntryValue(record.ItemNm);

            entitySalePrice.SetEntryValue(record.DftPrc);
            entityCurrentStock.SetEntryValue(record.InitlQty);
            entityCurrentStock.SetReadOnly(!newFlag);

            entityClassCode.SetEntryValue(record.ItemClsCd);
            entityClassCodeIcon.IsVisible = newFlag;
            entityClassName.SetEntryValue(record.ItemClsName);

            entityOrigin.SetEntryValue(record.OrgnNatName);
            entityOriginIcon.IsVisible = newFlag;

            entityPkgUnit.SetSelecteItem(new SystemCode() { Id = record.PkgUnitCd, Name = "" });
            entityPkgUnit.SetReadOnly(!newFlag);
            entityItemType.SetSelecteItem(new SystemCode() { Id = record.ItemTyCd, Name = "" });
            entityItemType.SetReadOnly(!newFlag);

            entityQtyUnit.SetSelecteItem(new SystemCode() { Id = record.QtyUnitCd, Name = "" });
            entityQtyUnit.SetReadOnly(!newFlag);
            entityTaxType.SetSelecteItem(new SystemCode() { Id = record.TaxTyCd, Name = "" });
            entityTaxType.SetReadOnly(!newFlag);

        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            if (string.IsNullOrEmpty(entityItemCode.GetEntryValue()) || entityItemCode.GetEntryValue().Length < 5)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Item code..", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(record.OrgnNatCd))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "OrgnNatCd is Empty.", "OK");
                return;
            }

            if (!record.OrgnNatCd.Equals(entityItemCode.GetEntryValue().Substring(0, 2)))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "OrgnNatCd is wrong.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(entityClassCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Class code..", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityItemName.GetEntryValue()) || entityItemName.GetEntryValue().Length < 3)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Please enter the Item name.", "Ok");
                return;
            }

            if (record == null)
            {
                record = new TaxpayerItemRecord();
            }

            if (string.IsNullOrEmpty(entityItemType.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the item type.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityPkgUnit.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the pkg unit.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityTaxType.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the tax type.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(entityQtyUnit.GetSelectedItem().Id))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving item. Please select the Qty Unit.", "Ok");
                return;
            }

            string locationTitle2 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage2 = UILocation.Instance().GetLocationText("Do you want to save this Item?");
            var result = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
            if (!result) return;

            record.Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            record.UseAdiYn = "Y";
            record.Bcd = entityBarcode.GetEntryValue();

            record.ItemCd = entityItemCode.GetEntryValue();
            record.ItemNm = entityItemName.GetEntryValue();

            record.DftPrc = entitySalePrice.GetEntryValue();
            record.InitlQty = entityCurrentStock.GetEntryValue();
            record.InitlWhUntpc = 0;

            record.ItemClsCd = entityClassCode.GetEntryValue();
            record.ItemClsName = entityClassName.GetEntryValue();

            record.PkgUnitCd = entityPkgUnit.GetSelectedItem().Id;
            record.ItemTyCd = entityItemType.GetSelectedItem().Id;

            record.QtyUnitCd = entityQtyUnit.GetSelectedItem().Id;
            record.TaxTyCd = entityTaxType.GetSelectedItem().Id;

            string UserId = UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserID;
            string UserNm = UIManager.Instance().PosModel.RegiTotal.RegiHeader.UserName;
            record.RegrId = UserId;
            record.RegrNm = UserNm;
            record.ModrId = UserId;
            record.ModrNm = UserNm;

            StockMasterRecord stockRecord = new StockMasterRecord();
            stockRecord.Tin = record.Tin;
            stockRecord.BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            stockRecord.ItemCd = record.ItemCd;
            stockRecord.RsdQty = 0;
            // 저장 이력
            stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.ModrId = UserId;
            stockRecord.ModrNm = UserNm;
            stockRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            stockRecord.RegrId = UserId;
            stockRecord.RegrNm = UserNm;

            bool IsExist = master.GetItemCode(record.Tin, record.ItemCd);

            if (NewFlag)
            {
                if (IsExist)
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "This code already exists.", "Ok");
                    return;
                }
            }

            bool ret = master.ToTable(record, stockRecord);
            if (ret)
            {
                //============ 20191210 JCNA
                if (!IsExist)
                {
                    StockIoMaster StockIoMaster = new StockIoMaster();
                    StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();
                    // 초기화
                    string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                    string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                    long SarNo = StockIoMaster.GetStockIoSeq();
                    string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
                    string RegTyCd = "A"; // 입력유형 (A:자동, M:수기입력)

                    TransactionStockInOutModel StockInOutModel = new TransactionStockInOutModel();
                    StockInOutModel.CurrentItemRecord = null;
                    StockInOutModel.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
                    StockInOutModel.SetCurrentItem(record);
                    StockInOutModel.CurrentItemRecord.Qty = record.InitlQty;
                    StockInOutModel.CurrentItemRecord.RegrId = UserId;
                    StockInOutModel.CurrentItemRecord.RegrNm = UserNm;
                    StockInOutModel.CurrentItemRecord.ModrId = UserId;
                    StockInOutModel.CurrentItemRecord.ModrNm = UserNm;

                    StockInOutModel.ConfirmCurrentItem();
                    StockInOutModel.TranRecord.SarTyCd = "06"; // 재고유형 - 06 : 재고조정입고
                    StockInOutModel.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();

                    StockIoMaster.InsertTable(StockInOutModel.TranRecord);
                    StockIoItemMaster.InsertTable(1, StockInOutModel.ItemRecords, StockInOutModel.TranRecord.SarNo);
                }

                //===>>>>>>>>>
                //JCNA 20191204
                ItemRraSdcUpload itemRraSdcUpload = new ItemRraSdcUpload();
                itemRraSdcUpload.SendItemSave(record.Tin, record.ItemCd);

                //===>>>>>>>>>
                // JCNA 20191204 TR 전송 명령 실행
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();

                // JCNA : CLEAR 202001
                SetData(new TaxpayerItemRecord(), true);
                NewFlag = true;

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Saved successfully.", "Ok");
            }
            else EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Failed to save.", "Ok");
        }
        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            NewFlag = true;
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

        async void OnFunctionQueryClassCode(object sender, EventArgs e)
        {
            var popupPage = new PhoneSearchClassPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    ItemClassRecord popupRecord = (ItemClassRecord)((ExtEventArgs)ex).EnteredObject;

                    entityClassCode.SetEntryValue(popupRecord.ItemClsCd);
                    entityClassName.SetEntryValue(popupRecord.ItemClsNm);
                    entityTaxType.SetSelecteItem(new SystemCode() { Id = popupRecord.TaxTyCd, Name = "" });

                    entityItemType.SetSelecteItem(new SystemCode() { Id = popupRecord.TaxTyCd, Name = "" });

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

        async void OnFunctionQueryOriginCode(object sender, EventArgs e)
        {
            var popupPage = new PhoneSearchOriginPopupPage();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    CodeDtlRecord popupRecord = (CodeDtlRecord)((ExtEventArgs)ex).EnteredObject;
                    record.OrgnNatCd = popupRecord.Cd;
                    entityOrigin.SetEntryValue(popupRecord.CdNm);
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
        async void OnFunctionNumbering(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entityItemCode.GetEntryValue()))
            {
                if (string.IsNullOrEmpty(record.OrgnNatCd))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "OrgnNatCd is Empty.", "OK");
                    return;
                }

                if (entityItemType.GetSelectedItem() == null || string.IsNullOrEmpty(entityItemType.GetSelectedItem().Id))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "ItemType is Empty.", "OK");
                    return;
                }
                record.ItemTyCd = entityItemType.GetSelectedItem().Id;
                if (entityPkgUnit.GetSelectedItem() == null || string.IsNullOrEmpty(entityPkgUnit.GetSelectedItem().Id))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "PkgUnit is Empty.", "OK");
                    return;
                }
                record.PkgUnitCd = entityPkgUnit.GetSelectedItem().Id;
                if (entityQtyUnit.GetSelectedItem() == null || string.IsNullOrEmpty(entityQtyUnit.GetSelectedItem().Id))
                {
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "QtyUnit is Empty.", "OK");
                    return;
                }
                record.QtyUnitCd = entityQtyUnit.GetSelectedItem().Id;

                string strItemCode = master.GenItemCode(record, true);
                entityItemCode.SetEntryValue(strItemCode);
            }
        }

        private void OnNewButtonClicked(object sender, EventArgs e)
        {
            NewFlag = true;

            SetData(new TaxpayerItemRecord(), true);
        }
    }
}

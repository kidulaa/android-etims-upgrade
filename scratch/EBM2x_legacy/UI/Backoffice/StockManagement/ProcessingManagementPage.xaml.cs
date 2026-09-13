using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessingManagementPage : ContentPage
    {
        // PROCESS
        TransactionStockInOutModel StockInOutModelRaw { get; set; }
        TransactionStockInOutModel StockInOutModelFinished { get; set; }
        StockIoItemRecord SelectedItemFinished { get; set; }
        StockIoItemRecord SelectedItemRaw { get; set; }

        public ProcessingManagementPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            Init();
        }
        public void Init()
        {
            StockIoMaster StockIoMaster = new StockIoMaster();
            // 초기화
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            long SarNo = StockIoMaster.GetStockIoSeq();
            string OcrnDt = DateTime.Now.ToString("yyyyMMdd");
            string RegTyCd = "M"; // 입력유형 (A:자동, M:수기입력)
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            StockInOutModelRaw = new TransactionStockInOutModel();
            StockInOutModelRaw.CurrentItemRecord = null;
            StockInOutModelRaw.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
            StockInOutModelRaw.TranRecord.SarTyCd = "14"; // 재고유형 - 14 : 재가공출고

            StockInOutModelFinished = new TransactionStockInOutModel();
            StockInOutModelFinished.CurrentItemRecord = null;
            StockInOutModelFinished.InitModel(Tin, BhfId, SarNo, OcrnDt, RegTyCd, UserId, UserNm);
            StockInOutModelFinished.TranRecord.SarTyCd = "05"; // 재고유형 - 05 : 재가공입고

            UpdateHeaderViewRaw();
            UpdateItemViewRaw(new StockIoItemRecord());

            UpdateHeaderViewFinished();
            UpdateItemViewFinished(new StockIoItemRecord());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateHeaderViewRaw();
            if (StockInOutModelRaw.CurrentItemRecord != null)
            {
                UpdateItemViewRaw(StockInOutModelRaw.CurrentItemRecord);
                etRawQuantity.SetReadOnly(false);
                etRawQuantity.SetFocus();
            }
            else
            {
                UpdateItemViewRaw(new StockIoItemRecord());
            }

            UpdateHeaderViewFinished();
            if (StockInOutModelFinished.CurrentItemRecord != null)
            {
                UpdateItemViewFinished(StockInOutModelFinished.CurrentItemRecord);
                etFinishedQuantity.SetReadOnly(false);
                etFinishedQuantity.SetFocus();
            }
            else
            {
                UpdateItemViewFinished(new StockIoItemRecord());
            }
        }

        public void UpdateHeaderViewRaw()
        {
        }
        public void UpdateItemViewRaw(StockIoItemRecord itemRecord)
        {
            etRawItemCode.SetEntryValue(itemRecord.ItemCd);
            etRawItemName.SetEntryValue(itemRecord.ItemNm);
            etRawClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etRawClassName.SetEntryValue(itemRecord.ItemClsNm);
            etRawCurrentStock.SetEntryValue(itemRecord.RdsQty.ToString());
            etRawQuantity.SetEntryValue(itemRecord.Qty);

            etRawItemCode.SetReadOnly(true);
            etRawItemName.SetReadOnly(true);
            etRawClassCode.SetReadOnly(true);
            etRawClassName.SetReadOnly(true);
            etRawCurrentStock.SetReadOnly(true);
            etRawQuantity.SetReadOnly(true);
        }
        public void UpdateHeaderViewFinished()
        {
        }
        public void UpdateItemViewFinished(StockIoItemRecord itemRecord)
        {
            etFinishedItemCode.SetEntryValue(itemRecord.ItemCd);
            etFinishedItemName.SetEntryValue(itemRecord.ItemNm);
            etFinishedClassCode.SetEntryValue(itemRecord.ItemClsCd);
            etFinishedClassName.SetEntryValue(itemRecord.ItemClsNm);
            etFinishedCurrentStock.SetEntryValue(itemRecord.RdsQty.ToString());
            etFinishedQuantity.SetEntryValue(itemRecord.Qty);

            etFinishedItemCode.SetReadOnly(true);
            etFinishedItemName.SetReadOnly(true);
            etFinishedClassCode.SetReadOnly(true);
            etFinishedClassName.SetReadOnly(true);
            etFinishedCurrentStock.SetReadOnly(true);
            etFinishedQuantity.SetReadOnly(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionConfirm(object sender, System.EventArgs e)
        {
            //@ychan_20191208 Raw, Finished 상품 등록건수가 없으면 실행안됨
            if(StockInOutModelRaw.ItemRecords.Count <= 0 || StockInOutModelFinished.ItemRecords.Count <= 0)
            {
                return;
            }

            StockIoMaster StockIoMaster = new StockIoMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            // Raw
            // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
            StockInOutModelRaw.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
            StockIoMaster.InsertTable(StockInOutModelRaw.TranRecord);
            StockIoItemMaster.InsertTable(-1, StockInOutModelRaw.ItemRecords, StockInOutModelRaw.TranRecord.SarNo);

            // Finished
            // 재고 IN/OUT 반영 : STOCK_IO, STOCK_IO_ITEM
            StockInOutModelFinished.TranRecord.SarNo = StockIoMaster.GetStockIoSeq();
            StockIoMaster.InsertTable(StockInOutModelFinished.TranRecord);
            StockIoItemMaster.InsertTable(1, StockInOutModelFinished.ItemRecords, StockInOutModelFinished.TranRecord.SarNo);

            //===>>>>>>>>>
            //JCNA 20191204
            StockIoRraSdcUpload stockIoRraSdcUpload = new StockIoRraSdcUpload();
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModelRaw.TranRecord.Tin, StockInOutModelRaw.TranRecord.BhfId, StockInOutModelRaw.TranRecord.SarNo);
            stockIoRraSdcUpload.SendStockIoSave(StockInOutModelFinished.TranRecord.Tin, StockInOutModelFinished.TranRecord.BhfId, StockInOutModelFinished.TranRecord.SarNo);

            //===>>>>>>>>>
            // JCNA 20191204 TR 전송 명령 실행
            RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
            rraSdcUploadProcess.UploadProcess();

            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            await Navigation.PopAsync();
        }

        async void OnFunctionClose(object sender, System.EventArgs e)
        {
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            await Navigation.PopAsync();
        }

        async void OnFunctionRawConfirm(object sender, System.EventArgs e)
        {
            if (StockInOutModelRaw.CurrentItemRecord != null)
            {
                //etQuantity.GetEntry().C // Entry 입력 종료처리 필요.
                if (etRawQuantity.GetEntryValue() != 0)
                {
                    if (StockInOutModelRaw.CurrentItemRecord.RdsQty - etRawQuantity.GetEntryValue() < 0)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Processing Quantity is greater than current stock qty. Please Check.", "OK");
                    }
                    else
                    {
                        StockInOutModelRaw.CurrentItemRecord.Qty = etRawQuantity.GetEntryValue();
                        StockInOutModelRaw.CurrentItemRecord.AfterQty = StockInOutModelRaw.CurrentItemRecord.RdsQty - StockInOutModelRaw.CurrentItemRecord.Qty;
                        StockInOutModelRaw.CalculateCurrentItem();
                        StockInOutModelRaw.ConfirmCurrentItem();
                        listViewRaw.ItemsSource = StockInOutModelRaw.GetObservableCollection();

                        UpdateHeaderViewRaw();
                        UpdateItemViewRaw(new StockIoItemRecord());
                    }
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
        async void OnFunctionFinishedConfirm(object sender, System.EventArgs e)
        {
            if (StockInOutModelFinished.CurrentItemRecord != null)
            {
                //etQuantity.GetEntry().C // Entry 입력 종료처리 필요.
                if (etFinishedQuantity.GetEntryValue() != 0)
                {
                    StockInOutModelFinished.CurrentItemRecord.Qty = etFinishedQuantity.GetEntryValue();
                    StockInOutModelFinished.CurrentItemRecord.AfterQty = StockInOutModelFinished.CurrentItemRecord.RdsQty + StockInOutModelFinished.CurrentItemRecord.Qty;
                    StockInOutModelFinished.CalculateCurrentItem();
                    StockInOutModelFinished.ConfirmCurrentItem();
                    listViewFinished.ItemsSource = StockInOutModelFinished.GetObservableCollection();

                    UpdateHeaderViewFinished();
                    UpdateItemViewFinished(new StockIoItemRecord());
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
        private void OnRawRemove(object sender, System.EventArgs e)
        {
            if (SelectedItemRaw != null)
            {
                StockInOutModelRaw.Delete(SelectedItemRaw);
                listViewRaw.ItemsSource = StockInOutModelRaw.GetObservableCollection();
            }  

            //StockInOutModelRaw.Delete((StockIoItemRecord)listViewRaw.SelectedItem);
            //listViewRaw.ItemsSource = StockInOutModelRaw.GetObservableCollection();

        }
        private void OnFinishedRemove(object sender, System.EventArgs e)
        {
            if (SelectedItemFinished != null)
            {
                StockInOutModelFinished.Delete(SelectedItemFinished);
                listViewFinished.ItemsSource = StockInOutModelFinished.GetObservableCollection();
            }
        }


        private void OnRawEmpty(object sender, System.EventArgs e)
        {
            StockInOutModelRaw.DeleteAll();
            listViewRaw.ItemsSource = StockInOutModelRaw.GetObservableCollection();
        }
        private void OnFinishedEmpty(object sender, System.EventArgs e)
        {
            StockInOutModelFinished.DeleteAll();
            listViewFinished.ItemsSource = StockInOutModelFinished.GetObservableCollection();
        }

        async void OnFunctionRawItemCode(object sender, System.EventArgs e)
        {
            var popupPage = new ItemPopupPage("Type-RawItem");
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;
                    if (StockInOutModelRaw.IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {

                        StockInOutModelRaw.SetCurrentItem(popupRecord);
                        UpdateItemViewRaw(StockInOutModelRaw.CurrentItemRecord);
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
        async void OnFunctionFinishedItemCode(object sender, System.EventArgs e)
        {
            ////@ychan_20191208 Finished Product는 1개씩만 저장 가능하게 수정
            //if (StockInOutModelFinished.ItemRecords.Count != 0)
            //    return;

            var popupPage = new ItemPopupPage("Type-FinishedItem");
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;

                    if (StockInOutModelFinished.IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {
                        StockInOutModelFinished.SetCurrentItem(popupRecord);
                        UpdateItemViewFinished(StockInOutModelFinished.CurrentItemRecord);
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

        async void OnFinishedRawMaterial(object sender, EventArgs e)
        {
            StockInOutModelRaw.DeleteAll();
            listViewRaw.ItemsSource = StockInOutModelFinished.GetObservableCollection();

            StockInOutModelFinished.DeleteAll();
            listViewFinished.ItemsSource = StockInOutModelFinished.GetObservableCollection();

            var popupPage = new ProcessingItemPopup();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    List<TaxpayerItemCompositionRecord> listItemComposition = (List<TaxpayerItemCompositionRecord>)((ExtEventArgs)ex).EnteredObject;
                    if (listItemComposition != null && listItemComposition.Count > 0)
                    {
                        TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
                        long qty = popupPage.GetQty;
                        for (int i = 0; i < listItemComposition.Count; i++)
                        {
                            TaxpayerItemCompositionRecord taxpayerItemCompositionRecord = listItemComposition[i];
                            //Raw
                            TaxpayerItemRecord taxpayerItemRecord = new TaxpayerItemRecord();
                            taxpayerItemMaster.ToRecord(taxpayerItemRecord, taxpayerItemCompositionRecord.Tin, taxpayerItemCompositionRecord.CpstItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                            StockInOutModelRaw.SetCurrentItem(taxpayerItemRecord);
                            StockInOutModelRaw.CurrentItemRecord.Qty = (taxpayerItemCompositionRecord.CpstQty * qty);
                            StockInOutModelRaw.CurrentItemRecord.AfterQty = StockInOutModelRaw.CurrentItemRecord.RdsQty - StockInOutModelRaw.CurrentItemRecord.Qty;
                            StockInOutModelRaw.CalculateCurrentItem();
                            StockInOutModelRaw.ConfirmCurrentItem();
                        }
                        listViewRaw.ItemsSource = StockInOutModelRaw.GetObservableCollection();
                        UpdateHeaderViewRaw();
                        UpdateItemViewRaw(new StockIoItemRecord());

                        //Finished
                        TaxpayerItemRecord taxpayerItemRecord2 = new TaxpayerItemRecord();
                        taxpayerItemMaster.ToRecord(taxpayerItemRecord2, listItemComposition[0].Tin, listItemComposition[0].ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                        StockInOutModelFinished.SetCurrentItem(taxpayerItemRecord2);
                        StockInOutModelFinished.CurrentItemRecord.Qty = qty;
                        StockInOutModelFinished.CurrentItemRecord.AfterQty = StockInOutModelFinished.CurrentItemRecord.RdsQty + StockInOutModelFinished.CurrentItemRecord.Qty;
                        StockInOutModelFinished.CalculateCurrentItem();
                        StockInOutModelFinished.ConfirmCurrentItem();
                        listViewFinished.ItemsSource = StockInOutModelFinished.GetObservableCollection();
                        UpdateHeaderViewFinished();
                        UpdateItemViewFinished(new StockIoItemRecord());
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

        private void OnSelectedItemFinished(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItemFinished = (StockIoItemRecord)e.SelectedItem;
            }
            else
            {
                SelectedItemFinished = null;
            }
        }

        private void OnSelectedItemRaw(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItemRaw = (StockIoItemRecord)e.SelectedItem;
            }
            else
            {
                SelectedItemRaw = null;
            }
        }
    }
}

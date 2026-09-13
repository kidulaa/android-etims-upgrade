using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessingItemPopup : ContentPage
    {
        TaxpayerItemRecord taxpayerItemRecord { get; set; }
        TaxpayerItemCompositionMaster taxpayerItemCompositionMaster { get; set; }
        TaxpayerItemCompositionRecord taxpayerItemCompositionRecord { get; set; }
        public ObservableCollection<TaxpayerItemCompositionRecord> lvCompositionManagement { get; set; }
        List<TaxpayerItemCompositionRecord> listItemComposition { get; set; }

        public ProcessingItemPopup()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            taxpayerItemCompositionMaster = new TaxpayerItemCompositionMaster();
            taxpayerItemCompositionRecord = new TaxpayerItemCompositionRecord();
            listItemComposition = new List<TaxpayerItemCompositionRecord>();

            Init();
        }
        public void Init()
        {
            // 초기화
            UpdateHeaderView(new TaxpayerItemRecord());
            //UpdateItemView(new TaxpayerItemCompositionRecord());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(taxpayerItemRecord == null) UpdateHeaderView(new TaxpayerItemRecord());
            else UpdateHeaderView(taxpayerItemRecord);

            //if (taxpayerItemCompositionRecord != null)
            //{
            //    UpdateItemView(taxpayerItemCompositionRecord);
            //    etQuantity.SetReadOnly(false);
            //    etQuantity.SetFocus();
            //}
            //else
            //{
            //    UpdateItemView(new TaxpayerItemCompositionRecord());
            //}
        }

        public void UpdateHeaderView(TaxpayerItemRecord taxpayerItemRecord)
        {
            etProcessingItemCode.SetEntryValue(taxpayerItemRecord.ItemCd);
            etProcessingItemName.SetEntryValue(taxpayerItemRecord.ItemNm);
            etProcessingQuantity.SetEntryValue(0);

            etProcessingItemCode.SetReadOnly(true);
            etProcessingItemName.SetReadOnly(true);
            etProcessingQuantity.SetReadOnly(false);
        }
        //public void UpdateItemView(TaxpayerItemCompositionRecord itemCompositionRecord)
        //{
        //    etItemCode.SetEntryValue(itemCompositionRecord.CpstItemCd);
        //    etItemName.SetEntryValue(itemCompositionRecord.CpstItemNm);
        //    etClassCode.SetEntryValue(itemCompositionRecord.CpstItemClsCd);
        //    etClassName.SetEntryValue(itemCompositionRecord.CpstItemClsNm);
        //    etQuantity.SetEntryValue(itemCompositionRecord.CpstQty);

        //    etItemCode.SetReadOnly(true);
        //    etItemName.SetReadOnly(true);
        //    etClassCode.SetReadOnly(true);
        //    etClassName.SetReadOnly(true);
        //    etQuantity.SetReadOnly(true);
        //}

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        public long GetQty = 0;

        void SetList(List<TaxpayerItemCompositionRecord> datas)
        {
            try
            {
                lvCompositionManagement = new ObservableCollection<TaxpayerItemCompositionRecord>();
                listView.ItemsSource = lvCompositionManagement;

                for (int i = 0; i < datas.Count; i++)
                {
                    lvCompositionManagement.Add(datas[i]);
                }
            }
            catch
            {
            }
        }

        async void OnFunctionClose(object sender, System.EventArgs e)
        {
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnFunctionCompositionConfirm(object sender, EventArgs e)
        {
            if(etProcessingQuantity.GetEntryValue() == 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
                etProcessingQuantity.SetFocus();
                return;
            }
            if (listItemComposition == null || listItemComposition.Count <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid Raw Material.", "OK");
                etProcessingQuantity.SetFocus();
                return;
            }
            if (OnResult != null && listItemComposition != null && listItemComposition.Count > 0)
            {
                GetQty = etProcessingQuantity.GetEntryValue();

                ////@ychan_20191208 Save 버튼 삭제 후 Confirm 에서 저장하고 나가게끔 수정
                //taxpayerItemCompositionMaster.InsertTable(listItemComposition);
                for (int i = 0; i < listItemComposition.Count; i++)
                {
                    double qtyProcessingQuantity = GetQty * listItemComposition[i].CpstQty;
                    if(qtyProcessingQuantity > listItemComposition[i].RdsQty)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Processing Quantity is greater than current stock qty. Please Check.", "OK");
                        return;
                    }
                }

                ExtEventArgs extEventArgs = new ExtEventArgs("Select", listItemComposition);
                OnResult?.Invoke(this, extEventArgs);
            }
        }

        //async void OnFunctionConfirm(object sender, System.EventArgs e)
        //{
        //    if (taxpayerItemCompositionRecord != null)
        //    {
        //        //etQuantity.GetEntry().C // Entry 입력 종료처리 필요.
        //        if (etQuantity.GetEntryValue() != 0)
        //        {
        //            bool bRet = false;
        //            taxpayerItemCompositionRecord.CpstQty = etQuantity.GetEntryValue();

        //            foreach(TaxpayerItemCompositionRecord itm in listItemComposition)
        //            {
        //                if(itm.ItemCd.Equals(taxpayerItemCompositionRecord.ItemCd) &&
        //                    itm.ItemNm.Equals(taxpayerItemCompositionRecord.ItemNm) &&
        //                    itm.CpstItemCd.Equals(taxpayerItemCompositionRecord.CpstItemCd) &&
        //                    itm.CpstItemNm.Equals(taxpayerItemCompositionRecord.CpstItemNm) &&
        //                    itm.CpstItemClsCd.Equals(taxpayerItemCompositionRecord.CpstItemClsCd) &&
        //                    itm.CpstItemClsNm.Equals(taxpayerItemCompositionRecord.CpstItemClsNm))
        //                {
        //                    itm.CpstQty = taxpayerItemCompositionRecord.CpstQty;
        //                    bRet = true;
        //                    break;
        //                }
        //            }

        //            if(bRet.Equals(false))
        //            {
        //                listItemComposition.Add(taxpayerItemCompositionRecord);
        //            }

        //            SetList(listItemComposition);

        //            taxpayerItemCompositionRecord = null;

        //            UpdateItemView(new TaxpayerItemCompositionRecord());
        //        }
        //        else
        //        {
        //            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
        //        }
        //    }
        //    else
        //    {
        //        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a product.", "OK");
        //    }
        //}

        private void OnRemove(object sender, System.EventArgs e)
        {
            //StockInOutModel.DeleteAll();
            //listView.ItemsSource = StockInOutModel.GetObservableCollection();
        }

        private void OnEmpty(object sender, System.EventArgs e)
        {
            //StockInOutModel.DeleteAll();
            //listView.ItemsSource = StockInOutModel.GetObservableCollection();
        }
        async void OnFunctionProcessingItemCode(object sender, EventArgs e)
        {
            var popupPage = new ItemPopupPage("Type-FinishedItem");
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    taxpayerItemRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;
                    etProcessingItemCode.SetEntryValue(taxpayerItemRecord.ItemCd);
                    etProcessingItemName.SetEntryValue(taxpayerItemRecord.ItemNm);

                    List<TaxpayerItemCompositionRecord>  listItem  = taxpayerItemCompositionMaster.getTaxpayerItemCompositionTable(taxpayerItemRecord.Tin, taxpayerItemRecord.ItemCd);
                    if(listItem != null && listItem.Count > 0)
                    {
                        for(int i = 0; i < listItem.Count; i++)
                        {
                            listItemComposition.Add(listItem[i]);
                        }
                        SetList(listItemComposition);
                    }
                    else
                    {
                        listItemComposition.Clear();
                        SetList(listItemComposition);
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

        //@ychan_20191208 Processing Item Management Popup 화면 추가
        async void OnFunctionCreate(object sender, EventArgs e)
        {
            var popupPage = new ProcessingItemManagementPopup();
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
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

        //async void OnFunctionItemCode(object sender, System.EventArgs e)
        //{
        //    if (taxpayerItemRecord == null || string.IsNullOrEmpty(etProcessingItemCode.GetEntryValue()))
        //    {
        //        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Finished Product.", "OK");
        //        return;
        //    }

        //    string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
        //    string UserId = UIManager.Instance().UserModel.UserId;
        //    string UserNm = UIManager.Instance().UserModel.UserNm;

        //    var popupPage = new ItemPopupPage();
        //    popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

        //    popupPage.OnResult += (popup, ex) => {
        //        Device.BeginInvokeOnMainThread(() => {
        //            TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;
        //            taxpayerItemCompositionRecord = new TaxpayerItemCompositionRecord();
        //            taxpayerItemCompositionRecord.Tin = taxpayerItemRecord.Tin;
        //            taxpayerItemCompositionRecord.ItemCd = taxpayerItemRecord.ItemCd;
        //            taxpayerItemCompositionRecord.CpstItemCd = popupRecord.ItemCd;
        //            taxpayerItemCompositionRecord.CpstItemNm = popupRecord.ItemNm;
        //            taxpayerItemCompositionRecord.CpstItemClsCd = popupRecord.ItemClsCd;
        //            taxpayerItemCompositionRecord.CpstItemClsNm = popupRecord.ItemClsName;
        //            taxpayerItemCompositionRecord.CpstQty = 0;
        //            taxpayerItemCompositionRecord.RegrId = UserId;
        //            taxpayerItemCompositionRecord.RegrNm = UserNm;
        //            taxpayerItemCompositionRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");

        //            UpdateItemView(taxpayerItemCompositionRecord);

        //            Navigation.PopAsync();
        //        });
        //    };

        //    popupPage.OnCanceled += (senderx, ex) => {
        //        Device.BeginInvokeOnMainThread(() => {
        //            Navigation.PopAsync();
        //        });
        //    };
        //    await Navigation.PushAsync(popupPage);
        //}

        //async void OnFunctionSave(object sender, EventArgs e)
        //{
        //    taxpayerItemCompositionMaster.InsertTable(listItemComposition);

        //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
        //}
    }
}

using EBM2x.Database.Master;
using EBM2x.Database.MasterEbm2x;
using EBM2x.RraSdc.process;
using EBM2x.UI.Backoffice.ItemManagement;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Backoffice.StockManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessingItemManagementPopup : ContentPage
    {
        TaxpayerItemRecord taxpayerItemRecord { get; set; }
        TaxpayerItemCompositionMaster taxpayerItemCompositionMaster { get; set; }
        TaxpayerItemCompositionRecord taxpayerItemCompositionRecord { get; set; }
        public ObservableCollection<TaxpayerItemCompositionRecord> lvCompositionManagement { get; set; }
        List<TaxpayerItemCompositionRecord> listItemComposition { get; set; }

        public ProcessingItemManagementPopup()
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
            UpdateItemView(new TaxpayerItemCompositionRecord());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(taxpayerItemRecord == null) UpdateHeaderView(new TaxpayerItemRecord());
            else UpdateHeaderView(taxpayerItemRecord);

            if (taxpayerItemCompositionRecord != null)
            {
                UpdateItemView(taxpayerItemCompositionRecord);
                etQuantity.SetReadOnly(false);
                etQuantity.SetFocus();
            }
            else
            {
                UpdateItemView(new TaxpayerItemCompositionRecord());
            }
        }
        public bool IsExist(string itemCode)
        {
            try
            {
                for (int i = 0; i < listItemComposition.Count; i++)
                {
                    if (listItemComposition[i].ItemCd.Equals(itemCode)) return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public void UpdateHeaderView(TaxpayerItemRecord taxpayerItemRecord)
        {
            etProcessingItemCode.SetEntryValue(taxpayerItemRecord.ItemCd);
            etProcessingItemName.SetEntryValue(taxpayerItemRecord.ItemNm);
            //etProcessingQuantity.SetEntryValue(0);

            etProcessingItemCode.SetReadOnly(true);
            etProcessingItemName.SetReadOnly(true);
            //etProcessingQuantity.SetReadOnly(false);
        }
        public void UpdateItemView(TaxpayerItemCompositionRecord itemCompositionRecord)
        {
            etItemCode.SetEntryValue(itemCompositionRecord.CpstItemCd);
            etItemName.SetEntryValue(itemCompositionRecord.CpstItemNm);
            etClassCode.SetEntryValue(itemCompositionRecord.CpstItemClsCd);
            etClassName.SetEntryValue(itemCompositionRecord.CpstItemClsNm);
            etQuantity.SetEntryValue(itemCompositionRecord.CpstQty);

            etItemCode.SetReadOnly(true);
            etItemName.SetReadOnly(true);
            etClassCode.SetReadOnly(true);
            etClassName.SetReadOnly(true);
            //etQuantity.SetReadOnly(true);
        }

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
            string locationTitle21 = UILocation.Instance().GetLocationText("Confirm");
            string locationMessage21 = UILocation.Instance().GetLocationText("Please check your save. Do you want to close this screen?");
            var result = await DisplayAlert(locationTitle21, locationMessage21, "Yes", "No");
            if (!result) return;

            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }

        async void OnFunctionCompositionConfirm(object sender, EventArgs e)
        {
            //if(etProcessingQuantity.GetEntryValue() == 0)
            //{
            //    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid quantity value.", "OK");
            //    etProcessingQuantity.SetFocus();
            //    return;
            //}
            //if (OnResult != null && listItemComposition != null && listItemComposition.Count > 0)
            //{
            //    GetQty = etProcessingQuantity.GetEntryValue();
            //    ExtEventArgs extEventArgs = new ExtEventArgs("Select", listItemComposition);
            //    OnResult?.Invoke(this, extEventArgs);
            //}

            ExtEventArgs extEventArgs = new ExtEventArgs("Select", listItemComposition);
            OnResult?.Invoke(this, extEventArgs);
        }

        async void OnFunctionConfirm(object sender, System.EventArgs e)
        {
            if (taxpayerItemCompositionRecord != null)
            {
                //etQuantity.GetEntry().C // Entry 입력 종료처리 필요.
                if (etQuantity.GetEntryValue() != 0)
                {
                    bool bRet = false;
                    taxpayerItemCompositionRecord.CpstQty = etQuantity.GetEntryValue();

                    foreach (TaxpayerItemCompositionRecord itm in listItemComposition)
                    {
                        if (itm.ItemCd.Equals(taxpayerItemCompositionRecord.ItemCd) &&
                            itm.ItemNm.Equals(taxpayerItemCompositionRecord.ItemNm) &&
                            itm.CpstItemCd.Equals(taxpayerItemCompositionRecord.CpstItemCd) &&
                            itm.CpstItemNm.Equals(taxpayerItemCompositionRecord.CpstItemNm) &&
                            itm.CpstItemClsCd.Equals(taxpayerItemCompositionRecord.CpstItemClsCd) &&
                            itm.CpstItemClsNm.Equals(taxpayerItemCompositionRecord.CpstItemClsNm))
                        {
                            itm.CpstQty = taxpayerItemCompositionRecord.CpstQty;
                            bRet = true;
                            break;
                        }
                    }

                    if (bRet.Equals(false))
                    {
                        listItemComposition.Add(taxpayerItemCompositionRecord);
                    }

                    SetList(listItemComposition);
                    taxpayerItemCompositionRecord = null;
                    UpdateItemView(new TaxpayerItemCompositionRecord());
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
        private void OnRemove(object sender, System.EventArgs e)
        {
            if (taxpayerItemCompositionRecord != null)
            {
                listItemComposition.Remove(taxpayerItemCompositionRecord);
                SetList(listItemComposition);
                taxpayerItemCompositionRecord = null;
                UpdateItemView(new TaxpayerItemCompositionRecord());
            }
        }

        private void OnEmpty(object sender, System.EventArgs e)
        {
            listItemComposition.Clear();
            SetList(listItemComposition);
            taxpayerItemCompositionRecord = null;
            UpdateItemView(new TaxpayerItemCompositionRecord());
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

        async void OnFunctionItemCode(object sender, System.EventArgs e)
        {
            if (taxpayerItemRecord == null || string.IsNullOrEmpty(etProcessingItemCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "Please select a Finished Product.", "OK");
                return;
            }

            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string UserId = UIManager.Instance().UserModel.UserId;
            string UserNm = UIManager.Instance().UserModel.UserNm;

            var popupPage = new ItemPopupPage("Type-RawItem");
            popupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

            popupPage.OnResult += (popup, ex) => {
                Device.BeginInvokeOnMainThread(() => {
                    TaxpayerItemRecord popupRecord = (TaxpayerItemRecord)((ExtEventArgs)ex).EnteredObject;
                    if (IsExist(popupRecord.ItemCd))
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "There is a registered product.", "OK");
                    }
                    else
                    {
                        taxpayerItemCompositionRecord = new TaxpayerItemCompositionRecord();
                        taxpayerItemCompositionRecord.Tin = taxpayerItemRecord.Tin;
                        taxpayerItemCompositionRecord.ItemCd = taxpayerItemRecord.ItemCd;
                        taxpayerItemCompositionRecord.CpstItemCd = popupRecord.ItemCd;
                        taxpayerItemCompositionRecord.CpstItemNm = popupRecord.ItemNm;
                        taxpayerItemCompositionRecord.CpstItemClsCd = popupRecord.ItemClsCd;
                        taxpayerItemCompositionRecord.CpstItemClsNm = popupRecord.ItemClsName;
                        taxpayerItemCompositionRecord.CpstQty = 0;
                        taxpayerItemCompositionRecord.RegrId = UserId;
                        taxpayerItemCompositionRecord.RegrNm = UserNm;
                        taxpayerItemCompositionRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");

                        UpdateItemView(taxpayerItemCompositionRecord);
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

        async void OnFunctionSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(etProcessingItemCode.GetEntryValue()))
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid ItemCode value.", "OK");
                return;
            }
            if (listItemComposition == null || listItemComposition.Count <= 0)
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "This is not a valid Raw Material.", "OK");
                return;
            }

            taxpayerItemCompositionMaster.DeleteComposition(listItemComposition[0]);
            bool ret = taxpayerItemCompositionMaster.InsertTable(listItemComposition);
            if (ret)
            {
                //===>>>>>>>>>
                //JCNA 20191204
                ItemCompositionRraSdcUpload itemCompositionRraSdcUpload = new ItemCompositionRraSdcUpload();
                itemCompositionRraSdcUpload.SendItemCompositionSave(listItemComposition[0].Tin, listItemComposition[0].ItemCd);

                //===>>>>>>>>>
                // JCNA 20191204 TR 전송 명령 실행
                RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                rraSdcUploadProcess.UploadProcess();

                //ExtEventArgs extEventArgs = new ExtEventArgs("Select", listItemComposition);
                ExtEventArgs extEventArgs = new ExtEventArgs("Select", null);
                OnResult?.Invoke(this, extEventArgs);

                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Save", "Saved successfully.", "OK");
            }
            else
            {
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "Error saving Item Composition.", "Ok");
            }
        }

        private void onSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                taxpayerItemCompositionRecord = (TaxpayerItemCompositionRecord)e.SelectedItem;
                UpdateItemView((TaxpayerItemCompositionRecord)e.SelectedItem);
            }
        }
    }
}

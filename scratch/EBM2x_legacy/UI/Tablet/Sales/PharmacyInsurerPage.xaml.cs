using EBM2x.Models;
using EBM2x.Models.ListView;
using EBM2x.Models.tran;
using EBM2x.Process.insurer;
using EBM2x.Process.search;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet.Sales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PharmacyInsurerPage : ContentPage
    {
        public ObservableCollection<SearchInsurerListViewModel> SearchInsurerList { get; set; }
        List<SearchInsurerNode> listSearchInsurerNode;

        public PharmacyInsurerPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            SearchInsurerList = new ObservableCollection<SearchInsurerListViewModel>();
            listView.ItemsSource = SearchInsurerList;
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가
        }

        void GetSearchItemList()
        {
            SearchInsurerList.Clear();

            listSearchInsurerNode = SearchInsurerProcess.QuerydSearchInsurer();
            foreach (SearchInsurerNode node in listSearchInsurerNode)
            {
                SearchInsurerList.Add(new SearchInsurerListViewModel { Node = node });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Subscribe();

            UIManager.Instance().PosModel.SetSalesTitleText("Pharmacy|Insurer");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().PosModel.TranModel.TranNode.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.InvalidateSurface();
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Select the Insurer");

            if(listSearchInsurerNode == null || listSearchInsurerNode.Count <= 0)
            {
                AnimationLoop();
            }
        }
        async void AnimationLoop()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            listSearchInsurerNode = SearchInsurerProcess.QuerydSearchInsurer();
            foreach (SearchInsurerNode node in listSearchInsurerNode)
            {
                SearchInsurerList.Add(new SearchInsurerListViewModel { Node = node });
            }
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

            MessagingCenter.Subscribe<Object, TranNode>(this, "Tran Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesReceive.InvalidateSurface(arg);
                    salesChange.InvalidateSurface(arg);
                    salesTotal.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, ItemList>(this, "Item Node", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesPanel.SalesItemListInvalidateSurface(arg);
                });
            });
 
            MessagingCenter.Subscribe<Object, string>(this, "Hold Count", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    holdTextButton.InvalidateSurface(string.Format("Hold[{0}]", arg));
                });
            });
            MessagingCenter.Subscribe<Object, string>(this, "Wait Count", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    waitTextButton.InvalidateSurface(string.Format("Wait[{0}]", arg));
                });
            });
        }

        void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");
            MessagingCenter.Unsubscribe<Object, TranNode>(this, "Tran Node");
            MessagingCenter.Unsubscribe<Object, ItemList>(this, "Item Node");
            MessagingCenter.Unsubscribe<Object, PosModel>(this, "Hold Count");
            MessagingCenter.Unsubscribe<Object, PosModel>(this, "Wait Count");
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string locationTitle3 = UILocation.Instance().GetLocationText("Info");
                string locationMessage3 = UILocation.Instance().GetLocationText("Are you sure set a discount");
                var result = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No");
                if (result)
                {
                    SearchInsurerNode searchInsurerNode = ((SearchInsurerListViewModel)e.SelectedItem).Node;
                    AddSearchInsurerProcess.excuteProcess(searchInsurerNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                }
                UIManager.Instance().InputModel.Clear();
            }
            else
            {
                //GetSearchItemList();
                //UIManager.Instance().InputModel.Clear();
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Info", "First click on the insurance company inquiry button.", "Ok");
            }
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "QueryInsurer":
                    GetSearchItemList();
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "InsurerRate":
                    int InsurerRate = 0;
                    if (UIManager.Instance().InputModel.EnteredText.Length > 0 && UIManager.Instance().InputModel.EnteredText.Length <= 3 )
                    {
                        InsurerRate = int.Parse(UIManager.Instance().InputModel.EnteredText);
                        if(InsurerRate < 1 || InsurerRate > 100)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Information", "Rate values ​​greater than 1 and less than 100.", "Ok");
                            break;
                        }
                    }
                    else
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Information", "Rate values ​​greater than 1 and less than 100.", "Ok");
                        break;
                    }
                    string locationTitle3 = UILocation.Instance().GetLocationText("Confirm?");
                    string locationMessage3 = UILocation.Instance().GetLocationText("Are you sure to Change rate.");
                    var confirm = await DisplayAlert(locationTitle3, locationMessage3, "Yes", "No"); 
                    if (confirm)
                    {
                        SearchInsurerNode searchInsurerNode = UIManager.Instance().PosModel.TranModel.TranNode.InsurerNode;
                        if(searchInsurerNode == null || string.IsNullOrEmpty(searchInsurerNode.InsurerCode))
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Information", "Choose an insurance company.", "Ok");
                            break;
                        }
                        searchInsurerNode.InsurerRate = InsurerRate;
                        AddSearchInsurerProcess.excuteProcess(searchInsurerNode, UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "CancelInsurer":
                    string locationTitle2 = UILocation.Instance().GetLocationText("Cancel?");
                    string locationMessage2 = UILocation.Instance().GetLocationText("Are you sure to Cancel");
                    var cancel = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                    if (cancel)
                    {
                        CancelInsurerProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;

                case "BackToSales":
                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;

                case "Payment":
                    Navigation.InsertPageBefore(new TabletPaymentPage(), this);
                    await Navigation.PopAsync();
                    break;

                default:
                    break;
            }
        }
    }
}

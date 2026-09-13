using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Popup;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletHeaderNonPanel : ContentView
    {
        public event EventHandler FunctionCalled;
        public static readonly BindableProperty FunctionCalledProperty = BindableProperty.Create(
                                                         propertyName: "FunctionCalled",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(TabletHeaderNonPanel),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);
        public bool OffEventHandler
        {
            get { return (bool)base.GetValue(OffEventHandlerProperty); }
            set { base.SetValue(OffEventHandlerProperty, value); }
        }
        public static readonly BindableProperty OffEventHandlerProperty = BindableProperty.Create(
                                                         propertyName: "OffEventHandler",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(TabletHeaderNonPanel),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public TabletHeaderNonPanel()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<Object, string>(this, "Business Day", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    businessDayText.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "User Information", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    userInformationText.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "Receipt Information", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    receiptInformationText.InvalidateSurface(arg);
                });
            });

            MessagingCenter.Subscribe<Object, string>(this, "Logo Image", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    logoImage.InvalidateSurface(arg);
                });
            });
            MessagingCenter.Subscribe<Object, string>(this, "Sales Title Color", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesTitleBox.InvalidateSurface(arg);
                });
            });
            MessagingCenter.Subscribe<Object, string>(this, "Sales Title Text", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    salesTitleText.InvalidateSurface(arg);
                });
            });
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
            if (!OffEventHandler)
            {
                if (FunctionCalled != null) FunctionCalled?.Invoke(this, new EventArgs());
            }
        }

        async void OnLocationButtonClicked(object sender, EventArgs e)
        {
            if (!OffEventHandler)
            {
                var locationPopupPage = new TabletLocationPopupPage();
                locationPopupPage.SetValue(NavigationPage.HasNavigationBarProperty, false);

                locationPopupPage.OnResult += (popup, ex) => {
                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(() => {
                        UILocation.Instance().SetLocation(((ExtEventArgs)ex).EnteredText);
                        //MessagingCenter.Send<Object, string>(this, "Location", ((FunctionEventArgs)e).EnteredText);

                        UIManager.Instance().InputModel.Clear();
                        Navigation.PopAsync();
                    });
                };

                locationPopupPage.OnCanceled += (popup, ex) => {
                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(() => {
                        UIManager.Instance().InputModel.Clear();
                        Navigation.PopAsync();
                    });
                };

                // Navigate to our location page
                await Navigation.PushAsync(locationPopupPage);
            }
        }
    }
}

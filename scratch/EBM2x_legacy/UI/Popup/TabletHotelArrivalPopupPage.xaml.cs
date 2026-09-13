using EBM2x.Database.Master;
using EBM2x.Models.config;
using EBM2x.Models.HotelRoom;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabletHotelArrivalPopupPage : ContentPage
    {
        public TabletHotelArrivalPopupPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid.InitializeComponent();
            stretchFixedGrid.InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            entityGuestName.TitleInvalidateSurface();

            entityArrivalDate.TitleInvalidateSurface();
            entityDepartureDate.TitleInvalidateSurface();

            entityNumberOfNights.TitleInvalidateSurface();

            UIManager.Instance().PosModel.SetSalesTitleText("Check In");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please enter");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public event EventHandler OnResult;
        public event EventHandler OnCanceled;

        public void SetData(string GuestName, DateTime ArrivalDate, DateTime DepartureDate, int quantity)
        {
            entityGuestName.GetEntry().Text = GuestName;

            entityArrivalDate.SetDateTime(ArrivalDate);
            entityDepartureDate.SetDateTime(DepartureDate);

            entityNumberOfNights.GetEntry().Text = "" + quantity;
        }

        async void OnCalculatorButtonClicked(object sender, EventArgs e)
        {
            HotelRoomNode hotelRoomNode = new HotelRoomNode();
            hotelRoomNode.ArrivalDate = entityArrivalDate.GetDateTime();
            hotelRoomNode.DepartureDate = entityDepartureDate.GetDateTime();

            if (hotelRoomNode.ArrivalDate > hotelRoomNode.DepartureDate)
            {
                hotelRoomNode.DepartureDate = hotelRoomNode.ArrivalDate;
                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The Departure Date is invalid.", "OK");
                return;
            }
            TimeSpan timeDiff = hotelRoomNode.DepartureDate - hotelRoomNode.ArrivalDate;
            int diffDays = timeDiff.Days;
            hotelRoomNode.NumberOfNights = diffDays;
            entityNumberOfNights.GetEntry().Text = "" + hotelRoomNode.NumberOfNights;
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (OnResult != null)
            {
                HotelRoomNode hotelRoomNode = new HotelRoomNode();
                hotelRoomNode.GuestName = entityGuestName.GetEntry().Text;
                hotelRoomNode.ArrivalDate = entityArrivalDate.GetDateTime();
                hotelRoomNode.DepartureDate = entityDepartureDate.GetDateTime();

                if (hotelRoomNode.ArrivalDate > hotelRoomNode.DepartureDate)
                {
                    hotelRoomNode.DepartureDate = hotelRoomNode.ArrivalDate;
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The Departure Date is invalid.", "OK");
                    return;
                }

                try
                {
                    hotelRoomNode.NumberOfNights = int.Parse(entityNumberOfNights.GetEntry().Text);
                }
                catch(Exception ex)
                {
                    hotelRoomNode.NumberOfNights = 0;
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is invalid.", "OK");
                    return;
                }

                ExtEventArgs extEventArgs = new ExtEventArgs("Select", hotelRoomNode);
                OnResult?.Invoke(this, extEventArgs);
            }
        }
        void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Clear();
            if (OnCanceled != null) OnCanceled?.Invoke(this, new EventArgs());
        }
        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }
    }
}

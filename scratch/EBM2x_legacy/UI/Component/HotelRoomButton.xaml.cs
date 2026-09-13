using EBM2x.Models.HotelRoom;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotelRoomButton : ContentView
    {
        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(HotelRoomButton),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);
        public string FunctionID
        {
            get { return base.GetValue(FunctionIDProperty).ToString(); }
            set { base.SetValue(FunctionIDProperty, value); }
        }
        public static readonly BindableProperty FunctionIDProperty = BindableProperty.Create(
                                                         propertyName: "FunctionID",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(HotelRoomButton),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public event EventHandler ItemSelected;
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(
                                                         propertyName: "ItemSelected",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(HotelRoomButton),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        public string BoxColor
        {
            get { return base.GetValue(BoxColorProperty).ToString(); }
            set { base.SetValue(BoxColorProperty, value); }
        }
        public static readonly BindableProperty BoxColorProperty = BindableProperty.Create(
                                                         propertyName: "BoxColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(HotelRoomButton),
                                                         defaultValue: "ecb7d1",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public bool IsCurrent
        {
            get { return (bool)base.GetValue(IsCurrentProperty); }
            set { base.SetValue(IsCurrentProperty, value); }
        }
        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(
                                                         propertyName: "IsCurrent",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(HotelRoomButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public HotelRoomButton()
        {
            InitializeComponent();

            groupBox.IsVisible = false;
            groupNum.IsVisible = false;
        }

        public void InvalidateSurface(HotelRoomNode itemNode, int no, bool isCurrent, bool useButtonEvent, bool editMode)
        {
            IsCurrent = isCurrent;

            backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null && itemNode.IsVisible)
            {
                roomNumber.InvalidateSurface(itemNode.HotelRoomCode);

                if (itemNode.IsGroup)
                {
                    groupBox.IsVisible = true;
                    groupNum.IsVisible = true;
                    groupBox.InvalidateSurface(itemNode.GroupCode);
                    groupNum.InvalidateSurface(itemNode.GroupCode);
                }
                else
                {
                    groupNum.InvalidateSurface("");
                    groupBox.IsVisible = false;
                    groupNum.IsVisible = false;
                }

                if (itemNode.IsOrdered)
                {
                    backgroungBox.InvalidateSurface("9ef7c9");

                    noNumber.InvalidateSurface(no, true);
                    arrivalDate.InvalidateSurface("Arrival Date : " + itemNode.GetArrivalDateText());
                    departureDate.InvalidateSurface("Departure Date : " + itemNode.GetDepartureDateText());
                    numberOfNights.InvalidateSurface("Number Of Nights : " + itemNode.NumberOfNights);
                    guestName.InvalidateSurface("Guest name : " + itemNode.GuestName);

                    double amount = itemNode.Amount; // + (itemNode.PriceOfNight * itemNode.NumberOfNights);
                    totalNumber.InvalidateSurface(amount, true);
                }
                else
                {
                    backgroungBox.InvalidateSurface("daaaf0");

                    noNumber.InvalidateSurface(no, true);
                    arrivalDate.InvalidateSurface("");
                    departureDate.InvalidateSurface("");
                    numberOfNights.InvalidateSurface("");
                    guestName.InvalidateSurface("");

                    totalNumber.InvalidateSurface(0, false);
                }
                selectOwner.IsVisible = itemNode.IsOwnerVisible;
                buttonEvent.IsVisible = true;
            }
            else if (itemNode != null)
            {
                backgroungBox.InvalidateSurface("9b989c");
                
                groupNum.InvalidateSurface("");
                groupBox.IsVisible = false;
                groupNum.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                if (editMode && !string.IsNullOrEmpty(itemNode.HotelRoomCode))
                {
                    roomNumber.InvalidateSurface(itemNode.HotelRoomCode);
                }
                else
                {
                    // JIIT_201911, 
                    roomNumber.InvalidateSurface("");
                }

                arrivalDate.InvalidateSurface("");
                departureDate.InvalidateSurface("");
                numberOfNights.InvalidateSurface("");
                guestName.InvalidateSurface("");

                totalNumber.InvalidateSurface(0, false);

                selectOwner.IsVisible = itemNode.IsOwnerVisible;
                buttonEvent.IsVisible = useButtonEvent;
            }
            else
            {
                backgroungBox.InvalidateSurface("9b989c");

                roomNumber.InvalidateSurface("");

                groupNum.InvalidateSurface("");
                groupBox.IsVisible = false;
                groupNum.IsVisible = false;

                noNumber.InvalidateSurface(no, true);
                arrivalDate.InvalidateSurface("");
                departureDate.InvalidateSurface("");
                numberOfNights.InvalidateSurface("");
                guestName.InvalidateSurface("");

                totalNumber.InvalidateSurface(0, false);

                selectOwner.IsVisible = false;
                buttonEvent.IsVisible = useButtonEvent;
            }
        }

        void OnBoxButtonClicked(object sender, EventArgs e)
        {
            if (ButtonClicked != null) ButtonClicked?.Invoke(this, new ExtEventArgs(FunctionID,""));
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}

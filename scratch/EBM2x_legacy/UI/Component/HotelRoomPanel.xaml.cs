using EBM2x.Models.HotelRoom;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotelRoomPanel : ContentView
    {
        public string BoxColor
        {
            get { return base.GetValue(BoxColorProperty).ToString(); }
            set { base.SetValue(BoxColorProperty, value); }
        }
        public static readonly BindableProperty BoxColorProperty = BindableProperty.Create(
                                                         propertyName: "BoxColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(HotelRoomPanel),
                                                         defaultValue: "daaaf0",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public HotelRoomPanel()
        {
            InitializeComponent();

            groupBox.IsVisible = false;
            groupNum.IsVisible = false;
        }

        public void InvalidateSurface(HotelRoomNode itemNode)
        {
            backgroungBox.InvalidateSurface(BoxColor);

            if (itemNode != null && itemNode.IsVisible)
            {
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

                    roomNumber.InvalidateSurface(itemNode.HotelRoomCode + "     Guest name : "+ itemNode.GuestName);

                    arrivalDate.InvalidateSurface("Arrival Date : " + itemNode.GetArrivalDateText());
                    departureDate.InvalidateSurface("Departure Date : " + itemNode.GetDepartureDateText());
                    numberOfNights.InvalidateSurface("Number Of Nights : " + itemNode.NumberOfNights);

                    totalTitle.InvalidateSurface("Total:");
                    double amount = itemNode.Amount; // + (itemNode.PriceOfNight * itemNode.NumberOfNights);
                    totalNumber.InvalidateSurface(amount, true);
                }
            }
        }

    }
}

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatePickerEntryPanel : ContentView
    {
        
        public DatePickerEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
        }
        public DatePicker GetDatePicker()
        {
            return entryDatePicker;
        }
        public DateTime GetDateTime()
        {
            return entryDatePicker.Date;
        }
        public void SetDateTime(DateTime date)
        {
            if(date != null && date > DateTime.MinValue) entryDatePicker.Date = date;
        }

        public DateTime GetEntryValue()
        {
            return entryDatePicker.Date;
        }
        public void SetEntryValue(DateTime date)
        {
            if (date != null && date > DateTime.MinValue) entryDatePicker.Date = date;
        }
        public bool SetFocus()
        {
            return entryDatePicker.Focus();
        }
        public bool IsValid()
        {
            return true;
        }        
    }
}

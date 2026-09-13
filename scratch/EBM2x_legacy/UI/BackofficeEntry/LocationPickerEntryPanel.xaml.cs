using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPickerEntryPanel : ContentView
    {
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> LocationList { get; set; }
        public LocationPickerEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            
            SystemCode = null;
            LocationList = new List<SystemCode>();
            LocationList.Add(new SystemCode() { Id = "en", Name = "English" });
            LocationList.Add(new SystemCode() { Id = "rw", Name = "Kiswahili" });
            //LocationList.Add(new SystemCode() { Id = "fr", Name = "French" });
            entryPicker.ItemDisplayBinding = new Binding("Name");
            entryPicker.ItemsSource = LocationList;
        }
        public SystemCode GetSelectedItem()
        {
            if (SystemCode == null) return new SystemCode();
            else return SystemCode;
        }
        public void SetSelecteItem(SystemCode systemCode)
        {
            for (int i = 0; i < LocationList.Count; i++)
            {
                if (systemCode.Id.Equals(LocationList[i].Id))
                {
                    entryPicker.SelectedIndex = i;
                    return;
                }
            }
            entryPicker.SelectedIndex = -1;
        }
        private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            SystemCode = (SystemCode)picker.SelectedItem; // This is the model selected in the picker
        }
        public bool SetFocus()
        {
            return entryPicker.Focus();
        }
        public bool IsValid()
        {
            return SystemCode == null ? false : true;
        }
        public void SetReadOnly(bool readOnly)
        {
            entryPicker.IsEnabled = !readOnly;
        }

    }
}

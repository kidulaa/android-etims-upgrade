using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class POSNumPickerEntryPanel : ContentView
    {
        public string SystemCode { get; set; }
        public List<string> PermissionList { get; set; }
        public POSNumPickerEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            
            SystemCode = string.Empty;
            PermissionList = new List<string>();
            PermissionList.Add("0001");
            PermissionList.Add("0002");
            PermissionList.Add("0003");
            PermissionList.Add("0004");
            PermissionList.Add("0005");
            entryPicker.ItemsSource = PermissionList;
        }
        public string GetSelectedItem()
        {
            return SystemCode;
        }
        public void SetSelecteItem(string systemCode)
        {
            for (int i = 0; i < PermissionList.Count; i++)
            {
                if (systemCode.Equals(PermissionList[i]))
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
            SystemCode = (string)picker.SelectedItem; // This is the model selected in the picker
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

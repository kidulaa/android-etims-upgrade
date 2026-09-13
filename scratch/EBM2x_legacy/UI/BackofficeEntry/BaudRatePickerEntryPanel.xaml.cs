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
    public partial class BaudRatePickerEntryPanel : ContentView
    {
        public int SystemCode { get; set; }
        public List<int> PermissionList { get; set; }
        public BaudRatePickerEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            
            SystemCode = 19200;
            PermissionList = new List<int>();
            PermissionList.Add(19200);
            PermissionList.Add(9600);
            PermissionList.Add(115200);
            entryPicker.ItemsSource = PermissionList;
        }
        public int GetSelectedItem()
        {
            return SystemCode;
        }
        public void SetSelecteItem(int systemCode)
        {
            for (int i = 0; i < PermissionList.Count; i++)
            {
                if (systemCode == PermissionList[i])
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
            SystemCode = (int)picker.SelectedItem; // This is the model selected in the picker
        }
        public bool SetFocus()
        {
            return entryPicker.Focus();
        }
        public bool IsValid()
        {
            return SystemCode < 1 ? false : true;
        }
        public void SetReadOnly(bool readOnly)
        {
            entryPicker.IsEnabled = !readOnly;
        }
    }
}

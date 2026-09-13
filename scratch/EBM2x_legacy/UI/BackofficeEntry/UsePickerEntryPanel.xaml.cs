using EBM2x.Database.Master;
using EBM2x.Models.config;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsePickerEntryPanel : ContentView
    {
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> pickerList { get; set; }

        public UsePickerEntryPanel()
        {
            InitializeComponent();

            SystemCode = null;

            CodeComboMaster codeComboMaster = new CodeComboMaster();
            pickerList = codeComboMaster.LoadCombo("22", "CODE DESC");

            entryFixedGrid.InitializeComponent();

            entryPicker.ItemDisplayBinding = new Binding("Name");
            entryPicker.ItemsSource = pickerList;
        }
        public SystemCode GetSelectedItem()
        {
            if (SystemCode == null) return new SystemCode();
            else return SystemCode;
        }
        public void SetSelecteItem(SystemCode systemCode)
        {
            for (int i = 0; i < pickerList.Count; i++)
            {
                if (systemCode.Id.Equals(pickerList[i].Id))
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

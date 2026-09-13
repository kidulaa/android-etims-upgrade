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
    public partial class RolePickerEntryPanel : ContentView
    {
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> pickerList { get; set; }

        public RolePickerEntryPanel()
        {
            InitializeComponent();

            SystemCode = null;

            CodeComboMaster codeComboMaster = new CodeComboMaster();
            //pickerList = codeComboMaster.LoadCombo("28", "CODE ASC");
            pickerList = new List<SystemCode>();
            pickerList.Add(new SystemCode() { Id = "1", Name = "admin" });
            pickerList.Add(new SystemCode() { Id = "2", Name = "Manager" });
            pickerList.Add(new SystemCode() { Id = "3", Name = "Teller" });
            pickerList.Add(new SystemCode() { Id = "4", Name = "Auditor" });
            pickerList.Add(new SystemCode() { Id = "5", Name = "Accountant" });

            entryFixedGrid.InitializeComponent();

            entryPicker.ItemDisplayBinding = new Binding("Name");
            entryPicker.ItemsSource = pickerList;
        }
        public SystemCode GetSelectedItem()
        {
            return SystemCode;
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

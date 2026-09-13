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
    public partial class BhfPickerEntryPanel : ContentView
    {
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> pickerList { get; set; }

        public BhfPickerEntryPanel()
        {
            InitializeComponent();

            SystemCode = null;

            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            CodeComboMaster codeComboMaster = new CodeComboMaster();
            pickerList = codeComboMaster.LoadBhf(BhfId);
            
            //pickerList = new List<SystemCode>();
            //pickerList.Add(new SystemCode() { Id = "01", Name = "Bhf 01" });
            //pickerList.Add(new SystemCode() { Id = "02", Name = "Bhf 02" });

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
                    break;
                }
            }
        }
        private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            SystemCode = (SystemCode)picker.SelectedItem; // This is the model selected in the picker
        }
        public Picker GetPicker()
        {
            return entryPicker;
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

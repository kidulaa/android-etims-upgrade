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
    public partial class CustGroupPickerEntryPanel : ContentView
    {
        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                         propertyName: "BorderColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(CustGroupPickerEntryPanel),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public string BackColor
        {
            get { return base.GetValue(BackColorProperty).ToString(); }
            set { base.SetValue(BackColorProperty, value); }
        }
        public static readonly BindableProperty BackColorProperty = BindableProperty.Create(
                                                         propertyName: "BackColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(CustGroupPickerEntryPanel),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> SystemCodeList { get; set; }
        public CustGroupPickerEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();

            SystemCode = null;

            SystemCodeList = new List<SystemCode>();
            SystemCodeList.Add(new SystemCode() { Id = "DF", Name = "Default" });
            SystemCodeList.Add(new SystemCode() { Id = "L1", Name = "Level 1" });
            //SystemCodeList.Add(new SystemCode() { Id = "L2", Name = "Level 2" });
            //SystemCodeList.Add(new SystemCode() { Id = "L3", Name = "Level 3" });
            //SystemCodeList.Add(new SystemCode() { Id = "L4", Name = "Level 4" });
            //SystemCodeList.Add(new SystemCode() { Id = "L5", Name = "Level 5" });

            entryPicker.ItemDisplayBinding = new Binding("Name"); 
            entryPicker.ItemsSource = SystemCodeList;
        }
        
        public SystemCode GetSelectedItem()
        {
            if (SystemCode == null) return new SystemCode();
            else return SystemCode;
        }
        public void SetSelecteItem(SystemCode systemCode)
        {
            for (int i = 0; i < SystemCodeList.Count; i++)
            {
                if (systemCode.Id.Equals(SystemCodeList[i].Id))
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

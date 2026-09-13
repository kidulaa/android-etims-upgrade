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
    public partial class ClassificationLvlEntryPanel : ContentView
    {
        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                         propertyName: "BorderColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ClassificationLvlEntryPanel),
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
                                                         declaringType: typeof(ClassificationLvlEntryPanel),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> pickerList { get; set; }
        public ClassificationLvlEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();

            pickerList = new List<SystemCode>();
            pickerList.Add(new SystemCode() { Id = "1", Name = "Lvl 1" });
            pickerList.Add(new SystemCode() { Id = "2", Name = "Lvl 2" });
            pickerList.Add(new SystemCode() { Id = "3", Name = "Lvl 3" });
            pickerList.Add(new SystemCode() { Id = "4", Name = "Lvl 4" });
            pickerList.Add(new SystemCode() { Id = "5", Name = "Lvl 5" });

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

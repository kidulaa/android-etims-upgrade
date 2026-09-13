using EBM2x.Models.config;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.EntryPanel
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPickerEntryPanel : ContentView
    {
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(LocationPickerEntryPanel),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public SystemCode SystemCode { get; set; }
        public List<SystemCode> LocationList { get; set; }
        public LocationPickerEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            entryTitle.InvalidateSurface(Text);
            
            SystemCode = null;
            LocationList = new List<SystemCode>();
            LocationList.Add(new SystemCode() { Id = "en", Name = "english" });
            LocationList.Add(new SystemCode() { Id = "rw", Name = "rwanda" }); 
            entryPicker.ItemDisplayBinding = new Binding("Name");
            entryPicker.ItemsSource = LocationList;
        }
        public void TitleInvalidateSurface(string text)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            entryTitle.InvalidateSurface(text);
        }
        public void TitleInvalidateSurface()
        {
            entryTitle.InvalidateSurface(Text);
        }
        public void TitleInvalidateSurface(string text, string textAlign, string textColor, float fillRate)
        {
            if (string.IsNullOrEmpty(text)) Text = "";
            else Text = text;
            entryTitle.InvalidateSurface(text, textAlign, textColor, fillRate);
        }
        public DrawText GetEntryTitle()
        {
            return entryTitle;
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

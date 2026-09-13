using EBM2x.Database.Master;
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
    public partial class TaxTypePickerEntryPanel : ContentView
    {
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(TaxTypePickerEntryPanel),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);

        public SystemCode SystemCode { get; set; }
        public List<SystemCode> pickerList { get; set; }

        public TaxTypePickerEntryPanel()
        {
            InitializeComponent();

            SystemCode = null;

            CodeComboMaster codeComboMaster = new CodeComboMaster();
            pickerList = codeComboMaster.LoadCombo("04", "CODE ASC");

            //TaxTypeList = new List<SystemCode>();
            //TaxTypeList.Add(new SystemCode() { Id = "01", Name = "A - EX" });
            //TaxTypeList.Add(new SystemCode() { Id = "02", Name = "B - 18.00 %" });
            //TaxTypeList.Add(new SystemCode() { Id = "03", Name = "C" });
            //TaxTypeList.Add(new SystemCode() { Id = "04", Name = "D" });

            entryFixedGrid.InitializeComponent();
            entryTitle.InvalidateSurface(Text);

            entryPicker.ItemDisplayBinding = new Binding("Name");
            entryPicker.ItemsSource = pickerList;
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
            boxButton.IsVisible = readOnly;
        }

    }
}

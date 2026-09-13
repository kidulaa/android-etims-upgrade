using EBM2x.UI.Draw;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarCodeMaskedEntryPanel : ContentView
    {
        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                         propertyName: "BorderColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(BarCodeMaskedEntryPanel),
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
                                                         declaringType: typeof(BarCodeMaskedEntryPanel),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public BarCodeMaskedEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            entryEntry.TextChanged += OnTextChanged;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(e.NewTextValue, "^[0-9]+$", RegexOptions.CultureInvariant))
                (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9]", string.Empty);
        }

        public Entry GetEntry()
        {
            return entryEntry;
        }
        public string GetEntryValue()
        {
            if (entryEntry.Text == null) return "";
            return entryEntry.Text;
        }
        public void SetEntryValue(string stringValue)
        {
            entryEntry.Text = stringValue;
        }
        public bool SetFocus()
        {
            return entryEntry.Focus();
        }
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(entryEntry.Text);
        }
        public void SetReadOnly(bool readOnly)
        {
            entryEntry.IsReadOnly = readOnly;
        }
    }

}

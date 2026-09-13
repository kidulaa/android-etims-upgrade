using EBM2x.UI.Draw;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.EntryPanel
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendedDateEntryPanel : ContentView
    {
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ExtendedDateEntryPanel),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public ExtendedDateEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            entryTitle.InvalidateSurface(Text);
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
            try
            {
                if (stringValue.Length == 8)
                {
                    entryEntry.Text = DateFormat(stringValue);
                }
                else if (stringValue.Length == 14)
                {
                    entryEntry.Text = DateTimeFormat(stringValue);
                }
                else
                {
                    entryEntry.Text = stringValue;
                }
            }
            catch
            {
                entryEntry.Text = stringValue;
            }
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
        private string DateFormat(string date)
        {
            string result = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");
            return result;
        }
        private string DateTimeFormat(string date)
        {
            string result = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm:ss");
            return result;
        }
    }
}

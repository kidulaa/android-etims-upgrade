using EBM2x.UI.Draw;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.EntryPanel
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LongMaskedEntryPanel : ContentView
    {
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(LongMaskedEntryPanel),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public LongMaskedEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            entryTitle.InvalidateSurface(Text);
            entryEntry.TextChanged += OnTextChanged;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(e.NewTextValue, "^[0-9,]+$", RegexOptions.CultureInvariant))
                (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9,]", string.Empty);

            string buffer = (sender as Entry).Text;
            if (buffer.Length > 15) buffer = buffer.Substring(0, 15);
            try
            {
                double inputValue = double.Parse(buffer.Replace(",", ""));
                buffer = inputValue.ToString("#,##0");

                (sender as Entry).Text = buffer;
            }
            catch
            {
            }
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
        public long GetEntryValue()
        {
            if (string.IsNullOrEmpty(entryEntry.Text)) return 0;
            else
            {
                try
                {
                    return Convert.ToInt32(entryEntry.Text.Replace(",", ""));
                }
                catch
                {
                    return 0;
                }
            }
        }
        public void SetEntryValue(long stringValue)
        {
            if (stringValue > 999999999 && stringValue < 0) entryEntry.Text = "0";
            else entryEntry.Text = Convert.ToString(stringValue);
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

using EBM2x.UI.Draw;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.EntryPanel
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DigitExtendedEntryPanel : ContentView
    {
        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                         propertyName: "Text",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DigitExtendedEntryPanel),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public DigitExtendedEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            entryTitle.InvalidateSurface(Text);
            entryEntry.TextChanged += OnTextChanged;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as Entry).Text == null) (sender as Entry).Text = "";
            if (!Regex.IsMatch(e.NewTextValue, "^[0-9]+$", RegexOptions.CultureInvariant))
                (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9]", string.Empty);
            
            //if ((sender as Entry).Text == null) (sender as Entry).Text = "";
            //if (!Regex.IsMatch(e.NewTextValue, "^[0-9]+$", RegexOptions.CultureInvariant))
            //{
            //    string text = Regex.Replace(e.NewTextValue, "[^0-9]", string.Empty);
            //    if (string.IsNullOrEmpty(text)) (sender as Entry).Text = "";
            //    else (sender as Entry).Text = text;
            //}
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
            if (string.IsNullOrEmpty(stringValue)) stringValue = "";
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

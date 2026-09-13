using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RateMaskedEntryPanel : ContentView
    {
        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                         propertyName: "BorderColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(RateMaskedEntryPanel),
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
                                                         declaringType: typeof(RateMaskedEntryPanel),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);

        public RateMaskedEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();
            entryEntry.TextChanged += OnTextChanged;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(e.NewTextValue, "^[0-9,]+$", RegexOptions.CultureInvariant))
                (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9,]", string.Empty);

            string buffer = (sender as Entry).Text;
            if (buffer.Length > 2) buffer = buffer.Substring(0, 3);

            try
            {
                double inputValue = double.Parse(buffer.Replace(",", ""));
                buffer = inputValue.ToString("#0");

                if(inputValue > 1000)
                {
                    buffer = buffer.Substring(0, 3);
                }

                (sender as Entry).Text = buffer;
            }
            catch
            {
            }
        }

        public Entry GetEntry()
        {
            return entryEntry;
        }
        public int GetEntryValue()
        {
            if (string.IsNullOrEmpty(entryEntry.Text))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Convert.ToInt16(entryEntry.Text);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public void SetEntryValue(int stringValue)
        {
            if (stringValue > 99 && stringValue < 0) entryEntry.Text = "0";
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

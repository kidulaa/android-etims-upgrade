using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.BackofficeEntry
{
    //[Xamarin.Forms.RenderWith(typeof(Xamarin.Forms.Platform._PickerRenderer))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AmountMaskedEntryPanel : ContentView
    {
        public string BorderColor
        {
            get { return base.GetValue(BorderColorProperty).ToString(); }
            set { base.SetValue(BorderColorProperty, value); }
        }
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
                                                         propertyName: "BorderColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(AmountMaskedEntryPanel),
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
                                                         declaringType: typeof(AmountMaskedEntryPanel),
                                                         defaultValue: "000000",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public AmountMaskedEntryPanel()
        {
            InitializeComponent();
            entryFixedGrid.InitializeComponent();

            entryEntry.TextChanged += OnTextChanged;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(e.NewTextValue, "^[0-9,.]+$", RegexOptions.CultureInvariant))
                (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9,.]", string.Empty);

            string buffer = (sender as Entry).Text;
            if (buffer.Length > 17) buffer = buffer.Substring(0, 17);

            if (buffer.IndexOf(".") >= 0)
            {
                try
                {
                    // 2021.02.22
                    ////if (buffer.IndexOf("..") >= 0) buffer = buffer.Replace("..", ".");
                    if (buffer.IndexOf(".") != buffer.LastIndexOf(".")) buffer = buffer.Substring(0, buffer.LastIndexOf("."));

                    string decimalPoint = buffer.Substring(buffer.IndexOf("."));
                    if (decimalPoint.Length > 3) decimalPoint = decimalPoint.Substring(0, 3);

                    double inputValue = double.Parse(buffer.Replace(",",""));

                    //Added By Aimee on 28.3.2022
                    buffer = inputValue.ToString("#,##0.####");

                    //buffer = inputValue.ToString("#,##0.##");

                    //END

                    // 2021.02.22
                    //if (buffer.IndexOf(".") < 0) buffer += ".";
                    if (buffer.IndexOf(".") < 0) buffer += decimalPoint;

                    (sender as Entry).Text = buffer;
                }
                catch
                {
                }
            }
            else
            {
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
        }

        public Entry GetEntry()
        {
            return entryEntry;
        }
        public double GetEntryValue()
        {
            if (string.IsNullOrEmpty(entryEntry.Text)) return 0;
            else
            {
                try
                {
                    return Convert.ToDouble(entryEntry.Text.Replace(",", ""));
                }
                catch
                {
                    return 0;
                }
            }
        }
        public void SetEntryValue(double stringValue)
        {
            if (stringValue > 99999999999) entryEntry.Text = "0";
            else if (stringValue < 0)
            {
                entryEntry.Text = "0";
            }
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

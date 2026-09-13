using Xamarin.Forms;

namespace EBM2x.UI.Draw
{
    public class StretchFixedGrid : Grid
    {
        public int ColumnCount
        {
            get { return (int)(base.GetValue(ColumnCountProperty)); }
            set { base.SetValue(ColumnCountProperty, value); }
        }
        public static readonly BindableProperty ColumnCountProperty = BindableProperty.Create(
                                                         propertyName: "ColumnCount",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(StretchFixedGrid),
                                                         defaultValue: 0,
                                                         defaultBindingMode: BindingMode.OneWay);
        public int RowCount
        {
            get { return (int)(base.GetValue(RowCountProperty)); }
            set { base.SetValue(RowCountProperty, value); }
        }
        public static readonly BindableProperty RowCountProperty = BindableProperty.Create(
                                                         propertyName: "RowCount",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(StretchFixedGrid),
                                                         defaultValue: 0,
                                                         defaultBindingMode: BindingMode.OneWay);

        public double Spacing
        {
            get { return (double)(base.GetValue(SpacingProperty)); }
            set { base.SetValue(SpacingProperty, value); }
        }
        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
                                                         propertyName: "Spacing",
                                                         returnType: typeof(double),
                                                         declaringType: typeof(StretchFixedGrid),
                                                         defaultValue: 0.0d,
                                                         defaultBindingMode: BindingMode.OneWay);

        public StretchFixedGrid() : base()
        {
            this.RowSpacing = Spacing;
            this.ColumnSpacing = Spacing;
        }

        public void InitializeComponent()
        {
            int PageRowCount = RowCount;
            int PageColumnCount = ColumnCount;
            if(UIManager.Instance().IsWindows)
            {
                PageRowCount = RowCount;
                PageColumnCount = ColumnCount * 2;
            }

            this.RowSpacing = Spacing;
            this.ColumnSpacing = Spacing;

            for (int i = 0; i < PageRowCount; i++)
            {
                RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < PageColumnCount; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
}

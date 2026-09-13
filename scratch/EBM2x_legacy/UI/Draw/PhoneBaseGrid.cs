using Xamarin.Forms;

namespace EBM2x.UI.Draw
{
    public class PhoneBaseGrid : Grid
    {
        int rowCount = 23;
        int colCount = 45;

        public PhoneBaseGrid()
        {
            this.RowSpacing = 0;
            this.ColumnSpacing = 0;

            for (int i = 0; i < rowCount; i++)
            {
                RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < colCount; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
}

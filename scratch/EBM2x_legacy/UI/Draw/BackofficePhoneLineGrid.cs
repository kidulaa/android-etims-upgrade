using Xamarin.Forms;

namespace EBM2x.UI.Draw
{
    public class BackofficePhoneLineGrid : Grid
    {
        int rowCount = 2;
        int colCount = 39;

        public BackofficePhoneLineGrid()
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

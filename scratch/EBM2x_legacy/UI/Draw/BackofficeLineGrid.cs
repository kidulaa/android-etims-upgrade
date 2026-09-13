using Xamarin.Forms;

namespace EBM2x.UI.Draw
{
    public class BackofficeLineGrid : Grid
    {
        int rowCount = 1;
        int colCount = 89;

        public BackofficeLineGrid()
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

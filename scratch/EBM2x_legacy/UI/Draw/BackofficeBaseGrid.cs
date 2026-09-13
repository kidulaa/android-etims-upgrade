using Xamarin.Forms;

namespace EBM2x.UI.Draw
{
    public class BackofficeBaseGrid : Grid
    {
        int rowCount = 20;
        int colCount = 90;

        public BackofficeBaseGrid()
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

using Xamarin.Forms;

namespace EBM2x.UI.Draw
{
    public class FlexibleGrid : Grid
    {
        public FlexibleGrid(int colCount, int rowCount, double columnSpacing, double rowSpacing)
        {
            this.RowSpacing = rowSpacing;
            this.ColumnSpacing = columnSpacing;

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

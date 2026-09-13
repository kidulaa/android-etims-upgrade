using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroceryStoreSalesPanel : ContentView
    {
        List<TabletSalesItemView> SalesItemViewList = null;

        public GroceryStoreSalesPanel()
        {
            InitializeComponent();

            SalesItemViewList = new List<TabletSalesItemView>();
            SalesItemViewList.Add(itemNode01);
            SalesItemViewList.Add(itemNode02);
            SalesItemViewList.Add(itemNode03);
            SalesItemViewList.Add(itemNode04);
            SalesItemViewList.Add(itemNode05);
            SalesItemViewList.Add(itemNode06);
            SalesItemViewList.Add(itemNode07);
            SalesItemViewList.Add(itemNode08);
            SalesItemViewList.Add(itemNode09);
            SalesItemViewList.Add(itemNode10);
        }

        public void SalesItemListInvalidateSurface(ItemList arg)
        {
            if (arg.LinesAtWhichPageBegins == 0)
            {
                ItemNode itemNode = null;
                for(int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    SalesItemViewList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + (i + 1), false);
                }
            }
            else
            {
                ItemNode itemNode = null;
                bool isCurrent = false;

                for (int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    if (arg.LinesAtWhichPageBegins + i <= arg.Count())
                    {
                        if (arg.CurrentLineNumber == arg.LinesAtWhichPageBegins + i) isCurrent = true;
                        itemNode = arg.Get(arg.LinesAtWhichPageBegins + (i - 1));
                    }
                    else
                    {
                        itemNode = null;
                    }
                    SalesItemViewList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + i, isCurrent);
                    isCurrent = false;
                }
            }
        }

        public void InvalidateSurface(TranNode arg)
        {
            if (arg.TranFlag.Equals(TranDefine.TRAN_RETURN))
            {
                salesHeader.InvalidateSurface("701919");

                for (int i = 0; i < 10; i++)
                {
                    SalesItemViewList[i].InvalidateSurface("f7caca");
                    i++;
                }
            }
        }

        void OnPageUpButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.PageUp();
        }

        void OnPageDownButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.PageDown();
        }

        void OnBoxButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            UIManager.Instance().PosModel.TranModel.TranNode.ItemList.SetCurrent(index);
        }
    }
}

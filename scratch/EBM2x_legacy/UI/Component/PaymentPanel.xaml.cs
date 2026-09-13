using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPanel : ContentView
    {
        List<PaymentTenderView> SalesTenderViewList = null;

        public PaymentPanel()
        {
            InitializeComponent();

            SalesTenderViewList = new List<PaymentTenderView>();
            SalesTenderViewList.Add(itemNode01);
            SalesTenderViewList.Add(itemNode02);
            SalesTenderViewList.Add(itemNode03);
            SalesTenderViewList.Add(itemNode04);
            SalesTenderViewList.Add(itemNode05);
        }
        public void InvalidateSurface(TranNode arg)
        {
            if (arg.TranFlag.Equals(TranDefine.TRAN_RETURN))
            {
                paymentHeader.InvalidateSurface("701919");
                for (int i = 0; i < 5; i++)
                {
                    SalesTenderViewList[i].InvalidateSurface("f7caca");
                    i++;
                }
            }
        }
        public void SalesTenderListInvalidateSurface(TenderList arg)
        {
            if (arg.LinesAtWhichPageBegins == 0)
            {
                TenderNode itemNode = null;
                for (int i = 0; i < arg.CountOfItemsToDisplayOnOnePage; i++)
                {
                    SalesTenderViewList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + (i + 1), false);
                }
            }
            else
            {
                TenderNode itemNode = null;
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
                    SalesTenderViewList[i].InvalidateSurface(itemNode, arg.LinesAtWhichPageBegins + i, isCurrent);
                    isCurrent = false;
                }
            }
        }

        void OnPageUpButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.PageUp();
        }

        void OnPageDownButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.PageDown();
        }

        void OnBoxButtonClicked(object sender, EventArgs e)
        {
            int index = int.Parse(((ExtEventArgs)e).FunctionID);
            UIManager.Instance().PosModel.TranModel.TranNode.TenderList.SetCurrent(index);
        }
    }
}

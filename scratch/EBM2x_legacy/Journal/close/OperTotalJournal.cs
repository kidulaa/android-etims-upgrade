using EBM2x.Models;
using EBM2x.Models.regitotal;
using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal.close
{
    public class OperTotalJournal
    {
        public void create(PosModel posModel)
        {
            try
            {
                posModel.Journal.Clear();
                RegiDetail regiDetail = posModel.OperTotal.RegiDetail;

               
                Journal.JournalHeader header = new Journal.JournalHeader();
                header.create(posModel);

                makeRegiBodystring(posModel, regiDetail);

                Journal.JournalFooterEtc footer = new Journal.JournalFooterEtc();
                footer.create(posModel);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

        private void makeRegiBodystring(PosModel posModel, RegiDetail regiDetail)
        {
            string text;
            posModel.Journal.AddBoldLine();
            if (UIManager.Instance().Is58mmPrinter)
            {
                text = "###  Z   R e p o r t  ###";
            }
            else
            {
                text = "    #####    Z   R e p o r t   #####    ";
            }
            //text = "    #####  #####    ";
            posModel.Journal.Add("hide", text);
            posModel.Journal.Add(string.Empty);

         
            int totSaleCount = regiDetail.VoidAmount.Count + regiDetail.ReturnSale.Count + regiDetail.TotalAmount.Count;
            double totSaleAmount = regiDetail.VoidAmount.Amount + regiDetail.ReturnSale.Amount + regiDetail.TotalAmount.Amount;
            //posModel.Journal.AddRegiFormat("", totSaleCount, totSaleAmount);
            posModel.Journal.AddRegiFormat(" Tot Trans   ", totSaleCount, totSaleAmount);

            posModel.Journal.Add(string.Empty);
            //     Item        Qty          Amount   
            //  Void sales (123456)  1234567890123 
            //1234567890123456789012345678901234567"
            if (UIManager.Instance().Is58mmPrinter)
            {
                posModel.Journal.Add("------------------------------");
                posModel.Journal.Add("Item          Qty       Amount");
                posModel.Journal.Add("------------------------------");
            }
            else
            {
                posModel.Journal.Add("-------------------------------------");
                posModel.Journal.Add("     Item        Qty          Amount ");
                posModel.Journal.Add("-------------------------------------");
            }
            //posModel.Journal.AddRegiFormat("    ", regiDetail.VoidAmount);
            posModel.Journal.AddRegiFormat(" Void sales  ", regiDetail.VoidAmount);

            //posModel.Journal.AddRegiFormat("     ", regiDetail.ReturnSale);
            posModel.Journal.AddRegiFormat(" Refund sales", regiDetail.ReturnSale);

            posModel.Journal.Add(string.Empty);

            // 총매출액
            //posModel.Journal.AddRegiFormat("     ", regiDetail.TotalAmount);
            posModel.Journal.AddRegiFormat(" Total sales ", regiDetail.TotalAmount);

            posModel.Journal.Add(string.Empty);

            // 할인금액
            //posModel.Journal.AddRegiFormat("    ", regiDetail.DiscountPartRate);
            posModel.Journal.AddRegiFormat(" Discount(%) ", regiDetail.DiscountPartRate);

            //// 에누리
            ////posModel.Journal.AddRegiFormat("   ", regiDetail.DiscountPartAmount);
            //posModel.Journal.AddRegiFormat(" Discout(amt)", regiDetail.DiscountPartAmount);

            // 에누리계
            //posModel.Journal.AddRegiFormat("     ", regiDetail.DiscountAmount);
            posModel.Journal.AddRegiFormat(" Tot Discount", regiDetail.DiscountAmount);

            posModel.Journal.Add(string.Empty);

            // 순매출액
            //posModel.Journal.AddRegiFormat("  ", regiDetail.PureAmount);
            posModel.Journal.AddRegiFormatSubtotal(" Subtotal    ", regiDetail.PureAmount);

            posModel.Journal.Add(string.Empty);

            if (UIManager.Instance().Is58mmPrinter)
            {
                posModel.Journal.Add("------- Other Payment --------");
            }
            else
            {
                posModel.Journal.Add("------------ Other Payment -------------");
            }
            posModel.Journal.AddRegiFormat(" Credit card ", regiDetail.Credit);

            posModel.Journal.AddRegiFormat(" Debit card  ", regiDetail.Debit);

            // MobileWallet
            posModel.Journal.AddRegiFormat(" MobileWallet", regiDetail.MobileWallet);

            //posModel.Journal.AddRegiFormat(" Gift card   ", regiDetail.Gift);

            posModel.Journal.AddLine();
            
            posModel.Journal.AddRegiFormat(" Other Total ", regiDetail.GetSubstituteTotalCount(), regiDetail.GetSubstituteTotalAmount());

            posModel.Journal.Add(string.Empty);
            if (regiDetail.Cutting.Count > 0)
            {
                posModel.Journal.AddRegiFormat("  Cut off    ", regiDetail.Cutting);
            }

            //posModel.Journal.AddRegiFormat(" ", regiDetail.CashSale);
            posModel.Journal.AddRegiFormat(" Cash sales  ", regiDetail.CashSale);

            ////posModel.Journal.AddRegiFormat("  ", regiDetail.ReservedFund);
            //posModel.Journal.AddRegiFormat(" Cash in     ", regiDetail.ReservedFund);

            //posModel.Journal.AddRegiFormat("   ", regiDetail.GetCashTotalAmount());
            posModel.Journal.AddRegiFormat(" Tot cash in ", regiDetail.GetCashTotalAmount());
            posModel.Journal.AddLine();

            posModel.Journal.Add(string.Empty);
            

            //posModel.Journal.Add(string.Empty);

            ////posModel.Journal.AddRegiFormat(" ", regiDetail.GetOutputCashAmount());
            //posModel.Journal.AddRegiFormat(" Cash out Amt ", regiDetail.GetOutputCashAmount());

            posModel.Journal.Add(string.Empty);

            
            //posModel.Journal.AddRegiFormat("      ", regiDetail.GetCashBalanceAmount());
            posModel.Journal.AddRegiFormat(" Cash balance", regiDetail.GetCashBalanceAmount());

            
            //posModel.Journal.AddRegiFormat("     ", regiDetail.CashStock.Amount);
            posModel.Journal.AddRegiFormat(" Cash stock  ", regiDetail.CashStock.Amount);

            posModel.Journal.Add(string.Empty);

            //posModel.Journal.AddRegiFormat("     ", regiDetail.SumCashOverShortAmount());
            posModel.Journal.AddRegiFormat(" Cash over   ", regiDetail.SumCashOverShortAmount());

            posModel.Journal.AddLine();
        }
    }
}

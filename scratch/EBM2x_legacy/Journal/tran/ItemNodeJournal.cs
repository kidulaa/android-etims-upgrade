using EBM2x.Models;
using EBM2x.Models.tran;
using EBM2x.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Journal.tran
{
    public class ItemNodeJournal
    {
        public void getItemListString(PosModel posModel, ItemList fRecord)
        {
            posModel.Environment.EnvFunctionNode.NoPrintItemList = "N";
            if (posModel.Environment.EnvFunctionNode.NoPrintItemList.ToUpper().Equals("N"))
                getItemListAllString(posModel, fRecord);
            else
            {                
                posModel.Journal.AddDoubleLine();
                //posModel.Journal.Add("hide", "Sales   Amt" + Journal.JournalUtil.lpad(29, posModel.TranModel.TranNode.Subtotal * posModel.TranModel.TranNode.Sign));
            }
            getDiscountString(posModel, fRecord);
            getTaxString(posModel, fRecord);
        }

        public void getItemListBackupString(PosModel posModel, ItemList fRecord)
        {
            getItemListAllBackupString(posModel, fRecord);

            posModel.Journal.AddDoubleLine();

            getDiscountString(posModel, fRecord);
            getTaxString(posModel, fRecord);
        }

        public void getBackupItemListString(PosModel posModel, ItemList fRecord)
        {
            getItemListPartString(posModel, fRecord);

            posModel.Journal.AddDoubleLine();

            getSlipString(posModel, fRecord);
            getTaxString(posModel, fRecord);
        }

        public void getVoidItemListString(PosModel posModel, ItemList fRecord)
        {
            getItemListAllString(posModel, fRecord);
        }

        public void getItemListAllString(PosModel posModel, ItemList fRecord)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
             }
            else
            {
                // 0---------1---------2---------3---------4---------5
                //  ========================================
             
                //  ----------------------------------------
             
                //    *8801016314622 99999990-999-99,999,990
                //            99999990    -99,999,990
                //            99999990    -99,999,990
                // 0---------1---------2---------3---------4---------5
            }

            int seq = 1;
            for (int i = 0; i < fRecord.Count(); i++)
            {
                if (fRecord.Get(i).Quantity > 0)
                {
                    getItemNodeString(posModel, seq++, fRecord.Get(i));
                }
            }
        }

        public void getItemListAllBackupString(PosModel posModel, ItemList fRecord)
        {
            // Title
            posModel.Journal.AddDoubleLine();
            if (UIManager.Instance().Is58mmPrinter)
            {
                posModel.Journal.Add("Item name");
                posModel.Journal.Add("         Price  Qty     Amount");
            }
            else
            {
                posModel.Journal.Add("      Item         Price  Qty     Amount");
            }
            posModel.Journal.AddLine();

           
            int seq = 1;
            for (int i = 0; i < fRecord.Count(); i++)
            {
                if (fRecord.Get(i).Quantity > 0)
                {
                    getItemNodeString(posModel, seq++, fRecord.Get(i));
                }
            }
        }

        public void getItemListPartString(PosModel posModel, ItemList fRecord)
        {
            // Title
            posModel.Journal.AddDoubleLine();
            if (UIManager.Instance().Is58mmPrinter)
            {
                posModel.Journal.Add("Item name");
                posModel.Journal.Add("         Price  Qty     Amount");
            }
            else
            {
                posModel.Journal.Add("      Item         Price  Qty     Amount");
            }
            posModel.Journal.AddLine();

            int seq = 1;
            for (int i = 0; i < fRecord.Count(); i++)
            {
                if (fRecord.Get(i).Quantity > 0)
                {
                    getItemNodeString(posModel, seq++, fRecord.Get(i));
                    if (seq == 4) break;
                }
            }
            
            if (seq < 4)
            {
                posModel.Journal.Add(string.Empty);
                posModel.Journal.Add(string.Empty);
            }
        
            if (fRecord.Count() > 3)
            {
                int tmpCount = fRecord.Count() - 3;
                posModel.Journal.AddFormat("        other  {0}", tmpCount.ToString());

            }
            else
            {
                posModel.Journal.Add(string.Empty);
            }
        }

        public void getDiscountString(PosModel posModel, ItemList fRecord)
        {
           
            TranNode tranNode = posModel.TranModel.TranNode;

            if (fRecord.SubtotalRemainderDiscount > 0)
            {
              
                posModel.Journal.AddFormat("                 ROUND DC{0, 15}",
                    tranNode.ItemList.SubtotalRemainderDiscount * fRecord.GetLast().Sign);
            }

            //----------------------------------------------------------------------------------
            // ITEM
            if (posModel.TranModel.TranNode.DiscountAmount > 0)
            {
                if (posModel.TranModel.TranNode.ItemList.CheckInsurance() == null) // 보험요율적용인 경우는 Discount로 표시하지 않고 TranJournal에서 별도로 표시함.
                {
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        posModel.Journal.AddLine();
                        string strDcAmt = string.Format("{0:###,###,###0.00}", posModel.TranModel.TranNode.DiscountAmount * posModel.TranModel.TranNode.Sign);
                        posModel.Journal.AddFormat("TOTAL DISCOUNT AMT {0, 13}", strDcAmt);
                    }
                    else
                    {
                        posModel.Journal.AddLine();
                        string strDcAmt = string.Format("{0:###,###,###0.00}", posModel.TranModel.TranNode.DiscountAmount * posModel.TranModel.TranNode.Sign);
                        posModel.Journal.AddFormat("TOTAL DISCOUNT AMOUNT {0, 13}", strDcAmt);
                    }
                }
            }
            //----------------------------------------------------------------------------------

            if (tranNode.ItemList.SubtotalDiscountFlag.Equals("R"))
            {
              posModel.Journal.AddFormat("               PAYMENT DC{0,2}%{1,12}",
                    tranNode.ItemList.SubtotalDiscountRate,
                    tranNode.ItemList.SubtotalDiscountAmount * fRecord.GetLast().Sign);
            }
            else if (tranNode.ItemList.SubtotalDiscountFlag.Equals("D"))
            {
              posModel.Journal.AddFormat("               PAYMENT DC{0, 15}",
                    tranNode.ItemList.SubtotalDiscountAmount * fRecord.GetLast().Sign);
            }
        }

        public void getSlipString(PosModel posModel, ItemList fRecord)
        {
            TranNode tranNode = posModel.TranModel.TranNode;

            if (fRecord.SubtotalRemainderDiscount > 0)
            {
             posModel.Journal.AddFormat("                 ROUND DC{0, 15}",
                    tranNode.ItemList.SubtotalRemainderDiscount * fRecord.GetLast().Sign);
            }

            // ITEM
            if (posModel.TranModel.TranNode.DiscountAmount > 0)
            {
              posModel.Journal.AddFormat("ITEM DC TOT         {0, 15}",
                    posModel.TranModel.TranNode.DiscountAmount * posModel.TranModel.TranNode.Sign);
            }
            
            if (tranNode.ItemList.SubtotalDiscountFlag.Equals("R"))
            {
             
                posModel.Journal.AddFormat("               PAYMENT DC{0,2}%{1,12}",
                    tranNode.ItemList.SubtotalDiscountRate,
                    tranNode.ItemList.SubtotalDiscountAmount * fRecord.GetLast().Sign);
            }
            
            else if (tranNode.ItemList.SubtotalDiscountFlag.Equals("D"))
            {
             posModel.Journal.AddFormat("               PAYMENT DC{0, 15}",
                    tranNode.ItemList.SubtotalDiscountAmount * fRecord.GetLast().Sign);
            }

            if ((posModel.TranModel.TranNode.DiscountAmount == 0) && (tranNode.ItemList.SubtotalDiscountFlag.Equals("N")))
            {
                posModel.Journal.AddFormat("               DISC  Amt {0, 15}", 0);
            }
        }

        public void getTaxString(PosModel posModel, ItemList fRecord)
        {
            TranNode tranNode = posModel.TranModel.TranNode;

            // JINIT_201911, double totAmount = tranNode.Total - tranNode.VatAmount;  
            double totAmount = tranNode.Subtotal - tranNode.VatAmount;       
            double vatAmount = tranNode.VatAmount;                          
            //double freeAmount = tranNode.NetAmount;                           

            posModel.Journal.AddLine();

            if (UIManager.Instance().Is58mmPrinter)
            {
                //if (totAmount != 0)
                //{
                double totDspAmount = tranNode.Subtotal;
                Models.tran.ItemNode checkItemNode = posModel.TranModel.TranNode.ItemList.CheckInsurance();
                if (checkItemNode != null) totDspAmount = tranNode.Subtotal + tranNode.InsuranceDiscountAmount;
                posModel.Journal.Add("bold",Journal.JournalUtil.lpad(17, "TOTAL") + Journal.JournalUtil.lpad(15, (totDspAmount * tranNode.Sign)));
                posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (tranNode.NetAmount * tranNode.Sign)));
                posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL B-16.00%") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagBAmount * tranNode.Sign)));
                posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (vatAmount * tranNode.Sign)));
                posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL E") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagEAmount * tranNode.Sign)));
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT) {
                    posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL D") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagDAmount * tranNode.Sign)));
                    posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (0 * tranNode.Sign)));
                }
                else
                {
                }
                posModel.Journal.Add(Journal.JournalUtil.lpad(17, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (vatAmount * tranNode.Sign)));
                //}
            }
            else
            {
                //if (totAmount != 0)
                //{
                double totDspAmount = tranNode.Subtotal;
                Models.tran.ItemNode checkItemNode = posModel.TranModel.TranNode.ItemList.CheckInsurance();
                if (checkItemNode != null) totDspAmount = tranNode.Subtotal + tranNode.InsuranceDiscountAmount;
                
                posModel.Journal.Add("bold", Journal.JournalUtil.lpad(20, "TOTAL") + Journal.JournalUtil.lpad(15, (totDspAmount * tranNode.Sign)));
                posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL A-EX") + Journal.JournalUtil.lpad(15, (tranNode.NetAmount * tranNode.Sign)));
                var taxb = tranNode.TaxFlagBAmount * tranNode.Sign;
                if(taxb > 0)
                {
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, (vatAmount * tranNode.Sign)));

                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagBAmount * tranNode.Sign)- (vatAmount * tranNode.Sign)));

                }else
                {
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-B") + Journal.JournalUtil.lpad(15, 0.00));

                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, 0.00));

                }
                // posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL B-16%") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagBAmount * tranNode.Sign)));

                var taxe = tranNode.TaxFlagEAmount * tranNode.Sign;

                if(taxe > 0)
                {
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, (vatAmount * tranNode.Sign)));
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagEAmount * tranNode.Sign)- (vatAmount * tranNode.Sign)));

                }else
                {
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-E") + Journal.JournalUtil.lpad(15, 0.00));
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL E-8%") + Journal.JournalUtil.lpad(15, 0.00));

                }
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT)
                {
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL D") + Journal.JournalUtil.lpad(15, (tranNode.TaxFlagDAmount * tranNode.Sign)));
                    posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX-D") + Journal.JournalUtil.lpad(15, (0 * tranNode.Sign)));
                }
                else
                {
                }
                posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (vatAmount * tranNode.Sign)));
                    /*
                    posModel.Journal.AddFormat("TOTAL               {0, 15}", tranNode.Subtotal * tranNode.Sign);
                    posModel.Journal.AddFormat("TOTAL A-EX          {0, 15}", 0.00);
                    posModel.Journal.AddFormat("TOTAL B-18%         {0, 15}", tranNode.Subtotal * tranNode.Sign);
                    posModel.Journal.AddFormat("TOTAL TAX-B         {0, 15}", vatAmount * tranNode.Sign);
                    posModel.Journal.AddFormat("TOTAL TAX           {0, 15}", vatAmount * tranNode.Sign);
                    */
                //}
            }
        }

        public void getItemNodeString(PosModel posModel, int seq, ItemNode fRecord)
        {
            getItemNodeString(posModel, seq, fRecord, true);
        }

        public void getItemNodeString(PosModel posModel, int seq, ItemNode fRecord, bool dcflag)
        {
            posModel.Journal.Add("", fRecord.ItemName);
            posModel.Journal.Add("", fRecord.ItemCode);
            //posModel.Journal.AddFormat("{0,-20}{1,15}",(double)(fRecord.Price * fRecord .Sign) + "x" + fRecord.Quantity, fRecord.Subtotal * fRecord.Sign);

            string taxName = "";
            switch (fRecord.TaxFlag)
            {
                case "A":
                    taxName = "A-EX ";
                    break;
                case "B":
                    taxName = "B-16%";
                    break;
                case "C":
                    taxName = "TAX C";
                    break;
                case "D":
                    taxName = "TAX D";
                    break;
                case "E":
                    taxName = "TAX E-8%";
                    break;
                default:
                    break;
            }

            string txtPrcie = string.Format("{0:###,###,###0.00}", fRecord.Price * fRecord.Sign);
            //string txtSubtotal = string.Format("{0:###,###,###0.00}", fRecord.Subtotal * fRecord.Sign);
            string txtTotal = string.Format("{0:###,###,###0.00}", fRecord.Total * fRecord.Sign);
            //double price = (double)(fRecord.Price * fRecord.Sign);
            //double subtotal = (double)(fRecord.Subtotal * fRecord.Sign);

            //if (UIManager.Instance().Is58mmPrinter)
            //{
            //    posModel.Journal.AddFormat("{0,-12}{1,3}{2,17}", txtPrcie + "x", fRecord.Quantity, txtTotal + taxName);
            //}
            //else
            //{
            //    posModel.Journal.AddFormat("{0,-15}{1,3}{2,17}", txtPrcie + "x", fRecord.Quantity, txtTotal + taxName);
            //}
            if (UIManager.Instance().Is58mmPrinter)
            {
                posModel.Journal.AddFormat("{0,-15}{1,17}", txtPrcie + "x" + fRecord.Quantity, txtTotal + taxName);
            }
            else
            {
                posModel.Journal.AddFormat("{0,-18}{1,17}", txtPrcie + "x" + fRecord.Quantity, txtTotal + taxName);
            }

            //posModel.Journal.Add(Journal.JournalUtil.lpad(20, "TOTAL TAX") + Journal.JournalUtil.lpad(15, (vatAmount * tranNode.Sign)));

            if (fRecord.DiscountAmount != 0)
            {
                //string strDiscountAmount = string.Format("{0:###,###,###0.00}", fRecord.DiscountAmount * fRecord.Sign);
                //posModel.Journal.AddFormat("{0,-18}{1,17}", "DISCOUNT AMOUNT", strDiscountAmount + "     ");

                string strDiscountAmount = string.Format("{0:#,###,###0.00}", fRecord.DiscountAmount * fRecord.Sign);
                string strSubtotal = string.Format("{0:###,###,###0.00}", fRecord.Subtotal * fRecord.Sign);
                if (fRecord.IsrcAplcbYn.Equals("Y"))
                {
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        posModel.Journal.AddFormat("{0,10} {1,-8} {2,12}", strDiscountAmount, "INS:" + fRecord.InsurerRate + "%", strSubtotal); 
                    }
                    else
                    {
                        posModel.Journal.AddFormat("{0,12} {1,-8} {2,18}", strDiscountAmount, "INS:" + fRecord.InsurerRate + "%", strSubtotal + "     ");
                    }
                }
                else
                {
                    if (UIManager.Instance().Is58mmPrinter)
                    {
                        posModel.Journal.AddFormat("{0,10} {1,-8} {2,12}", strDiscountAmount, "D/C:"+ fRecord .DiscountRate + "%", strSubtotal);
                    }
                    else
                    {
                        posModel.Journal.AddFormat("{0,12} {1,-8} {2,18}", strDiscountAmount, "D/C:"+ fRecord.DiscountRate + "%", strSubtotal + "     ");
                    }
                }
            }
        }
    }
}

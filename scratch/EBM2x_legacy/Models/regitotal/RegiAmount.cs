using EBM2x.Models.config;
using EBM2x.Models.tran;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.regitotal
{
    public class RegiAmount
    {
        public RegiAmount()
        {
        }

        public void excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if(posModel.TranModel.TranInformation.LogFlag == TranDefine.LOG_TRAN)
            {
                updateItemList(posModel.RegiTotal.RegiDetail, posModel.TranModel.TranNode);
                updateTenderList(posModel.RegiTotal.RegiDetail, posModel.TranModel.TranNode);

                updateItemList(posModel.OperTotal.RegiDetail, posModel.TranModel.TranNode);
                updateTenderList(posModel.OperTotal.RegiDetail, posModel.TranModel.TranNode);
            }
        }

        public void updateItemList(RegiDetail regiDetail, TranNode tranNode)
        {
            regiDetail.TotalAmount.Count += 1;
            regiDetail.TotalAmount.Amount += tranNode.Total * tranNode.Sign;
            regiDetail.TotalAmount.SubtotalAmount += tranNode.Subtotal * tranNode.Sign;
            regiDetail.TotalAmount.DiscountAmount += tranNode.DiscountAmount * tranNode.Sign;

            regiDetail.PureAmount.Count += 1;
            regiDetail.PureAmount.Amount += tranNode.Total * tranNode.Sign;
            regiDetail.PureAmount.SubtotalAmount += tranNode.Subtotal * tranNode.Sign;
            regiDetail.PureAmount.DiscountAmount += tranNode.DiscountAmount * tranNode.Sign;

            if (tranNode.DiscountAmount > 0)
            {
                regiDetail.DiscountAmount.Count += 1;
                regiDetail.DiscountAmount.Amount += tranNode.DiscountAmount * tranNode.Sign;
                regiDetail.DiscountAmount.SubtotalAmount += tranNode.DiscountAmount * tranNode.Sign;
                regiDetail.DiscountAmount.DiscountAmount += tranNode.DiscountAmount * tranNode.Sign;
            }

            // public UnitTotal NormalSale { get; set; }        
            if (tranNode.Sign > 0)
            {
                regiDetail.NormalSale.Count += 1;
                regiDetail.NormalSale.Amount += tranNode.Total * tranNode.Sign;
                regiDetail.NormalSale.SubtotalAmount += tranNode.Subtotal * tranNode.Sign;
                regiDetail.NormalSale.DiscountAmount += tranNode.DiscountAmount * tranNode.Sign;
            }
            // public UnitTotal ReturnSale { get; set; }        
            else
            {
                regiDetail.ReturnSale.Count += 1;
                regiDetail.ReturnSale.Amount += tranNode.Total * tranNode.Sign;
                regiDetail.ReturnSale.SubtotalAmount += tranNode.Subtotal * tranNode.Sign;
                regiDetail.ReturnSale.DiscountAmount += tranNode.DiscountAmount * tranNode.Sign;
            }

            for (int i = 0; i < tranNode.ItemList.Count(); i++)
            {
                ItemNode itemNode = tranNode.ItemList.Get(i);

                updateClassList(regiDetail, itemNode);
            }
        }
        public void updateClassList(RegiDetail regiDetail, ItemNode itemNode)
        {
            bool update = false;
            for (int i = 0; i < regiDetail.ClassTotalList.Count(); i++)
            {
                ClassTotal classTotal = regiDetail.ClassTotalList.Get(i);
                if(classTotal.ClassCode.Equals(itemNode.ClassCode))
                {
                    update = true;

                    classTotal.Count += (int)(itemNode.Quantity * itemNode.Sign);               // 건수
                    classTotal.Amount += itemNode.Total * itemNode.Sign;                        // 합계금액
                    classTotal.SubtotalAmount += itemNode.Subtotal * itemNode.Sign;             // 합계금액
                    classTotal.DiscountAmount += itemNode.DiscountAmount * itemNode.Sign;       // 에누리금액

                    break;
                }
            }
            if(!update)
            {
                ClassTotal classTotal = new ClassTotal();

                classTotal.ClassCode = itemNode.ClassCode;
                classTotal.ClassName = itemNode.ClassName;
                classTotal.Count = (int)(itemNode.Quantity * itemNode.Sign);               // 건수
                classTotal.Amount = itemNode.Total * itemNode.Sign;                        // 합계금액
                classTotal.SubtotalAmount = itemNode.Subtotal * itemNode.Sign;             // 합계금액
                classTotal.DiscountAmount = itemNode.DiscountAmount * itemNode.Sign;       // 에누리금액

                regiDetail.ClassTotalList.Add(classTotal);
            }
        }
        public void updateTenderList(RegiDetail regiDetail, TranNode tranNode)
        {
            for (int i = 0; i < tranNode.TenderList.Count(); i++)
            {
                TenderNode tenderNode = tranNode.TenderList.Get(i);

                if (tenderNode.TenderFlag.Equals("Cash"))
                {
                    if (tenderNode.Sign > 0)
                    {
                        regiDetail.CashSale.Count += 1;
                        regiDetail.CashSale.Amount += tenderNode.SubtotalAmount * tenderNode.Sign;
                        regiDetail.CashSale.SubtotalAmount += tenderNode.SubtotalAmount * tenderNode.Sign;
                        regiDetail.CashSale.DiscountAmount += 0;
                    }
                    else
                    {
                        regiDetail.CashReturn.Count += 1;
                        regiDetail.CashReturn.Amount += tenderNode.SubtotalAmount * tenderNode.Sign;
                        regiDetail.CashReturn.SubtotalAmount += tenderNode.SubtotalAmount * tenderNode.Sign;
                        regiDetail.CashReturn.DiscountAmount += 0;
                    }
                }
                else if (tenderNode.TenderFlag.Equals("Credit"))
                {
                    regiDetail.Credit.Count += 1;
                    regiDetail.Credit.Amount += tenderNode.SubtotalAmount * tenderNode.Sign;
                    regiDetail.Credit.SubtotalAmount += tenderNode.SubtotalAmount * tenderNode.Sign;
                    regiDetail.Credit.DiscountAmount += 0;
                }
                else if (tenderNode.TenderFlag.Equals("Debit"))
                {
                    regiDetail.Debit.Count += 1;
                    regiDetail.Debit.Amount += tenderNode.SubtotalAmount * tenderNode.Sign;
                    regiDetail.Debit.SubtotalAmount += tenderNode.SubtotalAmount * tenderNode.Sign;
                    regiDetail.Debit.DiscountAmount += 0;
                }
                else if (tenderNode.TenderFlag.Equals("Mobile Wallet"))
                {
                    regiDetail.MobileWallet.Count += 1;
                    regiDetail.MobileWallet.Amount += tenderNode.SubtotalAmount * tenderNode.Sign;
                    regiDetail.MobileWallet.SubtotalAmount += tenderNode.SubtotalAmount * tenderNode.Sign;
                    regiDetail.MobileWallet.DiscountAmount += 0;
                }

            }
        }
    }
}

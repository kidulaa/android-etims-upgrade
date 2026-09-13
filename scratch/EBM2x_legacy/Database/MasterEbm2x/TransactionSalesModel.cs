using EBM2x.Database.Master;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EBM2x.Database.MasterEbm2x
{
    public class TransactionSalesModel
    {
        public TrnsSaleRecord TranRecord { get; set; }
        public List<TrnsSaleItemRecord> ItemRecords { get; set; }

        public TrnsSaleItemRecord CurrentItemRecord { get; set; }

        public TransactionSalesModel()
        {
            TranRecord = new TrnsSaleRecord();
            ItemRecords = new List<TrnsSaleItemRecord>();
        }

        public double GetTotDCAmount()
        {
            double count = 0;

            for (int j = 0; j < ItemRecords.Count; j++)
            {
                    count += ItemRecords[j].DcAmt;
            }
            return count;
        }

        public void InitModel(string Tin, string BhfId, long InvcNo, string SalesDt, string SalesTyCd, string RcptTyCd, string UserId, string UserName)
        {
            TranRecord.Tin = Tin;
            TranRecord.BhfId = BhfId;
            //JCNA 202001 DELETE TranRecord.DvcId = DvcId;
            TranRecord.InvcNo = InvcNo;
            TranRecord.SalesDt = SalesDt;
            TranRecord.SalesTyCd = SalesTyCd;
            TranRecord.PrchrAcptcYn = "N";
            TranRecord.SalesTyCd = SalesTyCd;
            TranRecord.RcptTyCd = RcptTyCd;        // S : Sale, R : Refund  
            TranRecord.PmtTyCd = "01";             // (07: 01(CASH))
            TranRecord.SalesSttsCd = "01";         // (11: 01(Wait for Release), 02(Released), 03(Cancel Requested), 04(Canceled), 05(Refunded))

            TranRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            TranRecord.RegrId = UserId;
            TranRecord.RegrNm = UserName;
            TranRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
            TranRecord.ModrId = UserId;
            TranRecord.ModrNm = UserName;
        }

        public void Delete(TrnsSaleItemRecord record)
        {
            ItemRecords.Remove(record);
        }

        public void DeleteAll()
        {
            ItemRecords.Clear();
        }

        public void SetCurrentItem(TaxpayerItemRecord itemNode)
        {
            CurrentItemRecord = new TrnsSaleItemRecord();
            CurrentItemRecord.ItemCd = itemNode.ItemCd;
            CurrentItemRecord.ItemNm = itemNode.ItemNm;
            CurrentItemRecord.ItemClsCd = itemNode.ItemClsCd;
            CurrentItemRecord.ItemClsNm = itemNode.ItemClsName;

            CurrentItemRecord.PkgUnitCd = itemNode.PkgUnitCd;
            CurrentItemRecord.Pkg = 0;
            CurrentItemRecord.QtyUnitCd = itemNode.QtyUnitCd;
            CurrentItemRecord.TaxTyCd = itemNode.TaxTyCd;

            CurrentItemRecord.Bcd = itemNode.Bcd;
            CurrentItemRecord.Prc = itemNode.DftPrc;
            CurrentItemRecord.Qty = 0;
            CurrentItemRecord.SplyAmt = 0;
            CurrentItemRecord.TotAmt = CurrentItemRecord.SplyAmt * CurrentItemRecord.Qty;
        }

        public void ConfirmCurrentItem()
        {
            ItemRecords.Add(CurrentItemRecord);
            CurrentItemRecord = null;
            CalculateTran();
        }
        public void CalculateCurrentItem()
        {
            CalculateItem(CurrentItemRecord);
        }

        public void CalculateItem(TrnsSaleItemRecord itemNode)
        {
            if (itemNode != null)
            {  
                itemNode.SplyAmt = Math.Round(itemNode.Prc * itemNode.Qty, 2);
                // JINIT_201911
                //itemNode.DcAmt = itemNode.SplyAmt * (itemNode.DcRt / 100);
                itemNode.DcAmt = (itemNode.SplyAmt * itemNode.DcRt) / 100; // 할인단가
                
                itemNode.TaxblAmt = itemNode.SplyAmt - itemNode.DcAmt;
                if (itemNode.TaxTyCd.Equals("B"))
                {
                    double vatRate = 16;
                    itemNode.TaxAmt = (itemNode.TaxblAmt / 1.16) * (vatRate / 100);
                    itemNode.TaxAmt = Math.Round(itemNode.TaxAmt, 2);
                }else if(itemNode.TaxTyCd.Equals("E"))
                {
                    double vatRate = 8;
                    itemNode.TaxAmt = (itemNode.TaxblAmt / 1.08) * (vatRate / 100);
                    itemNode.TaxAmt = Math.Round(itemNode.TaxAmt, 2);
                }
                else
                {
                    itemNode.TaxAmt = 0;
                }
                
                itemNode.TotAmt = itemNode.TaxblAmt;
            }
        }

        public void CalculateTran()
        {
            TranRecord.TotItemCnt = ItemRecords.Count;        // Total Item Count
            TranRecord.TaxblAmtA = 0;                         // Taxable Amount A
            TranRecord.TaxblAmtB = 0;                         // Taxable Amount B
            TranRecord.TaxblAmtC = 0;                         // Taxable Amount C
            TranRecord.TaxblAmtD = 0;                         // Taxable Amount D
            TranRecord.TaxblAmtE = 0;                         // Taxable Amount E
            TranRecord.TaxRtA = 0;                            // Tax Rate A
            TranRecord.TaxRtB = 0;                            // Tax Rate B
            TranRecord.TaxRtC = 0;                            // Tax Rate C
            TranRecord.TaxRtD = 0;                            // Tax Rate D
            TranRecord.TaxRtE = 0;                            // Tax Rate E
            TranRecord.TaxAmtA = 0;                           // Tax Amount A
            TranRecord.TaxAmtB = 0;                           // Tax Amount B
            TranRecord.TaxAmtC = 0;                           // Tax Amount C
            TranRecord.TaxAmtD = 0;                           // Tax Amount D
            TranRecord.TaxAmtE = 0;                           // Tax Amount E
            TranRecord.TotTaxblAmt = 0;                       // Total Taxable Amount
            TranRecord.TotTaxAmt = 0;                         // Total Tax Amount
            TranRecord.TotAmt = 0;                            // Total Amount

            TrnsSaleItemRecord itemRecord;
            for (int i = 0; i < ItemRecords.Count; i++)
            {
                itemRecord = ItemRecords[i];
                itemRecord.Tin = TranRecord.Tin;
                itemRecord.BhfId = TranRecord.BhfId;
                itemRecord.InvcNo = TranRecord.InvcNo;
                itemRecord.ItemSeq = i + 1;

                itemRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                itemRecord.ModrId = TranRecord.ModrId;
                itemRecord.ModrNm = TranRecord.ModrNm;
                itemRecord.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                itemRecord.RegrId = TranRecord.RegrId;
                itemRecord.RegrNm = TranRecord.RegrNm;

                if (itemRecord.TaxTyCd.Equals("A"))
                {
                    TranRecord.TaxblAmtA += itemRecord.TaxblAmt;      // Taxable Amount A
                    TranRecord.TaxRtA = 0;                            // Tax Rate A
                    TranRecord.TaxAmtA += itemRecord.TaxAmt;          // Tax Amount A
                }
                else if (itemRecord.TaxTyCd.Equals("B"))
                {
                    TranRecord.TaxblAmtB += itemRecord.TaxblAmt;       // Taxable Amount B
                    TranRecord.TaxRtB = 16;                            // Tax Rate B
                    TranRecord.TaxAmtB += itemRecord.TaxAmt;           // Tax Amount B
                }
                else if (itemRecord.TaxTyCd.Equals("C"))
                {
                    TranRecord.TaxblAmtC += itemRecord.TaxblAmt;      // Taxable Amount C
                    TranRecord.TaxRtC = 0;                            // Tax Rate C
                    TranRecord.TaxAmtC += itemRecord.TaxAmt;          // Tax Amount C
                }
                else if (itemRecord.TaxTyCd.Equals("D"))
                {
                    TranRecord.TaxblAmtD += itemRecord.TaxblAmt;      // Taxable Amount D
                    TranRecord.TaxRtD = 0;                            // Tax Rate D
                    TranRecord.TaxAmtD += itemRecord.TaxAmt;          // Tax Amount D
                }

                else if (itemRecord.TaxTyCd.Equals("E"))
                {
                    TranRecord.TaxblAmtE += itemRecord.TaxblAmt;       // Taxable Amount B
                    TranRecord.TaxRtE = 8;                            // Tax Rate B
                    TranRecord.TaxAmtE += itemRecord.TaxAmt;
                }

                TranRecord.TotTaxblAmt += itemRecord.TaxblAmt;        // Total Taxable Amount
                TranRecord.TotTaxAmt += itemRecord.TaxAmt;            // Total Tax Amount
                TranRecord.TotAmt += itemRecord.TotAmt;               // Total Amount
            }

            // 20200228 JCNA
            TranRecord.TotTaxAmt = Math.Round(TranRecord.TotTaxAmt, 2);
            TranRecord.TaxAmtB = Math.Round(TranRecord.TaxAmtB, 2);
        }
        public ObservableCollection<TrnsSaleItemRecord> GetObservableCollection()
        {
            ObservableCollection<TrnsSaleItemRecord> observableCollection = new ObservableCollection<TrnsSaleItemRecord>();

            try
            {
                for (int i = 0; i < ItemRecords.Count; i++)
                {
                    observableCollection.Add(ItemRecords[i]);
                }
            }
            catch
            {
            }

            return observableCollection;
        }

        public bool IsExist(string itemCode)
        {
            try
            {
                for (int i = 0; i < ItemRecords.Count; i++)
                {
                    if (ItemRecords[i].ItemCd.Equals(itemCode)) return true;
                }
            }
            catch
            {
            }

            return false;
        }

        // JINIT_20191208,
        public int GetItemCount()
        {
            if (ItemRecords == null) return 0;
            return ItemRecords.Count;
        }
    }
}

using EBM2x.Database.Master;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class TransactionSalesReceitpModel
    {
        public TrnsSaleReceiptRecord TranRecord { get; set; }
        public List<TrnsSaleItemRecord> ItemRecords { get; set; }

        public TrnsSaleItemRecord CurrentItemRecord { get; set; }

        public TransactionSalesReceitpModel()
        {
            TranRecord = new TrnsSaleReceiptRecord();
            ItemRecords = new List<TrnsSaleItemRecord>();
        }

        public void InitModel(string Tin, string BhfId, long InvcNo, string SalesDt, string SalesTyCd, string RcptTyCd, string UserId, string UserName)
        {
            TranRecord.Tin = Tin;
            TranRecord.BhfId = BhfId;
            TranRecord.InvcNo = InvcNo;
            TranRecord.PrchrAcptcYn = "N";

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
        }
        public void CalculateCurrentItem()
        {
            CalculateItem(CurrentItemRecord);
        }

        public void CalculateItem(TrnsSaleItemRecord itemNode)
        {
            if (itemNode != null)
            {
                // 공급가 = 단가 * 수량
                itemNode.SplyAmt = itemNode.Prc * itemNode.Qty;
                // 할인액 = 공급가 * 할인율
                itemNode.DcAmt = itemNode.SplyAmt * (itemNode.DcRt / 100);
                // 할인적용 공급가(실 공급가) = 공급가 - 할인액
                itemNode.TaxblAmt = itemNode.SplyAmt - itemNode.DcAmt;
                // 부가세 = 할인적용 공급가(실 공급가) * 부가세율
                if (itemNode.TaxTyCd.Equals("B"))
                {
                    double vatRate = 16;
                    itemNode.TaxAmt = (itemNode.TaxblAmt / 1.16) * (vatRate / 100);
                    itemNode.TaxAmt = Math.Round(itemNode.TaxAmt, 2);
                }
                else if(itemNode.TaxTyCd.Equals("E"))
                {
                    double vatRate = 8;
                    itemNode.TaxAmt = (itemNode.TaxblAmt / 1.08) * (vatRate / 100);
                    itemNode.TaxAmt = Math.Round(itemNode.TaxAmt, 2);
                }
                else
                {
                    itemNode.TaxAmt = 0;
                }
                // 합계 = 할인적용 공급가(실 공급가) + 부가세액
                itemNode.TotAmt = itemNode.TaxblAmt;
            }
        }

        public void CalculateTran()
        {
            TrnsSaleItemRecord itemRecord;
            for (int i = 0; i < ItemRecords.Count; i++)
            {
                itemRecord = ItemRecords[i];
                itemRecord.Tin = TranRecord.Tin;
                itemRecord.BhfId = TranRecord.BhfId;
                itemRecord.InvcNo = TranRecord.InvcNo;
                itemRecord.ItemSeq = i + 1;

            }
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
    }
}

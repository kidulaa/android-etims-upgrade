using EBM2x.Database.Master;
using EBM2x.Database.TableIO;
using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class OpeningCloseingStockMaster : ModelIO
    {
        public List<OpeningCloseingStockRecord> getOpeningCloseingStockTable(string StartDate, string EndDate, string likeValue)
        {
            List<OpeningCloseingStockRecord> arrayList = new List<OpeningCloseingStockRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("  select TBL1.TIN    ");
            sql.Append("      ,  TBL1.bhf_id       ");
            sql.Append("      ,  TBL1.item_cd      ");
            sql.Append("      ,  TBL1.item_nm         ");
            sql.Append("      ,  TBL1.opening_stock      ");
            sql.Append("      ,  TBL1.closeing_stock     ");
            sql.Append("      ,  TBL1.current_stock     ");
            //sql.Append("      ,  IFNULL((SELECT DFLT_DL_UNTPC FROM ITMITEM WHERE ITEM_CD = TBL1.ITEM_CD),0) AS default_unit_price     ");
            sql.Append("      ,  CASE WHEN TBL1.closeing_stock = 0 THEN 0 ELSE ROUND ( TBL1.total_amount / TBL1.closeing_stock ) END  AS unit_price     ");
            sql.Append("      ,  TBL1.total_amount        ");
            sql.Append("  from (       ");
            sql.Append("  SELECT VB1.TIN, VB1.bhf_id      ");
            sql.Append("      ,  VB1.item_cd      ");
            sql.Append("      ,  VB1.item_nm      ");
            sql.Append("      ,  SUM ( CASE WHEN VB0.OCRN_DT < @StartDate       ");
            sql.Append("               THEN VB1.qty * CASE WHEN VB0.SAR_TY_CD IN ( '01' ,'02' ,'04' ,'05' ,'06' ) THEN 1 ELSE -1 END     ");
            sql.Append("               ELSE 0      ");
            sql.Append("               END      ");
            sql.Append("             ) AS opening_stock         ");
            sql.Append("      ,  SUM ( CASE WHEN VB0.OCRN_DT <= @EndDate       ");
            sql.Append("               THEN VB1.qty * CASE WHEN VB0.SAR_TY_CD IN ( '01' ,'02' ,'04' ,'05' ,'06' ) THEN 1 ELSE -1 END     ");
            sql.Append("               ELSE 0       ");
            sql.Append("               END       ");
            sql.Append("             ) AS closeing_stock         ");
            sql.Append("      ,  SUM ( VB1.qty * CASE WHEN VB0.SAR_TY_CD IN ( '01' ,'02' ,'04' ,'05' ,'06' ) THEN 1 ELSE -1 END      ");
            sql.Append("             ) AS current_stock        ");
            sql.Append("      ,  SUM ( CASE WHEN VB0.OCRN_DT BETWEEN @StartDate AND @EndDate      ");
            sql.Append("               THEN VB1.tot_amt * CASE WHEN VB0.SAR_TY_CD IN ( '01' ,'02' ,'04' ,'05' ,'06' ) THEN 1 ELSE -1 END      ");
            sql.Append("               ELSE 0       ");
            sql.Append("               END      ");
            sql.Append("             ) AS total_amount      ");
            sql.Append("    FROM STOCK_IO VB0     ");
            sql.Append("       , STOCK_IO_ITEM VB1      ");
            sql.Append("       , TAXPAYER_ITEM VB2      ");
            sql.Append("   WHERE VB0.SAR_NO = VB1.SAR_NO       ");
            sql.Append("     AND VB0.bhf_id = VB1.bhf_id      ");
            sql.Append("     AND VB0.TIN = VB2.TIN         ");
            sql.Append("     AND VB1.item_cd = VB2.item_cd         ");
            sql.Append("     AND VB2.ITEM_TY_CD IN ( '1', '2' )    ");
            sql.Append("     AND VB2.USE_YN = 'Y'    ");
            if (!string.IsNullOrEmpty(likeValue)) sql.Append("   and (VB1.item_cd like @likeValue or VB1.item_nm like @likeValue or VB2.BCD like @likeValue )");
            sql.Append("   GROUP BY VB1.TIN, VB1.bhf_id, VB1.item_cd, VB1.item_nm       ");
            sql.Append("  ) as TBL1        ");

            TaxpayerItemMaster taxpayerItemMaster = new TaxpayerItemMaster();
            TaxpayerItemRecord taxpayerItemRecord = new TaxpayerItemRecord();
            StockMasterMaster StockMasterMaster = new StockMasterMaster();
            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;
                
                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@EndDate";
                param.Value = EndDate;
                command.Parameters.Add(param);
                
                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    OpeningCloseingStockRecord record = new OpeningCloseingStockRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.ItemCd = reader.GetString(2);               // Item Code
                    if (!reader.IsDBNull(3)) record.ItemNm = reader.GetString(3);               // Item Name
                    if (!reader.IsDBNull(4)) record.OpeningQty = reader.GetDouble(4);           // Quantity
                    if (!reader.IsDBNull(5)) record.ClosingQty = reader.GetDouble(5);           // Quantity
                    if (!reader.IsDBNull(6)) record.RsdQty = reader.GetDouble(6);               // Resodual Quantity
                    //if (!reader.IsDBNull(7)) record.Prc = reader.GetDouble(7);                // Price
                    //if (!reader.IsDBNull(8)) record.TotAmt = reader.GetDouble(8);             // Total Amount

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (OpeningCloseingStockRecord record in arrayList)
            {
                record.RsdQty = StockMasterMaster.GetCurrentStock(record.Tin, record.ItemCd); // 현재고 수량

                taxpayerItemMaster.ToRecord(taxpayerItemRecord, record.Tin, record.ItemCd, UIManager.Instance().PosModel.Environment.EnvPosSetup.NonVAT);
                record.Prc = taxpayerItemRecord.DftPrc;
                record.TotAmt = record.Prc * record.RsdQty;
                // JCNA 20191211
                //if (record.Prc < 0) record.Prc = record.Prc * (-1);

                // JCNA 20200123
                record.SftyQty = taxpayerItemRecord.SftyQty;
                if (record.RsdQty < record.SftyQty)
                {
                    record.TextColor = "cc0000";
                }
            }

            return arrayList;
        }
    }
}

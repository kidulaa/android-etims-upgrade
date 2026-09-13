using EBM2x.Database.Master;
using EBM2x.Database.TableIO;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class StockStatusMaster : ModelIO
    {
        public List<OpeningCloseingStockRecord> getStockStatusTable(string StartDate, string EndDate, string likeValue)
        {
            List<OpeningCloseingStockRecord> arrayList = new List<OpeningCloseingStockRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();

            sql.Append("      SELECT TB0.TIN  ");
            sql.Append("      ,      TB0.bhf_id     ");
            sql.Append("      ,      TB1.item_cd      ");
            sql.Append("      ,      TB1.item_nm      ");
            sql.Append("      ,      CASE WHEN tb2.item_ty_cd IN ('2', '3') then SUM(TB1.qty * (CASE WHEN TB0.SAR_TY_CD IN ( '01' ,'02' ,'04' ,'05' ,'06' ) THEN 1 ELSE -1 END)) else 0 end AS finished_product ");
            sql.Append("      ,      CASE WHEN tb2.item_ty_cd IN ('1') then SUM(TB1.qty * (CASE WHEN TB0.SAR_TY_CD IN ( '01' ,'02' ,'04' ,'05' ,'06' ) THEN 1 ELSE -1 END)) else 0 end AS raw_material ");
            sql.Append("      FROM STOCK_IO TB0       ");
            sql.Append("      ,    STOCK_IO_ITEM TB1      ");
            sql.Append("      ,    TAXPAYER_ITEM TB2      ");
            sql.Append("      WHERE  TB0.SAR_NO = TB1.SAR_NO       ");
            sql.Append("        AND  TB0.bhf_id = TB1.bhf_id         ");
            sql.Append("        AND  TB0.TIN = TB2.TIN         ");
            sql.Append("        AND  TB0.OCRN_DT BETWEEN @StartDate AND @EndDate       ");
            sql.Append("        AND  TB1.item_cd = TB2.item_cd         ");
            sql.Append("        AND  TB2.ITEM_TY_CD IN ( '1', '2' )    ");
            sql.Append("        AND  TB2.USE_YN = 'Y'    ");
            if (!string.IsNullOrEmpty(likeValue)) sql.Append("   and (TB1.item_cd like @likeValue or TB1.item_nm like @likeValue or TB2.BCD like @likeValue )");
            sql.Append("      GROUP BY TB0.TIN, TB0.bhf_id, TB1.item_cd, TB1.item_nm  ");

            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();
            TrnsPurchaseItemMaster trnsPurchaseItemMaster = new TrnsPurchaseItemMaster();
            TaxpayerItemCompositionMaster itemCompositionMaster = new TaxpayerItemCompositionMaster();
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
                    if (!reader.IsDBNull(4)) record.FinishedProduct = reader.GetDouble(4);      // JINIT_20191210, GetDouble, FinishedProduct
                    //if (!reader.IsDBNull(5)) record.ExpirDt = reader.GetString(5);              // ExpirDt
                    if (!reader.IsDBNull(5)) record.RawMaterial = reader.GetDouble(5);          // RawMaterial

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
                record.ExpirDt = StockIoItemMaster.GetExpirationDate(record.Tin, record.ItemCd); 
                record.RdsQty = StockMasterMaster.GetCurrentStock(record.Tin, record.ItemCd); // 현재고 수량
            }

            return arrayList;
        }
    }
}

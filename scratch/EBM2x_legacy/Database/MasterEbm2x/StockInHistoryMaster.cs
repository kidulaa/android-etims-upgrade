using EBM2x.Database.TableIO;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class StockInHistoryMaster : ModelIO
    {
        public List<OpeningCloseingStockRecord> getStockInHistoryTable(string StartDate, string EndDate, string likeValue)
        {
            List<OpeningCloseingStockRecord> arrayList = new List<OpeningCloseingStockRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("              SELECT  TB1.TIN         ");
            sql.Append("              ,       TB1.bhf_id       ");
            sql.Append("              ,       TB1.item_cd       ");
            sql.Append("              ,       TB1.item_nm       ");
            sql.Append("              ,       SUM ( TB1.purchse_qty )       AS purchse_qty        ");
            sql.Append("              ,       CASE WHEN SUM ( TB1.purchse_qty ) = 0 THEN 0 ELSE ROUND ( SUM ( TB1.purchse_amt ) / SUM ( TB1.purchse_qty ) ) END  AS purchse_price      ");
            sql.Append("              ,       SUM ( TB1.purchse_amt )       AS purchse_amt       ");
            sql.Append("              ,       SUM ( TB1.information_qty )       AS information_qty            ");
            sql.Append("              ,       CASE WHEN SUM ( TB1.information_qty ) = 0 THEN 0 ELSE ROUND ( SUM ( TB1.information_amt ) / SUM ( TB1.information_qty ) ) END  AS nformation_price        ");
            sql.Append("              ,       SUM ( TB1.information_amt )       AS information_amt        ");
            sql.Append("              ,       SUM ( TB1.adjust_qty )       AS adjust_qty        ");
            sql.Append("              ,       SUM ( TB1.processing_qty )       AS processing_qty       ");
            sql.Append("              ,       SUM ( TB1.shipment_qty )       AS shipment_qty        ");
            sql.Append("              FROM ( SELECT  VB0.TIN, VB0.bhf_id        ");
            sql.Append("                     ,       VB1.item_cd      ");
            sql.Append("                     ,       VB1.item_nm       ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '02' ,'03' )       ");
            sql.Append("                                THEN VB1.qty * CASE WHEN VB0.SAR_TY_CD = '02' THEN 1 ELSE -1 END       ");
            sql.Append("                                ELSE 0          ");
            sql.Append("                             END                                                                AS purchse_qty       ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '02' ,'03' )            ");
            sql.Append("                                THEN VB1.tot_amt * CASE WHEN VB0.SAR_TY_CD = '02' THEN 1 ELSE -1 END      ");
            sql.Append("                                ELSE 0          ");
            sql.Append("                             END                                                               AS purchse_amt          ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '01' ) THEN VB1.qty ELSE 0 END       AS information_qty      ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '01' ) THEN VB1.tot_amt ELSE 0 END   AS information_amt     ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '06' ) THEN VB1.qty ELSE 0 END       AS adjust_qty          ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '05' ) THEN VB1.qty ELSE 0 END       AS processing_qty      ");
            sql.Append("                     ,       CASE WHEN VB0.SAR_TY_CD IN ( '04' ) THEN VB1.qty ELSE 0 END       AS shipment_qty        ");
            sql.Append("                     FROM STOCK_IO VB0                ");
            sql.Append("                     ,    STOCK_IO_ITEM VB1           ");
            sql.Append("                     ,    TAXPAYER_ITEM VB2      ");
            sql.Append("                     WHERE  VB0.SAR_NO = VB1.SAR_NO       ");
            sql.Append("                       AND  VB0.bhf_id = VB1.bhf_id        ");
            sql.Append("                       AND  VB0.TIN = VB2.TIN         ");
            sql.Append("                       AND  VB1.item_cd = VB2.item_cd         ");
            sql.Append("                       AND  VB0.OCRN_DT BETWEEN @StartDate AND @EndDate       ");
            sql.Append("                       AND  VB0.SAR_TY_CD IN ( '04' ,'01' ,'02' ,'03' ,'06' ,'05' )     ");
            sql.Append("                       AND  VB2.ITEM_TY_CD IN ( '1', '2' )    ");
            sql.Append("                       AND  VB2.USE_YN = 'Y'    ");
            if (!string.IsNullOrEmpty(likeValue)) sql.Append("   and (VB1.item_cd like @likeValue or VB1.item_nm like @likeValue or VB2.BCD like @likeValue )");
            sql.Append("                    ) TB1          ");
            sql.Append("              GROUP BY TB1.TIN, TB1.bhf_id, TB1.item_cd, TB1.item_nm        ");

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
                    if (!reader.IsDBNull(4)) record.PurchaseQty = reader.GetDouble(4);
                    if (!reader.IsDBNull(5)) record.PurchasePrice = reader.GetDouble(5);
                    if (!reader.IsDBNull(6)) record.PurchaseTotalAmount = reader.GetDouble(6);
                    if (!reader.IsDBNull(7)) record.ImportationQty = reader.GetDouble(7);
                    if (!reader.IsDBNull(8)) record.ImportationPrice = reader.GetDouble(8);
                    if (!reader.IsDBNull(9)) record.ImportationTotalAmount = reader.GetDouble(9);
                    if (!reader.IsDBNull(10)) record.AdjusmentInQty = reader.GetDouble(10);
                    if (!reader.IsDBNull(11)) record.ProcessingInQty = reader.GetDouble(11);
                    if (!reader.IsDBNull(12)) record.ShipmentReceivedQty = reader.GetDouble(12);

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
    }
}

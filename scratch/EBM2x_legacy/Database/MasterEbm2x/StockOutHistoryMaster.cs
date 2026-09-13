using EBM2x.Database.TableIO;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class StockOutHistoryMaster : ModelIO
    {
        public List<OpeningCloseingStockRecord> getStockOutHistoryTable(string StartDate, string EndDate, string likeValue)
        {
            List<OpeningCloseingStockRecord> arrayList = new List<OpeningCloseingStockRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("          SELECT  TB1.TIN     ");
            sql.Append("          ,       TB1.bhf_id       ");
            sql.Append("          ,       TB1.item_cd      ");
            sql.Append("          ,       TB1.item_nm      ");
            sql.Append("          ,       SUM ( TB1.sale_qty )                                                                                      AS sale_qty           ");
            sql.Append("          ,       CASE WHEN SUM ( TB1.sale_qty ) = 0 THEN 0 ELSE ROUND ( SUM ( TB1.sale_amt ) / SUM ( TB1.sale_qty ) ) END  AS sale_price          ");
            sql.Append("          ,       SUM ( TB1.sale_amt )                                                                                      AS sale_amt           ");
            sql.Append("          ,       SUM ( TB1.shipment_qty )                                                                                  AS shipment_qty       ");
            sql.Append("          ,       SUM ( TB1.adjust_qty )                                                                                    AS adjust_qty         ");
            sql.Append("          ,       SUM ( TB1.dammaged_qty )                                                                                  AS dammaged_qty      ");
            sql.Append("          ,       SUM ( TB1.processing_qty )                                                                                AS processing_qty    ");
            sql.Append("          FROM ( SELECT  VB0.TIN, VB0.bhf_id             ");
            sql.Append("                 ,       VB1.item_cd           ");
            sql.Append("                 ,       VB1.item_nm          ");
            sql.Append("                 ,       CASE WHEN VB0.SAR_TY_CD IN ( '11' ,'12' )          ");
            sql.Append("                            THEN VB1.qty * CASE WHEN VB0.SAR_TY_CD = '11' THEN 1 ELSE -1 END        ");
            sql.Append("                            ELSE 0         ");
            sql.Append("                         END                                                                AS sale_qty     ");
            sql.Append("                 ,       CASE WHEN VB0.SAR_TY_CD IN ( '11' ,'12' )       ");
            sql.Append("                            THEN VB1.tot_amt * CASE WHEN VB0.SAR_TY_CD = '11' THEN 1 ELSE -1 END     ");
            sql.Append("                            ELSE 0          ");
            sql.Append("                         END                                                               AS sale_amt      ");
            sql.Append("                 ,       CASE WHEN VB0.SAR_TY_CD IN ( '13' ) THEN VB1.qty ELSE 0 END       AS shipment_qty     ");
            sql.Append("                 ,       CASE WHEN VB0.SAR_TY_CD IN ( '16' ) THEN VB1.qty ELSE 0 END       AS adjust_qty        ");
            sql.Append("                 ,       CASE WHEN VB0.SAR_TY_CD IN ( '15' ) THEN VB1.qty ELSE 0 END       AS dammaged_qty      ");
            sql.Append("                 ,       CASE WHEN VB0.SAR_TY_CD IN ( '14' ) THEN VB1.qty ELSE 0 END       AS processing_qty    ");
            sql.Append("                 FROM STOCK_IO VB0       ");
            sql.Append("                 ,    STOCK_IO_ITEM VB1     ");
            sql.Append("                     ,    TAXPAYER_ITEM VB2      ");
            sql.Append("                 WHERE  VB0.SAR_NO = VB1.SAR_NO       ");
            sql.Append("                   AND  VB0.bhf_id = VB1.bhf_id       ");
            sql.Append("                   AND  VB0.TIN = VB2.TIN         ");
            sql.Append("                   AND  VB1.item_cd = VB2.item_cd         ");
            sql.Append("                   AND  VB0.OCRN_DT BETWEEN @StartDate AND @EndDate       ");
            sql.Append("                   AND  VB0.SAR_TY_CD IN ( '13' ,'11' ,'12' ,'16' ,'14' ,'15' )       ");
            sql.Append("                   AND  VB2.ITEM_TY_CD IN ( '1', '2' )    ");
            sql.Append("                   AND  VB2.USE_YN = 'Y'    ");
            if (!string.IsNullOrEmpty(likeValue)) sql.Append("   and (VB1.item_cd like @likeValue or VB1.item_nm like @likeValue or VB2.BCD like @likeValue )");
            sql.Append("                ) TB1          ");
            sql.Append("          GROUP BY TB1.TIN, TB1.bhf_id, TB1.item_cd, TB1.item_nm      ");

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
                    if (!reader.IsDBNull(4)) record.SalesQty = reader.GetDouble(4);
                    if (!reader.IsDBNull(5)) record.SalesPrice = reader.GetDouble(5);
                    if (!reader.IsDBNull(6)) record.SalesTotalAmount = reader.GetDouble(6);
                    if (!reader.IsDBNull(7)) record.ShipmentOutQty = reader.GetDouble(7);
                    if (!reader.IsDBNull(8)) record.AdjustmentOutQty = reader.GetDouble(8);
                    if (!reader.IsDBNull(9)) record.DammagedExpiredQty = reader.GetDouble(9);
                    if (!reader.IsDBNull(10)) record.ProcessingOutQty = reader.GetDouble(10);

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

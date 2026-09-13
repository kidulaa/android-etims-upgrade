using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of StockIoItemMaster.
    /// </summary>
    public class StockIoItemMaster : ModelIO
    {
        public List<StockIoItemRecord> getStockIoItemTable(string valTin, string valBhfId, string valSarNo)
        {
            List<StockIoItemRecord> arrayList = new List<StockIoItemRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockIoItemTable stockIoItemTable = new StockIoItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(stockIoItemTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND SAR_NO = @SAR_NO ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = valBhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SAR_NO";
                param.Value = valSarNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockIoItemRecord record = new StockIoItemRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.SarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.ItemSeq = reader.GetInt16(3);               // Item Sequence
                    if (!reader.IsDBNull(4)) record.ItemCd = reader.GetString(4);               // Item Code
                    if (!reader.IsDBNull(5)) record.ItemClsCd = reader.GetString(5);            // Item Classification Code
                    if (!reader.IsDBNull(6)) record.ItemNm = reader.GetString(6);               // Item Name
                    if (!reader.IsDBNull(7)) record.Bcd = reader.GetString(7);                  // Barcode
                    if (!reader.IsDBNull(8)) record.PkgUnitCd = reader.GetString(8);            // Package unit code
                    if (!reader.IsDBNull(9)) record.Pkg = reader.GetDouble(9);                  // Package
                    if (!reader.IsDBNull(10)) record.QtyUnitCd = reader.GetString(10);          // Quantity Unit Code
                    if (!reader.IsDBNull(11)) record.Qty = reader.GetDouble(11);                // Quantity
                    if (!reader.IsDBNull(12)) record.ItemExprDt = reader.GetString(12); // Item Expiration Date
                    if (!reader.IsDBNull(13)) record.Prc = reader.GetDouble(13);                // Price
                    if (!reader.IsDBNull(14)) record.SplyAmt = reader.GetDouble(14);            // Supply Amount
                    if (!reader.IsDBNull(15)) record.TotDcAmt = reader.GetDouble(15);           // Total Discount Amount
                    if (!reader.IsDBNull(16)) record.TaxblAmt = reader.GetDouble(16);           // Taxable Amount
                    if (!reader.IsDBNull(17)) record.TaxTyCd = reader.GetString(17);            // Taxation Type Code
                    if (!reader.IsDBNull(18)) record.TaxAmt = reader.GetDouble(18);             // Tax Amount
                    if (!reader.IsDBNull(19)) record.TotAmt = reader.GetDouble(19);             // Total Amount
                    if (!reader.IsDBNull(20)) record.RegrId = reader.GetString(20);             // Registrant ID
                    if (!reader.IsDBNull(21)) record.RegrNm = reader.GetString(21);             // Registrant Name
                    if (!reader.IsDBNull(22)) record.RegDt = reader.GetString(22);      // Registered Date
                    if (!reader.IsDBNull(23)) record.ModrId = reader.GetString(23);             // Modifier ID
                    if (!reader.IsDBNull(24)) record.ModrNm = reader.GetString(24);             // Modifier Name
                    if (!reader.IsDBNull(25)) record.ModDt = reader.GetString(25);      // Modified Date

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

        public string GetExpirationDate(string valTin, string valItemCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return "";
            }

            StockIoItemTable stockIoItemTable = new StockIoItemTable();

            StringBuilder sql = new StringBuilder();
            sql.Append("select MAX(ITEM_EXPR_DT) from STOCK_IO_ITEM");
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND ITEM_CD = @ITEM_CD ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCd;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (string.IsNullOrEmpty(codeName)) return "";
                return codeName;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return "";
            }
        }

        public bool ToRecord(StockIoItemRecord record, string valTin, string valBhfId, string valSarNo, int valItemSeq)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoItemTable stockIoItemTable = new StockIoItemTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(stockIoItemTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND SAR_NO = @SAR_NO ");
            sql.Append("   AND ITEM_SEQ = @ITEM_SEQ ");


            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = valBhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SAR_NO";
                param.Value = valSarNo;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_SEQ";
                param.Value = valItemSeq;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.SarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.ItemSeq = reader.GetInt16(3);               // Item Sequence
                    if (!reader.IsDBNull(4)) record.ItemCd = reader.GetString(4);               // Item Code
                    if (!reader.IsDBNull(5)) record.ItemClsCd = reader.GetString(5);            // Item Classification Code
                    if (!reader.IsDBNull(6)) record.ItemNm = reader.GetString(6);               // Item Name
                    if (!reader.IsDBNull(7)) record.Bcd = reader.GetString(7);                  // Barcode
                    if (!reader.IsDBNull(8)) record.PkgUnitCd = reader.GetString(8);            // Package unit code
                    if (!reader.IsDBNull(9)) record.Pkg = reader.GetDouble(9);                  // Package
                    if (!reader.IsDBNull(10)) record.QtyUnitCd = reader.GetString(10);          // Quantity Unit Code
                    if (!reader.IsDBNull(11)) record.Qty = reader.GetDouble(11);                // Quantity
                    if (!reader.IsDBNull(12)) record.ItemExprDt = reader.GetString(12); // Item Expiration Date
                    if (!reader.IsDBNull(13)) record.Prc = reader.GetDouble(13);                // Price
                    if (!reader.IsDBNull(14)) record.SplyAmt = reader.GetDouble(14);            // Supply Amount
                    if (!reader.IsDBNull(15)) record.TotDcAmt = reader.GetDouble(15);           // Total Discount Amount
                    if (!reader.IsDBNull(16)) record.TaxblAmt = reader.GetDouble(16);           // Taxable Amount
                    if (!reader.IsDBNull(17)) record.TaxTyCd = reader.GetString(17);            // Taxation Type Code
                    if (!reader.IsDBNull(18)) record.TaxAmt = reader.GetDouble(18);             // Tax Amount
                    if (!reader.IsDBNull(19)) record.TotAmt = reader.GetDouble(19);             // Total Amount
                    if (!reader.IsDBNull(20)) record.RegrId = reader.GetString(20);             // Registrant ID
                    if (!reader.IsDBNull(21)) record.RegrNm = reader.GetString(21);             // Registrant Name
                    if (!reader.IsDBNull(22)) record.RegDt = reader.GetString(22);      // Registered Date
                    if (!reader.IsDBNull(23)) record.ModrId = reader.GetString(23);             // Modifier ID
                    if (!reader.IsDBNull(24)) record.ModrNm = reader.GetString(24);             // Modifier Name
                    if (!reader.IsDBNull(25)) record.ModDt = reader.GetString(25);      // Modified Date

                    reader.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool ToTable(StockIoItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoItemTable stockIoItemTable = new StockIoItemTable();

            try
            {
                command.Parameters.Clear();
                stockIoItemTable.SetParameters(command, record);

                command.CommandText = stockIoItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = stockIoItemTable.GetInsertSQL();
                    command.CommandType = CommandType.Text;

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        // JINIT_20191201,
        //public bool InsertTable(int sign, List<StockIoItemRecord> listTaxpayerItem)
        public bool InsertTable(int sign, List<StockIoItemRecord> listTaxpayerItem, long sarNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterMaster stockMasterMaster = new StockMasterMaster();
            StockIoItemTable stockIoItemTable = new StockIoItemTable();

            StockMasterRraSdcUpload stockMasterRraSdcUpload = new StockMasterRraSdcUpload();

            try
            {
                command.CommandText = stockIoItemTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                bool result = false;
                for (int i = 0; i < listTaxpayerItem.Count; i++)
                {
                    listTaxpayerItem[i].SarNo = sarNo; // JINIT_20191201
                    listTaxpayerItem[i].ItemSeq = i + 1;
                    command.Parameters.Clear();
                    stockIoItemTable.SetParameters(command, listTaxpayerItem[i]);
                    if (command.ExecuteNonQuery() >= 1)
                    {
                        // 재고 MASTER 반영 : STOCK_MASTER
                        StockMasterRecord stockRecord = new StockMasterRecord();
                        stockRecord.Tin = listTaxpayerItem[i].Tin;
                        stockRecord.BhfId = listTaxpayerItem[i].BhfId;
                        stockRecord.ItemCd = listTaxpayerItem[i].ItemCd;
                        // 재고 속성에 따라 재고 증가/감소
                        stockRecord.RsdQty = listTaxpayerItem[i].Qty * sign;
                        stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                        stockRecord.ModrId = listTaxpayerItem[i].ModrId;
                        stockRecord.ModrNm = listTaxpayerItem[i].ModrNm;
                        stockMasterMaster.UpdateStock(stockRecord);

                        //JCNA 20191204
                        stockMasterRraSdcUpload.SendStockMasterSave(stockRecord.Tin, stockRecord.BhfId, stockRecord.ItemCd);

                        result = true;
                        continue;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool InsertTableSDC(int sign, List<StockIoItemRecord> listTaxpayerItem, long sarNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockMasterMaster stockMasterMaster = new StockMasterMaster();
            StockIoItemTable stockIoItemTable = new StockIoItemTable();

            StockMasterRraSdcUpload stockMasterRraSdcUpload = new StockMasterRraSdcUpload();

            try
            {
                command.CommandText = stockIoItemTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                bool result = false;
                for (int i = 0; i < listTaxpayerItem.Count; i++)
                {
                    listTaxpayerItem[i].SarNo = sarNo; // JINIT_20191201
                    listTaxpayerItem[i].ItemSeq = i + 1;
                    command.Parameters.Clear();
                    stockIoItemTable.SetParametersSDC(command, listTaxpayerItem[i]);
                    if (command.ExecuteNonQuery() >= 1)
                    {
                        //  MASTER: STOCK_MASTER
                        StockMasterRecord stockRecord = new StockMasterRecord();
                        stockRecord.Tin = listTaxpayerItem[i].Tin;
                        stockRecord.BhfId = listTaxpayerItem[i].BhfId;
                        stockRecord.ItemCd = listTaxpayerItem[i].ItemCd;
                        stockRecord.RsdQty = listTaxpayerItem[i].Qty * sign;
                        stockRecord.ModDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                        stockRecord.ModrId = listTaxpayerItem[i].ModrId;
                        stockRecord.ModrNm = listTaxpayerItem[i].ModrNm;
                        stockMasterMaster.UpdateStock(stockRecord);

                        //JCNA 20191204
                        stockMasterRraSdcUpload.SendStockMasterSave(stockRecord.Tin, stockRecord.BhfId, stockRecord.ItemCd);

                        result = true;
                        continue;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool DeleteTable(StockIoItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StockIoItemTable stockIoItemTable = new StockIoItemTable();

            try
            {
                command.CommandText = stockIoItemTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                stockIoItemTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        //start update qty_unit_cd and pkg_unit_cd
        public bool UpdateItemPkgQty(string valpkgUnitCd, string valqtyUnitCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("update STOCK_IO_ITEM ");
            sql.Append(" set PKG_UNIT_CD = @PKG_UNIT_CD ");  // Item Expiration Date
            sql.Append(",QTY_UNIT_CD = @QTY_UNIT_CD ");
            sql.Append(" WHERE PKG_UNIT_CD=''");         // Taxpayer Identification Number(TIN)
            sql.Append("or QTY_UNIT_CD=''");
            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                command.Parameters.Clear();

                param = command.CreateParameter();
                param.ParameterName = "@PKG_UNIT_CD";
                param.Value = valpkgUnitCd;
                command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

                param = command.CreateParameter();
                param.ParameterName = "@QTY_UNIT_CD";
                param.Value = valqtyUnitCd;
                command.Parameters.Add(param);

                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        //end
    }
}

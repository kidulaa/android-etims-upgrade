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
    /// Description of TrnsPurchaseItemMaster.
    /// </summary>
    public class TrnsPurchaseItemMaster : ModelIO
    {
        public string GetExprDt(string valTin, string valBhfId, string valItemCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT ITEM_EXPR_DT FROM TRNS_PURCHASE_ITEM WHERE TIN = @TIN AND BHF_ID = @BHF_ID AND ITEM_CD = @ITEM_CD ORDER BY ITEM_EXPR_DT DESC";
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
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCd;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (codeName == null) codeName = "";
                return codeName;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return "";
            }
        }

        public List<TrnsPurchaseItemRecord> getTrnsPurchaseItemTable( string valTin, string valBhfId, string valSpplrTin, long valInvcNo)
        {
            List<TrnsPurchaseItemRecord> arrayList = new List<TrnsPurchaseItemRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsPurchaseItemTable trnsPurchaseItemTable = new TrnsPurchaseItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsPurchaseItemTable.GetSelectSQL());
            sql.Append(" WHERE INVC_NO = @INVC_NO ");
            //sql.Append(" WHERE TIN = @TIN ");
            //sql.Append("   AND BHF_ID = @BHF_ID ");
            //sql.Append("   AND SPPLR_TIN = @SPPLR_TIN ");
            //sql.Append("   AND INVC_NO = @INVC_NO ");

            CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
            ItemClassMaster itemClassMaster = new ItemClassMaster();
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
                param.ParameterName = "@SPPLR_TIN";
                param.Value = valSpplrTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@INVC_NO";
                param.Value = valInvcNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsPurchaseItemRecord record = new TrnsPurchaseItemRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.InvcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.SpplrTin = reader.GetString(3);             // Supplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(4)) record.ItemSeq = reader.GetInt16(4);               // Item Sequence
                    if (!reader.IsDBNull(5)) record.ItemCd = reader.GetString(5);               // Item Code
                    if (!reader.IsDBNull(6)) record.ItemClsCd = reader.GetString(6);            // Item Classification Code
                    if (!reader.IsDBNull(7)) record.ItemNm = reader.GetString(7);               // Item Name
                    if (!reader.IsDBNull(8)) record.Bcd = reader.GetString(8);                  // Barcode
                    if (!reader.IsDBNull(9)) record.SpplrItemClsCd = reader.GetString(9);       // Supplier Item Classification Code
                    if (!reader.IsDBNull(10)) record.SpplrItemCd = reader.GetString(10);        // Supplier Item Code
                    if (!reader.IsDBNull(11)) record.SpplrItemNm = reader.GetString(11);        // Supplier Item Name
                    if (!reader.IsDBNull(12)) record.PkgUnitCd = reader.GetString(12);          // Package Unit Code
                    if (!reader.IsDBNull(13)) record.Pkg = reader.GetDouble(13);                // Package
                    if (!reader.IsDBNull(14)) record.QtyUnitCd = reader.GetString(14);          // Quantity Unit Code
                    if (!reader.IsDBNull(15)) record.Qty = reader.GetDouble(15);                // Quantity
                    if (!reader.IsDBNull(16)) record.Prc = reader.GetDouble(16);                // Price
                    if (!reader.IsDBNull(17)) record.SplyAmt = reader.GetDouble(17);            // Supply Amount
                    if (!reader.IsDBNull(18)) record.DcRt = reader.GetDouble(18);               // Discount Rate
                    if (!reader.IsDBNull(19)) record.DcAmt = reader.GetDouble(19);              // Discount Amount
                    if (!reader.IsDBNull(20)) record.TaxblAmt = reader.GetDouble(20);           // Taxable Amount
                    if (!reader.IsDBNull(21)) record.TaxTyCd = reader.GetString(21);            // Taxation Type Code
                    if (!reader.IsDBNull(22)) record.TaxAmt = reader.GetDouble(22);             // Tax Amount
                    if (!reader.IsDBNull(23)) record.TotAmt = reader.GetDouble(23);             // Total Amount
                    if (!reader.IsDBNull(24)) record.ItemExprDt = reader.GetString(24);         // Item Expiration Date
                    if (!reader.IsDBNull(25)) record.RegrId = reader.GetString(25);             // Registrant ID
                    if (!reader.IsDBNull(26)) record.RegrNm = reader.GetString(26);             // Registrant Name
                    if (!reader.IsDBNull(27)) record.RegDt = reader.GetString(27);      // Registered Date
                    if (!reader.IsDBNull(28)) record.ModrId = reader.GetString(28);             // Modifier ID
                    if (!reader.IsDBNull(29)) record.ModrNm = reader.GetString(29);             // Modifier Name
                    if (!reader.IsDBNull(30)) record.ModDt = reader.GetString(30);      // Modified Date

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }
            foreach (TrnsPurchaseItemRecord record in arrayList)
            {
                record.TaxTyNm = codeDtlMaster.TaxTyName(record.TaxTyCd);                 // Taxation Type Name
            }
            return arrayList;
        }

        public bool ToRecord(TrnsPurchaseItemRecord record, string valTin, string valBhfId, string valSpplrTin, string valInvcNo, int valItemSeq)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseItemTable trnsPurchaseItemTable = new TrnsPurchaseItemTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(trnsPurchaseItemTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND SPPLR_TIN = @SPPLR_TIN ");
            sql.Append("   AND INVC_NO = @INVC_NO ");
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
                param.ParameterName = "@SPPLR_TIN";
                param.Value = valSpplrTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@INVC_NO";
                param.Value = valInvcNo;
                command.Parameters.Add(param); 
                
                param = command.CreateParameter();
                param.ParameterName = "@ITEM_SEQ";
                param.Value = valItemSeq;
                command.Parameters.Add(param);

                CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
                ItemClassMaster itemClassMaster = new ItemClassMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.InvcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.SpplrTin = reader.GetString(3);             // Supplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(4)) record.ItemSeq = reader.GetInt16(4);               // Item Sequence
                    if (!reader.IsDBNull(5)) record.ItemCd = reader.GetString(5);               // Item Code
                    if (!reader.IsDBNull(6)) record.ItemClsCd = reader.GetString(6);            // Item Classification Code
                    if (!reader.IsDBNull(7)) record.ItemNm = reader.GetString(7);               // Item Name
                    if (!reader.IsDBNull(8)) record.Bcd = reader.GetString(8);                  // Barcode
                    if (!reader.IsDBNull(9)) record.SpplrItemClsCd = reader.GetString(9);       // Supplier Item Classification Code
                    if (!reader.IsDBNull(10)) record.SpplrItemCd = reader.GetString(10);        // Supplier Item Code
                    if (!reader.IsDBNull(11)) record.SpplrItemNm = reader.GetString(11);        // Supplier Item Name
                    if (!reader.IsDBNull(12)) record.PkgUnitCd = reader.GetString(12);          // Package Unit Code
                    if (!reader.IsDBNull(13)) record.Pkg = reader.GetDouble(13);                // Package
                    if (!reader.IsDBNull(14)) record.QtyUnitCd = reader.GetString(14);          // Quantity Unit Code
                    if (!reader.IsDBNull(15)) record.Qty = reader.GetDouble(15);                // Quantity
                    if (!reader.IsDBNull(16)) record.Prc = reader.GetDouble(16);                // Price
                    if (!reader.IsDBNull(17)) record.SplyAmt = reader.GetDouble(17);            // Supply Amount
                    if (!reader.IsDBNull(18)) record.DcRt = reader.GetDouble(18);               // Discount Rate
                    if (!reader.IsDBNull(19)) record.DcAmt = reader.GetDouble(19);              // Discount Amount
                    if (!reader.IsDBNull(20)) record.TaxblAmt = reader.GetDouble(20);           // Taxable Amount
                    if (!reader.IsDBNull(21)) record.TaxTyCd = reader.GetString(21);            // Taxation Type Code
                    if (!reader.IsDBNull(22)) record.TaxAmt = reader.GetDouble(22);             // Tax Amount
                    if (!reader.IsDBNull(23)) record.TotAmt = reader.GetDouble(23);             // Total Amount
                    if (!reader.IsDBNull(24)) record.ItemExprDt = reader.GetString(24);         // Item Expiration Date
                    if (!reader.IsDBNull(25)) record.RegrId = reader.GetString(25);             // Registrant ID
                    if (!reader.IsDBNull(26)) record.RegrNm = reader.GetString(26);             // Registrant Name
                    if (!reader.IsDBNull(27)) record.RegDt = reader.GetString(27);      // Registered Date
                    if (!reader.IsDBNull(28)) record.ModrId = reader.GetString(28);             // Modifier ID
                    if (!reader.IsDBNull(29)) record.ModrNm = reader.GetString(29);             // Modifier Name
                    if (!reader.IsDBNull(30)) record.ModDt = reader.GetString(30);      // Modified Date

                    reader.Close();

                    record.TaxTyNm = codeDtlMaster.TaxTyName(record.TaxTyCd);                 // Taxation Type Name

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

        public bool ToTable(TrnsPurchaseItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseItemTable trnsPurchaseItemTable = new TrnsPurchaseItemTable();

            try
            {
                command.Parameters.Clear();
                trnsPurchaseItemTable.SetParameters(command, record);
 
                command.CommandText = trnsPurchaseItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = trnsPurchaseItemTable.GetInsertSQL();
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
        public bool InsertTable(List<TrnsPurchaseItemRecord> listTaxpayerItem)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseItemTable trnsPurchaseItemTable = new TrnsPurchaseItemTable();

            try
            {
                command.CommandText = trnsPurchaseItemTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                bool result = false;
                for (int i = 0; i < listTaxpayerItem.Count; i++)
                {
                    listTaxpayerItem[i].ItemSeq = i + 1;
                    command.Parameters.Clear();
                    trnsPurchaseItemTable.SetParameters(command, listTaxpayerItem[i]);
                    if (command.ExecuteNonQuery() >= 1)
                    {
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

        public bool DeleteTable(TrnsPurchaseItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseItemTable trnsPurchaseItemTable = new TrnsPurchaseItemTable();

            try
            {
                command.CommandText = trnsPurchaseItemTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsPurchaseItemTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        // JINIT_20191208, 전표자체를 삭제처리
        public bool DeleteTable(TrnsPurchaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseItemTable trnsPurchaseItemTable = new TrnsPurchaseItemTable();

            try
            {
                command.CommandText = trnsPurchaseItemTable.GetInvoiceNoDeleteSQL();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                command.Parameters.Clear();

                param = command.CreateParameter();
                param.ParameterName = "@Tin";
                param.Value = record.Tin;
                command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

                param = command.CreateParameter();
                param.ParameterName = "@BhfId";
                param.Value = record.BhfId;
                command.Parameters.Add(param);                  // Branch Office ID

                param = command.CreateParameter();
                param.ParameterName = "@InvcNo";
                param.Value = record.InvcNo;
                command.Parameters.Add(param);                  // Invoice No.

                param = command.CreateParameter();
                param.ParameterName = "@SpplrTin";
                param.Value = record.SpplrTin;
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
        public bool UpdateItemExprDt(TrnsPurchaseItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("update TRNS_PURCHASE_ITEM "); // 거래 구매 품목
            sql.Append("   set ITEM_EXPR_DT = @ItemExprDt ");  // Item Expiration Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                command.Parameters.Clear();

                param = command.CreateParameter();
                param.ParameterName = "@Tin";
                param.Value = record.Tin;
                command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

                param = command.CreateParameter();
                param.ParameterName = "@BhfId";
                param.Value = record.BhfId;
                command.Parameters.Add(param);                  // Branch Office ID

                param = command.CreateParameter();
                param.ParameterName = "@InvcNo";
                param.Value = record.InvcNo;
                command.Parameters.Add(param);                  // Invoice No.

                param = command.CreateParameter();
                param.ParameterName = "@ItemSeq";
                param.Value = record.ItemSeq;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ItemExprDt";
                param.Value = record.ItemExprDt;
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

        //start update qty_unit_cd and pkg_unit_cd
        public bool UpdateItemPkgQty(string valpkgUnitCd, string valqtyUnitCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("update TRNS_PURCHASE_ITEM "); // 거래 구매 품목
            sql.Append(" set PKG_UNIT_CD = @PKG_UNIT_CD ");  // Item Expiration Date
            sql.Append(",QTY_UNIT_CD = @QTY_UNIT_CD ");
            sql.Append(" WHERE PKG_UNIT_CD=''");
            sql.Append("or QTY_UNIT_CD=''");
            // Taxpayer Identification Number(TIN)
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

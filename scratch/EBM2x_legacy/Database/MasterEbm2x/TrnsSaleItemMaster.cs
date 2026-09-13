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
    /// Description of TrnsSaleItemMaster.
    /// </summary>
    public class TrnsSaleItemMaster : ModelIO
    {
        // JINIT_20191201,
        //public List<TrnsSaleItemRecord> getTrnsSaleItemTable(string valTin, string valBhfId, long valInvcNo)
        public List<TrnsSaleItemRecord> getTrnsSaleItemTable(string valTin, string valBhfId, long valInvcNo, string rcptTyCd)
        {
            List<TrnsSaleItemRecord> arrayList = new List<TrnsSaleItemRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleItemTable trnsSaleItemTable = new TrnsSaleItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleItemTable.GetSelectSQL());
            sql.Append(" WHERE INVC_NO = @INVC_NO ");
            //sql.Append(" WHERE TIN = @TIN ");
            //sql.Append("   AND BHF_ID = @BHF_ID ");
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
                param.ParameterName = "@INVC_NO";
                param.Value = valInvcNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSaleItemRecord record = new TrnsSaleItemRecord();

                    // JINIT_20191201, 
                    int sign = 1;
                    if (rcptTyCd == "R") sign = -1;

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.InvcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.ItemSeq = reader.GetInt16(3);               // Item Sequence
                    if (!reader.IsDBNull(4)) record.ItemCd = reader.GetString(4);               // Item Code
                    if (!reader.IsDBNull(5)) record.IsrccCd = reader.GetString(5);              // Insurance Company Code
                    if (!reader.IsDBNull(6)) record.ItemClsCd = reader.GetString(6);            // Item Classification Code
                    if (!reader.IsDBNull(7)) record.ItemNm = reader.GetString(7);               // Item Name
                    if (!reader.IsDBNull(8)) record.Bcd = reader.GetString(8);                  // Barcode
                    if (!reader.IsDBNull(9)) record.PkgUnitCd = reader.GetString(9);            // Package Unit Code
                    if (!reader.IsDBNull(10)) record.Pkg = Math.Round(reader.GetDouble(10), 2);                // Package
                    if (!reader.IsDBNull(11)) record.QtyUnitCd = reader.GetString(11);          // Quantity Unit Code
                    //Added By Bright 5.6.2022 From  round 2 digits to 4 digits
                    //if (!reader.IsDBNull(12)) record.Qty = Math.Round(reader.GetDouble(12) * sign, 2);         // Quantity
                    //if (!reader.IsDBNull(13)) record.Prc = Math.Round(reader.GetDouble(13), 2);                // Unit Price

                    if (!reader.IsDBNull(12)) record.Qty = Math.Round(reader.GetDouble(12) * sign, 4);         // Quantity
                    if (!reader.IsDBNull(13)) record.Prc = Math.Round(reader.GetDouble(13), 4);                // Unit Price

                    //End  By Bright 5.6.2022
                    if (!reader.IsDBNull(14)) record.SplyAmt = Math.Round(reader.GetDouble(14), 2);            // Supply Price
                    if (!reader.IsDBNull(15)) record.DcRt = reader.GetInt16(15);                // Discount Rate
                    if (!reader.IsDBNull(16)) record.DcAmt = Math.Round(reader.GetDouble(16) * sign, 2);       // JINIT_20191201, Discount Amount
                    if (!reader.IsDBNull(17)) record.IsrccNm = reader.GetString(17);            // Insurance Company Name
                    if (!reader.IsDBNull(18)) record.IsrcRt = reader.GetInt16(18);              // Insurance Rate
                    if (!reader.IsDBNull(19)) record.IsrcAmt = Math.Round(reader.GetDouble(19) * sign, 2);     // JINIT_20191201, Insurance Amount
                    if (!reader.IsDBNull(20)) record.TaxTyCd = reader.GetString(20);            // Tax type code
                    if (!reader.IsDBNull(21)) record.TaxblAmt = Math.Round(reader.GetDouble(21) * sign, 2);    // JINIT_20191201, Taxable Amount
                    if (!reader.IsDBNull(22)) record.TaxAmt = Math.Round(reader.GetDouble(22) * sign, 2);      // JINIT_20191201, Tax Amount
                    if (!reader.IsDBNull(23)) record.TotAmt = Math.Round(reader.GetDouble(23) * sign, 2);      // JINIT_20191201, Total Amount
                    if (!reader.IsDBNull(24)) record.RegrId = reader.GetString(24);             // Registrant ID
                    if (!reader.IsDBNull(25)) record.RegrNm = reader.GetString(25);             // Registrant Name
                    if (!reader.IsDBNull(26)) record.RegDt = reader.GetString(26);              // Registered Date
                    if (!reader.IsDBNull(27)) record.ModrId = reader.GetString(27);             // Modifier ID
                    if (!reader.IsDBNull(28)) record.ModrNm = reader.GetString(28);             // Modifier Name
                    if (!reader.IsDBNull(29)) record.ModDt = reader.GetString(29);              // Modified Date

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TrnsSaleItemRecord record in arrayList)
            {
                record.TaxTyNm = codeDtlMaster.TaxTyName(record.TaxTyCd);                   // Taxation Type Name
            }

            return arrayList;
        }

        public bool ToRecord(TrnsSaleItemRecord record, string valTin, string valBhfId, long valInvcNo, int valItemSeq)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleItemTable trnsSaleItemTable = new TrnsSaleItemTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleItemTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
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
                    if (!reader.IsDBNull(3)) record.ItemSeq = reader.GetInt16(3);               // Item Sequence
                    if (!reader.IsDBNull(4)) record.ItemCd = reader.GetString(4);               // Item Code
                    if (!reader.IsDBNull(5)) record.IsrccCd = reader.GetString(5);              // Insurance Company Code
                    if (!reader.IsDBNull(6)) record.ItemClsCd = reader.GetString(6);            // Item Classification Code
                    if (!reader.IsDBNull(7)) record.ItemNm = reader.GetString(7);               // Item Name
                    if (!reader.IsDBNull(8)) record.Bcd = reader.GetString(8);                  // Barcode
                    if (!reader.IsDBNull(9)) record.PkgUnitCd = reader.GetString(9);            // Package Unit Code
                    if (!reader.IsDBNull(10)) record.Pkg = reader.GetDouble(10);                // Package
                    if (!reader.IsDBNull(11)) record.QtyUnitCd = reader.GetString(11);          // Quantity Unit Code
                    if (!reader.IsDBNull(12)) record.Qty = Math.Round(reader.GetDouble(12), 2);                // Quantity
                    if (!reader.IsDBNull(13)) record.Prc = Math.Round(reader.GetDouble(13), 2);                // Unit Price
                    if (!reader.IsDBNull(14)) record.SplyAmt = Math.Round(reader.GetDouble(14), 2);            // Supply Price
                    if (!reader.IsDBNull(15)) record.DcRt = reader.GetInt16(15);               // Discount Rate
                    if (!reader.IsDBNull(16)) record.DcAmt = Math.Round(reader.GetDouble(16), 2);              // Discount Amount
                    if (!reader.IsDBNull(17)) record.IsrccNm = reader.GetString(17);            // Insurance Company Name
                    if (!reader.IsDBNull(18)) record.IsrcRt = reader.GetInt16(18);              // Insurance Rate
                    if (!reader.IsDBNull(19)) record.IsrcAmt = Math.Round(reader.GetDouble(19), 2);            // Insurance Amount
                    if (!reader.IsDBNull(20)) record.TaxTyCd = reader.GetString(20);            // Tax type code
                    if (!reader.IsDBNull(21)) record.TaxblAmt = Math.Round(reader.GetDouble(21), 2);           // Taxable Amount
                    if (!reader.IsDBNull(22)) record.TaxAmt = Math.Round(reader.GetDouble(22), 2);             // Tax Amount
                    if (!reader.IsDBNull(23)) record.TotAmt = Math.Round(reader.GetDouble(23), 2);             // Total Amount
                    if (!reader.IsDBNull(24)) record.RegrId = reader.GetString(24);             // Registrant ID
                    if (!reader.IsDBNull(25)) record.RegrNm = reader.GetString(25);             // Registrant Name
                    if (!reader.IsDBNull(26)) record.RegDt = reader.GetString(26);      // Registered Date
                    if (!reader.IsDBNull(27)) record.ModrId = reader.GetString(27);             // Modifier ID
                    if (!reader.IsDBNull(28)) record.ModrNm = reader.GetString(28);             // Modifier Name
                    if (!reader.IsDBNull(29)) record.ModDt = reader.GetString(29);      // Modified Date

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

        public bool ToTable(TrnsSaleItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleItemTable trnsSaleItemTable = new TrnsSaleItemTable();

            try
            {
                command.Parameters.Clear();
                trnsSaleItemTable.SetParameters(command, record);

                command.CommandText = trnsSaleItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = trnsSaleItemTable.GetInsertSQL();
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
        // JINIT_20191208,
        //public bool InsertTable(List<TrnsSaleItemRecord> listTaxpayerItem)
        public bool InsertTable(TrnsSaleRecord trnsSaleRecord, List<TrnsSaleItemRecord> listTaxpayerItem)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleItemTable trnsSaleItemTable = new TrnsSaleItemTable();

            try
            {
                command.CommandText = trnsSaleItemTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                bool result = false;
                for (int i = 0; i < listTaxpayerItem.Count; i++)
                {
                    listTaxpayerItem[i].ItemSeq = i + 1;

                    // JINIT_20191208
                    listTaxpayerItem[i].InvcNo = trnsSaleRecord.InvcNo;

                    command.Parameters.Clear();
                    trnsSaleItemTable.SetParameters(command, listTaxpayerItem[i]);
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

        public bool DeleteTable(TrnsSaleItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleItemTable trnsSaleItemTable = new TrnsSaleItemTable();

            try
            {
                command.CommandText = trnsSaleItemTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsSaleItemTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        // JINIT_20191208
        public bool DeleteTable(TrnsSaleRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleItemTable trnsSaleItemTable = new TrnsSaleItemTable();

            try
            {
                command.CommandText = trnsSaleItemTable.GetInvoiceNoDeleteSQL();
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
            sql.Append("update TRNS_SALE_ITEM "); 
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

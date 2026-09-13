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
    /// Description of TaxpayerItemMaster.
    /// </summary>
    public class TaxpayerItemMaster : ModelIO
    {
        public string GenItemCode(TaxpayerItemRecord record, bool IsChecked)
        {
            string strItemCode = "";
            string strItemGroupCode = "";
            string strOrg = "";
            string strItemType = "";
            string strPackingUnit = "";
            string strPackingType = "";
            string strAdiInfo = "";

            if (record.OrgnNatCd.Length > 0) strOrg = record.OrgnNatCd;
            // Item Type
            if (record.ItemTyCd.Length > 0) strItemType = record.ItemTyCd;

            // Packing Unit
            if (record.PkgUnitCd.Length <= 0)
            {
            }
            else if (record.PkgUnitCd.Length == 2)
                strPackingUnit = record.PkgUnitCd + "X";
            else
                strPackingUnit = record.PkgUnitCd;

            // Qty Type
            if (record.QtyUnitCd.Length <= 0)
            {
            }
            else if (record.QtyUnitCd.Length == 2)
                strPackingType = record.QtyUnitCd + "X";
            else
                strPackingType = record.QtyUnitCd;

            strItemGroupCode = strItemType + strPackingUnit + strPackingType;
            if (IsChecked)
            {
                strAdiInfo = GetNextSerial(strItemGroupCode);
            }
            else
            {
                strAdiInfo = "0000000";
            }
            strItemCode = strOrg + strItemType + strPackingUnit + strPackingType + strAdiInfo;

            return strItemCode;
        }

        //check item ITEM_CLS_CD  added by Aime on 10.5.2022

        public bool GetItemClassCode(string itemClsCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return false;

            try
            {
                command.CommandText = "SELECT ITEM_CLS_CD FROM ITEM_CLASS WHERE ITEM_CLS_CD = @ITEM_CLS_CD  AND USE_YN = 'Y'";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CLS_CD";
                param.Value = itemClsCode;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (string.IsNullOrEmpty(codeName)) return false;
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        //end to check ITEM_CLS_CD added by Aime

        public List<TaxpayerItemRecord> getTaxpayerItemTable(string valueTin, string likeValue, string valueUseYn)
        {
            return getTaxpayerItemTable(valueTin, likeValue, valueUseYn, "", "" );
        }
        public List<TaxpayerItemRecord> getTaxpayerItemTable(string valueTin, string likeValue, string valueUseYn, string valueItemTy)
        {
            return getTaxpayerItemTable(valueTin, likeValue, valueUseYn, valueItemTy, "");
        }

        public List<TaxpayerItemRecord> getTaxpayerItemTable(string valueTin, string likeValue, string valueUseYn, string valueItemTy, string valueWithoutItemTy)
        {
            return getTaxpayerItemTable(valueTin, likeValue, valueUseYn, valueItemTy, valueWithoutItemTy, false);
        }

        public List<TaxpayerItemRecord> getTaxpayerItemTable(string valueTin, string likeValue, string valueUseYn, string valueItemTy, string valueWithoutItemTy, bool nonVat)
        {
            List<TaxpayerItemRecord> arrayList = new List<TaxpayerItemRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();
            ItemClassMaster ItemClassMaster = new ItemClassMaster();
            StockMasterMaster StockMasterMaster = new StockMasterMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemTable.GetSelectSQL());
            sql.Append(" where TIN = @TIN ");
            if (string.IsNullOrEmpty(likeValue))
            {
                if (!string.IsNullOrEmpty(valueUseYn)) sql.Append("   and USE_YN = @USE_YN");
                if (!string.IsNullOrEmpty(valueItemTy)) sql.Append("  and ITEM_TY_CD = @ITEM_TY_CD");
                if (!string.IsNullOrEmpty(valueWithoutItemTy)) sql.Append("  and ITEM_TY_CD <> @WITHOUT_ITEM_TY_CD");
                sql.Append(" order by mod_dt desc ");
                sql.Append(" LIMIT 20 ");
            }
            else
            {
                sql.Append("   and (ITEM_CD like @likeValue or ITEM_NM like @likeValue or BCD like @likeValue )");
                if (!string.IsNullOrEmpty(valueUseYn)) sql.Append("   and USE_YN = @USE_YN");
                if (!string.IsNullOrEmpty(valueItemTy)) sql.Append("  and ITEM_TY_CD = @ITEM_TY_CD");
                if (!string.IsNullOrEmpty(valueWithoutItemTy)) sql.Append("  and ITEM_TY_CD <> @WITHOUT_ITEM_TY_CD");
                sql.Append(" order by ITEM_CD ASC ");
            }

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valueTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@USE_YN";
                param.Value = valueUseYn;
                command.Parameters.Add(param);

                //**********************************
                //@ychan_20191208 �߰�
                param = command.CreateParameter();
                param.ParameterName = "@ITEM_TY_CD";
                param.Value = valueItemTy;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@WITHOUT_ITEM_TY_CD";
                param.Value = valueWithoutItemTy;
                command.Parameters.Add(param);
                //**********************************

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TaxpayerItemRecord record = new TaxpayerItemRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.ItemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.ItemClsCd = reader.GetString(2);            // Item Classification Code (RRA)
                    if (!reader.IsDBNull(3)) record.ItemTyCd = reader.GetString(3);             // Item Type Code
                    if (!reader.IsDBNull(4)) record.ItemNm = reader.GetString(4);               // Item Name
                    if (!reader.IsDBNull(5)) record.ItemStdNm = reader.GetString(5);            // Item Stand Name
                    if (!reader.IsDBNull(6)) record.OrgnNatCd = reader.GetString(6);            // Origin National Code
                    if (!reader.IsDBNull(7)) record.PkgUnitCd = reader.GetString(7);            // Package Unit Code
                    if (!reader.IsDBNull(8)) record.QtyUnitCd = reader.GetString(8);            // Quantity Unit Code
                    if (!reader.IsDBNull(9)) record.TaxTyCd = reader.GetString(9);              // Taxation Type Code
                    if (!reader.IsDBNull(10)) record.Bcd = reader.GetString(10);                // Barcode
                    if (!reader.IsDBNull(11)) record.RegBhfId = reader.GetString(11);           // Branch Office ID
                    if (!reader.IsDBNull(12)) record.UseYn = reader.GetString(12);              // Use(Y/N)
                    if (!reader.IsDBNull(13)) record.RraModYn = reader.GetString(13);           // RRA Modified(Y/N)
                    if (!reader.IsDBNull(14)) record.AddInfo = reader.GetString(14);            // Additional Information
                    if (!reader.IsDBNull(15)) record.SftyQty = reader.GetDouble(15);            // Safety Quantity
                    if (!reader.IsDBNull(16)) record.IsrcAplcbYn = reader.GetString(16);        // Insurance Appicable(Y/N)
                    if (!reader.IsDBNull(17)) record.DftPrc = reader.GetDouble(17);             // Default Price
                    if (!reader.IsDBNull(18)) record.GrpPrcL1 = reader.GetDouble(18);           // Group Default Price L1
                    if (!reader.IsDBNull(19)) record.GrpPrcL2 = reader.GetDouble(19);           // Group Default Price L2
                    if (!reader.IsDBNull(20)) record.GrpPrcL3 = reader.GetDouble(20);           // Group Default Price L3
                    if (!reader.IsDBNull(21)) record.GrpPrcL4 = reader.GetDouble(21);           // Group Default Price L4
                    if (!reader.IsDBNull(22)) record.GrpPrcL5 = reader.GetDouble(22);           // Group Default Price L5
                    if (!reader.IsDBNull(23)) record.RegrId = reader.GetString(23);             // Registrant ID
                    if (!reader.IsDBNull(24)) record.RegrNm = reader.GetString(24);             // Registrant Name
                    if (!reader.IsDBNull(25)) record.RegDt = reader.GetString(25);      // Registered Date
                    if (!reader.IsDBNull(26)) record.ModrId = reader.GetString(26);             // Modifier ID
                    if (!reader.IsDBNull(27)) record.ModrNm = reader.GetString(27);             // Modifier Name
                    if (!reader.IsDBNull(28)) record.ModDt = reader.GetString(28);      // Modified Date
                    if (!reader.IsDBNull(29)) record.InitlWhUntpc = reader.GetDouble(29);       // 초기 입고단가
                    if (!reader.IsDBNull(30)) record.InitlQty = reader.GetDouble(30);           // 초기 입고수량
                    if (!reader.IsDBNull(31)) record.Rm = reader.GetString(31);                 // 비고
                    if (!reader.IsDBNull(32)) record.UseBarcode = reader.GetString(32);         // 바코드사용여부
                    if (!reader.IsDBNull(33)) record.UseAdiYn = reader.GetString(33);           // 부가정보사용여부
                    if (!reader.IsDBNull(34)) record.BatchNum = reader.GetString(34);           // BatchNum
                    if (!reader.IsDBNull(35)) record.UseExpiration = reader.GetString(35);           // UseExpiration

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TaxpayerItemRecord record in arrayList)
            {
                // 202101 Non TAX : NonVAT 대상이며 TaxTyCd이 "B"이면 TaxTyCd을 "D"로 바꾼다.
                if (nonVat && record.TaxTyCd.Equals("B"))
                {
                    record.TaxTyCd = "D";
                }

                record.ExpirationDate = StockIoItemMaster.GetExpirationDate(record.Tin, record.ItemCd);
                //JCNA 202001 DELETE
                //if (!reader.IsDBNull(35)) record.ExpirationDtUse = reader.GetString(35);    // Expiration Dt Use

                record.OrgnNatName = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                record.ItemTyName = CodeDtlMaster.ItemTyName(record.ItemTyCd);              // Item Type Name
                record.PkgUnitName = CodeDtlMaster.PkgUnitName(record.PkgUnitCd);           // Package Unit Name
                record.QtyUnitName = CodeDtlMaster.QtyUnitName(record.QtyUnitCd);           // Quantity Unit Name
                record.TaxTyName = CodeDtlMaster.TaxTyName(record.TaxTyCd);                 // Taxation Type Name
                record.ItemClsName = ItemClassMaster.GetItemClassName(record.ItemClsCd);    // Item Classification Name (RRA)
                record.RdsQty = StockMasterMaster.GetCurrentStock(record.Tin, record.ItemCd);
            }

            return arrayList;
        }

        public string GetItemType(string valTin, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT ITEM_TY_CD FROM TAXPAYER_ITEM WHERE TIN = @TIN AND ITEM_CD = @ITEM_CD";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valCode;
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

        public string GetItemName(string valTin, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT ITEM_NM FROM TAXPAYER_ITEM WHERE TIN = @TIN AND ITEM_CD = @ITEM_CD";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valCode;
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
        public bool GetItemCode(string valTin, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return false;

            try
            {
                command.CommandText = "SELECT ITEM_CD FROM TAXPAYER_ITEM WHERE TIN = @TIN AND ITEM_CD = @ITEM_CD AND USE_YN = 'Y'";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valCode;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (string.IsNullOrEmpty(codeName)) return false;
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public string GetItemCodeWithBarcode(string valTin, string valCode)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT ITEM_CD FROM TAXPAYER_ITEM WHERE TIN = @TIN AND BCD = @BCD AND USE_YN = 'Y'";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BCD";
                param.Value = valCode;
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
        public string GetItemCodeWithBarcodeBatchNum(string valTin, string valCode, string valBatchNum)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT ITEM_CD FROM TAXPAYER_ITEM WHERE TIN = @TIN AND BCD = @BCD AND BATCH_NUM = @BATCH_NUM AND USE_YN = 'Y'";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BCD";
                param.Value = valCode;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BATCH_NUM";
                param.Value = valBatchNum;
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
        public bool ToRecord(TaxpayerItemRecord record, string valTin, string valItemCode, bool nonVat)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();
            ItemClassMaster ItemClassMaster = new ItemClassMaster();
            StockMasterMaster StockMasterMaster = new StockMasterMaster();
            StockIoItemMaster StockIoItemMaster = new StockIoItemMaster();

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemTable.GetSelectSQL());
//            sql.Append(" WHERE TIN = @TIN ");
            sql.Append(" WHERE ITEM_CD = @ITEM_CD ");

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
                param.Value = valItemCode;
                command.Parameters.Add(param);
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.ItemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.ItemClsCd = reader.GetString(2);            // Item Classification Code (RRA)
                    if (!reader.IsDBNull(3)) record.ItemTyCd = reader.GetString(3);             // Item Type Code
                    if (!reader.IsDBNull(4)) record.ItemNm = reader.GetString(4);               // Item Name
                    if (!reader.IsDBNull(5)) record.ItemStdNm = reader.GetString(5);            // Item Stand Name
                    if (!reader.IsDBNull(6)) record.OrgnNatCd = reader.GetString(6);            // Origin National Code
                    if (!reader.IsDBNull(7)) record.PkgUnitCd = reader.GetString(7);            // Package Unit Code
                    if (!reader.IsDBNull(8)) record.QtyUnitCd = reader.GetString(8);            // Quantity Unit Code
                    if (!reader.IsDBNull(9)) record.TaxTyCd = reader.GetString(9);              // Taxation Type Code
                    if (!reader.IsDBNull(10)) record.Bcd = reader.GetString(10);                // Barcode
                    if (!reader.IsDBNull(11)) record.RegBhfId = reader.GetString(11);           // Branch Office ID
                    if (!reader.IsDBNull(12)) record.UseYn = reader.GetString(12);              // Use(Y/N)
                    if (!reader.IsDBNull(13)) record.RraModYn = reader.GetString(13);           // RRA Modified(Y/N)
                    if (!reader.IsDBNull(14)) record.AddInfo = reader.GetString(14);            // Additional Information
                    if (!reader.IsDBNull(15)) record.SftyQty = reader.GetDouble(15);            // Safety Quantity
                    if (!reader.IsDBNull(16)) record.IsrcAplcbYn = reader.GetString(16);        // Insurance Appicable(Y/N)
                    if (!reader.IsDBNull(17)) record.DftPrc = reader.GetDouble(17);             // Default Price
                    if (!reader.IsDBNull(18)) record.GrpPrcL1 = reader.GetDouble(18);           // Group Default Price L1
                    if (!reader.IsDBNull(19)) record.GrpPrcL2 = reader.GetDouble(19);           // Group Default Price L2
                    if (!reader.IsDBNull(20)) record.GrpPrcL3 = reader.GetDouble(20);           // Group Default Price L3
                    if (!reader.IsDBNull(21)) record.GrpPrcL4 = reader.GetDouble(21);           // Group Default Price L4
                    if (!reader.IsDBNull(22)) record.GrpPrcL5 = reader.GetDouble(22);           // Group Default Price L5
                    if (!reader.IsDBNull(23)) record.RegrId = reader.GetString(23);             // Registrant ID
                    if (!reader.IsDBNull(24)) record.RegrNm = reader.GetString(24);             // Registrant Name
                    if (!reader.IsDBNull(25)) record.RegDt = reader.GetString(25);      // Registered Date
                    if (!reader.IsDBNull(26)) record.ModrId = reader.GetString(26);             // Modifier ID
                    if (!reader.IsDBNull(27)) record.ModrNm = reader.GetString(27);             // Modifier Name
                    if (!reader.IsDBNull(28)) record.ModDt = reader.GetString(28);      // Modified Date
                    if (!reader.IsDBNull(29)) record.InitlWhUntpc = reader.GetDouble(29);    
                    if (!reader.IsDBNull(30)) record.InitlQty = reader.GetDouble(30);      
                    if (!reader.IsDBNull(31)) record.Rm = reader.GetString(31);      
                    if (!reader.IsDBNull(32)) record.UseBarcode = reader.GetString(32);
                    if (!reader.IsDBNull(33)) record.UseAdiYn = reader.GetString(33);
                    if (!reader.IsDBNull(34)) record.BatchNum = reader.GetString(34);           // BatchNum
                    if (!reader.IsDBNull(35)) record.UseExpiration = reader.GetString(35);           // UseExpiration

                    reader.Close();

                    // 202101 Non TAX : NonVAT  TaxTyCd이 "B"TaxTyCd.
                    if (nonVat && record.TaxTyCd.Equals("B"))
                    {
                        record.TaxTyCd = "D";
                    }

                    record.ExpirationDate = StockIoItemMaster.GetExpirationDate(record.Tin, record.ItemCd);
                    //JCNA 202001 DELETE
                    //if (!reader.IsDBNull(35)) record.ExpirationDtUse = reader.GetString(35);    // Expiration Dt Use

                    record.OrgnNatName = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                    record.ItemTyName = CodeDtlMaster.ItemTyName(record.ItemTyCd);              // Item Type Name
                    record.PkgUnitName = CodeDtlMaster.PkgUnitName(record.PkgUnitCd);           // Package Unit Name
                    record.QtyUnitName = CodeDtlMaster.QtyUnitName(record.QtyUnitCd);           // Quantity Unit Name
                    record.TaxTyName = CodeDtlMaster.TaxTyName(record.TaxTyCd);                 // Taxation Type Name
                    record.ItemClsName = ItemClassMaster.GetItemClassName(record.ItemClsCd);    // Item Classification Name (RRA)
                    record.RdsQty = StockMasterMaster.GetCurrentStock(record.Tin, record.ItemCd); 
                    // Field join
                    //                    if (!reader.IsDBNull(36)) record.OrgnNatName = reader.GetString(36);        // Origin National Namee
                    //                    if (!reader.IsDBNull(37)) record.ItemTyName = reader.GetString(37);         // Item Type Name
                    //                    if (!reader.IsDBNull(38)) record.PkgUnitName = reader.GetString(38);        // Package Unit Name
                    //                    if (!reader.IsDBNull(39)) record.QtyUnitName = reader.GetString(39);        // Quantity Unit Name
                    //                    if (!reader.IsDBNull(40)) record.TaxTyName = reader.GetString(40);          // Taxation Type Name
                    //                    if (!reader.IsDBNull(41)) record.ItemClsName = reader.GetString(41);        // Item Classification Name (RRA)
                    //                    if (!reader.IsDBNull(42)) record.RdsQty = reader.GetDouble(42);             // 현재고 수량

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

        public bool ToTable(TaxpayerItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();

            try
            {
                command.CommandText = taxpayerItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerItemTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerItemTable.GetInsertSQL();
                    command.CommandType = CommandType.Text;

                    command.Parameters.Clear();
                    taxpayerItemTable.SetParameters(command, record);
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
        public bool ToTableSDC(EBM2x.RraSdc.model.Item item, TaxpayerItemRecord record)
        {
            string itemName = GetItemName(item.tin, item.itemCd);
            if (!string.IsNullOrEmpty(itemName)) return true;

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();

            try
            {
                command.Parameters.Clear();
                taxpayerItemTable.SetParametersSDC(command, item, record);

                //command.CommandText = taxpayerItemTable.GetUpdateSQL();
                //command.CommandType = CommandType.Text;

                //if (command.ExecuteNonQuery() < 1)
                //{
                    command.CommandText = taxpayerItemTable.GetInsertSQL();
                    command.CommandType = CommandType.Text;

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                //}
                //else
                //{
                //    return true;
                //}
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool ToTable(TaxpayerItemRecord record, StockMasterRecord stockRecord)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }
            IDbCommand commandStock = GetDbCommand();
            if (commandStock == null)
            {
                return false;
            }

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();
            StockMasterTable stockMasterTable = new StockMasterTable();

            try
            {
                command.Parameters.Clear();
                taxpayerItemTable.SetParameters(command, record);

                command.CommandText = taxpayerItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerItemTable.GetInsertSQL();
                    command.CommandType = CommandType.Text;

                    if (command.ExecuteNonQuery() >= 1)
                    {
                        commandStock.CommandText = stockMasterTable.GetInsertSQL();
                        commandStock.CommandType = CommandType.Text;

                        // 신규 상품인 경우 재고 마스터에도 반영
                        command.Parameters.Clear();
                        stockMasterTable.SetParameters(commandStock, stockRecord);
                        if (commandStock.ExecuteNonQuery() >= 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
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

        public bool DeleteTable(TaxpayerItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerItemTable taxpayerItemTable = new TaxpayerItemTable();

            try
            {
                command.CommandText = taxpayerItemTable.GetDeleteUpdateSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerItemTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public string GetNextSerial(string strItemGroupCode)
        {
            int length = strItemGroupCode.Length;
            int offset = 3 + length + 1;

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return "0000000";
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT IFNULL(MAX(CAST((SUBSTR(ITEM_CD, @Offset, 7)) as decimal)), 0) ");
            sql.Append("   FROM TAXPAYER_ITEM ");
            sql.Append("  WHERE SUBSTR(ITEM_CD, 3, @Length) = @strItemGroupCode ");

            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Offset";
            param.Value = offset;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@Length";
            param.Value = length;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@strItemGroupCode";
            param.Value = strItemGroupCode;
            command.Parameters.Add(param);

            try
            {
                long serial = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    serial = long.Parse(firstColumn.ToString());
                }

                return String.Format("{0:0000000}", serial + 1);
            }
            catch (Exception ex)
            {
                return String.Format("{0:0000000}", 0);
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
            sql.Append("update TAXPAYER_ITEM "); 
            sql.Append(" set PKG_UNIT_CD = @PKG_UNIT_CD ");  // Item Expiration Date
            sql.Append(",QTY_UNIT_CD = @QTY_UNIT_CD ");
            sql.Append(" WHERE PKG_UNIT_CD=''");         // Taxpayer Identification Number(TIN)\
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

using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.RraSdc.model;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ImportItemMaster.
    /// </summary>
    public class ImportItemMaster : ModelIO
    {
        public long GetWaitCount()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT COUNT(*) FROM IMPORT_ITEM WHERE IMPT_ITEM_STTS_CD = '2'";
                command.CommandType = CommandType.Text;

                long SalesSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    SalesSeq = long.Parse(firstColumn.ToString());
                }
                return (long)SalesSeq;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }
        public bool IsExist(string valTaskCd, string valDclDe, int valItemSeq, string valHsCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return false;

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT count(*) FROM IMPORT_ITEM ");
                sql.Append("  WHERE TASK_CD = @TASK_CD");
                sql.Append("    AND DCL_DE = @DCL_DE");
                sql.Append("    AND ITEM_SEQ = @ITEM_SEQ");
                sql.Append("    AND HS_CD = @HS_CD ");  // 

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TASK_CD";
                param.Value = valTaskCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@DCL_DE";
                param.Value = valDclDe;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_SEQ";
                param.Value = valItemSeq;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@HS_CD";
                param.Value = valHsCd;
                command.Parameters.Add(param);

                long count = (long)command.ExecuteScalar();
                //Decimal count = (Decimal)command.ExecuteScalar();
                return count > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public int Update(string valTaskCd, string valDclDe, int valItemSeq, string valHsCd, string calSttsCd, string valItemCode, string valItemClsCode, string valModrId, string valModrNm, string valProcessDate)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" UPDATE IMPORT_ITEM ");
                sql.Append("    SET IMPT_ITEM_STTS_CD = @IMPT_ITEM_STTS_CD, ");
                sql.Append("        ITEM_CD = @ITEM_CD, ");
                sql.Append("        ITEM_CLS_CD = @ITEM_CLS_CD, ");
                sql.Append("        MODR_ID = @ModrId, ");       // Modifier ID
                sql.Append("        MODR_NM = @ModrNm, ");       // Modifier Name
                sql.Append("        MOD_DT = @PROCESS_DATE ");
                sql.Append("  WHERE TASK_CD = @TASK_CD");
                sql.Append("    AND DCL_DE = @DCL_DE");
                sql.Append("    AND ITEM_SEQ = @ITEM_SEQ");
                sql.Append("    AND HS_CD = @HS_CD ");  // 

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@IMPT_ITEM_STTS_CD";
                param.Value = calSttsCd;
                command.Parameters.Add(param);
                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCode;
                command.Parameters.Add(param);
                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CLS_CD";
                param.Value = valItemClsCode;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ModrId";
                param.Value = valModrId;
                command.Parameters.Add(param);                  // Modifier ID

                param = command.CreateParameter();
                param.ParameterName = "@ModrNm";
                param.Value = valModrNm;
                command.Parameters.Add(param);                  // Modifier Name

                param = command.CreateParameter();
                param.ParameterName = "@PROCESS_DATE";
                param.Value = valProcessDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@TASK_CD";
                param.Value = valTaskCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@DCL_DE";
                param.Value = valDclDe;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_SEQ";
                param.Value = valItemSeq;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@HS_CD";
                param.Value = valHsCd;
                command.Parameters.Add(param);

                int count = command.ExecuteNonQuery();
                return count;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }

        public List<ImportItemRecordVAT> getImportItemTableVAT(string StartDate, string EndDate)
        {
            List<ImportItemRecordVAT> arrayList = new List<ImportItemRecordVAT>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT DCL_TAXOFC_CD ");
            sql.Append("      , DCL_NO ");
            sql.Append("      , DCL_DE ");
            sql.Append("      , ITEM_NM ");
            sql.Append("      , ORGN_NAT_CD ");
            sql.Append("      , CASE WHEN TRFF_AMT IS NULL THEN 0 ELSE TRFF_AMT END AS TRFF_AMT ");
            sql.Append("      , CASE WHEN VAT_AMT IS NULL THEN 0 ELSE VAT_AMT END AS VAT_AMT ");
            sql.Append("   FROM IMPORT_ITEM ");
            sql.Append("  WHERE DCL_DE BETWEEN @StartDate AND @EndDate ");

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

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ImportItemRecordVAT record = new ImportItemRecordVAT();

                    if (!reader.IsDBNull(0)) record.DclTaxofcCd = reader.GetString(0);        // Declaration Tax Office Code
                    if (!reader.IsDBNull(1)) record.DclNo = reader.GetString(1);              // Declaration Number
                    if (!reader.IsDBNull(2)) record.DclDe = reader.GetString(2);              // Declaration Date
                    if (!reader.IsDBNull(3)) record.ItemNm = reader.GetString(3);             // ItemName
                    if (!reader.IsDBNull(4)) record.OrgnNatCd = reader.GetString(4);          // Country Code of Origin
                    if (!reader.IsDBNull(5)) record.TrffAmt = reader.GetDouble(5);            // Tariff Amount
                    if (!reader.IsDBNull(6)) record.VatAmt = reader.GetDouble(6);             // VAT

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
        public List<ImportItemRecord> getImportItemTable(string StartDate, string EndDate, string valueSupplierName, string valueImptItemStts)
        {
            List<ImportItemRecord> arrayList = new List<ImportItemRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ImportItemTable importItemTable = new ImportItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(importItemTable.GetSelectSQL());
            sql.Append(" WHERE A.DCL_DE BETWEEN @StartDate AND @EndDate ");
            if (!string.IsNullOrEmpty(valueSupplierName))
            {
                sql.Append("   AND A.SUPPLIER_NM Like @SupplierName ");
            }
            if (!string.IsNullOrEmpty(valueImptItemStts))
            {
                sql.Append("   AND A.IMPT_ITEM_STTS_CD Like @IMPT_ITEM_STTS_CD ");
            }

            CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();
            ItemClassMaster ItemClassMaster = new ItemClassMaster();
            TaxpayerItemMaster TaxpayerItemMaster = new TaxpayerItemMaster();

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

                if (!string.IsNullOrEmpty(valueSupplierName))
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@SupplierName";
                    param.Value = MakeLikeString(valueSupplierName);
                    command.Parameters.Add(param);
                }

                if (!string.IsNullOrEmpty(valueImptItemStts))
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@IMPT_ITEM_STTS_CD";
                    param.Value = MakeLikeString(valueImptItemStts);
                    command.Parameters.Add(param);
                }

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ImportItemRecord record = new ImportItemRecord();

                    if (!reader.IsDBNull(0)) record.TaskCd = reader.GetString(0);               // Operation Code
                    if (!reader.IsDBNull(1)) record.DclDe = reader.GetString(1);                // Declaration Date
                    if (!reader.IsDBNull(2)) record.ItemSeq = reader.GetInt16(2);               // Item Sequence
                    if (!reader.IsDBNull(3)) record.DclNo = reader.GetString(3);                // Declaration Number
                    if (!reader.IsDBNull(4)) record.ImptItemSttsCd = reader.GetString(4);       // Import Item Status Status Code
                    if (!reader.IsDBNull(5)) record.Tin = reader.GetString(5);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(6)) record.TaxprNm = reader.GetString(6);              // Taxpayer's Name
                    if (!reader.IsDBNull(7)) record.ItemCd = reader.GetString(7);               // Item Code
                    if (!reader.IsDBNull(8)) record.ItemClsCd = reader.GetString(8);            // Item Classification Code
                    if (!reader.IsDBNull(9)) record.HsCd = reader.GetString(9);                 // HS Code
                    if (!reader.IsDBNull(10)) record.ItemNm = reader.GetString(10);             // ItemName
                    if (!reader.IsDBNull(11)) record.OrgnNatCd = reader.GetString(11);          // Country Code of Origin
                    if (!reader.IsDBNull(12)) record.ExptNatCd = reader.GetString(12);          // Country Code of Export
                    if (!reader.IsDBNull(13)) record.Pkg = reader.GetDouble(13);                // Packing
                    if (!reader.IsDBNull(14)) record.PkgUnitCd = reader.GetString(14);          // Packing Unit Code
                    if (!reader.IsDBNull(15)) record.Qty = reader.GetDouble(15);                // Quantity
                    if (!reader.IsDBNull(16)) record.QtyUnitCd = reader.GetString(16);          // Quantity Unit Code
                    if (!reader.IsDBNull(17)) record.TotWt = reader.GetDouble(17);              // Gross Weight
                    if (!reader.IsDBNull(18)) record.NetWt = reader.GetDouble(18);              // Net Weight
                    if (!reader.IsDBNull(19)) record.SpplrNm = reader.GetString(19);            // Supplier's name
                    if (!reader.IsDBNull(20)) record.AgntNm = reader.GetString(20);             // Agent's Name
                    if (!reader.IsDBNull(21)) record.InvcFcurAmt = reader.GetDouble(21);        // Invoice Foreign Currency Amount
                    if (!reader.IsDBNull(22)) record.InvcFcurCd = reader.GetString(22);         // Invoice Foreign Currency Code
                    if (!reader.IsDBNull(23)) record.InvcFcurExcrt = reader.GetDouble(23);      // Invoice Foreign Currency Rate
                    if (!reader.IsDBNull(24)) record.DclTaxofcCd = reader.GetString(24);        // Declaration Tax Office Code
                    if (!reader.IsDBNull(25)) record.TrffAmt = reader.GetDouble(25);            // Tariff Amount
                    if (!reader.IsDBNull(26)) record.VatAmt = reader.GetDouble(26);             // VAT
                    if (!reader.IsDBNull(27)) record.Remark = reader.GetString(27);             // Remark
                    if (!reader.IsDBNull(28)) record.RegrId = reader.GetString(28);             // Registrant ID
                    if (!reader.IsDBNull(29)) record.RegrNm = reader.GetString(29);             // Registrant Name
                    if (!reader.IsDBNull(30)) record.RegDt = reader.GetString(30);              // Registered Date
                    if (!reader.IsDBNull(31)) record.ModrId = reader.GetString(31);             // Modifier ID
                    if (!reader.IsDBNull(32)) record.ModrNm = reader.GetString(32);             // Modifier Name
                    if (!reader.IsDBNull(33)) record.ModDt = reader.GetString(33);              // Modified Date

                    //if (!reader.IsDBNull(34)) record.ImptItemSttsNm = reader.GetString(34);
                    //if (!reader.IsDBNull(35)) record.OrgnNatNm = reader.GetString(35);
                    //if (!reader.IsDBNull(36)) record.ExptNatNm = reader.GetString(36);
                    //if (!reader.IsDBNull(37)) record.QtyUnitNm = reader.GetString(37);
                    //if (!reader.IsDBNull(38)) record.InvcFcurNm = reader.GetString(38);
                    //if (!reader.IsDBNull(39)) record.ItemClsNm = reader.GetString(39);
                    //if (!reader.IsDBNull(40)) record.SupplierItemNm = reader.GetString(40);
                    //if (!reader.IsDBNull(41)) record.SupplierItemCd = reader.GetString(41);
                    //if (!reader.IsDBNull(42)) record.ProcessDate = reader.GetString(42);
                    //if (!reader.IsDBNull(43)) record.DclrNo = reader.GetString(43);
                    //if (!reader.IsDBNull(44)) record.AgentNm = reader.GetString(44);

                    //sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '26' AND CD = A.IMPT_ITEM_STTS_CD) AS APPROVAL_STATUS_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.ORGN_NAT_CD),'') AS ORGPLCE_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.EXPT_NAT_CD),'') AS EXPORT_NATION_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '10' AND CD = A.QTY_UNIT_CD),'') AS QTY_UNIT_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '33' AND CD = A.INVC_FCUR_CD),'') AS INV_CUR_NM,   ");
                    //sql.Append("       IFNULL((SELECT ITEM_CLS_NM FROM ITEM_CLASS WHERE ITEM_CLS_CD = A.ITEM_CLS_CD),'') AS ITEM_CLS_NM,   ");
                    //sql.Append("       IFNULL((SELECT ITEM_NM FROM TAXPAYER_ITEM WHERE TIN = A.TIN AND ITEM_CD = A.ITEM_CD),'') AS SUPPLIER_ITEM_NM,   ");
                    //sql.Append("       IFNULL(A.ITEM_CD,'') AS SUPPLIER_ITEM_CD,   ");
                    //sql.Append("       IFNULL(A.MOD_DT,'') AS PROCESS_DATE,   ");
                    //sql.Append("       IFNULL(A.REGR_ID,'') AS DCLR_NO,   ");
                    //sql.Append("       IFNULL(A.REGR_ID,'') AS AGENT_NM  ");

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach(ImportItemRecord record in arrayList)
            {
                record.ImptItemSttsNm = CodeDtlMaster.ImptItemSttsNm(record.ImptItemSttsCd);
                record.OrgnNatNm = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                record.ExptNatNm = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                record.QtyUnitNm = CodeDtlMaster.QtyUnitName(record.QtyUnitCd);           // Quantity Unit Name
                record.InvcFcurNm = CodeDtlMaster.InvcFcurNm(record.InvcFcurCd);
                record.ItemClsNm = ItemClassMaster.GetItemClassName(record.ItemClsCd);    // Item Classification Name (RRA)
                record.SupplierItemNm = TaxpayerItemMaster.GetItemName(record.Tin, record.ItemCd);
                record.SupplierItemCd = record.ItemCd;
                record.ProcessDate = record.ModDt;
                record.DclrNo = record.RegrId;
                record.AgentNm = record.RegrId;
            }

            return arrayList;
        }

        public bool ToRecord(ImportItemRecord record, string valTaskCd, string valDclDe, int valItemSeq, string valHsCd)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ImportItemTable importItemTable = new ImportItemTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(importItemTable.GetSelectSQL());
            sql.Append(" WHERE TASK_CD = @TASK_CD ");
            sql.Append("   AND DCL_DE = @DCL_DE ");
            sql.Append("   AND ITEM_SEQ = @ITEM_SEQ ");
            sql.Append("    AND HS_CD = @HsCd ");  // 

            CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();
            ItemClassMaster ItemClassMaster = new ItemClassMaster();
            TaxpayerItemMaster TaxpayerItemMaster = new TaxpayerItemMaster();
            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TASK_CD";
                param.Value = valTaskCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@DCL_DE";
                param.Value = valDclDe;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_SEQ";
                param.Value = valItemSeq;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@HS_CD";
                param.Value = valHsCd;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.TaskCd = reader.GetString(0);               // Operation Code
                    if (!reader.IsDBNull(1)) record.DclDe = reader.GetString(1);                // Declaration Date
                    if (!reader.IsDBNull(2)) record.ItemSeq = reader.GetInt16(2);               // Item Sequence
                    if (!reader.IsDBNull(3)) record.DclNo = reader.GetString(3);                // Declaration Number
                    if (!reader.IsDBNull(4)) record.ImptItemSttsCd = reader.GetString(4);       // Import Item Status Status Code
                    if (!reader.IsDBNull(5)) record.Tin = reader.GetString(5);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(6)) record.TaxprNm = reader.GetString(6);              // Taxpayer's Name
                    if (!reader.IsDBNull(7)) record.ItemCd = reader.GetString(7);               // Item Code
                    if (!reader.IsDBNull(8)) record.ItemClsCd = reader.GetString(8);            // Item Classification Code
                    if (!reader.IsDBNull(9)) record.HsCd = reader.GetString(9);                 // HS Code
                    if (!reader.IsDBNull(10)) record.ItemNm = reader.GetString(10);             // ItemName
                    if (!reader.IsDBNull(11)) record.OrgnNatCd = reader.GetString(11);          // Country Code of Origin
                    if (!reader.IsDBNull(12)) record.ExptNatCd = reader.GetString(12);          // Country Code of Export
                    if (!reader.IsDBNull(13)) record.Pkg = reader.GetDouble(13);                // Packing
                    if (!reader.IsDBNull(14)) record.PkgUnitCd = reader.GetString(14);          // Packing Unit Code
                    if (!reader.IsDBNull(15)) record.Qty = reader.GetDouble(15);                // Quantity
                    if (!reader.IsDBNull(16)) record.QtyUnitCd = reader.GetString(16);          // Quantity Unit Code
                    if (!reader.IsDBNull(17)) record.TotWt = reader.GetDouble(17);              // Gross Weight
                    if (!reader.IsDBNull(18)) record.NetWt = reader.GetDouble(18);              // Net Weight
                    if (!reader.IsDBNull(19)) record.SpplrNm = reader.GetString(19);            // Supplier's name
                    if (!reader.IsDBNull(20)) record.AgntNm = reader.GetString(20);             // Agent's Name
                    if (!reader.IsDBNull(21)) record.InvcFcurAmt = reader.GetDouble(21);        // Invoice Foreign Currency Amount
                    if (!reader.IsDBNull(22)) record.InvcFcurCd = reader.GetString(22);         // Invoice Foreign Currency Code
                    if (!reader.IsDBNull(23)) record.InvcFcurExcrt = reader.GetDouble(23);      // Invoice Foreign Currency Rate
                    if (!reader.IsDBNull(24)) record.DclTaxofcCd = reader.GetString(24);        // Declaration Tax Office Code
                    if (!reader.IsDBNull(25)) record.TrffAmt = reader.GetDouble(25);            // Tariff Amount
                    if (!reader.IsDBNull(26)) record.VatAmt = reader.GetDouble(26);             // VAT
                    if (!reader.IsDBNull(27)) record.Remark = reader.GetString(27);             // Remark
                    if (!reader.IsDBNull(28)) record.RegrId = reader.GetString(28);             // Registrant ID
                    if (!reader.IsDBNull(29)) record.RegrNm = reader.GetString(29);             // Registrant Name
                    if (!reader.IsDBNull(30)) record.RegDt = reader.GetString(30);              // Registered Date
                    if (!reader.IsDBNull(31)) record.ModrId = reader.GetString(31);     // Modifier ID
                    if (!reader.IsDBNull(32)) record.ModrNm = reader.GetString(32);             // Modifier Name
                    if (!reader.IsDBNull(33)) record.ModDt = reader.GetString(33);      // Modified Date

                    //if (!reader.IsDBNull(34)) record.ImptItemSttsNm = reader.GetString(34);
                    //if (!reader.IsDBNull(35)) record.OrgnNatNm = reader.GetString(35);
                    //if (!reader.IsDBNull(36)) record.ExptNatNm = reader.GetString(36);
                    //if (!reader.IsDBNull(37)) record.QtyUnitNm = reader.GetString(37);
                    //if (!reader.IsDBNull(38)) record.InvcFcurNm = reader.GetString(38);
                    //if (!reader.IsDBNull(39)) record.ItemClsNm = reader.GetString(39);
                    //if (!reader.IsDBNull(40)) record.SupplierItemNm = reader.GetString(40);
                    //if (!reader.IsDBNull(41)) record.SupplierItemCd = reader.GetString(41);
                    //if (!reader.IsDBNull(42)) record.ProcessDate = reader.GetString(42);
                    //if (!reader.IsDBNull(43)) record.DclrNo = reader.GetString(43);
                    //if (!reader.IsDBNull(44)) record.AgentNm = reader.GetString(44);

                    //sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '26' AND CD = A.IMPT_ITEM_STTS_CD) AS APPROVAL_STATUS_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.ORGN_NAT_CD),'') AS ORGPLCE_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.EXPT_NAT_CD),'') AS EXPORT_NATION_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '10' AND CD = A.QTY_UNIT_CD),'') AS QTY_UNIT_NM,   ");
                    //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '33' AND CD = A.INVC_FCUR_CD),'') AS INV_CUR_NM,   ");
                    //sql.Append("       IFNULL((SELECT ITEM_CLS_NM FROM ITEM_CLASS WHERE ITEM_CLS_CD = A.ITEM_CLS_CD),'') AS ITEM_CLS_NM,   ");
                    //sql.Append("       IFNULL((SELECT ITEM_NM FROM TAXPAYER_ITEM WHERE TIN = A.TIN AND ITEM_CD = A.ITEM_CD),'') AS SUPPLIER_ITEM_NM,   ");
                    //sql.Append("       IFNULL(A.ITEM_CD,'') AS SUPPLIER_ITEM_CD,   ");
                    //sql.Append("       IFNULL(A.MOD_DT,'') AS PROCESS_DATE,   ");
                    //sql.Append("       IFNULL(A.REGR_ID,'') AS DCLR_NO,   ");
                    //sql.Append("       IFNULL(A.REGR_ID,'') AS AGENT_NM  ");

                    reader.Close();

                    record.ImptItemSttsNm = CodeDtlMaster.ImptItemSttsNm(record.ImptItemSttsCd);
                    record.OrgnNatNm = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                    record.ExptNatNm = CodeDtlMaster.OrgnNatName(record.OrgnNatCd);           // Origin National Namee
                    record.QtyUnitNm = CodeDtlMaster.QtyUnitName(record.QtyUnitCd);           // Quantity Unit Name
                    record.InvcFcurNm = CodeDtlMaster.InvcFcurNm(record.InvcFcurCd);
                    record.ItemClsNm = ItemClassMaster.GetItemClassName(record.ItemClsCd);    // Item Classification Name (RRA)
                    record.SupplierItemNm = TaxpayerItemMaster.GetItemName(record.Tin, record.ItemCd);
                    record.SupplierItemCd = record.ItemCd;
                    record.ProcessDate = record.ModDt;
                    record.DclrNo = record.RegrId;
                    record.AgentNm = record.RegrId;

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

        public bool ToTable(ImportItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ImportItemTable importItemTable = new ImportItemTable();

            try
            {
                command.Parameters.Clear();
                importItemTable.SetParameters(command, record);

                command.CommandText = importItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = importItemTable.GetInsertSQL();
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
        public bool ToTableSDC(ImportItem importItem, ImportItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ImportItemTable importItemTable = new ImportItemTable();

            try
            {
                command.Parameters.Clear();
                importItemTable.SetParametersSDC(command, importItem, record);

                command.CommandText = importItemTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = importItemTable.GetInsertSQL();
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
        public bool DeleteTable(ImportItemRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ImportItemTable importItemTable = new ImportItemTable();

            try
            {
                command.CommandText = importItemTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                importItemTable.SetParameters(command, record);
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
            sql.Append("update IMPORT_ITEM "); 
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

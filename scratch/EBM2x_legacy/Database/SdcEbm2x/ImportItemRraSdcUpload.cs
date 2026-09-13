using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Datafile.trlog;
    using EBM2x.RraSdc;
    using EBM2x.RraSdc.model;
    using EBM2x.UI;
    using EBM2x.Utils;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ItemRraSdcUpload.
    /// </summary>
    public class ImportItemRraSdcUpload : ModelIO
    {
        public List<ImportItemUpdateReq> getImportItemTable()
        {
            List<ImportItemUpdateReq> arrayList = new List<ImportItemUpdateReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ImportItemTable importItemTable = new ImportItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(importItemTable.GetSelectSQL());

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ImportItemUpdateReq record = new ImportItemUpdateReq();

                    if (!reader.IsDBNull(0)) record.taskCd = reader.GetString(0);               // Operation Code
                    if (!reader.IsDBNull(1)) record.dclDe = reader.GetString(1);                // Declaration Date
                    if (!reader.IsDBNull(2)) record.itemSeq = reader.GetInt16(2);               // Item Sequence
                    //if (!reader.IsDBNull(3)) record.cclNo = reader.GetString(3);                // Declaration Number
                    if (!reader.IsDBNull(4)) record.imptItemSttsCd = reader.GetString(4);       // Import Item Status Status Code
                    if (!reader.IsDBNull(5)) record.tin = reader.GetString(5);                  // Taxpayer Identification Number(TIN)
                    //if (!reader.IsDBNull(6)) record.taxprNm = reader.GetString(6);              // Taxpayer's Name
                    if (!reader.IsDBNull(7)) record.itemCd = reader.GetString(7);               // Item Code
                    if (!reader.IsDBNull(8)) record.itemClsCd = reader.GetString(8);            // Item Classification Code
                    if (!reader.IsDBNull(9)) record.hsCd = reader.GetString(9);                 // HS Code
                    //if (!reader.IsDBNull(10)) record.itemNm = reader.GetString(10);             // ItemName
                    //if (!reader.IsDBNull(11)) record.orgnNatCd = reader.GetString(11);          // Country Code of Origin
                    //if (!reader.IsDBNull(12)) record.exptNatCd = reader.GetString(12);          // Country Code of Export
                    //if (!reader.IsDBNull(13)) record.pkg = reader.GetDouble(13);                // Packing
                    //if (!reader.IsDBNull(14)) record.pkgUnitCd = reader.GetString(14);          // Packing Unit Code
                    //if (!reader.IsDBNull(15)) record.qty = reader.GetDouble(15);                // Quantity
                    //if (!reader.IsDBNull(16)) record.qtyUnitCd = reader.GetString(16);          // Quantity Unit Code
                    //if (!reader.IsDBNull(17)) record.totWt = reader.GetDouble(17);              // Gross Weight
                    //if (!reader.IsDBNull(18)) record.netWt = reader.GetDouble(18);              // Net Weight
                    //if (!reader.IsDBNull(19)) record.spplrNm = reader.GetString(19);            // Supplier's name
                    //if (!reader.IsDBNull(20)) record.agntNm = reader.GetString(20);             // Agent's Name
                    //if (!reader.IsDBNull(21)) record.invcFcurAmt = reader.GetDouble(21);        // Invoice Foreign Currency Amount
                    //if (!reader.IsDBNull(22)) record.invcFcurCd = reader.GetString(22);         // Invoice Foreign Currency Code
                    //if (!reader.IsDBNull(23)) record.invcFcurExcrt = reader.GetDouble(23);      // Invoice Foreign Currency Rate
                    //if (!reader.IsDBNull(24)) record.dclTaxofcCd = reader.GetString(24);        // Declaration Tax Office Code
                    //if (!reader.IsDBNull(25)) record.trffAmt = reader.GetDouble(25);            // Tariff Amount
                    //if (!reader.IsDBNull(26)) record.vatAmt = reader.GetDouble(26);             // VAT
                    if (!reader.IsDBNull(27)) record.remark = reader.GetString(27);             // Remark
                    if (!reader.IsDBNull(28)) record.regrId = reader.GetString(28);             // Registrant ID
                    if (!reader.IsDBNull(29)) record.regrNm = reader.GetString(29);             // Registrant Name
                    //if (!reader.IsDBNull(30)) record.RegDt = reader.GetString(30);              // Registered Date
                    if (!reader.IsDBNull(31)) record.modrId = reader.GetString(31);             // Modifier ID
                    if (!reader.IsDBNull(32)) record.modrNm = reader.GetString(32);             // Modifier Name
                    //if (!reader.IsDBNull(33)) record.ModDt = reader.GetString(33);              // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name


                    arrayList.Add(record);
                }
                reader.Close();

                foreach (ImportItemUpdateReq record in arrayList)
                {
                    record.tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                    record.bhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }

        public void SendImportItemSave(string taskCd, string dclDe, int itemSeq)
        {
            List<ImportItemUpdateReq> sendList = getImportItemTable( taskCd,  dclDe,  itemSeq);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "ImportItem";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_IMPORT_ITEM_UPDATE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<ImportItemUpdateReq> getImportItemTable(string taskCd, string dclDe, int itemSeq)
        {
            List<ImportItemUpdateReq> arrayList = new List<ImportItemUpdateReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ImportItemTable importItemTable = new ImportItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(importItemTable.GetSelectSQL());
            sql.Append(" WHERE TASK_CD = @TASK_CD ");
            sql.Append("   AND DCL_DE = @DCL_DE ");
            sql.Append("   AND ITEM_SEQ = @ITEM_SEQ ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TASK_CD";
                param.Value = taskCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@DCL_DE";
                param.Value = dclDe;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_SEQ";
                param.Value = itemSeq;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ImportItemUpdateReq record = new ImportItemUpdateReq();

                    if (!reader.IsDBNull(0)) record.taskCd = reader.GetString(0);               // Operation Code
                    if (!reader.IsDBNull(1)) record.dclDe = reader.GetString(1);                // Declaration Date
                    if (!reader.IsDBNull(2)) record.itemSeq = reader.GetInt16(2);               // Item Sequence
                    //if (!reader.IsDBNull(3)) record.cclNo = reader.GetString(3);                // Declaration Number
                    if (!reader.IsDBNull(4)) record.imptItemSttsCd = reader.GetString(4);       // Import Item Status Status Code
                    if (!reader.IsDBNull(5)) record.tin = reader.GetString(5);                  // Taxpayer Identification Number(TIN)
                    //if (!reader.IsDBNull(6)) record.taxprNm = reader.GetString(6);              // Taxpayer's Name
                    if (!reader.IsDBNull(7)) record.itemCd = reader.GetString(7);               // Item Code
                    if (!reader.IsDBNull(8)) record.itemClsCd = reader.GetString(8);            // Item Classification Code
                    if (!reader.IsDBNull(9)) record.hsCd = reader.GetString(9);                 // HS Code
                    //if (!reader.IsDBNull(10)) record.itemNm = reader.GetString(10);             // ItemName
                    //if (!reader.IsDBNull(11)) record.orgnNatCd = reader.GetString(11);          // Country Code of Origin
                    //if (!reader.IsDBNull(12)) record.exptNatCd = reader.GetString(12);          // Country Code of Export
                    //if (!reader.IsDBNull(13)) record.pkg = reader.GetDouble(13);                // Packing
                    //if (!reader.IsDBNull(14)) record.pkgUnitCd = reader.GetString(14);          // Packing Unit Code
                    //if (!reader.IsDBNull(15)) record.qty = reader.GetDouble(15);                // Quantity
                    //if (!reader.IsDBNull(16)) record.qtyUnitCd = reader.GetString(16);          // Quantity Unit Code
                    //if (!reader.IsDBNull(17)) record.totWt = reader.GetDouble(17);              // Gross Weight
                    //if (!reader.IsDBNull(18)) record.netWt = reader.GetDouble(18);              // Net Weight
                    //if (!reader.IsDBNull(19)) record.spplrNm = reader.GetString(19);            // Supplier's name
                    //if (!reader.IsDBNull(20)) record.agntNm = reader.GetString(20);             // Agent's Name
                    //if (!reader.IsDBNull(21)) record.invcFcurAmt = reader.GetDouble(21);        // Invoice Foreign Currency Amount
                    //if (!reader.IsDBNull(22)) record.invcFcurCd = reader.GetString(22);         // Invoice Foreign Currency Code
                    //if (!reader.IsDBNull(23)) record.invcFcurExcrt = reader.GetDouble(23);      // Invoice Foreign Currency Rate
                    //if (!reader.IsDBNull(24)) record.dclTaxofcCd = reader.GetString(24);        // Declaration Tax Office Code
                    //if (!reader.IsDBNull(25)) record.trffAmt = reader.GetDouble(25);            // Tariff Amount
                    //if (!reader.IsDBNull(26)) record.vatAmt = reader.GetDouble(26);             // VAT
                    if (!reader.IsDBNull(27)) record.remark = reader.GetString(27);             // Remark
                    if (!reader.IsDBNull(28)) record.regrId = reader.GetString(28);             // Registrant ID
                    if (!reader.IsDBNull(29)) record.regrNm = reader.GetString(29);             // Registrant Name
                    //if (!reader.IsDBNull(30)) record.RegDt = reader.GetString(30);              // Registered Date
                    if (!reader.IsDBNull(31)) record.modrId = reader.GetString(31);             // Modifier ID
                    if (!reader.IsDBNull(32)) record.modrNm = reader.GetString(32);             // Modifier Name
                    //if (!reader.IsDBNull(33)) record.ModDt = reader.GetString(33);              // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    record.tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
                    record.bhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;

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

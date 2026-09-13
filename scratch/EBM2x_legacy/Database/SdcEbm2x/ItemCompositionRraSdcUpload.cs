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
    using EBM2x.Utils;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ItemRraSdcUpload.
    /// </summary>
    public class ItemCompositionRraSdcUpload : ModelIO
    {
        public void SendItemCompositionSave(string valTin, string valItemCd)
        {
            List<ItemCompositionSaveReq> sendList = getItemCompositionTable(valTin, valItemCd);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "ItemComposition";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_ITEM_COMPOSITION_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<ItemCompositionSaveReq> getItemCompositionTable(string valTin, string valItemCd)
        {
            List<ItemCompositionSaveReq> arrayList = new List<ItemCompositionSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemCompositionTable.GetSelectSQL());
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

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ItemCompositionSaveReq record = new ItemCompositionSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.itemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.cpstItemCd = reader.GetString(2);           // Composition Item Code
                    if (!reader.IsDBNull(3)) record.cpstQty = reader.GetDouble(3);              // Composition Quantity
                    if (!reader.IsDBNull(4)) record.regrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.regrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(4)) record.modrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.modrNm = reader.GetString(5);               // Registrant Name
                    //if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);                // Registered Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

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

        public List<ItemCompositionSaveReq> getItemCompositionTable()
        {
            List<ItemCompositionSaveReq> arrayList = new List<ItemCompositionSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerItemCompositionTable taxpayerItemCompositionTable = new TaxpayerItemCompositionTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerItemCompositionTable.GetSelectSQL());

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ItemCompositionSaveReq record = new ItemCompositionSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.itemCd = reader.GetString(1);               // Item Code
                    if (!reader.IsDBNull(2)) record.cpstItemCd = reader.GetString(2);           // Composition Item Code
                    if (!reader.IsDBNull(3)) record.cpstQty = reader.GetDouble(3);              // Composition Quantity
                    if (!reader.IsDBNull(4)) record.regrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.regrNm = reader.GetString(5);               // Registrant Name
                    if (!reader.IsDBNull(4)) record.modrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.modrNm = reader.GetString(5);               // Registrant Name
                    //if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);                // Registered Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

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

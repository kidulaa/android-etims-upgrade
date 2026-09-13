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
    /// Description of TrnsSaleMaster.
    /// </summary>
    public class StockMasterRraSdcUpload : ModelIO
    {
        public List<StockMasterSaveReq> getStockMasterTable()
        {
            List<StockMasterSaveReq> arrayList = new List<StockMasterSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockMasterTable trnsSaleReceiptTable = new StockMasterTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
            //sql.Append(" WHERE A.SALES_DT BETWEEN @StartDate AND @EndDate ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                //param = command.CreateParameter();
                //param.ParameterName = "@StartDate";
                //param.Value = StartDate;
                //command.Parameters.Add(param);

                //param = command.CreateParameter();
                //param.ParameterName = "@EndDate";
                //param.Value = EndDate;
                //command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockMasterSaveReq record = new StockMasterSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.itemCd = reader.GetString(2);               // Item Code
                    if (!reader.IsDBNull(3)) record.rsdQty = Math.Round(reader.GetDouble(3), 2);               // Resodual Quantity
                    if (!reader.IsDBNull(4)) record.regrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.regrNm = reader.GetString(5);               // Registrant Name
                    //if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);        // Registered Date
                    if (!reader.IsDBNull(7)) record.modrId = reader.GetString(7);               // Modifier ID
                    if (!reader.IsDBNull(8)) record.modrNm = reader.GetString(8);               // Modifier Name
                    //if (!reader.IsDBNull(9)) record.ModDt = reader.GetString(9);        // Modified Date
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

        public void SendStockMasterSave(string valTin, string valBhfId, string valItemCode)
        {
            List<StockMasterSaveReq> sendList = getStockMasterTable( valTin,  valBhfId,  valItemCode);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "StockMaster";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_STOCK_MASTER_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<StockMasterSaveReq> getStockMasterTable(string valTin, string valBhfId, string valItemCode)
        {
            List<StockMasterSaveReq> arrayList = new List<StockMasterSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockMasterTable trnsSaleReceiptTable = new StockMasterTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
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
                param.ParameterName = "@BHF_ID";
                param.Value = valBhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ITEM_CD";
                param.Value = valItemCode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockMasterSaveReq record = new StockMasterSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.itemCd = reader.GetString(2);               // Item Code
                    if (!reader.IsDBNull(3)) record.rsdQty = Math.Round(reader.GetDouble(3), 2);               // Resodual Quantity
                    if (!reader.IsDBNull(4)) record.regrId = reader.GetString(4);               // Registrant ID
                    if (!reader.IsDBNull(5)) record.regrNm = reader.GetString(5);               // Registrant Name
                    //if (!reader.IsDBNull(6)) record.RegDt = reader.GetString(6);        // Registered Date
                    if (!reader.IsDBNull(7)) record.modrId = reader.GetString(7);               // Modifier ID
                    if (!reader.IsDBNull(8)) record.modrNm = reader.GetString(8);               // Modifier Name
                    //if (!reader.IsDBNull(9)) record.ModDt = reader.GetString(9);        // Modified Date
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

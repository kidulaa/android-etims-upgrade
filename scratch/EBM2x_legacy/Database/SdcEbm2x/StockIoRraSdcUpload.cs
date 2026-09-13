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
    /// Description of StockIoRraSdcUpload.
    /// </summary>
    public class StockIoRraSdcUpload : ModelIO
    {
        public List<StockIoSaveReq> getStockIoTable(string StartDate, string EndDate)
        {
            List<StockIoSaveReq> arrayList = new List<StockIoSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockIoTable stockIoTable = new StockIoTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(stockIoTable.GetSelectSQL());
            sql.Append(" WHERE OCRN_DT BETWEEN @StartDate AND @EndDate ");

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

                TaxpayerBhfCustMaster taxpayerBhfCustMaster = new TaxpayerBhfCustMaster();
                CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockIoSaveReq record = new StockIoSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.sarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.orgSarNo = GetLong(reader, 3);             // Original Stored and Released No.
                    if (!reader.IsDBNull(4)) record.regTyCd = reader.GetString(4);              // Registration Type Code
                                                                                                //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(5)) record.taxprNm = reader.GetString(5);              // Taxpayer's Name
                    if (!reader.IsDBNull(6)) record.custTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(7)) record.custBhfId = reader.GetString(7);            // Customer Branch ID
                    if (!reader.IsDBNull(8)) record.custNm = reader.GetString(8);               // Customer Name
                                                                                                //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(9)) record.invcNo = reader.GetInt32(9);               // Onvoice No.
                    if (!reader.IsDBNull(9)) record.sarTyCd = reader.GetString(9);            // Stored and Released Type Code
                    if (!reader.IsDBNull(10)) record.ocrnDt = reader.GetString(10);     // Occurred Date time
                    if (!reader.IsDBNull(11)) record.totItemCnt = reader.GetInt32(11);          // Total Item Count
                    if (!reader.IsDBNull(12)) record.totTaxblAmt = Math.Round(reader.GetDouble(12), 2);        // Total Taxable Amount
                    if (!reader.IsDBNull(13)) record.totTaxAmt = Math.Round(reader.GetDouble(13), 2);          // Total Tax Amount
                    if (!reader.IsDBNull(14)) record.totAmt = Math.Round(reader.GetDouble(14), 2);             // Total Amount
                    if (!reader.IsDBNull(15)) record.remark = reader.GetString(15);             // Remark

                    if (!reader.IsDBNull(16)) record.regrId = reader.GetString(16);             // Registrant ID
                    if (!reader.IsDBNull(17)) record.regrNm = reader.GetString(17);             // Registrant Name
                    //if (!reader.IsDBNull(18)) record.RegDt = reader.GetString(18);      // Registered Date
                    if (!reader.IsDBNull(19)) record.modrId = reader.GetString(19);             // Modifier ID
                    if (!reader.IsDBNull(20)) record.modrNm = reader.GetString(20);             // Modifier Name
                    //if (!reader.IsDBNull(21)) record.ModDt = reader.GetString(21);      // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (StockIoSaveReq record in arrayList)
                {
                    record.itemList = getStockIoItemSaveReqTable(record.tin, record.bhfId, record.sarNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }


        public void SendStockIoSave(string valTin, string valBhfId, long sarNo)
        {
            List<StockIoSaveReq> sendList = getStockIoTable( valTin,  valBhfId,  sarNo);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "StockIo";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_STOCK_IO_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                if (sendList[i].itemList.Count > 0)
                {
                    RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
                }
            }
        }

        public List<StockIoSaveReq> getStockIoTable(string valTin, string valBhfId, long sarNo)
        {
            List<StockIoSaveReq> arrayList = new List<StockIoSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockIoTable stockIoTable = new StockIoTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(stockIoTable.GetSelectSQL());
            sql.Append(" WHERE SAR_NO = @SAR_NO ");

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
                param.Value = sarNo;
                command.Parameters.Add(param);

                TaxpayerBhfCustMaster taxpayerBhfCustMaster = new TaxpayerBhfCustMaster();
                CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockIoSaveReq record = new StockIoSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.sarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.orgSarNo = GetLong(reader, 3);             // Original Stored and Released No.
                    if (!reader.IsDBNull(4)) record.regTyCd = reader.GetString(4);              // Registration Type Code
                                                                                                //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(5)) record.taxprNm = reader.GetString(5);              // Taxpayer's Name
                    if (!reader.IsDBNull(6)) record.custTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(7)) record.custBhfId = reader.GetString(7);            // Customer Branch ID
                    if (!reader.IsDBNull(8)) record.custNm = reader.GetString(8);               // Customer Name
                                                                                                //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(9)) record.invcNo = reader.GetInt32(9);               // Onvoice No.
                    if (!reader.IsDBNull(9)) record.sarTyCd = reader.GetString(9);            // Stored and Released Type Code
                    if (!reader.IsDBNull(10)) record.ocrnDt = reader.GetString(10);     // Occurred Date time
                    if (!reader.IsDBNull(11)) record.totItemCnt = reader.GetInt32(11);          // Total Item Count
                    if (!reader.IsDBNull(12)) record.totTaxblAmt = Math.Round(reader.GetDouble(12), 2);        // Total Taxable Amount
                    if (!reader.IsDBNull(13)) record.totTaxAmt = Math.Round(reader.GetDouble(13), 2);          // Total Tax Amount
                    if (!reader.IsDBNull(14)) record.totAmt = Math.Round(reader.GetDouble(14), 2);             // Total Amount
                    if (!reader.IsDBNull(15)) record.remark = reader.GetString(15);             // Remark

                    if (!reader.IsDBNull(16)) record.regrId = reader.GetString(16);             // Registrant ID
                    if (!reader.IsDBNull(17)) record.regrNm = reader.GetString(17);             // Registrant Name
                    //if (!reader.IsDBNull(18)) record.RegDt = reader.GetString(18);      // Registered Date
                    if (!reader.IsDBNull(19)) record.modrId = reader.GetString(19);             // Modifier ID
                    if (!reader.IsDBNull(20)) record.modrNm = reader.GetString(20);             // Modifier Name
                    //if (!reader.IsDBNull(21)) record.ModDt = reader.GetString(21);      // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (StockIoSaveReq record in arrayList)
                {
                    record.itemList = getStockIoItemSaveReqTable(record.tin, record.bhfId, record.sarNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }

        public List<StockIoItemSaveReq> getStockIoItemSaveReqTable(string valTin, string valBhfId, long sarNo)
        {
            List<StockIoItemSaveReq> arrayList = new List<StockIoItemSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StockIoItemTable stockIoItemTable = new StockIoItemTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(stockIoItemTable.GetSelectSQL());
            sql.Append(" WHERE SAR_NO = @SAR_NO ");
            //sql.Append(" WHERE TIN = @TIN ");
            //sql.Append("   AND BHF_ID = @BHF_ID ");
            //sql.Append("   AND SAR_NO = @SAR_NO ");

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
                param.Value = sarNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    StockIoItemSaveReq record = new StockIoItemSaveReq();

                    //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    //JCNA DEBUG_DELETE                    if (!reader.IsDBNull(2)) record.sarNo = GetLong(reader, 2);                // Stored and Released No.
                    if (!reader.IsDBNull(3)) record.itemSeq = reader.GetInt16(3);               // Item Sequence
                    if (!reader.IsDBNull(4)) record.itemCd = reader.GetString(4);               // Item Code
                    if (!reader.IsDBNull(5)) record.itemClsCd = reader.GetString(5);            // Item Classification Code
                    if (!reader.IsDBNull(6)) record.itemNm = reader.GetString(6);               // Item Name
                    if (!reader.IsDBNull(7)) record.bcd = reader.GetString(7);                  // Barcode
                    if (!reader.IsDBNull(8)) record.pkgUnitCd = reader.GetString(8);            // Package unit code
                    if (!reader.IsDBNull(9)) record.pkg = Math.Round(reader.GetDouble(9), 2);                  // Package
                    if (!reader.IsDBNull(10)) record.qtyUnitCd = reader.GetString(10);          // Quantity Unit Code
                    if (!reader.IsDBNull(11)) record.qty = Math.Round(reader.GetDouble(11), 2);                // Quantity
                    if (!reader.IsDBNull(12)) record.itemExprDt = reader.GetString(12); // Item Expiration Date
                    if (!reader.IsDBNull(13)) record.prc = Math.Round(reader.GetDouble(13), 2);                // Price
                    if (!reader.IsDBNull(14)) record.splyAmt = Math.Round(reader.GetDouble(14), 2);            // Supply Amount
                    if (!reader.IsDBNull(15)) record.totDcAmt = Math.Round(reader.GetDouble(15), 2);           // Total Discount Amount
                    if (!reader.IsDBNull(16)) record.taxblAmt = Math.Round(reader.GetDouble(16), 2);           // Taxable Amount
                    if (!reader.IsDBNull(17)) record.taxTyCd = reader.GetString(17);            // Taxation Type Code
                    if (!reader.IsDBNull(18)) record.taxAmt = Math.Round(reader.GetDouble(18), 2);             // Tax Amount
                    if (!reader.IsDBNull(19)) record.totAmt = Math.Round(reader.GetDouble(19), 2);             // Total Amount
                    //if (!reader.IsDBNull(20)) record.RegrId = reader.GetString(20);             // Registrant ID
                    //if (!reader.IsDBNull(21)) record.RegrNm = reader.GetString(21);             // Registrant Name
                    //if (!reader.IsDBNull(22)) record.RegDt = reader.GetString(22);      // Registered Date
                    //if (!reader.IsDBNull(23)) record.ModrId = reader.GetString(23);             // Modifier ID
                    //if (!reader.IsDBNull(24)) record.ModrNm = reader.GetString(24);             // Modifier Name
                    //if (!reader.IsDBNull(25)) record.ModDt = reader.GetString(25);      // Modified Date

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

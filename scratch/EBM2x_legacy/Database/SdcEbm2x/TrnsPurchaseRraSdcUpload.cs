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
    /// Description of TrnsPurchaseRraSdcUpload.
    /// </summary>
    public class TrnsPurchaseRraSdcUpload : ModelIO
    {
        public List<TrnsPurchaseSaveReq> getTrnsPurchaseTable(string StartDate, string EndDate)
        {
            List<TrnsPurchaseSaveReq> arrayList = new List<TrnsPurchaseSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsPurchaseTable.GetSelectSQL());
            sql.Append(" WHERE A.PCHS_DT BETWEEN @StartDate AND @EndDate ");

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
                    TrnsPurchaseSaveReq record = new TrnsPurchaseSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.spplrTin = reader.GetString(2);             // Cupplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(3)) record.invcNo = GetLong(reader, 3);               // Invoice No.
                    if (!reader.IsDBNull(4)) record.orgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.taxprNm = reader.GetString(5);              // Taxpayer's Name
                    if (!reader.IsDBNull(6)) record.spplrBhfId = reader.GetString(6);           // Supplier Branch Offie ID
                    if (!reader.IsDBNull(7)) record.spplrNm = reader.GetString(7);              // Customer Name
                    if (!reader.IsDBNull(8)) record.spplrInvcNo = reader.GetInt32(8);         // Supplier Invc No.
                    if (!reader.IsDBNull(9)) record.regTyCd = reader.GetString(9);            // Registration Type Code
                    if (!reader.IsDBNull(10)) record.pchsTyCd = reader.GetString(10);           // Purchase Type Code
                    if (!reader.IsDBNull(11)) record.rcptTyCd = reader.GetString(11);           // Receipt Type Code
                    if (!reader.IsDBNull(12)) record.pmtTyCd = reader.GetString(12);            // Payment Type Code
                    if (!reader.IsDBNull(13)) record.pchsSttsCd = reader.GetString(13);         // Purchase Status Code
                    if (!reader.IsDBNull(14)) record.cfmDt = reader.GetString(14);              // Confirmed Date
                    if (!reader.IsDBNull(15)) record.pchsDt = reader.GetString(15);             // Purchased Date
                    if (!reader.IsDBNull(16)) record.wrhsDt = reader.GetString(16);             // Warehousing Date
                    if (!reader.IsDBNull(17)) record.cnclReqDt = reader.GetString(17);          // Cancel Requested Date
                    if (!reader.IsDBNull(18)) record.cnclDt = reader.GetString(18);             // Canceled Date
                    if (!reader.IsDBNull(19)) record.rfdDt = reader.GetString(19);              // Refunded Date
                    if (!reader.IsDBNull(20)) record.totItemCnt = reader.GetInt16(20);          // Total Item Count
                    if (!reader.IsDBNull(21)) record.taxblAmtA = Math.Round(reader.GetDouble(21), 2);          // Taxable Amount A
                    if (!reader.IsDBNull(22)) record.taxblAmtB = Math.Round(reader.GetDouble(22), 2);          // Taxable Amount B
                    if (!reader.IsDBNull(23)) record.taxblAmtC = Math.Round(reader.GetDouble(23), 2);          // Taxable Amount C
                    if (!reader.IsDBNull(24)) record.taxblAmtD = Math.Round(reader.GetDouble(24), 2);          // Taxable Amount D
                    if (!reader.IsDBNull(24)) record.taxblAmtE = Math.Round(reader.GetDouble(24), 2);          // Taxable Amount E

                    if (!reader.IsDBNull(25)) record.taxRtA = reader.GetInt16(25);             // Tax Rate A
                    if (!reader.IsDBNull(26)) record.taxRtB = reader.GetInt16(26);             // Tax Rate B
                    if (!reader.IsDBNull(27)) record.taxRtC = reader.GetInt16(27);             // Tax Rate C
                    if (!reader.IsDBNull(28)) record.taxRtD = reader.GetInt16(28);             // Tax Rate D
                    if (!reader.IsDBNull(28)) record.taxRtE = reader.GetInt16(28);             // Tax Rate D

                    if (!reader.IsDBNull(29)) record.taxAmtA = Math.Round(reader.GetDouble(29), 2);            // Tax Amount A
                    if (!reader.IsDBNull(30)) record.taxAmtB = Math.Round(reader.GetDouble(30), 2);            // Tax Amount B
                    if (!reader.IsDBNull(31)) record.taxAmtC = Math.Round(reader.GetDouble(31), 2);            // Tax Amount C
                    if (!reader.IsDBNull(32)) record.taxAmtD = Math.Round(reader.GetDouble(32), 2);            // Tax Amount D
                    if (!reader.IsDBNull(32)) record.taxAmtE = Math.Round(reader.GetDouble(32), 2);            // Tax Amount D

                    if (!reader.IsDBNull(33)) record.totTaxblAmt = Math.Round(reader.GetDouble(33), 2);        // Total Taxable Amount
                    if (!reader.IsDBNull(34)) record.totTaxAmt = Math.Round(reader.GetDouble(34), 2);          // Total Tax Amount
                    if (!reader.IsDBNull(35)) record.totAmt = Math.Round(reader.GetDouble(35), 2);             // Total Amount
                    if (!reader.IsDBNull(36)) record.remark = reader.GetString(36);             // Remark
                    if (!reader.IsDBNull(37)) record.regrId = reader.GetString(37);             // Registrant ID
                    if (!reader.IsDBNull(38)) record.regrNm = reader.GetString(38);             // Registrant Name
                    //if (!reader.IsDBNull(39)) record.RegDt = reader.GetString(39);      // Registered Date
                    if (!reader.IsDBNull(40)) record.modrId = reader.GetString(40);             // Modifier ID
                    if (!reader.IsDBNull(41)) record.modrNm = reader.GetString(41);             // Modifier Name
                    //if (!reader.IsDBNull(42)) record.ModDt = reader.GetString(42);      // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsPurchaseSaveReq record in arrayList)
                {
                    record.itemList = getTrnsPurchaseItemTable(record.tin, record.bhfId, record.spplrTin, record.invcNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
        public void SendTranPurchaseSave(string valTin, string valBhfId, string valSpplrTin, long valInvcNo)
        {
            List<TrnsPurchaseSaveReq> sendList = getTrnsPurchaseTable( valTin,  valBhfId,  valSpplrTin,  valInvcNo);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Purchase";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_PURCHASE_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                if (sendList[i].itemList.Count > 0)
                {
                    RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
                }
            }
        }

        public List<TrnsPurchaseSaveReq> getTrnsPurchaseTable(string valTin, string valBhfId, string valSpplrTin, long valInvcNo)
        {
            List<TrnsPurchaseSaveReq> arrayList = new List<TrnsPurchaseSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsPurchaseTable.GetSelectSQL());
            sql.Append(" WHERE INVC_NO = @INVC_NO ");

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
                    TrnsPurchaseSaveReq record = new TrnsPurchaseSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.spplrTin = reader.GetString(2);             // Cupplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(3)) record.invcNo = GetLong(reader, 3);               // Invoice No.
                    if (!reader.IsDBNull(4)) record.orgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.taxprNm = reader.GetString(5);              // Taxpayer's Name
                    if (!reader.IsDBNull(6)) record.spplrBhfId = reader.GetString(6);           // Supplier Branch Offie ID
                    if (!reader.IsDBNull(7)) record.spplrNm = reader.GetString(7);              // Customer Name
                    if (!reader.IsDBNull(8)) record.spplrInvcNo = reader.GetInt32(8);         // Supplier Invc No.
                    if (!reader.IsDBNull(9)) record.regTyCd = reader.GetString(9);            // Registration Type Code
                    if (!reader.IsDBNull(10)) record.pchsTyCd = reader.GetString(10);           // Purchase Type Code
                    if (!reader.IsDBNull(11)) record.rcptTyCd = reader.GetString(11);           // Receipt Type Code
                    if (!reader.IsDBNull(12)) record.pmtTyCd = reader.GetString(12);            // Payment Type Code
                    if (!reader.IsDBNull(13)) record.pchsSttsCd = reader.GetString(13);         // Purchase Status Code
                    if (!reader.IsDBNull(14)) record.cfmDt = reader.GetString(14);              // Confirmed Date
                    if (!reader.IsDBNull(15)) record.pchsDt = reader.GetString(15);             // Purchased Date
                    if (!reader.IsDBNull(16)) record.wrhsDt = reader.GetString(16);             // Warehousing Date
                    if (!reader.IsDBNull(17)) record.cnclReqDt = reader.GetString(17);          // Cancel Requested Date
                    if (!reader.IsDBNull(18)) record.cnclDt = reader.GetString(18);             // Canceled Date
                    if (!reader.IsDBNull(19)) record.rfdDt = reader.GetString(19);              // Refunded Date
                    if (!reader.IsDBNull(20)) record.totItemCnt = Math.Round(reader.GetDouble(20), 2);          // Total Item Count
                    if (!reader.IsDBNull(21)) record.taxblAmtA = Math.Round(reader.GetDouble(21), 2);          // Taxable Amount A
                    if (!reader.IsDBNull(22)) record.taxblAmtB = Math.Round(reader.GetDouble(22), 2);          // Taxable Amount B
                    if (!reader.IsDBNull(23)) record.taxblAmtC = Math.Round(reader.GetDouble(23), 2);          // Taxable Amount C
                    if (!reader.IsDBNull(24)) record.taxblAmtD = Math.Round(reader.GetDouble(24), 2);          // Taxable Amount D
                    if (!reader.IsDBNull(24)) record.taxblAmtE = Math.Round(reader.GetDouble(24), 2);          // Taxable Amount D

                    if (!reader.IsDBNull(25)) record.taxRtA = reader.GetInt16(25);             // Tax Rate A
                    if (!reader.IsDBNull(26)) record.taxRtB = reader.GetInt16(26);             // Tax Rate B
                    if (!reader.IsDBNull(27)) record.taxRtC = reader.GetInt16(27);             // Tax Rate C
                    if (!reader.IsDBNull(28)) record.taxRtD = reader.GetInt16(28);             // Tax Rate D
                    if (!reader.IsDBNull(29)) record.taxAmtA = Math.Round(reader.GetDouble(29), 2);            // Tax Amount A
                    if (!reader.IsDBNull(30)) record.taxAmtB = Math.Round(reader.GetDouble(30), 2);            // Tax Amount B
                    if (!reader.IsDBNull(31)) record.taxAmtC = Math.Round(reader.GetDouble(31), 2);            // Tax Amount C
                    if (!reader.IsDBNull(32)) record.taxAmtD = Math.Round(reader.GetDouble(32), 2);            // Tax Amount D
                    if (!reader.IsDBNull(33)) record.totTaxblAmt = Math.Round(reader.GetDouble(33), 2);        // Total Taxable Amount
                    if (!reader.IsDBNull(34)) record.totTaxAmt = Math.Round(reader.GetDouble(34), 2);          // Total Tax Amount
                    if (!reader.IsDBNull(35)) record.totAmt = Math.Round(reader.GetDouble(35), 2);             // Total Amount
                    if (!reader.IsDBNull(36)) record.remark = reader.GetString(36);             // Remark
                    if (!reader.IsDBNull(37)) record.regrId = reader.GetString(37);             // Registrant ID
                    if (!reader.IsDBNull(38)) record.regrNm = reader.GetString(38);             // Registrant Name
                    //if (!reader.IsDBNull(39)) record.RegDt = reader.GetString(39);      // Registered Date
                    if (!reader.IsDBNull(40)) record.modrId = reader.GetString(40);             // Modifier ID
                    if (!reader.IsDBNull(41)) record.modrNm = reader.GetString(41);             // Modifier Name
                    //if (!reader.IsDBNull(42)) record.ModDt = reader.GetString(42);      // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsPurchaseSaveReq record in arrayList)
                {
                    record.itemList = getTrnsPurchaseItemTable(record.tin, record.bhfId, record.spplrTin, record.invcNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }

        public List<TrnsPurchaseSaveItem> getTrnsPurchaseItemTable(string valTin, string valBhfId, string valSpplrTin, long valInvcNo)
        {
            List<TrnsPurchaseSaveItem> arrayList = new List<TrnsPurchaseSaveItem>();

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
                    TrnsPurchaseSaveItem record = new TrnsPurchaseSaveItem();

                    //if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    //if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    //if (!reader.IsDBNull(2)) record.invcNo = GetLong(reader, 2);               // Invoice No.
                    //if (!reader.IsDBNull(3)) record.spplrTin = reader.GetString(3);             // Supplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(4)) record.itemSeq = reader.GetInt16(4);               // Item Sequence
                    if (!reader.IsDBNull(5)) record.itemCd = reader.GetString(5);               // Item Code
                    if (!reader.IsDBNull(6)) record.itemClsCd = reader.GetString(6);            // Item Classification Code
                    if (!reader.IsDBNull(7)) record.itemNm = reader.GetString(7);               // Item Name
                    if (!reader.IsDBNull(8)) record.bcd = reader.GetString(8);                  // Barcode
                    if (!reader.IsDBNull(9)) record.spplrItemClsCd = reader.GetString(9);       // Supplier Item Classification Code
                    if (!reader.IsDBNull(10)) record.spplrItemCd = reader.GetString(10);        // Supplier Item Code
                    if (!reader.IsDBNull(11)) record.spplrItemNm = reader.GetString(11);        // Supplier Item Name
                    if (!reader.IsDBNull(12)) record.pkgUnitCd = reader.GetString(12);          // Package Unit Code
                    if (!reader.IsDBNull(13)) record.pkg = Math.Round(reader.GetDouble(13), 2);                // Package
                    if (!reader.IsDBNull(14)) record.qtyUnitCd = reader.GetString(14);          // Quantity Unit Code
                    if (!reader.IsDBNull(15)) record.qty = Math.Round(reader.GetDouble(15), 2);                // Quantity
                    if (!reader.IsDBNull(16)) record.prc = Math.Round(reader.GetDouble(16), 2);                // Price
                    if (!reader.IsDBNull(17)) record.splyAmt = Math.Round(reader.GetDouble(17), 2);            // Supply Amount
                    if (!reader.IsDBNull(18)) record.dcRt = Math.Round(reader.GetDouble(18), 2);               // Discount Rate
                    if (!reader.IsDBNull(19)) record.dcAmt = Math.Round(reader.GetDouble(19), 2);              // Discount Amount
                    if (!reader.IsDBNull(20)) record.taxblAmt = Math.Round(reader.GetDouble(20), 2);           // Taxable Amount
                    if (!reader.IsDBNull(21)) record.taxTyCd = reader.GetString(21);            // Taxation Type Code
                    if (!reader.IsDBNull(22)) record.taxAmt = reader.GetDouble(22);             // Tax Amount
                    if (!reader.IsDBNull(23)) record.totAmt = reader.GetDouble(23);             // Total Amount
                    if (!reader.IsDBNull(24)) record.itemExprDt = reader.GetString(24);         // Item Expiration Date
                    //if (!reader.IsDBNull(25)) record.regrId = reader.GetString(25);             // Registrant ID
                    //if (!reader.IsDBNull(26)) record.RegrNm = reader.GetString(26);             // Registrant Name
                    //if (!reader.IsDBNull(27)) record.RegDt = reader.GetString(27);      // Registered Date
                    //if (!reader.IsDBNull(28)) record.ModrId = reader.GetString(28);             // Modifier ID
                    //if (!reader.IsDBNull(29)) record.ModrNm = reader.GetString(29);             // Modifier Name
                    //if (!reader.IsDBNull(30)) record.ModDt = reader.GetString(30);      // Modified Date

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

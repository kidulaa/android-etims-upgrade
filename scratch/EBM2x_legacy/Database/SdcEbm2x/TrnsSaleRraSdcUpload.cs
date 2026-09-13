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
    public class TrnsSaleRraSdcUpload : ModelIO
    {
        public List<TrnsSalesSaveReq> getTrnsSaleTable(string StartDate, string EndDate)
        {
            List<TrnsSalesSaveReq> arrayList = new List<TrnsSalesSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleTable.GetSelectSQL());
            sql.Append(" WHERE A.SALES_DT BETWEEN @StartDate AND @EndDate ");

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
                    TrnsSalesSaveReq record = new TrnsSalesSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    //if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer Name
                    if (!reader.IsDBNull(6)) record.custTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if(!string.IsNullOrEmpty(record.custTin) && record.custTin.Length < 9)
                    {
                        record.custTin = record.custTin.PadRight(9, '0');
                    }
                    //if (!reader.IsDBNull(7)) record.custBhfId = reader.GetString(7);            // Customer Branch Office ID
                    if (!reader.IsDBNull(8)) record.custNm = reader.GetString(8);               // Customer Name
                    if (!reader.IsDBNull(9)) record.salesTyCd = reader.GetString(9);          // Sale Type Code
                    if (!reader.IsDBNull(10)) record.rcptTyCd = reader.GetString(10);           // Receipt Type Code
                    if (!reader.IsDBNull(11)) record.pmtTyCd = reader.GetString(11);            // Payment Type Code
                    if (!reader.IsDBNull(12)) record.salesSttsCd = reader.GetString(12);        // Sale Status Code
                    if (!reader.IsDBNull(13)) record.cfmDt = reader.GetString(13);      // Confirmed Date
                    if (record.cfmDt.Length == 8) record.cfmDt = record.cfmDt + "120101";
                    if (!reader.IsDBNull(14)) record.salesDt = reader.GetString(14);    // Sale Date
                    if (!reader.IsDBNull(15)) record.stockRlsDt = reader.GetString(15); // Stock Released Date
                    if (!reader.IsDBNull(16)) record.cnclReqDt = reader.GetString(16);  // Cancel Reqeuested Date
                    if (!reader.IsDBNull(17)) record.cnclDt = reader.GetString(17);     // Canceled Date
                    if (!reader.IsDBNull(18)) record.rfdDt = reader.GetString(18);      // Refunded Date
                    if (!reader.IsDBNull(19)) record.totItemCnt = reader.GetInt16(19);          // Total Item Count
                    if (!reader.IsDBNull(20)) record.taxblAmtA = Math.Round(reader.GetDouble(20), 2); ;          // Taxable Amount A
                    if (!reader.IsDBNull(21)) record.taxblAmtB = Math.Round(reader.GetDouble(21), 2); ;          // Taxable Amount B
                    if (!reader.IsDBNull(22)) record.taxblAmtC = Math.Round(reader.GetDouble(22), 2); ;          // Taxable Amount C
                    if (!reader.IsDBNull(23)) record.taxblAmtD = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount D
                    if (!reader.IsDBNull(23)) record.taxblAmtE = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount E

                    if (!reader.IsDBNull(24)) record.taxRtA = reader.GetInt16(24);             // Tax Rate A
                    if (!reader.IsDBNull(25)) record.taxRtB = reader.GetInt16(25);             // Tax Rate B
                    if (!reader.IsDBNull(26)) record.taxRtC = reader.GetInt16(26);             // Tax Rate C
                    if (!reader.IsDBNull(27)) record.taxRtD = reader.GetInt16(27);             // Tax Rate D
                    if (!reader.IsDBNull(27)) record.taxRtE = reader.GetInt16(27);             // Tax Rate E

                    if (!reader.IsDBNull(28)) record.taxAmtA = Math.Round(reader.GetDouble(28), 2); ;            // Tax Amount A
                    if (!reader.IsDBNull(29)) record.taxAmtB = Math.Round(reader.GetDouble(29), 2); ;            // Tax Amount B
                    if (!reader.IsDBNull(30)) record.taxAmtC = Math.Round(reader.GetDouble(30), 2); ;            // Tax Amount C
                    if (!reader.IsDBNull(31)) record.taxAmtD = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount D
                    if (!reader.IsDBNull(31)) record.taxAmtE = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount E

                    if (!reader.IsDBNull(32)) record.totTaxblAmt = Math.Round(reader.GetDouble(32), 2); ;        // Total Taxable Amount
                    if (!reader.IsDBNull(33)) record.totTaxAmt = Math.Round(reader.GetDouble(33), 2); ;          // Total Tax Amount
                    if (!reader.IsDBNull(34)) record.totAmt = Math.Round(reader.GetDouble(34), 2);              // Total Amount
                    if (!reader.IsDBNull(35)) record.remark = reader.GetString(35);             // Remark
                    if (!reader.IsDBNull(36)) record.regrId = reader.GetString(36);             // Registrant ID
                    if (!reader.IsDBNull(37)) record.regrNm = reader.GetString(37);             // Registrant Name
                    //if (!reader.IsDBNull(38)) record.RegDt = reader.GetString(38);      // Registered Date
                    if (!reader.IsDBNull(39)) record.modrId = reader.GetString(39);             // Modifier ID
                    if (!reader.IsDBNull(40)) record.modrNm = reader.GetString(40);             // Modifier Name
                    //if (!reader.IsDBNull(41)) record.ModDt = reader.GetString(41);      // Modified Date

                    if (!reader.IsDBNull(42)) record.rfdRsnCd = reader.GetString(42);       // RefundReason
                    //if (!reader.IsDBNull(43)) record.RefundReasonText = reader.GetString(43);   // RefundReasonText

                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length > 9) record.custTin = "";

                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsSalesSaveReq record in arrayList)
                {
                    record.itemList = getTrnsSaleItemTable(record.tin, record.bhfId, record.invcNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
        public List<TrnsSalesSaveReq> getTrnsSaleTable(int fromInvoice, int toInvoice)
        {
            List<TrnsSalesSaveReq> arrayList = new List<TrnsSalesSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleTable.GetSelectSQL());
            sql.Append(" WHERE A.INVC_NO BETWEEN @FromInvoice AND @ToInvoice ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@FromInvoice";
                param.Value = fromInvoice;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ToInvoice";
                param.Value = toInvoice;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSalesSaveReq record = new TrnsSalesSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    //if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer Name
                    if (!reader.IsDBNull(6)) record.custTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length < 9)
                    {
                        record.custTin = record.custTin.PadRight(9, '0');
                    }
                    //if (!reader.IsDBNull(7)) record.custBhfId = reader.GetString(7);            // Customer Branch Office ID
                    if (!reader.IsDBNull(8)) record.custNm = reader.GetString(8);               // Customer Name
                    if (!reader.IsDBNull(9)) record.salesTyCd = reader.GetString(9);          // Sale Type Code
                    if (!reader.IsDBNull(10)) record.rcptTyCd = reader.GetString(10);           // Receipt Type Code
                    if (!reader.IsDBNull(11)) record.pmtTyCd = reader.GetString(11);            // Payment Type Code
                    if (!reader.IsDBNull(12)) record.salesSttsCd = reader.GetString(12);        // Sale Status Code
                    if (!reader.IsDBNull(13)) record.cfmDt = reader.GetString(13);      // Confirmed Date
                    if (record.cfmDt.Length == 8) record.cfmDt = record.cfmDt + "120101";
                    if (!reader.IsDBNull(14)) record.salesDt = reader.GetString(14);    // Sale Date
                    if (!reader.IsDBNull(15)) record.stockRlsDt = reader.GetString(15); // Stock Released Date
                    if (!reader.IsDBNull(16)) record.cnclReqDt = reader.GetString(16);  // Cancel Reqeuested Date
                    if (!reader.IsDBNull(17)) record.cnclDt = reader.GetString(17);     // Canceled Date
                    if (!reader.IsDBNull(18)) record.rfdDt = reader.GetString(18);      // Refunded Date
                    if (!reader.IsDBNull(19)) record.totItemCnt = reader.GetInt16(19);          // Total Item Count
                    if (!reader.IsDBNull(20)) record.taxblAmtA = Math.Round(reader.GetDouble(20), 2); ;          // Taxable Amount A
                    if (!reader.IsDBNull(21)) record.taxblAmtB = Math.Round(reader.GetDouble(21), 2); ;          // Taxable Amount B
                    if (!reader.IsDBNull(22)) record.taxblAmtC = Math.Round(reader.GetDouble(22), 2); ;          // Taxable Amount C
                    if (!reader.IsDBNull(23)) record.taxblAmtD = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount D
                    if (!reader.IsDBNull(23)) record.taxblAmtE = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount E

                    if (!reader.IsDBNull(24)) record.taxRtA = reader.GetInt16(24);             // Tax Rate A
                    if (!reader.IsDBNull(25)) record.taxRtB = reader.GetInt16(25);             // Tax Rate B
                    if (!reader.IsDBNull(26)) record.taxRtC = reader.GetInt16(26);             // Tax Rate C
                    if (!reader.IsDBNull(27)) record.taxRtD = reader.GetInt16(27);             // Tax Rate D
                    if (!reader.IsDBNull(27)) record.taxRtE = reader.GetInt16(27);             // Tax Rate E

                    if (!reader.IsDBNull(28)) record.taxAmtA = Math.Round(reader.GetDouble(28), 2); ;            // Tax Amount A
                    if (!reader.IsDBNull(29)) record.taxAmtB = Math.Round(reader.GetDouble(29), 2); ;            // Tax Amount B
                    if (!reader.IsDBNull(30)) record.taxAmtC = Math.Round(reader.GetDouble(30), 2); ;            // Tax Amount C
                    if (!reader.IsDBNull(31)) record.taxAmtD = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount D
                    if (!reader.IsDBNull(31)) record.taxAmtE = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount D

                    if (!reader.IsDBNull(32)) record.totTaxblAmt = Math.Round(reader.GetDouble(32), 2); ;        // Total Taxable Amount
                    if (!reader.IsDBNull(33)) record.totTaxAmt = Math.Round(reader.GetDouble(33), 2); ;          // Total Tax Amount
                    if (!reader.IsDBNull(34)) record.totAmt = Math.Round(reader.GetDouble(34), 2);              // Total Amount
                    if (!reader.IsDBNull(35)) record.remark = reader.GetString(35);             // Remark
                    if (!reader.IsDBNull(36)) record.regrId = reader.GetString(36);             // Registrant ID
                    if (!reader.IsDBNull(37)) record.regrNm = reader.GetString(37);             // Registrant Name
                    //if (!reader.IsDBNull(38)) record.RegDt = reader.GetString(38);      // Registered Date
                    if (!reader.IsDBNull(39)) record.modrId = reader.GetString(39);             // Modifier ID
                    if (!reader.IsDBNull(40)) record.modrNm = reader.GetString(40);             // Modifier Name
                    //if (!reader.IsDBNull(41)) record.ModDt = reader.GetString(41);      // Modified Date

                    if (!reader.IsDBNull(42)) record.rfdRsnCd = reader.GetString(42);       // RefundReason
                    //if (!reader.IsDBNull(43)) record.RefundReasonText = reader.GetString(43);   // RefundReasonText

                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length > 9) record.custTin = "";

                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsSalesSaveReq record in arrayList)
                {
                    record.itemList = getTrnsSaleItemTable(record.tin, record.bhfId, record.invcNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
        //non reporting Query
        public List<TrnsSalesSaveReq> getNonTrnsSaleTable(long curRcptNo)
        {
            List<TrnsSalesSaveReq> arrayList = new List<TrnsSalesSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleTable.GetSelectSQLNonReporting());
            sql.Append(" WHERE B.CUR_RCPT_NO=@curRcptNo ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@curRcptNo";
                param.Value = curRcptNo;
                command.Parameters.Add(param);
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSalesSaveReq record = new TrnsSalesSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    //if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer Name
                    if (!reader.IsDBNull(6)) record.custTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length < 9)
                    {
                        record.custTin = record.custTin.PadRight(9, '0');
                    }
                    //if (!reader.IsDBNull(7)) record.custBhfId = reader.GetString(7);            // Customer Branch Office ID
                    if (!reader.IsDBNull(8)) record.custNm = reader.GetString(8);               // Customer Name
                    if (!reader.IsDBNull(9)) record.salesTyCd = reader.GetString(9);          // Sale Type Code
                    if (!reader.IsDBNull(10)) record.rcptTyCd = reader.GetString(10);           // Receipt Type Code
                    if (!reader.IsDBNull(11)) record.pmtTyCd = reader.GetString(11);            // Payment Type Code
                    if (!reader.IsDBNull(12)) record.salesSttsCd = reader.GetString(12);        // Sale Status Code
                    if (!reader.IsDBNull(13)) record.cfmDt = reader.GetString(13);      // Confirmed Date
                    if (record.cfmDt.Length == 8) record.cfmDt = record.cfmDt + "120101";
                    if (!reader.IsDBNull(14)) record.salesDt = reader.GetString(14);    // Sale Date
                    if (!reader.IsDBNull(15)) record.stockRlsDt = reader.GetString(15); // Stock Released Date
                    if (!reader.IsDBNull(16)) record.cnclReqDt = reader.GetString(16);  // Cancel Reqeuested Date
                    if (!reader.IsDBNull(17)) record.cnclDt = reader.GetString(17);     // Canceled Date
                    if (!reader.IsDBNull(18)) record.rfdDt = reader.GetString(18);      // Refunded Date
                    if (!reader.IsDBNull(19)) record.totItemCnt = reader.GetInt16(19);          // Total Item Count
                    if (!reader.IsDBNull(20)) record.taxblAmtA = Math.Round(reader.GetDouble(20), 2); ;          // Taxable Amount A
                    if (!reader.IsDBNull(21)) record.taxblAmtB = Math.Round(reader.GetDouble(21), 2); ;          // Taxable Amount B
                    if (!reader.IsDBNull(22)) record.taxblAmtC = Math.Round(reader.GetDouble(22), 2); ;          // Taxable Amount C
                    if (!reader.IsDBNull(23)) record.taxblAmtD = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount D
                    if (!reader.IsDBNull(23)) record.taxblAmtE = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount E

                    if (!reader.IsDBNull(24)) record.taxRtA = reader.GetInt16(24);             // Tax Rate A
                    if (!reader.IsDBNull(25)) record.taxRtB = reader.GetInt16(25);             // Tax Rate B
                    if (!reader.IsDBNull(26)) record.taxRtC = reader.GetInt16(26);             // Tax Rate C
                    if (!reader.IsDBNull(27)) record.taxRtD = reader.GetInt16(27);             // Tax Rate D
                    if (!reader.IsDBNull(27)) record.taxRtE = reader.GetInt16(27);             // Tax Rate E

                    if (!reader.IsDBNull(28)) record.taxAmtA = Math.Round(reader.GetDouble(28), 2); ;            // Tax Amount A
                    if (!reader.IsDBNull(29)) record.taxAmtB = Math.Round(reader.GetDouble(29), 2); ;            // Tax Amount B
                    if (!reader.IsDBNull(30)) record.taxAmtC = Math.Round(reader.GetDouble(30), 2); ;            // Tax Amount C
                    if (!reader.IsDBNull(31)) record.taxAmtD = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount D
                    if (!reader.IsDBNull(31)) record.taxAmtE = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount E

                    if (!reader.IsDBNull(32)) record.totTaxblAmt = Math.Round(reader.GetDouble(32), 2); ;        // Total Taxable Amount
                    if (!reader.IsDBNull(33)) record.totTaxAmt = Math.Round(reader.GetDouble(33), 2); ;          // Total Tax Amount
                    if (!reader.IsDBNull(34)) record.totAmt = Math.Round(reader.GetDouble(34), 2);              // Total Amount
                    if (!reader.IsDBNull(35)) record.remark = reader.GetString(35);             // Remark
                    if (!reader.IsDBNull(36)) record.regrId = reader.GetString(36);             // Registrant ID
                    if (!reader.IsDBNull(37)) record.regrNm = reader.GetString(37);             // Registrant Name
                    //if (!reader.IsDBNull(38)) record.RegDt = reader.GetString(38);      // Registered Date
                    if (!reader.IsDBNull(39)) record.modrId = reader.GetString(39);             // Modifier ID
                    if (!reader.IsDBNull(40)) record.modrNm = reader.GetString(40);             // Modifier Name
                    //if (!reader.IsDBNull(41)) record.ModDt = reader.GetString(41);      // Modified Date

                    if (!reader.IsDBNull(42)) record.rfdRsnCd = reader.GetString(42);       // RefundReason
                    //if (!reader.IsDBNull(43)) record.RefundReasonText = reader.GetString(43);   // RefundReasonText

                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length > 9) record.custTin = "";

                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsSalesSaveReq record in arrayList)
                {
                    record.itemList = getTrnsSaleItemTable(record.tin, record.bhfId, record.invcNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
        //end non reporting 
        public void SendTranSalesSave(string valTin, string valBhfId, long valInvcNo)
        {
            List<TrnsSalesSaveReq> sendList = getTrnsSaleTable( valTin,  valBhfId,  valInvcNo);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "Sales";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                if (sendList[i].itemList.Count > 0)
                {
                    RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
                }
            }
        }
        public List<TrnsSalesSaveReq> getTrnsSaleTable(string valTin, string valBhfId, long valInvcNo)
        {
            List<TrnsSalesSaveReq> arrayList = new List<TrnsSalesSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleTable.GetSelectSQL());
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
                param.ParameterName = "@INVC_NO";
                param.Value = valInvcNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSalesSaveReq record = new TrnsSalesSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    //if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer Name
                    if (!reader.IsDBNull(6)) record.custTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length < 9)
                    {
                        record.custTin = record.custTin.PadRight(9, '0');
                    }
                    //if (!reader.IsDBNull(7)) record.custBhfId = reader.GetString(7);            // Customer Branch Office ID
                    if (!reader.IsDBNull(8)) record.custNm = reader.GetString(8);               // Customer Name
                    if (!reader.IsDBNull(9)) record.salesTyCd = reader.GetString(9);          // Sale Type Code
                    if (!reader.IsDBNull(10)) record.rcptTyCd = reader.GetString(10);           // Receipt Type Code
                    if (!reader.IsDBNull(11)) record.pmtTyCd = reader.GetString(11);            // Payment Type Code
                    if (!reader.IsDBNull(12)) record.salesSttsCd = reader.GetString(12);        // Sale Status Code
                    if (!reader.IsDBNull(13)) record.cfmDt = reader.GetString(13);      // Confirmed Date
                    if (record.cfmDt.Length == 8) record.cfmDt = record.cfmDt + "120101";
                    if (!reader.IsDBNull(14)) record.salesDt = reader.GetString(14);    // Sale Date
                    if (!reader.IsDBNull(15)) record.stockRlsDt = reader.GetString(15); // Stock Released Date
                    if (!reader.IsDBNull(16)) record.cnclReqDt = reader.GetString(16);  // Cancel Reqeuested Date
                    if (!reader.IsDBNull(17)) record.cnclDt = reader.GetString(17);     // Canceled Date
                    if (!reader.IsDBNull(18)) record.rfdDt = reader.GetString(18);      // Refunded Date
                    if (!reader.IsDBNull(19)) record.totItemCnt = reader.GetInt16(19);          // Total Item Count
                    if (!reader.IsDBNull(20)) record.taxblAmtA = Math.Round(reader.GetDouble(20), 2); ;          // Taxable Amount A
                    if (!reader.IsDBNull(21)) record.taxblAmtB = Math.Round(reader.GetDouble(21), 2); ;          // Taxable Amount B
                    if (!reader.IsDBNull(22)) record.taxblAmtC = Math.Round(reader.GetDouble(22), 2); ;          // Taxable Amount C
                    if (!reader.IsDBNull(23)) record.taxblAmtD = Math.Round(reader.GetDouble(23), 2); ;          // Taxable Amount D
                    if (!reader.IsDBNull(24)) record.taxRtA = reader.GetInt16(24);             // Tax Rate A
                    if (!reader.IsDBNull(25)) record.taxRtB = reader.GetInt16(25);             // Tax Rate B
                    if (!reader.IsDBNull(26)) record.taxRtC = reader.GetInt16(26);             // Tax Rate C
                    if (!reader.IsDBNull(27)) record.taxRtD = reader.GetInt16(27);             // Tax Rate D
                    if (!reader.IsDBNull(28)) record.taxAmtA = Math.Round(reader.GetDouble(28), 2); ;            // Tax Amount A
                    if (!reader.IsDBNull(29)) record.taxAmtB = Math.Round(reader.GetDouble(29), 2); ;            // Tax Amount B
                    if (!reader.IsDBNull(30)) record.taxAmtC = Math.Round(reader.GetDouble(30), 2); ;            // Tax Amount C
                    if (!reader.IsDBNull(31)) record.taxAmtD = Math.Round(reader.GetDouble(31), 2); ;            // Tax Amount D
                    if (!reader.IsDBNull(32)) record.totTaxblAmt = Math.Round(reader.GetDouble(32), 2); ;        // Total Taxable Amount
                    if (!reader.IsDBNull(33)) record.totTaxAmt = Math.Round(reader.GetDouble(33), 2); ;          // Total Tax Amount
                    if (!reader.IsDBNull(34)) record.totAmt = Math.Round(reader.GetDouble(34), 2);              // Total Amount
                    if (!reader.IsDBNull(35)) record.remark = reader.GetString(35);             // Remark
                    if (!reader.IsDBNull(36)) record.regrId = reader.GetString(36);             // Registrant ID
                    if (!reader.IsDBNull(37)) record.regrNm = reader.GetString(37);             // Registrant Name
                    //if (!reader.IsDBNull(38)) record.RegDt = reader.GetString(38);      // Registered Date
                    if (!reader.IsDBNull(39)) record.modrId = reader.GetString(39);             // Modifier ID
                    if (!reader.IsDBNull(40)) record.modrNm = reader.GetString(40);             // Modifier Name
                    //if (!reader.IsDBNull(41)) record.ModDt = reader.GetString(41);      // Modified Date

                    if (!reader.IsDBNull(42)) record.rfdRsnCd = reader.GetString(42);       // RefundReason
                    //if (!reader.IsDBNull(43)) record.RefundReasonText = reader.GetString(43);   // RefundReasonText

                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsSalesSaveReq record in arrayList)
                {
                    record.itemList = getTrnsSaleItemTable(record.tin, record.bhfId, record.invcNo);
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
        public List<TrnsSalesSaveItem> getTrnsSaleItemTable(string valTin, string valBhfId, long valInvcNo)
        {
            List<TrnsSalesSaveItem> arrayList = new List<TrnsSalesSaveItem>();

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
                    TrnsSalesSaveItem record = new TrnsSalesSaveItem();

                    //if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                // Taxpayer Identification Number(TIN)
                    //if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);              // Branch Office ID
                    //if (!reader.IsDBNull(2)) record.invcNo = GetLong(reader, 2);              // Invoice No.
                    if (!reader.IsDBNull(3)) record.itemSeq = reader.GetInt16(3);               // Item Sequence
                    if (!reader.IsDBNull(4)) record.itemCd = reader.GetString(4);               // Item Code
                    if (!reader.IsDBNull(5)) record.isrccCd = reader.GetString(5);              // Insurance Company Code
                    if (!reader.IsDBNull(6)) record.itemClsCd = reader.GetString(6);            // Item Classification Code
                    if (!reader.IsDBNull(7)) record.itemNm = reader.GetString(7);               // Item Name
                    if (!reader.IsDBNull(8)) record.bcd = reader.GetString(8);                  // Barcode
                    if (!reader.IsDBNull(9)) record.pkgUnitCd = reader.GetString(9);            // Package Unit Code
                    if (!reader.IsDBNull(10)) record.pkg = Math.Round(reader.GetDouble(10), 2);                // Package
                    if (!reader.IsDBNull(11)) record.qtyUnitCd = reader.GetString(11);          // Quantity Unit Code
                    if (!reader.IsDBNull(12)) record.qty = Math.Round(reader.GetDouble(12), 2);                // Quantity
                    if (!reader.IsDBNull(13)) record.prc = Math.Round(reader.GetDouble(13), 2);                // Unit Price
                    if (!reader.IsDBNull(14)) record.splyAmt = Math.Round(reader.GetDouble(14), 2);            // Supply Price
                    if (!reader.IsDBNull(15)) record.dcRt = reader.GetInt16(15);                // Discount Rate
                    if (!reader.IsDBNull(16)) record.dcAmt = Math.Round(reader.GetDouble(16), 2);              // Discount Amount
                    if (!reader.IsDBNull(17)) record.isrccNm = reader.GetString(17);            // Insurance Company Name
                    if (!reader.IsDBNull(18)) record.isrcRt = reader.GetInt16(18);              // Insurance Rate
                    if (!reader.IsDBNull(19)) record.isrcAmt = Math.Round(reader.GetDouble(19), 2);            // Insurance Amount
                    if (!reader.IsDBNull(20)) record.taxTyCd = reader.GetString(20);            // Tax type code
                    if (!reader.IsDBNull(21)) record.taxblAmt = Math.Round(reader.GetDouble(21), 2);           // Taxable Amount
                    if (!reader.IsDBNull(22)) record.taxAmt = Math.Round(reader.GetDouble(22), 2);             // Tax Amount
                    if (!reader.IsDBNull(23)) record.totAmt = Math.Round(reader.GetDouble(23), 2);             // Total Amount
                    //if (!reader.IsDBNull(24)) record.RegrId = reader.GetString(24);           // Registrant ID
                    //if (!reader.IsDBNull(25)) record.RegrNm = reader.GetString(25);           // Registrant Name
                    //if (!reader.IsDBNull(26)) record.RegDt = reader.GetString(26);            // Registered Date
                    //if (!reader.IsDBNull(27)) record.ModrId = reader.GetString(27);           // Modifier ID
                    //if (!reader.IsDBNull(28)) record.ModrNm = reader.GetString(28);           // Modifier Name
                    //if (!reader.IsDBNull(29)) record.ModDt = reader.GetString(29);            // Modified Date

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

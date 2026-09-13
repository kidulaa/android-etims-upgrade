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
    public class TrnsSaleRcptRraSdcUpload : ModelIO
    {
        public List<TrnsSalesRcptSaveReq> getTrnsSaleRcptTable(string StartDate, string EndDate)
        {
            List<TrnsSalesRcptSaveReq> arrayList = new List<TrnsSalesRcptSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
            sql.Append(" WHERE RCPT_PBCT_DT BETWEEN @StartDate AND @EndDate ");

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
                    TrnsSalesRcptSaveReq record = new TrnsSalesRcptSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = reader.GetInt32(2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = reader.GetInt32(4);             // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.curRcptNo = reader.GetInt32(5);             // Current Receipt No.
                    if (!reader.IsDBNull(6)) record.totRcptNo = reader.GetInt32(6);             // Total Receipt No.
                    //if (!reader.IsDBNull(7)) record.taxprNm = reader.GetString(7);              // Taxpayer Name
                    if (!reader.IsDBNull(8)) record.rcptPbctDt = reader.GetString(8);           // Receipt Published Date
                    if (!reader.IsDBNull(9)) record.intrlData = reader.GetString(9);            // Internal Data
                    if (!reader.IsDBNull(10)) record.rcptSign = reader.GetString(10);           // Receipt Signature
                    if (!reader.IsDBNull(11)) record.jrnl = reader.GetString(11);               // Journal
                    if (!reader.IsDBNull(12)) record.trdeNm = ""; // reader.GetString(12);             // Tradmark Name
                    if (!reader.IsDBNull(13)) record.adrs = reader.GetString(13);               // Address
                    if (!reader.IsDBNull(14)) record.topMsg = reader.GetString(14);             // Top Message
                    if (!reader.IsDBNull(15)) record.btmMsg = reader.GetString(15);             // Bottom Message
                    if (!reader.IsDBNull(16)) record.rptNo = reader.GetInt32(16);               // Receipt No.
                    //if (!reader.IsDBNull(17)) record.rptDt = reader.GetString(17);              // Receipt Date
                    
                    //if (!reader.IsDBNull(18)) record.taskId = reader.GetInt32(18);              // Task ID
                    //if (!reader.IsDBNull(19)) record.taskStrtDt = reader.GetString(19);         // Task Start Date
                    //if (!reader.IsDBNull(20)) record.taskEndDt = reader.GetString(20);          // Task End Date
                    //if (!reader.IsDBNull(21)) record.taskCmptYn = reader.GetString(21);         // Task Completed(Y/N)
                    //if (!reader.IsDBNull(22)) record.audtFile = reader.GetString(22);           // Audit File
                    //if (!reader.IsDBNull(23)) record.audtFileEcrt = reader.GetString(23);       // Audit File Encryption
                    //if (!reader.IsDBNull(24)) record.ebmSendDt = reader.GetString(24);          // EBM Send Date
                    //if (!reader.IsDBNull(25)) record.ebmRes = reader.GetString(25);             // EBM Response
                    //if (!reader.IsDBNull(26)) record.ebmResCd = reader.GetString(26);           // EBM Response Code
                    //if (!reader.IsDBNull(27)) record.scmSignData = reader.GetString(27);        // SCM Signature Date
                    //if (!reader.IsDBNull(28)) record.scmSign = reader.GetString(28);            // SCM Signature
                    //if (!reader.IsDBNull(29)) record.scmSignCfm = reader.GetString(29);         // SCM Signature Confirmation
                    //if (!reader.IsDBNull(30)) record.sdcDt = reader.GetString(30);              // SDC Date
                    
                    if (!reader.IsDBNull(18)) record.regrId = reader.GetString(18);             // Registrant ID
                    if (!reader.IsDBNull(19)) record.regrNm = reader.GetString(19);             // Registrant Name
                    //if (!reader.IsDBNull(20)) record.RegDt = reader.GetString(20);              // Registered Date
                    if (!reader.IsDBNull(21)) record.modrId = reader.GetString(21);             // Modifier ID
                    if (!reader.IsDBNull(22)) record.modrNm = reader.GetString(22);             // Modifier Name
                    //if (!reader.IsDBNull(23)) record.ModDt = reader.GetString(23);              // Modified Date
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

        public List<TrnsSalesRcptSaveReq> getTrnsSaleRcptTable(int fromInvoice, int toInvoice)
        {
            List<TrnsSalesRcptSaveReq> arrayList = new List<TrnsSalesRcptSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
            sql.Append(" WHERE INVC_NO BETWEEN @FromInvoice AND @ToInvoice ");

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
                    TrnsSalesRcptSaveReq record = new TrnsSalesRcptSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = reader.GetInt32(2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = reader.GetInt32(4);             // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.curRcptNo = reader.GetInt32(5);             // Current Receipt No.
                    if (!reader.IsDBNull(6)) record.totRcptNo = reader.GetInt32(6);             // Total Receipt No.
                    //if (!reader.IsDBNull(7)) record.taxprNm = reader.GetString(7);              // Taxpayer Name
                    if (!reader.IsDBNull(8)) record.rcptPbctDt = reader.GetString(8);           // Receipt Published Date
                    if (!reader.IsDBNull(9)) record.intrlData = reader.GetString(9);            // Internal Data
                    if (!reader.IsDBNull(10)) record.rcptSign = reader.GetString(10);           // Receipt Signature
                    if (!reader.IsDBNull(11)) record.jrnl = reader.GetString(11);               // Journal
                    if (!reader.IsDBNull(12)) record.trdeNm = ""; // reader.GetString(12);             // Tradmark Name
                    if (!reader.IsDBNull(13)) record.adrs = reader.GetString(13);               // Address
                    if (!reader.IsDBNull(14)) record.topMsg = reader.GetString(14);             // Top Message
                    if (!reader.IsDBNull(15)) record.btmMsg = reader.GetString(15);             // Bottom Message
                    if (!reader.IsDBNull(16)) record.rptNo = reader.GetInt32(16);               // Receipt No.
                                                                                                //if (!reader.IsDBNull(17)) record.rptDt = reader.GetString(17);              // Receipt Date

                    //if (!reader.IsDBNull(18)) record.taskId = reader.GetInt32(18);              // Task ID
                    //if (!reader.IsDBNull(19)) record.taskStrtDt = reader.GetString(19);         // Task Start Date
                    //if (!reader.IsDBNull(20)) record.taskEndDt = reader.GetString(20);          // Task End Date
                    //if (!reader.IsDBNull(21)) record.taskCmptYn = reader.GetString(21);         // Task Completed(Y/N)
                    //if (!reader.IsDBNull(22)) record.audtFile = reader.GetString(22);           // Audit File
                    //if (!reader.IsDBNull(23)) record.audtFileEcrt = reader.GetString(23);       // Audit File Encryption
                    //if (!reader.IsDBNull(24)) record.ebmSendDt = reader.GetString(24);          // EBM Send Date
                    //if (!reader.IsDBNull(25)) record.ebmRes = reader.GetString(25);             // EBM Response
                    //if (!reader.IsDBNull(26)) record.ebmResCd = reader.GetString(26);           // EBM Response Code
                    //if (!reader.IsDBNull(27)) record.scmSignData = reader.GetString(27);        // SCM Signature Date
                    //if (!reader.IsDBNull(28)) record.scmSign = reader.GetString(28);            // SCM Signature
                    //if (!reader.IsDBNull(29)) record.scmSignCfm = reader.GetString(29);         // SCM Signature Confirmation
                    //if (!reader.IsDBNull(30)) record.sdcDt = reader.GetString(30);              // SDC Date

                    if (!reader.IsDBNull(18)) record.regrId = reader.GetString(18);             // Registrant ID
                    if (!reader.IsDBNull(19)) record.regrNm = reader.GetString(19);             // Registrant Name
                    //if (!reader.IsDBNull(20)) record.RegDt = reader.GetString(20);              // Registered Date
                    if (!reader.IsDBNull(21)) record.modrId = reader.GetString(21);             // Modifier ID
                    if (!reader.IsDBNull(22)) record.modrNm = reader.GetString(22);             // Modifier Name
                    //if (!reader.IsDBNull(23)) record.ModDt = reader.GetString(23);              // Modified Date
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
        //added by Aime for non reporting
        public List<TrnsSalesRcptSaveReq> getNonReportingTrnsSaleRcptTable(long curRcptNo)
        {
            List<TrnsSalesRcptSaveReq> arrayList = new List<TrnsSalesRcptSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
            sql.Append(" WHERE CUR_RCPT_NO=@curRcptNo ");

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
                    TrnsSalesRcptSaveReq record = new TrnsSalesRcptSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = reader.GetInt32(2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = reader.GetInt32(4);             // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.curRcptNo = reader.GetInt32(5);             // Current Receipt No.
                    if (!reader.IsDBNull(6)) record.totRcptNo = reader.GetInt32(6);             // Total Receipt No.
                    //if (!reader.IsDBNull(7)) record.taxprNm = reader.GetString(7);              // Taxpayer Name
                    if (!reader.IsDBNull(8)) record.rcptPbctDt = reader.GetString(8);           // Receipt Published Date
                    if (!reader.IsDBNull(9)) record.intrlData = reader.GetString(9);            // Internal Data
                    if (!reader.IsDBNull(10)) record.rcptSign = reader.GetString(10);           // Receipt Signature
                    if (!reader.IsDBNull(11)) record.jrnl = reader.GetString(11);               // Journal
                    if (!reader.IsDBNull(12)) record.trdeNm = ""; // reader.GetString(12);             // Tradmark Name
                    if (!reader.IsDBNull(13)) record.adrs = reader.GetString(13);               // Address
                    if (!reader.IsDBNull(14)) record.topMsg = reader.GetString(14);             // Top Message
                    if (!reader.IsDBNull(15)) record.btmMsg = reader.GetString(15);             // Bottom Message
                    if (!reader.IsDBNull(16)) record.rptNo = reader.GetInt32(16);               // Receipt No.
                                                                                                //if (!reader.IsDBNull(17)) record.rptDt = reader.GetString(17);              // Receipt Date

                    //if (!reader.IsDBNull(18)) record.taskId = reader.GetInt32(18);              // Task ID
                    //if (!reader.IsDBNull(19)) record.taskStrtDt = reader.GetString(19);         // Task Start Date
                    //if (!reader.IsDBNull(20)) record.taskEndDt = reader.GetString(20);          // Task End Date
                    //if (!reader.IsDBNull(21)) record.taskCmptYn = reader.GetString(21);         // Task Completed(Y/N)
                    //if (!reader.IsDBNull(22)) record.audtFile = reader.GetString(22);           // Audit File
                    //if (!reader.IsDBNull(23)) record.audtFileEcrt = reader.GetString(23);       // Audit File Encryption
                    //if (!reader.IsDBNull(24)) record.ebmSendDt = reader.GetString(24);          // EBM Send Date
                    //if (!reader.IsDBNull(25)) record.ebmRes = reader.GetString(25);             // EBM Response
                    //if (!reader.IsDBNull(26)) record.ebmResCd = reader.GetString(26);           // EBM Response Code
                    //if (!reader.IsDBNull(27)) record.scmSignData = reader.GetString(27);        // SCM Signature Date
                    //if (!reader.IsDBNull(28)) record.scmSign = reader.GetString(28);            // SCM Signature
                    //if (!reader.IsDBNull(29)) record.scmSignCfm = reader.GetString(29);         // SCM Signature Confirmation
                    //if (!reader.IsDBNull(30)) record.sdcDt = reader.GetString(30);              // SDC Date

                    if (!reader.IsDBNull(18)) record.regrId = reader.GetString(18);             // Registrant ID
                    if (!reader.IsDBNull(19)) record.regrNm = reader.GetString(19);             // Registrant Name
                    //if (!reader.IsDBNull(20)) record.RegDt = reader.GetString(20);              // Registered Date
                    if (!reader.IsDBNull(21)) record.modrId = reader.GetString(21);             // Modifier ID
                    if (!reader.IsDBNull(22)) record.modrNm = reader.GetString(22);             // Modifier Name
                    //if (!reader.IsDBNull(23)) record.ModDt = reader.GetString(23);              // Modified Date
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
        //end by Added by Aime For Non Reporting

        public void SendTranSalesRcptSave(string valTin, string valBhfId, long valInvcNo)
        {
            List<TrnsSalesRcptSaveReq> sendList = getTrnsSaleRcptTable( valTin,  valBhfId,  valInvcNo);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "SalesReceipt";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_TRNS_SALES_RECEIPT_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<TrnsSalesRcptSaveReq> getTrnsSaleRcptTable(string valTin, string valBhfId, long valInvcNo)
        {
            List<TrnsSalesRcptSaveReq> arrayList = new List<TrnsSalesRcptSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
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

                TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSalesRcptSaveReq record = new TrnsSalesRcptSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.invcNo = reader.GetInt32(2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.prchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.orgInvcNo = reader.GetInt32(4);             // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.curRcptNo = reader.GetInt32(5);             // Current Receipt No.
                    if (!reader.IsDBNull(6)) record.totRcptNo = reader.GetInt32(6);             // Total Receipt No.
                    //if (!reader.IsDBNull(7)) record.taxprNm = reader.GetString(7);              // Taxpayer Name
                    if (!reader.IsDBNull(8)) record.rcptPbctDt = reader.GetString(8);           // Receipt Published Date
                    if (!reader.IsDBNull(9)) record.intrlData = reader.GetString(9);            // Internal Data
                    if (!reader.IsDBNull(10)) record.rcptSign = reader.GetString(10);           // Receipt Signature
                    if (!reader.IsDBNull(11)) record.jrnl = reader.GetString(11);               // Journal
                    if (!reader.IsDBNull(12)) record.trdeNm = ""; // reader.GetString(12);             // Tradmark Name
                    if (!reader.IsDBNull(13)) record.adrs = reader.GetString(13);               // Address
                    if (!reader.IsDBNull(14)) record.topMsg = reader.GetString(14);             // Top Message
                    if (!reader.IsDBNull(15)) record.btmMsg = reader.GetString(15);             // Bottom Message

                    if (!reader.IsDBNull(16)) record.rptNo = reader.GetInt32(16);               // Receipt No.
                    //if (!reader.IsDBNull(17)) record.rptDt = reader.GetString(17);              // Receipt Date
                    //if (!reader.IsDBNull(18)) record.taskId = reader.GetInt32(18);              // Task ID
                    //if (!reader.IsDBNull(19)) record.taskStrtDt = reader.GetString(19);         // Task Start Date
                    //if (!reader.IsDBNull(20)) record.taskEndDt = reader.GetString(20);          // Task End Date
                    //if (!reader.IsDBNull(21)) record.taskCmptYn = reader.GetString(21);         // Task Completed(Y/N)
                    //if (!reader.IsDBNull(22)) record.audtFile = reader.GetString(22);           // Audit File
                    //if (!reader.IsDBNull(23)) record.audtFileEcrt = reader.GetString(23);       // Audit File Encryption
                    //if (!reader.IsDBNull(24)) record.ebmSendDt = reader.GetString(24);          // EBM Send Date
                    //if (!reader.IsDBNull(25)) record.ebmRes = reader.GetString(25);             // EBM Response
                    //if (!reader.IsDBNull(26)) record.ebmResCd = reader.GetString(26);           // EBM Response Code
                    //if (!reader.IsDBNull(27)) record.scmSignData = reader.GetString(27);        // SCM Signature Date
                    //if (!reader.IsDBNull(28)) record.scmSign = reader.GetString(28);            // SCM Signature
                    //if (!reader.IsDBNull(29)) record.scmSignCfm = reader.GetString(29);         // SCM Signature Confirmation
                    //if (!reader.IsDBNull(30)) record.sdcDt = reader.GetString(30);              // SDC Date

                    if (!reader.IsDBNull(18)) record.regrId = reader.GetString(18);             // Registrant ID
                    if (!reader.IsDBNull(19)) record.regrNm = reader.GetString(19);             // Registrant Name
                    //if (!reader.IsDBNull(20)) record.RegDt = reader.GetString(20);              // Registered Date
                    if (!reader.IsDBNull(21)) record.modrId = reader.GetString(21);             // Modifier ID
                    if (!reader.IsDBNull(22)) record.modrNm = reader.GetString(22);             // Modifier Name
                    //if (!reader.IsDBNull(23)) record.ModDt = reader.GetString(23);              // Modified Date
                    if (string.IsNullOrEmpty(record.regrId)) record.regrId = "system";             // Registrant ID
                    if (string.IsNullOrEmpty(record.regrNm)) record.regrNm = "system";             // Registrant Name
                    if (string.IsNullOrEmpty(record.modrId)) record.modrId = "system";             // Modifier ID
                    if (string.IsNullOrEmpty(record.modrNm)) record.modrNm = "system";             // Modifier Name

                    arrayList.Add(record);
                }
                reader.Close();

                foreach (TrnsSalesRcptSaveReq record in arrayList)
                {
                    TrnsSaleRecord trnsSaleRecord = new TrnsSaleRecord();
                    trnsSaleMaster.ToRecord(trnsSaleRecord, valTin, valBhfId, valInvcNo);

                    // JCNA : custTin, custMblNo 분리
                    if (!string.IsNullOrEmpty(trnsSaleRecord.CustTin))
                    {
                        if (string.IsNullOrEmpty(trnsSaleRecord.CustNm))
                        {
                            record.custMblNo = trnsSaleRecord.CustTin;
                        }
                        else
                        {
                            record.custTin = trnsSaleRecord.CustTin;
                            if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length > 9) record.custTin = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            return arrayList;
        }
    }
}

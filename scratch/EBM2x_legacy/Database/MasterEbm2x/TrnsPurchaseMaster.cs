using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.UI;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of TrnsPurchaseMaster.
    /// </summary>
    public class TrnsPurchaseMaster : ModelIO
    {
        public long GetWaitCount()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT COUNT(*) FROM TRNS_PURCHASE WHERE PCHS_STTS_CD = '01'";
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

        public long GetPurchaseSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(INVC_NO), 0)  FROM TRNS_PURCHASE";
                command.CommandType = CommandType.Text;

                long PurchaseSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    PurchaseSeq = long.Parse(firstColumn.ToString());
                }
                if (PurchaseSeq == 0)
                {
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.LastPchsInvcNo > 0)
                    {
                        PurchaseSeq = UIManager.Instance().PosModel.Environment.EnvPosSetup.LastPchsInvcNo;
                    }
                }
                return (long)PurchaseSeq + 1;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 1;
            }
        }
        public bool GetPurchaseInvoice(string valSpplrTin, long valSpplrInvcNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return false;

            try
            {
                command.CommandText = "SELECT count(*)  FROM TRNS_PURCHASE where SPPLR_TIN = @SPPLR_TIN and SPPLR_INVC_NO = @SPPLR_INVC_NO";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@SPPLR_TIN";
                param.Value = valSpplrTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SPPLR_INVC_NO";
                param.Value = valSpplrInvcNo;
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

        public List<TrnsPurchaseRecordVAT> getTrnsPurchaseTableVAT(string StartDate, string EndDate)
        {
            List<TrnsPurchaseRecordVAT> arrayList = new List<TrnsPurchaseRecordVAT>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT tp.INVC_NO ");
            sql.Append("      , tp.SPPLR_TIN ");
            sql.Append("      , tp.SPPLR_NM ");
            sql.Append("      , tp.SPPLR_INVC_NO ");
            sql.Append("      , tp.PCHS_DT ");
            sql.Append("      , tp.REG_DT AS RCPT_PBCT_DT ");
            sql.Append("      , tp.TOT_AMT * CASE WHEN tp.RCPT_TY_CD = 'P' THEN 1 ELSE -1 END AS TOT_AMT ");
            sql.Append("      , tp.TOT_TAX_AMT * CASE WHEN tp.RCPT_TY_CD = 'P' THEN 1 ELSE -1 END AS TOT_VAT ");
            sql.Append("   FROM TRNS_PURCHASE tp ");
            sql.Append("  WHERE tp.PCHS_DT BETWEEN @StartDate AND @EndDate ");

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
                    TrnsPurchaseRecordVAT record = new TrnsPurchaseRecordVAT();

                    if (!reader.IsDBNull(0)) record.InvcNo = GetLong(reader, 0);                // Invoice No.
                    if (!reader.IsDBNull(1)) record.SpplrTin = reader.GetString(1);             // Cupplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(2)) record.SpplrNm = reader.GetString(2);              // Customer Name
                    if (!reader.IsDBNull(3)) record.SpplrInvcNo = GetLong(reader, 3);           // Registration Type Code
                    if (!reader.IsDBNull(4)) record.PchsDt = reader.GetString(4);               // Purchased Date
                    if (!reader.IsDBNull(5)) record.RcptPbctDt = reader.GetString(5);           // RcptPbctDt Date
                    if (!reader.IsDBNull(6)) record.TotAmt = reader.GetDouble(6);               // Total Amount
                    if (!reader.IsDBNull(7)) record.TotTaxAmt = reader.GetDouble(7);            // Total Tax Amount

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

        public List<TrnsPurchaseRecord> getTrnsPurchaseTable(string StartDate, string EndDate, long InvoiceNo, string valuePchsStts)
        {
            List<TrnsPurchaseRecord> arrayList = new List<TrnsPurchaseRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsPurchaseTable.GetSelectSQL());
            sql.Append(" WHERE A.PCHS_DT BETWEEN @StartDate AND @EndDate ");
            if (InvoiceNo > 0)
            {
                sql.Append("   AND A.INVC_NO Like @InvoiceNo ");
            }
            if (!string.IsNullOrEmpty(valuePchsStts)) sql.Append("   and PCHS_STTS_CD = @PCHS_STTS_CD");
            sql.Append(" ORDER BY A.INVC_NO DESC ");

            TaxpayerBhfCustMaster taxpayerBhfCustMaster = new TaxpayerBhfCustMaster();
            CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
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

                if (InvoiceNo > 0)
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@InvoiceNo";
                    param.Value = InvoiceNo;
                    command.Parameters.Add(param);
                }

                param = command.CreateParameter();
                param.ParameterName = "@PCHS_STTS_CD";
                param.Value = valuePchsStts;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsPurchaseRecord record = new TrnsPurchaseRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.SpplrTin = reader.GetString(2);             // Cupplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(3)) record.InvcNo = GetLong(reader, 3);               // Invoice No.
                    if (!reader.IsDBNull(4)) record.OrgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer's Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(6)) record.DvcId = reader.GetString(6);                // Device ID
                    if (!reader.IsDBNull(6)) record.SpplrBhfId = reader.GetString(6);           // Supplier Branch Offie ID
                    if (!reader.IsDBNull(7)) record.SpplrNm = reader.GetString(7);              // Customer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(9)) record.SpplrDvcId = reader.GetString(9);           // Supplier Device ID
                    if (!reader.IsDBNull(8)) record.SpplrInvcNo = reader.GetInt32(8);         // Supplier Receipt No.
                    if (!reader.IsDBNull(9)) record.RegTyCd = reader.GetString(9);            // Registration Type Code
                    if (!reader.IsDBNull(10)) record.PchsTyCd = reader.GetString(10);           // Purchase Type Code
                    if (!reader.IsDBNull(11)) record.RcptTyCd = reader.GetString(11);           // Receipt Type Code
                    if (!reader.IsDBNull(12)) record.PmtTyCd = reader.GetString(12);            // Payment Type Code
                    if (!reader.IsDBNull(13)) record.PchsSttsCd = reader.GetString(13);         // Purchase Status Code
                    if (!reader.IsDBNull(14)) record.CfmDt = reader.GetString(14);              // Confirmed Date
                    if (!reader.IsDBNull(15)) record.PchsDt = reader.GetString(15);             // Purchased Date
                    if (!reader.IsDBNull(16)) record.WrhsDt = reader.GetString(16);             // Warehousing Date
                    if (!reader.IsDBNull(17)) record.CnclReqDt = reader.GetString(17);          // Cancel Requested Date
                    if (!reader.IsDBNull(18)) record.CnclDt = reader.GetString(18);             // Canceled Date
                    if (!reader.IsDBNull(19)) record.RfdDt = reader.GetString(19);              // Refunded Date
                    if (!reader.IsDBNull(20)) record.TotItemCnt = reader.GetInt16(20);          // Total Item Count
                    if (!reader.IsDBNull(21)) record.TaxblAmtA = reader.GetDouble(21);          // Taxable Amount A
                    if (!reader.IsDBNull(22)) record.TaxblAmtB = reader.GetDouble(22);          // Taxable Amount B
                    if (!reader.IsDBNull(23)) record.TaxblAmtC = reader.GetDouble(23);          // Taxable Amount C
                    if (!reader.IsDBNull(24)) record.TaxblAmtD = reader.GetDouble(24);          // Taxable Amount D
                    if (!reader.IsDBNull(24)) record.TaxblAmtE = reader.GetDouble(24);          // Taxable Amount E

                    if (!reader.IsDBNull(25)) record.TaxRtA = reader.GetInt16(25);             // Tax Rate A
                    if (!reader.IsDBNull(26)) record.TaxRtB = reader.GetInt16(26);             // Tax Rate B
                    if (!reader.IsDBNull(27)) record.TaxRtC = reader.GetInt16(27);             // Tax Rate C
                    if (!reader.IsDBNull(28)) record.TaxRtD = reader.GetInt16(28);             // Tax Rate D
                    if (!reader.IsDBNull(28)) record.TaxRtE = reader.GetInt16(28);             // Tax Rate E

                    if (!reader.IsDBNull(29)) record.TaxAmtA = reader.GetDouble(29);            // Tax Amount A
                    if (!reader.IsDBNull(30)) record.TaxAmtB = reader.GetDouble(30);            // Tax Amount B
                    if (!reader.IsDBNull(31)) record.TaxAmtC = reader.GetDouble(31);            // Tax Amount C
                    if (!reader.IsDBNull(32)) record.TaxAmtD = reader.GetDouble(32);            // Tax Amount D
                    if (!reader.IsDBNull(32)) record.TaxAmtE = reader.GetDouble(32);            // Tax Amount E

                    if (!reader.IsDBNull(33)) record.TotTaxblAmt = reader.GetDouble(33);        // Total Taxable Amount
                    if (!reader.IsDBNull(34)) record.TotTaxAmt = reader.GetDouble(34);          // Total Tax Amount
                    if (!reader.IsDBNull(35)) record.TotAmt = reader.GetDouble(35);             // Total Amount
                    if (!reader.IsDBNull(36)) record.Remark = reader.GetString(36);             // Remark
                    if (!reader.IsDBNull(37)) record.RegrId = reader.GetString(37);             // Registrant ID
                    if (!reader.IsDBNull(38)) record.RegrNm = reader.GetString(38);             // Registrant Name
                    if (!reader.IsDBNull(39)) record.RegDt = reader.GetString(39);      // Registered Date
                    if (!reader.IsDBNull(40)) record.ModrId = reader.GetString(40);             // Modifier ID
                    if (!reader.IsDBNull(41)) record.ModrNm = reader.GetString(41);             // Modifier Name
                    if (!reader.IsDBNull(42)) record.ModDt = reader.GetString(42);      // Modified Date
                    if (!reader.IsDBNull(43)) record.Rowid = reader.GetInt16(43);

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TrnsPurchaseRecord record in arrayList)
            {
                record.PchsSttsNm = codeDtlMaster.PchsSttsName(record.PchsSttsCd);
                record.TradeNm = taxpayerBhfCustMaster.GetTaxpayerBhfCustName(record.Tin, record.BhfId, record.SpplrTin);
            }

            return arrayList;
        }

        public bool ToRecord(TrnsPurchaseRecord record, string valTin, string valBhfId, string valSpplrTin, string valInvcNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(trnsPurchaseTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND SPPLR_TIN = @SPPLR_TIN ");
            sql.Append("   AND INVC_NO = @INVC_NO ");

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

                TaxpayerBhfCustMaster taxpayerBhfCustMaster = new TaxpayerBhfCustMaster();
                CodeDtlMaster codeDtlMaster = new CodeDtlMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.SpplrTin = reader.GetString(2);             // Cupplier Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(3)) record.InvcNo = GetLong(reader, 3);               // Invoice No.
                    if (!reader.IsDBNull(4)) record.OrgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer's Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(6)) record.DvcId = reader.GetString(6);                // Device ID
                    if (!reader.IsDBNull(6)) record.SpplrBhfId = reader.GetString(6);           // Supplier Branch Offie ID
                    if (!reader.IsDBNull(7)) record.SpplrNm = reader.GetString(7);              // Customer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(9)) record.SpplrDvcId = reader.GetString(9);           // Supplier Device ID
                    if (!reader.IsDBNull(8)) record.SpplrInvcNo = reader.GetInt32(8);         // Supplier Receipt No.
                    if (!reader.IsDBNull(9)) record.RegTyCd = reader.GetString(9);            // Registration Type Code
                    if (!reader.IsDBNull(10)) record.PchsTyCd = reader.GetString(10);           // Purchase Type Code
                    if (!reader.IsDBNull(11)) record.RcptTyCd = reader.GetString(11);           // Receipt Type Code
                    if (!reader.IsDBNull(12)) record.PmtTyCd = reader.GetString(12);            // Payment Type Code
                    if (!reader.IsDBNull(13)) record.PchsSttsCd = reader.GetString(13);         // Purchase Status Code
                    if (!reader.IsDBNull(14)) record.CfmDt = reader.GetString(14);              // Confirmed Date
                    if (!reader.IsDBNull(15)) record.PchsDt = reader.GetString(15);             // Purchased Date
                    if (!reader.IsDBNull(16)) record.WrhsDt = reader.GetString(16);             // Warehousing Date
                    if (!reader.IsDBNull(17)) record.CnclReqDt = reader.GetString(17);          // Cancel Requested Date
                    if (!reader.IsDBNull(18)) record.CnclDt = reader.GetString(18);             // Canceled Date
                    if (!reader.IsDBNull(19)) record.RfdDt = reader.GetString(19);              // Refunded Date
                    if (!reader.IsDBNull(20)) record.TotItemCnt = reader.GetInt16(20);          // Total Item Count
                    if (!reader.IsDBNull(21)) record.TaxblAmtA = reader.GetDouble(21);          // Taxable Amount A
                    if (!reader.IsDBNull(22)) record.TaxblAmtB = reader.GetDouble(22);          // Taxable Amount B
                    if (!reader.IsDBNull(23)) record.TaxblAmtC = reader.GetDouble(23);          // Taxable Amount C
                    if (!reader.IsDBNull(24)) record.TaxblAmtD = reader.GetDouble(24);          // Taxable Amount D
                    if (!reader.IsDBNull(24)) record.TaxblAmtE = reader.GetDouble(24);          // Taxable Amount E

                    if (!reader.IsDBNull(25)) record.TaxRtA = reader.GetInt16(25);             // Tax Rate A
                    if (!reader.IsDBNull(26)) record.TaxRtB = reader.GetInt16(26);             // Tax Rate B
                    if (!reader.IsDBNull(27)) record.TaxRtC = reader.GetInt16(27);             // Tax Rate C
                    if (!reader.IsDBNull(28)) record.TaxRtD = reader.GetInt16(28);             // Tax Rate D
                    if (!reader.IsDBNull(28)) record.TaxRtE = reader.GetInt16(28);             // Tax Rate E

                    if (!reader.IsDBNull(29)) record.TaxAmtA = reader.GetDouble(29);            // Tax Amount A
                    if (!reader.IsDBNull(30)) record.TaxAmtB = reader.GetDouble(30);            // Tax Amount B
                    if (!reader.IsDBNull(31)) record.TaxAmtC = reader.GetDouble(31);            // Tax Amount C
                    if (!reader.IsDBNull(32)) record.TaxAmtD = reader.GetDouble(32);            // Tax Amount D
                    if (!reader.IsDBNull(32)) record.TaxAmtE = reader.GetDouble(32);            // Tax Amount E

                    if (!reader.IsDBNull(33)) record.TotTaxblAmt = reader.GetDouble(33);        // Total Taxable Amount
                    if (!reader.IsDBNull(34)) record.TotTaxAmt = reader.GetDouble(34);          // Total Tax Amount
                    if (!reader.IsDBNull(35)) record.TotAmt = reader.GetDouble(35);             // Total Amount
                    if (!reader.IsDBNull(36)) record.Remark = reader.GetString(36);             // Remark
                    if (!reader.IsDBNull(37)) record.RegrId = reader.GetString(37);             // Registrant ID
                    if (!reader.IsDBNull(38)) record.RegrNm = reader.GetString(38);             // Registrant Name
                    if (!reader.IsDBNull(39)) record.RegDt = reader.GetString(39);      // Registered Date
                    if (!reader.IsDBNull(40)) record.ModrId = reader.GetString(40);             // Modifier ID
                    if (!reader.IsDBNull(41)) record.ModrNm = reader.GetString(41);             // Modifier Name
                    if (!reader.IsDBNull(42)) record.ModDt = reader.GetString(42);      // Modified Date
                    if (!reader.IsDBNull(43)) record.Rowid = reader.GetInt16(43);

                    reader.Close();

                    record.PchsSttsNm = codeDtlMaster.PchsSttsName(record.PchsSttsCd);
                    record.TradeNm = taxpayerBhfCustMaster.GetTaxpayerBhfCustName(record.Tin, record.BhfId, record.SpplrTin);

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

        public bool ToTable(TrnsPurchaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();

            try
            {
                command.Parameters.Clear();
                trnsPurchaseTable.SetParameters(command, record);

                command.CommandText = trnsPurchaseTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = trnsPurchaseTable.GetInsertSQL();
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

        public bool InsertTable(TrnsPurchaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();

            try
            {
                command.CommandText = trnsPurchaseTable.GetInsertSQL();
   
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsPurchaseTable.SetParameters(command, record);
            
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool DeleteTable(TrnsPurchaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsPurchaseTable trnsPurchaseTable = new TrnsPurchaseTable();

            try
            {
                command.CommandText = trnsPurchaseTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsPurchaseTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool UpdateTable(string pchsSttsCd, TrnsPurchaseRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE TRNS_PURCHASE ");
            sql.Append("   SET PCHS_STTS_CD = @PCHS_STTS_CD, ");
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt,  ");         // Modified Date
            switch (pchsSttsCd)
            {
                case "02":
                    sql.Append("  CFM_DT = @CfmDt, ");
                    sql.Append("  WRHS_DT = @CfmDt");
                    break;
                case "03":
                    sql.Append("  CNCL_REQ_DT = @CnclReqDt");
                    break;
                case "04":
                    sql.Append("  CNCL_DT = @CnclDt");
                    break;
                case "06":
                    /*
                    strQuery = strQuery & "  INV_STATUS_CD = '" & strInvStatusCd & "', "
                    strQuery = strQuery & "  BHF_ID = '" & cboBranchOffice.SelectedValue.ToString.Trim & "', "
                    strQuery = strQuery + "  REG_TY_CD = 'T', COMM_F='N' "
                    */
                    break;
            }
            sql.Append(" WHERE TIN = @TIN");
            sql.Append("   AND BHF_ID = @BHF_ID");
            sql.Append("   AND INVC_NO = @INVC_NO");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;
                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@PCHS_STTS_CD";
                param.Value = pchsSttsCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ModrId";
                param.Value = record.ModrId;
                command.Parameters.Add(param);                  // Modifier ID

                param = command.CreateParameter();
                param.ParameterName = "@ModrNm";
                param.Value = record.ModrNm;
                command.Parameters.Add(param);                  // Modifier Name

                param = command.CreateParameter();
                param.ParameterName = "@ModDt";
                param.Value = record.ModDt;
                command.Parameters.Add(param);                  // Modified Date
                
                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = record.Tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = record.BhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@INVC_NO";
                param.Value = record.InvcNo;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CfmDt";
                param.Value = record.CfmDt;
                command.Parameters.Add(param);                  // Confirmed Date

                param = command.CreateParameter();
                param.ParameterName = "@CnclReqDt";
                param.Value = record.CnclReqDt;
                command.Parameters.Add(param);                  // Cancel Reqeuested Date

                param = command.CreateParameter();
                param.ParameterName = "@CnclDt";
                param.Value = record.CnclDt;
                command.Parameters.Add(param);                  // Canceled Date

                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
    }
}

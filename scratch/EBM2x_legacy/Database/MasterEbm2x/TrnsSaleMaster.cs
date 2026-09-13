using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Modules;
    using EBM2x.UI;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of TrnsSaleMaster.
    /// </summary>
    public class TrnsSaleMaster : ModelIO
    {
        public long GetWaitCount()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT COUNT(*) FROM TRNS_SALE WHERE SALES_STTS_CD = '01'";
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
        public long GetSalesSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(INVC_NO), 0)  FROM TRNS_SALE";
                command.CommandType = CommandType.Text;

                long SalesSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    SalesSeq = long.Parse(firstColumn.ToString());
                }
                if (SalesSeq == 0)
                {
                    if (UIManager.Instance().PosModel.Environment.EnvPosSetup.LastSaleInvcNo > 0)
                    {
                        SalesSeq = UIManager.Instance().PosModel.Environment.EnvPosSetup.LastSaleInvcNo;
                    }
                }

                return (long)SalesSeq + 1;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 1;
            }
        }
        public string GetReceiptDate()
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return "";
            }

            StringBuilder sql = new StringBuilder();
            if (Session.SQLTimeCol.Contains("DATETIME"))
            {
                sql.Append(" SELECT SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),9,2)||SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),6,2)||SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),1,4)|| SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),12,2)||SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),15,2)|| SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),18,2) ");
            }
            else
            {
                sql.Append(" SELECT CONCAT(SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),9,2), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),6,2), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),1,4), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),12,2),SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),15,2), SUBSTR(CAST( ");
                sql.Append("        @SQLTimeCol AS CHAR),18,2)) ");
            }

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@SQLTimeCol";
                param.Value = Session.SQLTimeCol;
                command.Parameters.Add(param);

                string seq = (string)command.ExecuteScalar();

                return seq;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public List<TrnsSaleRecordVAT> getTrnsSaleTableVAT(string StartDate, string EndDate)
        {
            List<TrnsSaleRecordVAT> arrayList = new List<TrnsSaleRecordVAT>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT ts.CUST_TIN ");
            sql.Append("      , ts.CUST_NM ");
            sql.Append("      , tsr.INVC_NO ");
            sql.Append("      , tsr.RCPT_PBCT_DT ");
            sql.Append("      , (ts.TOT_AMT - ts.TOT_TAX_AMT) * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE - 1 END AS TOT_AMT ");
            sql.Append("      , ts.TAXBL_AMT_A * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE - 1 END AS TAXBL_AMT_A ");
            sql.Append("      , ts.TAXBL_AMT_C * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE - 1 END AS TAXBL_AMT_C ");
            sql.Append("      , (ts.TAXBL_AMT_B - ts.TOT_TAX_AMT) * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE - 1 END AS TAXBL_AMT_B ");
            sql.Append("      , ts.TOT_TAX_AMT * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE - 1 END AS TOT_TAX_AMT ");
            sql.Append(" FROM TRNS_SALE_RECEIPT tsr, TRNS_SALE ts ");
            sql.Append(" WHERE tsr.TIN = ts.TIN ");
            sql.Append("    AND tsr.BHF_ID = ts.BHF_ID ");
            sql.Append("    AND tsr.INVC_NO = ts.INVC_NO ");
            sql.Append("    AND ts.SALES_DT BETWEEN @StartDate AND @EndDate ");

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
                    TrnsSaleRecordVAT record = new TrnsSaleRecordVAT();

                    if (!reader.IsDBNull(0)) record.CustTin = reader.GetString(0);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.CustNm = reader.GetString(1);               // Customer Name
                    if (!reader.IsDBNull(2)) record.InvcNo = GetLong(reader, 2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.RcptPbctDt = reader.GetString(3);           // Receipt Type Code
                    if (!reader.IsDBNull(4)) record.TotAmt = reader.GetDouble(4);               // Total Amount
                    if (!reader.IsDBNull(5)) record.TaxblAmtA = reader.GetDouble(5);            // Taxable Amount A
                    if (!reader.IsDBNull(6)) record.TaxblAmtC = reader.GetDouble(6);            // Taxable Amount C
                    if (!reader.IsDBNull(7)) record.TaxblAmtB = reader.GetDouble(7);            // Taxable Amount B
                    //if (!reader.IsDBNull(7)) record.TaxblAmtE = reader.GetDouble(7);            // Taxable Amount E
                    if (!reader.IsDBNull(8)) record.TotTaxAmt = reader.GetDouble(8);            // Total Tax Amount

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

        public List<TrnsSaleRecord> getTrnsSaleTable(string StartDate, string EndDate, long InvoiceNo,string valueSalesStts)
        {
            List<TrnsSaleRecord> arrayList = new List<TrnsSaleRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleTable.GetSelectSQL());
            sql.Append(" WHERE A.SALES_DT BETWEEN @StartDate AND @EndDate ");
            if (InvoiceNo > 0)
            {
                sql.Append("   AND A.INVC_NO = @InvoiceNo ");
            }
            if (!string.IsNullOrEmpty(valueSalesStts)) sql.Append("   and SALES_STTS_CD = @SALES_STTS_CD");
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
                param.ParameterName = "@SALES_STTS_CD";
                param.Value = valueSalesStts;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSaleRecord record = new TrnsSaleRecord();

                    // JINIT_20191201, 
                    int sign = 1;
                    if (reader["RCPT_TY_CD"].ToString() == "R") sign = -1;

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.InvcNo = GetLong(reader, 2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.PrchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.OrgInvcNo = GetLong(reader, 4);            // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(6)) record.CustNo = reader.GetString(6);               // Customer No.
                    if (!reader.IsDBNull(6)) record.CustTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(7)) record.CustBhfId = reader.GetString(7);            // Customer Branch Office ID
                    if (!reader.IsDBNull(8)) record.CustNm = reader.GetString(8);               // Customer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(10)) record.DvcId = reader.GetString(10);              // Device ID
                    if (!reader.IsDBNull(9)) record.SalesTyCd = reader.GetString(9);          // Sale Type Code
                    if (!reader.IsDBNull(10)) record.RcptTyCd = reader.GetString(10);           // Receipt Type Code
                    if (!reader.IsDBNull(11)) record.PmtTyCd = reader.GetString(11);            // Payment Type Code
                    if (!reader.IsDBNull(12)) record.SalesSttsCd = reader.GetString(12);        // Sale Status Code
                    if (!reader.IsDBNull(13)) record.CfmDt = reader.GetString(13);              // Confirmed Date
                    if (!reader.IsDBNull(14)) record.SalesDt = reader.GetString(14);            // Sale Date
                    if (!reader.IsDBNull(15)) record.StockRlsDt = reader.GetString(15);         // Stock Released Date
                    if (!reader.IsDBNull(16)) record.CnclReqDt = reader.GetString(16);          // Cancel Reqeuested Date
                    if (!reader.IsDBNull(17)) record.CnclDt = reader.GetString(17);             // Canceled Date
                    if (!reader.IsDBNull(18)) record.RfdDt = reader.GetString(18);              // Refunded Date
                    if (!reader.IsDBNull(19)) record.TotItemCnt = reader.GetInt16(19) * sign;   // JINIT_20191201, Total Item Count
                    if (!reader.IsDBNull(20)) record.TaxblAmtA = Math.Round(reader.GetDouble(20) * sign, 2);   // JINIT_20191201, Taxable Amount A
                    if (!reader.IsDBNull(21)) record.TaxblAmtB = Math.Round(reader.GetDouble(21) * sign, 2);   // JINIT_20191201, Taxable Amount B
                    if (!reader.IsDBNull(22)) record.TaxblAmtC = Math.Round(reader.GetDouble(22) * sign, 2);   // JINIT_20191201, Taxable Amount C
                    if (!reader.IsDBNull(23)) record.TaxblAmtD = Math.Round(reader.GetDouble(23) * sign, 2);   // JINIT_20191201, Taxable Amount D
                   // if (!reader.IsDBNull(23)) record.TaxblAmtE = Math.Round(reader.GetDouble(23) * sign, 2);   // JINIT_20191201, Taxable Amount E
                    if (!reader.IsDBNull(24)) record.TaxRtA = reader.GetInt16(24);              // Tax Rate A
                    if (!reader.IsDBNull(25)) record.TaxRtB = reader.GetInt16(25);              // Tax Rate B
                    if (!reader.IsDBNull(26)) record.TaxRtC = reader.GetInt16(26);              // Tax Rate C
                    if (!reader.IsDBNull(27)) record.TaxRtD = reader.GetInt16(27);              // Tax Rate D
                    //if (!reader.IsDBNull(27)) record.TaxRtE = reader.GetInt16(27);              // Tax Rate E
                    //Round to 4 digits from 2 digits Changed By Aimee
                    if (!reader.IsDBNull(28)) record.TaxAmtA = Math.Round(reader.GetDouble(28) * sign, 4);     // JINIT_20191201, Tax Amount A
                    if (!reader.IsDBNull(29)) record.TaxAmtB = Math.Round(reader.GetDouble(29) * sign, 4);     // JINIT_20191201, Tax Amount B
                    if (!reader.IsDBNull(30)) record.TaxAmtC = Math.Round(reader.GetDouble(30) * sign, 4);     // JINIT_20191201, Tax Amount C
                    if (!reader.IsDBNull(31)) record.TaxAmtD = Math.Round(reader.GetDouble(31) * sign, 4);     // JINIT_20191201, Tax Amount D
                    //if (!reader.IsDBNull(31)) record.TaxAmtE = Math.Round(reader.GetDouble(31) * sign, 4);     // JINIT_20191201, Tax Amount E
                    if (!reader.IsDBNull(32)) record.TotTaxblAmt = Math.Round(reader.GetDouble(32) * sign, 4); // JINIT_20191201, Total Taxable Amount
                    if (!reader.IsDBNull(33)) record.TotTaxAmt = Math.Round(reader.GetDouble(33) * sign, 4);   // JINIT_20191201, Total Tax Amount
                    if (!reader.IsDBNull(34)) record.TotAmt = Math.Round(reader.GetDouble(34) * sign, 4);      // JINIT_20191201, Total Amount
                    //END
                    if (!reader.IsDBNull(35)) record.Remark = reader.GetString(35);             // Remark
                    if (!reader.IsDBNull(36)) record.RegrId = reader.GetString(36);             // Registrant ID
                    if (!reader.IsDBNull(37)) record.RegrNm = reader.GetString(37);             // Registrant Name
                    if (!reader.IsDBNull(38)) record.RegDt = reader.GetString(38);      // Registered Date
                    if (!reader.IsDBNull(39)) record.ModrId = reader.GetString(39);             // Modifier ID
                    if (!reader.IsDBNull(40)) record.ModrNm = reader.GetString(40);             // Modifier Name
                    if (!reader.IsDBNull(41)) record.ModDt = reader.GetString(41);      // Modified Date

                    if (!reader.IsDBNull(42)) record.RefundReason = reader.GetString(42);       // RefundReason
                    if (!reader.IsDBNull(43)) record.RefundReasonText = reader.GetString(43);   // RefundReasonText
                    
                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TrnsSaleRecord record in arrayList)
            {
                record.SalesSttsNm = codeDtlMaster.SalesSttsName(record.SalesSttsCd);
                record.TradeNm = taxpayerBhfCustMaster.GetTaxpayerBhfCustName(record.Tin, record.BhfId, record.CustTin);
            }

            return arrayList;
        }

        public bool ToRecord(TrnsSaleRecord record, string valTin, string valBhfId, long valInvcNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
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
                    if (!reader.IsDBNull(2)) record.InvcNo = reader.GetInt32(2);               // Invoice No.
                    if (!reader.IsDBNull(3)) record.PrchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.OrgInvcNo = reader.GetInt32(4);            // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.TaxprNm = reader.GetString(5);              // Taxpayer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(6)) record.CustNo = reader.GetString(6);               // Customer No.
                    if (!reader.IsDBNull(6)) record.CustTin = reader.GetString(6);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(7)) record.CustBhfId = reader.GetString(7);            // Customer Branch Office ID
                    if (!reader.IsDBNull(8)) record.CustNm = reader.GetString(8);               // Customer Name
                    //JCNA 202001 DELETE if (!reader.IsDBNull(10)) record.DvcId = reader.GetString(10);              // Device ID
                    if (!reader.IsDBNull(9)) record.SalesTyCd = reader.GetString(9);          // Sale Type Code
                    if (!reader.IsDBNull(10)) record.RcptTyCd = reader.GetString(10);           // Receipt Type Code
                    if (!reader.IsDBNull(11)) record.PmtTyCd = reader.GetString(11);            // Payment Type Code
                    if (!reader.IsDBNull(12)) record.SalesSttsCd = reader.GetString(12);        // Sale Status Code
                    if (!reader.IsDBNull(13)) record.CfmDt = reader.GetString(13);      // Confirmed Date
                    if (!reader.IsDBNull(14)) record.SalesDt = reader.GetString(14);    // Sale Date
                    if (!reader.IsDBNull(15)) record.StockRlsDt = reader.GetString(15); // Stock Released Date
                    if (!reader.IsDBNull(16)) record.CnclReqDt = reader.GetString(16);  // Cancel Reqeuested Date
                    if (!reader.IsDBNull(17)) record.CnclDt = reader.GetString(17);     // Canceled Date
                    if (!reader.IsDBNull(18)) record.RfdDt = reader.GetString(18);      // Refunded Date
                    if (!reader.IsDBNull(19)) record.TotItemCnt = reader.GetInt16(19);          // Total Item Count
                    if (!reader.IsDBNull(20)) record.TaxblAmtA = Math.Round(reader.GetDouble(20), 2);          // Taxable Amount A
                    if (!reader.IsDBNull(21)) record.TaxblAmtB = Math.Round(reader.GetDouble(21), 2);          // Taxable Amount B
                    if (!reader.IsDBNull(22)) record.TaxblAmtC = Math.Round(reader.GetDouble(22), 2);          // Taxable Amount C
                    if (!reader.IsDBNull(23)) record.TaxblAmtD = Math.Round(reader.GetDouble(23), 2);          // Taxable Amount D
                    //if (!reader.IsDBNull(23)) record.TaxblAmtE = Math.Round(reader.GetDouble(23), 2);          // Taxable Amount E
                    if (!reader.IsDBNull(24)) record.TaxRtA = reader.GetInt16(24);             // Tax Rate A
                    if (!reader.IsDBNull(25)) record.TaxRtB = reader.GetInt16(25);             // Tax Rate B
                    if (!reader.IsDBNull(26)) record.TaxRtC = reader.GetInt16(26);             // Tax Rate C
                    if (!reader.IsDBNull(27)) record.TaxRtD = reader.GetInt16(27);             // Tax Rate D
                    //if (!reader.IsDBNull(27)) record.TaxRtE = reader.GetInt16(27);             // Tax Rate E
                    if (!reader.IsDBNull(28)) record.TaxAmtA = Math.Round(reader.GetDouble(28), 2);            // Tax Amount A
                    if (!reader.IsDBNull(29)) record.TaxAmtB = Math.Round(reader.GetDouble(29), 2);            // Tax Amount B
                    if (!reader.IsDBNull(30)) record.TaxAmtC = Math.Round(reader.GetDouble(30), 2);            // Tax Amount C
                    if (!reader.IsDBNull(31)) record.TaxAmtD = Math.Round(reader.GetDouble(31), 2);            // Tax Amount D
                    //if (!reader.IsDBNull(31)) record.TaxAmtE = Math.Round(reader.GetDouble(31), 2);            // Tax Amount E
                    if (!reader.IsDBNull(32)) record.TotTaxblAmt = Math.Round(reader.GetDouble(32), 2);        // Total Taxable Amount
                    if (!reader.IsDBNull(33)) record.TotTaxAmt = Math.Round(reader.GetDouble(33), 2);          // Total Tax Amount
                    if (!reader.IsDBNull(34)) record.TotAmt = Math.Round(reader.GetDouble(34), 2);             // Total Amount
                    if (!reader.IsDBNull(35)) record.Remark = reader.GetString(35);             // Remark
                    if (!reader.IsDBNull(36)) record.RegrId = reader.GetString(36);             // Registrant ID
                    if (!reader.IsDBNull(37)) record.RegrNm = reader.GetString(37);             // Registrant Name
                    if (!reader.IsDBNull(38)) record.RegDt = reader.GetString(38);      // Registered Date
                    if (!reader.IsDBNull(39)) record.ModrId = reader.GetString(39);             // Modifier ID
                    if (!reader.IsDBNull(40)) record.ModrNm = reader.GetString(40);             // Modifier Name
                    if (!reader.IsDBNull(41)) record.ModDt = reader.GetString(41);      // Modified Date

                    if (!reader.IsDBNull(42)) record.RefundReason = reader.GetString(42);       // RefundReason
                    if (!reader.IsDBNull(43)) record.RefundReasonText = reader.GetString(43);   // RefundReasonText

                    reader.Close();

                    record.SalesSttsNm = codeDtlMaster.SalesSttsName(record.SalesSttsCd);
                    record.TradeNm = taxpayerBhfCustMaster.GetTaxpayerBhfCustName(record.Tin, record.BhfId, record.CustTin);

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

        public bool ToTable(TrnsSaleRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();

            try
            {
                command.Parameters.Clear();
                trnsSaleTable.SetParameters(command, record);
 
                command.CommandText = trnsSaleTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = trnsSaleTable.GetInsertSQL();
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
        public bool InsertTable(TrnsSaleRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();

            try
            {
                command.CommandText = trnsSaleTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsSaleTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool DeleteTable(TrnsSaleRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleTable trnsSaleTable = new TrnsSaleTable();

            try
            {
                command.CommandText = trnsSaleTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsSaleTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool UpdateTable(string salesSttsCd, string paymentCode, TrnsSaleRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE TRNS_SALE ");
            sql.Append("   SET SALES_STTS_CD = @SALES_STTS_CD, ");
            sql.Append("       REFUND_REASON = @REFUND_REASON, ");
            sql.Append("       REFUND_REASON_TEXT = @REFUND_REASON_TEXT, ");
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt,  ");         // Modified Date
            if (!string.IsNullOrEmpty(paymentCode))
            {
                sql.Append("   PMT_TY_CD = @PMT_TY_CD ");
            }
            switch (salesSttsCd)
            {
                case "02":
                    if (!string.IsNullOrEmpty(paymentCode)) sql.Append(", ");
                    sql.Append("  CFM_DT = @CfmDt");
                    break;
                case "03":
                    if (!string.IsNullOrEmpty(paymentCode)) sql.Append(", ");
                    sql.Append("  CNCL_REQ_DT = @CnclReqDt");
                    break;
                case "04":
                    if (!string.IsNullOrEmpty(paymentCode)) sql.Append(", ");
                    sql.Append("  CNCL_DT = @CnclDt");
                    break;
                case "05":
                    if (!string.IsNullOrEmpty(paymentCode)) sql.Append(", ");
                    sql.Append("  RFD_DT = @RfdDt");
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
                param.ParameterName = "@SALES_STTS_CD";
                param.Value = salesSttsCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@REFUND_REASON";
                param.Value = string.IsNullOrEmpty(record.RefundReason) ? "" : record.RefundReason;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@REFUND_REASON_TEXT";
                param.Value = string.IsNullOrEmpty(record.RefundReasonText) ? "" : record.RefundReasonText; 
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

                if (!string.IsNullOrEmpty(paymentCode))
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@PMT_TY_CD";
                    param.Value = paymentCode;
                    command.Parameters.Add(param);
                }

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

                param = command.CreateParameter();
                param.ParameterName = "@RfdDt";
                param.Value = record.RfdDt;
                command.Parameters.Add(param);                  // Refunded Date

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

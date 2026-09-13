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
    /// Description of TrnsSaleReceiptMaster.
    /// </summary>
    public class TrnsSaleReceiptMaster : ModelIO
    {
        public long GetReceiptSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(CUR_RCPT_NO), 0) FROM TRNS_SALE_RECEIPT";
                command.CommandType = CommandType.Text;

                long SalesSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    SalesSeq = long.Parse(firstColumn.ToString());
                }
                return (long)SalesSeq + 1;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 1;
            }
        }
        public long GetTotReceiptSeq()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(TOT_RCPT_NO), 0)  FROM TRNS_SALE_RECEIPT";
                command.CommandType = CommandType.Text;

                long SalesSeq = 0;
                var firstColumn = command.ExecuteScalar();
                if (firstColumn != null)
                {
                    SalesSeq = long.Parse(firstColumn.ToString());
                }
                return (long)SalesSeq + 1;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 1;
            }
        }

        //Added By Bright 4.4.2022 Start
        public string GetLastReceiptSeqTime()
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return DateTime.Now.ToString("yyyyMMddHHmmss");

            try
            {
                command.CommandText = "SELECT IFNULL(MAX(RCPT_PBCT_DT),'') FROM TRNS_SALE_RECEIPT";
                command.CommandType = CommandType.Text;
               
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        //Added By Bright 4.4.2022 End
        public List<TrnsSaleReceiptRecord> getTrnsSaleReceiptTable()
        {
            List<TrnsSaleReceiptRecord> arrayList = new List<TrnsSaleReceiptRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();

            try
            {
                command.CommandText = trnsSaleReceiptTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TrnsSaleReceiptRecord record = new TrnsSaleReceiptRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.InvcNo = reader.GetInt32(2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.PrchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.OrgInvcNo = reader.GetInt32(4);             // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.CurRcptNo = reader.GetInt32(5);             // Current Receipt No.
                    if (!reader.IsDBNull(6)) record.TotRcptNo = reader.GetInt32(6);             // Total Receipt No.
                    if (!reader.IsDBNull(7)) record.TaxprNm = reader.GetString(7);              // Taxpayer Name
                    if (!reader.IsDBNull(8)) record.RcptPbctDt = reader.GetString(8);           // Receipt Published Date
                    if (!reader.IsDBNull(9)) record.IntrlData = reader.GetString(9);            // Internal Data
                    if (!reader.IsDBNull(10)) record.RcptSign = reader.GetString(10);           // Receipt Signature
                    if (!reader.IsDBNull(11)) record.Jrnl = reader.GetString(11);               // Journal
                    if (!reader.IsDBNull(12)) record.TrdeNm = reader.GetString(12);             // Tradmark Name
                    if (!reader.IsDBNull(13)) record.Adrs = reader.GetString(13);               // Address
                    if (!reader.IsDBNull(14)) record.TopMsg = reader.GetString(14);             // Top Message
                    if (!reader.IsDBNull(15)) record.BtmMsg = reader.GetString(15);             // Bottom Message
                    if (!reader.IsDBNull(16)) record.RptNo = reader.GetInt32(16);               // Receipt No.
                    if (!reader.IsDBNull(17)) record.RptDt = reader.GetString(17);              // Receipt Date
                    //JCNA 202001 DELETE 
                    //if (!reader.IsDBNull(18)) record.TaskId = reader.GetInt32(18);              // Task ID
                    //if (!reader.IsDBNull(19)) record.TaskStrtDt = reader.GetString(19);         // Task Start Date
                    //if (!reader.IsDBNull(20)) record.TaskEndDt = reader.GetString(20);          // Task End Date
                    //if (!reader.IsDBNull(21)) record.TaskCmptYn = reader.GetString(21);         // Task Completed(Y/N)
                    //if (!reader.IsDBNull(22)) record.AudtFile = reader.GetString(22);           // Audit File
                    //if (!reader.IsDBNull(23)) record.AudtFileEcrt = reader.GetString(23);       // Audit File Encryption
                    //if (!reader.IsDBNull(24)) record.EbmSendDt = reader.GetString(24);          // EBM Send Date
                    //if (!reader.IsDBNull(25)) record.EbmRes = reader.GetString(25);             // EBM Response
                    //if (!reader.IsDBNull(26)) record.EbmResCd = reader.GetString(26);           // EBM Response Code
                    //if (!reader.IsDBNull(27)) record.ScmSignData = reader.GetString(27);        // SCM Signature Date
                    //if (!reader.IsDBNull(28)) record.ScmSign = reader.GetString(28);            // SCM Signature
                    //if (!reader.IsDBNull(29)) record.ScmSignCfm = reader.GetString(29);         // SCM Signature Confirmation
                    //if (!reader.IsDBNull(30)) record.SdcDt = reader.GetString(30);              // SDC Date
                    if (!reader.IsDBNull(18)) record.RegrId = reader.GetString(18);             // Registrant ID
                    if (!reader.IsDBNull(19)) record.RegrNm = reader.GetString(19);             // Registrant Name
                    if (!reader.IsDBNull(20)) record.RegDt = reader.GetString(20);              // Registered Date
                    if (!reader.IsDBNull(21)) record.ModrId = reader.GetString(21);             // Modifier ID
                    if (!reader.IsDBNull(22)) record.ModrNm = reader.GetString(22);             // Modifier Name
                    if (!reader.IsDBNull(23)) record.ModDt = reader.GetString(23);              // Modified Date

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

        public bool ToRecord(TrnsSaleReceiptRecord record, string valTin, string valBhfId, string valInvcNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(trnsSaleReceiptTable.GetSelectSQL());
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

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.InvcNo = reader.GetInt32(2);                // Invoice No.
                    if (!reader.IsDBNull(3)) record.PrchrAcptcYn = reader.GetString(3);         // Purchaser Accepted(Y/N)
                    if (!reader.IsDBNull(4)) record.OrgInvcNo = reader.GetInt32(4);             // Original Invoice No.
                    if (!reader.IsDBNull(5)) record.CurRcptNo = reader.GetInt32(5);             // Current Receipt No.
                    if (!reader.IsDBNull(6)) record.TotRcptNo = reader.GetInt32(6);             // Total Receipt No.
                    if (!reader.IsDBNull(7)) record.TaxprNm = reader.GetString(7);              // Taxpayer Name
                    if (!reader.IsDBNull(8)) record.RcptPbctDt = reader.GetString(8);           // Receipt Published Date
                    if (!reader.IsDBNull(9)) record.IntrlData = reader.GetString(9);            // Internal Data
                    if (!reader.IsDBNull(10)) record.RcptSign = reader.GetString(10);           // Receipt Signature
                    if (!reader.IsDBNull(11)) record.Jrnl = reader.GetString(11);               // Journal
                    if (!reader.IsDBNull(12)) record.TrdeNm = reader.GetString(12);             // Tradmark Name
                    if (!reader.IsDBNull(13)) record.Adrs = reader.GetString(13);               // Address
                    if (!reader.IsDBNull(14)) record.TopMsg = reader.GetString(14);             // Top Message
                    if (!reader.IsDBNull(15)) record.BtmMsg = reader.GetString(15);             // Bottom Message
                    if (!reader.IsDBNull(16)) record.RptNo = reader.GetInt32(16);               // Receipt No.
                    if (!reader.IsDBNull(17)) record.RptDt = reader.GetString(17);              // Receipt Date
                    //JCNA 202001 DELETE 
                    //if (!reader.IsDBNull(18)) record.TaskId = reader.GetInt32(18);              // Task ID
                    //if (!reader.IsDBNull(19)) record.TaskStrtDt = reader.GetString(19);         // Task Start Date
                    //if (!reader.IsDBNull(20)) record.TaskEndDt = reader.GetString(20);          // Task End Date
                    //if (!reader.IsDBNull(21)) record.TaskCmptYn = reader.GetString(21);         // Task Completed(Y/N)
                    //if (!reader.IsDBNull(22)) record.AudtFile = reader.GetString(22);           // Audit File
                    //if (!reader.IsDBNull(23)) record.AudtFileEcrt = reader.GetString(23);       // Audit File Encryption
                    //if (!reader.IsDBNull(24)) record.EbmSendDt = reader.GetString(24);          // EBM Send Date
                    //if (!reader.IsDBNull(25)) record.EbmRes = reader.GetString(25);             // EBM Response
                    //if (!reader.IsDBNull(26)) record.EbmResCd = reader.GetString(26);           // EBM Response Code
                    //if (!reader.IsDBNull(27)) record.ScmSignData = reader.GetString(27);        // SCM Signature Date
                    //if (!reader.IsDBNull(28)) record.ScmSign = reader.GetString(28);            // SCM Signature
                    //if (!reader.IsDBNull(29)) record.ScmSignCfm = reader.GetString(29);         // SCM Signature Confirmation
                    //if (!reader.IsDBNull(30)) record.SdcDt = reader.GetString(30);              // SDC Date
                    if (!reader.IsDBNull(18)) record.RegrId = reader.GetString(18);             // Registrant ID
                    if (!reader.IsDBNull(19)) record.RegrNm = reader.GetString(19);             // Registrant Name
                    if (!reader.IsDBNull(20)) record.RegDt = reader.GetString(20);              // Registered Date
                    if (!reader.IsDBNull(21)) record.ModrId = reader.GetString(21);             // Modifier ID
                    if (!reader.IsDBNull(22)) record.ModrNm = reader.GetString(22);             // Modifier Name
                    if (!reader.IsDBNull(23)) record.ModDt = reader.GetString(23);              // Modified Date

                    reader.Close();
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

        public bool ToTable(TrnsSaleReceiptRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();

            try
            {
                command.Parameters.Clear();
                trnsSaleReceiptTable.SetParameters(command, record);
 
                command.CommandText = trnsSaleReceiptTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = trnsSaleReceiptTable.GetInsertSQL();
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

        public bool InsertTable(TrnsSaleReceiptRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();

            try
            {
                command.CommandText = trnsSaleReceiptTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsSaleReceiptTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool DeleteTable(TrnsSaleReceiptRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TrnsSaleReceiptTable trnsSaleReceiptTable = new TrnsSaleReceiptTable();

            try
            {
                command.CommandText = trnsSaleReceiptTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                trnsSaleReceiptTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        private string sdc_run_nb(long run_nb, int nb_char)
        {
            string r;
            int i;
            r = run_nb.ToString();

            if (r.Length == nb_char | nb_char == 0) return r;
            else if (r.Length < nb_char)
            {
                var loopTo = nb_char - r.Length;
                for (i = 1; i <= loopTo; i++)
                    r = " " + r;
                return r;
            }
            else
                return "ERROR";
        }

        private string sdc_amount(string amt, int nb_char)
        {
            int i;
            amt = amt.Replace(".", ",");
            if (amt.Length == nb_char)
                return amt;
            else if (amt.Length < nb_char)
            {
                var loopTo = nb_char - amt.Length;
                for (i = 1; i <= loopTo; i++)
                    amt = " " + amt;
                return amt;
            }
            else
                return "ERROR";
        }



        public string GetReceiptSignature(ReceiptSignature receiptSignature)
        {
            string data = receiptSignature.RcptDt;  //sdc_change_dt(strRcptDt)
            data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblSpcpcA), 15);           // sdc_amount(Format(dblSpcpcA, "#0.#0"), 15)
            data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblVatA), 15) + " 0,00";   // sdc_amount(Format(dblVatA, "#0.#0"), 15) & " 0,00"
            data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblSpcpcB), 15);           // sdc_amount(Format(dblSpcpcB, "#0.#0"), 15) 
            data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblVatB), 15) + " 0,00";   // sdc_amount(Format(dblVatB, "#0.#0"), 15) & " 0,00" 

            //data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblSpcpcB), 15);           // sdc_amount(Format(dblSpcpcB, "#0.#0"), 15) 
            //data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblVatB), 15) + " 0,00";   // sdc_amount(Format(dblVatB, "#0.#0"), 15) & " 0,00" 
            //data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblSpcpcA), 15);           // sdc_amount(Format(dblSpcpcA, "#0.#0"), 15)
            //data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblVatA), 15) + " 0,00";   // sdc_amount(Format(dblVatA, "#0.#0"), 15) & " 0,00"

            data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblSpcpcC), 15);           //  sdc_amount(Format(dblSpcpcC, "#0.#0"), 15) 
            data += sdc_amount(string.Format("{0:#0.#0}", receiptSignature.DblVatC), 15) + " 0,00";   // sdc_amount(Format(dblVatC, "#0.#0"), 15) & " 0,00"
            data += "           0,00           0,00"; // sdc_amount("0,00", 15) & sdc_amount("0,00", 15)
            data += receiptSignature.TranRcptTyCD + receiptSignature.GblSdcSysNum + receiptSignature.RcptDt; //"NSSDC00700124220191205222136" tranTyCD & rcptTyCD & GblSdcSysNum & sdc_change_dt(strRcptDt) 
            data += sdc_run_nb(receiptSignature.CurReceiptID, 10); //sdc_run_nb(Val(getReceiptID("B", tranTyCD, rcptTyCD)), 10) 
            data += sdc_run_nb(receiptSignature.TotReceiptID, 10); //sdc_run_nb(getReceiptID("A", "", ""), 10)
            data += receiptSignature.Tin;
            data += receiptSignature.CustTin;
            data += receiptSignature.GblMrcSysCod;                     // data = data & Rowary(0) & sdc_client_tin(strTinCd) & GblMrcSysCod
            data += sdc_run_nb(receiptSignature.InvID, 10) + "16,00";  // data = data & sdc_run_nb(Val(strInvID.Trim()), 10) & "18,00"

            return data;
        }
        public string GetInternalData(long normalSales, long returnSales, string strOcdt, long receipt)
        {
            int days = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount();

            string data = (string)Base32.longToHex(normalSales, 5);
            data += (string)Base32.longToHex(receipt, 4);
            data += (string)Base32.longToHex(returnSales, 5);
            data += (string)Base32.longToHex(days, 2);
            
            return data;
        }
    }

    public class ReceiptSignature
    {
        public string RcptDt { get; set; }
        public string Tin { get; set; }
        public string CustTin { get; set; }
        public string GblMrcSysCod { get; set; }
        public long InvID { get; set; }
        public double DblSpcpcA { get; set; }
        public double DblVatA { get; set; }
        public double DblSpcpcB { get; set; }
        public double DblSpcpcE { get; set; }
        public double DblVatB { get; set; }
        public double DblVatE { get; set; }
        public double DblSpcpcC { get; set; }
        public double DblVatC { get; set; }

        public string GblSdcSysNum { get; set; }
        public string TranRcptTyCD { get; set; }
        public long CurReceiptID { get; set; }
        public long TotReceiptID { get; set; }

        public ReceiptSignature()
        {
        }
    }
}

/*
Receipt_signature(data, GblKeySign)

20191205222136999000031100602245WIS01001222        2818,00        2000,00         305,08 0,00           0,00           0,00 0,00           0,00           0,00 0,00           0,00           0,00NSSDC00700124220191205222136        20        20

// strRcptDt
20191205222136
              123456789123456789WIS12345678
              자기TIN  고객TIN
              TIN      strTinCd GblMrcSysCod      
20191205222136999000031100602245WIS01001222
                                           123456789012345
                                           strInvID  18,00
20191205222136999000031100602245WIS01001222        2818,00
                                                          12345678901234512345678901234512345123456789012345123456789012345
                                                          dblSpcpcB      dblVatB        _0,00dblSpcpcA      dblVatA
20191205222136999000031100602245WIS01001222        2818,00        2000,00         305,08 0,00           0,00           0,00                                                                      tranTyCD
                                                                                                                           12345                                                                  rcptTyCD                   12345678901234567890
                                                                                                                           _0,00dblSpcpcC      dblVatC        _0,00___________0,00___________0,00  GblSdcSysNumstrRcptDt     영수증번호영수증번호
20191205222136999000031100602245WIS01001222        2818,00        2000,00         305,08 0,00           0,00           0,00 0,00           0,00           0,00 0,00           0,00           0,00NSSDC00700124220191205222136        20        20


// 집계자료
Internal_data(int_data, GblKeyInternal)
0000002029000000130E000B00000014

longToHex(Long.Parse(GF_FindTotTax("N", "S", c_tot_S)), 5)  정상 과세
0000002029 000000130E000B00000014

longToHex(Long.Parse(GF_FindTotTax("N", "R", c_tot_R)), 5)  반품 과세
0000002029 000000130E 000B00000014

longToHex(Long.Parse(GF_FindNbDailyRpt(strOcdt1)), 2) YYYYMMDD
0000002029 000000130E 000B 00000014

longToHex(Long.Parse(sdc_run_nb(getReceiptID("A", "", ""), 0)), 4)  영수증번호
0000002029 000000130E 000B 00000014
 */

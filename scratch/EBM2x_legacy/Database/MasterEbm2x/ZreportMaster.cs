using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.MasterEbm2x;
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.UI;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ZreportMaster.
    /// </summary>
    public class ZreportMaster : ModelIO
    {
        public bool UpdateZreportTable()
        {
            //// 2021.1.13 OFF
            //return true;

            bool ret = false;

            for (int i = 2; i <= 14; i++)
            {
                DateTime dateTimeOld = DateTime.Now.AddDays(-1 * i);
                ret = UpdateZreportTable(dateTimeOld);

                if (ret == false) break;
            }

            DateTime dateTime = DateTime.Now.AddDays(-1);
            return UpdateZreportTable(dateTime);
        }
        public bool UpdateZreportTable(DateTime dateTime) 
        {
            string Tin = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblTaxIdNo;
            string BhfId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblBrcCod;
            string GblSdcSysNum = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblSdcSysNum;

            string rptDe = dateTime.ToString("yyyyMMdd"); 

            ZreportRecord record = new ZreportRecord();
            record.Tin = Tin;
            record.BhfId = BhfId;
            record.SdcId = GblSdcSysNum;                           // SDC_ID
            record.Reportdate = rptDe;

            //영업일 순번
            int days = UIManager.Instance().PosModel.Environment.EnvPosSetup.GetDayCount(dateTime);
            record.Reportnumber = days;                     // REPORTNUMBER

            // 2021.01.08
            if (record.Reportnumber < 0) return true;

            // 있는지 확인
            bool countFlag = RecordsCount(Tin, BhfId, rptDe);
            if (!countFlag)
            {
                double amount = 0;
                long count = 0;
                // 없으면 DATA 생성
                SalesReportMaster salesReportMaster = new SalesReportMaster();

                string fromStrText = dateTime.ToString("yyyyMMdd");
                string toStrText = dateTime.ToString("yyyyMMdd");

                count = salesReportMaster.GetCodeCount(fromStrText, toStrText, "N", "");
                record.Dailynbofreceipts = count;                // DAILYNBOFRECEIPTS 
                count = salesReportMaster.GetCodeCountMIN(fromStrText, toStrText, "N", "");
                record.Openingrunnumber = count;                 // OPENINGRUNNUMBER 
                count = salesReportMaster.GetCodeCountMAX(fromStrText, toStrText, "N", "");
                record.Closingrunnumber = count;                 // CLOSINGRUNNUMBER 

                count = salesReportMaster.GetCodeCount(fromStrText, toStrText, "N", "S");
                record.Normaltotalreceipts = count;              // NORMALTOTALRECEIPTS ; 
                count = salesReportMaster.GetCodeCountMIN(fromStrText, toStrText, "N", "S");
                record.Normalopeningrunnumber = count;           // NORMALOPENINGRUNNUMBER 
                count = salesReportMaster.GetCodeCountMAX(fromStrText, toStrText, "N", "S");
                record.Normalclosingrunnumber = count;          // NORMALCLOSINGRUNNUMBER 

                amount = 0;
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "S", "TRNS_SALE.TOT_AMT");
                record.Normaltotalsaleamount = Math.Round(amount, 2);         // NORMALTOTALSALEAMOUNT 

                amount = 0;
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "R", "TRNS_SALE.TOT_AMT");
                record.Normaltotalreturnamount = Math.Round(amount, 2); // NORMALTOTALRETURNAMOUNT 

                amount = 0;
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "S", "TRNS_SALE.TAX_AMT_A");
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "S", "TRNS_SALE.TAX_AMT_B");
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "S", "TRNS_SALE.TAX_AMT_C");
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "S", "TRNS_SALE.TAX_AMT_D");
                record.Normaltotaltaxsaleamount = Math.Round(amount, 2);       // NORMALTOTALTAXSALEAMOUNT 

                amount = 0;
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "R", "TRNS_SALE.TAX_AMT_A"); // TOTAL TAXE A-EX
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "R", "TRNS_SALE.TAX_AMT_B"); // TOTAL TAXE B
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "R", "TRNS_SALE.TAX_AMT_C"); // TOTAL TAXE C
                amount += salesReportMaster.GetCodeValue(fromStrText, toStrText, "N", "R", "TRNS_SALE.TAX_AMT_D"); // TOTAL TAXE D
                record.Normaltotaltaxreturnamount = Math.Round(amount, 2);     // NORMALTOTALTAXRETURNAMOUNT 

                //record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");        // REG_DT
                //record.UserId = "system";                                      // USERNAME
                //record.Username = "system";                                    // USERNAME

                ToTableInsert(record);
                ReportZRraSdcUpload reportZRraSdcUpload = new ReportZRraSdcUpload();
                reportZRraSdcUpload.SendReportZ(Tin, BhfId, rptDe);

                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ZreportRecord> getZreportTable()
        {
            List<ZreportRecord> arrayList = new List<ZreportRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ZreportTable zreportTable = new ZreportTable();

            try
            {
                command.CommandText = zreportTable.GetSelectSQL();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ZreportRecord record = new ZreportRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                             // TIN
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                           // BHF_ID
                    if (!reader.IsDBNull(2)) record.SdcId = reader.GetString(2);                           // SDC_ID
                    if (!reader.IsDBNull(3)) record.Reportdate = reader.GetString(3);                      // REPORTDATE
                    if (!reader.IsDBNull(4)) record.Reportnumber = reader.GetInt32(4);                     // REPORTNUMBER
                    if (!reader.IsDBNull(5)) record.Dailynbofreceipts = reader.GetInt32(5);                // DAILYNBOFRECEIPTS
                    if (!reader.IsDBNull(6)) record.Openingrunnumber = reader.GetInt32(6);                 // OPENINGRUNNUMBER
                    if (!reader.IsDBNull(7)) record.Closingrunnumber = reader.GetInt32(7);                 // CLOSINGRUNNUMBER
                    if (!reader.IsDBNull(8)) record.Normaltotalreceipts = reader.GetInt32(8);              // NORMALTOTALRECEIPTS
                    if (!reader.IsDBNull(9)) record.Normalopeningrunnumber = reader.GetInt32(9);           // NORMALOPENINGRUNNUMBER
                    if (!reader.IsDBNull(10)) record.Normalclosingrunnumber = reader.GetInt32(10);         // NORMALCLOSINGRUNNUMBER
                    if (!reader.IsDBNull(11)) record.Normaltotalsaleamount = reader.GetDouble(11);         // NORMALTOTALSALEAMOUNT
                    if (!reader.IsDBNull(12)) record.Normaltotalreturnamount = reader.GetDouble(12);       // NORMALTOTALRETURNAMOUNT
                    if (!reader.IsDBNull(13)) record.Normaltotaltaxsaleamount = reader.GetDouble(13);      // NORMALTOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(14)) record.Normaltotaltaxreturnamount = reader.GetDouble(14);    // NORMALTOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(15)) record.Copytotalreceipts = reader.GetInt32(15);              // COPYTOTALRECEIPTS
                    if (!reader.IsDBNull(16)) record.Copyopeningrunnumber = reader.GetInt32(16);           // COPYOPENINGRUNNUMBER
                    if (!reader.IsDBNull(17)) record.Copyclosingrunnumber = reader.GetInt32(17);           // COPYCLOSINGRUNNUMBER
                    if (!reader.IsDBNull(18)) record.Copytotalsaleamount = reader.GetDouble(18);           // COPYTOTALSALEAMOUNT
                    if (!reader.IsDBNull(19)) record.Copytotalreturnamount = reader.GetDouble(19);         // COPYTOTALRETURNAMOUNT
                    if (!reader.IsDBNull(20)) record.Copytotaltaxsaleamount = reader.GetDouble(20);        // COPYTOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(21)) record.Copytotaltaxreturnamount = reader.GetDouble(21);      // COPYTOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(22)) record.Trainingtotalreceipts = reader.GetInt32(22);          // TRAININGTOTALRECEIPTS
                    if (!reader.IsDBNull(23)) record.Trainingopeningrunnumber = reader.GetInt32(23);       // TRAININGOPENINGRUNNUMBER
                    if (!reader.IsDBNull(24)) record.Trainingclosingrunnumber = reader.GetInt32(24);       // TRAININGCLOSINGRUNNUMBER
                    if (!reader.IsDBNull(25)) record.Trainingtotalsaleamount = reader.GetDouble(25);       // TRAININGTOTALSALEAMOUNT
                    if (!reader.IsDBNull(26)) record.Trainingtotalreturnamount = reader.GetDouble(26);     // TRAININGTOTALRETURNAMOUNT
                    if (!reader.IsDBNull(27)) record.Trainingtotaltaxsaleamount = reader.GetDouble(27);    // TRAININGTOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(28)) record.Trainingtotaltaxreturnamount = reader.GetDouble(28);  // TRAININGTOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(29)) record.Proformatotalreceipts = reader.GetInt32(29);          // PROFORMATOTALRECEIPTS
                    if (!reader.IsDBNull(30)) record.Proformaopeningrunnumber = reader.GetInt32(30);       // PROFORMAOPENINGRUNNUMBER
                    if (!reader.IsDBNull(31)) record.Proformaclosingrunnumber = reader.GetInt32(31);       // PROFORMACLOSINGRUNNUMBER
                    if (!reader.IsDBNull(32)) record.Proformatotalsaleamount = reader.GetDouble(32);       // PROFORMATOTALSALEAMOUNT
                    if (!reader.IsDBNull(33)) record.Proformatotalreturnamount = reader.GetDouble(33);     // PROFORMATOTALRETURNAMOUNT
                    if (!reader.IsDBNull(34)) record.Proformatotaltaxsaleamount = reader.GetDouble(34);    // PROFORMATOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(35)) record.Proformatotaltaxreturnamount = reader.GetDouble(35);  // PROFORMATOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(36)) record.Totnbreceipts = reader.GetInt32(36);                  // TOTNBRECEIPTS
                    if (!reader.IsDBNull(37)) record.Totnbreceiptsnormal = reader.GetInt32(37);            // TOTNBRECEIPTSNORMAL
                    if (!reader.IsDBNull(38)) record.Totnbreceiptscopy = reader.GetInt32(38);              // TOTNBRECEIPTSCOPY
                    if (!reader.IsDBNull(39)) record.Totnbreceiptstraining = reader.GetInt32(39);          // TOTNBRECEIPTSTRAINING
                    if (!reader.IsDBNull(40)) record.Totnbreceiptsproforma = reader.GetInt32(40);          // TOTNBRECEIPTSPROFORMA
                    if (!reader.IsDBNull(41)) record.Totalsaleamount = reader.GetDouble(41);               // TOTALSALEAMOUNT
                    if (!reader.IsDBNull(42)) record.Totalsalestaxamount = reader.GetDouble(42);           // TOTALSALESTAXAMOUNT
                    if (!reader.IsDBNull(43)) record.Totalreturnamount = reader.GetDouble(43);             // TOTALRETURNAMOUNT
                    if (!reader.IsDBNull(44)) record.Totalreturntaxamount = reader.GetDouble(44);          // TOTALRETURNTAXAMOUNT
                    if (!reader.IsDBNull(45)) record.RegDt = reader.GetString(45);                 // REG_DT
                    if (!reader.IsDBNull(46)) record.UserId = reader.GetString(46);                      // USERNAME
                    if (!reader.IsDBNull(47)) record.Username = reader.GetString(47);                      // USERNAME

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

        public bool RecordsCount(string valTin, string valBhfId, string valReportdate)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("select * from ZREPORT");
            sql.Append(" where TIN = @Tin ");         // TIN
            sql.Append("   and BHF_ID = @BhfId ");    // BHF_ID
            sql.Append("   and REPORTDATE = @Reportdate ");  // REPORTDATE

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@Tin";
                param.Value = valTin;
                command.Parameters.Add(param);                  // TIN

                param = command.CreateParameter();
                param.ParameterName = "@BhfId";
                param.Value = valBhfId;
                command.Parameters.Add(param);                  // BHF_ID

                param = command.CreateParameter();
                param.ParameterName = "@Reportdate";
                param.Value = valReportdate;
                command.Parameters.Add(param);                  // REPORTDATE

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
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
                return true;
            }
        }

        public bool ToRecord(ZreportRecord record, string valTin, string valBhfId, string valSdcId, string valReportdate)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ZreportTable zreportTable = new ZreportTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(zreportTable.GetSelectSQL());
            sql.Append(" where TIN = @Tin ");         // TIN
            sql.Append("   and BHF_ID = @BhfId ");    // BHF_ID
            sql.Append("   and REPORTDATE = @Reportdate ");  // REPORTDATE

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@Tin";
                param.Value = valTin;
                command.Parameters.Add(param);                  // TIN

                param = command.CreateParameter();
                param.ParameterName = "@BhfId";
                param.Value = valBhfId;
                command.Parameters.Add(param);                  // BHF_ID

                param = command.CreateParameter();
                param.ParameterName = "@Reportdate";
                param.Value = valReportdate;
                command.Parameters.Add(param);                  // REPORTDATE

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                             // TIN
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                           // BHF_ID
                    if (!reader.IsDBNull(2)) record.SdcId = reader.GetString(2);                           // SDC_ID
                    if (!reader.IsDBNull(3)) record.Reportdate = reader.GetString(3);                      // REPORTDATE
                    if (!reader.IsDBNull(4)) record.Reportnumber = reader.GetInt32(4);                     // REPORTNUMBER
                    if (!reader.IsDBNull(5)) record.Dailynbofreceipts = reader.GetInt32(5);                // DAILYNBOFRECEIPTS
                    if (!reader.IsDBNull(6)) record.Openingrunnumber = reader.GetInt32(6);                 // OPENINGRUNNUMBER
                    if (!reader.IsDBNull(7)) record.Closingrunnumber = reader.GetInt32(7);                 // CLOSINGRUNNUMBER
                    if (!reader.IsDBNull(8)) record.Normaltotalreceipts = reader.GetInt32(8);              // NORMALTOTALRECEIPTS
                    if (!reader.IsDBNull(9)) record.Normalopeningrunnumber = reader.GetInt32(9);           // NORMALOPENINGRUNNUMBER
                    if (!reader.IsDBNull(10)) record.Normalclosingrunnumber = reader.GetInt32(10);         // NORMALCLOSINGRUNNUMBER
                    if (!reader.IsDBNull(11)) record.Normaltotalsaleamount = reader.GetDouble(11);         // NORMALTOTALSALEAMOUNT
                    if (!reader.IsDBNull(12)) record.Normaltotalreturnamount = reader.GetDouble(12);       // NORMALTOTALRETURNAMOUNT
                    if (!reader.IsDBNull(13)) record.Normaltotaltaxsaleamount = reader.GetDouble(13);      // NORMALTOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(14)) record.Normaltotaltaxreturnamount = reader.GetDouble(14);    // NORMALTOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(15)) record.Copytotalreceipts = reader.GetInt32(15);              // COPYTOTALRECEIPTS
                    if (!reader.IsDBNull(16)) record.Copyopeningrunnumber = reader.GetInt32(16);           // COPYOPENINGRUNNUMBER
                    if (!reader.IsDBNull(17)) record.Copyclosingrunnumber = reader.GetInt32(17);           // COPYCLOSINGRUNNUMBER
                    if (!reader.IsDBNull(18)) record.Copytotalsaleamount = reader.GetDouble(18);           // COPYTOTALSALEAMOUNT
                    if (!reader.IsDBNull(19)) record.Copytotalreturnamount = reader.GetDouble(19);         // COPYTOTALRETURNAMOUNT
                    if (!reader.IsDBNull(20)) record.Copytotaltaxsaleamount = reader.GetDouble(20);        // COPYTOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(21)) record.Copytotaltaxreturnamount = reader.GetDouble(21);      // COPYTOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(22)) record.Trainingtotalreceipts = reader.GetInt32(22);          // TRAININGTOTALRECEIPTS
                    if (!reader.IsDBNull(23)) record.Trainingopeningrunnumber = reader.GetInt32(23);       // TRAININGOPENINGRUNNUMBER
                    if (!reader.IsDBNull(24)) record.Trainingclosingrunnumber = reader.GetInt32(24);       // TRAININGCLOSINGRUNNUMBER
                    if (!reader.IsDBNull(25)) record.Trainingtotalsaleamount = reader.GetDouble(25);       // TRAININGTOTALSALEAMOUNT
                    if (!reader.IsDBNull(26)) record.Trainingtotalreturnamount = reader.GetDouble(26);     // TRAININGTOTALRETURNAMOUNT
                    if (!reader.IsDBNull(27)) record.Trainingtotaltaxsaleamount = reader.GetDouble(27);    // TRAININGTOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(28)) record.Trainingtotaltaxreturnamount = reader.GetDouble(28);  // TRAININGTOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(29)) record.Proformatotalreceipts = reader.GetInt32(29);          // PROFORMATOTALRECEIPTS
                    if (!reader.IsDBNull(30)) record.Proformaopeningrunnumber = reader.GetInt32(30);       // PROFORMAOPENINGRUNNUMBER
                    if (!reader.IsDBNull(31)) record.Proformaclosingrunnumber = reader.GetInt32(31);       // PROFORMACLOSINGRUNNUMBER
                    if (!reader.IsDBNull(32)) record.Proformatotalsaleamount = reader.GetDouble(32);       // PROFORMATOTALSALEAMOUNT
                    if (!reader.IsDBNull(33)) record.Proformatotalreturnamount = reader.GetDouble(33);     // PROFORMATOTALRETURNAMOUNT
                    if (!reader.IsDBNull(34)) record.Proformatotaltaxsaleamount = reader.GetDouble(34);    // PROFORMATOTALTAXSALEAMOUNT
                    if (!reader.IsDBNull(35)) record.Proformatotaltaxreturnamount = reader.GetDouble(35);  // PROFORMATOTALTAXRETURNAMOUNT
                    if (!reader.IsDBNull(36)) record.Totnbreceipts = reader.GetInt32(36);                  // TOTNBRECEIPTS
                    if (!reader.IsDBNull(37)) record.Totnbreceiptsnormal = reader.GetInt32(37);            // TOTNBRECEIPTSNORMAL
                    if (!reader.IsDBNull(38)) record.Totnbreceiptscopy = reader.GetInt32(38);              // TOTNBRECEIPTSCOPY
                    if (!reader.IsDBNull(39)) record.Totnbreceiptstraining = reader.GetInt32(39);          // TOTNBRECEIPTSTRAINING
                    if (!reader.IsDBNull(40)) record.Totnbreceiptsproforma = reader.GetInt32(40);          // TOTNBRECEIPTSPROFORMA
                    if (!reader.IsDBNull(41)) record.Totalsaleamount = reader.GetDouble(41);               // TOTALSALEAMOUNT
                    if (!reader.IsDBNull(42)) record.Totalsalestaxamount = reader.GetDouble(42);           // TOTALSALESTAXAMOUNT
                    if (!reader.IsDBNull(43)) record.Totalreturnamount = reader.GetDouble(43);             // TOTALRETURNAMOUNT
                    if (!reader.IsDBNull(44)) record.Totalreturntaxamount = reader.GetDouble(44);          // TOTALRETURNTAXAMOUNT
                    if (!reader.IsDBNull(45)) record.RegDt = reader.GetString(45);                 // REG_DT
                    if (!reader.IsDBNull(46)) record.UserId = reader.GetString(46);                      // USERNAME
                    if (!reader.IsDBNull(47)) record.Username = reader.GetString(47);                      // USERNAME

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

        public bool ToTableInsert(ZreportRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ZreportTable zreportTable = new ZreportTable();

            try
            {
                command.Parameters.Clear();
                zreportTable.SetParameters(command, record);
 
                command.CommandText = zreportTable.GetInsertSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool ToTable(ZreportRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ZreportTable zreportTable = new ZreportTable();

            try
            {
                command.Parameters.Clear();
                zreportTable.SetParameters(command, record);

                command.CommandText = zreportTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = zreportTable.GetInsertSQL();
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

        public bool DeleteTable(ZreportRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            ZreportTable zreportTable = new ZreportTable();

            try
            {
                command.CommandText = zreportTable.GetDeleteSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                zreportTable.SetParameters(command, record);
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

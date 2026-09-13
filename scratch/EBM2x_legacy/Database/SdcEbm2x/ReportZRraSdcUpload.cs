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
    public class ReportZRraSdcUpload : ModelIO
    {
        public List<ReportZReq> getReportZTable(string valTin, string valBhfId, string StartDate, string EndDate)
        {
            List<ReportZReq> arrayList = new List<ReportZReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ZreportTable zreportTable = new ZreportTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(zreportTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND REPORTDATE BETWEEN @StartDate AND @EndDate ");

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
                    ReportZReq record = new ReportZReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);             // TIN
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);           // BHF_ID
                    if (!reader.IsDBNull(2)) record.sdcId = reader.GetString(2);         // SDC_ID
                    if (!reader.IsDBNull(3)) record.rptDe = reader.GetString(3);           // REPORTDATE
                    if (!reader.IsDBNull(4)) record.rptNo = reader.GetInt32(4).ToString(); // REPORTNUMBER

                    if (!reader.IsDBNull(5)) record.rcptPbctCnt = reader.GetInt32(5).ToString(); // DAILYNBOFRECEIPTS// 영수증 발행 수
                    if (!reader.IsDBNull(6)) record.rcptOpnNo = reader.GetInt32(6);              // OPENINGRUNNUMBER// 영수증 개시 번호
                    if (!reader.IsDBNull(7)) record.rcptClsNo = reader.GetInt32(7);              // CLOSINGRUNNUMBER// 영수증 마감 번호
                    if (!reader.IsDBNull(8)) record.nrmRcptPbctCnt = reader.GetInt32(8);         // NORMALTOTALRECEIPTS// 일반 영수증 발행 수
                    if (!reader.IsDBNull(9)) record.nrmRcptOpnNo = reader.GetInt32(9);           // NORMALOPENINGRUNNUMBER// 일반 영수증 개시 번호
                    if (!reader.IsDBNull(10)) record.nrmRcptClsNo = reader.GetInt32(10);         // NORMALCLOSINGRUNNUMBER// 일반 영수증 마감 번호
                    if (!reader.IsDBNull(11)) record.nrmSalesAmt = reader.GetDouble(11);         // NORMALTOTALSALEAMOUNT// 일반 판매 금액
                    if (!reader.IsDBNull(12)) record.nrmRfdAmt = reader.GetDouble(12);           // NORMALTOTALRETURNAMOUNT// 일반 환불 금액
                    if (!reader.IsDBNull(13)) record.nrmSalesTaxAmt = reader.GetDouble(13);      // NORMALTOTALTAXSALEAMOUNT// 일반 판매 과세 금액
                    if (!reader.IsDBNull(14)) record.nrmRfdTaxAmt = reader.GetDouble(14);        // NORMALTOTALTAXRETURNAMOUNT// 일반 환불 과세 금액
                    
                    //if (!reader.IsDBNull(15)) record.Copytotalreceipts = 0;                 // COPYTOTALRECEIPTS
                    //if (!reader.IsDBNull(16)) record.Copyopeningrunnumber = 0;              // COPYOPENINGRUNNUMBER
                    //if (!reader.IsDBNull(17)) record.Copyclosingrunnumber = 0;              // COPYCLOSINGRUNNUMBER
                    //if (!reader.IsDBNull(18)) record.Copytotalsaleamount = 0;               // COPYTOTALSALEAMOUNT
                    //if (!reader.IsDBNull(19)) record.Copytotalreturnamount = 0;             // COPYTOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(20)) record.Copytotaltaxsaleamount = 0;            // COPYTOTALTAXSALEAMOUNT
                    //if (!reader.IsDBNull(21)) record.Copytotaltaxreturnamount = 0;          // COPYTOTALTAXRETURNAMOUNT
                    //if (!reader.IsDBNull(22)) record.Trainingtotalreceipts = 0;             // TRAININGTOTALRECEIPTS
                    //if (!reader.IsDBNull(23)) record.Trainingopeningrunnumber = 0;          // TRAININGOPENINGRUNNUMBER
                    //if (!reader.IsDBNull(24)) record.Trainingclosingrunnumber = 0;          // TRAININGCLOSINGRUNNUMBER
                    //if (!reader.IsDBNull(25)) record.Trainingtotalsaleamount = 0;           // TRAININGTOTALSALEAMOUNT
                    //if (!reader.IsDBNull(26)) record.Trainingtotalreturnamount = 0;         // TRAININGTOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(27)) record.Trainingtotaltaxsaleamount = 0;        // TRAININGTOTALTAXSALEAMOUNT
                    //if (!reader.IsDBNull(28)) record.Trainingtotaltaxreturnamount = 0;      // TRAININGTOTALTAXRETURNAMOUNT
                    //if (!reader.IsDBNull(29)) record.Proformatotalreceipts = 0;             // PROFORMATOTALRECEIPTS
                    //if (!reader.IsDBNull(30)) record.Proformaopeningrunnumber = 0;          // PROFORMAOPENINGRUNNUMBER
                    //if (!reader.IsDBNull(31)) record.Proformaclosingrunnumber = 0;          // PROFORMACLOSINGRUNNUMBER
                    //if (!reader.IsDBNull(32)) record.Proformatotalsaleamount = 0;           // PROFORMATOTALSALEAMOUNT
                    //if (!reader.IsDBNull(33)) record.Proformatotalreturnamount = 0;         // PROFORMATOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(34)) record.Proformatotaltaxsaleamount = 0;        // PROFORMATOTALTAXSALEAMOUNT
                    //if (!reader.IsDBNull(35)) record.Proformatotaltaxreturnamount = 0;      // PROFORMATOTALTAXRETURNAMOUNT
                    
                    //if (!reader.IsDBNull(36)) record.Totnbreceipts = 0;                     // TOTNBRECEIPTS
                    //if (!reader.IsDBNull(37)) record.Totnbreceiptsnormal = 0;               // TOTNBRECEIPTSNORMAL
                    //if (!reader.IsDBNull(38)) record.Totnbreceiptscopy = 0;                 // TOTNBRECEIPTSCOPY
                    //if (!reader.IsDBNull(39)) record.Totnbreceiptstraining = 0;             // TOTNBRECEIPTSTRAINING
                    //if (!reader.IsDBNull(40)) record.Totnbreceiptsproforma = 0;             // TOTNBRECEIPTSPROFORMA
                    //if (!reader.IsDBNull(41)) record.Totalsaleamount = 0;                   // TOTALSALEAMOUNT
                    //if (!reader.IsDBNull(42)) record.Totalsalestaxamount = 0;               // TOTALSALESTAXAMOUNT
                    //if (!reader.IsDBNull(43)) record.Totalreturnamount = 0;                 // TOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(44)) record.Totalreturntaxamount = 0;              // TOTALRETURNTAXAMOUNT                    
                    //if (!reader.IsDBNull(45)) record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // REG_DT
                    
                    if (!reader.IsDBNull(46)) record.regrId = "system";                 // USERID
                    if (!reader.IsDBNull(47)) record.regrNm = "system";               // USERNAME

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
        public void SendReportZ(string valTin, string valBhfId, string rptDe)
        {
            List<ReportZReq> sendList = getReportZTable( valTin, valBhfId, rptDe);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "ReportZ";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_REPORT_Z_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<ReportZReq> getReportZTable(string valTin, string valBhfId, string rptDe)
        {
            List<ReportZReq> arrayList = new List<ReportZReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            ZreportTable zreportTable = new ZreportTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(zreportTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND REPORTDATE = @REPORTDATE ");

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
                param.ParameterName = "@REPORTDATE";
                param.Value = rptDe;
                command.Parameters.Add(param);

                TrnsSaleMaster trnsSaleMaster = new TrnsSaleMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    ReportZReq record = new ReportZReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);             // TIN
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);           // BHF_ID
                    if (!reader.IsDBNull(2)) record.sdcId = reader.GetString(2);           // SDC_ID
                    if (!reader.IsDBNull(3)) record.rptDe = reader.GetString(3);           // REPORTDATE
                    if (!reader.IsDBNull(4)) record.rptNo = reader.GetInt32(4).ToString(); // REPORTNUMBER

                    if (!reader.IsDBNull(5)) record.rcptPbctCnt = reader.GetInt32(5).ToString();                 // DAILYNBOFRECEIPTS// 영수증 발행 수
                    if (!reader.IsDBNull(6)) record.rcptOpnNo = reader.GetInt32(6);                  // OPENINGRUNNUMBER// 영수증 개시 번호
                    if (!reader.IsDBNull(7)) record.rcptClsNo = reader.GetInt32(7);                  // CLOSINGRUNNUMBER// 영수증 마감 번호
                    if (!reader.IsDBNull(8)) record.nrmRcptPbctCnt = reader.GetInt32(8);               // NORMALTOTALRECEIPTS// 일반 영수증 발행 수
                    if (!reader.IsDBNull(9)) record.nrmRcptOpnNo = reader.GetInt32(9);            // NORMALOPENINGRUNNUMBER// 일반 영수증 개시 번호
                    if (!reader.IsDBNull(10)) record.nrmRcptClsNo = reader.GetInt32(10);            // NORMALCLOSINGRUNNUMBER// 일반 영수증 마감 번호
                    if (!reader.IsDBNull(11)) record.nrmSalesAmt = reader.GetDouble(11);             // NORMALTOTALSALEAMOUNT// 일반 판매 금액
                    if (!reader.IsDBNull(12)) record.nrmRfdAmt = reader.GetDouble(12);           // NORMALTOTALRETURNAMOUNT// 일반 환불 금액
                    if (!reader.IsDBNull(13)) record.nrmSalesTaxAmt = reader.GetDouble(13);          // NORMALTOTALTAXSALEAMOUNT// 일반 판매 과세 금액
                    if (!reader.IsDBNull(14)) record.nrmRfdTaxAmt = reader.GetDouble(14);        // NORMALTOTALTAXRETURNAMOUNT// 일반 환불 과세 금액

                    //if (!reader.IsDBNull(15)) record.Copytotalreceipts = 0;                 // COPYTOTALRECEIPTS
                    //if (!reader.IsDBNull(16)) record.Copyopeningrunnumber = 0;              // COPYOPENINGRUNNUMBER
                    //if (!reader.IsDBNull(17)) record.Copyclosingrunnumber = 0;              // COPYCLOSINGRUNNUMBER
                    //if (!reader.IsDBNull(18)) record.Copytotalsaleamount = 0;               // COPYTOTALSALEAMOUNT
                    //if (!reader.IsDBNull(19)) record.Copytotalreturnamount = 0;             // COPYTOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(20)) record.Copytotaltaxsaleamount = 0;            // COPYTOTALTAXSALEAMOUNT
                    //if (!reader.IsDBNull(21)) record.Copytotaltaxreturnamount = 0;          // COPYTOTALTAXRETURNAMOUNT
                    //if (!reader.IsDBNull(22)) record.Trainingtotalreceipts = 0;             // TRAININGTOTALRECEIPTS
                    //if (!reader.IsDBNull(23)) record.Trainingopeningrunnumber = 0;          // TRAININGOPENINGRUNNUMBER
                    //if (!reader.IsDBNull(24)) record.Trainingclosingrunnumber = 0;          // TRAININGCLOSINGRUNNUMBER
                    //if (!reader.IsDBNull(25)) record.Trainingtotalsaleamount = 0;           // TRAININGTOTALSALEAMOUNT
                    //if (!reader.IsDBNull(26)) record.Trainingtotalreturnamount = 0;         // TRAININGTOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(27)) record.Trainingtotaltaxsaleamount = 0;        // TRAININGTOTALTAXSALEAMOUNT
                    //if (!reader.IsDBNull(28)) record.Trainingtotaltaxreturnamount = 0;      // TRAININGTOTALTAXRETURNAMOUNT
                    //if (!reader.IsDBNull(29)) record.Proformatotalreceipts = 0;             // PROFORMATOTALRECEIPTS
                    //if (!reader.IsDBNull(30)) record.Proformaopeningrunnumber = 0;          // PROFORMAOPENINGRUNNUMBER
                    //if (!reader.IsDBNull(31)) record.Proformaclosingrunnumber = 0;          // PROFORMACLOSINGRUNNUMBER
                    //if (!reader.IsDBNull(32)) record.Proformatotalsaleamount = 0;           // PROFORMATOTALSALEAMOUNT
                    //if (!reader.IsDBNull(33)) record.Proformatotalreturnamount = 0;         // PROFORMATOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(34)) record.Proformatotaltaxsaleamount = 0;        // PROFORMATOTALTAXSALEAMOUNT
                    //if (!reader.IsDBNull(35)) record.Proformatotaltaxreturnamount = 0;      // PROFORMATOTALTAXRETURNAMOUNT

                    //if (!reader.IsDBNull(36)) record.Totnbreceipts = 0;                     // TOTNBRECEIPTS
                    //if (!reader.IsDBNull(37)) record.Totnbreceiptsnormal = 0;               // TOTNBRECEIPTSNORMAL
                    //if (!reader.IsDBNull(38)) record.Totnbreceiptscopy = 0;                 // TOTNBRECEIPTSCOPY
                    //if (!reader.IsDBNull(39)) record.Totnbreceiptstraining = 0;             // TOTNBRECEIPTSTRAINING
                    //if (!reader.IsDBNull(40)) record.Totnbreceiptsproforma = 0;             // TOTNBRECEIPTSPROFORMA
                    //if (!reader.IsDBNull(41)) record.Totalsaleamount = 0;                   // TOTALSALEAMOUNT
                    //if (!reader.IsDBNull(42)) record.Totalsalestaxamount = 0;               // TOTALSALESTAXAMOUNT
                    //if (!reader.IsDBNull(43)) record.Totalreturnamount = 0;                 // TOTALRETURNAMOUNT
                    //if (!reader.IsDBNull(44)) record.Totalreturntaxamount = 0;              // TOTALRETURNTAXAMOUNT                    
                    //if (!reader.IsDBNull(45)) record.RegDt = DateTime.Now.ToString("yyyyMMddHHmmss");                  // REG_DT

                    if (!reader.IsDBNull(46)) record.regrId = "system";                 // USERID
                    if (!reader.IsDBNull(47)) record.regrNm = "system";               // USERNAME

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

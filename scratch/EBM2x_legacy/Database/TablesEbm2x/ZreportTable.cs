using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of ZreportTable.
    /// </summary>
    public class ZreportTable
    {
        public ZreportTable()
        {
        }

        public string GetTableCheckSQL(bool isMysql)
        {
            StringBuilder sql = new StringBuilder();

            if (isMysql)
            {
                sql.Append("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'ZREPORT'");
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM sqlite_master WHERE name = 'ZREPORT'");
            }

            return sql.ToString();
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists ZREPORT ( ");                                   // ZREPORT
            sql.Append("       TIN                              CHAR(9)        not null , ");      // TIN
            sql.Append("       BHF_ID                           CHAR(2)        not null , ");      // BHF_ID
            sql.Append("       SDC_ID                           CHAR(20)       not null , ");      // SDC_ID
            sql.Append("       REPORTDATE                       CHAR(8)        not null , ");      // REPORTDATE
            sql.Append("       REPORTNUMBER                     DECIMAL(11)    null , ");          // REPORTNUMBER
            sql.Append("       DAILYNBOFRECEIPTS                DECIMAL(11)    null , ");          // DAILYNBOFRECEIPTS
            sql.Append("       OPENINGRUNNUMBER                 DECIMAL(11)    null , ");          // OPENINGRUNNUMBER
            sql.Append("       CLOSINGRUNNUMBER                 DECIMAL(11)    null , ");          // CLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALRECEIPTS              DECIMAL(11)    null , ");          // NORMALTOTALRECEIPTS
            sql.Append("       NORMALOPENINGRUNNUMBER           DECIMAL(11)    null , ");          // NORMALOPENINGRUNNUMBER
            sql.Append("       NORMALCLOSINGRUNNUMBER           DECIMAL(11)    null , ");          // NORMALCLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALSALEAMOUNT            DECIMAL(18, 2) null , ");          // NORMALTOTALSALEAMOUNT
            sql.Append("       NORMALTOTALRETURNAMOUNT          DECIMAL(18, 2) null , ");          // NORMALTOTALRETURNAMOUNT
            sql.Append("       NORMALTOTALTAXSALEAMOUNT         DECIMAL(18, 2) null , ");          // NORMALTOTALTAXSALEAMOUNT
            sql.Append("       NORMALTOTALTAXRETURNAMOUNT       DECIMAL(18, 2) null , ");          // NORMALTOTALTAXRETURNAMOUNT
            sql.Append("       COPYTOTALRECEIPTS                DECIMAL(11)    null , ");          // COPYTOTALRECEIPTS
            sql.Append("       COPYOPENINGRUNNUMBER             DECIMAL(11)    null , ");          // COPYOPENINGRUNNUMBER
            sql.Append("       COPYCLOSINGRUNNUMBER             DECIMAL(11)    null , ");          // COPYCLOSINGRUNNUMBER
            sql.Append("       COPYTOTALSALEAMOUNT              DECIMAL(18, 2) null , ");          // COPYTOTALSALEAMOUNT
            sql.Append("       COPYTOTALRETURNAMOUNT            DECIMAL(18, 2) null , ");          // COPYTOTALRETURNAMOUNT
            sql.Append("       COPYTOTALTAXSALEAMOUNT           DECIMAL(18, 2) null , ");          // COPYTOTALTAXSALEAMOUNT
            sql.Append("       COPYTOTALTAXRETURNAMOUNT         DECIMAL(18, 2) null , ");          // COPYTOTALTAXRETURNAMOUNT
            sql.Append("       TRAININGTOTALRECEIPTS            DECIMAL(11)    null , ");          // TRAININGTOTALRECEIPTS
            sql.Append("       TRAININGOPENINGRUNNUMBER         DECIMAL(11)    null , ");          // TRAININGOPENINGRUNNUMBER
            sql.Append("       TRAININGCLOSINGRUNNUMBER         DECIMAL(11)    null , ");          // TRAININGCLOSINGRUNNUMBER
            sql.Append("       TRAININGTOTALSALEAMOUNT          DECIMAL(18, 2) null , ");          // TRAININGTOTALSALEAMOUNT
            sql.Append("       TRAININGTOTALRETURNAMOUNT        DECIMAL(18, 2) null , ");          // TRAININGTOTALRETURNAMOUNT
            sql.Append("       TRAININGTOTALTAXSALEAMOUNT       DECIMAL(18, 2) null , ");          // TRAININGTOTALTAXSALEAMOUNT
            sql.Append("       TRAININGTOTALTAXRETURNAMOUNT     DECIMAL(18, 2) null , ");          // TRAININGTOTALTAXRETURNAMOUNT
            sql.Append("       PROFORMATOTALRECEIPTS            DECIMAL(11)    null , ");          // PROFORMATOTALRECEIPTS
            sql.Append("       PROFORMAOPENINGRUNNUMBER         DECIMAL(11)    null , ");          // PROFORMAOPENINGRUNNUMBER
            sql.Append("       PROFORMACLOSINGRUNNUMBER         DECIMAL(11)    null , ");          // PROFORMACLOSINGRUNNUMBER
            sql.Append("       PROFORMATOTALSALEAMOUNT          DECIMAL(18, 2) null , ");          // PROFORMATOTALSALEAMOUNT
            sql.Append("       PROFORMATOTALRETURNAMOUNT        DECIMAL(18, 2) null , ");          // PROFORMATOTALRETURNAMOUNT
            sql.Append("       PROFORMATOTALTAXSALEAMOUNT       DECIMAL(18, 2) null , ");          // PROFORMATOTALTAXSALEAMOUNT
            sql.Append("       PROFORMATOTALTAXRETURNAMOUNT     DECIMAL(18, 2) null , ");          // PROFORMATOTALTAXRETURNAMOUNT
            sql.Append("       TOTNBRECEIPTS                    DECIMAL(11)    null , ");          // TOTNBRECEIPTS
            sql.Append("       TOTNBRECEIPTSNORMAL              DECIMAL(11)    null , ");          // TOTNBRECEIPTSNORMAL
            sql.Append("       TOTNBRECEIPTSCOPY                DECIMAL(11)    null , ");          // TOTNBRECEIPTSCOPY
            sql.Append("       TOTNBRECEIPTSTRAINING            DECIMAL(11)    null , ");          // TOTNBRECEIPTSTRAINING
            sql.Append("       TOTNBRECEIPTSPROFORMA            DECIMAL(11)    null , ");          // TOTNBRECEIPTSPROFORMA
            sql.Append("       TOTALSALEAMOUNT                  DECIMAL(18, 2) null , ");          // TOTALSALEAMOUNT
            sql.Append("       TOTALSALESTAXAMOUNT              DECIMAL(18, 2) null , ");          // TOTALSALESTAXAMOUNT
            sql.Append("       TOTALRETURNAMOUNT                DECIMAL(18, 2) null , ");          // TOTALRETURNAMOUNT
            sql.Append("       TOTALRETURNTAXAMOUNT             DECIMAL(18, 2) null , ");          // TOTALRETURNTAXAMOUNT
            sql.Append("       REG_DT                           VARCHAR(14)    null , ");          // REG_DT
            sql.Append("       USERID                           VARCHAR(20)    null , ");          // USERID
            sql.Append("       USERNAME                         VARCHAR(100)   null , ");          // USERNAME
            sql.Append("       primary key ( TIN, BHF_ID, REPORTDATE ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // TIN
            sql.Append("       BHF_ID, ");                  // BHF_ID
            sql.Append("       SDC_ID, ");                  // SDC_ID
            sql.Append("       REPORTDATE, ");              // REPORTDATE
            sql.Append("       REPORTNUMBER, ");            // REPORTNUMBER
            sql.Append("       DAILYNBOFRECEIPTS, ");       // DAILYNBOFRECEIPTS
            sql.Append("       OPENINGRUNNUMBER, ");        // OPENINGRUNNUMBER
            sql.Append("       CLOSINGRUNNUMBER, ");        // CLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALRECEIPTS, ");     // NORMALTOTALRECEIPTS
            sql.Append("       NORMALOPENINGRUNNUMBER, ");  // NORMALOPENINGRUNNUMBER
            sql.Append("       NORMALCLOSINGRUNNUMBER, ");  // NORMALCLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALSALEAMOUNT, ");   // NORMALTOTALSALEAMOUNT
            sql.Append("       NORMALTOTALRETURNAMOUNT, ");  // NORMALTOTALRETURNAMOUNT
            sql.Append("       NORMALTOTALTAXSALEAMOUNT, ");  // NORMALTOTALTAXSALEAMOUNT
            sql.Append("       NORMALTOTALTAXRETURNAMOUNT, ");  // NORMALTOTALTAXRETURNAMOUNT
            sql.Append("       COPYTOTALRECEIPTS, ");       // COPYTOTALRECEIPTS
            sql.Append("       COPYOPENINGRUNNUMBER, ");    // COPYOPENINGRUNNUMBER
            sql.Append("       COPYCLOSINGRUNNUMBER, ");    // COPYCLOSINGRUNNUMBER
            sql.Append("       COPYTOTALSALEAMOUNT, ");     // COPYTOTALSALEAMOUNT
            sql.Append("       COPYTOTALRETURNAMOUNT, ");   // COPYTOTALRETURNAMOUNT
            sql.Append("       COPYTOTALTAXSALEAMOUNT, ");  // COPYTOTALTAXSALEAMOUNT
            sql.Append("       COPYTOTALTAXRETURNAMOUNT, ");  // COPYTOTALTAXRETURNAMOUNT
            sql.Append("       TRAININGTOTALRECEIPTS, ");   // TRAININGTOTALRECEIPTS
            sql.Append("       TRAININGOPENINGRUNNUMBER, ");  // TRAININGOPENINGRUNNUMBER
            sql.Append("       TRAININGCLOSINGRUNNUMBER, ");  // TRAININGCLOSINGRUNNUMBER
            sql.Append("       TRAININGTOTALSALEAMOUNT, ");  // TRAININGTOTALSALEAMOUNT
            sql.Append("       TRAININGTOTALRETURNAMOUNT, ");  // TRAININGTOTALRETURNAMOUNT
            sql.Append("       TRAININGTOTALTAXSALEAMOUNT, ");  // TRAININGTOTALTAXSALEAMOUNT
            sql.Append("       TRAININGTOTALTAXRETURNAMOUNT, ");  // TRAININGTOTALTAXRETURNAMOUNT
            sql.Append("       PROFORMATOTALRECEIPTS, ");   // PROFORMATOTALRECEIPTS
            sql.Append("       PROFORMAOPENINGRUNNUMBER, ");  // PROFORMAOPENINGRUNNUMBER
            sql.Append("       PROFORMACLOSINGRUNNUMBER, ");  // PROFORMACLOSINGRUNNUMBER
            sql.Append("       PROFORMATOTALSALEAMOUNT, ");  // PROFORMATOTALSALEAMOUNT
            sql.Append("       PROFORMATOTALRETURNAMOUNT, ");  // PROFORMATOTALRETURNAMOUNT
            sql.Append("       PROFORMATOTALTAXSALEAMOUNT, ");  // PROFORMATOTALTAXSALEAMOUNT
            sql.Append("       PROFORMATOTALTAXRETURNAMOUNT, ");  // PROFORMATOTALTAXRETURNAMOUNT
            sql.Append("       TOTNBRECEIPTS, ");           // TOTNBRECEIPTS
            sql.Append("       TOTNBRECEIPTSNORMAL, ");     // TOTNBRECEIPTSNORMAL
            sql.Append("       TOTNBRECEIPTSCOPY, ");       // TOTNBRECEIPTSCOPY
            sql.Append("       TOTNBRECEIPTSTRAINING, ");   // TOTNBRECEIPTSTRAINING
            sql.Append("       TOTNBRECEIPTSPROFORMA, ");   // TOTNBRECEIPTSPROFORMA
            sql.Append("       TOTALSALEAMOUNT, ");         // TOTALSALEAMOUNT
            sql.Append("       TOTALSALESTAXAMOUNT, ");     // TOTALSALESTAXAMOUNT
            sql.Append("       TOTALRETURNAMOUNT, ");       // TOTALRETURNAMOUNT
            sql.Append("       TOTALRETURNTAXAMOUNT, ");    // TOTALRETURNTAXAMOUNT
            sql.Append("       REG_DT, ");                  // REG_DT
            sql.Append("       USERID, ");                // USERNAME
            sql.Append("       USERNAME ");                // USERNAME
            sql.Append("  from ZREPORT "); // ZREPORT
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into ZREPORT ( "); // ZREPORT
            sql.Append("       TIN, ");                     // TIN
            sql.Append("       BHF_ID, ");                  // BHF_ID
            sql.Append("       SDC_ID, ");                  // SDC_ID
            sql.Append("       REPORTDATE, ");              // REPORTDATE
            sql.Append("       REPORTNUMBER, ");            // REPORTNUMBER
            sql.Append("       DAILYNBOFRECEIPTS, ");       // DAILYNBOFRECEIPTS
            sql.Append("       OPENINGRUNNUMBER, ");        // OPENINGRUNNUMBER
            sql.Append("       CLOSINGRUNNUMBER, ");        // CLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALRECEIPTS, ");     // NORMALTOTALRECEIPTS
            sql.Append("       NORMALOPENINGRUNNUMBER, ");  // NORMALOPENINGRUNNUMBER
            sql.Append("       NORMALCLOSINGRUNNUMBER, ");  // NORMALCLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALSALEAMOUNT, ");   // NORMALTOTALSALEAMOUNT
            sql.Append("       NORMALTOTALRETURNAMOUNT, ");  // NORMALTOTALRETURNAMOUNT
            sql.Append("       NORMALTOTALTAXSALEAMOUNT, ");  // NORMALTOTALTAXSALEAMOUNT
            sql.Append("       NORMALTOTALTAXRETURNAMOUNT, ");  // NORMALTOTALTAXRETURNAMOUNT
            sql.Append("       COPYTOTALRECEIPTS, ");       // COPYTOTALRECEIPTS
            sql.Append("       COPYOPENINGRUNNUMBER, ");    // COPYOPENINGRUNNUMBER
            sql.Append("       COPYCLOSINGRUNNUMBER, ");    // COPYCLOSINGRUNNUMBER
            sql.Append("       COPYTOTALSALEAMOUNT, ");     // COPYTOTALSALEAMOUNT
            sql.Append("       COPYTOTALRETURNAMOUNT, ");   // COPYTOTALRETURNAMOUNT
            sql.Append("       COPYTOTALTAXSALEAMOUNT, ");  // COPYTOTALTAXSALEAMOUNT
            sql.Append("       COPYTOTALTAXRETURNAMOUNT, ");  // COPYTOTALTAXRETURNAMOUNT
            sql.Append("       TRAININGTOTALRECEIPTS, ");   // TRAININGTOTALRECEIPTS
            sql.Append("       TRAININGOPENINGRUNNUMBER, ");  // TRAININGOPENINGRUNNUMBER
            sql.Append("       TRAININGCLOSINGRUNNUMBER, ");  // TRAININGCLOSINGRUNNUMBER
            sql.Append("       TRAININGTOTALSALEAMOUNT, ");  // TRAININGTOTALSALEAMOUNT
            sql.Append("       TRAININGTOTALRETURNAMOUNT, ");  // TRAININGTOTALRETURNAMOUNT
            sql.Append("       TRAININGTOTALTAXSALEAMOUNT, ");  // TRAININGTOTALTAXSALEAMOUNT
            sql.Append("       TRAININGTOTALTAXRETURNAMOUNT, ");  // TRAININGTOTALTAXRETURNAMOUNT
            sql.Append("       PROFORMATOTALRECEIPTS, ");   // PROFORMATOTALRECEIPTS
            sql.Append("       PROFORMAOPENINGRUNNUMBER, ");  // PROFORMAOPENINGRUNNUMBER
            sql.Append("       PROFORMACLOSINGRUNNUMBER, ");  // PROFORMACLOSINGRUNNUMBER
            sql.Append("       PROFORMATOTALSALEAMOUNT, ");  // PROFORMATOTALSALEAMOUNT
            sql.Append("       PROFORMATOTALRETURNAMOUNT, ");  // PROFORMATOTALRETURNAMOUNT
            sql.Append("       PROFORMATOTALTAXSALEAMOUNT, ");  // PROFORMATOTALTAXSALEAMOUNT
            sql.Append("       PROFORMATOTALTAXRETURNAMOUNT, ");  // PROFORMATOTALTAXRETURNAMOUNT
            sql.Append("       TOTNBRECEIPTS, ");           // TOTNBRECEIPTS
            sql.Append("       TOTNBRECEIPTSNORMAL, ");     // TOTNBRECEIPTSNORMAL
            sql.Append("       TOTNBRECEIPTSCOPY, ");       // TOTNBRECEIPTSCOPY
            sql.Append("       TOTNBRECEIPTSTRAINING, ");   // TOTNBRECEIPTSTRAINING
            sql.Append("       TOTNBRECEIPTSPROFORMA, ");   // TOTNBRECEIPTSPROFORMA
            sql.Append("       TOTALSALEAMOUNT, ");         // TOTALSALEAMOUNT
            sql.Append("       TOTALSALESTAXAMOUNT, ");     // TOTALSALESTAXAMOUNT
            sql.Append("       TOTALRETURNAMOUNT, ");       // TOTALRETURNAMOUNT
            sql.Append("       TOTALRETURNTAXAMOUNT, ");    // TOTALRETURNTAXAMOUNT
            sql.Append("       REG_DT, ");                  // REG_DT
            sql.Append("       USERID, ");                // USERNAME
            sql.Append("       USERNAME ");                // USERNAME
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // TIN
            sql.Append("       @BhfId, ");                  // BHF_ID
            sql.Append("       @SdcId, ");                  // SDC_ID
            sql.Append("       @Reportdate, ");             // REPORTDATE
            sql.Append("       @Reportnumber, ");           // REPORTNUMBER
            sql.Append("       @Dailynbofreceipts, ");      // DAILYNBOFRECEIPTS
            sql.Append("       @Openingrunnumber, ");       // OPENINGRUNNUMBER
            sql.Append("       @Closingrunnumber, ");       // CLOSINGRUNNUMBER
            sql.Append("       @Normaltotalreceipts, ");    // NORMALTOTALRECEIPTS
            sql.Append("       @Normalopeningrunnumber, ");  // NORMALOPENINGRUNNUMBER
            sql.Append("       @Normalclosingrunnumber, ");  // NORMALCLOSINGRUNNUMBER
            sql.Append("       @Normaltotalsaleamount, ");  // NORMALTOTALSALEAMOUNT
            sql.Append("       @Normaltotalreturnamount, ");  // NORMALTOTALRETURNAMOUNT
            sql.Append("       @Normaltotaltaxsaleamount, ");  // NORMALTOTALTAXSALEAMOUNT
            sql.Append("       @Normaltotaltaxreturnamount, ");  // NORMALTOTALTAXRETURNAMOUNT
            sql.Append("       @Copytotalreceipts, ");      // COPYTOTALRECEIPTS
            sql.Append("       @Copyopeningrunnumber, ");   // COPYOPENINGRUNNUMBER
            sql.Append("       @Copyclosingrunnumber, ");   // COPYCLOSINGRUNNUMBER
            sql.Append("       @Copytotalsaleamount, ");    // COPYTOTALSALEAMOUNT
            sql.Append("       @Copytotalreturnamount, ");  // COPYTOTALRETURNAMOUNT
            sql.Append("       @Copytotaltaxsaleamount, ");  // COPYTOTALTAXSALEAMOUNT
            sql.Append("       @Copytotaltaxreturnamount, ");  // COPYTOTALTAXRETURNAMOUNT
            sql.Append("       @Trainingtotalreceipts, ");  // TRAININGTOTALRECEIPTS
            sql.Append("       @Trainingopeningrunnumber, ");  // TRAININGOPENINGRUNNUMBER
            sql.Append("       @Trainingclosingrunnumber, ");  // TRAININGCLOSINGRUNNUMBER
            sql.Append("       @Trainingtotalsaleamount, ");  // TRAININGTOTALSALEAMOUNT
            sql.Append("       @Trainingtotalreturnamount, ");  // TRAININGTOTALRETURNAMOUNT
            sql.Append("       @Trainingtotaltaxsaleamount, ");  // TRAININGTOTALTAXSALEAMOUNT
            sql.Append("       @Trainingtotaltaxreturnamount, ");  // TRAININGTOTALTAXRETURNAMOUNT
            sql.Append("       @Proformatotalreceipts, ");  // PROFORMATOTALRECEIPTS
            sql.Append("       @Proformaopeningrunnumber, ");  // PROFORMAOPENINGRUNNUMBER
            sql.Append("       @Proformaclosingrunnumber, ");  // PROFORMACLOSINGRUNNUMBER
            sql.Append("       @Proformatotalsaleamount, ");  // PROFORMATOTALSALEAMOUNT
            sql.Append("       @Proformatotalreturnamount, ");  // PROFORMATOTALRETURNAMOUNT
            sql.Append("       @Proformatotaltaxsaleamount, ");  // PROFORMATOTALTAXSALEAMOUNT
            sql.Append("       @Proformatotaltaxreturnamount, ");  // PROFORMATOTALTAXRETURNAMOUNT
            sql.Append("       @Totnbreceipts, ");          // TOTNBRECEIPTS
            sql.Append("       @Totnbreceiptsnormal, ");    // TOTNBRECEIPTSNORMAL
            sql.Append("       @Totnbreceiptscopy, ");      // TOTNBRECEIPTSCOPY
            sql.Append("       @Totnbreceiptstraining, ");  // TOTNBRECEIPTSTRAINING
            sql.Append("       @Totnbreceiptsproforma, ");  // TOTNBRECEIPTSPROFORMA
            sql.Append("       @Totalsaleamount, ");        // TOTALSALEAMOUNT
            sql.Append("       @Totalsalestaxamount, ");    // TOTALSALESTAXAMOUNT
            sql.Append("       @Totalreturnamount, ");      // TOTALRETURNAMOUNT
            sql.Append("       @Totalreturntaxamount, ");   // TOTALRETURNTAXAMOUNT
            sql.Append("       @RegDt, ");                  // REG_DT
            sql.Append("       @UserId, ");               // USERNAME
            sql.Append("       @Username ");               // USERNAME
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update ZREPORT "); // ZREPORT
            sql.Append("   set REPORTNUMBER = @Reportnumber, ");  // REPORTNUMBER
            sql.Append("       DAILYNBOFRECEIPTS = @Dailynbofreceipts, ");  // DAILYNBOFRECEIPTS
            sql.Append("       OPENINGRUNNUMBER = @Openingrunnumber, ");  // OPENINGRUNNUMBER
            sql.Append("       CLOSINGRUNNUMBER = @Closingrunnumber, ");  // CLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALRECEIPTS = @Normaltotalreceipts, ");  // NORMALTOTALRECEIPTS
            sql.Append("       NORMALOPENINGRUNNUMBER = @Normalopeningrunnumber, ");  // NORMALOPENINGRUNNUMBER
            sql.Append("       NORMALCLOSINGRUNNUMBER = @Normalclosingrunnumber, ");  // NORMALCLOSINGRUNNUMBER
            sql.Append("       NORMALTOTALSALEAMOUNT = @Normaltotalsaleamount, ");  // NORMALTOTALSALEAMOUNT
            sql.Append("       NORMALTOTALRETURNAMOUNT = @Normaltotalreturnamount, ");  // NORMALTOTALRETURNAMOUNT
            sql.Append("       NORMALTOTALTAXSALEAMOUNT = @Normaltotaltaxsaleamount, ");  // NORMALTOTALTAXSALEAMOUNT
            sql.Append("       NORMALTOTALTAXRETURNAMOUNT = @Normaltotaltaxreturnamount, ");  // NORMALTOTALTAXRETURNAMOUNT
            sql.Append("       COPYTOTALRECEIPTS = @Copytotalreceipts, ");  // COPYTOTALRECEIPTS
            sql.Append("       COPYOPENINGRUNNUMBER = @Copyopeningrunnumber, ");  // COPYOPENINGRUNNUMBER
            sql.Append("       COPYCLOSINGRUNNUMBER = @Copyclosingrunnumber, ");  // COPYCLOSINGRUNNUMBER
            sql.Append("       COPYTOTALSALEAMOUNT = @Copytotalsaleamount, ");  // COPYTOTALSALEAMOUNT
            sql.Append("       COPYTOTALRETURNAMOUNT = @Copytotalreturnamount, ");  // COPYTOTALRETURNAMOUNT
            sql.Append("       COPYTOTALTAXSALEAMOUNT = @Copytotaltaxsaleamount, ");  // COPYTOTALTAXSALEAMOUNT
            sql.Append("       COPYTOTALTAXRETURNAMOUNT = @Copytotaltaxreturnamount, ");  // COPYTOTALTAXRETURNAMOUNT
            sql.Append("       TRAININGTOTALRECEIPTS = @Trainingtotalreceipts, ");  // TRAININGTOTALRECEIPTS
            sql.Append("       TRAININGOPENINGRUNNUMBER = @Trainingopeningrunnumber, ");  // TRAININGOPENINGRUNNUMBER
            sql.Append("       TRAININGCLOSINGRUNNUMBER = @Trainingclosingrunnumber, ");  // TRAININGCLOSINGRUNNUMBER
            sql.Append("       TRAININGTOTALSALEAMOUNT = @Trainingtotalsaleamount, ");  // TRAININGTOTALSALEAMOUNT
            sql.Append("       TRAININGTOTALRETURNAMOUNT = @Trainingtotalreturnamount, ");  // TRAININGTOTALRETURNAMOUNT
            sql.Append("       TRAININGTOTALTAXSALEAMOUNT = @Trainingtotaltaxsaleamount, ");  // TRAININGTOTALTAXSALEAMOUNT
            sql.Append("       TRAININGTOTALTAXRETURNAMOUNT = @Trainingtotaltaxreturnamount, ");  // TRAININGTOTALTAXRETURNAMOUNT
            sql.Append("       PROFORMATOTALRECEIPTS = @Proformatotalreceipts, ");  // PROFORMATOTALRECEIPTS
            sql.Append("       PROFORMAOPENINGRUNNUMBER = @Proformaopeningrunnumber, ");  // PROFORMAOPENINGRUNNUMBER
            sql.Append("       PROFORMACLOSINGRUNNUMBER = @Proformaclosingrunnumber, ");  // PROFORMACLOSINGRUNNUMBER
            sql.Append("       PROFORMATOTALSALEAMOUNT = @Proformatotalsaleamount, ");  // PROFORMATOTALSALEAMOUNT
            sql.Append("       PROFORMATOTALRETURNAMOUNT = @Proformatotalreturnamount, ");  // PROFORMATOTALRETURNAMOUNT
            sql.Append("       PROFORMATOTALTAXSALEAMOUNT = @Proformatotaltaxsaleamount, ");  // PROFORMATOTALTAXSALEAMOUNT
            sql.Append("       PROFORMATOTALTAXRETURNAMOUNT = @Proformatotaltaxreturnamount, ");  // PROFORMATOTALTAXRETURNAMOUNT
            sql.Append("       TOTNBRECEIPTS = @Totnbreceipts, ");  // TOTNBRECEIPTS
            sql.Append("       TOTNBRECEIPTSNORMAL = @Totnbreceiptsnormal, ");  // TOTNBRECEIPTSNORMAL
            sql.Append("       TOTNBRECEIPTSCOPY = @Totnbreceiptscopy, ");  // TOTNBRECEIPTSCOPY
            sql.Append("       TOTNBRECEIPTSTRAINING = @Totnbreceiptstraining, ");  // TOTNBRECEIPTSTRAINING
            sql.Append("       TOTNBRECEIPTSPROFORMA = @Totnbreceiptsproforma, ");  // TOTNBRECEIPTSPROFORMA
            sql.Append("       TOTALSALEAMOUNT = @Totalsaleamount, ");  // TOTALSALEAMOUNT
            sql.Append("       TOTALSALESTAXAMOUNT = @Totalsalestaxamount, ");  // TOTALSALESTAXAMOUNT
            sql.Append("       TOTALRETURNAMOUNT = @Totalreturnamount, ");  // TOTALRETURNAMOUNT
            sql.Append("       TOTALRETURNTAXAMOUNT = @Totalreturntaxamount, ");  // TOTALRETURNTAXAMOUNT
            sql.Append("       REG_DT = @RegDt, ");         // REG_DT
            sql.Append("       USERID = @UserId, ");    // USERID
            sql.Append("       USERNAME = @Username ");    // USERNAME
            sql.Append(" where TIN = @Tin ");         // TIN
            sql.Append("   and BHF_ID = @BhfId ");    // BHF_ID
            sql.Append("   and REPORTDATE = @Reportdate ");  // REPORTDATE
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ZREPORT "); // ZREPORT
            sql.Append(" where TIN = @Tin ");         // TIN
            sql.Append("   and BHF_ID = @BhfId ");    // BHF_ID
            sql.Append("   and REPORTDATE = @Reportdate ");  // REPORTDATE
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, ZreportRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // TIN

            param = command.CreateParameter();
            param.ParameterName = "@BhfId";
            param.Value = record.BhfId;
            command.Parameters.Add(param);                  // BHF_ID

            param = command.CreateParameter();
            param.ParameterName = "@SdcId";
            param.Value = record.SdcId;
            command.Parameters.Add(param);                  // SDC_ID

            param = command.CreateParameter();
            param.ParameterName = "@Reportdate";
            param.Value = record.Reportdate;
            command.Parameters.Add(param);                  // REPORTDATE

            param = command.CreateParameter();
            param.ParameterName = "@Reportnumber";
            param.Value = record.Reportnumber;
            command.Parameters.Add(param);                  // REPORTNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Dailynbofreceipts";
            param.Value = record.Dailynbofreceipts;
            command.Parameters.Add(param);                  // DAILYNBOFRECEIPTS

            param = command.CreateParameter();
            param.ParameterName = "@Openingrunnumber";
            param.Value = record.Openingrunnumber;
            command.Parameters.Add(param);                  // OPENINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Closingrunnumber";
            param.Value = record.Closingrunnumber;
            command.Parameters.Add(param);                  // CLOSINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Normaltotalreceipts";
            param.Value = record.Normaltotalreceipts;
            command.Parameters.Add(param);                  // NORMALTOTALRECEIPTS

            param = command.CreateParameter();
            param.ParameterName = "@Normalopeningrunnumber";
            param.Value = record.Normalopeningrunnumber;
            command.Parameters.Add(param);                  // NORMALOPENINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Normalclosingrunnumber";
            param.Value = record.Normalclosingrunnumber;
            command.Parameters.Add(param);                  // NORMALCLOSINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Normaltotalsaleamount";
            param.Value = record.Normaltotalsaleamount;
            command.Parameters.Add(param);                  // NORMALTOTALSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Normaltotalreturnamount";
            param.Value = record.Normaltotalreturnamount;
            command.Parameters.Add(param);                  // NORMALTOTALRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Normaltotaltaxsaleamount";
            param.Value = record.Normaltotaltaxsaleamount;
            command.Parameters.Add(param);                  // NORMALTOTALTAXSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Normaltotaltaxreturnamount";
            param.Value = record.Normaltotaltaxreturnamount;
            command.Parameters.Add(param);                  // NORMALTOTALTAXRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Copytotalreceipts";
            param.Value = record.Copytotalreceipts;
            command.Parameters.Add(param);                  // COPYTOTALRECEIPTS

            param = command.CreateParameter();
            param.ParameterName = "@Copyopeningrunnumber";
            param.Value = record.Copyopeningrunnumber;
            command.Parameters.Add(param);                  // COPYOPENINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Copyclosingrunnumber";
            param.Value = record.Copyclosingrunnumber;
            command.Parameters.Add(param);                  // COPYCLOSINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Copytotalsaleamount";
            param.Value = record.Copytotalsaleamount;
            command.Parameters.Add(param);                  // COPYTOTALSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Copytotalreturnamount";
            param.Value = record.Copytotalreturnamount;
            command.Parameters.Add(param);                  // COPYTOTALRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Copytotaltaxsaleamount";
            param.Value = record.Copytotaltaxsaleamount;
            command.Parameters.Add(param);                  // COPYTOTALTAXSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Copytotaltaxreturnamount";
            param.Value = record.Copytotaltaxreturnamount;
            command.Parameters.Add(param);                  // COPYTOTALTAXRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Trainingtotalreceipts";
            param.Value = record.Trainingtotalreceipts;
            command.Parameters.Add(param);                  // TRAININGTOTALRECEIPTS

            param = command.CreateParameter();
            param.ParameterName = "@Trainingopeningrunnumber";
            param.Value = record.Trainingopeningrunnumber;
            command.Parameters.Add(param);                  // TRAININGOPENINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Trainingclosingrunnumber";
            param.Value = record.Trainingclosingrunnumber;
            command.Parameters.Add(param);                  // TRAININGCLOSINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Trainingtotalsaleamount";
            param.Value = record.Trainingtotalsaleamount;
            command.Parameters.Add(param);                  // TRAININGTOTALSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Trainingtotalreturnamount";
            param.Value = record.Trainingtotalreturnamount;
            command.Parameters.Add(param);                  // TRAININGTOTALRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Trainingtotaltaxsaleamount";
            param.Value = record.Trainingtotaltaxsaleamount;
            command.Parameters.Add(param);                  // TRAININGTOTALTAXSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Trainingtotaltaxreturnamount";
            param.Value = record.Trainingtotaltaxreturnamount;
            command.Parameters.Add(param);                  // TRAININGTOTALTAXRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Proformatotalreceipts";
            param.Value = record.Proformatotalreceipts;
            command.Parameters.Add(param);                  // PROFORMATOTALRECEIPTS

            param = command.CreateParameter();
            param.ParameterName = "@Proformaopeningrunnumber";
            param.Value = record.Proformaopeningrunnumber;
            command.Parameters.Add(param);                  // PROFORMAOPENINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Proformaclosingrunnumber";
            param.Value = record.Proformaclosingrunnumber;
            command.Parameters.Add(param);                  // PROFORMACLOSINGRUNNUMBER

            param = command.CreateParameter();
            param.ParameterName = "@Proformatotalsaleamount";
            param.Value = record.Proformatotalsaleamount;
            command.Parameters.Add(param);                  // PROFORMATOTALSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Proformatotalreturnamount";
            param.Value = record.Proformatotalreturnamount;
            command.Parameters.Add(param);                  // PROFORMATOTALRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Proformatotaltaxsaleamount";
            param.Value = record.Proformatotaltaxsaleamount;
            command.Parameters.Add(param);                  // PROFORMATOTALTAXSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Proformatotaltaxreturnamount";
            param.Value = record.Proformatotaltaxreturnamount;
            command.Parameters.Add(param);                  // PROFORMATOTALTAXRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Totnbreceipts";
            param.Value = record.Totnbreceipts;
            command.Parameters.Add(param);                  // TOTNBRECEIPTS

            param = command.CreateParameter();
            param.ParameterName = "@Totnbreceiptsnormal";
            param.Value = record.Totnbreceiptsnormal;
            command.Parameters.Add(param);                  // TOTNBRECEIPTSNORMAL

            param = command.CreateParameter();
            param.ParameterName = "@Totnbreceiptscopy";
            param.Value = record.Totnbreceiptscopy;
            command.Parameters.Add(param);                  // TOTNBRECEIPTSCOPY

            param = command.CreateParameter();
            param.ParameterName = "@Totnbreceiptstraining";
            param.Value = record.Totnbreceiptstraining;
            command.Parameters.Add(param);                  // TOTNBRECEIPTSTRAINING

            param = command.CreateParameter();
            param.ParameterName = "@Totnbreceiptsproforma";
            param.Value = record.Totnbreceiptsproforma;
            command.Parameters.Add(param);                  // TOTNBRECEIPTSPROFORMA

            param = command.CreateParameter();
            param.ParameterName = "@Totalsaleamount";
            param.Value = record.Totalsaleamount;
            command.Parameters.Add(param);                  // TOTALSALEAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Totalsalestaxamount";
            param.Value = record.Totalsalestaxamount;
            command.Parameters.Add(param);                  // TOTALSALESTAXAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Totalreturnamount";
            param.Value = record.Totalreturnamount;
            command.Parameters.Add(param);                  // TOTALRETURNAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@Totalreturntaxamount";
            param.Value = record.Totalreturntaxamount;
            command.Parameters.Add(param);                  // TOTALRETURNTAXAMOUNT

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record.RegDt;
            command.Parameters.Add(param);                  // REG_DT

            param = command.CreateParameter();
            param.ParameterName = "@UserId";
            param.Value = record.UserId;
            command.Parameters.Add(param);                  // USERID

            param = command.CreateParameter();
            param.ParameterName = "@Username";
            param.Value = record.Username;
            command.Parameters.Add(param);                  // USERNAME

            return true;
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of BbsNoticeTable.
    /// </summary>
    public class BbsNoticeTable
    {
        public BbsNoticeTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists BBS_NOTICE ( "); 
            sql.Append("       NOTICE_NO          INT                not null , ");      // Notice No.
            sql.Append("       TITLE              VARCHAR(1000)      not null , ");      // Title
            sql.Append("       CONT               VARCHAR(4000)      not null , ");      // Contents
            sql.Append("       DTL_URL            VARCHAR(200)       null , ");          // Read Count
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       primary key ( NOTICE_NO ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select NOTICE_NO, ");               // Notice No.
            sql.Append("       TITLE, ");                   // Title
            sql.Append("       CONT, ");                    // Contents
            sql.Append("       DTL_URL, ");                  // Read Count
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       REG_DT  ");                  // Registered Date
            sql.Append("  from BBS_NOTICE ");               
            sql.Append("  order by NOTICE_NO desc "); 
            sql.Append("  LIMIT 10  ");
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into BBS_NOTICE ( ");
            sql.Append("       NOTICE_NO, ");               // Notice No.
            sql.Append("       TITLE, ");                   // Title
            sql.Append("       CONT, ");                    // Contents
            sql.Append("       DTL_URL, ");                  // Read Count
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       REG_DT  ");                  // Registered Date
            sql.Append("     ) values ( ");
            sql.Append("       @NoticeNo, ");               // Notice No.
            sql.Append("       @Title, ");                  // Title
            sql.Append("       @Cont, ");                   // Contents
            sql.Append("       @DtlUrl, ");                  // Read Count
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @ModrId, ");                 // Modifier ID
            sql.Append("       @ModrNm, ");                 // Modifier Name
            sql.Append("       @ModDt, ");                  // Modified Date
            sql.Append("       @RegDt  ");                  // Registered Date
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update BBS_NOTICE "); 
            sql.Append("   set TITLE = @Title, ");          // Title
            sql.Append("       CONT = @Cont, ");            // Contents
            sql.Append("       DTL_URL = @DtlUrl, ");         // Read Count
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt, ");         // Modified Date
            sql.Append("       REG_DT = @RegDt  ");         // Registered Date
            sql.Append(" where NOTICE_NO = @NoticeNo ");  // Notice No.
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from BBS_NOTICE "); // 게시판 공지사항
            sql.Append(" where NOTICE_NO = @NoticeNo ");  // Notice No.
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, BbsNoticeRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@NoticeNo";
            param.Value = record.NoticeNo;
            command.Parameters.Add(param);                  // Notice No.

            param = command.CreateParameter();
            param.ParameterName = "@Title";
            param.Value = record.Title;
            command.Parameters.Add(param);                  // Title

            param = command.CreateParameter();
            param.ParameterName = "@Cont";
            param.Value = record.Cont;
            command.Parameters.Add(param);                  // Contents

            param = command.CreateParameter();
            param.ParameterName = "@DtlUrl";
            param.Value = record.DtlUrl;
            command.Parameters.Add(param);                  // Read Count

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

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
            param.ParameterName = "@RegDt";
            param.Value = record.RegDt;
            command.Parameters.Add(param);                  // Registered Date

            return true;
        }
        public bool SetParametersSDC(IDbCommand command, Notice record, BbsNoticeRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@NoticeNo";
            param.Value = record.noticeNo;
            command.Parameters.Add(param);                  // Notice No.

            param = command.CreateParameter();
            param.ParameterName = "@Title";
            param.Value = string.IsNullOrEmpty(record.title) ? "" : record.title;
            command.Parameters.Add(param);                  // Title

            param = command.CreateParameter();
            param.ParameterName = "@Cont";
            param.Value = string.IsNullOrEmpty(record.cont) ? "" : record.cont;
            command.Parameters.Add(param);                  // Contents

            param = command.CreateParameter();
            param.ParameterName = "@DtlUrl";
            param.Value = record.dtlUrl;
            command.Parameters.Add(param);                  // Read Count

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record2.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record2.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record2.ModrId;
            command.Parameters.Add(param);                  // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record2.ModrNm;
            command.Parameters.Add(param);                  // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record2.ModDt;
            command.Parameters.Add(param);                  // Modified Date

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record2.RegDt;
            command.Parameters.Add(param);                  // Registered Date

            return true;
        }
    }
}

using EBM2x.Database.Master;
using EBM2x.Utils;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerBhfDeviceUserTable.
    /// </summary>
    public class TaxpayerBhfDeviceUserTable
    {
        public TaxpayerBhfDeviceUserTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TAXPAYER_BHF_DEVICE_USER ( "); 
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       USER_ID            VARCHAR(20)        not null , ");      // User ID
            sql.Append("       USER_NM            VARCHAR(60)        null , ");          // User Name
            sql.Append("       PWD                VARCHAR(255)       null , ");          // Password
            sql.Append("       ADRS               VARCHAR(200)       null , ");          // Address
            sql.Append("       CNTC               VARCHAR(20)        null , ");          // Contact
            sql.Append("       AUTH_CD            VARCHAR(100)       null , ");          // Authority Code
            sql.Append("       REMARK             VARCHAR(400)       null , ");          // Remark
            sql.Append("       USE_YN             CHAR(1)            null , ");          // Use(Y/N)
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // 
            sql.Append("       CONTACT            VARCHAR(20)        not null , ");      
            sql.Append("       ROLE_CD            CHAR(1)            null , ");          
            sql.Append("       IMAGE_NM           VARCHAR(100)       null , ");          
            sql.Append("       IMAGE              BLOB               null , ");          
            sql.Append("       primary key ( TIN, BHF_ID, USER_ID ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       USER_ID, ");                 // User ID
            sql.Append("       USER_NM, ");                 // User Name
            sql.Append("       PWD, ");                     // Password
            sql.Append("       ADRS, ");                    // Address
            sql.Append("       CNTC, ");                    // Contact
            sql.Append("       AUTH_CD, ");                 // Authority Code
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // 
            sql.Append("       CONTACT, ");                 // 사용자전화번호
            sql.Append("       ROLE_CD, ");                 // 권한코드
            sql.Append("       IMAGE_NM, ");                // 사용자 사진 파일 경로
            sql.Append("       IMAGE  ");                  // 사용자 사진 파일
            sql.Append("  from TAXPAYER_BHF_DEVICE_USER "); // 납세자 지점 장비 사용자
            return sql.ToString();
        }

        public string GetInsertSQL(bool imageUse)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TAXPAYER_BHF_DEVICE_USER ( "); // 납세자 지점 장비 사용자
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       USER_ID, ");                 // User ID
            sql.Append("       USER_NM, ");                 // User Name
            sql.Append("       PWD, ");                     // Password
            sql.Append("       ADRS, ");                    // Address
            sql.Append("       CNTC, ");                    // Contact
            sql.Append("       AUTH_CD, ");                 // Authority Code
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // 
            sql.Append("       CONTACT, ");                 // 사용자전화번호
            if (imageUse)
            {
                sql.Append("       ROLE_CD, ");                 // 권한코드
                sql.Append("       IMAGE_NM, ");                // 사용자 사진 파일 경로
                sql.Append("       IMAGE  ");                   // 사용자 사진 파일
            }
            else
            {
                sql.Append("       ROLE_CD ");                 // 권한코드
            }
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @UserId, ");                 // User ID
            sql.Append("       @UserNm, ");                 // User Name
            sql.Append("       @Pwd, ");                    // Password
            sql.Append("       @Adrs, ");                   // Address
            sql.Append("       @Cntc, ");                   // Contact
            sql.Append("       @AuthCd, ");                 // Authority Code
            sql.Append("       @Remark, ");                 // Remark
            sql.Append("       @UseYn, ");                  // Use(Y/N)
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @RegDt, ");                  // 
            sql.Append("       @Contact, ");                // 사용자전화번호
            if (imageUse)
            {
                sql.Append("       @RoleCd, ");                 // 권한코드
                sql.Append("       @ImageNm, ");                // 사용자 사진 파일 경로
                sql.Append("       @Image  ");                  // 사용자 사진 파일
            }
            else
            {
                sql.Append("       @RoleCd  ");                 // 권한코드
            }
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL(bool imageUse)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_BHF_DEVICE_USER "); // 납세자 지점 장비 사용자
            sql.Append("   set USER_NM = @UserNm, ");       // User Name
            sql.Append("       PWD = @Pwd, ");              // Password
            sql.Append("       ADRS = @Adrs, ");            // Address
            sql.Append("       CNTC = @Cntc, ");            // Contact
            sql.Append("       AUTH_CD = @AuthCd, ");       // Authority Code
            sql.Append("       REMARK = @Remark, ");        // Remark
            sql.Append("       USE_YN = @UseYn, ");         // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // 
            sql.Append("       CONTACT = @Contact, ");      // 사용자전화번호
            if (imageUse)
            {
                sql.Append("       ROLE_CD = @RoleCd, ");       // 권한코드
                sql.Append("       IMAGE_NM = @ImageNm, ");     // 사용자 사진 파일 경로
                sql.Append("       IMAGE = @Image  ");          // 사용자 사진 파일
            }
            else
            {
                sql.Append("       ROLE_CD = @RoleCd  ");       // 권한코드
            }
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and USER_ID = @UserId ");  // User ID
            return sql.ToString();

        }
        public string GetDeleteUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_BHF_DEVICE_USER "); // 납세자 지점 장비 사용자
            sql.Append("   set USE_YN = 'N', ");         // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt  ");         // 
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and USER_ID = @UserId ");  // User ID
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TAXPAYER_BHF_DEVICE_USER "); // 납세자 지점 장비 사용자
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and USER_ID = @UserId ");  // User ID
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerBhfDeviceUserRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@BhfId";
            param.Value = record.BhfId;
            command.Parameters.Add(param);                  // Branch Office ID

            param = command.CreateParameter();
            param.ParameterName = "@UserId";
            param.Value = record.UserId;
            command.Parameters.Add(param);                  // User ID

            param = command.CreateParameter();
            param.ParameterName = "@UserNm";
            param.Value = record.UserNm;
            command.Parameters.Add(param);                  // User Name

            // 암호화 처리
            string pwd = "{{#}}" + Common.AESEncrypt256(Common.AES256Password(), record.Pwd);

            param = command.CreateParameter();
            param.ParameterName = "@Pwd";
            param.Value = pwd; // record.Pwd;
            command.Parameters.Add(param);                  // Password

            param = command.CreateParameter();
            param.ParameterName = "@Adrs";
            param.Value = record.Adrs;
            command.Parameters.Add(param);                  // Address

            param = command.CreateParameter();
            param.ParameterName = "@Cntc";
            param.Value = record.Cntc;
            command.Parameters.Add(param);                  // Contact

            param = command.CreateParameter();
            param.ParameterName = "@AuthCd";
            param.Value = record.AuthCd;
            command.Parameters.Add(param);                  // Authority Code

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = record.Remark;
            command.Parameters.Add(param);                  // Remark

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.UseYn;
            command.Parameters.Add(param);                  // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record.RegDt;
            command.Parameters.Add(param);                  // 

            param = command.CreateParameter();
            param.ParameterName = "@Contact";
            param.Value = record.Contact;
            command.Parameters.Add(param);                  // 사용자전화번호

            param = command.CreateParameter();
            param.ParameterName = "@RoleCd";
            param.Value = record.RoleCd;
            command.Parameters.Add(param);                  // 권한코드

            if (record.Image != null)
            {
                param = command.CreateParameter();
                param.ParameterName = "@ImageNm";
                param.Value = record.ImageNm;
                command.Parameters.Add(param);                  // 사용자 사진 파일 경로

                param = command.CreateParameter();
                param.ParameterName = "@Image";
                param.Value = record.Image;
                command.Parameters.Add(param);                  // 사용자 사진 파일
            }

            return true;
        }
    }
}

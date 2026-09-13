using System;
using System.Data;
using System.Text;
namespace EBM2x.Database.Master
{
    using EBM2x.Database.TableIO;
    using EBM2x.Database.Tables;
    using EBM2x.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Description of TaxpayerBhfDeviceUserMaster.
    /// </summary>
    public class TaxpayerBhfDeviceUserMaster : ModelIO
    {
        public List<TaxpayerBhfDeviceUserRecord> getTaxpayerBhfDeviceUserTable(string valueTin, string valueBhfid, string likeValue, string valueUseYn)
        {
            List<TaxpayerBhfDeviceUserRecord> arrayList = new List<TaxpayerBhfDeviceUserRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfDeviceUserTable taxpayerBhfDeviceUserTable = new TaxpayerBhfDeviceUserTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfDeviceUserTable.GetSelectSQL());
            sql.Append(" where TIN = @TIN ");
            sql.Append("   and BHF_ID = @BHF_ID ");
            if (string.IsNullOrEmpty(likeValue))
            {
                if (!string.IsNullOrEmpty(valueUseYn)) sql.Append("   and USE_YN = @USE_YN");
                sql.Append(" order by reg_dt desc ");
                sql.Append(" LIMIT 20 ");
            }
            else
            {
                if (!string.IsNullOrEmpty(likeValue)) sql.Append("   and (USER_ID like @likeValue or USER_NM like @likeValue )");
                if (!string.IsNullOrEmpty(valueUseYn)) sql.Append("   and USE_YN = @USE_YN");
                sql.Append(" order by USER_ID ASC ");
            }

            CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valueTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = valueBhfid;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@likeValue";
                param.Value = MakeLikeString(likeValue);
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@USE_YN";
                param.Value = valueUseYn;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    TaxpayerBhfDeviceUserRecord record = new TaxpayerBhfDeviceUserRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.UserId = reader.GetString(2);               // User ID
                    if (!reader.IsDBNull(3)) record.UserNm = reader.GetString(3);               // User Name
                    if (!reader.IsDBNull(4)) record.Pwd = reader.GetString(4);                  // Password

                    // 복호화 처리
                    if (!string.IsNullOrEmpty(record.Pwd) && record.Pwd.Length > 5 && record.Pwd.Substring(0, 5).Equals("{{#}}"))
                    {
                        string pwd = record.Pwd.Substring(5);
                        record.Pwd = Common.AESDecrypt256(Common.AES256Password(), pwd);
                    }

                    if (!reader.IsDBNull(5)) record.Adrs = reader.GetString(5);                 // Address
                    if (!reader.IsDBNull(6)) record.Cntc = reader.GetString(6);                 // Contact
                    if (!reader.IsDBNull(7)) record.AuthCd = reader.GetString(7);               // Authority Code
                    if (!reader.IsDBNull(8)) record.Remark = reader.GetString(8);               // Remark
                    if (!reader.IsDBNull(9)) record.UseYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.RegrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.RegrNm = reader.GetString(11);             // Registrant Name
                    if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // 
                    if (!reader.IsDBNull(13)) record.Contact = reader.GetString(13);           
                    if (!reader.IsDBNull(14)) record.RoleCd = reader.GetString(14);
                    if (!reader.IsDBNull(15)) record.ImageNm = reader.GetString(15);

                    // JINIT_20191210, DB Image 
                    if (!reader.IsDBNull(16))
                    {
                        try
                        {
                            record.Image = (System.Byte[])reader.GetValue(16);
                        }
                        catch
                        {
                            record.Image = null;
                        }
                    }

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TaxpayerBhfDeviceUserRecord record in arrayList)
            {
                record.RoleName = CodeDtlMaster.RoleName(record.RoleCd);
            }

            return arrayList;
        }

        public bool ToRecord(TaxpayerBhfDeviceUserRecord record, string valTin, string valBhfId, string valUserId)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfDeviceUserTable taxpayerBhfDeviceUserTable = new TaxpayerBhfDeviceUserTable();

            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfDeviceUserTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND USER_ID = @USER_ID ");

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
                param.ParameterName = "@USER_ID";
                param.Value = valUserId;
                command.Parameters.Add(param);

                CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.UserId = reader.GetString(2);               // User ID
                    if (!reader.IsDBNull(3)) record.UserNm = reader.GetString(3);               // User Name
                    if (!reader.IsDBNull(4)) record.Pwd = reader.GetString(4);                  // Password
                    
                   
                    if(!string.IsNullOrEmpty(record.Pwd) && record.Pwd.Length > 5 && record.Pwd.Substring(0,5).Equals("{{#}}") )
                    {
                        string pwd = record.Pwd.Substring(5);
                        record.Pwd = Common.AESDecrypt256(Common.AES256Password(), pwd);
                    }

                    if (!reader.IsDBNull(5)) record.Adrs = reader.GetString(5);                 // Address
                    if (!reader.IsDBNull(6)) record.Cntc = reader.GetString(6);                 // Contact
                    if (!reader.IsDBNull(7)) record.AuthCd = reader.GetString(7);               // Authority Code
                    if (!reader.IsDBNull(8)) record.Remark = reader.GetString(8);               // Remark
                    if (!reader.IsDBNull(9)) record.UseYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.RegrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.RegrNm = reader.GetString(11);             // Registrant Name
                    if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // 
                    if (!reader.IsDBNull(13)) record.Contact = reader.GetString(13);            // 사용자전화번호
                    if (!reader.IsDBNull(14)) record.RoleCd = reader.GetString(14);             // 권한코드
                    if (!reader.IsDBNull(15)) record.ImageNm = reader.GetString(15);            // 사용자 사진 파일 경로

                    // JINIT_20191210, 
                    //if (!reader.IsDBNull(16)) record.Image = reader.GetString(16);              // 사용자 사진 파일
                    if (!reader.IsDBNull(16))
                    {
                        try
                        {
                            record.Image = (System.Byte[])reader.GetValue(16);
                        }
                        catch
                        {
                            record.Image = null;
                        }
                    }

                    reader.Close();

                    record.RoleName = CodeDtlMaster.RoleName(record.RoleCd);

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

        public bool ToTable(TaxpayerBhfDeviceUserRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfDeviceUserTable taxpayerBhfDeviceUserTable = new TaxpayerBhfDeviceUserTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBhfDeviceUserTable.SetParameters(command, record);
 
                command.CommandText = taxpayerBhfDeviceUserTable.GetUpdateSQL((record.Image != null));
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBhfDeviceUserTable.GetInsertSQL((record.Image != null));
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

        public bool DeleteTable(TaxpayerBhfDeviceUserRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfDeviceUserTable taxpayerBhfDeviceUserTable = new TaxpayerBhfDeviceUserTable();

            try
            {
                command.CommandText = taxpayerBhfDeviceUserTable.GetDeleteUpdateSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerBhfDeviceUserTable.SetParameters(command, record);
                if (command.ExecuteNonQuery() >= 1) return true;
                else return false;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
        public bool ChangePassword(TaxpayerBhfDeviceUserRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_BHF_DEVICE_USER "); 
            sql.Append("   set PWD = @Pwd, ");              // Password
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // JINIT_20191208, Registrant Name
            sql.Append("       REG_DT = @RegDt ");          // 
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND USER_ID = @USER_ID ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = record.Tin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = record.BhfId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@USER_ID";
                param.Value = record.UserId;
                command.Parameters.Add(param);


                string pwd = "{{#}}" + Common.AESEncrypt256(Common.AES256Password(), record.Pwd);

                param = command.CreateParameter();
                param.ParameterName = "@Pwd";
                param.Value = pwd;     // record.Pwd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@RegrId";
                param.Value = record.RegrId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@RegrNm"; // JINIT_20191208
                param.Value = record.RegrNm;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@RegDt";
                param.Value = record.RegDt;
                command.Parameters.Add(param);

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

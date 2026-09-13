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
    /// Description of ItemRraSdcUpload.
    /// </summary>
    public class BhfUserRraSdcUpload : ModelIO
    {
        public void SendBhfUserSave(string valTin, string valBhfId, string valUserId)
        {
            List<BhfUserSaveReq> sendList = getBhfUserTable(valTin, valBhfId, valUserId);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "BhfUser";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_BHF_USER_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<BhfUserSaveReq> getBhfUserTable(string valTin, string valBhfId, string valUserId)
        {
            List<BhfUserSaveReq> arrayList = new List<BhfUserSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
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

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BhfUserSaveReq record = new BhfUserSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.userId = reader.GetString(2);               // User ID
                    if (!reader.IsDBNull(3)) record.userNm = reader.GetString(3);               // User Name
                    if (!reader.IsDBNull(4)) record.pwd = reader.GetString(4);                  // Password
                    if (!reader.IsDBNull(5)) record.adrs = reader.GetString(5);                 // Address
                    if (!reader.IsDBNull(6)) record.cntc = reader.GetString(6);                 // Contact
                    if (!reader.IsDBNull(7)) record.authCd = reader.GetString(7);               // Authority Code
                    if (!reader.IsDBNull(8)) record.remark = reader.GetString(8);               // Remark
                    if (!reader.IsDBNull(9)) record.useYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.regrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.regrNm = reader.GetString(11);             // Registrant Name
                    if (!reader.IsDBNull(10)) record.modrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.modrNm = reader.GetString(11);             // Registrant Name
                    //if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // 
                    //if (!reader.IsDBNull(13)) record.Contact = reader.GetString(13);            // 사용자전화번호
                    //if (!reader.IsDBNull(14)) record.RoleCd = reader.GetString(14);             // 권한코드
                    //if (!reader.IsDBNull(15)) record.ImageNm = reader.GetString(15);            // 사용자 사진 파일 경로
                    //if (!reader.IsDBNull(16)) record.Image = reader.GetString(16);              // 사용자 사진 파일
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

        public List<BhfUserSaveReq> getBhfUserTable()
        {
            List<BhfUserSaveReq> arrayList = new List<BhfUserSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfDeviceUserTable taxpayerBhfDeviceUserTable = new TaxpayerBhfDeviceUserTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfDeviceUserTable.GetSelectSQL());

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BhfUserSaveReq record = new BhfUserSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.userId = reader.GetString(2);               // User ID
                    if (!reader.IsDBNull(3)) record.userNm = reader.GetString(3);               // User Name
                    if (!reader.IsDBNull(4)) record.pwd = reader.GetString(4);                  // Password
                    if (!reader.IsDBNull(5)) record.adrs = reader.GetString(5);                 // Address
                    if (!reader.IsDBNull(6)) record.cntc = reader.GetString(6);                 // Contact
                    if (!reader.IsDBNull(7)) record.authCd = reader.GetString(7);               // Authority Code
                    if (!reader.IsDBNull(8)) record.remark = reader.GetString(8);               // Remark
                    if (!reader.IsDBNull(9)) record.useYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.regrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.regrNm = reader.GetString(11);             // Registrant Name
                    if (!reader.IsDBNull(10)) record.modrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.modrNm = reader.GetString(11);             // Registrant Name
                    //if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // 
                    //if (!reader.IsDBNull(13)) record.Contact = reader.GetString(13);            // 사용자전화번호
                    //if (!reader.IsDBNull(14)) record.RoleCd = reader.GetString(14);             // 권한코드
                    //if (!reader.IsDBNull(15)) record.ImageNm = reader.GetString(15);            // 사용자 사진 파일 경로
                    //if (!reader.IsDBNull(16)) record.Image = reader.GetString(16);              // 사용자 사진 파일
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
    }
}

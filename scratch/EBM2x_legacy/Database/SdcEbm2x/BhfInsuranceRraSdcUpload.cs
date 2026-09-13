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
    public class BhfInsuranceRraSdcUpload : ModelIO
    {
        public void SendBhfInsuranceSave(string valTin, string valBhfId, string valIssrcc)
        {
            List<BhfInsuranceSaveReq> sendList = getBhfInsuranceTable(valTin, valBhfId, valIssrcc);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "BhfInsurance";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_BHF_INSURANCE_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
            
        }

        public List<BhfInsuranceSaveReq> getBhfInsuranceTable(string valTin, string valBhfId, string valIssrcc)
        {
            List<BhfInsuranceSaveReq> arrayList = new List<BhfInsuranceSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfInsuranceTable taxpayerBhfInsuranceTable = new TaxpayerBhfInsuranceTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfInsuranceTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND ISSRCC_CD = @ISSRCC_CD ");

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
                param.ParameterName = "@ISSRCC_CD";
                param.Value = valIssrcc;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BhfInsuranceSaveReq record = new BhfInsuranceSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.isrccCd = reader.GetString(2);              // Insurance Company Code
                    if (!reader.IsDBNull(3)) record.isrccNm = reader.GetString(3);              // Insurance Company Name
                    if (!reader.IsDBNull(4)) record.isrcRt = reader.GetInt16(4);                // Insurance Rate
                    if (!reader.IsDBNull(5))
                    {
                        record.useYn = reader.GetString(5);                // Use(Y/N)
                        if (string.IsNullOrEmpty(record.useYn)) record.useYn = "Y";
                    }
                    if (!reader.IsDBNull(6)) record.modrId = reader.GetString(6);             // Modifier ID
                    if (!reader.IsDBNull(7)) record.modrNm = reader.GetString(7);             // Modifier Name
                    //if (!reader.IsDBNull(8)) record.ModDt = reader.GetString(8);              // Modified Date
                    if (!reader.IsDBNull(9)) record.regrId = reader.GetString(9);             // Registrant ID
                    if (!reader.IsDBNull(10)) record.regrNm = reader.GetString(10);           // Registrant Name
                    //if (!reader.IsDBNull(11)) record.RegDt = reader.GetString(11);            // Registered Date
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

        public List<BhfInsuranceSaveReq> getBhfInsuranceTable()
        {
            List<BhfInsuranceSaveReq> arrayList = new List<BhfInsuranceSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfInsuranceTable taxpayerBhfInsuranceTable = new TaxpayerBhfInsuranceTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfInsuranceTable.GetSelectSQL());

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BhfInsuranceSaveReq record = new BhfInsuranceSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.isrccCd = reader.GetString(2);              // Insurance Company Code
                    if (!reader.IsDBNull(3)) record.isrccNm = reader.GetString(3);              // Insurance Company Name
                    if (!reader.IsDBNull(4)) record.isrcRt = reader.GetInt16(4);                // Insurance Rate
                    if (!reader.IsDBNull(5))
                    {
                        record.useYn = reader.GetString(5);                // Use(Y/N)
                        if (string.IsNullOrEmpty(record.useYn)) record.useYn = "Y";
                    }
                    if (!reader.IsDBNull(6)) record.modrId = reader.GetString(6);             // Modifier ID
                    if (!reader.IsDBNull(7)) record.modrNm = reader.GetString(7);             // Modifier Name
                    //if (!reader.IsDBNull(8)) record.ModDt = reader.GetString(8);              // Modified Date
                    if (!reader.IsDBNull(9)) record.regrId = reader.GetString(9);             // Registrant ID
                    if (!reader.IsDBNull(10)) record.regrNm = reader.GetString(10);           // Registrant Name
                    //if (!reader.IsDBNull(11)) record.RegDt = reader.GetString(11);            // Registered Date
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

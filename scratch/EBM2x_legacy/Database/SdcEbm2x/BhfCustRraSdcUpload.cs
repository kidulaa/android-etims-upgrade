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
    public class BhfCustRraSdcUpload : ModelIO
    {
        public void SendBhfCustSave(string valTin, string valBhfId, string valCustNo)
        {
            List<BhfCustSaveReq> sendList = getBhfCustTable(valTin, valBhfId, valCustNo);
            for (int i = 0; i < sendList.Count; i++)
            {
                RraSdcUploadModel rraSdcUploadModel = new RraSdcUploadModel();
                rraSdcUploadModel.FileType = "BhfCust";
                rraSdcUploadModel.RequestDt = DateTime.Now.ToString("yyyyMMddHHmmss");
                rraSdcUploadModel.FunctionName = RraSdcService.URL_BHF_CUST_SAVE;
                rraSdcUploadModel.JsonRequest = JsonConvert.SerializeObject(sendList[i]);

                RraSdcJsonWriter.WriteTransaction(rraSdcUploadModel);
            }
        }

        public List<BhfCustSaveReq> getBhfCustTable(string valTin, string valBhfId, string valCustNo)
        {
            List<BhfCustSaveReq> arrayList = new List<BhfCustSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfCustTable taxpayerBhfCustTable = new TaxpayerBhfCustTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfCustTable.GetSelectSQL());
            sql.Append(" WHERE TIN = @TIN ");
            sql.Append("   AND BHF_ID = @BHF_ID ");
            sql.Append("   AND CUST_NO = @CUST_NO ");

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
                param.ParameterName = "@CUST_NO";
                param.Value = valCustNo;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BhfCustSaveReq record = new BhfCustSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.custNo = reader.GetString(2);               // Customer No.
                    if (!reader.IsDBNull(3)) record.custTin = reader.GetString(3);              // Customer Taxpayer Identification Number(TIN)
                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length != 9)
                    {
                        record.custTin = record.custTin.PadRight(9, '0');
                    }
                    if (!reader.IsDBNull(4)) record.custBhfId = reader.GetString(4);            // Customer Branch ID
                    if(!string.IsNullOrEmpty(record.custBhfId) && record.custBhfId.Length != 9)
                    {
                        record.custBhfId = record.custBhfId.PadRight(9, '0');
                    }
                    if (!reader.IsDBNull(5)) record.custNid = reader.GetString(5);              // Customer National Idetification
                    if (!reader.IsDBNull(6)) record.custNm = reader.GetString(6);               // Customer Name
                    if (!reader.IsDBNull(7)) record.telNo = reader.GetString(7);                // Telephone Number
                    if (!reader.IsDBNull(8)) record.adrs = reader.GetString(8);                 // Address
                    if (!reader.IsDBNull(9)) record.useYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.regrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.regrNm = reader.GetString(11);             // Registrant Name
                    //if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // Registered Date
                    if (!reader.IsDBNull(13)) record.modrId = reader.GetString(13);             // Modifier ID
                    if (!reader.IsDBNull(14)) record.modrNm = reader.GetString(14);             // Modifier Name
                    //if (!reader.IsDBNull(15)) record.ModDt = reader.GetString(15);      // Modified Date
                    //if (!reader.IsDBNull(16)) record.NationCd = reader.GetString(16);           // 거래처 국가코드
                    //if (!reader.IsDBNull(17)) record.ChargerNm = reader.GetString(17);          // 거래처 담당자명
                    //if (!reader.IsDBNull(18)) record.Contact1 = reader.GetString(18);           // 거래처 전화번호1
                    //if (!reader.IsDBNull(19)) record.Contact2 = reader.GetString(19);           // 거래처 전화번호2
                    if (!reader.IsDBNull(20)) record.email = reader.GetString(20);              // 거래처 이메일
                    if (!reader.IsDBNull(21)) record.faxNo = reader.GetString(21);                // 거래처 팩스
                    if (!reader.IsDBNull(22)) record.remark = reader.GetString(22);                 // 거라채 비고
                    //if (!reader.IsDBNull(23)) record.InitlUnclamt = reader.GetDouble(23);       // 초기 미수금
                    //if (!reader.IsDBNull(24)) record.InitlNpyamt = reader.GetDouble(24);        // 초기 미납급
                    //if (!reader.IsDBNull(25)) record.Unclamt = reader.GetDouble(25);            // 미수금 합계
                    //if (!reader.IsDBNull(26)) record.Npyamt = reader.GetDouble(26);             // 미납금 합계
                    //if (!reader.IsDBNull(27)) record.BcncType = reader.GetString(27);           // 거래처타입
                    //if (!reader.IsDBNull(28)) record.Bank = reader.GetString(28);               // 거래은행명
                    //if (!reader.IsDBNull(29)) record.Account = reader.GetString(29);            // 거래은행계좌
                    //if (!reader.IsDBNull(30)) record.Depositor = reader.GetString(30);          // 거래은행계좌주
                    //if (!reader.IsDBNull(31)) record.CustGroup = reader.GetString(31);          // 

                    //if (record.BcncType.Equals("01")) record.BcncTypeName = "Corperate";        // 거래처 TYPE
                    //else record.BcncTypeName = "Individual";
                    //record.NationName = CodeDtlMaster.OrgnNatName(record.NationCd);             // 국가명
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
        public List<BhfCustSaveReq> getBhfCustTable()
        {
            List<BhfCustSaveReq> arrayList = new List<BhfCustSaveReq>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfCustTable taxpayerBhfCustTable = new TaxpayerBhfCustTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfCustTable.GetSelectSQL());

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    BhfCustSaveReq record = new BhfCustSaveReq();

                    if (!reader.IsDBNull(0)) record.tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.bhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.custNo = reader.GetString(2);               // Customer No.
                    if (!reader.IsDBNull(3)) record.custTin = reader.GetString(3);              // Customer Taxpayer Identification Number(TIN)
                    if (!string.IsNullOrEmpty(record.custTin) && record.custTin.Length != 9)
                    {
                        record.custTin = record.custTin.PadRight(9, '0');
                    }
                    if (!reader.IsDBNull(4)) record.custBhfId = reader.GetString(4);            // Customer Branch ID
                    if (!string.IsNullOrEmpty(record.custBhfId) && record.custBhfId.Length != 9)
                    {
                        record.custBhfId = record.custBhfId.PadRight(9, '0');
                    }
                    if (!reader.IsDBNull(5)) record.custNid = reader.GetString(5);              // Customer National Idetification
                    if (!reader.IsDBNull(6)) record.custNm = reader.GetString(6);               // Customer Name
                    if (!reader.IsDBNull(7)) record.telNo = reader.GetString(7);                // Telephone Number
                    if (!reader.IsDBNull(8)) record.adrs = reader.GetString(8);                 // Address
                    //if (!reader.IsDBNull(9)) record.useYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.regrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.regrNm = reader.GetString(11);             // Registrant Name
                    //if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // Registered Date
                    if (!reader.IsDBNull(13)) record.modrId = reader.GetString(13);             // Modifier ID
                    if (!reader.IsDBNull(14)) record.modrNm = reader.GetString(14);             // Modifier Name
                    //if (!reader.IsDBNull(15)) record.ModDt = reader.GetString(15);      // Modified Date
                    //if (!reader.IsDBNull(16)) record.NationCd = reader.GetString(16);           // 거래처 국가코드
                    //if (!reader.IsDBNull(17)) record.ChargerNm = reader.GetString(17);          // 거래처 담당자명
                    //if (!reader.IsDBNull(18)) record.Contact1 = reader.GetString(18);           // 거래처 전화번호1
                    //if (!reader.IsDBNull(19)) record.Contact2 = reader.GetString(19);           // 거래처 전화번호2
                    //if (!reader.IsDBNull(20)) record.Email = reader.GetString(20);              // 거래처 이메일
                    //if (!reader.IsDBNull(21)) record.Fax = reader.GetString(21);                // 거래처 팩스
                    //if (!reader.IsDBNull(22)) record.Rm = reader.GetString(22);                 // 거라채 비고
                    //if (!reader.IsDBNull(23)) record.InitlUnclamt = reader.GetDouble(23);       // 초기 미수금
                    //if (!reader.IsDBNull(24)) record.InitlNpyamt = reader.GetDouble(24);        // 초기 미납급
                    //if (!reader.IsDBNull(25)) record.Unclamt = reader.GetDouble(25);            // 미수금 합계
                    //if (!reader.IsDBNull(26)) record.Npyamt = reader.GetDouble(26);             // 미납금 합계
                    //if (!reader.IsDBNull(27)) record.BcncType = reader.GetString(27);           // 거래처타입
                    //if (!reader.IsDBNull(28)) record.Bank = reader.GetString(28);               // 거래은행명
                    //if (!reader.IsDBNull(29)) record.Account = reader.GetString(29);            // 거래은행계좌
                    //if (!reader.IsDBNull(30)) record.Depositor = reader.GetString(30);          // 거래은행계좌주
                    //if (!reader.IsDBNull(31)) record.CustGroup = reader.GetString(31);          // 

                    //if (record.BcncType.Equals("01")) record.BcncTypeName = "Corperate";        // 거래처 TYPE
                    //else record.BcncTypeName = "Individual";
                    //record.NationName = CodeDtlMaster.OrgnNatName(record.NationCd);             // 국가명
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

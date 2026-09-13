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
    /// Description of TaxpayerBhfCustMaster.
    /// </summary>
    public class TaxpayerBhfCustMaster : ModelIO
    {
        public string GetTaxpayerBhfCustName(string valTin, string valBhfid, string valCustNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null) return "";

            try
            {
                command.CommandText = "SELECT CUST_NM FROM TAXPAYER_BHF_CUST WHERE TIN = @TIN AND BHF_ID = @BHF_ID AND CUST_NO = @CUST_NO ";
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = valBhfid;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CUST_NO";
                param.Value = valCustNo;
                command.Parameters.Add(param);

                string codeName = (string)command.ExecuteScalar();
                if (codeName == null) codeName = "";
                return codeName;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return "";
            }
        }

        public List<TaxpayerBhfCustRecord> getTaxpayerBhfCustTable(string valueTin, string valueBhfId, string likeValue, string valueUseYn)
        {
            List<TaxpayerBhfCustRecord> arrayList = new List<TaxpayerBhfCustRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            TaxpayerBhfCustTable taxpayerBhfCustTable = new TaxpayerBhfCustTable();
            StringBuilder sql = new StringBuilder();
            sql.Append(taxpayerBhfCustTable.GetSelectSQL());
            sql.Append(" where TIN = @TIN ");
            sql.Append("   and BHF_ID = @BHF_ID ");
            if (string.IsNullOrEmpty(likeValue))
            {
                if (!string.IsNullOrEmpty(valueUseYn)) sql.Append("   and USE_YN = @USE_YN");
                sql.Append(" order by mod_dt desc ");
                sql.Append(" LIMIT 20 ");
            }
            else
            {
                sql.Append("   and (CUST_TIN like @likeValue or CUST_NM like @likeValue )");
                if (!string.IsNullOrEmpty(valueUseYn)) sql.Append("   and USE_YN = @USE_YN");
                sql.Append(" order by CUST_TIN ASC ");
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
                param.Value = valueBhfId;
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
                    TaxpayerBhfCustRecord record = new TaxpayerBhfCustRecord();

                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.CustNo = reader.GetString(2);               // Customer No.
                    if (!reader.IsDBNull(3)) record.CustTin = reader.GetString(3);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(4)) record.CustBhfId = reader.GetString(4);            // Customer Branch ID
                    if (!reader.IsDBNull(5)) record.CustNid = reader.GetString(5);              // Customer National Idetification
                    if (!reader.IsDBNull(6)) record.CustNm = reader.GetString(6);               // Customer Name
                    if (!reader.IsDBNull(7)) record.TelNo = reader.GetString(7);                // Telephone Number
                    if (!reader.IsDBNull(8)) record.Adrs = reader.GetString(8);                 // Address
                    if (!reader.IsDBNull(9)) record.UseYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.RegrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.RegrNm = reader.GetString(11);             // Registrant Name
                    if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // Registered Date
                    if (!reader.IsDBNull(13)) record.ModrId = reader.GetString(13);             // Modifier ID
                    if (!reader.IsDBNull(14)) record.ModrNm = reader.GetString(14);             // Modifier Name
                    if (!reader.IsDBNull(15)) record.ModDt = reader.GetString(15);      // Modified Date
                    if (!reader.IsDBNull(16)) record.NationCd = reader.GetString(16);           
                    if (!reader.IsDBNull(17)) record.ChargerNm = reader.GetString(17);         
                    if (!reader.IsDBNull(18)) record.Contact1 = reader.GetString(18);        
                    if (!reader.IsDBNull(19)) record.Contact2 = reader.GetString(19);          
                    if (!reader.IsDBNull(20)) record.Email = reader.GetString(20);              
                    if (!reader.IsDBNull(21)) record.Fax = reader.GetString(21);                
                    if (!reader.IsDBNull(22)) record.Rm = reader.GetString(22);                
                    if (!reader.IsDBNull(23)) record.InitlUnclamt = reader.GetDouble(23);       
                    if (!reader.IsDBNull(24)) record.InitlNpyamt = reader.GetDouble(24);        
                    if (!reader.IsDBNull(25)) record.Unclamt = reader.GetDouble(25);            
                    if (!reader.IsDBNull(26)) record.Npyamt = reader.GetDouble(26);             
                    if (!reader.IsDBNull(27)) record.BcncType = reader.GetString(27);           
                    if (!reader.IsDBNull(28)) record.Bank = reader.GetString(28);              
                    if (!reader.IsDBNull(29)) record.Account = reader.GetString(29);            
                    if (!reader.IsDBNull(30)) record.Depositor = reader.GetString(30);          
                    if (!reader.IsDBNull(31)) record.CustGroup = reader.GetString(31);           

                    if (record.BcncType.Equals("01")) record.BcncTypeName = "Corperate";    
                    else record.BcncTypeName = "Individual";

                    arrayList.Add(record);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }

            foreach (TaxpayerBhfCustRecord record in arrayList)
            {
                record.NationName = CodeDtlMaster.OrgnNatName(record.NationCd);             // 국가명
            }

            return arrayList;
        }

        public bool ToRecord(TaxpayerBhfCustRecord record, string valTin, string valBhfId, string valCustNo)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
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

                CodeDtlMaster CodeDtlMaster = new CodeDtlMaster();
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) record.Tin = reader.GetString(0);                  // Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(1)) record.BhfId = reader.GetString(1);                // Branch Office ID
                    if (!reader.IsDBNull(2)) record.CustNo = reader.GetString(2);               // Customer No.
                    if (!reader.IsDBNull(3)) record.CustTin = reader.GetString(3);              // Customer Taxpayer Identification Number(TIN)
                    if (!reader.IsDBNull(4)) record.CustBhfId = reader.GetString(4);            // Customer Branch ID
                    if (!reader.IsDBNull(5)) record.CustNid = reader.GetString(5);              // Customer National Idetification
                    if (!reader.IsDBNull(6)) record.CustNm = reader.GetString(6);               // Customer Name
                    if (!reader.IsDBNull(7)) record.TelNo = reader.GetString(7);                // Telephone Number
                    if (!reader.IsDBNull(8)) record.Adrs = reader.GetString(8);                 // Address
                    if (!reader.IsDBNull(9)) record.UseYn = reader.GetString(9);                // Use(Y/N)
                    if (!reader.IsDBNull(10)) record.RegrId = reader.GetString(10);             // Registrant ID
                    if (!reader.IsDBNull(11)) record.RegrNm = reader.GetString(11);             // Registrant Name
                    if (!reader.IsDBNull(12)) record.RegDt = reader.GetString(12);      // Registered Date
                    if (!reader.IsDBNull(13)) record.ModrId = reader.GetString(13);             // Modifier ID
                    if (!reader.IsDBNull(14)) record.ModrNm = reader.GetString(14);             // Modifier Name
                    if (!reader.IsDBNull(15)) record.ModDt = reader.GetString(15);      // Modified Date
                    if (!reader.IsDBNull(16)) record.NationCd = reader.GetString(16);           // 거래처 국가코드
                    if (!reader.IsDBNull(17)) record.ChargerNm = reader.GetString(17);          // 거래처 담당자명
                    if (!reader.IsDBNull(18)) record.Contact1 = reader.GetString(18);           // 거래처 전화번호1
                    if (!reader.IsDBNull(19)) record.Contact2 = reader.GetString(19);           // 거래처 전화번호2
                    if (!reader.IsDBNull(20)) record.Email = reader.GetString(20);              // 거래처 이메일
                    if (!reader.IsDBNull(21)) record.Fax = reader.GetString(21);                // 거래처 팩스
                    if (!reader.IsDBNull(22)) record.Rm = reader.GetString(22);                 // 거라채 비고
                    if (!reader.IsDBNull(23)) record.InitlUnclamt = reader.GetDouble(23);       // 초기 미수금
                    if (!reader.IsDBNull(24)) record.InitlNpyamt = reader.GetDouble(24);        // 초기 미납급
                    if (!reader.IsDBNull(25)) record.Unclamt = reader.GetDouble(25);            // 미수금 합계
                    if (!reader.IsDBNull(26)) record.Npyamt = reader.GetDouble(26);             // 미납금 합계
                    if (!reader.IsDBNull(27)) record.BcncType = reader.GetString(27);           // 거래처타입
                    if (!reader.IsDBNull(28)) record.Bank = reader.GetString(28);               // 거래은행명
                    if (!reader.IsDBNull(29)) record.Account = reader.GetString(29);            // 거래은행계좌
                    if (!reader.IsDBNull(30)) record.Depositor = reader.GetString(30);          // 거래은행계좌주
                    if (!reader.IsDBNull(31)) record.CustGroup = reader.GetString(31);          // 

                    if (record.BcncType.Equals("01")) record.BcncTypeName = "Corperate";        // 거래처 TYPE
                    else record.BcncTypeName = "Individual";

                    reader.Close();

                    record.NationName = CodeDtlMaster.OrgnNatName(record.NationCd);             // 국가명

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

        public bool ToTable(TaxpayerBhfCustRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfCustTable taxpayerBhfCustTable = new TaxpayerBhfCustTable();

            try
            {
                command.Parameters.Clear();
                taxpayerBhfCustTable.SetParameters(command, record);
 
                command.CommandText = taxpayerBhfCustTable.GetUpdateSQL();
                command.CommandType = CommandType.Text;

                if (command.ExecuteNonQuery() < 1)
                {
                    command.CommandText = taxpayerBhfCustTable.GetInsertSQL();
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

        public bool DeleteTable(TaxpayerBhfCustRecord record)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            TaxpayerBhfCustTable taxpayerBhfCustTable = new TaxpayerBhfCustTable();

            try
            {
                command.CommandText = taxpayerBhfCustTable.GetDeleteUpdateSQL();
                command.CommandType = CommandType.Text;

                command.Parameters.Clear();
                taxpayerBhfCustTable.SetParameters(command, record);
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

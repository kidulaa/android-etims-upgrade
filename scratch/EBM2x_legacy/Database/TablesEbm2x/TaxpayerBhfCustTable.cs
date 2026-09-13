using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerBhfCustTable.
    /// </summary>
    public class TaxpayerBhfCustTable
    {
        public TaxpayerBhfCustTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TAXPAYER_BHF_CUST ( ");
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       CUST_NO            CHAR(9)            not null , ");      // Customer No.
            sql.Append("       CUST_TIN           CHAR(9)            null , ");          // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID        CHAR(2)            null , ");          // Customer Branch ID
            sql.Append("       CUST_NID           VARCHAR(20)        null , ");          // Customer National Idetification
            sql.Append("       CUST_NM            VARCHAR(60)        null , ");          // Customer Name
            sql.Append("       TEL_NO             VARCHAR(20)        null , ");          // Telephone Number
            sql.Append("       ADRS               VARCHAR(300)       null , ");          // Address
            sql.Append("       USE_YN             CHAR(1)            null , ");          // Use(Y/N)
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       NATION_CD          VARCHAR(3)         not null , ");     
            sql.Append("       CHARGER_NM         VARCHAR(60)        not null , ");      
            sql.Append("       CONTACT1           VARCHAR(20)        not null , ");     
            sql.Append("       CONTACT2           VARCHAR(20)        null , ");      
            sql.Append("       EMAIL              VARCHAR(5)         null , ");          
            sql.Append("       FAX                VARCHAR(20)        null , ");          
            sql.Append("       RM                 VARCHAR(400)       null , ");          
            sql.Append("       INITL_UNCLAMT      DECIMAL(18,2)      null , ");          
            sql.Append("       INITL_NPYAMT       DECIMAL(18,2)      null , ");          
            sql.Append("       UNCLAMT            DECIMAL(18,2)      null , ");          
            sql.Append("       NPYAMT             DECIMAL(18,2)      null , ");          
            sql.Append("       BCNC_TYPE          VARCHAR(3)         null , ");          
            sql.Append("       BANK               VARCHAR(50)        null , ");          
            sql.Append("       ACCOUNT            VARCHAR(30)        null , ");          
            sql.Append("       DEPOSITOR          VARCHAR(60)        null , ");          
            sql.Append("       CUST_GROUP         CHAR(2)            null , ");          // Customer group
            sql.Append("       primary key ( TIN, BHF_ID, CUST_NO ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       CUST_NO, ");                 // Customer No.
            sql.Append("       CUST_TIN, ");                // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID, ");             // Customer Branch ID
            sql.Append("       CUST_NID, ");                // Customer National Idetification
            sql.Append("       CUST_NM, ");                 // Customer Name
            sql.Append("       TEL_NO, ");                  // Telephone Number
            sql.Append("       ADRS, ");                    // Address
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       NATION_CD, ");               
            sql.Append("       CHARGER_NM, ");              
            sql.Append("       CONTACT1, ");                
            sql.Append("       CONTACT2, ");               
            sql.Append("       EMAIL, ");                   
            sql.Append("       FAX, ");                     
            sql.Append("       RM, ");                     
            sql.Append("       INITL_UNCLAMT, ");           
            sql.Append("       INITL_NPYAMT, ");            
            sql.Append("       UNCLAMT, ");                 
            sql.Append("       NPYAMT, ");                  
            sql.Append("       BCNC_TYPE, ");              
            sql.Append("       BANK, ");                  
            sql.Append("       ACCOUNT, ");                 
            sql.Append("       DEPOSITOR,  ");             
            sql.Append("       CUST_GROUP  ");             // 
            //sql.Append("       (CASE WHEN BCNC_TYPE = '01' THEN 'Corperate' ELSE 'Individual' END) AS BCNC_TYPE_NM, ");
            //sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = NATION_CD) AS NATION_NM, ");
            sql.Append("  from TAXPAYER_BHF_CUST "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TAXPAYER_BHF_CUST ( "); 
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       CUST_NO, ");                 // Customer No.
            sql.Append("       CUST_TIN, ");                // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID, ");             // Customer Branch ID
            sql.Append("       CUST_NID, ");                // Customer National Idetification
            sql.Append("       CUST_NM, ");                 // Customer Name
            sql.Append("       TEL_NO, ");                  // Telephone Number
            sql.Append("       ADRS, ");                    // Address
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       NATION_CD, ");               
            sql.Append("       CHARGER_NM, ");              
            sql.Append("       CONTACT1, ");               
            sql.Append("       CONTACT2, ");               
            sql.Append("       EMAIL, ");                   
            sql.Append("       FAX, ");                     
            sql.Append("       RM, ");                      
            sql.Append("       INITL_UNCLAMT, ");           
            sql.Append("       INITL_NPYAMT, ");            
            sql.Append("       UNCLAMT, ");                 
            sql.Append("       NPYAMT, ");                  
            sql.Append("       BCNC_TYPE, ");              
            sql.Append("       BANK, ");                   
            sql.Append("       ACCOUNT, ");                 
            sql.Append("       DEPOSITOR,  ");               
            sql.Append("       CUST_GROUP  ");           
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @CustNo, ");                 // Customer No.
            sql.Append("       @CustTin, ");                // Customer Taxpayer Identification Number(TIN)
            sql.Append("       @CustBhfId, ");              // Customer Branch ID
            sql.Append("       @CustNid, ");                // Customer National Idetification
            sql.Append("       @CustNm, ");                 // Customer Name
            sql.Append("       @TelNo, ");                  // Telephone Number
            sql.Append("       @Adrs, ");                   // Address
            sql.Append("       @UseYn, ");                  // Use(Y/N)
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @RegDt, ");                  // Registered Date
            sql.Append("       @ModrId, ");                 // Modifier ID
            sql.Append("       @ModrNm, ");                 // Modifier Name
            sql.Append("       @ModDt, ");                  // Modified Date
            sql.Append("       @NationCd, ");              
            sql.Append("       @ChargerNm, ");              
            sql.Append("       @Contact1, ");               
            sql.Append("       @Contact2, ");               
            sql.Append("       @Email, ");                  
            sql.Append("       @Fax, ");                    
            sql.Append("       @Rm, ");                    
            sql.Append("       @InitlUnclamt, ");          
            sql.Append("       @InitlNpyamt, ");            
            sql.Append("       @Unclamt, ");                
            sql.Append("       @Npyamt, ");                 
            sql.Append("       @BcncType, ");               
            sql.Append("       @Bank, ");                  
            sql.Append("       @Account, ");               
            sql.Append("       @Depositor,  ");           
            sql.Append("       @CustGroup  ");            
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_BHF_CUST "); // 납세자 지점 고객
            sql.Append("   set CUST_TIN = @CustTin, ");     // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID = @CustBhfId, ");  // Customer Branch ID
            sql.Append("       CUST_NID = @CustNid, ");     // Customer National Idetification
            sql.Append("       CUST_NM = @CustNm, ");       // Customer Name
            sql.Append("       TEL_NO = @TelNo, ");         // Telephone Number
            sql.Append("       ADRS = @Adrs, ");            // Address
            sql.Append("       USE_YN = @UseYn, ");         // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt, ");         // Modified Date
            sql.Append("       NATION_CD = @NationCd, ");   
            sql.Append("       CHARGER_NM = @ChargerNm, ");  
            sql.Append("       CONTACT1 = @Contact1, ");    
            sql.Append("       CONTACT2 = @Contact2, "); 
            sql.Append("       EMAIL = @Email, ");         
            sql.Append("       FAX = @Fax, ");              
            sql.Append("       RM = @Rm, ");                
            sql.Append("       INITL_UNCLAMT = @InitlUnclamt, ");  
            sql.Append("       INITL_NPYAMT = @InitlNpyamt, "); 
            sql.Append("       UNCLAMT = @Unclamt, ");    
            sql.Append("       NPYAMT = @Npyamt, ");        
            sql.Append("       BCNC_TYPE = @BcncType, ");  
            sql.Append("       BANK = @Bank, ");            
            sql.Append("       ACCOUNT = @Account, ");     
            sql.Append("       DEPOSITOR = @Depositor,  ");  
            sql.Append("       CUST_GROUP = @CustGroup  "); 
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and CUST_NO = @CustNo ");  // Customer No.
            return sql.ToString();

        }
        public string GetDeleteUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_BHF_CUST ");       
            sql.Append("   set USE_YN = 'N', ");            // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");               // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");          // Branch Office ID
            sql.Append("   and CUST_NO = @CustNo ");        // Customer No.
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TAXPAYER_BHF_CUST "); 
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and CUST_NO = @CustNo ");  // Customer No.
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerBhfCustRecord record)
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
            param.ParameterName = "@CustNo";
            param.Value = record.CustNo;
            command.Parameters.Add(param);                  // Customer No.

            param = command.CreateParameter();
            param.ParameterName = "@CustTin";
            param.Value = record.CustTin;
            command.Parameters.Add(param);                  // Customer Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@CustBhfId";
            param.Value = record.CustBhfId;
            command.Parameters.Add(param);                  // Customer Branch ID

            param = command.CreateParameter();
            param.ParameterName = "@CustNid";
            param.Value = record.CustNid;
            command.Parameters.Add(param);                  // Customer National Idetification

            param = command.CreateParameter();
            param.ParameterName = "@CustNm";
            param.Value = record.CustNm;
            command.Parameters.Add(param);                  // Customer Name

            param = command.CreateParameter();
            param.ParameterName = "@TelNo";
            param.Value = record.TelNo;
            command.Parameters.Add(param);                  // Telephone Number

            param = command.CreateParameter();
            param.ParameterName = "@Adrs";
            param.Value = record.Adrs;
            command.Parameters.Add(param);                  // Address

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
            command.Parameters.Add(param);                  // Registered Date

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
            param.ParameterName = "@NationCd";
            param.Value = record.NationCd;
            command.Parameters.Add(param);                 

            param = command.CreateParameter();
            param.ParameterName = "@ChargerNm";
            param.Value = record.ChargerNm;
            command.Parameters.Add(param);                  

            param = command.CreateParameter();
            param.ParameterName = "@Contact1";
            param.Value = record.Contact1;
            command.Parameters.Add(param);                 

            param = command.CreateParameter();
            param.ParameterName = "@Contact2";
            param.Value = record.Contact2;
            command.Parameters.Add(param);                 

            param = command.CreateParameter();
            param.ParameterName = "@Email";
            param.Value = record.Email;
            command.Parameters.Add(param);                

            param = command.CreateParameter();
            param.ParameterName = "@Fax";
            param.Value = record.Fax;
            command.Parameters.Add(param);                  

            param = command.CreateParameter();
            param.ParameterName = "@Rm";
            param.Value = record.Rm;
            command.Parameters.Add(param);                 

            param = command.CreateParameter();
            param.ParameterName = "@InitlUnclamt";
            param.Value = record.InitlUnclamt;
            command.Parameters.Add(param);                  

            param = command.CreateParameter();
            param.ParameterName = "@InitlNpyamt";
            param.Value = record.InitlNpyamt;
            command.Parameters.Add(param);                 

            param = command.CreateParameter();
            param.ParameterName = "@Unclamt";
            param.Value = record.Unclamt;
            command.Parameters.Add(param);                 

            param = command.CreateParameter();
            param.ParameterName = "@Npyamt";
            param.Value = record.Npyamt;
            command.Parameters.Add(param);                

            param = command.CreateParameter();
            param.ParameterName = "@BcncType";
            param.Value = record.BcncType;
            command.Parameters.Add(param);               

            param = command.CreateParameter();
            param.ParameterName = "@Bank";
            param.Value = record.Bank;
            command.Parameters.Add(param);                

            param = command.CreateParameter();
            param.ParameterName = "@Account";
            param.Value = record.Account;
            command.Parameters.Add(param);                  

            param = command.CreateParameter();
            param.ParameterName = "@Depositor";
            param.Value = record.Depositor;
            command.Parameters.Add(param);                  

            param = command.CreateParameter();
            param.ParameterName = "@CustGroup";
            param.Value = record.CustGroup;
            command.Parameters.Add(param);                 

            return true;
        }
    }
}

using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of StockIoTable.
    /// </summary>
    public class StockIoTable
    {
        public StockIoTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists STOCK_IO ( "); 
            sql.Append("       TIN                CHAR(9)        not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)        not null , ");      // Branch Office ID
            sql.Append("       SAR_NO             INT            not null , ");      // Stored and Released No.
            sql.Append("       ORG_SAR_NO         INT            null , ");          // Original Stored and Released No.
            sql.Append("       REG_TY_CD          VARCHAR(5)     null , ");          // Registration Type Code
            sql.Append("       TAXPR_NM           VARCHAR(60)    null , ");          // Taxpayer's Name
            sql.Append("       CUST_TIN           CHAR(9)        null , ");          // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID        CHAR(2)        null , ");          // Customer Branch ID
            sql.Append("       CUST_NM            VARCHAR(60)    null , ");          // Customer Name
            //JCNA 202001 DELETE sql.Append("       INVC_NO            INT            null , ");          // Onvoice No.
            sql.Append("       SAR_TY_CD          VARCHAR(5)     null , ");          // Stored and Released Type Code
            //JCNA 202001 DELETE sql.Append("       SAR_RSN_CD         VARCHAR(5)     null , ");          // Stored and Released Reason Code
            sql.Append("       OCRN_DT            CHAR(8)        null , ");          // Occurred Date time
            sql.Append("       TOT_ITEM_CNT       INT            null , ");          // Total Item Count
            sql.Append("       TOT_TAXBL_AMT      DECIMAL(18,2)  null , ");          // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT        DECIMAL(18,2)  null , ");          // Total Tax Amount
            sql.Append("       TOT_AMT            DECIMAL(18,2)  null , ");          // Total Amount
            sql.Append("       REMARK             VARCHAR(400)   null , ");          // Remark
            sql.Append("       REGR_ID            VARCHAR(20)    null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)    null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)    null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)    null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)    null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)    null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID, SAR_NO ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       SAR_NO, ");                  // Stored and Released No.
            sql.Append("       ORG_SAR_NO, ");              // Original Stored and Released No.
            sql.Append("       REG_TY_CD, ");               // Registration Type Code
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            sql.Append("       CUST_TIN, ");                // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID, ");             // Customer Branch ID
            sql.Append("       CUST_NM, ");                 // Customer Name
            //JCNA 202001 DELETE sql.Append("       INVC_NO, ");                 // Onvoice No.
            sql.Append("       SAR_TY_CD, ");               // Stored and Released Type Code
            //JCNA 202001 DELETE sql.Append("       SAR_RSN_CD, ");              // Stored and Released Reason Code
            sql.Append("       OCRN_DT, ");                 // Occurred Date time
            sql.Append("       TOT_ITEM_CNT, ");            // Total Item Count
            sql.Append("       TOT_TAXBL_AMT, ");           // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT, ");             // Total Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from STOCK_IO ");
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into STOCK_IO ( ");
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       SAR_NO, ");                  // Stored and Released No.
            sql.Append("       ORG_SAR_NO, ");              // Original Stored and Released No.
            sql.Append("       REG_TY_CD, ");               // Registration Type Code
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            sql.Append("       CUST_TIN, ");                // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID, ");             // Customer Branch ID
            sql.Append("       CUST_NM, ");                 // Customer Name
            //JCNA 202001 DELETE sql.Append("       INVC_NO, ");                 // Onvoice No.
            sql.Append("       SAR_TY_CD, ");               // Stored and Released Type Code
            //JCNA 202001 DELETE sql.Append("       SAR_RSN_CD, ");              // Stored and Released Reason Code
            sql.Append("       OCRN_DT, ");                 // Occurred Date time
            sql.Append("       TOT_ITEM_CNT, ");            // Total Item Count
            sql.Append("       TOT_TAXBL_AMT, ");           // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT, ");             // Total Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @SarNo, ");                  // Stored and Released No.
            sql.Append("       @OrgSarNo, ");               // Original Stored and Released No.
            sql.Append("       @RegTyCd, ");                // Registration Type Code
            sql.Append("       @TaxprNm, ");                // Taxpayer's Name
            sql.Append("       @CustTin, ");                // Customer Taxpayer Identification Number(TIN)
            sql.Append("       @CustBhfId, ");              // Customer Branch ID
            sql.Append("       @CustNm, ");                 // Customer Name
            //JCNA 202001 DELETE sql.Append("       @InvcNo, ");                 // Onvoice No.
            sql.Append("       @SarTyCd, ");                // Stored and Released Type Code
            //JCNA 202001 DELETE sql.Append("       @SarRsnCd, ");               // Stored and Released Reason Code
            sql.Append("       @OcrnDt, ");                 // Occurred Date time
            sql.Append("       @TotItemCnt, ");             // Total Item Count
            sql.Append("       @TotTaxblAmt, ");            // Total Taxable Amount
            sql.Append("       @TotTaxAmt, ");              // Total Tax Amount
            sql.Append("       @TotAmt, ");                 // Total Amount
            sql.Append("       @Remark, ");                 // Remark
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @RegDt, ");                  // Registered Date
            sql.Append("       @ModrId, ");                 // Modifier ID
            sql.Append("       @ModrNm, ");                 // Modifier Name
            sql.Append("       @ModDt  ");                  // Modified Date
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update STOCK_IO "); 
            sql.Append("   set ORG_SAR_NO = @OrgSarNo, ");  // Original Stored and Released No.
            sql.Append("       REG_TY_CD = @RegTyCd, ");    // Registration Type Code
            sql.Append("       TAXPR_NM = @TaxprNm, ");     // Taxpayer's Name
            sql.Append("       CUST_TIN = @CustTin, ");     // Customer Taxpayer Identification Number(TIN)
            sql.Append("       CUST_BHF_ID = @CustBhfId, ");  // Customer Branch ID
            sql.Append("       CUST_NM = @CustNm, ");       // Customer Name
            //JCNA 202001 DELETE sql.Append("       INVC_NO = @InvcNo, ");       // Onvoice No.
            sql.Append("       SAR_TY_CD = @SarTyCd, ");    // Stored and Released Type Code
            //JCNA 202001 DELETE sql.Append("       SAR_RSN_CD = @SarRsnCd, ");  // Stored and Released Reason Code
            sql.Append("       OCRN_DT = @OcrnDt, ");       // Occurred Date time
            sql.Append("       TOT_ITEM_CNT = @TotItemCnt, ");  // Total Item Count
            sql.Append("       TOT_TAXBL_AMT = @TotTaxblAmt, ");  // Total Taxable Amount
            sql.Append("       TOT_TAX_AMT = @TotTaxAmt, ");  // Total Tax Amount
            sql.Append("       TOT_AMT = @TotAmt, ");       // Total Amount
            sql.Append("       REMARK = @Remark, ");        // Remark
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and SAR_NO = @SarNo ");    // Stored and Released No.
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from STOCK_IO "); 
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and SAR_NO = @SarNo ");    // Stored and Released No.
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, StockIoRecord record)
        {
            record.UpdateNull();

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
            param.ParameterName = "@SarNo";
            param.Value = record.SarNo;
            command.Parameters.Add(param);                  // Stored and Released No.

            param = command.CreateParameter();
            param.ParameterName = "@OrgSarNo";
            param.Value = record.OrgSarNo;
            command.Parameters.Add(param);                  // Original Stored and Released No.

            param = command.CreateParameter();
            param.ParameterName = "@RegTyCd";
            param.Value = record.RegTyCd;
            command.Parameters.Add(param);                  // Registration Type Code

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = record.TaxprNm;
            command.Parameters.Add(param);                  // Taxpayer's Name

            param = command.CreateParameter();
            param.ParameterName = "@CustTin";
            param.Value = record.CustTin;
            command.Parameters.Add(param);                  // Customer Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@CustBhfId";
            param.Value = record.CustBhfId;
            command.Parameters.Add(param);                  // Customer Branch ID

            param = command.CreateParameter();
            param.ParameterName = "@CustNm";
            param.Value = record.CustNm;
            command.Parameters.Add(param);                  // Customer Name

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@InvcNo";
            //param.Value = record.InvcNo;
            //command.Parameters.Add(param);                  // Onvoice No.

            param = command.CreateParameter();
            param.ParameterName = "@SarTyCd";
            param.Value = record.SarTyCd;
            command.Parameters.Add(param);                  // Stored and Released Type Code

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@SarRsnCd";
            //param.Value = record.SarRsnCd;
            //command.Parameters.Add(param);                  // Stored and Released Reason Code

            param = command.CreateParameter();
            param.ParameterName = "@OcrnDt";
            param.Value = record.OcrnDt;
            command.Parameters.Add(param);                  // Occurred Date time

            param = command.CreateParameter();
            param.ParameterName = "@TotItemCnt";
            param.Value = record.TotItemCnt;
            command.Parameters.Add(param);                  // Total Item Count

            param = command.CreateParameter();
            param.ParameterName = "@TotTaxblAmt";
            param.Value = record.TotTaxblAmt;
            command.Parameters.Add(param);                  // Total Taxable Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotTaxAmt";
            param.Value = record.TotTaxAmt;
            command.Parameters.Add(param);                  // Total Tax Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotAmt";
            param.Value = record.TotAmt;
            command.Parameters.Add(param);                  // Total Amount

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = record.Remark;
            command.Parameters.Add(param);                  // Remark

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

            return true;
        }
    }
}
/*
insert into STOCK_IO ( 
       TIN, 
       BHF_ID, 
       SAR_NO, 
       ORG_SAR_NO, 
       REG_TY_CD, 
       TAXPR_NM, 
       CUST_TIN, 
       CUST_BHF_ID, 
       CUST_NM, 
       INVC_NO, 
       SAR_TY_CD, 
       SAR_RSN_CD, 
       OCRN_DT, 
       TOT_ITEM_CNT, 
       TOT_TAXBL_AMT, 
       TOT_TAX_AMT, 
       TOT_AMT, 
       REMARK, 
       REGR_ID, 
       REGR_NM, 
       REG_DT, 
       MODR_ID, 
       MODR_NM, 
       MOD_DT 
) 					
select     
       '000000000' as TIN, 
       BHF_ID as BHF_ID, 
       STC_ID as SAR_NO, 
       '' as ORG_SAR_NO, 
       REG_TY_CD as REG_TY_CD, 
       BCNC_ID as TAXPR_NM, 
       BCNC_ID as CUST_TIN, 
       '00' as CUST_BHF_ID, 
       BCNC_ID as CUST_NM, 
       '' as INVC_NO, 
       STC_TY_CD as SAR_TY_CD, 
       '' as SAR_RSN_CD, 
       OCDE as OCRN_DT, 
       TOT_NUM_ITEM as TOT_ITEM_CNT, 
       TOT_SPLPC as TOT_TAXBL_AMT, 
       TOT_VAT_AMT as TOT_TAX_AMT, 
       TOT_AMT as TOT_AMT, 
       RM as REMARK, 
       REGUSR_ID as REGR_ID, 
       REGUSR_ID as REGR_NM, 
       REG_DT as REG_DT, 
       REGUSR_ID as MODR_ID, 
       REGUSR_ID as MODR_NM, 
       REG_DT as MOD_DT 
  from STCWHIO 
 */

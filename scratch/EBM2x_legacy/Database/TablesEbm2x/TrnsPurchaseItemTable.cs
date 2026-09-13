using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TrnsPurchaseItemTable.
    /// </summary>
    public class TrnsPurchaseItemTable
    {
        public TrnsPurchaseItemTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TRNS_PURCHASE_ITEM ( "); // 거래 구매 품목
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       INVC_NO            INT                not null , ");      // Invoice No.
            sql.Append("       SPPLR_TIN          CHAR(9)            not null , ");      // Supplier Taxpayer Identification Number(TIN)
            sql.Append("       ITEM_SEQ           INT                not null , ");      // Item Sequence
            sql.Append("       ITEM_CD            VARCHAR(20)        null , ");          // Item Code
            sql.Append("       ITEM_CLS_CD        VARCHAR(10)        null , ");          // Item Classification Code
            sql.Append("       ITEM_NM            VARCHAR(200)       null , ");          // Item Name
            sql.Append("       BCD                VARCHAR(20)        null , ");          // Barcode
            sql.Append("       SPPLR_ITEM_CLS_CD  CHAR(10)           null , ");          // Supplier Item Classification Code
            sql.Append("       SPPLR_ITEM_CD      VARCHAR(20)        null , ");          // Supplier Item Code
            sql.Append("       SPPLR_ITEM_NM      VARCHAR(200)       null , ");          // Supplier Item Name
            sql.Append("       PKG_UNIT_CD        VARCHAR(5)         null , ");          // Package Unit Code
            sql.Append("       PKG                DECIMAL(13,2)      null , ");          // Package
            sql.Append("       QTY_UNIT_CD        VARCHAR(5)         null , ");          // Quantity Unit Code
            sql.Append("       QTY                DECIMAL(13,2)      null , ");          // Quantity
            sql.Append("       PRC                DECIMAL(13,2)      null , ");          // Price
            sql.Append("       SPLY_AMT           DECIMAL(18,2)      null , ");          // Supply Amount
            sql.Append("       DC_RT              INT                null , ");          // Discount Rate
            sql.Append("       DC_AMT             DECIMAL(18,2)      null , ");          // Discount Amount
            sql.Append("       TAXBL_AMT          DECIMAL(18,2)      null , ");          // Taxable Amount
            sql.Append("       TAX_TY_CD          VARCHAR(5)         null , ");          // Taxation Type Code
            sql.Append("       TAX_AMT            DECIMAL(18,2)      null , ");          // Tax Amount
            sql.Append("       TOT_AMT            DECIMAL(18,2)      null , ");          // Total Amount
            sql.Append("       ITEM_EXPR_DT       CHAR(8)            null , ");          // Item Expiration Date
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID, INVC_NO, SPPLR_TIN, ITEM_SEQ ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       SPPLR_TIN, ");               // Supplier Taxpayer Identification Number(TIN)
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       SPPLR_ITEM_CLS_CD, ");       // Supplier Item Classification Code
            sql.Append("       SPPLR_ITEM_CD, ");           // Supplier Item Code
            sql.Append("       SPPLR_ITEM_NM, ");           // Supplier Item Name
            sql.Append("       PKG_UNIT_CD, ");             // Package Unit Code
            sql.Append("       PKG, ");                     // Package
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       PRC, ");                     // Price
            sql.Append("       SPLY_AMT, ");                // Supply Amount
            sql.Append("       DC_RT, ");                   // Discount Rate
            sql.Append("       DC_AMT, ");                  // Discount Amount
            sql.Append("       TAXBL_AMT, ");               // Taxable Amount
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       TAX_AMT, ");                 // Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       ITEM_EXPR_DT, ");            // Item Expiration Date
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       A.rowid  ");                  // Modified Date
            sql.Append("  from TRNS_PURCHASE_ITEM A "); // 거래 구매 품목
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TRNS_PURCHASE_ITEM ( "); // 거래 구매 품목
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       SPPLR_TIN, ");               // Supplier Taxpayer Identification Number(TIN)
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       SPPLR_ITEM_CLS_CD, ");       // Supplier Item Classification Code
            sql.Append("       SPPLR_ITEM_CD, ");           // Supplier Item Code
            sql.Append("       SPPLR_ITEM_NM, ");           // Supplier Item Name
            sql.Append("       PKG_UNIT_CD, ");             // Package Unit Code
            sql.Append("       PKG, ");                     // Package
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       PRC, ");                     // Price
            sql.Append("       SPLY_AMT, ");                // Supply Amount
            sql.Append("       DC_RT, ");                   // Discount Rate
            sql.Append("       DC_AMT, ");                  // Discount Amount
            sql.Append("       TAXBL_AMT, ");               // Taxable Amount
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       TAX_AMT, ");                 // Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       ITEM_EXPR_DT, ");            // Item Expiration Date
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @BhfId, ");                  // Branch Office ID
            sql.Append("       @InvcNo, ");                 // Invoice No.
            sql.Append("       @SpplrTin, ");               // Supplier Taxpayer Identification Number(TIN)
            sql.Append("       @ItemSeq, ");                // Item Sequence
            sql.Append("       @ItemCd, ");                 // Item Code
            sql.Append("       @ItemClsCd, ");              // Item Classification Code
            sql.Append("       @ItemNm, ");                 // Item Name
            sql.Append("       @Bcd, ");                    // Barcode
            sql.Append("       @SpplrItemClsCd, ");         // Supplier Item Classification Code
            sql.Append("       @SpplrItemCd, ");            // Supplier Item Code
            sql.Append("       @SpplrItemNm, ");            // Supplier Item Name
            sql.Append("       @PkgUnitCd, ");              // Package Unit Code
            sql.Append("       @Pkg, ");                    // Package
            sql.Append("       @QtyUnitCd, ");              // Quantity Unit Code
            sql.Append("       @Qty, ");                    // Quantity
            sql.Append("       @Prc, ");                    // Price
            sql.Append("       @SplyAmt, ");                // Supply Amount
            sql.Append("       @DcRt, ");                   // Discount Rate
            sql.Append("       @DcAmt, ");                  // Discount Amount
            sql.Append("       @TaxblAmt, ");               // Taxable Amount
            sql.Append("       @TaxTyCd, ");                // Taxation Type Code
            sql.Append("       @TaxAmt, ");                 // Tax Amount
            sql.Append("       @TotAmt, ");                 // Total Amount
            sql.Append("       @ItemExprDt, ");             // Item Expiration Date
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
            sql.Append("update TRNS_PURCHASE_ITEM "); // 거래 구매 품목
            sql.Append("   set ITEM_CD = @ItemCd, ");       // Item Code
            sql.Append("       ITEM_CLS_CD = @ItemClsCd, ");  // Item Classification Code
            sql.Append("       ITEM_NM = @ItemNm, ");         // Item Name
            sql.Append("       BCD = @Bcd, ");                // Barcode
            sql.Append("       SPPLR_ITEM_CLS_CD = @SpplrItemClsCd, ");  // Supplier Item Classification Code
            sql.Append("       SPPLR_ITEM_CD = @SpplrItemCd, ");  // Supplier Item Code
            sql.Append("       SPPLR_ITEM_NM = @SpplrItemNm, ");  // Supplier Item Name
            sql.Append("       PKG_UNIT_CD = @PkgUnitCd, ");      // Package Unit Code
            sql.Append("       PKG = @Pkg, ");                    // Package
            sql.Append("       QTY_UNIT_CD = @QtyUnitCd, ");      // Quantity Unit Code
            sql.Append("       QTY = @Qty, ");              // Quantity
            sql.Append("       PRC = @Prc, ");              // Price
            sql.Append("       SPLY_AMT = @SplyAmt, ");     // Supply Amount
            sql.Append("       DC_RT = @DcRt, ");           // Discount Rate
            sql.Append("       DC_AMT = @DcAmt, ");         // Discount Amount
            sql.Append("       TAXBL_AMT = @TaxblAmt, ");   // Taxable Amount
            sql.Append("       TAX_TY_CD = @TaxTyCd, ");    // Taxation Type Code
            sql.Append("       TAX_AMT = @TaxAmt, ");       // Tax Amount
            sql.Append("       TOT_AMT = @TotAmt, ");       // Total Amount
            sql.Append("       ITEM_EXPR_DT = @ItemExprDt, ");  // Item Expiration Date
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            sql.Append("   and SPPLR_TIN = @SpplrTin ");  // Supplier Taxpayer Identification Number(TIN)
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TRNS_PURCHASE_ITEM "); // 거래 구매 품목
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            sql.Append("   and SPPLR_TIN = @SpplrTin ");  // Supplier Taxpayer Identification Number(TIN)
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            return sql.ToString();
        }

        // JINIT_20191208, 전표자체를 삭제하는 SQL 추가
        public string GetInvoiceNoDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TRNS_PURCHASE_ITEM "); // 거래 구매 품목
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            sql.Append("   and SPPLR_TIN = @SpplrTin ");  // Supplier Taxpayer Identification Number(TIN)
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TrnsPurchaseItemRecord record)
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
            param.ParameterName = "@InvcNo";
            param.Value = record.InvcNo;
            command.Parameters.Add(param);                  // Invoice No.

            param = command.CreateParameter();
            param.ParameterName = "@SpplrTin";
            param.Value = record.SpplrTin;
            command.Parameters.Add(param);                  // Supplier Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@ItemSeq";
            param.Value = record.ItemSeq;
            command.Parameters.Add(param);                  // Item Sequence

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = record.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = record.ItemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemNm";
            param.Value = record.ItemNm;
            command.Parameters.Add(param);                  // Item Name

            param = command.CreateParameter();
            param.ParameterName = "@Bcd";
            param.Value = record.Bcd;
            command.Parameters.Add(param);                  // Barcode

            param = command.CreateParameter();
            param.ParameterName = "@SpplrItemClsCd";
            param.Value = record.SpplrItemClsCd;
            command.Parameters.Add(param);                  // Supplier Item Classification Code

            param = command.CreateParameter();
            param.ParameterName = "@SpplrItemCd";
            param.Value = record.SpplrItemCd;
            command.Parameters.Add(param);                  // Supplier Item Code

            param = command.CreateParameter();
            param.ParameterName = "@SpplrItemNm";
            param.Value = record.SpplrItemNm;
            command.Parameters.Add(param);                  // Supplier Item Name

            param = command.CreateParameter();
            param.ParameterName = "@PkgUnitCd";
            param.Value = record.PkgUnitCd;
            command.Parameters.Add(param);                  // Package Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@Pkg";
            param.Value = record.Pkg;
            command.Parameters.Add(param);                  // Package

            param = command.CreateParameter();
            param.ParameterName = "@QtyUnitCd";
            param.Value = record.QtyUnitCd;
            command.Parameters.Add(param);                  // Quantity Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@Qty";
            param.Value = record.Qty;
            command.Parameters.Add(param);                  // Quantity

            param = command.CreateParameter();
            param.ParameterName = "@Prc";
            param.Value = record.Prc;
            command.Parameters.Add(param);                  // Price

            param = command.CreateParameter();
            param.ParameterName = "@SplyAmt";
            param.Value = record.SplyAmt;
            command.Parameters.Add(param);                  // Supply Amount

            param = command.CreateParameter();
            param.ParameterName = "@DcRt";
            param.Value = record.DcRt;
            command.Parameters.Add(param);                  // Discount Rate

            param = command.CreateParameter();
            param.ParameterName = "@DcAmt";
            param.Value = record.DcAmt;
            command.Parameters.Add(param);                  // Discount Amount

            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmt";
            param.Value = record.TaxblAmt;
            command.Parameters.Add(param);                  // Taxable Amount

            param = command.CreateParameter();
            param.ParameterName = "@TaxTyCd";
            param.Value = record.TaxTyCd;
            command.Parameters.Add(param);                  // Taxation Type Code

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmt";
            param.Value = record.TaxAmt;
            command.Parameters.Add(param);                  // Tax Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotAmt";
            param.Value = record.TotAmt;
            command.Parameters.Add(param);                  // Total Amount

            param = command.CreateParameter();
            param.ParameterName = "@ItemExprDt";
            param.Value = record.ItemExprDt;
            command.Parameters.Add(param);                  // Item Expiration Date

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
insert into TRNS_PURCHASE_ITEM ( 
       TIN, 
       BHF_ID, 
       INVC_NO, 
       SPPLR_TIN, 
       ITEM_SEQ, 
       ITEM_CD, 
       ITEM_CLS_CD, 
       ITEM_NM, 
       BCD, 
       SPPLR_ITEM_CLS_CD, 
       SPPLR_ITEM_CD, 
       SPPLR_ITEM_NM, 
       PKG_UNIT_CD, 
       PKG, 
       QTY_UNIT_CD, 
       QTY, 
       PRC, 
       SPLY_AMT, 
       DC_RT, 
       DC_AMT, 
       TAXBL_AMT, 
       TAX_TY_CD, 
       TAX_AMT, 
       TOT_AMT, 
       ITEM_EXPR_DT, 
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
       INV_ID as INVC_NO, 
       BCNC_ID as SPPLR_TIN, 
       ITEM_SEQ as ITEM_SEQ, 
       ITEM_CD as ITEM_CD, 
       ITEM_CLS_CD as ITEM_CLS_CD, 
       ITEM_NM as ITEM_NM, 
       '' as BCD, 
       BCNC_ITEM_CLS_CD as SPPLR_ITEM_CLS_CD, 
       BCNC_ITEM_CD as SPPLR_ITEM_CD, 
       BCNC_ITEM_NM as SPPLR_ITEM_NM, 
       PKG_UNIT_CD as PKG_UNIT_CD, 
       PKG_QTY as PKG, 
       QTY_UNIT_CD as QTY_UNIT_CD, 
       QTY as QTY, 
       UNTPC as PRC, 
       SPLPC as SPLY_AMT, 
       DC_RATE as DC_RT, 
       DC_AMT as DC_AMT, 
       TAXABL_AMT as TAXBL_AMT, 
       TAX_TY_CD as TAX_TY_CD, 
       TAX as TAX_AMT, 
       TOT_AMT as TOT_AMT, 
       EXPIR_DT as ITEM_EXPR_DT, 
       '' as REGR_ID, 
       '' as REGR_NM, 
       SND_DT as REG_DT, 
       '' as MODR_ID, 
       '' as MODR_NM, 
       RCV_DT as MOD_DT
  from TRNPURCHASEITEM
 */

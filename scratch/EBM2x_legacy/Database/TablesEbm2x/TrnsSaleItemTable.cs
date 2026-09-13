using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TrnsSaleItemTable.
    /// </summary>
    public class TrnsSaleItemTable
    {
        public TrnsSaleItemTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TRNS_SALE_ITEM ( "); // 거래 판매 품목
            sql.Append("       TIN                CHAR(9)             not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)             not null , ");      // Branch Office ID
            sql.Append("       INVC_NO            INT                 not null , ");      // Invoice No.
            sql.Append("       ITEM_SEQ           INT                 not null , ");      // Item Sequence
            sql.Append("       ITEM_CD            VARCHAR(20)         null , ");          // Item Code
            sql.Append("       ISRCC_CD           VARCHAR(10)         null , ");          // Insurance Company Code
            sql.Append("       ITEM_CLS_CD        VARCHAR(10)         null , ");          // Item Classification Code
            sql.Append("       ITEM_NM            VARCHAR(200)        null , ");          // Item Name
            sql.Append("       BCD                VARCHAR(20)         null , ");          // Barcode
            sql.Append("       PKG_UNIT_CD        VARCHAR(5)          null , ");          // Package Unit Code
            sql.Append("       PKG                DECIMAL(13,2)       null , ");          // Package
            sql.Append("       QTY_UNIT_CD        VARCHAR(5)          null , ");          // Quantity Unit Code
            sql.Append("       QTY                DECIMAL(13,2)       null , ");          // Quantity
            sql.Append("       PRC                DECIMAL(13,2)       null , ");          // Unit Price
            sql.Append("       SPLY_AMT           DECIMAL(18,2)       null , ");          // Supply Price
            sql.Append("       DC_RT              INT                 null , ");          // Discount Rate
            sql.Append("       DC_AMT             DECIMAL(18,2)       null , ");          // Discount Amount
            sql.Append("       ISRCC_NM           VARCHAR(100)        null , ");          // Insurance Company Name
            sql.Append("       ISRC_RT            INT                 null , ");          // Insurance Rate
            sql.Append("       ISRC_AMT           DECIMAL(18,2)       null , ");          // Insurance Amount
            sql.Append("       TAX_TY_CD          VARCHAR(5)          null , ");          // Tax type code
            sql.Append("       TAXBL_AMT          DECIMAL(18,2)       null , ");          // Taxable Amount
            sql.Append("       TAX_AMT            DECIMAL(18,2)       null , ");          // Tax Amount
            sql.Append("       TOT_AMT            DECIMAL(18,2)       null , ");          // Total Amount
            sql.Append("       REGR_ID            VARCHAR(20)         null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)         null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)         null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)         null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)         null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)         null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID, INVC_NO, ITEM_SEQ ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ISRCC_CD, ");                // Insurance Company Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       PKG_UNIT_CD, ");             // Package Unit Code
            sql.Append("       PKG, ");                     // Package
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       PRC, ");                     // Unit Price
            sql.Append("       SPLY_AMT, ");                // Supply Price
            sql.Append("       DC_RT, ");                   // Discount Rate
            sql.Append("       DC_AMT, ");                  // Discount Amount
            sql.Append("       ISRCC_NM, ");                // Insurance Company Name
            sql.Append("       ISRC_RT, ");                 // Insurance Rate
            sql.Append("       ISRC_AMT, ");                // Insurance Amount
            sql.Append("       TAX_TY_CD, ");               // Tax type code
            sql.Append("       TAXBL_AMT, ");               // Taxable Amount
            sql.Append("       TAX_AMT, ");                 // Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT ");                  // Modified Date
            sql.Append("  from TRNS_SALE_ITEM A "); // 거래 판매 품목
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TRNS_SALE_ITEM ( "); // 거래 판매 품목
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       INVC_NO, ");                 // Invoice No.
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ISRCC_CD, ");                // Insurance Company Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       PKG_UNIT_CD, ");             // Package Unit Code
            sql.Append("       PKG, ");                     // Package
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       PRC, ");                     // Unit Price
            sql.Append("       SPLY_AMT, ");                // Supply Price
            sql.Append("       DC_RT, ");                   // Discount Rate
            sql.Append("       DC_AMT, ");                  // Discount Amount
            sql.Append("       ISRCC_NM, ");                // Insurance Company Name
            sql.Append("       ISRC_RT, ");                 // Insurance Rate
            sql.Append("       ISRC_AMT, ");                // Insurance Amount
            sql.Append("       TAX_TY_CD, ");               // Tax type code
            sql.Append("       TAXBL_AMT, ");               // Taxable Amount
            sql.Append("       TAX_AMT, ");                 // Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
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
            sql.Append("       @ItemSeq, ");                // Item Sequence
            sql.Append("       @ItemCd, ");                 // Item Code
            sql.Append("       @IsrccCd, ");                // Insurance Company Code
            sql.Append("       @ItemClsCd, ");              // Item Classification Code
            sql.Append("       @ItemNm, ");                 // Item Name
            sql.Append("       @Bcd, ");                    // Barcode
            sql.Append("       @PkgUnitCd, ");              // Package Unit Code
            sql.Append("       @Pkg, ");                    // Package
            sql.Append("       @QtyUnitCd, ");              // Quantity Unit Code
            sql.Append("       @Qty, ");                    // Quantity
            sql.Append("       @Prc, ");                    // Unit Price
            sql.Append("       @SplyAmt, ");                // Supply Price
            sql.Append("       @DcRt, ");                   // Discount Rate
            sql.Append("       @DcAmt, ");                  // Discount Amount
            sql.Append("       @IsrccNm, ");                // Insurance Company Name
            sql.Append("       @IsrcRt, ");                 // Insurance Rate
            sql.Append("       @IsrcAmt, ");                // Insurance Amount
            sql.Append("       @TaxTyCd, ");                // Tax type code
            sql.Append("       @TaxblAmt, ");               // Taxable Amount
            sql.Append("       @TaxAmt, ");                 // Tax Amount
            sql.Append("       @TotAmt, ");                 // Total Amount
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
            sql.Append("update TRNS_SALE_ITEM "); // 거래 판매 품목
            sql.Append("   set ITEM_CD = @ItemCd, ");       // Item Code
            sql.Append("       ISRCC_CD = @IsrccCd, ");     // Insurance Company Code
            sql.Append("       ITEM_CLS_CD = @ItemClsCd, ");  // Item Classification Code
            sql.Append("       ITEM_NM = @ItemNm, ");       // Item Name
            sql.Append("       BCD = @Bcd, ");              // Barcode
            sql.Append("       PKG_UNIT_CD = @PkgUnitCd, ");  // Package Unit Code
            sql.Append("       PKG = @Pkg, ");              // Package
            sql.Append("       QTY_UNIT_CD = @QtyUnitCd, ");  // Quantity Unit Code
            sql.Append("       QTY = @Qty, ");              // Quantity
            sql.Append("       PRC = @Prc, ");              // Unit Price
            sql.Append("       SPLY_AMT = @SplyAmt, ");     // Supply Price
            sql.Append("       DC_RT = @DcRt, ");           // Discount Rate
            sql.Append("       DC_AMT = @DcAmt, ");         // Discount Amount
            sql.Append("       ISRCC_NM = @IsrccNm, ");     // Insurance Company Name
            sql.Append("       ISRC_RT = @IsrcRt, ");       // Insurance Rate
            sql.Append("       ISRC_AMT = @IsrcAmt, ");     // Insurance Amount
            sql.Append("       TAX_TY_CD = @TaxTyCd, ");    // Tax type code
            sql.Append("       TAXBL_AMT = @TaxblAmt, ");   // Taxable Amount
            sql.Append("       TAX_AMT = @TaxAmt, ");       // Tax Amount
            sql.Append("       TOT_AMT = @TotAmt, ");       // Total Amount
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TRNS_SALE_ITEM "); // 거래 판매 품목
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            return sql.ToString();
        }

        // JINIT_20191208, 전표자체를 삭제하는 SQL 추가
        public string GetInvoiceNoDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TRNS_SALE_ITEM "); // 거래 판매 품목
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and INVC_NO = @InvcNo ");  // Invoice No.
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TrnsSaleItemRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = string.IsNullOrEmpty(record.Tin) ? "" : record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@BhfId";
            param.Value = string.IsNullOrEmpty(record.BhfId) ? "" : record.BhfId;
            command.Parameters.Add(param);                  // Branch Office ID

            param = command.CreateParameter();
            param.ParameterName = "@InvcNo";
            param.Value = record.InvcNo;
            command.Parameters.Add(param);                  // Invoice No.

            param = command.CreateParameter();
            param.ParameterName = "@ItemSeq";
            param.Value = record.ItemSeq;
            command.Parameters.Add(param);                  // Item Sequence

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = string.IsNullOrEmpty(record.ItemCd) ? "" : record.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@IsrccCd";
            param.Value = string.IsNullOrEmpty(record.IsrccCd) ? "" : record.IsrccCd;
            command.Parameters.Add(param);                  // Insurance Company Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = string.IsNullOrEmpty(record.ItemClsCd) ? "" : record.ItemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemNm";
            param.Value = string.IsNullOrEmpty(record.ItemNm) ? "" : record.ItemNm;
            command.Parameters.Add(param);                  // Item Name

            param = command.CreateParameter();
            param.ParameterName = "@Bcd";
            param.Value = string.IsNullOrEmpty(record.Bcd) ? "" : record.Bcd;
            command.Parameters.Add(param);                  // Barcode

            param = command.CreateParameter();
            param.ParameterName = "@PkgUnitCd";
            param.Value = string.IsNullOrEmpty(record.PkgUnitCd) ? "" : record.PkgUnitCd;
            command.Parameters.Add(param);                  // Package Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@Pkg";
            param.Value = record.Pkg;
            command.Parameters.Add(param);                  // Package

            param = command.CreateParameter();
            param.ParameterName = "@QtyUnitCd";
            param.Value = string.IsNullOrEmpty(record.QtyUnitCd) ? "" : record.QtyUnitCd;
            command.Parameters.Add(param);                  // Quantity Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@Qty";
            param.Value = record.Qty;
            command.Parameters.Add(param);                  // Quantity

            param = command.CreateParameter();
            param.ParameterName = "@Prc";
            param.Value = record.Prc;
            command.Parameters.Add(param);                  // Unit Price

            param = command.CreateParameter();
            param.ParameterName = "@SplyAmt";
            param.Value = record.SplyAmt;
            command.Parameters.Add(param);                  // Supply Price

            param = command.CreateParameter();
            param.ParameterName = "@DcRt";
            param.Value = record.DcRt;
            command.Parameters.Add(param);                  // Discount Rate

            param = command.CreateParameter();
            param.ParameterName = "@DcAmt";
            param.Value = record.DcAmt;
            command.Parameters.Add(param);                  // Discount Amount

            param = command.CreateParameter();
            param.ParameterName = "@IsrccNm";
            param.Value = string.IsNullOrEmpty(record.IsrccNm) ? "" : record.IsrccNm;
            command.Parameters.Add(param);                  // Insurance Company Name

            param = command.CreateParameter();
            param.ParameterName = "@IsrcRt";
            param.Value = record.IsrcRt;
            command.Parameters.Add(param);                  // Insurance Rate

            param = command.CreateParameter();
            param.ParameterName = "@IsrcAmt";
            param.Value = record.IsrcAmt;
            command.Parameters.Add(param);                  // Insurance Amount

            param = command.CreateParameter();
            param.ParameterName = "@TaxTyCd";
            param.Value = record.TaxTyCd;
            command.Parameters.Add(param);                  // Tax type code

            param = command.CreateParameter();
            param.ParameterName = "@TaxblAmt";
            param.Value = record.TaxblAmt;
            command.Parameters.Add(param);                  // Taxable Amount

            param = command.CreateParameter();
            param.ParameterName = "@TaxAmt";
            param.Value = record.TaxAmt;
            command.Parameters.Add(param);                  // Tax Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotAmt";
            param.Value = record.TotAmt;
            command.Parameters.Add(param);                  // Total Amount

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = string.IsNullOrEmpty(record.RegrNm) ? "" : record.RegrNm;
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
            param.Value = string.IsNullOrEmpty(record.ModrNm) ? "" : record.ModrNm;
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

insert into TRNS_SALE_ITEM ( 
       TIN, 
       BHF_ID, 
       INVC_NO, 
       ITEM_SEQ, 
       ITEM_CD, 
       ISRCC_CD, 
       ITEM_CLS_CD, 
       ITEM_NM, 
       BCD, 
       PKG_UNIT_CD, 
       PKG, 
       QTY_UNIT_CD, 
       QTY, 
       PRC, 
       SPLY_AMT, 
       DC_RT, 
       DC_AMT, 
       ISRCC_NM, 
       ISRC_RT, 
       ISRC_AMT, 
       TAX_TY_CD, 
       TAXBL_AMT, 
       TAX_AMT, 
       TOT_AMT, 
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
       ITEM_SEQ as ITEM_SEQ, 
       ITEM_CD as ITEM_CD, 
       '' as ISRCC_CD, 
       ITEM_CLS_CD as ITEM_CLS_CD, 
       ITEM_NM as ITEM_NM, 
       '' as BCD, 
       PKG_UNIT_CD as PKG_UNIT_CD, 
       PKG_QTY as PKG, 
       QTY_UNIT_CD as QTY_UNIT_CD, 
       QTY as QTY, 
       UNTPC as PRC, 
       SPLPC as SPLY_AMT, 
       DC_RATE as DC_RT, 
       DC_AMT as DC_AMT, 
       '' as ISRCC_NM, 
       0 as ISRC_RT, 
       0 as ISRC_AMT, 
       TAX_TY_CD as TAX_TY_CD, 
       TAXABL_AMT as TAXBL_AMT, 
       TAX as TAX_AMT, 
       TOT_AMT as TOT_AMT, 
       '' as REGR_ID, 
       '' as REGR_NM, 
       SND_DT as REG_DT, 
       '' as MODR_ID, 
       '' as MODR_NM, 
       SND_DT as MOD_DT
  from TRNSALEITEM 

*/

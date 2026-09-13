using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of StockIoItemTable.
    /// </summary>
    public class StockIoItemTable
    {
        public StockIoItemTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists STOCK_IO_ITEM ( "); 
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID             CHAR(2)            not null , ");      // Branch Office ID
            sql.Append("       SAR_NO             INT                not null , ");      // Stored and Released No.
            sql.Append("       ITEM_SEQ           INT                not null , ");      // Item Sequence
            sql.Append("       ITEM_CD            VARCHAR(20)        null , ");          // Item Code
            sql.Append("       ITEM_CLS_CD        VARCHAR(10)        null , ");          // Item Classification Code
            sql.Append("       ITEM_NM            VARCHAR(200)       null , ");          // Item Name
            sql.Append("       BCD                VARCHAR(20)        null , ");          // Barcode
            sql.Append("       PKG_UNIT_CD        VARCHAR(5)         null , ");          // Package unit code
            sql.Append("       PKG                DECIMAL(13,2)      null , ");          // Package
            sql.Append("       QTY_UNIT_CD        VARCHAR(5)         null , ");          // Quantity Unit Code
            sql.Append("       QTY                DECIMAL(13,2)      null , ");          // Quantity
            sql.Append("       ITEM_EXPR_DT       CHAR(8)             null , ");          // Item Expiration Date
            sql.Append("       PRC                DECIMAL(13,2)      null , ");          // Price
            sql.Append("       SPLY_AMT           DECIMAL(18,2)      null , ");          // Supply Amount
            sql.Append("       TOT_DC_AMT         DECIMAL(18,2)      null , ");          // Total Discount Amount
            sql.Append("       TAXBL_AMT          DECIMAL(18,2)      null , ");          // Taxable Amount
            sql.Append("       TAX_TY_CD          VARCHAR(5)         null , ");          // Taxation Type Code
            sql.Append("       TAX_AMT            DECIMAL(18,2)      null , ");          // Tax Amount
            sql.Append("       TOT_AMT            DECIMAL(18,2)      null , ");          // Total Amount
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( TIN, BHF_ID, SAR_NO, ITEM_SEQ ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       SAR_NO, ");                  // Stored and Released No.
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       PKG_UNIT_CD, ");             // Package unit code
            sql.Append("       PKG, ");                     // Package
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       ITEM_EXPR_DT, ");            // Item Expiration Date
            sql.Append("       PRC, ");                     // Price
            sql.Append("       SPLY_AMT, ");                // Supply Amount
            sql.Append("       TOT_DC_AMT, ");              // Total Discount Amount
            sql.Append("       TAXBL_AMT, ");               // Taxable Amount
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       TAX_AMT, ");                 // Tax Amount
            sql.Append("       TOT_AMT, ");                 // Total Amount
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("  from STOCK_IO_ITEM "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into STOCK_IO_ITEM ( "); 
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       BHF_ID, ");                  // Branch Office ID
            sql.Append("       SAR_NO, ");                  // Stored and Released No.
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       PKG_UNIT_CD, ");             // Package unit code
            sql.Append("       PKG, ");                     // Package
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       ITEM_EXPR_DT, ");            // Item Expiration Date
            sql.Append("       PRC, ");                     // Price
            sql.Append("       SPLY_AMT, ");                // Supply Amount
            sql.Append("       TOT_DC_AMT, ");              // Total Discount Amount
            sql.Append("       TAXBL_AMT, ");               // Taxable Amount
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
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
            sql.Append("       @SarNo, ");                  // Stored and Released No.
            sql.Append("       @ItemSeq, ");                // Item Sequence
            sql.Append("       @ItemCd, ");                 // Item Code
            sql.Append("       @ItemClsCd, ");              // Item Classification Code
            sql.Append("       @ItemNm, ");                 // Item Name
            sql.Append("       @Bcd, ");                    // Barcode
            sql.Append("       @PkgUnitCd, ");              // Package unit code
            sql.Append("       @Pkg, ");                    // Package
            sql.Append("       @QtyUnitCd, ");              // Quantity Unit Code
            sql.Append("       @Qty, ");                    // Quantity
            sql.Append("       @ItemExprDt, ");             // Item Expiration Date
            sql.Append("       @Prc, ");                    // Price
            sql.Append("       @SplyAmt, ");                // Supply Amount
            sql.Append("       @TotDcAmt, ");               // Total Discount Amount
            sql.Append("       @TaxblAmt, ");               // Taxable Amount
            sql.Append("       @TaxTyCd, ");                // Taxation Type Code
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
            sql.Append("update STOCK_IO_ITEM "); 
            sql.Append("   set ITEM_CD = @ItemCd, ");       // Item Code
            sql.Append("       ITEM_CLS_CD = @ItemClsCd, ");  // Item Classification Code
            sql.Append("       ITEM_NM = @ItemNm, ");       // Item Name
            sql.Append("       BCD = @Bcd, ");              // Barcode
            sql.Append("       PKG_UNIT_CD = @PkgUnitCd, ");  // Package unit code
            sql.Append("       PKG = @Pkg, ");              // Package
            sql.Append("       QTY_UNIT_CD = @QtyUnitCd, ");  // Quantity Unit Code
            sql.Append("       QTY = @Qty, ");              // Quantity
            sql.Append("       ITEM_EXPR_DT = @ItemExprDt, ");  // Item Expiration Date
            sql.Append("       PRC = @Prc, ");              // Price
            sql.Append("       SPLY_AMT = @SplyAmt, ");     // Supply Amount
            sql.Append("       TOT_DC_AMT = @TotDcAmt, ");  // Total Discount Amount
            sql.Append("       TAXBL_AMT = @TaxblAmt, ");   // Taxable Amount
            sql.Append("       TAX_TY_CD = @TaxTyCd, ");    // Taxation Type Code
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
            sql.Append("   and SAR_NO = @SarNo ");    // Stored and Released No.
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from STOCK_IO_ITEM "); // 재고 입출고 품목
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and BHF_ID = @BhfId ");    // Branch Office ID
            sql.Append("   and SAR_NO = @SarNo ");    // Stored and Released No.
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, StockIoItemRecord record)
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
            param.ParameterName = "@PkgUnitCd";
            param.Value = record.PkgUnitCd;
            command.Parameters.Add(param);                  // Package unit code

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
            param.ParameterName = "@ItemExprDt";
            param.Value = record.ItemExprDt;
            command.Parameters.Add(param);                  // Item Expiration Date

            param = command.CreateParameter();
            param.ParameterName = "@Prc";
            param.Value = record.Prc;
            command.Parameters.Add(param);                  // Price

            param = command.CreateParameter();
            param.ParameterName = "@SplyAmt";
            param.Value = record.SplyAmt;
            command.Parameters.Add(param);                  // Supply Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotDcAmt";
            param.Value = record.TotDcAmt;
            command.Parameters.Add(param);                  // Total Discount Amount

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

        public bool SetParametersSDC(IDbCommand command, StockIoItemRecord record)
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
            param.ParameterName = "@PkgUnitCd";
            param.Value = record.PkgUnitCd;
            command.Parameters.Add(param);                  // Package unit code

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
            param.ParameterName = "@ItemExprDt";
            param.Value = record.ItemExprDt;
            command.Parameters.Add(param);                  // Item Expiration Date

            param = command.CreateParameter();
            param.ParameterName = "@Prc";
            param.Value = record.Prc;
            command.Parameters.Add(param);                  // Price

            param = command.CreateParameter();
            param.ParameterName = "@SplyAmt";
            param.Value = record.SplyAmt;
            command.Parameters.Add(param);                  // Supply Amount

            param = command.CreateParameter();
            param.ParameterName = "@TotDcAmt";
            param.Value = record.TotDcAmt;
            command.Parameters.Add(param);                  // Total Discount Amount

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

insert into STOCK_IO_ITEM ( 
       TIN, 
       BHF_ID, 
       SAR_NO, 
       ITEM_SEQ, 
       ITEM_CD, 
       ITEM_CLS_CD, 
       ITEM_NM, 
       BCD, 
       PKG_UNIT_CD, 
       PKG, 
       QTY_UNIT_CD, 
       QTY, 
       ITEM_EXPR_DT, 
       PRC, 
       SPLY_AMT, 
       TOT_DC_AMT, 
       TAXBL_AMT, 
       TAX_TY_CD, 
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
       STC_ID as SAR_NO, 
       ITEM_SEQ as ITEM_SEQ, 
       ITEM_CD as ITEM_CD, 
       ITEM_CLS_CD as ITEM_CLS_CD, 
       ITEM_NM as ITEM_NM, 
       '' as BCD, 
       PKG_UNIT_CD as PKG_UNIT_CD, 
       PKG_QTY as PKG, 
       QTY_UNIT_CD as QTY_UNIT_CD, 
       QTY as QTY, 
       EXPIR_DT as ITEM_EXPR_DT, 
       UNTPC as PRC, 
       SPLPC as SPLY_AMT, 
       (DC_RATE * TAXABL_SPLPC) as TOT_DC_AMT, 
       TAXABL_SPLPC as TAXBL_AMT, 
       TAX_TY_CD as TAX_TY_CD, 
       VAT_AMT as TAX_AMT, 
       TOT_AMT as TOT_AMT, 
       '' as REGR_ID, 
       '' as REGR_NM, 
       SND_DT as REG_DT, 
       '' as MODR_ID, 
       '' as MODR_NM, 
       RCV_DT as MOD_DT
  from STCWHIOITEM 

 */

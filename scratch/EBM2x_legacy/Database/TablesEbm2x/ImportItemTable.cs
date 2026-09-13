using EBM2x.Database.Master;
using EBM2x.RraSdc.model;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of ImportItemTable.
    /// </summary>
    public class ImportItemTable
    {
        public ImportItemTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists IMPORT_ITEM ( "); 
            sql.Append("       TASK_CD            VARCHAR(50)        not null , ");      // Operation Code
            sql.Append("       DCL_DE             CHAR(8)            not null , ");      // Declaration Date
            sql.Append("       ITEM_SEQ           INT                not null , ");      // Item Sequence
            sql.Append("       DCL_NO             VARCHAR(50)        null , ");          // Declaration Number
            sql.Append("       IMPT_ITEM_STTS_CD  VARCHAR(5)         not null , ");      // Import Item Status Status Code
            sql.Append("       TIN                CHAR(9)            null , ");          // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM           VARCHAR(200)       null , ");          // Taxpayer's Name
            sql.Append("       ITEM_CD            VARCHAR(20)        null , ");          // Item Code
            sql.Append("       ITEM_CLS_CD        VARCHAR(10)        null , ");          // Item Classification Code
            sql.Append("       HS_CD              VARCHAR(17)        not null , ");      // HS Code
            sql.Append("       ITEM_NM            VARCHAR(500)       not null , ");      // ItemName
            sql.Append("       ORGN_NAT_CD        VARCHAR(5)         not null , ");      // Country Code of Origin
            sql.Append("       EXPT_NAT_CD        VARCHAR(5)         null , ");          // Country Code of Export
            sql.Append("       PKG                DECIMAL(13,2)      not null , ");      // Packing
            sql.Append("       PKG_UNIT_CD        VARCHAR(5)         not null , ");      // Packing Unit Code
            sql.Append("       QTY                DECIMAL(13,2)      null , ");          // Quantity
            sql.Append("       QTY_UNIT_CD        VARCHAR(5)         null , ");          // Quantity Unit Code
            sql.Append("       TOT_WT             DECIMAL(13,2)      not null , ");      // Gross Weight
            sql.Append("       NET_WT             DECIMAL(13,2)      not null , ");      // Net Weight
            sql.Append("       SPPLR_NM           VARCHAR(500)       null , ");          // Supplier's name
            sql.Append("       AGNT_NM            VARCHAR(500)       null , ");          // Agent's Name
            sql.Append("       INVC_FCUR_AMT      DECIMAL(18,2)      not null , ");      // Invoice Foreign Currency Amount
            sql.Append("       INVC_FCUR_CD       VARCHAR(5)         not null , ");      // Invoice Foreign Currency Code
            sql.Append("       INVC_FCUR_EXCRT    DECIMAL(18,2)      null , ");          // Invoice Foreign Currency Rate
            sql.Append("       DCL_TAXOFC_CD      VARCHAR(10)        null , ");          // Declaration Tax Office Code
            sql.Append("       TRFF_AMT           DECIMAL(18,2)      null , ");          // Tariff Amount
            sql.Append("       VAT_AMT            DECIMAL(18,2)      null , ");          // VAT
            sql.Append("       REMARK             VARCHAR(400)       null , ");          // Remark
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       primary key ( TASK_CD, DCL_DE, ITEM_SEQ, HS_CD ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TASK_CD, ");                 // Operation Code
            sql.Append("       DCL_DE, ");                  // Declaration Date
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       DCL_NO, ");                  // Declaration Number
            sql.Append("       IMPT_ITEM_STTS_CD, ");       // Import Item Status Status Code
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       HS_CD, ");                   // HS Code
            sql.Append("       ITEM_NM, ");                 // ItemName
            sql.Append("       ORGN_NAT_CD, ");             // Country Code of Origin
            sql.Append("       EXPT_NAT_CD, ");             // Country Code of Export
            sql.Append("       PKG, ");                     // Packing
            sql.Append("       PKG_UNIT_CD, ");             // Packing Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       TOT_WT, ");                  // Gross Weight
            sql.Append("       NET_WT, ");                  // Net Weight
            sql.Append("       SPPLR_NM, ");                // Supplier's name
            sql.Append("       AGNT_NM, ");                 // Agent's Name
            sql.Append("       INVC_FCUR_AMT, ");           // Invoice Foreign Currency Amount
            sql.Append("       INVC_FCUR_CD, ");            // Invoice Foreign Currency Code
            sql.Append("       INVC_FCUR_EXCRT, ");         // Invoice Foreign Currency Rate
            sql.Append("       DCL_TAXOFC_CD, ");           // Declaration Tax Office Code
            sql.Append("       TRFF_AMT, ");                // Tariff Amount
            sql.Append("       VAT_AMT, ");                 // VAT
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            //sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '26' AND CD = A.IMPT_ITEM_STTS_CD) AS APPROVAL_STATUS_NM,   ");
            //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.ORGN_NAT_CD),'') AS ORGPLCE_NM,   ");
            //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.EXPT_NAT_CD),'') AS EXPORT_NATION_NM,   ");
            //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '10' AND CD = A.QTY_UNIT_CD),'') AS QTY_UNIT_NM,   ");
            //sql.Append("       IFNULL((SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '33' AND CD = A.INVC_FCUR_CD),'') AS INV_CUR_NM,   ");
            //sql.Append("       IFNULL((SELECT ITEM_CLS_NM FROM ITEM_CLASS WHERE ITEM_CLS_CD = A.ITEM_CLS_CD),'') AS ITEM_CLS_NM,   ");
            //sql.Append("       IFNULL((SELECT ITEM_NM FROM TAXPAYER_ITEM WHERE TIN = A.TIN AND ITEM_CD = A.ITEM_CD),'') AS SUPPLIER_ITEM_NM,   ");
            //sql.Append("       IFNULL(A.ITEM_CD,'') AS SUPPLIER_ITEM_CD,   ");
            //sql.Append("       IFNULL(A.MOD_DT,'') AS PROCESS_DATE,   ");
            //sql.Append("       IFNULL(A.REGR_ID,'') AS DCLR_NO,   ");
            //sql.Append("       IFNULL(A.REGR_ID,'') AS AGENT_NM  ");
            sql.Append("  from IMPORT_ITEM A "); 
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into IMPORT_ITEM ( ");
            sql.Append("       TASK_CD, ");                 // Operation Code
            sql.Append("       DCL_DE, ");                  // Declaration Date
            sql.Append("       ITEM_SEQ, ");                // Item Sequence
            sql.Append("       DCL_NO, ");                  // Declaration Number
            sql.Append("       IMPT_ITEM_STTS_CD, ");       // Import Item Status Status Code
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM, ");                // Taxpayer's Name
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code
            sql.Append("       HS_CD, ");                   // HS Code
            sql.Append("       ITEM_NM, ");                 // ItemName
            sql.Append("       ORGN_NAT_CD, ");             // Country Code of Origin
            sql.Append("       EXPT_NAT_CD, ");             // Country Code of Export
            sql.Append("       PKG, ");                     // Packing
            sql.Append("       PKG_UNIT_CD, ");             // Packing Unit Code
            sql.Append("       QTY, ");                     // Quantity
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       TOT_WT, ");                  // Gross Weight
            sql.Append("       NET_WT, ");                  // Net Weight
            sql.Append("       SPPLR_NM, ");                // Supplier's name
            sql.Append("       AGNT_NM, ");                 // Agent's Name
            sql.Append("       INVC_FCUR_AMT, ");           // Invoice Foreign Currency Amount
            sql.Append("       INVC_FCUR_CD, ");            // Invoice Foreign Currency Code
            sql.Append("       INVC_FCUR_EXCRT, ");         // Invoice Foreign Currency Rate
            sql.Append("       DCL_TAXOFC_CD, ");           // Declaration Tax Office Code
            sql.Append("       TRFF_AMT, ");                // Tariff Amount
            sql.Append("       VAT_AMT, ");                 // VAT
            sql.Append("       REMARK, ");                  // Remark
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT  ");                  // Modified Date
            sql.Append("     ) values ( ");
            sql.Append("       @TaskCd, ");                 // Operation Code
            sql.Append("       @DclDe, ");                  // Declaration Date
            sql.Append("       @ItemSeq, ");                // Item Sequence
            sql.Append("       @DclNo, ");                  // Declaration Number
            sql.Append("       @ImptItemSttsCd, ");         // Import Item Status Status Code
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @TaxprNm, ");                // Taxpayer's Name
            sql.Append("       @ItemCd, ");                 // Item Code
            sql.Append("       @ItemClsCd, ");              // Item Classification Code
            sql.Append("       @HsCd, ");                   // HS Code
            sql.Append("       @ItemNm, ");                 // ItemName
            sql.Append("       @OrgnNatCd, ");              // Country Code of Origin
            sql.Append("       @ExptNatCd, ");              // Country Code of Export
            sql.Append("       @Pkg, ");                    // Packing
            sql.Append("       @PkgUnitCd, ");              // Packing Unit Code
            sql.Append("       @Qty, ");                    // Quantity
            sql.Append("       @QtyUnitCd, ");              // Quantity Unit Code
            sql.Append("       @TotWt, ");                  // Gross Weight
            sql.Append("       @NetWt, ");                  // Net Weight
            sql.Append("       @SpplrNm, ");                // Supplier's name
            sql.Append("       @AgntNm, ");                 // Agent's Name
            sql.Append("       @InvcFcurAmt, ");            // Invoice Foreign Currency Amount
            sql.Append("       @InvcFcurCd, ");             // Invoice Foreign Currency Code
            sql.Append("       @InvcFcurExcrt, ");          // Invoice Foreign Currency Rate
            sql.Append("       @DclTaxofcCd, ");            // Declaration Tax Office Code
            sql.Append("       @TrffAmt, ");                // Tariff Amount
            sql.Append("       @VatAmt, ");                 // VAT
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
            sql.Append("update IMPORT_ITEM ");
            sql.Append("   set DCL_NO = @DclNo, ");         // Declaration Number
            sql.Append("       IMPT_ITEM_STTS_CD = @ImptItemSttsCd, ");  // Import Item Status Status Code
            sql.Append("       TIN = @Tin, ");              // Taxpayer Identification Number(TIN)
            sql.Append("       TAXPR_NM = @TaxprNm, ");     // Taxpayer's Name
            sql.Append("       ITEM_CD = @ItemCd, ");       // Item Code
            sql.Append("       ITEM_CLS_CD = @ItemClsCd, ");  // Item Classification Code
            sql.Append("       HS_CD = @HsCd, ");           // HS Code
            sql.Append("       ITEM_NM = @ItemNm, ");       // ItemName
            sql.Append("       ORGN_NAT_CD = @OrgnNatCd, ");  // Country Code of Origin
            sql.Append("       EXPT_NAT_CD = @ExptNatCd, ");  // Country Code of Export
            sql.Append("       PKG = @Pkg, ");              // Packing
            sql.Append("       PKG_UNIT_CD = @PkgUnitCd, ");  // Packing Unit Code
            sql.Append("       QTY = @Qty, ");              // Quantity
            sql.Append("       QTY_UNIT_CD = @QtyUnitCd, ");  // Quantity Unit Code
            sql.Append("       TOT_WT = @TotWt, ");         // Gross Weight
            sql.Append("       NET_WT = @NetWt, ");         // Net Weight
            sql.Append("       SPPLR_NM = @SpplrNm, ");     // Supplier's name
            sql.Append("       AGNT_NM = @AgntNm, ");       // Agent's Name
            sql.Append("       INVC_FCUR_AMT = @InvcFcurAmt, ");  // Invoice Foreign Currency Amount
            sql.Append("       INVC_FCUR_CD = @InvcFcurCd, ");  // Invoice Foreign Currency Code
            sql.Append("       INVC_FCUR_EXCRT = @InvcFcurExcrt, ");  // Invoice Foreign Currency Rate
            sql.Append("       DCL_TAXOFC_CD = @DclTaxofcCd, ");  // Declaration Tax Office Code
            sql.Append("       TRFF_AMT = @TrffAmt, ");     // Tariff Amount
            sql.Append("       VAT_AMT = @VatAmt, ");       // VAT
            sql.Append("       REMARK = @Remark, ");        // Remark
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TASK_CD = @TaskCd ");  // Operation Code
            sql.Append("   and DCL_DE = @DclDe ");    // Declaration Date
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            sql.Append("   and HS_CD = @HsCd "); 
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from IMPORT_ITEM "); // 수입 품목
            sql.Append(" where TASK_CD = @TaskCd ");  // Operation Code
            sql.Append("   and DCL_DE = @DclDe ");    // Declaration Date
            sql.Append("   and ITEM_SEQ = @ItemSeq ");  // Item Sequence
            sql.Append("   and HS_CD = @HsCd ");  // 
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, ImportItemRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@TaskCd";
            param.Value = record.TaskCd;
            command.Parameters.Add(param);                  // Operation Code

            param = command.CreateParameter();
            param.ParameterName = "@DclDe";
            param.Value = record.DclDe;
            command.Parameters.Add(param);                  // Declaration Date

            param = command.CreateParameter();
            param.ParameterName = "@ItemSeq";
            param.Value = record.ItemSeq;
            command.Parameters.Add(param);                  // Item Sequence

            param = command.CreateParameter();
            param.ParameterName = "@DclNo";
            param.Value = record.DclNo;
            command.Parameters.Add(param);                  // Declaration Number

            param = command.CreateParameter();
            param.ParameterName = "@ImptItemSttsCd";
            param.Value = record.ImptItemSttsCd;
            command.Parameters.Add(param);                  // Import Item Status Status Code

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = record.TaxprNm;
            command.Parameters.Add(param);                  // Taxpayer's Name

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = record.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = record.ItemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code

            param = command.CreateParameter();
            param.ParameterName = "@HsCd";
            param.Value = record.HsCd;
            command.Parameters.Add(param);                  // HS Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemNm";
            param.Value = record.ItemNm;
            command.Parameters.Add(param);                  // ItemName

            param = command.CreateParameter();
            param.ParameterName = "@OrgnNatCd";
            param.Value = record.OrgnNatCd;
            command.Parameters.Add(param);                  // Country Code of Origin

            param = command.CreateParameter();
            param.ParameterName = "@ExptNatCd";
            param.Value = record.ExptNatCd;
            command.Parameters.Add(param);                  // Country Code of Export

            param = command.CreateParameter();
            param.ParameterName = "@Pkg";
            param.Value = record.Pkg;
            command.Parameters.Add(param);                  // Packing

            param = command.CreateParameter();
            param.ParameterName = "@PkgUnitCd";
            param.Value = record.PkgUnitCd;
            command.Parameters.Add(param);                  // Packing Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@Qty";
            param.Value = record.Qty;
            command.Parameters.Add(param);                  // Quantity

            param = command.CreateParameter();
            param.ParameterName = "@QtyUnitCd";
            param.Value = record.QtyUnitCd;
            command.Parameters.Add(param);                  // Quantity Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@TotWt";
            param.Value = record.TotWt;
            command.Parameters.Add(param);                  // Gross Weight

            param = command.CreateParameter();
            param.ParameterName = "@NetWt";
            param.Value = record.NetWt;
            command.Parameters.Add(param);                  // Net Weight

            param = command.CreateParameter();
            param.ParameterName = "@SpplrNm";
            param.Value = record.SpplrNm;
            command.Parameters.Add(param);                  // Supplier's name

            param = command.CreateParameter();
            param.ParameterName = "@AgntNm";
            param.Value = record.AgntNm;
            command.Parameters.Add(param);                  // Agent's Name

            param = command.CreateParameter();
            param.ParameterName = "@InvcFcurAmt";
            param.Value = record.InvcFcurAmt;
            command.Parameters.Add(param);                  // Invoice Foreign Currency Amount

            param = command.CreateParameter();
            param.ParameterName = "@InvcFcurCd";
            param.Value = record.InvcFcurCd;
            command.Parameters.Add(param);                  // Invoice Foreign Currency Code

            param = command.CreateParameter();
            param.ParameterName = "@InvcFcurExcrt";
            param.Value = record.InvcFcurExcrt;
            command.Parameters.Add(param);                  // Invoice Foreign Currency Rate

            param = command.CreateParameter();
            param.ParameterName = "@DclTaxofcCd";
            param.Value = record.DclTaxofcCd;
            command.Parameters.Add(param);                  // Declaration Tax Office Code

            param = command.CreateParameter();
            param.ParameterName = "@TrffAmt";
            param.Value = record.TrffAmt;
            command.Parameters.Add(param);                  // Tariff Amount

            param = command.CreateParameter();
            param.ParameterName = "@VatAmt";
            param.Value = record.VatAmt;
            command.Parameters.Add(param);                  // VAT

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
        public bool SetParametersSDC(IDbCommand command, ImportItem record, ImportItemRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@TaskCd";
            param.Value = record.taskCd;
            command.Parameters.Add(param);                  // Operation Code

            param = command.CreateParameter();
            param.ParameterName = "@DclDe";
            param.Value = record.dclDe;
            command.Parameters.Add(param);                  // Declaration Date

            param = command.CreateParameter();
            param.ParameterName = "@ItemSeq";
            param.Value = record.itemSeq;
            command.Parameters.Add(param);                  // Item Sequence

            param = command.CreateParameter();
            param.ParameterName = "@DclNo";
            param.Value = record.dclNo;
            command.Parameters.Add(param);                  // Declaration Number

            param = command.CreateParameter();
            param.ParameterName = "@ImptItemSttsCd";
            param.Value = (string.IsNullOrEmpty(record.imptItemsttsCd)) ? "2" : record.imptItemsttsCd;
            command.Parameters.Add(param);                  // Import Item Status Status Code

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record2.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@TaxprNm";
            param.Value = record2.TaxprNm;
            command.Parameters.Add(param);                  // Taxpayer's Name

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = record2.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = record2.ItemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code

            param = command.CreateParameter();
            param.ParameterName = "@HsCd";
            param.Value = record.hsCd;
            command.Parameters.Add(param);                  // HS Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemNm";
            param.Value = (string.IsNullOrEmpty(record.itemNm)) ? "" : record.itemNm;
            command.Parameters.Add(param);                  // ItemName

            param = command.CreateParameter();
            param.ParameterName = "@OrgnNatCd";
            param.Value = record.orgnNatCd;
            command.Parameters.Add(param);                  // Country Code of Origin

            param = command.CreateParameter();
            param.ParameterName = "@ExptNatCd";
            param.Value = record.exptNatCd;
            command.Parameters.Add(param);                  // Country Code of Export

            param = command.CreateParameter();
            param.ParameterName = "@Pkg";
            param.Value = record.pkg;
            command.Parameters.Add(param);                  // Packing

            param = command.CreateParameter();
            param.ParameterName = "@PkgUnitCd";
            param.Value = (string.IsNullOrEmpty(record.pkgUnitCd)) ? "" : record.pkgUnitCd;
            command.Parameters.Add(param);                  // Packing Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@Qty";
            param.Value = record.qty;
            command.Parameters.Add(param);                  // Quantity

            param = command.CreateParameter();
            param.ParameterName = "@QtyUnitCd";
            param.Value = record.qtyUnitCd;
            command.Parameters.Add(param);                  // Quantity Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@TotWt";
            param.Value = record.totWt;
            command.Parameters.Add(param);                  // Gross Weight

            param = command.CreateParameter();
            param.ParameterName = "@NetWt";
            param.Value = record.netWt;
            command.Parameters.Add(param);                  // Net Weight

            param = command.CreateParameter();
            param.ParameterName = "@SpplrNm";
            param.Value = (string.IsNullOrEmpty(record.spplrNm)) ? "" : record.spplrNm;
            command.Parameters.Add(param);                  // Supplier's name

            param = command.CreateParameter();
            param.ParameterName = "@AgntNm";
            param.Value = (string.IsNullOrEmpty(record.agntNm)) ? "" : record.agntNm;
            command.Parameters.Add(param);                  // Agent's Name

            param = command.CreateParameter();
            param.ParameterName = "@InvcFcurAmt";
            param.Value = record.invcFcurAmt;
            command.Parameters.Add(param);                  // Invoice Foreign Currency Amount

            param = command.CreateParameter();
            param.ParameterName = "@InvcFcurCd";
            param.Value = record.invcFcurCd;
            command.Parameters.Add(param);                  // Invoice Foreign Currency Code

            param = command.CreateParameter();
            param.ParameterName = "@InvcFcurExcrt";
            param.Value = record.invcFcurExcrt;
            command.Parameters.Add(param);                  // Invoice Foreign Currency Rate

            param = command.CreateParameter();
            param.ParameterName = "@DclTaxofcCd";
            param.Value = record2.DclTaxofcCd;
            command.Parameters.Add(param);                  // Declaration Tax Office Code

            param = command.CreateParameter();
            param.ParameterName = "@TrffAmt";
            param.Value = record2.TrffAmt;
            command.Parameters.Add(param);                  // Tariff Amount

            param = command.CreateParameter();
            param.ParameterName = "@VatAmt";
            param.Value = record2.VatAmt;
            command.Parameters.Add(param);                  // VAT

            param = command.CreateParameter();
            param.ParameterName = "@Remark";
            param.Value = record2.Remark;
            command.Parameters.Add(param);                  // Remark

            param = command.CreateParameter();
            param.ParameterName = "@RegrId";
            param.Value = record2.RegrId;
            command.Parameters.Add(param);                  // Registrant ID

            param = command.CreateParameter();
            param.ParameterName = "@RegrNm";
            param.Value = record2.RegrNm;
            command.Parameters.Add(param);                  // Registrant Name

            param = command.CreateParameter();
            param.ParameterName = "@RegDt";
            param.Value = record2.RegDt;
            command.Parameters.Add(param);                  // Registered Date

            param = command.CreateParameter();
            param.ParameterName = "@ModrId";
            param.Value = record2.ModrId;
            command.Parameters.Add(param);                  // Modifier ID

            param = command.CreateParameter();
            param.ParameterName = "@ModrNm";
            param.Value = record2.ModrNm;
            command.Parameters.Add(param);                  // Modifier Name

            param = command.CreateParameter();
            param.ParameterName = "@ModDt";
            param.Value = record2.ModDt;
            command.Parameters.Add(param);                  // Modified Date

            return true;
        }
    }
}


/*
insert into IMPORT_ITEM ( 
       TASK_CD, 
       DCL_DE, 
       ITEM_SEQ, 
       DCL_NO, 
       IMPT_ITEM_STTS_CD, 
       TIN, 
       TAXPR_NM, 
       ITEM_CD, 
       ITEM_CLS_CD, 
       HS_CD, 
       ITEM_NM, 
       ORGN_NAT_CD, 
       EXPT_NAT_CD, 
       PKG, 
       PKG_UNIT_CD, 
       QTY, 
       QTY_UNIT_CD, 
       TOT_WT, 
       NET_WT, 
       SPPLR_NM, 
       AGNT_NM, 
       INVC_FCUR_AMT, 
       INVC_FCUR_CD, 
       INVC_FCUR_EXCRT, 
       DCL_TAXOFC_CD, 
       TRFF_AMT, 
       VAT_AMT, 
       REMARK, 
       REGR_ID, 
       REGR_NM, 
       REG_DT, 
       MODR_ID, 
       MODR_NM, 
       MOD_DT 
) 					
select     
       OPERATION_CD as TASK_CD, 
       DCLRT_DATE as DCL_DE, 
       ITEM_SEQ as ITEM_SEQ, 
       DCLRT_NO as DCL_NO,        
       APPROVAL_STATUS_CD as IMPT_ITEM_STTS_CD, 
       '000000000' as TIN, 
       TAXPAYER_NM as TAXPR_NM, 
       ITEM_CD as ITEM_CD, 
       ITEM_CLS_CD as ITEM_CLS_CD,
       HS_CD as HS_CD, 
       ITEM_NM as ITEM_NM, 
       ORGPLCE_CD as ORGN_NAT_CD, 
       EXPORT_NATION_CD as EXPT_NAT_CD, 
       PKG_QTY as PKG, 
       '' as PKG_UNIT_CD, 
       QTY as QTY, 
       QTY_UNIT_CD as QTY_UNIT_CD, 
       GROSS_WT as TOT_WT, 
       NET_WT as NET_WT, 
       SUPPLIER_NM as SPPLR_NM, 
       AGENT_NM as AGNT_NM, 
       INV_AMT_FCX as INVC_FCUR_AMT, 
       INV_CUR_CD as INVC_FCUR_CD, 
       INV_CUR_RATE as INVC_FCUR_EXCRT,
       '' as DCL_TAXOFC_CD, 
       0 as TRFF_AMT, 
       0 as VAT_AMT, 
       REMARK as REMARK, 
       REGUSR_ID as REGR_ID, 
       REGUSR_ID as REGR_NM, 
       REG_DT as REG_DT, 
       REGUSR_ID as MODR_ID, 
       REGUSR_ID as MODR_NM, 
       REG_DT as MOD_DT 
  from STCIMPORTITEM
*/

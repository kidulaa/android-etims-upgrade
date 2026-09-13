using EBM2x.Database.Master;
using System.Data;
using System.Text;
namespace EBM2x.Database.Tables
{
    /// <summary>
    /// Description of TaxpayerItemTable.
    /// </summary>
    public class TaxpayerItemTable
    {
        public TaxpayerItemTable()
        {
        }

        public string GetCreateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("create table if not exists TAXPAYER_ITEM ( "); // 납세자 품목
            sql.Append("       TIN                CHAR(9)            not null , ");      // Taxpayer Identification Number(TIN)
            sql.Append("       ITEM_CD            VARCHAR(20)        not null , ");      // Item Code
            sql.Append("       ITEM_CLS_CD        VARCHAR(10)        not null , ");      // Item Classification Code (RRA)
            sql.Append("       ITEM_TY_CD         VARCHAR(5)         not null , ");      // Item Type Code
            sql.Append("       ITEM_NM            VARCHAR(200)       not null , ");      // Item Name
            sql.Append("       ITEM_STD_NM        VARCHAR(200)       null , ");          // Item Stand Name
            sql.Append("       ORGN_NAT_CD        VARCHAR(5)         not null , ");      // Origin National Code
            sql.Append("       PKG_UNIT_CD        VARCHAR(5)         not null , ");      // Package Unit Code
            sql.Append("       QTY_UNIT_CD        VARCHAR(5)         not null , ");      // Quantity Unit Code
            sql.Append("       TAX_TY_CD          VARCHAR(5)         not null , ");      // Taxation Type Code
            sql.Append("       BCD                VARCHAR(20)        null , ");          // Barcode
            sql.Append("       REG_BHF_ID         CHAR(2)            null , ");          // Branch Office ID
            sql.Append("       USE_YN             CHAR(1)            null , ");          // Use(Y/N)
            sql.Append("       RRA_MOD_YN         CHAR(1)            null , ");      // RRA Modified(Y/N)
            sql.Append("       ADD_INFO           VARCHAR(7)         null , ");          // Additional Information
            sql.Append("       SFTY_QTY           DECIMAL(13,2)      not null , ");      // Safety Quantity
            sql.Append("       ISRC_APLCB_YN      CHAR(1)            null , ");      // Insurance Appicable(Y/N)
            sql.Append("       DFT_PRC            DECIMAL(13,2)      null , ");          // Default Price
            sql.Append("       GRP_PRC_L1         DECIMAL(13,2)      null , ");          // Group Default Price L1
            sql.Append("       GRP_PRC_L2         DECIMAL(13,2)      null , ");          // Group Default Price L2
            sql.Append("       GRP_PRC_L3         DECIMAL(13,2)      null , ");          // Group Default Price L3
            sql.Append("       GRP_PRC_L4         DECIMAL(13,2)      null , ");          // Group Default Price L4
            sql.Append("       GRP_PRC_L5         DECIMAL(13,2)      null , ");          // Group Default Price L5
            sql.Append("       REGR_ID            VARCHAR(20)        null , ");          // Registrant ID
            sql.Append("       REGR_NM            VARCHAR(60)        null , ");          // Registrant Name
            sql.Append("       REG_DT             VARCHAR(14)        null , ");          // Registered Date
            sql.Append("       MODR_ID            VARCHAR(20)        null , ");          // Modifier ID
            sql.Append("       MODR_NM            VARCHAR(60)        null , ");          // Modifier Name
            sql.Append("       MOD_DT             VARCHAR(14)        null , ");          // Modified Date
            sql.Append("       INITL_WH_UNTPC     DECIMAL(18,2)      not null , ");      // 초기 입고단가
            sql.Append("       INITL_QTY          DECIMAL(13,2)      not null , ");      // 초기 입고수량
            sql.Append("       RM                 VARCHAR(400)       null , ");          // 비고
            sql.Append("       USE_BARCODE        CHAR(1)            not null , ");      // 바코드사용여부
            sql.Append("       USE_ADI_YN         CHAR(1)            null , ");          // 부가정보사용여부
            sql.Append("       BATCH_NUM          VARCHAR(20)        null, ");           // BatchNum
            sql.Append("       EXPIRATION_YN      CHAR(1)            null , ");          // 바코드사용여부
            sql.Append("       primary key ( TIN, ITEM_CD ) ) ");
            return sql.ToString();
        }

        public string GetSelectSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code (RRA)
            sql.Append("       ITEM_TY_CD, ");              // Item Type Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       ITEM_STD_NM, ");             // Item Stand Name
            sql.Append("       ORGN_NAT_CD, ");             // Origin National Code
            sql.Append("       PKG_UNIT_CD, ");             // Package Unit Code
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       REG_BHF_ID, ");              // Branch Office ID
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       RRA_MOD_YN, ");              // RRA Modified(Y/N)
            sql.Append("       ADD_INFO, ");                // Additional Information
            sql.Append("       SFTY_QTY, ");                // Safety Quantity
            sql.Append("       ISRC_APLCB_YN, ");           // Insurance Appicable(Y/N)
            sql.Append("       DFT_PRC, ");                 // Default Price
            sql.Append("       GRP_PRC_L1, ");              // Group Default Price L1
            sql.Append("       GRP_PRC_L2, ");              // Group Default Price L2
            sql.Append("       GRP_PRC_L3, ");              // Group Default Price L3
            sql.Append("       GRP_PRC_L4, ");              // Group Default Price L4
            sql.Append("       GRP_PRC_L5, ");              // Group Default Price L5
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       INITL_WH_UNTPC, ");          // 초기 입고단가
            sql.Append("       INITL_QTY, ");               // 초기 입고수량
            sql.Append("       RM, ");                      // 비고
            sql.Append("       USE_BARCODE, ");             // 바코드사용여부
            sql.Append("       USE_ADI_YN, ");              // 부가정보사용여부
            sql.Append("       BATCH_NUM, ");               // BatchNum
            sql.Append("       EXPIRATION_YN ");               // BatchNum
                                                           //            sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '05' AND CD = A.ORGN_NAT_CD) AS ORGPLCE_NM,  ");
                                                           //            sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '24' AND CD = A.ITEM_TY_CD) AS ITEM_TY_NM,   ");
                                                           //            sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '17' AND CD = A.PKG_UNIT_CD) AS PKG_UNIT_NM, ");
                                                           //            sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '10' AND CD = A.QTY_UNIT_CD) AS QTY_UNIT_NM, ");
                                                           //            sql.Append("       (SELECT CD_NM FROM CODE_DTL WHERE CD_CLS = '04' AND CD = A.TAX_TY_CD) AS TAX_TY_NM,     ");
                                                           //            sql.Append("       (SELECT ITEM_CLS_NM FROM ITEM_CLASS WHERE ITEM_CLS_CD = A.ITEM_CLS_CD) AS ITEM_CLS_NM,  ");
                                                           //            sql.Append("       IFNULL((SELECT IFNULL(RSD_QTY,0) FROM STOCK_MASTER WHERE TIN = A.TIN AND ITEM_CD = A.ITEM_CD),0) AS RSD_QTY, ");
            sql.Append("  from TAXPAYER_ITEM A"); // 납세자 품목
            return sql.ToString();
        }

        public string GetInsertSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into TAXPAYER_ITEM ( "); // 납세자 품목
            sql.Append("       TIN, ");                     // Taxpayer Identification Number(TIN)
            sql.Append("       ITEM_CD, ");                 // Item Code
            sql.Append("       ITEM_CLS_CD, ");             // Item Classification Code (RRA)
            sql.Append("       ITEM_TY_CD, ");              // Item Type Code
            sql.Append("       ITEM_NM, ");                 // Item Name
            sql.Append("       ITEM_STD_NM, ");             // Item Stand Name
            sql.Append("       ORGN_NAT_CD, ");             // Origin National Code
            sql.Append("       PKG_UNIT_CD, ");             // Package Unit Code
            sql.Append("       QTY_UNIT_CD, ");             // Quantity Unit Code
            sql.Append("       TAX_TY_CD, ");               // Taxation Type Code
            sql.Append("       BCD, ");                     // Barcode
            sql.Append("       REG_BHF_ID, ");              // Branch Office ID
            sql.Append("       USE_YN, ");                  // Use(Y/N)
            sql.Append("       RRA_MOD_YN, ");              // RRA Modified(Y/N)
            sql.Append("       ADD_INFO, ");                // Additional Information
            sql.Append("       SFTY_QTY, ");                // Safety Quantity
            sql.Append("       ISRC_APLCB_YN, ");           // Insurance Appicable(Y/N)
            sql.Append("       DFT_PRC, ");                 // Default Price
            sql.Append("       GRP_PRC_L1, ");              // Group Default Price L1
            sql.Append("       GRP_PRC_L2, ");              // Group Default Price L2
            sql.Append("       GRP_PRC_L3, ");              // Group Default Price L3
            sql.Append("       GRP_PRC_L4, ");              // Group Default Price L4
            sql.Append("       GRP_PRC_L5, ");              // Group Default Price L5
            sql.Append("       REGR_ID, ");                 // Registrant ID
            sql.Append("       REGR_NM, ");                 // Registrant Name
            sql.Append("       REG_DT, ");                  // Registered Date
            sql.Append("       MODR_ID, ");                 // Modifier ID
            sql.Append("       MODR_NM, ");                 // Modifier Name
            sql.Append("       MOD_DT, ");                  // Modified Date
            sql.Append("       INITL_WH_UNTPC, ");          // 초기 입고단가
            sql.Append("       INITL_QTY, ");               // 초기 입고수량
            sql.Append("       RM, ");                      // 비고
            sql.Append("       USE_BARCODE, ");             // 바코드사용여부
            sql.Append("       USE_ADI_YN, ");              // 부가정보사용여부
            sql.Append("       BATCH_NUM, ");               // BatchNum
            sql.Append("       EXPIRATION_YN ");               // BatchNum
            sql.Append("     ) values ( ");
            sql.Append("       @Tin, ");                    // Taxpayer Identification Number(TIN)
            sql.Append("       @ItemCd, ");                 // Item Code
            sql.Append("       @ItemClsCd, ");              // Item Classification Code (RRA)
            sql.Append("       @ItemTyCd, ");               // Item Type Code
            sql.Append("       @ItemNm, ");                 // Item Name
            sql.Append("       @ItemStdNm, ");              // Item Stand Name
            sql.Append("       @OrgnNatCd, ");              // Origin National Code
            sql.Append("       @PkgUnitCd, ");              // Package Unit Code
            sql.Append("       @QtyUnitCd, ");              // Quantity Unit Code
            sql.Append("       @TaxTyCd, ");                // Taxation Type Code
            sql.Append("       @Bcd, ");                    // Barcode
            sql.Append("       @RegBhfId, ");               // Branch Office ID
            sql.Append("       @UseYn, ");                  // Use(Y/N)
            sql.Append("       @RraModYn, ");               // RRA Modified(Y/N)
            sql.Append("       @AddInfo, ");                // Additional Information
            sql.Append("       @SftyQty, ");                // Safety Quantity
            sql.Append("       @IsrcAplcbYn, ");            // Insurance Appicable(Y/N)
            sql.Append("       @DftPrc, ");                 // Default Price
            sql.Append("       @GrpPrcL1, ");               // Group Default Price L1
            sql.Append("       @GrpPrcL2, ");               // Group Default Price L2
            sql.Append("       @GrpPrcL3, ");               // Group Default Price L3
            sql.Append("       @GrpPrcL4, ");               // Group Default Price L4
            sql.Append("       @GrpPrcL5, ");               // Group Default Price L5
            sql.Append("       @RegrId, ");                 // Registrant ID
            sql.Append("       @RegrNm, ");                 // Registrant Name
            sql.Append("       @RegDt, ");                  // Registered Date
            sql.Append("       @ModrId, ");                 // Modifier ID
            sql.Append("       @ModrNm, ");                 // Modifier Name
            sql.Append("       @ModDt, ");                  // Modified Date
            sql.Append("       @InitlWhUntpc, ");           // 초기 입고단가
            sql.Append("       @InitlQty, ");               // 초기 입고수량
            sql.Append("       @Rm, ");                     // 비고
            sql.Append("       @UseBarcode, ");             // 바코드사용여부
            sql.Append("       @UseAdiYn, ");               // 부가정보사용여부
            sql.Append("       @BatchNum, ");               // BatchNum
            sql.Append("       @UseExpiration ");               // BatchNum
            sql.Append("     ) ");
            return sql.ToString();
        }

        public string GetUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_ITEM "); // 납세자 품목
            sql.Append("   set ITEM_CLS_CD = @ItemClsCd, ");  // Item Classification Code (RRA)
            sql.Append("       ITEM_TY_CD = @ItemTyCd, ");  // Item Type Code
            sql.Append("       ITEM_NM = @ItemNm, ");       // Item Name
            sql.Append("       ITEM_STD_NM = @ItemStdNm, ");  // Item Stand Name
            sql.Append("       ORGN_NAT_CD = @OrgnNatCd, ");  // Origin National Code
            sql.Append("       PKG_UNIT_CD = @PkgUnitCd, ");  // Package Unit Code
            sql.Append("       QTY_UNIT_CD = @QtyUnitCd, ");  // Quantity Unit Code
            sql.Append("       TAX_TY_CD = @TaxTyCd, ");    // Taxation Type Code
            sql.Append("       BCD = @Bcd, ");              // Barcode
            sql.Append("       REG_BHF_ID = @RegBhfId, ");  // Branch Office ID
            sql.Append("       USE_YN = @UseYn, ");         // Use(Y/N)
            sql.Append("       RRA_MOD_YN = @RraModYn, ");  // RRA Modified(Y/N)
            sql.Append("       ADD_INFO = @AddInfo, ");     // Additional Information
            sql.Append("       SFTY_QTY = @SftyQty, ");     // Safety Quantity
            sql.Append("       ISRC_APLCB_YN = @IsrcAplcbYn, ");  // Insurance Appicable(Y/N)
            sql.Append("       DFT_PRC = @DftPrc, ");       // Default Price
            sql.Append("       GRP_PRC_L1 = @GrpPrcL1, ");  // Group Default Price L1
            sql.Append("       GRP_PRC_L2 = @GrpPrcL2, ");  // Group Default Price L2
            sql.Append("       GRP_PRC_L3 = @GrpPrcL3, ");  // Group Default Price L3
            sql.Append("       GRP_PRC_L4 = @GrpPrcL4, ");  // Group Default Price L4
            sql.Append("       GRP_PRC_L5 = @GrpPrcL5, ");  // Group Default Price L5
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt, ");         // Modified Date
            sql.Append("       INITL_WH_UNTPC = @InitlWhUntpc, ");  // 초기 입고단가
            sql.Append("       INITL_QTY = @InitlQty, ");   // 초기 입고수량
            sql.Append("       RM = @Rm, ");                // 비고
            sql.Append("       USE_BARCODE = @UseBarcode, ");  // 바코드사용여부
            sql.Append("       USE_ADI_YN = @UseAdiYn, ");  // 부가정보사용여부
            sql.Append("       BATCH_NUM = @BatchNum, ");   // BatchNum
            sql.Append("       EXPIRATION_YN = @UseExpiration ");   // BatchNum
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and ITEM_CD = @ItemCd ");  // Item Code
            return sql.ToString();

        }
        public string GetDeleteUpdateSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update TAXPAYER_ITEM "); // 납세자 품목
            sql.Append("   set USE_YN = 'N', ");            // Use(Y/N)
            sql.Append("       REGR_ID = @RegrId, ");       // Registrant ID
            sql.Append("       REGR_NM = @RegrNm, ");       // Registrant Name
            sql.Append("       REG_DT = @RegDt, ");         // Registered Date
            sql.Append("       MODR_ID = @ModrId, ");       // Modifier ID
            sql.Append("       MODR_NM = @ModrNm, ");       // Modifier Name
            sql.Append("       MOD_DT = @ModDt  ");         // Modified Date
            sql.Append(" where TIN = @Tin ");               // Taxpayer Identification Number(TIN)
            sql.Append("   and ITEM_CD = @ItemCd ");        // Item Code
            return sql.ToString();

        }

        public string GetDeleteSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from TAXPAYER_ITEM "); // 납세자 품목
            sql.Append(" where TIN = @Tin ");         // Taxpayer Identification Number(TIN)
            sql.Append("   and ITEM_CD = @ItemCd ");  // Item Code
            return sql.ToString();
        }

        public bool SetParameters(IDbCommand command, TaxpayerItemRecord record)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = record.ItemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = record.ItemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code (RRA)

            param = command.CreateParameter();
            param.ParameterName = "@ItemTyCd";
            param.Value = record.ItemTyCd;
            command.Parameters.Add(param);                  // Item Type Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemNm";
            param.Value = record.ItemNm;
            command.Parameters.Add(param);                  // Item Name

            param = command.CreateParameter();
            param.ParameterName = "@ItemStdNm";
            param.Value = record.ItemStdNm;
            command.Parameters.Add(param);                  // Item Stand Name

            param = command.CreateParameter();
            param.ParameterName = "@OrgnNatCd";
            param.Value = record.OrgnNatCd;
            command.Parameters.Add(param);                  // Origin National Code

            param = command.CreateParameter();
            param.ParameterName = "@PkgUnitCd";
            param.Value = record.PkgUnitCd;
            command.Parameters.Add(param);                  // Package Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@QtyUnitCd";
            param.Value = record.QtyUnitCd;
            command.Parameters.Add(param);                  // Quantity Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@TaxTyCd";
            param.Value = record.TaxTyCd;
            command.Parameters.Add(param);                  // Taxation Type Code

            param = command.CreateParameter();
            param.ParameterName = "@Bcd";
            param.Value = record.Bcd;
            command.Parameters.Add(param);                  // Barcode

            param = command.CreateParameter();
            param.ParameterName = "@RegBhfId";
            param.Value = record.RegBhfId;
            command.Parameters.Add(param);                  // Branch Office ID

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.UseYn;
            command.Parameters.Add(param);                  // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@RraModYn";
            param.Value = record.RraModYn;
            command.Parameters.Add(param);                  // RRA Modified(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@AddInfo";
            param.Value = record.AddInfo;
            command.Parameters.Add(param);                  // Additional Information

            param = command.CreateParameter();
            param.ParameterName = "@SftyQty";
            param.Value = record.SftyQty;
            command.Parameters.Add(param);                  // Safety Quantity

            param = command.CreateParameter();
            param.ParameterName = "@IsrcAplcbYn";
            param.Value = record.IsrcAplcbYn;
            command.Parameters.Add(param);                  // Insurance Appicable(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@DftPrc";
            param.Value = record.DftPrc;
            command.Parameters.Add(param);                  // Default Price

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL1";
            param.Value = record.GrpPrcL1;
            command.Parameters.Add(param);                  // Group Default Price L1

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL2";
            param.Value = record.GrpPrcL2;
            command.Parameters.Add(param);                  // Group Default Price L2

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL3";
            param.Value = record.GrpPrcL3;
            command.Parameters.Add(param);                  // Group Default Price L3

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL4";
            param.Value = record.GrpPrcL4;
            command.Parameters.Add(param);                  // Group Default Price L4

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL5";
            param.Value = record.GrpPrcL5;
            command.Parameters.Add(param);                  // Group Default Price L5

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
            param.ParameterName = "@InitlWhUntpc";
            param.Value = record.InitlWhUntpc;
            command.Parameters.Add(param);                  // 초기 입고단가

            param = command.CreateParameter();
            param.ParameterName = "@InitlQty";
            param.Value = record.InitlQty;
            command.Parameters.Add(param);                  // 초기 입고수량

            param = command.CreateParameter();
            param.ParameterName = "@Rm";
            param.Value = record.Rm;
            command.Parameters.Add(param);                  // 비고

            param = command.CreateParameter();
            param.ParameterName = "@UseBarcode";
            param.Value = record.UseBarcode;
            command.Parameters.Add(param);                  // 바코드사용여부

            param = command.CreateParameter();
            param.ParameterName = "@UseAdiYn";
            param.Value = record.UseAdiYn;
            command.Parameters.Add(param);                  // 부가정보사용여부

            param = command.CreateParameter();
            param.ParameterName = "@BatchNum";
            param.Value = record.BatchNum;
            command.Parameters.Add(param);                  // BatchNum

            param = command.CreateParameter();
            param.ParameterName = "@UseExpiration";
            param.Value = record.UseExpiration;
            command.Parameters.Add(param);                  // UseExpiration

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@ExpirationDtUse";
            //param.Value = record.ExpirationDtUse;
            //command.Parameters.Add(param);                  // Expiration Dt Use

            return true;
        }
        public bool SetParametersSDC(IDbCommand command, EBM2x.RraSdc.model.Item record, TaxpayerItemRecord record2)
        {
            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@Tin";
            param.Value = record2.Tin;
            command.Parameters.Add(param);                  // Taxpayer Identification Number(TIN)

            param = command.CreateParameter();
            param.ParameterName = "@ItemCd";
            param.Value = record.itemCd;
            command.Parameters.Add(param);                  // Item Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemClsCd";
            param.Value = record.itemClsCd;
            command.Parameters.Add(param);                  // Item Classification Code (RRA)

            param = command.CreateParameter();
            param.ParameterName = "@ItemTyCd";
            param.Value = record.itemTyCd;
            command.Parameters.Add(param);                  // Item Type Code

            param = command.CreateParameter();
            param.ParameterName = "@ItemNm";
            param.Value = (string.IsNullOrEmpty(record.itemNm)) ? "" : record.itemNm;
            command.Parameters.Add(param);                  // Item Name

            param = command.CreateParameter();
            param.ParameterName = "@ItemStdNm";
            param.Value = (string.IsNullOrEmpty(record.itemStdNm)) ? "" : record.itemStdNm;
            command.Parameters.Add(param);                  // Item Stand Name

            param = command.CreateParameter();
            param.ParameterName = "@OrgnNatCd";
            param.Value = record.orgnNatCd;
            command.Parameters.Add(param);                  // Origin National Code

            param = command.CreateParameter();
            param.ParameterName = "@PkgUnitCd";
            param.Value = record.pkgUnitCd;
            command.Parameters.Add(param);                  // Package Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@QtyUnitCd";
            param.Value = record.qtyUnitCd;
            command.Parameters.Add(param);                  // Quantity Unit Code

            param = command.CreateParameter();
            param.ParameterName = "@TaxTyCd";
            param.Value = record.taxTyCd;
            command.Parameters.Add(param);                  // Taxation Type Code

            param = command.CreateParameter();
            param.ParameterName = "@Bcd";
            param.Value = (string.IsNullOrEmpty(record.bcd)) ? "" : record.bcd;
            command.Parameters.Add(param);                  // Barcode

            param = command.CreateParameter();
            param.ParameterName = "@RegBhfId";
            param.Value = record2.RegBhfId;
            command.Parameters.Add(param);                  // Branch Office ID

            param = command.CreateParameter();
            param.ParameterName = "@UseYn";
            param.Value = record.useYn;
            command.Parameters.Add(param);                  // Use(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@RraModYn";
            param.Value = record.rraModYn;
            command.Parameters.Add(param);                  // RRA Modified(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@AddInfo";
            param.Value = (string.IsNullOrEmpty(record.addInfo)) ? "" : record.addInfo;
            command.Parameters.Add(param);                  // Additional Information

            param = command.CreateParameter();
            param.ParameterName = "@SftyQty";
            param.Value = record.sftyQty;
            command.Parameters.Add(param);                  // Safety Quantity

            param = command.CreateParameter();
            param.ParameterName = "@IsrcAplcbYn";
            param.Value = (string.IsNullOrEmpty(record.isrcAplcbYn)) ? "" : record.isrcAplcbYn;
            command.Parameters.Add(param);                  // Insurance Appicable(Y/N)

            param = command.CreateParameter();
            param.ParameterName = "@DftPrc";
            param.Value = record.dftPrc;
            command.Parameters.Add(param);                  // Default Price

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL1";
            param.Value = record.grpPrcL1;
            command.Parameters.Add(param);                  // Group Default Price L1

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL2";
            param.Value = record.grpPrcL2;
            command.Parameters.Add(param);                  // Group Default Price L2

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL3";
            param.Value = record.grpPrcL3;
            command.Parameters.Add(param);                  // Group Default Price L3

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL4";
            param.Value = record.grpPrcL4;
            command.Parameters.Add(param);                  // Group Default Price L4

            param = command.CreateParameter();
            param.ParameterName = "@GrpPrcL5";
            param.Value = record.grpPrcL5;
            command.Parameters.Add(param);                  // Group Default Price L5

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

            param = command.CreateParameter();
            param.ParameterName = "@InitlWhUntpc";
            param.Value = record2.InitlWhUntpc;
            command.Parameters.Add(param);                  // 초기 입고단가

            param = command.CreateParameter();
            param.ParameterName = "@InitlQty";
            param.Value = record2.InitlQty;
            command.Parameters.Add(param);                  // 초기 입고수량

            param = command.CreateParameter();
            param.ParameterName = "@Rm";
            param.Value = record2.Rm;
            command.Parameters.Add(param);                  // 비고

            param = command.CreateParameter();
            param.ParameterName = "@UseBarcode";
            param.Value = record2.UseBarcode;
            command.Parameters.Add(param);                  // 바코드사용여부

            param = command.CreateParameter();
            param.ParameterName = "@UseAdiYn";
            param.Value = record2.UseAdiYn;
            command.Parameters.Add(param);                  // 부가정보사용여부

            param = command.CreateParameter();
            param.ParameterName = "@BatchNum";
            param.Value = (string.IsNullOrEmpty(record.batchNum)) ? "" : record.batchNum;
            command.Parameters.Add(param);                  // BatchNum

            param = command.CreateParameter();
            param.ParameterName = "@UseExpiration";
            param.Value = "N";
            command.Parameters.Add(param);                  // UseExpiration

            //JCNA 202001 DELETE
            //param = command.CreateParameter();
            //param.ParameterName = "@ExpirationDtUse";
            //param.Value = record2.ExpirationDtUse;
            //command.Parameters.Add(param);                  // Expiration Dt Use

            return true;
        }
    }
}

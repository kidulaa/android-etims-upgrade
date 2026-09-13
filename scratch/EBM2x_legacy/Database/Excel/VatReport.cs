using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Database.Excel
{
    class VatReport
    {
    }
}
/*
[Sales Report]
SELECT ts.CUST_TIN
     , ts.CUST_NM
     , tsr.INVC_NO
     , tsr.RCPT_PBCT_DT AS RCPT_PBCT_DT
     , (ts.TOT_AMT - ts.TOT_TAX_AMT) * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END AS TOT_AMT
     , ts.TAXBL_AMT_A * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END   AS TAXBL_AMT_A
     , ts.TAXBL_AMT_C * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END   AS TAXBL_AMT_C
     , (ts.TAXBL_AMT_B - ts.TOT_TAX_AMT) * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END AS TAXBL_AMT_B
     , ts.TOT_TAX_AMT * CASE WHEN ts.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END   AS TOT_TAX_AMT
  FROM TRNS_SALE ts, TRNS_SALE_RECEIPT tsr
 WHERE tsr.TIN = ts.TIN
   AND tsr.BHF_ID = ts.BHF_ID
   AND tsr.INVC_NO = ts.INVC_NO
   AND ts.SALES_DT BETWEEN @StartDate AND @EndDate

SELECT
   ts.CUST_TIN
   , ts.CUST_NM
   , tsr.INVC_NO
   , TO_CHAR(tsr.RCPT_PBCT_DT, 'dd/MM/yyyy')                  AS RCPT_PBCT_DT
   , (ts.TOT_AMT - ts.TOT_TAX_AMT)
      * DECODE(ts.RCPT_TY_CD, 'S', 1, -1)            AS TOT_AMT
   , ts.TAXBL_AMT_A * DECODE(ts.RCPT_TY_CD, 'S', 1, -1)   AS TAXBL_AMT_A
   , ts.TAXBL_AMT_C * DECODE(ts.RCPT_TY_CD, 'S', 1, -1)   AS TAXBL_AMT_C
   , (ts.TAXBL_AMT_B - ts.TOT_TAX_AMT)
      * DECODE(ts.RCPT_TY_CD, 'S', 1, -1)            AS TAXBL_AMT_B
   , ts.TOT_TAX_AMT * DECODE(ts.RCPT_TY_CD, 'S', 1, -1)   AS TOT_TAX_AMT
FROM TRNS_SALE ts
   LEFT OUTER JOIN TRNS_SALE_RECEIPT tsr
   ON tsr.TIN = ts.TIN
   AND tsr.BHF_ID = ts.BHF_ID
   AND tsr.INVC_NO = ts.INVC_NO


[Purchase Report]
            sql.Append(" SELECT tp.INVC_NO ");
            sql.Append("      , tp.SPPLR_TIN ");
            sql.Append("      , tp.SPPLR_NM ");
            sql.Append("      , SPPLR_RCPT_NO AS RPT_NO ");
            sql.Append("      , PCHS_DT ");
            sql.Append("      , REG_DT AS RCPT_PBCT_DT ");
            sql.Append("      , tp.TOT_AMT * CASE WHEN tp.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END AS TOT_AMT ");
            sql.Append("      , tp.TOT_TAX_AMT * CASE WHEN tp.RCPT_TY_CD = 'S' THEN 1 ELSE -1 END AS TOT_VAT ");
            sql.Append("   FROM TRNS_PURCHASE tp ");
            sql.Append("  WHERE tp.PCHS_DT BETWEEN @StartDate AND @EndDate ");

SELECT   tp.INVC_NO
      , tp.SPPLR_TIN
      , tp.SPPLR_NM
      , tsr.RPT_NO
      , TO_CHAR(tp.PCHS_DT, 'dd/MM/yyyy') PCHS_DT
      , TO_CHAR(tsr.RCPT_PBCT_DT, 'dd/MM/yyyy') RCPT_PBCT_DT
      , tp.TOT_AMT * DECODE(tp.RCPT_TY_CD, 'P', 1, -1)      AS TOT_AMT
      , tp.TOT_TAX_AMT * DECODE(tp.RCPT_TY_CD, 'P', 1, -1)   AS TOT_VAT
FROM   TRNS_PURCHASE tp
   LEFT OUTER JOIN TRNS_SALE_RECEIPT tsr
      ON tsr.TIN = tp.TIN
      AND tsr.BHF_ID = tp.BHF_ID
      AND tsr.INVC_NO = tp.INVC_NO

[Import Item]
            sql.Append(" SELECT DCL_TAXOFC_CD ");
            sql.Append("      , DCL_NO ");
            sql.Append("      , DCL_DE ");
            sql.Append("      , ITEM_NM ");
            sql.Append("      , ORGN_NAT_CD ");
            sql.Append("      , CASE WHEN TRFF_AMT IS NULL THEN 0 ELSE TRFF_AMT END AS TRFF_AMT ");
            sql.Append("      , CASE WHEN VAT_AMT IS NULL THEN 0 ELSE VAT_AMT END AS VAT_AMT ");
            sql.Append("   FROM IMPORT_ITEM ");
            sql.Append("  WHERE DCL_DE BETWEEN @StartDate AND @EndDate ");

SELECT   DCL_TAXOFC_CD
      , DCL_NO
      , TO_CHAR(TO_DATE(DCL_DE,'YYYYMMDD'),'dd/MM/yyyy') DCL_DE
      , ITEM_NM
      , ORGN_NAT_CD
      , NVL(TRFF_AMT, 0) TRFF_AMT
      , NVL(VAT_AMT, 0) VAT_AMT
FROM   IMPORT_ITEM
*/

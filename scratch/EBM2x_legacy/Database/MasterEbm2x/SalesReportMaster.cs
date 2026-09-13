using EBM2x.Database.Master;
using EBM2x.Database.TableIO;
using EBM2x.UI;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.MasterEbm2x
{
    public class SalesReportMaster : ModelIO
    {
        public List<SalesReportRecord> getSalesReportTable(string StartDate, string EndDate, string valusMode)
        {
            List<SalesReportRecord> arrayList = new List<SalesReportRecord>();

            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            StringBuilder sql = new StringBuilder();
            //sql.Append("  SELECT TRNS_SALE_ITEM.ITEM_CD as ITEM_CD,     ");
            //sql.Append("         TRNS_SALE_ITEM.ITEM_NM AS ITEM_NM,      ");
            //sql.Append("         TRNS_SALE_ITEM.PKG_UNIT_CD As PKG_UNIT_CD,  ");
            //sql.Append("         (TRNS_SALE.SALES_TY_CD || TRNS_SALE.RCPT_TY_CD) AS OPER,   ");
            //sql.Append("         sum(TRNS_SALE_ITEM.QTY) As QTY,    ");
            //sql.Append("         TRNS_SALE_ITEM.PRC AS UNTPC,   ");
            //sql.Append("         sum(TRNS_SALE_ITEM.TOT_AMT) as TOT_AMOUNT,   ");
            //sql.Append("         sum(TRNS_SALE_ITEM.TAX_AMT) AS VAT_AMT ");
            //sql.Append("    FROM TRNS_SALE_RECEIPT,TRNS_SALE,TRNS_SALE_ITEM    ");
            //sql.Append("  WHERE TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE_ITEM.INVC_NO    ");
            //sql.Append("    AND TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO   ");
            //if (!valusMode.Equals("A")) sql.Append("    AND TRNS_SALE.SALES_TY_CD='N' AND TRNS_SALE.RCPT_TY_CD=@RCPT_TY_CD   ");
            //sql.Append("    AND TRNS_SALE.SALES_DT BETWEEN @StartDate AND @EndDate  ");
            //sql.Append("  GROUP BY TRNS_SALE_ITEM.ITEM_CD,TRNS_SALE_ITEM.ITEM_NM,TRNS_SALE_ITEM.PKG_UNIT_CD ,(TRNS_SALE.SALES_TY_CD || TRNS_SALE.RCPT_TY_CD)   ");
            //sql.Append("  ORDER BY TRNS_SALE_ITEM.ITEM_NM  ");

            sql.Append(" SELECT TC.ITEM_CD as ITEM_CD,    ");
            sql.Append("        TC.ITEM_NM AS ITEM_NM,    ");
            sql.Append("        sum(TC.QTY) As QTY,    ");
            sql.Append("        TC.PRC AS UNTPC,    ");
            sql.Append("        TB.CUST_TIN AS CUST_TIN,    ");
            sql.Append("        TA.INVC_NO AS INVC_NO,    ");
            sql.Append("        '' AS SDC_NUM,    ");
            sql.Append("        TA.CUR_RCPT_NO AS CUR_RCPT_NO,    ");
            sql.Append("        (TB.SALES_TY_CD || TB.RCPT_TY_CD) AS OPER,    ");
            sql.Append("        TB.SALES_DT AS SALES_DT,    ");
            sql.Append("        sum(TC.TAX_AMT) AS VAT_AMT,    ");
            sql.Append("        sum(TC.TOT_AMT) as TOT_AMOUNT     ");
            sql.Append("   FROM TRNS_SALE_RECEIPT TA,TRNS_SALE TB,TRNS_SALE_ITEM TC     ");
            sql.Append("  WHERE TA.INVC_NO = TC.INVC_NO     ");
            sql.Append("    AND TA.INVC_NO = TB.INVC_NO     ");
            if (!valusMode.Equals("A")) sql.Append("    AND TB.SALES_TY_CD='N' AND TB.RCPT_TY_CD=@RCPT_TY_CD   ");
            sql.Append("    AND TB.SALES_DT BETWEEN @StartDate AND @EndDate  ");
            sql.Append("  GROUP BY TC.ITEM_CD,TC.ITEM_NM,TC.PRC, TB.CUST_TIN, TA.INVC_NO, SDC_NUM, TA.CUR_RCPT_NO, (TB.SALES_TY_CD || TB.RCPT_TY_CD),TB.SALES_DT    ");
            sql.Append("  ORDER BY TC.INVC_NO, TC.ITEM_NM     ");

            try
            {
                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;
                
                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@EndDate";
                param.Value = EndDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@RCPT_TY_CD";
                param.Value = valusMode;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    SalesReportRecord record = new SalesReportRecord();

                    if (!reader.IsDBNull(0)) record.ItemCd = reader.GetString(0);   
                    if (!reader.IsDBNull(1)) record.ItemNm = reader.GetString(1);  
                    if (!reader.IsDBNull(2)) record.Qty = reader.GetDouble(2);
                    if (!reader.IsDBNull(3)) record.Prc = reader.GetDouble(3);
                    if (!reader.IsDBNull(4)) record.CustTin = reader.GetString(4);
                    if (!reader.IsDBNull(5)) record.InvcNo = reader.GetInt32(5);
                    //if (!reader.IsDBNull(6)) record.SdcId = reader.GetString(6);
                    if (!reader.IsDBNull(7)) record.CurRcptNo = reader.GetInt32(7);
                    if (!reader.IsDBNull(8)) record.Oper = reader.GetString(8);
                    if (!reader.IsDBNull(9)) record.SalesDt = reader.GetString(9);
                    if (!reader.IsDBNull(10)) record.VatAmt = reader.GetDouble(10);
                    if (!reader.IsDBNull(11)) record.TotAmt = reader.GetDouble(11);

                    record.SdcId = UIManager.Instance().PosModel.Environment.EnvPosSetup.GblSdcSysNum;

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

        public double GetCodeValue(string StartDate, string EndDate, string SalesTyCd, string RcptTyCd, string DataField)
        {
            double codeValue = 0;

            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("  SELECT sum({0}) as AMOUNT   ", DataField);
                sql.Append("    FROM TRNS_SALE_RECEIPT,TRNS_SALE ");
                sql.Append("  WHERE TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO   ");
                sql.Append("    AND TRNS_SALE.SALES_DT BETWEEN @StartDate AND @EndDate  ");
                sql.Append("    AND TRNS_SALE.SALES_TY_CD = @SALES_TY_CD   ");
                sql.Append("    AND TRNS_SALE.RCPT_TY_CD = @RCPT_TY_CD  ");

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@EndDate";
                param.Value = EndDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SALES_TY_CD";
                param.Value = SalesTyCd;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@RCPT_TY_CD";
                param.Value = RcptTyCd;
                command.Parameters.Add(param);

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) codeValue = reader.GetDouble(0);
                }
                reader.Close();

                return codeValue;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }
        public int GetCodeCount(string StartDate, string EndDate, string SalesTyCd, string RcptTyCd)
        {
            int codeValue = 0;

            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  SELECT IFNULL(count(*),0) as COUNTT ");
                sql.Append("    FROM TRNS_SALE_RECEIPT,TRNS_SALE ");
                sql.Append("  WHERE TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO ");
                sql.Append("    AND TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO ");
                sql.Append("    AND TRNS_SALE.SALES_DT BETWEEN @StartDate AND @EndDate  ");
                sql.Append("    AND TRNS_SALE.SALES_TY_CD = @SALES_TY_CD   ");
                if(!string.IsNullOrEmpty(RcptTyCd))
                    sql.Append("    AND TRNS_SALE.RCPT_TY_CD = @RCPT_TY_CD  ");

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@EndDate";
                param.Value = EndDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SALES_TY_CD";
                param.Value = SalesTyCd;
                command.Parameters.Add(param);

                if (!string.IsNullOrEmpty(RcptTyCd))
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@RCPT_TY_CD";
                    param.Value = RcptTyCd;
                    command.Parameters.Add(param);
                }

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) codeValue = reader.GetInt16(0);
                }
                reader.Close();

                return codeValue;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }
        public int GetCodeCountMIN(string StartDate, string EndDate, string SalesTyCd, string RcptTyCd)
        {
            int codeValue = 0;

            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  SELECT IFNULL(MIN(CUR_RCPT_NO),0) as COUNTT ");
                sql.Append("    FROM TRNS_SALE_RECEIPT,TRNS_SALE ");
                sql.Append("  WHERE TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO ");
                sql.Append("    AND TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO ");
                sql.Append("    AND TRNS_SALE.SALES_DT BETWEEN @StartDate AND @EndDate  ");
                sql.Append("    AND TRNS_SALE.SALES_TY_CD = @SALES_TY_CD   ");
                if (!string.IsNullOrEmpty(RcptTyCd))
                    sql.Append("    AND TRNS_SALE.RCPT_TY_CD = @RCPT_TY_CD  ");

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@EndDate";
                param.Value = EndDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SALES_TY_CD";
                param.Value = SalesTyCd;
                command.Parameters.Add(param);

                if (!string.IsNullOrEmpty(RcptTyCd))
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@RCPT_TY_CD";
                    param.Value = RcptTyCd;
                    command.Parameters.Add(param);
                }

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) codeValue = reader.GetInt16(0);
                }
                reader.Close();

                return codeValue;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }
        public int GetCodeCountMAX(string StartDate, string EndDate, string SalesTyCd, string RcptTyCd)
        {
            int codeValue = 0;

            IDbCommand command = GetDbCommand();
            if (command == null) return 0;

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  SELECT IFNULL(MAX(CUR_RCPT_NO),0) as COUNTT ");
                sql.Append("    FROM TRNS_SALE_RECEIPT,TRNS_SALE ");
                sql.Append("  WHERE TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO ");
                sql.Append("    AND TRNS_SALE_RECEIPT.INVC_NO = TRNS_SALE.INVC_NO ");
                sql.Append("    AND TRNS_SALE.SALES_DT BETWEEN @StartDate AND @EndDate  ");
                sql.Append("    AND TRNS_SALE.SALES_TY_CD = @SALES_TY_CD   ");
                if (!string.IsNullOrEmpty(RcptTyCd))
                    sql.Append("    AND TRNS_SALE.RCPT_TY_CD = @RCPT_TY_CD  ");

                command.CommandText = sql.ToString();
                command.CommandType = CommandType.Text;

                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@StartDate";
                param.Value = StartDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@EndDate";
                param.Value = EndDate;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@SALES_TY_CD";
                param.Value = SalesTyCd;
                command.Parameters.Add(param);

                if (!string.IsNullOrEmpty(RcptTyCd))
                {
                    param = command.CreateParameter();
                    param.ParameterName = "@RCPT_TY_CD";
                    param.Value = RcptTyCd;
                    command.Parameters.Add(param);
                }

                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0)) codeValue = reader.GetInt16(0);
                }
                reader.Close();

                return codeValue;
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return 0;
            }
        }
    }
}

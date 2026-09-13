using EBM2x.Database.Tables;
using EBM2x.UI;
using EBM2x.Utils;
using System;
namespace EBM2x.Database
{
    /// <summary>
    /// Description of Ebm2xCreateTables.
    /// </summary>
    public class Ebm2xCreateTables
    {
		public Ebm2xCreateTables()
		{
		}

        // 테이블 생성
        public static bool CreateZreportTables()
        {
            bool bRet = true;
            Ebm2xCreateTables bbm2xMasterManager = new Ebm2xCreateTables();

            bRet = bbm2xMasterManager.TableCheck(new ZreportTable().GetTableCheckSQL(UIManager.Instance().IsMySQL));
            if (bRet)
            {

                bRet = bbm2xMasterManager.CreateTable(new ZreportTable().GetCreateSQL());
                if (bRet != true)
                {
                    LogWriter.ErrorLog("[Error] CreateTable -> ZreportTable");
                }
            }

            return true;
        }


        // 테이블 생성
        public static bool CreateTables()
		{
			bool bRet = true;
            Ebm2xCreateTables bbm2xMasterManager = new Ebm2xCreateTables();

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerItemTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerItemTable");
            }

            return true;

            bRet = bbm2xMasterManager.CreateTable(new CodeClassTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> CodeClassTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new CodeDtlTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> CodeDtlTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new ItemClassTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> ItemClassTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerBaseTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerBaseTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerBhfCustTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerBhfCustTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerBhfDeviceUserTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerBhfDeviceUserTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerBhfInsuranceTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerBhfInsuranceTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerBhfTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerBhfTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TaxpayerItemTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TaxpayerItemTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new BbsNoticeTable().GetCreateSQL());
            if(bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> BbsNoticeTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new StockMasterTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> StockMasterTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new ImportItemTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> ImportItemTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new StockIoTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> StockIoTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new StockIoItemTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> StockIoItemTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TrnsPurchaseTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TrnsPurchaseTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TrnsPurchaseItemTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TrnsPurchaseItemTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TrnsSaleTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TrnsSaleTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TrnsSaleReceiptTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TrnsSaleReceiptTable");
            }

            bRet = bbm2xMasterManager.CreateTable(new TrnsSaleItemTable().GetCreateSQL());
            if (bRet != true)
            {
                LogWriter.ErrorLog("[Error] CreateTable -> TrnsSaleItemTable");
            }
            return bRet;
		}

        public bool TableCheck(string checkSQL)
        {
            EBM2xDBClientProvider provider = EBM2xDBClientProvider.getInstance();
            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
            {
                string DBServer = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer;
                string Database = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLDatabase;
                string DBUid = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLUid;
                string DBPwd = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLPwd;
                provider.OpenConnection(DBServer, Database, DBUid, DBPwd);
            }
            else
            {
                provider.OpenConnection("", "", "", "");
            }
            //provider.OpenConnection();

            try
            {
                if (provider.Connected())
                {
                    using (var command = provider.GetDbCommand())
                    {
                        command.CommandText = checkSQL;
                        long SalesSeq = 0;
                        var firstColumn = command.ExecuteScalar();
                        if (firstColumn != null)
                        {
                            SalesSeq = long.Parse(firstColumn.ToString());
                        }
                        if (SalesSeq > 0) return false;
                        else return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }

        public bool CreateTable(string createSQL)
        {
            EBM2xDBClientProvider provider = EBM2xDBClientProvider.getInstance();
            if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
            {
                string DBServer = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer;
                string Database = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLDatabase;
                string DBUid = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLUid;
                string DBPwd = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLPwd;
                provider.OpenConnection(DBServer, Database, DBUid, DBPwd);
            }
            else
            {
                provider.OpenConnection("", "", "", "");
            }
            //provider.OpenConnection();

            try
            {
                if (provider.Connected())
                {
                    using (var command = provider.GetDbCommand())
                    {
                        command.CommandText = createSQL;
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                return false;
            }
        }
    }
}

using System;
using System.Data;
namespace EBM2x.Database.InitSQLite
{
    using EBM2x.Database.TableIO;
    using EBM2x.Utils;

    /// <summary>
    /// Description of TrnsSaleMaster.
    /// </summary>
    public class SQLiteDatabaseInit : ModelIO
    {
        public bool UpdateTable(string valueTin, string valueBhf)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return false;
            }

            try
            {
                command.CommandType = CommandType.Text;
                IDbDataParameter param;

                param = command.CreateParameter();
                param.ParameterName = "@TIN";
                param.Value = valueTin;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@BHF_ID";
                param.Value = valueBhf;
                command.Parameters.Add(param);

                command.CommandText = " UPDATE TAXPAYER_BHF_DEVICE_USER  SET TIN = @TIN, BHF_ID = @BHF_ID ";
                command.ExecuteNonQuery();
/*
                command.CommandText = " UPDATE IMPORT_ITEM    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE STOCK_IO    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE STOCK_IO_ITEM    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE STOCK_MASTER    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TAXPAYER_BHF    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TAXPAYER_BHF_CUST    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TAXPAYER_BHF_INSURANCE    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TAXPAYER_ITEM    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TRNS_PURCHASE    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TRNS_PURCHASE_ITEM    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TRNS_SALE    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TRNS_SALE_ITEM    SET TIN = @TIN ";
                command.ExecuteNonQuery();

                command.CommandText = " UPDATE TRNS_SALE_RECEIPT    SET TIN = @TIN ";
                command.ExecuteNonQuery();
*/  
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }
            return true;
        }
    }
}

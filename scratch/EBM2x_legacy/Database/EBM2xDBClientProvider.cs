using EBM2x.Dependency;
using System.Data;
using Xamarin.Forms;

namespace EBM2x.Database
{
    public class EBM2xDBClientProvider : IEBM2xDatabaseProvider
    {
        // Singleton 패턴
        private static EBM2xDBClientProvider clientProvider = null;

        private IEBM2xDatabaseProvider databaseProvider;

        private EBM2xDBClientProvider()
        {
            /* DEBUG 20191123
               // Android tablet or smart phone (PDA)
               // WPF에서는 MsSQLite는 사용할 수 없다.
               databaseProvider = new EBM2xMsSQLiteClientProvider();
            */

            // MySQL, System.Data.SQLite (WPF)의 경우 DependencyService를 구현한다.
            // Android에서는 MySQL, System.Data.SQLite를 사용할 수 없다.
            databaseProvider = DependencyService.Get<IEBM2xDatabaseProvider>();
            if(databaseProvider == null)
            {
                // Android tablet or smart phone (PDA)
                // WPF에서는 MsSQLite는 사용할 수 없다.
                databaseProvider = new EBM2xMsSQLiteClientProvider();
            }
        }

        public static EBM2xDBClientProvider getInstance()
        {
            if (clientProvider == null) clientProvider = new EBM2xDBClientProvider();
            return clientProvider;
        }
        public void OpenConnection(string server, string database, string uid, string pwd)
        {
            databaseProvider.OpenConnection( server,  database,  uid,  pwd);
        }

        public void CloseConnection()
        {
            databaseProvider.CloseConnection();
        }

        public void Reconnect()
        {
            databaseProvider.Reconnect();
        }

        public bool Connected()
        {
            return databaseProvider.Connected();
        }

        public IDbCommand GetDbCommand()
        {
            return databaseProvider.GetDbCommand();
        }

        public void TransactionStart()
        {
            databaseProvider.TransactionStart();
        }

        public void TransactionCommit()
        {
            databaseProvider.TransactionCommit();
        }

        public void TransactionRollback()
        {
            databaseProvider.TransactionRollback();
        }
    }
}

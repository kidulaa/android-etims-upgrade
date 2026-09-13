using System;
using System.Data;

namespace EBM2x.Dependency
{
    public interface IEBM2xDatabaseProvider
    {
        // Determines if the connection to the server is active.
        bool Connected();

        void OpenConnection(string server, string database, string uid, string pwd);
        void CloseConnection();

        void Reconnect();

        // get SQLCommand from the connection
        IDbCommand GetDbCommand();

        // Method to initialize a transaction.
        void TransactionStart();

        // Method to commit a transaction.
        void TransactionCommit();

        // Method to rollback a transaction.
        void TransactionRollback();
    }
}

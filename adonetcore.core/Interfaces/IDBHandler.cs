using System;
using System.Data;

namespace adonetcore.core.Interfaces
{
    public interface IDBHandler : IDisposable
    {
        IDbConnection CreateConnection();
        void CloseConnection();
        IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection);
        IDataAdapter CreateAdapter();
        IDbDataParameter CreateParameter();
        ConnectionState CurrentConnectionState { get; }
        IDbTransaction Begin();
        void Commit();
        void Rollback();
    }
}

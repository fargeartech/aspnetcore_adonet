using adonetcore.core.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Data;

namespace adonetcore.core.Provider
{
    public sealed class SQLLiteDataProvider : IDBHandler
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private SqliteCommand _sqlcmd;
        private bool _disposed;
        private readonly string _connectionString;

        public ConnectionState CurrentConnectionState
        {
            get
            {
                if (_dbConnection == null)
                    return ConnectionState.Closed;
                return _dbConnection.State;
            }
        }

        public SQLLiteDataProvider(string connectionString)
        {
            _dbTransaction = null;
            _dbConnection = null;
            _sqlcmd = new SqliteCommand();
            _connectionString = connectionString;
        }
        public void CloseConnection()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }
        public IDbConnection CreateConnection()
        {
            //check db persistent ...
            if (_dbConnection == null && (CurrentConnectionState == ConnectionState.Broken
                || CurrentConnectionState == ConnectionState.Closed))
            {
                _dbConnection = new SqliteConnection(_connectionString);
                _dbConnection.Open();
                return _dbConnection;
            }
            return _dbConnection;
        }

        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            _sqlcmd.CommandText = commandText;
            _sqlcmd.Connection = (SqliteConnection)connection;
            _sqlcmd.CommandType = commandType;
            return _sqlcmd;
        }

        public IDataAdapter CreateAdapter()
        {
            throw new Exception();
        }

        public IDbDataParameter CreateParameter()
        {
            return _sqlcmd.CreateParameter();
        }
        public IDbTransaction Begin()
        {
            _dbTransaction = CreateConnection().BeginTransaction();
            _sqlcmd.Transaction = (SqliteTransaction)_dbTransaction;
            return _dbTransaction;
        }

        public void Commit()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Commit();
                _sqlcmd.Parameters.Clear();
                _dbTransaction.Dispose();
            }
            else
                throw new ApplicationException("Transaction is not even open");
        }
        public void Rollback()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
                _sqlcmd.Parameters.Clear();
                _dbTransaction.Dispose();
            }
            else
                throw new ApplicationException("Transaction is not even open");
        }
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }
        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dbConnection != null && CurrentConnectionState == ConnectionState.Open)
                        CloseConnection();
                    _sqlcmd.Dispose();

                    _dbTransaction = null;
                    _dbConnection = null;
                    _sqlcmd = null;
                }
                _disposed = true;
            }
        }
        /// <summary>
        /// faris ,destructor
        /// </summary>
        ~SQLLiteDataProvider()
        {
            dispose(false);
        }
    }
}
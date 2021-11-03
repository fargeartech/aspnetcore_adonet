using adonetcore.core.Interfaces;
using adonetcore.core.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace adonetcore.core.Implementations
{
    public class DBHandler : IDisposable
    {
        private IDbConnection _dbConnection;
        private IDBHandler database;
        private List<IDbDataParameter> parameters; //SO TIAP KALI USER CREATE INSTANCE ..AKAN RESET 
        private IDbCommand _dbCommand;
        private readonly string _providerName;

        public DBHandler(string connectionString, string providerName)
        {
            database = DBHandlerFactory.CreateDatabase(connectionString, providerName);
            parameters = new List<IDbDataParameter>(); //RESET
            _providerName = providerName;
        }

        public void BeginTransaction()
        {
            try
            {
                database.Begin();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("error " + ex.Message);
            }
        }

        public void Commit()
        {
            try
            {
                database.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error " + ex.Message);
            }
        }

        public void Rollback()
        {
            try
            {
                database.Rollback();
            }
            catch (Exception ex)
            {
                throw new Exception("error " + ex.Message);
            }
        }

        private IDbConnection GetDatabaseConnection()
        {
            _dbConnection = database.CreateConnection();
            return _dbConnection;
        }

        public void CreateParameter(string name, object value, DbType dbType)
        {
            parameters.Add(ParameterManager.CreateParameter(_providerName, name, value, dbType, ParameterDirection.Input));
        }

        public void CreateParameter(string name, int size, object value, DbType dbType)
        {
            parameters.Add(ParameterManager.CreateParameter(_providerName, name, size, value, dbType, ParameterDirection.Input));
        }

        public void CreateParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
        {
            parameters.Add(ParameterManager.CreateParameter(_providerName, name, size, value, dbType, direction));
        }

        public void ClearParameters()
        {
            parameters.Clear();
            if (_dbCommand != null && _dbCommand.Parameters.Count > 0)
                _dbCommand.Parameters.Clear();
        }

        public DataTable GetDataTable(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);
                return dataset.Tables[0];
            }
        }

        public DataSet GetDataSet(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);

                return dataset;
            }
        }

        public IDataReader GetDataReaderText(string commandText)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            _dbCommand = database.CreateCommand(commandText, CommandType.Text, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    _dbCommand.Parameters.Add(parameter);
                });
            }

            reader = _dbCommand.ExecuteReader();
            return reader;
        }
        public IDataReader GetDataReaderSP(string commandText)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            _dbCommand = database.CreateCommand(commandText, CommandType.StoredProcedure, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    _dbCommand.Parameters.Add(parameter);
                });
            }

            reader = _dbCommand.ExecuteReader();
            return reader;
        }

        public void Delete(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                _dbCommand.ExecuteNonQuery();
            }
        }

        public void Insert(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                _dbCommand.ExecuteNonQuery();
            }
        }

        public int Insert(string commandText, CommandType commandType, out int lastId)
        {
            lastId = 0;
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                object newId = _dbCommand.ExecuteScalar();
                lastId = Convert.ToInt32(newId);
                return lastId;
            }
        }

        public long Insert(string commandText, CommandType commandType, out long lastId)
        {
            lastId = 0;
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                object newId = _dbCommand.ExecuteScalar();
                lastId = Convert.ToInt64(newId);
                return lastId;
            }
        }

        public void InsertWithTransaction(string commandText, CommandType commandType)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {

                transactionScope = connection.BeginTransaction();

                using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            _dbCommand.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        _dbCommand.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Dispose();
                        _dbCommand.Parameters.Clear();
                    }
                }
            }
        }

        public void InsertWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {

                transactionScope = connection.BeginTransaction(isolationLevel);

                using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            _dbCommand.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        _dbCommand.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                        _dbCommand.Parameters.Clear();
                    }
                }
            }
        }

        public void Update(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                _dbCommand.ExecuteNonQuery();
            }
        }

        public void UpdateWithTransaction(string commandText, CommandType commandType)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {
                transactionScope = connection.BeginTransaction();

                using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            _dbCommand.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        _dbCommand.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public void UpdateWithTransaction(string commandText, CommandType commandType, IsolationLevel isolationLevel)
        {
            IDbTransaction transactionScope = null;
            using (var connection = GetDatabaseConnection())
            {
                transactionScope = connection.BeginTransaction(isolationLevel);

                using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
                {
                    if (parameters.Count > 0)
                    {
                        Parallel.ForEach(parameters, (parameter) =>
                        {
                            _dbCommand.Parameters.Add(parameter);
                        });
                    }

                    try
                    {
                        _dbCommand.ExecuteNonQuery();
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Change to generic. easy to use
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <param name="commandText">direct sql or stor proc name ler..</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>result int T</returns>
        public T GetScalarValue<T>(string commandText, CommandType commandType)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(commandText, commandType, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }
                var result = _dbCommand.ExecuteScalar();
                if (Convert.IsDBNull(result))
                    return default(T);
                if (result is T) // just unbox
                    return (T)result;
                else            // convert
                    return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        public DataTable ExecuteDatatableSQL(string sql)
        {
            var connection = GetDatabaseConnection();
            using (_dbCommand = database.CreateCommand(sql, CommandType.Text, connection))
            {
                if (parameters.Count > 0)
                {
                    Parallel.ForEach(parameters, (parameter) =>
                    {
                        _dbCommand.Parameters.Add(parameter);
                    });
                }

                var dataset = new DataSet();
                var dataAdaper = database.CreateAdapter();
                dataAdaper.Fill(dataset);
                return dataset.Tables[0];
            }

        }
        public IDataReader ExecuteDataReaderSQL(string commandText)
        {
            IDataReader reader = null;
            var connection = GetDatabaseConnection();
            _dbCommand = database.CreateCommand(commandText, CommandType.Text, connection);
            if (parameters.Count > 0)
            {
                Parallel.ForEach(parameters, (parameter) =>
                {
                    _dbCommand.Parameters.Add(parameter);
                });
            }
            reader = _dbCommand.ExecuteReader();

            return reader;
        }
        /// <summary>
        /// Faris.. clear all resourses!
        /// </summary>
        public void Dispose()
        {
            database.Dispose();
            if (_dbConnection != null)
                _dbConnection.Dispose();
            if (_dbCommand != null)
                _dbCommand.Dispose();
            _dbConnection = null;
            database = null;
            parameters = null;
            _dbCommand = null;
        }
    }
}

using adonetcore.core.Interfaces;
using adonetcore.core.Provider;

namespace adonetcore.core.Implementations
{
    public static class DBHandlerFactory
    {
        public static IDBHandler CreateDatabase(string connectionString, string providerName)
        {
            IDBHandler database = null;

            switch (providerName.ToLower())
            {
                case "system.data.sqlclient":
                //database = new MsSqlDataProvider(connectionStringSettings.ConnectionString);
                //break;
                case "mysql.data.mysqlclient":
                //database = new MySQLDataProvider(connectionStringSettings.ConnectionString);
                //break;
                case "system.data.sqlite":
                    database = new SQLLiteDataProvider(connectionString);
                    break;
                    //case "system.data.oleDb":
                    //    database = new OledbDataAccess(connectionStringSettings.ConnectionString);
                    //    break;
                    //case "system.data.odbc":
                    //    database = new OdbcDataAccess(connectionStringSettings.ConnectionString);
                    //    break;
            }

            return database;
        }
    }
}

using Microsoft.Data.SqlClient;

namespace SimpleSystem.DataAccess.Data
{
    public static class SqlConnectionFactory
    {
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(DataAccessSettings.ConnectionString);
        }
    }
}
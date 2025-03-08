using System.Data;
using Microsoft.Data.SqlClient;

namespace _2dRooms.Services;

public class SqlService
{
    protected readonly string sqlConnectionString;

    protected SqlService(string connectionString)
    {
        sqlConnectionString = connectionString;
    }

    protected IDbConnection CreateConnection()
    {
        return new SqlConnection(sqlConnectionString);
    }
}


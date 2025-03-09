using System.Data;
using Microsoft.Data.SqlClient;

namespace _2dRooms.Services;

public abstract class SqlService
{
    protected readonly string sqlConnectionString;

    // Constructor that receives the connection string
    protected SqlService(ConnectionStringService connectionStringService)
    {
        // Get the connection string from the injected service
        sqlConnectionString = connectionStringService.GetConnectionString();
    }

    protected IDbConnection CreateConnection()
    {
        return new SqlConnection(sqlConnectionString);
    }
}

using Dapper;
using _2dRooms.Models;
using _2dRooms.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace _2dRooms.Repositories;

public class Environment2DRepository : SqlService, IEnvironment2DRepository
{
    public Environment2DRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<Environment2D>> GetAllAsync(string userId)
    {
        var query = "SELECT * FROM Environment2Ds WHERE UserId = @UserId";
        using (var dbConnection = CreateConnection())
        {
            return await dbConnection.QueryAsync<Environment2D>(query, new { UserId = userId });
        }
    }

    public async Task<Environment2D> GetByIdAsync(string id)
    {
        var query = "SELECT * FROM Environment2Ds WHERE Id = @Id";
        using (var dbConnection = CreateConnection())
        {
            return await dbConnection.QueryFirstOrDefaultAsync<Environment2D>(query, new { Id = id });
        }
    }

    public async Task CreateAsync(Environment2D environment)
    {
        var query = "INSERT INTO Environment2Ds (Id, Name, MaxHeight, MaxWidth, UserId) " +
                    "VALUES (@Id, @Name, @MaxHeight, @MaxWidth, @UserId)";
        using (var dbConnection = CreateConnection())
        {
            await dbConnection.ExecuteAsync(query, environment);
        }
    }

    public async Task UpdateAsync(Environment2D environment)
    {
        var query = "UPDATE Environment2Ds SET Name = @Name, MaxHeight = @MaxHeight, MaxWidth = @MaxWidth " +
                    "WHERE Id = @Id";
        using (var dbConnection = CreateConnection())
        {
            await dbConnection.ExecuteAsync(query, environment);
        }
    }

    public async Task DeleteAsync(string id)
    {
        var query = "DELETE FROM Environment2Ds WHERE Id = @Id";
        using (var dbConnection = CreateConnection())
        {
            await dbConnection.ExecuteAsync(query, new { Id = id });
        }
    }
}

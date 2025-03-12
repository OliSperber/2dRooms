using Dapper;
using _2dRooms.Models;
using _2dRooms.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace _2dRooms.Repositories;

public class Object2DRepository : SqlService, IObject2DRepository
{
    public Object2DRepository(ConnectionStringService connectionStringService) : base(connectionStringService)
    {
    }

    // Get all objects for a specific environment
    public async Task<IEnumerable<Object2D>> GetObjectsByEnvironmentIdAsync(string environmentId)
    {
        var query = "SELECT * FROM Object2D WHERE EnvironmentId = @EnvironmentId";
        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Object2D>(query, new { EnvironmentId = environmentId });
        }
    }

    // Get a specific object by its ID
    public async Task<Object2D> GetObjectByIdAsync(string id)
    {
        var query = "SELECT * FROM Object2D WHERE Id = @Id";
        using (var connection = CreateConnection())
        {
            return await connection.QueryFirstOrDefaultAsync<Object2D>(query, new { Id = id });
        }
    }

    // Create a new Object2D
    public async Task CreateObjectAsync(Object2D object2D)
    {
        var query = "INSERT INTO Object2D (Id, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId) " +
                    "VALUES (@Id, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @EnvironmentId)";
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(query, object2D);
        }
    }

    // Update an existing Object2D
    public async Task UpdateObjectAsync(Object2D object2D)
    {
        var query = "UPDATE Object2D SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, " +
                    "ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer " +
                    "WHERE Id = @Id";
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(query, object2D);
        }
    }

    // Delete an object by its ID
    public async Task DeleteObjectAsync(string id)
    {
        var query = "DELETE FROM Object2D WHERE Id = @Id";
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }

    // Delete objects by environmentID
    public async Task DeleteObjectsByEnvironmentIdAsync(string environmentId)
    {
        var query = "DELETE FROM Object2D WHERE EnvironmentId = @EnvironmentId";
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(query, new { EnvironmentId = environmentId });
        }
    }
}


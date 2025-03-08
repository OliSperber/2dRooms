using Dapper;
using _2dRooms.Models;
using _2dRooms.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;

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
        // Validate the environment (name, max size, and uniqueness for creation)
        await ValidateEnvironment(environment, true);

        // If validation passes, insert the new environment
        var insertQuery = "INSERT INTO Environment2Ds (Id, Name, MaxHeight, MaxWidth, UserId) " +
                          "VALUES (@Id, @Name, @MaxHeight, @MaxWidth, @UserId)";
        using (var dbConnection = CreateConnection())
        {
            await dbConnection.ExecuteAsync(insertQuery, environment);
        }
    }

    public async Task UpdateAsync(Environment2D environment)
    {
        // Validate the environment (name, max size, and uniqueness for creation)
        await ValidateEnvironment(environment, false);

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

    public void ValidateNameAndSize(Environment2D environment)
    {
        if (string.IsNullOrEmpty(environment.Name) || environment.Name.Length < 1 || environment.Name.Length > 25)
        {
            throw new ArgumentException("The environment name must be between 1 and 25 characters.");
        }

        // Validate the maximum size
        if (environment.MaxWidth < 20 || environment.MaxWidth > 200 || environment.MaxHeight < 10 || environment.MaxHeight > 100)
        {
            throw new ArgumentException("The maximum width must be an integer between 20 and 200, and the maximum height must be an integer between 10 and 100.");
        }
    }
    public async Task ValidateEnvironment(Environment2D environment, bool isCreation)
    {
        ValidateNameAndSize(environment);

        if (isCreation)
        {
            // Check if the environment name already exists for the same user
            var checkNameQuery = "SELECT COUNT(*) FROM Environment2Ds WHERE Name = @Name AND UserId = @UserId";
            using (var dbConnection = CreateConnection())
            {
                var existingEnvironmentCount = await dbConnection.ExecuteScalarAsync<int>(checkNameQuery, new { Name = environment.Name, UserId = environment.UserId });

                if (existingEnvironmentCount > 0)
                {
                    throw new ArgumentException("An environment with the same name already exists for this user.");
                }

                // First, check how many environments are in the database
                var countQuery = "SELECT COUNT(*) FROM Environment2Ds";
                var environmentCount = await dbConnection.ExecuteScalarAsync<int>(countQuery);

                // If there are 5 or more environments, throw an exception
                if (environmentCount >= 5)
                {
                    throw new InvalidOperationException("Cannot create a new environment. The maximum number of environments (5) has already been reached.");
                }
            }
        }
        else
        {
            // For update: check if the name already exists for another environment (excluding the current environment being updated)
            var checkNameQuery = "SELECT COUNT(*) FROM Environment2Ds WHERE Name = @Name AND UserId = @UserId AND Id != @Id";
            using (var dbConnection = CreateConnection())
            {
                var existingEnvironmentCount = await dbConnection.ExecuteScalarAsync<int>(checkNameQuery, new { Name = environment.Name, UserId = environment.UserId, Id = environment.Id });

                if (existingEnvironmentCount > 0)
                {
                    throw new ArgumentException("An environment with the same name already exists for this user.");
                }
            }
        }
    }

}

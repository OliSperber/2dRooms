using _2dRooms.Models;

namespace _2dRooms.Repositories;
public interface IEnvironment2DRepository
{
    Task<IEnumerable<Environment2D>> GetAllAsync(string userId);
    Task<Environment2D> GetByIdAsync(string id);
    Task CreateAsync(Environment2D environment);
    Task UpdateAsync(Environment2D environment);
    Task DeleteAsync(string id);
}


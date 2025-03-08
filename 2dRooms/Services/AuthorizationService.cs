using _2dRooms.Repositories;
using Microsoft.AspNetCore.Identity;

namespace _2dRooms.Services;

public class AuthorizationService
{
    private readonly IEnvironment2DRepository _environmentRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager;

    // Constructor for dependency injection
    public AuthorizationService(IEnvironment2DRepository environmentRepository,
                                IHttpContextAccessor httpContextAccessor,
                                UserManager<IdentityUser> userManager)
    {
        _environmentRepository = environmentRepository;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    // Extract the user ID from the JWT token using the "sub" claim
    private string GetUserIdFromToken()
    {
        // Directly retrieve the "sub" claim for user ID
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
        return userId;
    }

    // Check if the user exists in the system
    private async Task<bool> IsUserExistsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return false; // If no userId, user doesn't exist
        }

        var user = await _userManager.FindByIdAsync(userId);
        return user != null;
    }

    // Check if the current user is authorized for the specific environment
    public async Task<bool> IsUserAuthorizedForEnvironmentAsync(string environmentId)
    {
        var userId = GetUserIdFromToken();
        if (userId == null || !await IsUserExistsAsync(userId))
        {
            return false; // No userId or user does not exist
        }

        var environment = await _environmentRepository.GetByIdAsync(environmentId);
        return environment != null && environment.UserId == userId;
    }

    // Check if the current user is authorized for a specific Object2D
    public async Task<bool> IsUserAuthorizedForObjectAsync(string environmentId, string objectId)
    {
        // Example: You might want to check if the user has access to a specific object
        return await IsUserAuthorizedForEnvironmentAsync(environmentId); // Customize this if needed
    }
}

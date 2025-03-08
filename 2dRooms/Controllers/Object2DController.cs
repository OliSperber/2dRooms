using _2dRooms.Models;
using _2dRooms.Repositories;
using Microsoft.AspNetCore.Mvc;
using _2dRooms.Services;
using System.Threading.Tasks;

namespace _2dRooms.Controllers;

[Route("api/environment2d/{environmentId}/object2d")]
[ApiController]
public class Object2DController : ControllerBase
{
    private readonly IObject2DRepository _object2DRepository;
    private readonly AuthorizationService _authorizationService;

    // Constructor for dependency injection
    public Object2DController(IObject2DRepository object2DRepository, AuthorizationService authorizationService)
    {
        _object2DRepository = object2DRepository;
        _authorizationService = authorizationService;
    }

    // GET: api/environment2d/{environmentId}/object2d
    [HttpGet]
    public async Task<IActionResult> GetObjectsByEnvironmentId(string environmentId)
    {
        var userId = User.FindFirst("sub")?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForEnvironmentAsync(environmentId))
        {
            return Forbid(); // If the user is not authorized for this environment
        }

        var objects = await _object2DRepository.GetObjectsByEnvironmentIdAsync(environmentId);
        if (objects == null || !objects.Any())
        {
            return NotFound("No objects found in this environment.");
        }

        return Ok(objects);
    }

    // GET: api/environment2d/{environmentId}/object2d/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetObjectById(string environmentId, string id)
    {
        var userId = User.FindFirst("sub")?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForObjectAsync(environmentId, id))
        {
            return Forbid(); // If the user is not authorized for this object
        }

        var object2D = await _object2DRepository.GetObjectByIdAsync(id);
        if (object2D == null || object2D.EnvironmentId != environmentId)
        {
            return NotFound();
        }

        return Ok(object2D);
    }

    // POST: api/environment2d/{environmentId}/object2d
    [HttpPost]
    public async Task<IActionResult> CreateObject(string environmentId, [FromBody] Object2D object2D)
    {
        var userId = User.FindFirst("sub")?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForEnvironmentAsync(environmentId))
        {
            return Forbid(); // If the user is not authorized to create object in this environment
        }

        object2D.EnvironmentId = environmentId; // Ensure the object is linked to the correct environment
        await _object2DRepository.CreateObjectAsync(object2D);

        return CreatedAtAction(nameof(GetObjectById), new { environmentId, id = object2D.Id }, object2D);
    }

    // PUT: api/environment2d/{environmentId}/object2d/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateObject(string environmentId, string id, [FromBody] Object2D object2D)
    {
        var userId = User.FindFirst("sub")?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForObjectAsync(environmentId, id))
        {
            return Forbid(); // If the user is not authorized to update this object
        }

        var existingObject = await _object2DRepository.GetObjectByIdAsync(id);
        if (existingObject == null || existingObject.EnvironmentId != environmentId)
        {
            return NotFound();
        }

        await _object2DRepository.UpdateObjectAsync(object2D);
        return NoContent();
    }

    // DELETE: api/environment2d/{environmentId}/object2d/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteObject(string environmentId, string id)
    {
        var userId = User.FindFirst("sub")?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForObjectAsync(environmentId, id))
        {
            return Forbid(); // If the user is not authorized to delete this object
        }

        var existingObject = await _object2DRepository.GetObjectByIdAsync(id);
        if (existingObject == null || existingObject.EnvironmentId != environmentId)
        {
            return NotFound();
        }

        await _object2DRepository.DeleteObjectAsync(id);
        return NoContent();
    }
}

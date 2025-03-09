using _2dRooms.Models;
using _2dRooms.Repositories;
using Microsoft.AspNetCore.Mvc;
using _2dRooms.Services;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace _2dRooms.Controllers;

[Route("api/environment2d")]
[ApiController]
public class Environment2DController : ControllerBase
{
    private readonly IEnvironment2DRepository _environmentRepository;
    private readonly AuthorizationService _authorizationService;

    // Constructor for dependency injection
    public Environment2DController(IEnvironment2DRepository environmentRepository, AuthorizationService authorizationService)
    {
        _environmentRepository = environmentRepository;
        _authorizationService = authorizationService;
    }

    // GET: api/environment2d
    [HttpGet]
    public async Task<IActionResult> GetAllEnvironments()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        // Get all environments that belong to the user
        var environments = await _environmentRepository.GetAllAsync(userId);

        if (environments == null || !environments.Any())
        {
            return NotFound("No environments found for this user.");
        }

        return Ok(environments);
    }

    // GET: api/environment2d/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEnvironmentById(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForEnvironmentAsync(id))
        {
            return Forbid(); // If the user is not authorized for this environment
        }

        var environment = await _environmentRepository.GetByIdAsync(id);
        if (environment == null)
        {
            return NotFound();
        }

        return Ok(environment);
    }

    // POST: api/environment2d
    [HttpPost]
    public async Task<IActionResult> CreateEnvironment([FromBody] Environment2D environment)
    {


        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForEnvironmentAsync(environment.Id))
        {
            return Forbid(); // If the user is not authorized to create this environment
        }

        try
        {
            await _environmentRepository.CreateAsync(environment);
            return CreatedAtAction(nameof(GetEnvironmentById), new { id = environment.Id }, environment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // Send error message to the client
        }
    }

    // PUT: api/environment2d/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEnvironment(string id, [FromBody] Environment2D environment)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForEnvironmentAsync(id))
        {
            return Forbid(); // If the user is not authorized to update this environment
        }

        var existingEnvironment = await _environmentRepository.GetByIdAsync(id);
        if (existingEnvironment == null)
        {
            return NotFound();
        }

        await _environmentRepository.UpdateAsync(environment);
        return NoContent();
    }

    // DELETE: api/environment2d/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnvironment(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the JWT token

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated.");
        }

        if (!await _authorizationService.IsUserAuthorizedForEnvironmentAsync(id))
        {
            return Forbid(); // If the user is not authorized to delete this environment
        }

        var existingEnvironment = await _environmentRepository.GetByIdAsync(id);
        if (existingEnvironment == null)
        {
            return NotFound();
        }

        await _environmentRepository.DeleteAsync(id);
        return NoContent();
    }
}

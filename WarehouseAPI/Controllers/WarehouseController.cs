using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class WarehouseController : ControllerBase
{
    private readonly ILogger<WarehouseController> _logger;
    
    public WarehouseController(ILogger<WarehouseController> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Get warehouse inventory (requires any authenticated user)
    /// </summary>
    [HttpGet("inventory")]
    [Authorize(Policy = "ViewerOrHigher")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult GetInventory()
    {
        var inventory = new
        {
            Items = new[]
            {
                new { Id = 1, Name = "Widget A", Quantity = 100, Location = "A1" },
                new { Id = 2, Name = "Widget B", Quantity = 50, Location = "B2" },
                new { Id = 3, Name = "Widget C", Quantity = 75, Location = "C3" }
            },
            LastUpdated = DateTime.UtcNow
        };
        
        return Ok(inventory);
    }
    
    /// <summary>
    /// Add new inventory item (requires User role or higher)
    /// </summary>
    [HttpPost("inventory")]
    [Authorize(Policy = "UserOrAdmin")]
    [ProducesResponseType(201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult AddInventoryItem([FromBody] object item)
    {
        // Simulate adding item
        var newItem = new
        {
            Id = 4,
            Name = "New Widget",
            Quantity = 25,
            Location = "D4",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name
        };
        
        _logger.LogInformation("Inventory item added by {User}", User.Identity?.Name);
        
        return CreatedAtAction(nameof(GetInventory), new { id = newItem.Id }, newItem);
    }
    
    /// <summary>
    /// Delete inventory item (requires Admin role)
    /// </summary>
    [HttpDelete("inventory/{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public IActionResult DeleteInventoryItem(int id)
    {
        // Simulate deletion
        _logger.LogWarning("Inventory item {Id} deleted by admin {User}", id, User.Identity?.Name);
        
        return NoContent();
    }
    
    /// <summary>
    /// Get system statistics (requires Admin role)
    /// </summary>
    [HttpGet("admin/stats")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult GetSystemStats()
    {
        var stats = new
        {
            TotalItems = 225,
            TotalUsers = 15,
            SystemUptime = TimeSpan.FromDays(30),
            LastBackup = DateTime.UtcNow.AddHours(-6),
            AdminUser = User.Identity?.Name
        };
        
        return Ok(stats);
    }
}
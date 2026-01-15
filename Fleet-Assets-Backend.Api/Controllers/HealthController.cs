using Fleet_Assets_Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Fleet_Assets_Backend.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController(FleetAssetsDbContext db, ILogger<HealthController> logger) : ControllerBase
{
    private readonly FleetAssetsDbContext _db = db;
    private readonly ILogger<HealthController> _logger = logger;

    /// <summary>
    /// Liveness: app is running (no dependency checks).
    /// </summary>
    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new
        {
            status = "ok",
            check = "live",
            utc = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Readiness: app can reach its critical dependencies (DB).
    /// </summary>
    [HttpGet("ready")]
    public async Task<IActionResult> Ready(CancellationToken ct)
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(ct);
            if (!canConnect)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "unhealthy",
                    check = "ready",
                    dependency = "database",
                    utc = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                status = "ok",
                check = "ready",
                dependency = "database",
                utc = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "unhealthy",
                check = "ready",
                dependency = "database",
                error = ex.GetType().Name,
                utc = DateTime.UtcNow
            });
        }
    }
}

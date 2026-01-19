using Fleet_Assets_Backend.Application.Dtos.Vehicle;
using Fleet_Assets_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fleet_Assets_Backend.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController(IVehicleService service) : ControllerBase
{
    private readonly IVehicleService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VehicleDto>>> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleRequest request, CancellationToken ct)
    {
        var correlationId = GetOrCreateCorrelationId();
        var created = await _service.CreateAsync(request, correlationId, actor: "system", ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleDto>> Update([FromRoute] Guid id, [FromBody] UpdateVehicleRequest request, CancellationToken ct)
    {
        var correlationId = GetOrCreateCorrelationId();
        var updated = await _service.UpdateAsync(id, request, correlationId, actor: "system", ct);

        return Ok(updated);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleDto>> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeVehicleStatusRequest request, CancellationToken ct)
    {
        var correlationId = GetOrCreateCorrelationId();
        var updated = await _service.ChangeStatusAsync(id, request, correlationId, actor: "system", ct);

        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var correlationId = GetOrCreateCorrelationId();
        var deleted = await _service.DeleteAsync(id, correlationId, actor: "system", ct);

        return deleted ? NoContent() : NotFound();
    }

    private Guid GetOrCreateCorrelationId()
    {
        if (Request.Headers.TryGetValue("X-Correlation-Id", out var values) &&
            Guid.TryParse(values.FirstOrDefault(), out var parsed))
        {
            return parsed;
        }

        return Guid.NewGuid();
    }
}

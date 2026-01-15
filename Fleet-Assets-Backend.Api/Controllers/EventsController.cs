using Fleet_Assets_Backend.Application.Dtos.EventLog;
using Fleet_Assets_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fleet_Assets_Backend.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(IEventQueryService service) : ControllerBase
{
    private readonly IEventQueryService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(List<EventLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventLogDto>>> Get([FromQuery] GetEventsQuery query, CancellationToken ct)
    {
        var result = await _service.GetAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventLogDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }
}

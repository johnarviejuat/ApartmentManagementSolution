using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public sealed class ApartmentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApartmentCommand cmd)
    {
        var id = await _sender.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApartmentDto>> GetById(Guid id)
    {
        var dto = await _sender.Send(new GetApartmentQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }
}

using Catalog.Application.Apartments;
using Catalog.Application.Apartments.Commands.AssignOwner;
using Catalog.Application.Apartments.Commands.Create;
using Catalog.Application.Apartments.Commands.Delete;
using Catalog.Application.Apartments.Commands.Update;
using Catalog.Application.Common;
using Catalog.Application.Common.Request;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ApartmentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApartmentCommand cmd, CancellationToken ct)
    {
        try
        {
            var id = await _sender.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (FluentValidation.ValidationException ex)
        {
            return ValidationProblem(ex.ToProblemDetails());
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ApartmentRequest dto, CancellationToken ct)
    {
        try
        {
            var cmd = new UpdateApartmentCommand(
                id,
                dto.Name,
                dto.UnitNumber,
                dto.Line1,
                dto.City,
                dto.State,
                dto.PostalCode,
                dto.Bedrooms,
                dto.Bathrooms,
                dto.Capacity,
                dto.MonthlyRent,
                dto.AdvanceRent,
                dto.SecurityDeposit,
                dto.SquareFeet,
                dto.AvailableFrom,
                dto.Description,
                dto.Status);

            var ok = await _sender.Send(cmd, ct);
            return ok ? NoContent() : NotFound();
        }
        catch (FluentValidation.ValidationException ex)
        {
            return ValidationProblem(ex.ToProblemDetails());
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _sender.Send(new DeleteApartmentCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApartmentDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _sender.Send(new GetApartmentQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ApartmentDto>>> GetAll(CancellationToken ct)
    {
        var dtos = await _sender.Send(new GetAllApartmentsQuery(), ct);
        return Ok(dtos);
    }

    [HttpPut("assign")]
    public async Task<IActionResult> AssignOwner([FromBody] AssignOwnerToApartmentCommand cmd, CancellationToken ct)
    {
        try
        {
            await _sender.Send(cmd, ct);
            return NoContent();
        }
        catch (FluentValidation.ValidationException ex)
        {
            return ValidationProblem(ex.ToProblemDetails());
        }
        catch (KeyNotFoundException nf)
        {
            return Problem(title: nf.Message, statusCode: StatusCodes.Status404NotFound);
        }
    }
}
file static class ValidationExtensions
{
    public static ValidationProblemDetails ToProblemDetails(this FluentValidation.ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest
        };
    }
}

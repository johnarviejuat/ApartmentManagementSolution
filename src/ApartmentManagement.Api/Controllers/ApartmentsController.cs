using ApartmentManagement.Application.Apartments;
using ApartmentManagement.Application.Apartments.Commands.AssignOwner;
using ApartmentManagement.Application.Apartments.Commands.Create;
using ApartmentManagement.Application.Apartments.Commands.Delete;
using ApartmentManagement.Application.Apartments.Commands.Update;
using ApartmentManagement.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.Api.Controllers
{
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
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return ValidationProblem(new ValidationProblemDetails(errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApartmentDto>> GetById(Guid id)
        {
            var dto = await _sender.Send(new GetApartmentQuery(id));
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ApartmentDto>>> GetAll()
        {
            var dtos = await _sender.Send(new GetAllApartmentsQuery());
            return Ok(dtos);
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
                dto.Address,
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
                var result = await _sender.Send(cmd, ct);
                return result ? NoContent() : NotFound();
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return ValidationProblem(new ValidationProblemDetails(errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _sender.Send(new DeleteApartmentCommand(id), ct);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{apartmentId:guid}/owner/{ownerId:guid}")]
        public async Task<IActionResult> AssignOwner(Guid apartmentId, Guid ownerId, CancellationToken ct)
        {
            try
            {
                await _sender.Send(new AssignOwnerToApartmentCommand(ownerId, apartmentId), ct);
                return NoContent();
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return ValidationProblem(new ValidationProblemDetails(errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (KeyNotFoundException nf)
            {
                return Problem(title: nf.Message, statusCode: StatusCodes.Status404NotFound);
            }
        }
    }
}


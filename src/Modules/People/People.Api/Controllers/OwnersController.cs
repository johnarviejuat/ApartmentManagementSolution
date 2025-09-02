
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using People.Application.Common;
using People.Application.Common.Request;
using People.Application.Owners;
using People.Application.Owners.Commands.Create;
using People.Application.Owners.Commands.Delete;
using People.Application.Owners.Commands.Update;

namespace People.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class OwnersController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOwnerCommand cmd, CancellationToken ct)
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OwnerRequest dto, CancellationToken ct)
        {
            try
            {
                var cmd = new UpdateOwnerCommand(
                    id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
                    dto.MailingLine1,
                    dto.MailingCity,
                    dto.MailingState,
                    dto.MailingPostalCode,
                    dto.Notes
                );
                var ok = await _sender.Send(cmd, ct);
                return ok ? NoContent() : NotFound();
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
            try
            {
                await _sender.Send(new DeleteOwnerCommand(id), ct);
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

        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OwnerDto>> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _sender.Send(new GetOwnerQuery(id), ct);
            if (dto is null)
            {
                return NotFound();
            }
            return Ok(dto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> GetAll(CancellationToken ct)
        {
            var dtos = await _sender.Send(new GetAllOwnerQuery(), ct);
            return Ok(dtos);
        }
    }
}

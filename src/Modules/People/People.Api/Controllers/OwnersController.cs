
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using People.Application.Common;
using People.Application.Owners;
using People.Application.Owners.Commands.Create;

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
    }
}

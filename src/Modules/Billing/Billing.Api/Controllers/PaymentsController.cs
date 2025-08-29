using Billing.Application.Common;
using Billing.Application.Payments;
using Billing.Application.Payments.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class PaymentsController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentCommand cmd, CancellationToken ct)
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
        public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _sender.Send(new GetPaymentQuery(id), ct);
            return dto is null ? NotFound() : Ok(dto);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using People.Application.Common;
using People.Application.Common.Request;
using People.Application.Tenants;
using People.Application.Tenants.Commands.AssignToApartment;
using People.Application.Tenants.Commands.Create;
using People.Application.Tenants.Commands.RemoveToApartment;
using People.Application.Tenants.Commands.RenewToApartment;
using People.Domain.Entities;

namespace People.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TenantsController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTenantCommand cmd, CancellationToken ct)
        {
            try
            {
                var id = await _sender.Send(cmd, ct);
                return CreatedAtAction(nameof(GetById), new { id }, id);
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
        public async Task<ActionResult<TenantDto>> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _sender.Send(new GetTenantQuery(id), ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AssignToApartment([FromBody] AssignTenantToApartmentCommand cmd, CancellationToken ct)
        {
            try
            {
                await _sender.Send(cmd, ct);
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

        [HttpPut("renew")]
        public async Task<IActionResult> RenewToApartment([FromBody] RenewLeaseRequest req, CancellationToken ct) {
            try
            {
                await _sender.Send(new RenewToaApartmentCommand(
                     TenantId: req.TenantId,
                     ApartmentId: req.ApartmentId,
                     NewStartDate: req.NewStartDate,
                     NewMonthlyRent: req.NewMonthlyRent,
                     NewDepositRequired: req.NewDepositRequired,
                     CarryOverCredit: req.CarryOverCredit,
                     CarryOverDeposit: req.CarryOverDeposit
                 ), ct);
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

        [HttpDelete("terminate")]
        public async Task<IActionResult> RemoveFromApartment([FromQuery] Guid tenantId, [FromQuery] Guid apartmentId, [FromQuery] TenantStatus tenantStatus, CancellationToken ct)
        {
            try
            {
                await _sender.Send(new RemoveTenantFromApartmentCommand(tenantId, apartmentId, tenantStatus), ct);
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
            catch (Exception ex)
            {
                return Problem(title: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}

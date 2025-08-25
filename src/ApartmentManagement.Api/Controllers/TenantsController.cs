using ApartmentManagement.Application.Common;
using ApartmentManagement.Application.Tenants;

//!COMMANDS
using ApartmentManagement.Application.Tenants.Commands.AssignToApartment;
using ApartmentManagement.Application.Tenants.Commands.Create;
using ApartmentManagement.Application.Tenants.Commands.RemoveToApartment;
using ApartmentManagement.Domain.Leasing.Tenants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.Api.Controllers
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

        [HttpPut("{tenantId:guid}/apartment/{apartmentId:guid}")]
        public async Task<IActionResult> AssignToApartment(Guid tenantId, Guid apartmentId, CancellationToken ct)
        {
            try
            {
                await _sender.Send(new AssignTenantToApartmentCommand(tenantId, apartmentId), ct);
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

        [HttpDelete("{tenantId:guid}/apartment/{apartmentId:guid}")]
        public async Task<IActionResult> RemoveFromApartment(Guid tenantId, Guid apartmentId, [FromQuery] TenantStatus tenantStatus, CancellationToken ct)
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

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using People.Application.Common;
using People.Application.Common.Request;
using People.Application.Tenants;
using People.Application.Tenants.Commands.AssignToApartment;
using People.Application.Tenants.Commands.Create;
using People.Application.Tenants.Commands.Delete;
using People.Application.Tenants.Commands.RemoveToApartment;
using People.Application.Tenants.Commands.RenewToApartment;
using People.Application.Tenants.Commands.Update;
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TenantRequest dto, CancellationToken ct)
        {
            try
            {
                var cmd = new UpdateTenantCommand(
                    id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
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
                var ok = await _sender.Send(new DeleteTenantCommand(id), ct);
                return ok ? NoContent() : NotFound();
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var pd = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                };
                return BadRequest(pd);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TenantDto>> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _sender.Send(new GetTenantQuery(id), ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll(CancellationToken ct)
        {
            var dtos = await _sender.Send(new GetAllTenantQuery(), ct);
            return Ok(dtos);
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

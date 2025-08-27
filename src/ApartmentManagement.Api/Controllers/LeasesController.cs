using ApartmentManagement.Application.Apartments;
using ApartmentManagement.Application.Common;
using ApartmentManagement.Application.Leases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class LeasesController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LeaseDto>>> GetAll()
        {
            var dtos = await _sender.Send(new GetAllLeaseQuery());
            return Ok(dtos);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<LeaseDto>> GetById(Guid id)
        {
            var dto = await _sender.Send(new GetLeaseAgreementQuery(id));
            return dto is null ? NotFound() : Ok(dto);
        }
    }
}

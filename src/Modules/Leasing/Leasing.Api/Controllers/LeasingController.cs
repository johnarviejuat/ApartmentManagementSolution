using Leasing.Application.Common;
using Leasing.Application.Leases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Leasing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class LeasingController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;


        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<LeaseDto>> GetById(Guid id)
        {
            var dto = await _sender.Send(new GetLeaseAgreementQuery(id));
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LeaseDto>>> GetAll()
        {
            var dtos = await _sender.Send(new GetAllLeaseQuery());
            return Ok(dtos);
        }

    }
}

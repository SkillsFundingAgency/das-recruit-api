using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Queries;

namespace SFA.DAS.Recruit.Api.Controllers
{
    [Route("api/status")]
    public class OrganisationStatusController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public OrganisationStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/status/provider/11111111
        [HttpGet("provider/{ukprn:long:min(10000000)}")]
        public async Task<IActionResult> Get(long ukprn)
        {
            var resp = await _mediator.Send(new GetOrganisationStatusQuery(ukprn));
            return GetApiResponse(resp);
        }
    }
}

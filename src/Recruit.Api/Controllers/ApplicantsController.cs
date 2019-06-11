using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Queries;

namespace SFA.DAS.Recruit.Api.Controllers
{
    [Route("api/vacancies/{vacancyReference:long}/[controller]")]
    public class ApplicantsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/vacancies/{vacancyReference}/applicants?applicantApplicationOutcomeFilter
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute]ulong vacancyReference, [FromQuery]string applicantApplicationOutcomeFilter)
        {
            var resp = await _mediator.Send(new GetApplicantsQuery((long)vacancyReference, applicantApplicationOutcomeFilter?.Trim()));
            return GetApiResponse(resp);
        }
    }
}
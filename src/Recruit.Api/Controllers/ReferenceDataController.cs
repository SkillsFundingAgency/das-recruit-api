using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.ReferenceData;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.ReferenceData}")]
public class ReferenceDataController: ControllerBase
{
    [HttpGet]
    [Route("candidate-skills")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public IResult GetCandidateSkills()
    {
        return TypedResults.Ok(Reference.CandidateSkills);
    }

    [HttpGet]
    [Route("candidate-qualifications")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public IResult GetCandidateQualifications()
    {
        return TypedResults.Ok(Reference.CandidateQualifications);
    }
}
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.ReferenceData}")]
public class ReferenceDataController: ControllerBase
{
    private static readonly List<string> CandidateSkills = [
        "Communication skills",
        "IT skills",
        "Attention to detail",
        "Organisation skills",
        "Customer care skills",
        "Problem solving skills",
        "Presentation skills",
        "Administrative skills",
        "Number skills",
        "Analytical skills",
        "Logical",
        "Team working",
        "Creative",
        "Initiative",
        "Non judgemental",
        "Patience",
        "Physical fitness"
    ];

    private static readonly List<string> CandidateQualifications = [
        "GCSE",
        "A Level",
        "T Level",
        "BTEC",
        "Degree",
        "Other"
    ];

    [HttpGet]
    [Route("candidate-skills")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public IResult GetCandidateSkills()
    {
        return TypedResults.Ok(CandidateSkills);
    }

    [HttpGet]
    [Route("candidate-qualifications")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public IResult GetCandidateQualifications()
    {
        return TypedResults.Ok(CandidateQualifications);
    }
}
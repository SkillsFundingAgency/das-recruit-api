using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.VacancyReference}")]
public class VacancyReferenceController
{
    [HttpGet]
    [ProducesResponseType(typeof(VacancyReferenceResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetNextVacancyReference([FromServices] IVacancyRepository repository, CancellationToken cancellationToken)
    {
        var result = await repository.GetNextVacancyReferenceAsync(cancellationToken);
        return Results.Ok(new VacancyReferenceResponse(result.Value));
    }
}
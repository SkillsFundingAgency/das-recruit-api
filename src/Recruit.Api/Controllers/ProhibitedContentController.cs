using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Data.ProhibitedContent;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route("api/[controller]/{contentType}")]
public class ProhibitedContentController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetAllByType(
        [FromServices] IProhibitedContentRepository repository,
        [FromRoute] ProhibitedContentType contentType,
        CancellationToken cancellationToken)
    {
        var results = await repository.GetByContentTypeAsync(contentType.ToDomain(), cancellationToken);
        return TypedResults.Ok(results.Select(x => x.Content));
    }
}
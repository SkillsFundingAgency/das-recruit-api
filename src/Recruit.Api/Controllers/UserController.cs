using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.User;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.User}/{{id:guid}}")]
public class UserController
{
    [HttpPut]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IUserRepository repository,
        [FromRoute] Guid id,
        [FromBody] PutUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(id), cancellationToken);

        return result.Created
            ? TypedResults.Created($"/{RouteNames.User}/{result.Entity.Id}", result.Entity.ToPutResponse())
            : TypedResults.Ok(result.Entity.ToPutResponse());
    }
}
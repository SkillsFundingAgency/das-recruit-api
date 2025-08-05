using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.Models.Responses.User;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.User}/{{id:guid}}")]
public class UserController
{
    [HttpGet]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IUserRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(id, cancellationToken);
        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }
    
    [HttpGet, Route($"~/{RouteElements.Api}/employerAccounts/{{employerAccountId}}")]
    [ProducesResponseType(typeof(List<RecruitUser>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAllByEmployerAccountId(
        [FromServices] IUserRepository repository,
        [FromRoute] string employerAccountId,
        CancellationToken cancellationToken)
    {
        var result = await repository.FindUsersByEmployerAccountIdAsync(employerAccountId, cancellationToken);
        return TypedResults.Ok(result.Select(x => x.ToGetResponse()));
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(PutUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PutUserResponse), StatusCodes.Status201Created)]
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
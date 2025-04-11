using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.EmployerProfile}/{{accountLegalEntityId:long}}")]
public class EmployerProfileController: ControllerBase
{
    [HttpGet, Route($"~/{RouteNames.Employer}/{{accountId:long}}/{RouteElements.EmployerProfiles}")]
    [ProducesResponseType(typeof(IEnumerable<EmployerProfile>), StatusCodes.Status200OK)]
    public async Task<IResult> GetMany(
        [FromServices] IEmployerProfileRepository repository,
        [FromRoute] long accountId,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyForAccountAsync(accountId, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet]
    [ProducesResponseType(typeof(EmployerProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IEmployerProfileRepository repository,
        [FromRoute] long accountLegalEntityId,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(accountLegalEntityId, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpPut]
    [ProducesResponseType(typeof(PutEmployerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PutEmployerProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IEmployerProfileRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromBody] PutEmployerProfileRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(accountLegalEntityId), cancellationToken);

        return result.Created
            ? TypedResults.Created($"/{RouteNames.EmployerProfile}/{result.Entity.AccountLegalEntityId}", result.Entity.ToPutResponse())
            : TypedResults.Ok(result.Entity.ToPutResponse());
    }

    [HttpPatch]
    [ProducesResponseType(typeof(PatchEmployerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IEmployerProfileRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromBody] JsonPatchDocument patchRequest,
        CancellationToken cancellationToken)
    {
        var employerProfile = await repository.GetOneAsync(accountLegalEntityId, cancellationToken);
        if (employerProfile is null)
        {
            return Results.NotFound();
        }

        var patchDocument = patchRequest.ToDomain<EmployerProfileEntity>();
        try
        {
            patchDocument.ThrowIfOperationsOn([nameof(EmployerProfileEntity.AccountId)]);
            patchDocument.ApplyTo(employerProfile);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }

        await repository.UpsertOneAsync(employerProfile, cancellationToken);
        return TypedResults.Ok(employerProfile.ToPatchResponse());
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteOne(
        [FromServices] IEmployerProfileRepository repository,
        [FromRoute] long accountLegalEntityId,
        CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteOneAsync(accountLegalEntityId, cancellationToken);

        return deleted
            ? Results.NoContent()
            : Results.NotFound();
    }
}
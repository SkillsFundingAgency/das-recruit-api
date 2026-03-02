using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.BlockedOrganisation;
using SFA.DAS.Recruit.Api.Models.Responses.BlockedOrganisation;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.BlockedOrganisation}/{{id:guid}}")]
public class BlockedOrganisationController : ControllerBase
{
    [HttpGet, Route($"~/{RouteNames.BlockedOrganisation}")]
    [ProducesResponseType(typeof(IEnumerable<BlockedOrganisation>), StatusCodes.Status200OK)]
    public async Task<IResult> GetMany(
        [FromServices] IBlockedOrganisationRepository repository,
        [FromQuery] string organisationType,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetByOrganisationTypeAsync(organisationType, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet, Route($"~/{RouteNames.BlockedOrganisation}/ByOrganisationId/{{organisationId}}")]
    [ProducesResponseType(typeof(BlockedOrganisation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetByOrganisationId(
        [FromServices] IBlockedOrganisationRepository repository,
        [FromRoute] string organisationId,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetLatestByOrganisationIdAsync(organisationId, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet]
    [ProducesResponseType(typeof(BlockedOrganisation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IBlockedOrganisationRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(id, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpPut]
    [ProducesResponseType(typeof(PutBlockedOrganisationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PutBlockedOrganisationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> PutOne(
        [FromServices] IBlockedOrganisationRepository repository,
        [FromRoute] Guid id,
        [FromBody] PutBlockedOrganisationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(id), cancellationToken);

        return result.Created
            ? TypedResults.Created($"/{RouteNames.BlockedOrganisation}/{result.Entity.Id}", result.Entity.ToPutResponse())
            : TypedResults.Ok(result.Entity.ToPutResponse());
    }

    [HttpPatch]
    [ProducesResponseType(typeof(PatchBlockedOrganisationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IBlockedOrganisationRepository repository,
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument patchRequest,
        CancellationToken cancellationToken)
    {
        var blockedOrganisation = await repository.GetOneAsync(id, cancellationToken);
        if (blockedOrganisation is null)
        {
            return Results.NotFound();
        }

        var patchDocument = patchRequest.ToDomain<BlockedOrganisationEntity>();
        try
        {
            patchDocument.ApplyTo(blockedOrganisation);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }

        await repository.UpsertOneAsync(blockedOrganisation, cancellationToken);
        return TypedResults.Ok(blockedOrganisation.ToPatchResponse());
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteOne(
        [FromServices] IBlockedOrganisationRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteOneAsync(id, cancellationToken);

        return deleted
            ? Results.NoContent()
            : Results.NotFound();
    }
}

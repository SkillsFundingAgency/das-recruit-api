using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.VacancyReviews}/{{id:guid}}")]
public class VacancyReviewController: ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(id, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpPut]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        [FromBody] PutVacancyReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(id), cancellationToken);

        return result.Created
            ? TypedResults.Created($"/{RouteNames.VacancyReviews}/{result.Entity.Id}", result.Entity.ToPutResponse())
            : TypedResults.Ok(result.Entity.ToPutResponse());
    }
    
    [HttpPatch]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument patchRequest,
        CancellationToken cancellationToken)
    {
        var employerProfile = await repository.GetOneAsync(id, cancellationToken);
        if (employerProfile is null)
        {
            return Results.NotFound();
        }

        var patchDocument = patchRequest.ToDomain<VacancyReviewEntity>();
        try
        {
            patchDocument.ThrowIfOperationsOn([
                nameof(VacancyReviewEntity.VacancyReference),
                nameof(VacancyReviewEntity.CreatedDate)
            ]);
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
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteOneAsync(id, cancellationToken);

        return deleted
            ? Results.NoContent()
            : Results.NotFound();
    }
    
    [HttpGet, Route($"~/{RouteNames.Vacancies}/{{vacancyReference}}/reviews")]
    [ProducesResponseType(typeof(List<VacancyReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetManyByVacancyReference(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] VacancyReference vacancyReference,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyByVacancyReference(vacancyReference, cancellationToken);

        return result is null or { Count: 0 }
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }
}
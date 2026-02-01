using System.Net;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [FromServices] IUserRepository userRepository,
        [FromRoute] Guid id,
        [FromBody] PutVacancyReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SubmittedByUserEmail))
        {
            var submittedUser =
                await userRepository.FindByUserIdAsync(request.SubmittedByUserId.ToString(), cancellationToken);
            request.SubmittedByUserEmail = submittedUser?.Email;
        }
        
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
        [FromBody] JsonPatchDocument<VacancyReview> patchRequest,
        CancellationToken cancellationToken)
    {
        var vacancyReview = await repository.GetOneAsync(id, cancellationToken);
        if (vacancyReview is null)
        {
            return Results.NotFound();
        }
        
        try
        {
            patchRequest.ThrowIfOperationsOn([
                nameof(VacancyReview.VacancyReference),
                nameof(VacancyReview.CreatedDate)
            ]);
            
            var patchDocument = patchRequest.ToDomain<VacancyReview, VacancyReviewEntity>();
            patchDocument.ApplyTo(vacancyReview);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }

        await repository.UpsertOneAsync(vacancyReview, cancellationToken);
        return TypedResults.Ok(vacancyReview.ToPatchResponse());
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

    [HttpGet, Route($"~/{RouteNames.VacancyReviews}/qa/dashboard")]
    [ProducesResponseType(typeof(QaDashboard), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetQaDashboard(
        [FromServices] IVacancyReviewRepository repository,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetQaDashboard(cancellationToken);

            return TypedResults.Ok(result);
        }
        catch
        {
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}